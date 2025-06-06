using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;
using System.Linq;

namespace SpeedDiesel.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly AppDbContext _context;
        public ScheduleController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var schedules = await _context.Schedules
                .Include(s => s.TransportRoute)
                .Include(s => s.Bus)
                .ToListAsync();

            return View(schedules);
        }

        public async Task<IActionResult> IndexUser()
        {
            var schedules = await _context.Schedules
                .Include(s => s.TransportRoute)
                .Include(s => s.Bus)
                .ToListAsync();

            return View(schedules);
        }


        public IActionResult UserSchedule()
        {
            var allSchedules = _context.Schedules.ToList();
            return View(allSchedules);
        }

        public async Task<IActionResult> Create()
        {
            // Get IDs of buses already used in any schedule
            var usedBusIds = await _context.Schedules
                                           .Where(s => s.BusId != null)
                                           .Select(s => s.BusId)
                                           .ToListAsync();

            // Get IDs of transport routes already used in any schedule
            var usedRouteIds = await _context.Schedules
                                             .Where(s => s.TransportRouteId != null)
                                             .Select(s => s.TransportRouteId)
                                             .ToListAsync();

            // Only buses NOT in usedBusIds
            ViewBag.Buses = await _context.Buses
                                          .Where(b => !usedBusIds.Contains(b.BusId))
                                          .ToListAsync();

            // Only routes NOT in usedRouteIds
            ViewBag.TransportRoutes = await _context.TransportRoutes
                                                    .Where(r => !usedRouteIds.Contains(r.TransportRoutesId))
                                                    .ToListAsync();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Schedule schedule)
        {
            schedule.Id = Guid.NewGuid().ToString();

            // Fetch the selected bus to get its capacity
            var bus = await _context.Buses
                .FirstOrDefaultAsync(b => b.BusId == schedule.BusId);

            if (bus == null)
            {
                ModelState.AddModelError("BusId", "Invalid bus selected.");
                return View(schedule); // Return view with validation message
            }

            // Set TicketLeft to the capacity of the bus
            schedule.TicketLeft = bus.Capacity;

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Schedule");
        }


        public IActionResult Modify()
        {

            return View();
        }

        public async Task<IActionResult> Detail(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.TransportRoute)  
                .Include(s => s.Bus)             
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        public async Task<IActionResult> DetailUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.TransportRoute)
                .Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }


        public IActionResult Delete()
        {

            return View();
        }
    }
}
