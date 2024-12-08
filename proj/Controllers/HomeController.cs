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
        // Obține rețetele în ordine descrescătoare după data publicării
        var recipes = _context.Recipes
            .OrderByDescending(r => r.Date)
            .ToList();

        return View(recipes);
    }

}
