using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;

namespace SDMNG.Controllers
{
    public class TransportRouteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminMessage> _logger;

        public TransportRouteController(AppDbContext context, IWebHostEnvironment env, ILogger<AdminMessage> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var routes = await _context.TransportRoutes
                .Include(r => r.RouteStop)
                .ToListAsync();
            return View(routes);
        }

        public async Task<IActionResult> IndexUser()
        {
            var routes = await _context.TransportRoutes
                .Include(r => r.RouteStop)
                .ToListAsync();
            return View(routes);
        }

        public async Task<IActionResult> Detail(string id)
        {
            if (id == null) return NotFound();

            var route = await _context.TransportRoutes
                .Include(r => r.RouteStop)
                    .ThenInclude(rs => rs.Stop)
                .FirstOrDefaultAsync(r => r.TransportRoutesId == id);

            if (route == null) return NotFound();

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

            return View(route);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransportRoute route)
        {
            if (ModelState.IsValid)
            {
                _context.Add(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(route);
        }

        public async Task<IActionResult> Modify(string id)
        {
            if (id == null) return NotFound();

            var route = await _context.TransportRoutes.FindAsync(id);
            if (route == null) return NotFound();

            return View(route);
        }

        [HttpPost]
        public async Task<IActionResult> Modify(string id, TransportRoute route)
        {
            if (id != route.TransportRoutesId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(route);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var route = await _context.TransportRoutes.FindAsync(id);
            return View(route);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var route = await _context.TransportRoutes.FindAsync(id);
            if (route != null)
            {
                _context.TransportRoutes.Remove(route);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
