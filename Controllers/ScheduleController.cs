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
            
            var usedBusIds = await _context.Schedules
                                           .Where(s => s.BusId != null)
                                           .Select(s => s.BusId)
                                           .ToListAsync();

            
            var usedRouteIds = await _context.Schedules
                                             .Where(s => s.TransportRouteId != null)
                                             .Select(s => s.TransportRouteId)
                                             .ToListAsync();

            
            ViewBag.Buses = await _context.Buses
                                          .Where(b => !usedBusIds.Contains(b.BusId))
                                          .ToListAsync();

            
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

            
            var bus = await _context.Buses
                .FirstOrDefaultAsync(b => b.BusId == schedule.BusId);

            if (bus == null)
            {
                ModelState.AddModelError("BusId", "Invalid bus selected.");
                return View(schedule); 
            }

            
            schedule.TicketLeft = bus.Capacity;

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Schedule");
        }

        // GET: Schedule/Modify/{id}
        [HttpGet]
        public async Task<IActionResult> Modify(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
                return NotFound();

            return View(schedule);
        }

        // POST: Schedule/Modify/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(string id, [Bind("Id,Name,DepartureTime,ArrivalTime")] Schedule schedule)
        {
            if (id != schedule.Id)
                return NotFound();

            try
            {
                var existing = await _context.Schedules.FindAsync(id);
                if (existing == null)
                    return NotFound();

                
                existing.Name = schedule.Name;
                existing.DepartureTime = schedule.DepartureTime;
                existing.ArrivalTime = schedule.ArrivalTime;

                _context.Update(existing);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update schedule: " + ex.Message;
                return View(schedule);
            }
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


        // GET: Schedule/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var schedule = await _context.Schedules
                .Include(s => s.TransportRoute)
                .Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
                return NotFound();

            return View(schedule); 
        }


        //Post method for deleting schedule
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
                return NotFound();

            if (schedule.Tickets != null && schedule.Tickets.Any())
            {
                _context.Tickets.RemoveRange(schedule.Tickets);
            }

            // Optional: unlink foreign keys (only if nullable)
            schedule.TransportRouteId = null;
            schedule.BusId = null;

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
