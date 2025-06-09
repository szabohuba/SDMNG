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
            var activeContacts = _context.Contacts
                .Include(c => c.Bus) 
                .Where(c => c.Active)
                .ToList();

            return View(activeContacts);
        }


        public IActionResult Allcontacts()
        {
            var allContacts = _context.Contacts
                .Include(c => c.Bus)
                .ToList();

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




        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

           
            var tickets = _context.Tickets.Where(t => t.ContactId == user.Id).ToList();
            if (tickets.Any())
            {
                _context.Tickets.RemoveRange(tickets);
            }


            
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction("Index"); 
            }

            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Delete", user); // Return to delete view with error messages
        }



        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Contacts.FindAsync(userId);

            if (user == null) return NotFound();

            return View(user); 
        }

       
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
                return View(user);
            }

            // Only update role if a new one is explicitly selected
            if (!string.IsNullOrWhiteSpace(selectedRole))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                if (await _roleManager.RoleExistsAsync(selectedRole))
                {
                    await _userManager.AddToRoleAsync(user, selectedRole);
                }
            }

            return RedirectToAction("Index");
        }


    }
}
