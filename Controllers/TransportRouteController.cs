﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;

namespace SDMNG.Controllers
{
    public class TransportRouteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TransportRouteController> _logger;

        public TransportRouteController(AppDbContext context, ILogger<TransportRouteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        

        // GET: TransportRoute/Index
        public async Task<IActionResult> Index()
        {
            var routes = await _context.TransportRoutes
                .Include(r => r.RouteStop)
                .ThenInclude(rs => rs.Stop)
                .ToListAsync();

            return View(routes);
        }

        public async Task<IActionResult> IndexUser()
        {
            var routes = await _context.TransportRoutes
            .Include(r => r.RouteStop)
            .ThenInclude(rs => rs.Stop)
             .ToListAsync();

            return View(routes);
        }


        // GET: TransportRoute/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TransportRoute/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransportRoute route)
        {
           
                route.TransportRoutesId = Guid.NewGuid().ToString();
                _context.Add(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           
        }

        // GET: TransportRoute/Modify/5
        [HttpGet]
        public async Task<IActionResult> Modify(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var route = await _context.TransportRoutes.FindAsync(id);
            if (route == null)
                return NotFound();

            return View(route);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(string id, TransportRoute route)
        {
            if (id != route.TransportRoutesId)
                return NotFound();

            try
            {
                _context.Update(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Log error
                ModelState.AddModelError("", "Unable to save changes.");
                return View(route);
            }
        }




        // GET: TransportRoute/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var route = await _context.TransportRoutes
                .Include(r => r.RouteStop)  // Include RouteStops for deletion
                .FirstOrDefaultAsync(r => r.TransportRoutesId == id);

            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var route = await _context.TransportRoutes
                .Include(r => r.Schedule) 
                .FirstOrDefaultAsync(r => r.TransportRoutesId == id);

            if (route == null)
                return NotFound();

            if (route.Schedule != null)
            {
                TempData["ErrorMessage"] = "Az útvonal nem törölhető, mert egy menetrendhez van hozzárendelve. Először törölje a menetrendet.";
                return RedirectToAction("DetailUser", "Schedule", new { id = route.Schedule.Id });
            }

            var routeStops = _context.RouteStops.Where(rs => rs.TransportRouteId == route.TransportRoutesId);
            if (routeStops.Any())
                _context.RouteStops.RemoveRange(routeStops);

            _context.TransportRoutes.Remove(route);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: TransportRoute/Detail/5
        public async Task<IActionResult> Detail(string id)
        {
            if (id == null) return NotFound();

            var route = await _context.TransportRoutes
                .Include(r => r.RouteStop)
                    .ThenInclude(rs => rs.Stop)
                .FirstOrDefaultAsync(r => r.TransportRoutesId == id);

            if (route == null) return NotFound();

            // Send stops list to the view for dropdown
            ViewBag.Stops = await _context.Stops.ToListAsync();

            return View(route);
        }

        public async Task<IActionResult> DetailUser(string id)
        {
            if (id == null) return NotFound();

            var route = await _context.TransportRoutes
                .Include(r => r.RouteStop)
                    .ThenInclude(rs => rs.Stop)
                .FirstOrDefaultAsync(r => r.TransportRoutesId == id);

            if (route == null) return NotFound();

            // Add this to send stops list to the view
            ViewBag.Stops = await _context.Stops.ToListAsync();

            return View(route);
        }

        // POST: TransportRoute/AddStop
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStop(string TransportRouteId, string StopId)
        {
            if (string.IsNullOrEmpty(TransportRouteId) || string.IsNullOrEmpty(StopId))
            {
                TempData["ErrorMessage"] = "Missing TransportRouteId or StopId.";
                return RedirectToAction(nameof(Detail), new { id = TransportRouteId });
            }

            // Check if stop already exists in this route
            bool stopAlreadyExists = await _context.RouteStops
                .AnyAsync(rs => rs.TransportRouteId == TransportRouteId && rs.StopId == StopId);

            if (stopAlreadyExists)
            {
                TempData["ErrorMessage"] = "This stop is already added to this route.";
                return RedirectToAction(nameof(Detail), new { id = TransportRouteId });
            }

            // Get the highest SequenceNumber in this route
            int maxSequence = await _context.RouteStops
                .Where(rs => rs.TransportRouteId == TransportRouteId)
                .MaxAsync(rs => (int?)rs.SequenceNumber) ?? 0;

            var stop = await _context.Stops.FirstOrDefaultAsync(s => s.StopId == StopId);
            if (stop == null)
            {
                TempData["ErrorMessage"] = "Selected stop does not exist.";
                return RedirectToAction(nameof(Detail), new { id = TransportRouteId });
            }

            var newRouteStop = new RouteStop
            {
                RouteStopId = Guid.NewGuid().ToString(),
                TransportRouteId = TransportRouteId,
                StopId = StopId,
                RoutStopName = stop.StopName,
                SequenceNumber = maxSequence + 1 // <-- Automatically set the next sequence
            };

            _context.RouteStops.Add(newRouteStop);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Stop added successfully!";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"Database error: {ex.InnerException?.Message}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unexpected error: {ex.Message}";
            }

            return RedirectToAction(nameof(Detail), new { id = TransportRouteId });
        }

        // POST: TransportRoute/DeleteStop
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStop(string routeStopId, string transportRouteId)
        {
            var routeStop = await _context.RouteStops
                .FirstOrDefaultAsync(rs => rs.RouteStopId == routeStopId);

            if (routeStop != null)
            {
                _context.RouteStops.Remove(routeStop);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Detail), new { id = transportRouteId });
        }

        
    }
}
