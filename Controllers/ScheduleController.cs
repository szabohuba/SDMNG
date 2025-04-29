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
            ViewBag.TransportRoutes = await _context.TransportRoutes.ToListAsync();
            ViewBag.Buses = await _context.Buses.ToListAsync(); // If you want to assign a bus too

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Schedule schedule)
        {
              schedule.Id = Guid.NewGuid().ToString();
             _context.Schedules.Add(schedule);
             await _context.SaveChangesAsync();

             return RedirectToAction("Index", "Schedule"); // you can create an Index later
            
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
