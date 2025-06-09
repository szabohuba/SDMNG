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

            // Only include contacts who are NOT already assigned
            var availableDrivers = _context.Contacts
                                           .Where(c => !assignedContactIds.Contains(c.Id))
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

            // Determine base upload path
            string basePath;
            if (_webHostEnvironment.IsDevelopment())
            {
                basePath = _webHostEnvironment.WebRootPath;
            }
            else
            {
                // Azure App Service uses this path
                basePath = Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "", "site", "wwwroot");
            }



            // Handle image upload
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

                    // Save relative URL path
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
        public async Task<IActionResult> Modify(Bus bus, IFormFile BusImage)
        {
            
            var existingBus = await _context.Buses.FindAsync(bus.BusId);

            if (existingBus == null)
            {
                return NotFound();
            }

            
            existingBus.BusNumber = bus.BusNumber;
            existingBus.Capacity = bus.Capacity;
            existingBus.BusType = bus.BusType;

            
            if (BusImage != null && BusImage.Length > 0)
            {
                string basePath;
                if (_webHostEnvironment.IsDevelopment())
                {
                    basePath = _webHostEnvironment.WebRootPath;
                }
                else
                {
                    basePath = Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "", "site", "wwwroot");
                }

                var uploadsFolder = Path.Combine(basePath, "images", "bus_images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(BusImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await BusImage.CopyToAsync(stream);
                }

                
                existingBus.ImageUrl = $"/images/bus_images/{uniqueFileName}";
            }

            _context.Update(existingBus);
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

            // Get driver name from Contacts table
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
                    TempData["ErrorMessage"] = "This bus is linked to a schedule. Please delete the schedule before deleting the bus.";
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

            // Unassign driver (if desired and ContactId is nullable)
            bus.ContactId = null;
            _context.Buses.Update(bus); // Needed to save null assignment

            // Delete attachments from file system
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
