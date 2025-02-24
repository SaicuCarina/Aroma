using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using proj.Data;
using proj.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proj.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class FavouriteRecipesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserCustom> _userManager;

        public FavouriteRecipesModel(ApplicationDbContext context, UserManager<UserCustom> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<RecipeModel> FavouriteRecipes { get; set; } = new List<RecipeModel>();

        public async Task OnGetAsync()
        {
            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Unable to load your user data.";
                return;
            }

            // Retrieve favourite recipes for the user, sorted by date descending
            FavouriteRecipes = await _context.FavouriteRecipes
                .Where(f => f.UserName == user.UserName)
                .Select(f => f.Recipe)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }
    }
}