using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SDMNG.Data;
using System.Linq;

namespace SpeedDiesel.Controllers
{
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;
        public TicketController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var allTicket = _context.Tickets.ToList();
            return View(allTicket);
        }


        public IActionResult UserTickets()
        {
            var allTicket = _context.Tickets.ToList();
            return View(allTicket);
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
