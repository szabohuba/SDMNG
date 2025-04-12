using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult UserStops()
        {
            var allStops = _context.Stops.ToList();
            return View(allStops);
        }

        public async Task<IActionResult> UserDetail(string id)
        {
            // Fetch the Stop object from the database using the provided id
            var stop = await _context.Stops
                .FirstOrDefaultAsync(s => s.StopId.ToString() == id); // Assuming StopId is an int

            // If no stop is found, return a NotFound result
            if (stop == null)
            {
                return NotFound();
            }

            // Pass the stop object to the view
            return View(stop);
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
