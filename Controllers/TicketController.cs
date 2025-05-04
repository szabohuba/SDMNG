using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace SpeedDiesel.Controllers
{
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Contact> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AdminMessage> _logger;

        public TicketController(AppDbContext context, UserManager<Contact> userManager, IConfiguration config, ILogger<AdminMessage> logger)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _logger = logger;
          
        }

        public IActionResult Index()
        {
            var allTicket = _context.Tickets.ToList();
            return View(allTicket);
        }

        public async Task<IActionResult> UserTickets()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var tickets = await _context.Tickets
                .Include(t => t.Contact)
                .Include(t => t.Schedule)
                .Where(t => t.ContactId == user.Id) // show only this user's tickets
                .ToListAsync();

            return View(tickets);
        }


        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Modify()
        {
            return View();
        }

        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Contact)
                .Include(t => t.Schedule)
                    .ThenInclude(s => s.TransportRoute) // optional: if you want to show route name
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        public IActionResult Delete()
        {
            return View();
        }




        [Authorize]
        public async Task<IActionResult> BuyTicket()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge(); // user not logged in, force login

            var schedules = _context.Schedules.ToList();
            ViewBag.ScheduleList = new SelectList(schedules, "Id", "Name");

            var SeatNumberRand = GenerateSeatNumber();

            var ticket = new Ticket
            {
                ContactId = user.Id,
                Contact = user,
                PurchaseDate = DateTime.Now,
                SeatNumber=SeatNumberRand
            };

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> BuyTicket(Ticket ticket)
        {
            var user = await _userManager.GetUserAsync(User);
            ticket.ContactId = user.Id;
            ticket.PurchaseDate = DateTime.Now;

            if (string.IsNullOrEmpty(ticket.SeatNumber))
            {
                ticket.SeatNumber = GenerateSeatNumber();
            }

            
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                // EMAIL SENDING TO USER
                var emailSettings = _config.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var port = int.Parse(emailSettings["Port"]);
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];

                var message = new MailMessage
                {
                    From = new MailAddress(username),
                    Subject = "Your Ticket Confirmation",
                    Body = $"Thank you for your purchase! Visit our website, login and check for your ticket information! \n\n" +
                           $"Ticket ID: {ticket.TicketId}\n" +
                           $"Seat Number: {ticket.SeatNumber}\n" +
                           $"Date: {ticket.PurchaseDate:yyyy-MM-dd HH:mm}\n" +
                           $"Thanks for your trust!",
                    IsBodyHtml = false
                };

                message.To.Add(user.Email);

                using var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                try
                {
                    await smtpClient.SendMailAsync(message);
                    _logger.LogInformation("Ticket confirmation email sent to user.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send ticket confirmation email: {ex.Message}");
                }

                return RedirectToAction("UserTickets");
            
        }


        //Generating seat number
        private string GenerateSeatNumber()
        {
            var random = new Random();
            int seat = random.Next(1, 100); // 1 to 99
            return $"S{seat:D2}"; // e.g., S01, S12, S99
        }


    }
}
