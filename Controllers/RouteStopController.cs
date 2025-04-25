using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;

namespace SDMNG.Controllers
{
    public class RouteStopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminMessage> _logger;

        public RouteStopController(AppDbContext context, IWebHostEnvironment env, ILogger<AdminMessage> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }
        


        public async Task<IActionResult> Index()
        {
            var routeStops = await _context.RouteStops
                .Include(rs => rs.Stop)
                .Include(rs => rs.TransportRoute)
                .OrderBy(rs => rs.SequenceNumber)
                .ToListAsync();
            return View(routeStops);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["StopId"] = new SelectList(_context.Stops, "StopId", "StopName");
            ViewData["TransportRouteId"] = new SelectList(_context.TransportRoutes, "TransportRoutesId", "TransportRoutesName");
            return View();
        }





        [HttpPost]
        public async Task<IActionResult> Create(RouteStop routeStop)
        {
            if (ModelState.IsValid)
            {
                routeStop.RouteStopId = Guid.NewGuid().ToString();
                _context.Add(routeStop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["StopId"] = new SelectList(_context.Stops, "StopId", "StopName", routeStop.StopId);
            ViewData["TransportRouteId"] = new SelectList(_context.TransportRoutes, "TransportRoutesId", "TransportRoutesName", routeStop.TransportRouteId);
            return View(routeStop);
        }




        public async Task<IActionResult> Delete(string id)
        {
            var routeStop = await _context.RouteStops
                .Include(rs => rs.Stop)
                .Include(rs => rs.TransportRoute)
                .FirstOrDefaultAsync(rs => rs.RouteStopId == id);

            return View(routeStop);
        }




        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var routeStop = await _context.RouteStops.FindAsync(id);
            _context.RouteStops.Remove(routeStop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
