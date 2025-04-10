using Microsoft.AspNetCore.Mvc;
using SDMNG.Data;
using System.Linq;

namespace SpeedDiesel.Controllers
{
    public class StopsController : Controller
    {
        private readonly AppDbContext _context;

        public StopsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var allStops = _context.Stops.ToList();
            return View(allStops);
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
