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
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(AdminMessage model)
        {
            
            // Log each property value for debugging
            _logger.LogInformation("Model received:");
            _logger.LogInformation($"adminmassegesId: {model?.adminmassegesId}");
            _logger.LogInformation($"userName: {model?.userName}");
            _logger.LogInformation($"userEmail: {model?.userEmail}");
            _logger.LogInformation($"Subject: {model?.Subject}");
            _logger.LogInformation($"Message: {model?.Message}");
            _logger.LogInformation($"IsRead: {model?.IsRead}");
            _logger.LogInformation($"SentAt: {model?.SentAt}");

            
                // Assign generated values
                model.adminmassegesId = Guid.NewGuid().ToString();
                model.SentAt = DateTime.UtcNow;
                model.IsRead = false;

                _context.AdminMessages.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved message to DB successfully");

                // Email
                var emailSettings = _config.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var port = int.Parse(emailSettings["Port"]);
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];
                var adminEmail = emailSettings["AdminEmail"];

                var message = new MailMessage
                {
                    From = new MailAddress(username),
                    Subject = $"New Message: {model.Subject}",
                    Body = $"Name: {model.userName}\nEmail: {model.userEmail}\n\nMessage:\n{model.Message}"
                };
                message.To.Add(adminEmail);

                using var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtpClient.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully");

                return RedirectToAction("MessageSent");
            
        }



        public IActionResult MessageSent()
        {
            return View();
        }

    }
}
