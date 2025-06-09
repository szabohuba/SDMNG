using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;
using System.Linq;

namespace SpeedDiesel.Controllers
{
    public class StopsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AdminMessage> _logger;

        public StopsController(AppDbContext context, IConfiguration config, ILogger<AdminMessage> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var allStops = _context.Stops.ToList();
            return View(allStops);
        }

        public IActionResult UserStops()
        {
            var allStops = _context.Stops.ToList();
            return View(allStops);
        }

        public async Task<IActionResult> UserDetail(string id)
        {
           
            var stop = await _context.Stops
                .FirstOrDefaultAsync(s => s.StopId.ToString() == id); 

            if (stop == null)
            {
                return NotFound();
            }

            return View(stop);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Stop stop)
        {
              stop.StopId = Guid.NewGuid().ToString(); // Important: set it manually
             _context.Stops.Add(stop);
             await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        
        }

        [HttpGet]
        public async Task<IActionResult> Modify(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var stop = await _context.Stops
                .FirstOrDefaultAsync(s => s.StopId == id);

            if (stop == null) return NotFound();

            return View(stop);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(string id, [Bind("StopId,StopName,Latitude,Longitude")] Stop stop)
        {
            if (id != stop.StopId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Stops.Any(s => s.StopId == stop.StopId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(stop);
        }






        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var stop = await _context.Stops
                .FirstOrDefaultAsync(s => s.StopId == id);

            if (stop == null) return NotFound();

            return View(stop);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var stop = await _context.Stops
                .FirstOrDefaultAsync(s => s.StopId == id);

            if (stop == null) return NotFound();

            return View(stop);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var stop = await _context.Stops.FindAsync(id);
            if (stop != null)
            {
                _context.Stops.Remove(stop);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
