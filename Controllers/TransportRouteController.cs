using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SDMNG.Data;
using System.Linq;

namespace SpeedDiesel.Controllers
{
    public class TransportRoutesController : Controller
    {
        private readonly AppDbContext _context;

        public TransportRoutesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var allRoutes = _context.TransportRoutes.ToList();
            return View(allRoutes);
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
