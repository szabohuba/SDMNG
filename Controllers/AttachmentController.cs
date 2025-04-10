using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMNG.Data;
using SDMNG.Models;
using System.Threading.Tasks;
using System.Linq;

namespace SDMNG.Controllers
{
    public class AttachmentController : Controller
    {
        private readonly AppDbContext _context;

        public AttachmentController(AppDbContext context)
        {
            _context = context;
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
    }
}
