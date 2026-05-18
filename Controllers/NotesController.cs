using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyNotes.Data;
using StudyNotes.Models;

namespace StudyNotes.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public NotesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string search)
        {
            var notes = _context.Notes
                .Include(n => n.Subject)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                notes = notes.Where(n =>
                    n.Title.Contains(search) ||
                    n.Content.Contains(search) ||
                    n.Subject!.Name.Contains(search));
            }

            ViewBag.Search = search;

            return View(await notes
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(
                _context.Subjects,
                "Id",
                "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Note note,
            IFormFile? uploadedFile)
        {
            if (ModelState.IsValid)
            {
                note.CreatedAt = DateTime.Now;

                if (uploadedFile != null)
                {
                    string uploadsFolder =
                        Path.Combine(_environment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName =
                        Guid.NewGuid().ToString() + "_" + uploadedFile.FileName;

                    string filePath =
                        Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    note.FilePath = "/uploads/" + uniqueFileName;
                    note.FileName = uploadedFile.FileName;
                }

                _context.Notes.Add(note);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubjectId = new SelectList(
                _context.Subjects,
                "Id",
                "Name",
                note.SubjectId);

            return View(note);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            ViewBag.SubjectId = new SelectList(
                _context.Subjects,
                "Id",
                "Name",
                note.SubjectId);

            return View(note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Note note,
            IFormFile? uploadedFile)
        {
            if (id != note.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingNote = await _context.Notes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (existingNote == null)
                {
                    return NotFound();
                }

                note.FileName = existingNote.FileName;
                note.FilePath = existingNote.FilePath;

                if (uploadedFile != null)
                {
                    string uploadsFolder =
                        Path.Combine(_environment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName =
                        Guid.NewGuid().ToString() + "_" + uploadedFile.FileName;

                    string filePath =
                        Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    note.FilePath = "/uploads/" + uniqueFileName;
                    note.FileName = uploadedFile.FileName;
                }

                _context.Update(note);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubjectId = new SelectList(
                _context.Subjects,
                "Id",
                "Name",
                note.SubjectId);

            return View(note);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var note = await _context.Notes
                .Include(n => n.Subject)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note != null)
            {
                if (!string.IsNullOrEmpty(note.FilePath))
                {
                    string fullPath = Path.Combine(
                        _environment.WebRootPath,
                        note.FilePath.TrimStart('/'));

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                _context.Notes.Remove(note);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}