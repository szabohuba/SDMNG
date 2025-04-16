using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;
using System.Net.Mail;
using System.Net;

namespace SpeedDiesel.Controllers
{
    public class AdminMessageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AdminMessage> _logger;

        public AdminMessageController (AppDbContext context, IConfiguration config, ILogger<AdminMessage> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }


        public IActionResult Index()
        {
            var allMessages = _context.AdminMessages.ToList();
            return View(allMessages);
        }
        


        public async Task<IActionResult> Detail(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminMessage = await _context.AdminMessages
                .FirstOrDefaultAsync(m => m.adminmassegesId == id);

            if (adminMessage == null)
            {
                return NotFound();
            }

            // Set IsRead to true
            adminMessage.IsRead = true;
            await _context.SaveChangesAsync();

            return View(adminMessage);
        }


        // GET: SendMessage
        public IActionResult ContactMe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(AdminMessage model)
        {
            if (ModelState.IsValid)
            {
                // Generate new ID and set properties
                model.adminmassegesId = Guid.NewGuid().ToString();
                model.SentAt = DateTime.UtcNow;
                model.IsRead = false;

                // Save to database
                _context.AdminMessages.Add(model);
                await _context.SaveChangesAsync();

                // Get email settings from configuration
                var emailSettings = _config.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var port = int.Parse(emailSettings["Port"]);
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];
                var adminEmail = emailSettings["AdminEmail"];

                // Send email
                var message = new MailMessage
                {
                    From = new MailAddress(username),
                    To = { adminEmail },
                    Subject = $"New Message: {model.Subject}",
                    Body = $"Name: {model.userName}\nEmail: {model.userEmail}\n\nMessage:\n{model.Message}"
                };

                using var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtpClient.SendMailAsync(message);

                return RedirectToAction("MessageSent");
            }

            return View(model);
        }

    }
}
