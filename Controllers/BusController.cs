using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SpeedDiesel.Controllers
{
    public class BusController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminMessage> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BusController(AppDbContext context, ILogger<AdminMessage> logger, IWebHostEnvironment webHostEnvironment )
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment= webHostEnvironment;
        }

        public IActionResult Index()
        {
            var allBuses = _context.Buses.ToList();
            return View(allBuses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Get IDs of contacts who are already assigned to a bus
            var assignedContactIds = _context.Buses
                                             .Select(b => b.ContactId)
                                             .ToList();

            var availableDrivers = _context.Contacts
                               .Where(c => c.Active && !assignedContactIds.Contains(c.Id))
                               .Select(c => new SelectListItem
                               {
                                   Value = c.Id,
                                   Text = c.FullName
                               })
                               .ToList();

            ViewBag.Drivers = availableDrivers;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bus bus, IFormFile BusImage)
        {
            

            if (BusImage == null || BusImage.Length == 0)
            {
                TempData["ErrorMessage"] = "Please add a bus image!";
                return RedirectToAction(nameof(Create));
            }

            bus.BusId = Guid.NewGuid().ToString();

           
            string basePath;
            if (_webHostEnvironment.IsDevelopment())
            {
                basePath = _webHostEnvironment.WebRootPath;
            }
            else
            {
                
                basePath = Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "", "site", "wwwroot");
            }



            
            if (BusImage != null && BusImage.Length > 0)
            {
                try
                {
                    var uploadsFolder = Path.Combine(basePath, "images", "bus_images");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(BusImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await BusImage.CopyToAsync(stream);
                    }

                    
                    bus.ImageUrl = $"/images/bus_images/{uniqueFileName}";
                }
                catch (Exception fileEx)
                {
                    _logger.LogError("Image upload failed: " + fileEx.Message);
                    ModelState.AddModelError("ImageUrl", "Image upload failed.");
                }
            }
           

                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }

        [HttpGet]
        public IActionResult Modify(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = _context.Buses
                              .Include(b => b.Contact)
                              .FirstOrDefault(b => b.BusId == id);

            if (bus == null)
            {
                return NotFound();
            }

            ViewBag.DriverName = bus.Contact?.FullName ?? "No driver assigned";
            return View(bus);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(Contact contact)
        {
            var existingContact = await _context.Contacts.FindAsync(contact.Id);

            if (existingContact == null)
            {
                return NotFound();
            }

            
            if (!contact.Active)
            {
                bool hasBus = _context.Buses.Any(b => b.ContactId == contact.Id);
                if (hasBus)
                {
                    ModelState.AddModelError("Active", "Ez a sofőr jelenleg egy buszhoz van rendelve, ezért nem tehető inaktívvá.");
                    return View(contact);
                }
            }

            // Mezők frissítése
            existingContact.FullName = contact.FullName;
            existingContact.Street = contact.Street;
            existingContact.Zipcode = contact.Zipcode;
            existingContact.Active = contact.Active;
           
            _context.Update(existingContact);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }







        // GET: Bus/Detail/{id}
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var bus = await _context.Buses
                .Include(b => b.Attachments)
                .FirstOrDefaultAsync(b => b.BusId == id);

            if (bus == null)
                return NotFound();

           
            var driver = await _context.Contacts
                .Where(c => c.Id == bus.ContactId)
                .Select(c => c.FullName)
                .FirstOrDefaultAsync();

            ViewBag.DriverName = driver ?? "Unassigned";

            return View(bus);
        }



        // GET: Bus/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var bus = await _context.Buses
                .Include(b => b.Contact)
                .Include(b => b.Schedules)
                .Include(b => b.Attachments)
                .FirstOrDefaultAsync(b => b.BusId == id);

            if (bus == null)
                return NotFound();

            // Bus is scheduled-> Go to schedule detail view
            if (bus.Schedules != null && bus.Schedules.Any())
            {
                var firstSchedule = bus.Schedules.FirstOrDefault();
                if (firstSchedule != null)
                {
                    TempData["ErrorMessage"] = "Eza a busz egy menetrendhez tartozik, elobb azt torolje.";
                    return RedirectToAction("Detail", "Schedule", new { id = firstSchedule.Id });
                }
            }


            return View(bus); 
        }




        // POST: Bus/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bus = await _context.Buses
                .Include(b => b.Attachments)
                .FirstOrDefaultAsync(b => b.BusId == id);

            if (bus == null)
                return NotFound();

            
            bus.ContactId = null;
            _context.Buses.Update(bus); 

            
            if (bus.Attachments != null && bus.Attachments.Any())
            {
                foreach (var attachment in bus.Attachments)
                {
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, attachment.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                _context.Attachments.RemoveRange(bus.Attachments);
            }

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
