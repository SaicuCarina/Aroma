using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj.Data;
using proj.Models;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<UserCustom> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<UserCustom> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> AdminHome()
    {
        return RedirectToAction("RecipeCountsByMonth", "Recipe");
    }
    public async Task<IActionResult> Index(string sortOrder, int currentPage = 1, int pageSize = 9)
    {
        //aici
        
        //return RedirectToAction("DetermineWinner", "Recipe");
        // Assume DetermineWinner() marks the winner and updates the user data
        var winner = await _context.Users.FirstOrDefaultAsync(u => u.isWinner);
            if (winner != null)
            {
                ViewBag.IsWinner = true;  // Winner is marked
            }
            else
            {
                ViewBag.IsWinner = false;
            }

          
        


        // Get the currently logged-in user
        var user = await _userManager.GetUserAsync(User);

        // Check if the user is a winner
        ViewBag.IsWinner = user?.isWinner ?? false;

        // Advertisements (unchanged)
        var oddAd = _context.Advertisements
            .Where(ad => ad.Id % 2 != 0)
            .OrderByDescending(ad => ad.Id)
            .FirstOrDefault();

        var evenAd = _context.Advertisements
            .Where(ad => ad.Id % 2 == 0)
            .OrderByDescending(ad => ad.Id)
            .FirstOrDefault();

        ViewData["OddAd"] = oddAd;
        ViewData["EvenAd"] = evenAd;

        // Base query for recipes
        var query = _context.Recipes.AsQueryable();

        if (!(User.IsInRole("User") || User.IsInRole("Admin")))
        {
            // Dacă utilizatorul nu este "User" sau "Admin" și rețeta este din concurs, nu o afișăm
            query = query.Where(r => !r.IsEligibleForVoting);
        }

        // Exclude contest recipes if needed
        // query = query.Where(r => !r.IsContest);

        // Apply sorting
        // Apply sorting based on sortOrder
        switch (sortOrder)
        {
            case "a-z":
                query = query.OrderBy(r => r.Title);
                break;
            case "z-a":
                query = query.OrderByDescending(r => r.Title);
                break;
            case "chronological":
                query = query.OrderBy(r => r.Date);
                break;
            case "reverse-chronological":
                query = query.OrderByDescending(r => r.Date);
                break;
            default:
                query = query.OrderByDescending(r => r.Date);
                break;
        }

        // Count total recipes for pagination
        int totalRecipes = query.Count();

        // Pagination
        var recipes = query
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        int totalPages = (int)Math.Ceiling(totalRecipes / (double)pageSize);

        // Pass pagination data to the view
        ViewBag.CurrentPage = currentPage;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;
        ViewBag.SortOrder = sortOrder;

        return View(recipes);
    }
}
