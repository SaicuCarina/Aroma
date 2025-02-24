using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using proj.Data; // Namespace-ul unde se află ApplicationDbContext
using proj.Models; // Namespace-ul pentru UserCustom și alte modele
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using proj.ViewModels;
using System.Data;

namespace proj.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<UserCustom> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ApplicationDbContext _context;

        public UsersController(
            ApplicationDbContext context,
            UserManager<UserCustom> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListUser()
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

            return View(userRolesViewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the user
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Populate the view model
            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                IsPostingRevoked = user.IsPostingRevoked // Retrieve from your user model
            };

            // Load roles
            var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();
            foreach (var role in roles)
            {
                model.Roles.Add(new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name,
                    Selected = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditUserRolesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Update user roles
            var userRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = model.SelectedRoles.Except(userRoles).ToList();
            var rolesToRemove = userRoles.Except(model.SelectedRoles).ToList();

            await _userManager.AddToRolesAsync(user, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            // Update the IsPostingRevoked flag
            user.IsPostingRevoked = model.IsPostingRevoked;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(ListUser));
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