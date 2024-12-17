using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using proj.Data;
using proj.Models;
using System;
using System.Data;

namespace proj.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(ApplicationDbContext context, ILogger<RecipeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var allCategories = _context.Categories.ToList();
            ViewData["Categories"] = new SelectList(allCategories, "Id", "CategoryName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Photo,Video,CategoryId,Time,Difficulty,IsContest")] RecipeModel recipe)
        {
            try
            {
                recipe.Date = DateTime.Now;
                recipe.UserName = User.Identity.Name;

                // If recipe is for the contest, associate it with the current contest
                if (recipe.IsContest)
                {
                    var currentMonth = DateTime.Now.ToString("MMMM");
                    var currentYear = DateTime.Now.Year.ToString();

                    // Find the current contest
                    var currentContest = _context.Contests
                        .FirstOrDefault(c => c.Month == currentMonth && c.Year == currentYear);

                    if (currentContest != null)
                    {
                        recipe.ContestId = currentContest.Id;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No contest available for the current month!";
                        return RedirectToAction("Create");
                    }
                }

                var picture = recipe.Photo;

                recipe.PhotoPath = picture.FileName;
                recipe.VideoPath = recipe.Video.FileName;

                var basePath = "C:\\Users\\Corina\\source\\repos\\Aroma\\proj\\wwwroot\\imag\\"; //de schimbat fiecare
                var fullPathPhoto = Path.Combine(basePath, recipe.PhotoPath);
                var fullPathVideo = Path.Combine(basePath, recipe.VideoPath);
                var fileStreamPhoto = new FileStream(fullPathPhoto, FileMode.Create, FileAccess.Write);
                var fileStreamVideo = new FileStream(fullPathVideo, FileMode.Create, FileAccess.Write);

                recipe.Photo.CopyTo(fileStreamPhoto);
                recipe.Video.CopyTo(fileStreamVideo);

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "The recipe has been added successfully!";

                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving recipe to the database");
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
            }

            var allCategories = _context.Categories.ToList();
            ViewData["Categories"] = new SelectList(allCategories, "Id", "CategoryName", recipe.CategoryId);

            return View(recipe);
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            // Căutăm rețeta în baza de date după ID
            var recipe = _context.Recipes
                .Include(r => r.Category)  // Include informațiile despre categorie
                .Include(r => r.Comments) // Include comentariile
                .Include(r => r.Ratings)  // Include rating-urile
                .FirstOrDefault(r => r.Id == id);

            // Dacă nu găsim rețeta, returnăm 404
            if (recipe == null)
            {
                return NotFound();
            }

            // Pregătim datele pentru view
            ViewBag.AverageRating = recipe.Ratings.Any() ? recipe.Ratings.Average(r => r.StarNumber) : 0;
            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, string commentContent, int? rating)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Salvează comentariul în baza de date, dacă există
                if (!string.IsNullOrWhiteSpace(commentContent))
                {
                    var comment = new CommentModel
                    {
                        RecipeId = id,
                        Content = commentContent,
                        UserName = User.Identity.Name,
                        Date = DateTime.Now
                    };
                    _context.Comments.Add(comment);
                }

                // Salvează rating-ul în baza de date, dacă există
                if (rating.HasValue)
                {
                    var existingRating = _context.Ratings.FirstOrDefault(r => r.RecipeId == id && r.UserName == User.Identity.Name);
                    if (existingRating != null)
                    {
                        existingRating.StarNumber = rating.Value;
                    }
                    else
                    {
                        var newRating = new RatingModel
                        {
                            RecipeId = id,
                            UserName = User.Identity.Name,
                            StarNumber = rating.Value
                        };
                        _context.Ratings.Add(newRating);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment or rating");
                ModelState.AddModelError("", "An error occurred. Please try again later.");
            }

            // Reîncarcă detaliile rețetei
            return RedirectToAction("Details", new { id });
        }



    }
}