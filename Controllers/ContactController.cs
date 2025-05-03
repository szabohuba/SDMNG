using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;

namespace SDMNG.Controllers
{
    public class ContactController : Controller
    {

        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
           _context = context;
        }

        public IActionResult Index()
        {
            var allContacts = _context.Contacts.ToList();
            return View(allContacts);
        }

        public IActionResult Create()
        {
            ViewBag.BusList = new SelectList(_context.Buses, "BusId", "BusNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BusList = new SelectList(_context.Buses, "BusId", "BusNumber", contact.Bus?.BusId);
            return View(contact);
        }

      
        public IActionResult Modify()
        {
         
            return View();
        }

        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }


        public IActionResult Delete()
        {

            return View();
        }


        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
