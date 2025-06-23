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

        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Contact)
                .Include(t => t.Schedule)
                .ToListAsync();

            return View(tickets);
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



        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Contact)
                .Include(t => t.Schedule)
                    .ThenInclude(s => s.Bus) 
                .Include(t => t.Schedule)
                    .ThenInclude(s => s.TransportRoute)
                        .ThenInclude(r => r.RouteStop)
                            .ThenInclude(rs => rs.Stop)
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

            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == ticket.ScheduleId);

            if (schedule == null)
            {
                return NotFound("Schedule not found.");
            }

            if (schedule.TicketLeft <= 0)
            {
                TempData["ErrorMessage"] = "Nincs jegy erre a menetrendre!";
                return RedirectToAction("DetailUser", "Schedule", new { id = ticket.ScheduleId });
            }

            
            schedule.TicketLeft -= 1;

            
            if (string.IsNullOrEmpty(ticket.SeatNumber))
            {
                ticket.SeatNumber = GenerateSeatNumber();
            }

            _context.Tickets.Add(ticket);
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();

            // Email configuration
            var emailSettings = _config.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var port = int.Parse(emailSettings["Port"]);
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];

            // Build absolute URL for QR code that works on Azure
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var ticketUrl = $"{baseUrl}/Ticket/Detail/{ticket.TicketId}";

            var message = new MailMessage
            {
                From = new MailAddress(username),
                Subject = "Your Ticket Confirmation",
                Body = $"Thank you for your purchase! Visit our website to check your ticket:\n\n" +
                       $"Ticket ID: {ticket.TicketId}\n" +
                       $"Seat Number: {ticket.SeatNumber}\n" +
                       $"Date: {ticket.PurchaseDate:yyyy-MM-dd HH:mm}\n" +
                       $"Ticket details: {ticketUrl}\n\n" +
                       $"Thanks for your trust!",
                IsBodyHtml = false
            };

            message.To.Add(user.Email);

            // Generate QR code with clickable link to ticket details
            var qrBytes = GenerateQrCode(ticketUrl);
            var qrStream = new MemoryStream(qrBytes);
            var qrAttachment = new System.Net.Mail.Attachment(qrStream, "ticket_qr.png", "image/png");
            message.Attachments.Add(qrAttachment);

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
                _logger.LogInformation("Ticket confirmation email with QR code sent to user.");
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
            int seat = random.Next(1, 52); // 1 to 99
            return $"A{seat:D2}"; // e.g., S01, S12, S99
        }

        //Generating QR code
        private byte[] GenerateQrCode(string qrText)
        {
            using var qrGenerator = new QRCoder.QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(qrText, QRCoder.QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCoder.PngByteQRCode(qrData);
            return qrCode.GetGraphic(20); // 20 = pixel size
        }


        // GET: Tickets/Modify/5
        [HttpGet]
        public async Task<IActionResult> Modify(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Schedule)
                .Include(t => t.Contact)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
                return NotFound();

            // Populate Schedules dropdown
            ViewBag.Schedules = new SelectList(
                await _context.Schedules.ToListAsync(),
                "Id",               
                "Name",             
                ticket.ScheduleId   
            );

            // Populate Contacts dropdown
            ViewBag.Contacts = new SelectList(
                await _context.Contacts.ToListAsync(),
                "Id",               
                "FullName",         
                ticket.ContactId    
            );

            return View(ticket);
        }



        // POST: Tickets/Modify/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(string id, [Bind("TicketId,PurchaseDate,SeatNumber,ContactId,ScheduleId")] Ticket updatedTicket)
        {
            if (id != updatedTicket.TicketId)
                return NotFound();

            
               _context.Update(updatedTicket);
               await _context.SaveChangesAsync();
               return RedirectToAction(nameof(Index));
               

            // Repopulate dropdowns if the form is invalid
            ViewBag.Schedules = new SelectList(
                await _context.Schedules.ToListAsync(),
                "Id",
                "Name",
                updatedTicket.ScheduleId
            );

            ViewBag.Contacts = new SelectList(
                await _context.Contacts.ToListAsync(),
                "Id",
                "FullName",
                updatedTicket.ContactId
            );

            return View(updatedTicket);
        }






    }
}
