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

        public ContestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Month,Year,Theme")]*/ ContestModel contest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contest);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The contest has been successfully added!";
                return RedirectToAction("Index", "Home");
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

            return View(contest);
        }
    }
}
