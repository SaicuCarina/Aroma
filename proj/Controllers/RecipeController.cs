using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using proj.Data;
using proj.Models;
using proj.ViewModels;
using System;
using System.Data;

namespace proj.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RecipeController> _logger;
        private readonly UserManager<UserCustom> _userManager;

        public RecipeController(ApplicationDbContext context, ILogger<RecipeController> logger, UserManager<UserCustom> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var allCategories = _context.Categories.ToList();
            ViewData["Categories"] = new SelectList(allCategories, "Id", "CategoryName");

            return View();
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Photo,Video,CategoryId,Time,Difficulty,IsContest")] RecipeModel recipe)
        {
            try
            {
                // Retrieve the current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "You must be logged in to post a recipe.";
                    return RedirectToAction("Index", "Home");
                }

                // Check if posting is revoked
                if (user.IsPostingRevoked)
                {
                    TempData["ErrorMessage"] = "Your posting rights have been revoked. You cannot add a recipe.";
                    return RedirectToAction("Index", "Home");
                }

                recipe.Date = DateTime.Now;
                recipe.UserName = user.UserName;

                // If recipe is for the contest, associate it with the current contest
                if (recipe.IsContest)
                {
                    var currentMonth = DateTime.Now.ToString("MMMM");
                    var currentYear = DateTime.Now.Year.ToString();

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

                // Handle file uploads
                var picture = recipe.Photo;
                recipe.PhotoPath = picture.FileName;
                recipe.VideoPath = recipe.Video.FileName;
                

                var basePath = "C:\Users\Corina\source\repos\AromaAfterEma\AromaCurrent\AromaCurrent\Aroma\proj\wwwroot\imag\\";
                var fullPathPhoto = Path.Combine(basePath, recipe.PhotoPath);
                var fullPathVideo = Path.Combine(basePath, recipe.VideoPath);

                using (var fileStreamPhoto = new FileStream(fullPathPhoto, FileMode.Create, FileAccess.Write))
                {
                    recipe.Photo.CopyTo(fileStreamPhoto);
                }

                using (var fileStreamVideo = new FileStream(fullPathVideo, FileMode.Create, FileAccess.Write))
                {
                    recipe.Video.CopyTo(fileStreamVideo);
                }

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
*/
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Title,Content,Photo,Video,CategoryId,Time,Difficulty,IsContest")] RecipeModel recipe)
{
    try
    {
        // Retrieve the current user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["ErrorMessage"] = "You must be logged in to post a recipe.";
            return RedirectToAction("Index", "Home");
        }

        // Check if posting is revoked
        if (user.IsPostingRevoked)
        {
            TempData["ErrorMessage"] = "Your posting rights have been revoked. You cannot add a recipe.";
            return RedirectToAction("Index", "Home");
        }

        recipe.Date = DateTime.Now;
        recipe.UserName = user.UserName;

        // If recipe is for the contest, associate it with the current contest
        if (recipe.IsContest)
        {
            var currentMonth = DateTime.Now.ToString("MMMM");
            var currentYear = DateTime.Now.Year.ToString();

            var currentContest = _context.Contests
                .FirstOrDefault(c => c.Month == currentMonth && c.Year == currentYear);

            if (currentContest != null)
            {
                recipe.ContestId = currentContest.Id;
                recipe.IsEligibleForVoting = true;  // Set to true because the recipe is part of the current contest
            }
            else
            {
                TempData["ErrorMessage"] = "No contest available for the current month!";
                return RedirectToAction("Create");
            }
        }

        // Handle file uploads
        var picture = recipe.Photo;
        recipe.PhotoPath = picture.FileName;
        recipe.VideoPath = recipe.Video.FileName;

        var basePath = "C:\\Users\\Corina\\source\\repos\\AromaAfterEma\\AromaCurrent\\AromaCurrent\\Aroma\\proj\\wwwroot\\imag\\";
        var fullPathPhoto = Path.Combine(basePath, recipe.PhotoPath);
        var fullPathVideo = Path.Combine(basePath, recipe.VideoPath);

        using (var fileStreamPhoto = new FileStream(fullPathPhoto, FileMode.Create, FileAccess.Write))
        {
            recipe.Photo.CopyTo(fileStreamPhoto);
        }

        using (var fileStreamVideo = new FileStream(fullPathVideo, FileMode.Create, FileAccess.Write))
        {
            recipe.Video.CopyTo(fileStreamVideo);
        }

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


        public void ResetEligibleForVoting()
        {
            var currentMonth = DateTime.Now.Month;

            // Get all recipes that are part of contests
            var recipesInContests = _context.Recipes.Where(r => r.IsContest).ToList();

            foreach (var recipe in recipesInContests)
            {
                // If the month the recipe was added is not the current month, reset IsEligibleForVoting to false
                if (recipe.Date.Month != currentMonth)
                {
                    recipe.IsEligibleForVoting = false;
                }
            }

            // Save changes to the database
            _context.SaveChanges();
        }





        [HttpGet]
        public IActionResult Details(int id)
        {
            // Fetch the recipe from the database by ID
            var recipe = _context.Recipes
                .Include(r => r.Category)  // Include category
                .Include(r => r.Comments)  // Include comments
                .Include(r => r.Ratings)   // Include ratings
                .FirstOrDefault(r => r.Id == id);

            // Return 404 if recipe not found
            if (recipe == null)
            {
                return NotFound();
            }

            // Calculate average rating
            ViewBag.AverageRating = recipe.Ratings.Any() ? recipe.Ratings.Average(r => r.StarNumber) : 0;

            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                // Determine if this recipe is already in the user's favorites
                var isFavorite = _context.FavouriteRecipes
                    .Any(f => f.UserName == userName && f.RecipeId == id);

                ViewBag.IsFavorite = isFavorite;
            }
            else
            {
                // If not logged in, consider it as not favorite
                ViewBag.IsFavorite = false;
            }

            return View(recipe);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, string commentContent, int? rating, int? vote)
        {
            // Make sure the user is logged in
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            // Fetch the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to leave a comment or rate.";
                return RedirectToAction("Details", new { id });
            }

            // Check if posting is revoked
            if (user.IsPostingRevoked)
            {
                TempData["ErrorMessage"] = "Your posting rights have been revoked. Please contact an administrator.";
                return RedirectToAction("Details", new { id });
            }

            try
            {
                // Save the comment if provided
                if (!string.IsNullOrWhiteSpace(commentContent))
                {
                    var comment = new CommentModel
                    {
                        RecipeId = id,
                        Content = commentContent,
                        UserName = user.UserName,
                        Date = DateTime.Now
                    };
                    _context.Comments.Add(comment);
                }

                // Save the rating if provided
                if (rating.HasValue)
                {
                    var existingRating = _context.Ratings
                        .FirstOrDefault(r => r.RecipeId == id && r.UserName == user.UserName);

                    if (existingRating != null)
                    {
                        existingRating.StarNumber = rating.Value;
                    }
                    else
                    {
                        var newRating = new RatingModel
                        {
                            RecipeId = id,
                            UserName = user.UserName,
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

            // Reload recipe details
            return RedirectToAction("Details", new { id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int id)
        {
            // Ensure the user is logged in
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            // Fetch the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to vote.";
                return RedirectToAction("Details", new { id });
            }

            // Check if posting is revoked
            if (user.IsPostingRevoked)
            {
                TempData["ErrorMessage"] = "Your posting rights have been revoked. Please contact an administrator.";
                return RedirectToAction("Details", new { id });
            }

            try
            {
                // Check if the recipe exists
                var recipe = await _context.Recipes.FindAsync(id);
                if (recipe == null)
                {
                    TempData["ErrorMessage"] = "Recipe not found.";
                    return RedirectToAction("Index");
                }

                // Check if the user has already voted
                var existingVote = await _context.UserRecipeVotes
                    .Where(v => v.UserName == user.UserName)
                    .FirstOrDefaultAsync();

                if (existingVote != null)
                {
                    // Decrement the vote count for the previously voted recipe
                    var previousRecipe = await _context.Recipes.FindAsync(existingVote.RecipeId);
                    if (previousRecipe != null)
                    {
                        previousRecipe.NumberOfVotes--;
                    }

                    // Update the existing vote to the new recipe
                    existingVote.RecipeId = id;
                }
                else
                {
                    // Insert a new vote directly into the UserRecipeVotes table
                    await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO UserRecipeVotes (UserName, RecipeId) VALUES ({0}, {1})",
                        user.UserName, id);
                }

                // Increment the vote count for the new recipe
                recipe.NumberOfVotes++;

                // Save changes to the database
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your vote has been recorded.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error voting");
                TempData["ErrorMessage"] = "An error occurred. Please try again later.";
            }

            // Redirect back to the recipe details page
            return RedirectToAction("Details", new { id });
        }




        [HttpGet]
        public async Task<IActionResult> DetermineWinner()
        {
            // Check if today is the 25th day of the month
            var today = DateTime.UtcNow;
            if (today.Day != 25)
            {
                return Ok();
            }

            var contestStartDate = new DateTime(today.Year, today.Month, 25);
            var contestEndDate = contestStartDate.AddMonths(1); // Start of the next month

            // Get the recipe(s) with the highest number of votes in the contest period
            var winners = await _context.Recipes
                .Where(r => r.Date >= contestStartDate && r.Date < contestEndDate)
                .OrderByDescending(r => r.NumberOfVotes)
                .Take(1) // Take the recipe with the most votes
                .ToListAsync();

            if (!winners.Any())
            {
                return Ok();
            }

            // Update the isWinner field for the users associated with the winning recipe(s)
            foreach (var winner in winners)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == winner.UserName); // Match user by UserName
                if (user != null)
                {
                    user.isWinner = true; // Set isWinner to true
                    _context.Users.Update(user);
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Schedule the reset of the isWinner field after 24 hours
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromHours(24)); // Wait for 24 hours
                foreach (var winner in winners)
                {
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.UserName == winner.UserName);
                    if (user != null)
                    {
                        user.isWinner = false; // Reset isWinner to false
                        _context.Users.Update(user);
                    }
                }
                await _context.SaveChangesAsync();
            });

            return Ok();
        }





        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new List<RecipeModel>());
            }
            var hacAccess = User.IsInRole("User") || User.IsInRole("Admin");
            query = query.ToLower();
            if (hacAccess)
            {
                var recipes = await _context.Recipes
                                                .Where(a => a.Title.ToLower().Contains(query) || a.Content.ToLower().Contains(query) || a.Category.CategoryName.ToLower().Contains(query))
                                                .ToListAsync();
                return View(recipes);
            }
            else
            {
                var recipes = await _context.Recipes
                                                .Where(r => !r.IsEligibleForVoting)
                                                .Where(a => a.Title.ToLower().Contains(query) || a.Content.ToLower().Contains(query))
                                                .ToListAsync();
                return View(recipes);
            }
            
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavourite(int recipeId)
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                TempData["ErrorMessage"] = "You must be logged in to mark favorites.";
                return RedirectToAction("Details", new { id = recipeId });
            }

            var existingFavourite = await _context.FavouriteRecipes
                .FirstOrDefaultAsync(f => f.UserName == userName && f.RecipeId == recipeId);

            if (existingFavourite != null)
            {
                // If recipe is already favorite, remove it
                _context.FavouriteRecipes.Remove(existingFavourite);
                TempData["Message"] = "Recipe removed from favorites.";
            }
            else
            {
                // If not favorite, add it
                var newFavourite = new FavouriteRecipe
                {
                    UserName = userName,
                    RecipeId = recipeId
                };

                _context.FavouriteRecipes.Add(newFavourite);
                TempData["Message"] = "Recipe added to favorites.";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = recipeId });
        }

        [HttpGet]
        public async Task<IActionResult> Favourite()
        {
            var userName = User.Identity.Name;

            var favouriteRecipes = await _context.FavouriteRecipes
                .Where(f => f.UserName == userName)
                .Select(f => f.Recipe)
                .ToListAsync();

            return View(favouriteRecipes);
        }




        public IActionResult RecipeCountsByMonth()
        {
            // Initialize arrays for recipe counts, contest counts, and user registrations
            int[] recipeCounts = new int[12];
            int[] contestCounts = new int[12];
            int[] usersCount = new int[12];

            // Define the target year (2025)
            int targetYear = 2025;

            // Query for overall recipe counts by month in 2025
            var recipeCountsQuery = _context.Recipes
                .Where(r => r.Date.Year == targetYear)
                .GroupBy(r => r.Date.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    Count = group.Count()
                })
                .ToList();

            foreach (var result in recipeCountsQuery)
            {
                recipeCounts[result.Month - 1] = result.Count;
            }

            // Query for contest recipe counts by month in 2025
            var contestCountsQuery = _context.Recipes
                .Where(r => r.IsContest && r.Date.Year == targetYear)
                .GroupBy(r => r.Date.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    Count = group.Count()
                })
                .ToList();

            foreach (var result in contestCountsQuery)
            {
                contestCounts[result.Month - 1] = result.Count;
            }

            // Query for user registrations by month in 2025
            var userRegistrationsQuery = _context.Users
                .Where(u => u.RegistrationDate.Year == targetYear)
                .GroupBy(u => u.RegistrationDate.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    Count = group.Count()
                })
                .ToList();

            foreach (var registration in userRegistrationsQuery)
            {
                usersCount[registration.Month - 1] = registration.Count;
            }

            // Pass all counts to the view
            ViewBag.RecipeCounts = recipeCounts;
            ViewBag.ContestCounts = contestCounts;
            ViewBag.UsersCount = usersCount;

            return View();
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountUser()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    User = user,
                    Roles = roles
                });
            }

            // Datele pentru graficul cu utilizatorii noi
            int currentYear = DateTime.Now.Year;
            int[] monthlyRegistrations = new int[12];

            var userRegistrations = await _context.Users
                .Where(u => u.RegistrationDate.Year == currentYear)
                .GroupBy(u => u.RegistrationDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            foreach (var registration in userRegistrations)
            {
                monthlyRegistrations[registration.Month - 1] = registration.Count;
            }

            // Trimite datele către View
            ViewBag.UsersCount = monthlyRegistrations;

            return View(userRolesViewModel);
        }

    }
}