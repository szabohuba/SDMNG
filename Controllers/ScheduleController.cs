using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
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

        public IActionResult Index()
        {
            var allSchedules = _context.Schedules.ToList();
            return View(allSchedules);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Modify()
        {

            return View();
        }

        public IActionResult Detail()
        {

            return View();
        }

        public IActionResult Delete()
        {

            return View();
        }
    }
}
