using Microsoft.AspNetCore.Mvc;
using StudyNotes.Data;

namespace StudyNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.SubjectsCount = _context.Subjects.Count();
            ViewBag.NotesCount = _context.Notes.Count();
            ViewBag.ImportantNotesCount = _context.Notes.Count(n => n.IsImportant);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}