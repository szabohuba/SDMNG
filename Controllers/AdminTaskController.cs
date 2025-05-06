using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;

namespace SDMNG.Controllers
{
    public class AdminTaskController : Controller
    {
        
            private readonly AppDbContext _context;
            private readonly IConfiguration _config;
            private readonly ILogger<AdminTask> _logger;

            public AdminTaskController(AppDbContext context, IConfiguration config, ILogger<AdminTask> logger)
            {
                _context = context;
                _config = config;
                _logger = logger;
            }

        public IActionResult Index()
        {
            var tasks = _context.AdminTasks.OrderBy(t => t.IsResolved).ThenBy(t => t.DueUntil).ToList();
            return View(tasks);
        }

        public IActionResult Detail(string id)
        {
            var task = _context.AdminTasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();

            return View(task);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AdminTask task)
        {
            
                task.Id = Guid.NewGuid().ToString(); // Ensure string ID
                task.CreatedAt = DateTime.UtcNow;
                _context.AdminTasks.Add(task);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            
        }

        public IActionResult Modify(string id)
        {
            var task = _context.AdminTasks.Find(id);
            if (task == null) return NotFound();

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modify(string id, AdminTask updatedTask)
        {
            if (id != updatedTask.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Entry(updatedTask).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(updatedTask);
        }


    }
}
