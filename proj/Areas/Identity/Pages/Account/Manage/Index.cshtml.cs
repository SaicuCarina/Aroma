using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using proj.Data; // Namespace-ul unde se află ApplicationDbContext
using proj.Models; // Namespace-ul pentru UserCustom și RecipeModel
using System.Linq;
using System.Threading.Tasks;

namespace proj.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<UserCustom> _userManager;
        private readonly SignInManager<UserCustom> _signInManager;
        private readonly ApplicationDbContext _context; // Adăugăm contextul de bază de date

        public IndexModel(
            UserManager<UserCustom> userManager,
            SignInManager<UserCustom> signInManager,
            ApplicationDbContext context) // Injectăm contextul în constructor
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }
        public string StatusMessage { get; set; }
        public List<RecipeModel> UserRecipes { get; set; } // Lista cu rețetele utilizatorului

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Username = user.UserName;

            // Extrage rețetele utilizatorului curent din baza de date
            UserRecipes = _context.Recipes // presupunând că tabelul se numește `Recipes`
                            .Where(r => r.UserName == user.UserName) // Filtrare după utilizator
                            .OrderByDescending(r => r.Date) // Sortare descrescătoare după dată
                            .ToList();

            return Page();
        }
    }
}
