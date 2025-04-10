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

        public IActionResult Detail()
        {

            return View();
        }

        public IActionResult Delete()
        {

            return View();
        }

    }
}
