using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

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
                                      Value = c.Id, // Assuming ContactId is the key
                                      Text = c.FullName // Adjust this depending on the Contact model
                                  })
                                  .ToList();

            // Pass the list to the view via ViewBag
            ViewBag.Drivers = drivers;

            return View();
        }

        // POST: Bus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bus bus, IFormFile BusImage)
        {
            if (ModelState.IsValid)
            {
                // Manually set the BusId
                bus.BusId = Guid.NewGuid().ToString(); // Important: set it manually

                // Log the input values before proceeding with the database operations
                _logger.LogInformation("Creating Bus:");
                _logger.LogInformation($"BusId: {bus.BusId}");
                _logger.LogInformation($"BusNumber: {bus.BusNumber}");
                _logger.LogInformation($"Capacity: {bus.Capacity}");
                _logger.LogInformation($"BusType: {bus.BusType}");
                _logger.LogInformation($"ImageUrl: {bus.ImageUrl}");
                _logger.LogInformation($"ContactId: {bus.ContactId}");

                // Handle the bus image upload
                if (BusImage != null)
                {
                    var uploadFolder = Path.Combine(@"C:\Users\Szabo Huba\Desktop\SDMNG\wwwroot\images", "bus_images");
                    var fileName = Path.GetFileName(BusImage.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await BusImage.CopyToAsync(fileStream);
                    }

                    // Save the image URL to the Bus instance
                    bus.ImageUrl = Path.Combine("images", "bus_images", fileName);
                }

                // Add the Bus object to the database
                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();

                // Log successful creation of the bus
                _logger.LogInformation($"Bus with BusId {bus.BusId} successfully created.");

                // Redirect to the Detail action (or another appropriate action)
                return RedirectToAction(nameof(Detail), new { id = bus.BusId });
            }

            // If the model is invalid, repopulate the drivers for the dropdown
            ViewBag.Drivers = new SelectList(_context.Contacts, "ContactId", "FullName", bus.ContactId);
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
            if (string.IsNullOrEmpty(id)) return NotFound();

            var bus = await _context.Buses
                .FirstOrDefaultAsync(b => b.BusId == id);

            if (bus == null) return NotFound();

            return View(bus);
        }

        // GET: Bus/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var bus = await _context.Buses
                .FirstOrDefaultAsync(b => b.BusId == id);

            if (bus == null) return NotFound();

            return View(bus);
        }


        // POST: Bus/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bus = await _context.Buses.FindAsync(id);

            if (bus != null)
            {
                _context.Buses.Remove(bus);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


       

       

    }
}
