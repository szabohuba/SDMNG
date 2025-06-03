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
            // Fetch the list of contacts (drivers) from your database
            var drivers = _context.Contacts
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.Id, 
                                      Text = c.FullName 
                                  })
                                  .ToList();

            ViewBag.Drivers = drivers;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bus bus, IFormFile BusImage)
        {
            try
            {
                // Handle file upload
                if (BusImage != null && BusImage.Length > 0)
                {
                    try
                    {
                        // Construct path using the wwwroot path
                        var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "bus_images");

                        // Ensure directory exists
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        // Create unique filename
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BusImage.FileName);
                        var filePath = Path.Combine(uploadFolder, fileName);

                        // Save file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await BusImage.CopyToAsync(fileStream);
                        }

                        // Store relative path for frontend use
                        bus.ImageUrl = $"/images/bus_images/{fileName}";
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogError($"File upload error: {fileEx.Message}");
                        bus.ImageUrl = null; // fallback
                    }
                }

                // Save to database
                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating bus: {ex.Message}");
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            // Repopulate dropdown in case of error
            ViewBag.Drivers = _context.Contacts
                                      .Select(c => new SelectListItem
                                      {
                                          Value = c.Id,
                                          Text = c.FullName,
                                          Selected = c.Id == bus.ContactId
                                      })
                                      .ToList();

            return View(bus);
        }



        // GET: Bus/Modify/{id}
        public async Task<IActionResult> Modify(string id)
            {
                if (string.IsNullOrEmpty(id)) return NotFound();

                var bus = await _context.Buses
                    .FirstOrDefaultAsync(b => b.BusId == id);

                if (bus == null) return NotFound();

                return View(bus);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Modify(string id, [Bind("BusId,BusNumber,Capacity,BusType,ImageUrl,ContactId")] Bus bus)
            {   

                if (id != bus.BusId) return NotFound();

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(bus);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.Buses.Any(e => e.BusId == bus.BusId)) return NotFound();
                        throw;
                    }

                    return RedirectToAction(nameof(Index));
                }

                return View(bus);
            }




        // GET: Bus/Detail/{id}
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var bus = await _context.Buses.FindAsync(id);
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


        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
                return NotFound();

            var driverName = await _context.Contacts
                .Where(c => c.Id == bus.ContactId)
                .Select(c => c.FullName)
                .FirstOrDefaultAsync();

            ViewBag.DriverName = driverName ?? "Unassigned";

            return View(bus);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bus = await _context.Buses
                .Include(b => b.Schedules) // Assuming navigation property
                .FirstOrDefaultAsync(b => b.BusId == id);

            if (bus == null)
                return NotFound();

            if (bus.Schedules != null && bus.Schedules.Any())
            {
                ModelState.AddModelError("", "Cannot delete this bus because it has associated schedules.");
                var driverName = await _context.Contacts
                    .Where(c => c.Id == bus.ContactId)
                    .Select(c => c.FullName)
                    .FirstOrDefaultAsync();
                ViewBag.DriverName = driverName ?? "Unassigned";
                return View(bus);
            }

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
