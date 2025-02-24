using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using proj.Data;

[Authorize(Roles = "Admin")]
public class AdminHomeController : Controller
{
    private readonly ApplicationDbContext _context;
    public AdminHomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Initialize the arrays to store the count of recipes and contest recipes for each month
        int[] userCounts = new int[12];

        // Define the target year (2025)
        int targetYear = 2025;

        // Query for overall recipe counts by month in 2025
        var userCountsQuery = _context.UsersCustom
            .Where(r => r.RegistrationDate.Year == targetYear)  // Filter by year 2025
            .GroupBy(r => r.RegistrationDate.Month)
            .Select(group => new
            {
                Month = group.Key,
                Count = group.Count()
            })
            .ToList();

        foreach (var result in userCountsQuery)
        {
            userCounts[result.Month - 1] = result.Count;
        }

        // Pass both counts to the view
        ViewBag.UserCounts = userCounts;

        return View();
       
    }


}
