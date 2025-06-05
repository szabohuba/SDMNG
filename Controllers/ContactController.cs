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
        private readonly UserManager<Contact> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContactController(UserManager<Contact> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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



        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            
            var contact = await _context.Contacts
                .Include(c => c.Attachments)
                .Include(c => c.Bus) 
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null) return NotFound();

          
            var roles = await _userManager.GetRolesAsync(contact);
            ViewBag.UserRole = roles.FirstOrDefault() ?? "No role assigned";

            
            ViewBag.BusNumber = contact.Bus?.BusNumber ?? "N/A";

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

        // GET: Contacts/Modify/{id}
        public async Task<IActionResult> Modify(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            ViewBag.Roles = new SelectList(allRoles);
            ViewBag.CurrentRole = userRoles.FirstOrDefault();

            return View(user);
        }

        // POST: Contacts/Modify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(Contact model, string selectedRole)
        {

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Street = model.Street;
            user.Zipcode = model.Zipcode;
            user.Active = model.Active;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user");
                return View(model);
            }

            
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            if (!string.IsNullOrEmpty(selectedRole) && await _roleManager.RoleExistsAsync(selectedRole))
            {
                await _userManager.AddToRoleAsync(user, selectedRole);
            }

            return RedirectToAction("Index");
        }

    }
}
