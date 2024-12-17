using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj.Data;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
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
  
        var recipes = _context.Recipes
            .Where(r => !r.IsContest) // Exclude contest recipes
            .OrderByDescending(r => r.Date)
            .ToList();

        return View(recipes);
    }

}
