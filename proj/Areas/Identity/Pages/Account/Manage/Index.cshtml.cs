using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using proj.Data; // Namespace-ul unde se află ApplicationDbContext
using proj.Models; // Namespace-ul pentru UserCustom, RecipeModel, și AdvertisementModel
using System.Linq;
using System.Threading.Tasks;

namespace proj.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<UserCustom> _userManager;
        private readonly SignInManager<UserCustom> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<UserCustom> userManager,
            SignInManager<UserCustom> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }
        public string StatusMessage { get; set; }
        public List<RecipeModel> UserRecipes { get; set; }
        public List<AdvertisementModel> Advertisements { get; set; } // Lista cu reclamele adminului
        public bool IsAdmin { get; set; } // Determină dacă utilizatorul este admin

        public DateTime RegistrationDate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Username = user.UserName;
            RegistrationDate = user.RegistrationDate; // <-- Store the user's registration date

            // Determine user role
            var roles = await _userManager.GetRolesAsync(user);
            IsAdmin = roles.Contains("Admin");

            if (IsAdmin)
            {
                Advertisements = _context.Advertisements
                                         .OrderByDescending(a => a.Id)
                                         .ToList();
            }
            else
            {
                UserRecipes = _context.Recipes
                                       .Where(r => r.UserName == user.UserName)
                                       .OrderByDescending(r => r.Date)
                                       .ToList();
            }

            return Page();
        }
    }
}
