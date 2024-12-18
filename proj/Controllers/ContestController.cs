using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using proj.Data;
using proj.Models;
using System.Data;

namespace proj.Controllers
{
    
    public class ContestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RecipeController> _logger;

        public ContestController(ApplicationDbContext context, ILogger<RecipeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContestModel contest)
        {
            try
            {
                if (contest.Photo == null || string.IsNullOrEmpty(contest.Photo.FileName))
                {
                    contest.PhotoPath = "WinTefalPan.jpg"; // Setăm o imagine implicită
                }
                else
                {
                    var picture = contest.Photo;

                    contest.PhotoPath = picture.FileName;

                    var basePath = "C:\\Users\\Corina\\source\\repos\\Aroma\\proj\\wwwroot\\imag\\"; //de schimbat fiecare
                    var fullPathPhoto = Path.Combine(basePath, contest.PhotoPath);
                    var fileStreamPhoto = new FileStream(fullPathPhoto, FileMode.Create, FileAccess.Write);

                    contest.Photo.CopyTo(fileStreamPhoto);

                }

                // Adaugă concursul în baza de date
                _context.Add(contest);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "The contest has been successfully added!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving contest to the database");
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
            }

            return View(contest);
        }

        [HttpGet]
        public IActionResult CurrentContest()
        {
            // Get the current month and year
            var currentMonth = DateTime.Now.ToString("MMMM"); // Full month name (e.g. "December")
            var currentYear = DateTime.Now.Year.ToString();

            // Fetch the contest that matches the current month and year
            var contest = _context.Contests
                .FirstOrDefault(c => c.Month == currentMonth && c.Year == currentYear);

            // If no contest exists for the current month, show a message
            if (contest == null)
            {
                TempData["ErrorMessage"] = "No contest for this month!";
                return RedirectToAction("Index", "Home");
            }

            // Fetch recipes associated with this contest
            var contestRecipes = _context.Recipes
                .Where(r => r.ContestId == contest.Id)
                .OrderByDescending(r => r.Date)
                .ToList();

            ViewBag.ContestRecipes = contestRecipes;

            return View(contest);
        }
    }
}
