using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SDMNG.Controllers
{
    public class AttachmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminMessage> _logger;

        public AttachmentController(AppDbContext context, IWebHostEnvironment env, ILogger<AdminMessage> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var attachments = _context.Attachments
                .Include(a => a.Contact)
                .Include(a => a.Bus)
                .ToList();

            return View(attachments);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Modify(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await _context.Attachments
                .Include(a => a.Contact)
                .Include(a => a.Bus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (attachment == null)
            {
                return NotFound();
            }

            return View(attachment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(string id, [Bind("Id,FileName,FilePath,FileType,UploadDate,ContactId,BusId")] Attachment attachment)
        {
            if (id != attachment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attachment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Attachments.Any(e => e.Id == attachment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(attachment);
        }

        public async Task<IActionResult> Detail(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await _context.Attachments
                .Include(a => a.Contact)
                .Include(a => a.Bus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (attachment == null)
            {
                return NotFound();
            }

            return View(attachment);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await _context.Attachments
                .Include(a => a.Contact)
                .Include(a => a.Bus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (attachment == null)
            {
                return NotFound();
            }

            return View(attachment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment != null)
            {
                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        /// ADD BUS ATTACHMENT

        [HttpGet]
        public async Task<IActionResult> CreateBusAttachment(string busId)
        {
            var bus = await _context.Buses.FindAsync(busId);
            if (bus == null) return NotFound();

            var model = new Attachment { BusId = busId };
            ViewBag.BusNumber = bus.BusNumber;

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBusAttachment(Attachment attachment, IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        // Determine the upload path based on environment (local or Azure)
                        string basePath = _env.IsDevelopment()
                            ? Path.Combine(_env.WebRootPath, "attachments")
                            : Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "", "site", "wwwroot", "attachments");

                        // Ensure the folder exists
                        if (!Directory.Exists(basePath))
                        {
                            Directory.CreateDirectory(basePath);
                        }

                        // Generate a unique file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(basePath, fileName);

                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // Populate the attachment object
                        attachment.FileName = fileName;
                        attachment.FilePath = "/attachments/" + fileName;
                        attachment.FileType = file.ContentType;
                        attachment.UploadDate = DateTime.Now;
                        attachment.expirationDate = attachment.UploadDate.AddYears(1);
                        attachment.ContactId = null; // Since this is a bus attachment

                        // Save to database
                        _context.Attachments.Add(attachment);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Detail", "Bus", new { id = attachment.BusId });
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogError($"File upload error: {fileEx.Message}");
                        ModelState.AddModelError("", "Error uploading the file. Please try again.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Please select a file to upload.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding attachment: {ex.Message}");
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            // On error, return to view with the original model to show validation errors
            return View(attachment);
        }




        /// ADD Contact ATTACHMENT

        [HttpGet]
        public async Task<IActionResult> CreateContactAttachment(string contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null) return NotFound();

            var model = new Attachment { ContactId = contactId };
            ViewBag.ContactName = contact.FullName; // Assuming Contact has a 'FullName' property, adjust as needed.

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContactAttachment(Attachment attachment, IFormFile file)
        {
            attachment.Id = Guid.NewGuid().ToString();
            try
            {
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        // Determine upload path (local or Azure)
                        string uploadFolder = _env.IsDevelopment()
                            ? Path.Combine(_env.WebRootPath, "attachments")
                            : Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "", "site", "wwwroot", "attachments");

                        // Ensure directory exists
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        // Generate unique filename
                        var fileName = attachment.Id + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadFolder, fileName);

                        // Save file to disk
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // Save metadata to database
                        attachment.FileName = fileName;
                        attachment.FilePath = $"/attachments/{fileName}"; // relative URL
                        attachment.FileType = file.ContentType;
                        attachment.UploadDate = DateTime.Now;
                        attachment.expirationDate = attachment.UploadDate.AddYears(1);

                        attachment.BusId = null; // it's a contact attachment

                        _context.Attachments.Add(attachment);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Detail", "Contact", new { id = attachment.ContactId });
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogError($"File upload error: {fileEx.Message}");
                        ModelState.AddModelError("", "Error uploading the file. Please try again.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Please select a file to upload.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding attachment: {ex.Message}");
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            return View(attachment);
        }




    }
}
