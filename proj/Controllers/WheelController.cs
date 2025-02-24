using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using proj.Data; // Asigură-te că folosești contextul corect al bazei de date
using proj.Models;
using Microsoft.EntityFrameworkCore; // Importă modelele relevante

namespace proj.Controllers
{
    public class WheelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WheelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Afișează pagina cu roata
        public IActionResult LuckyWheel()
        {
            return View();
        }

        // Verifică dacă utilizatorul a roti roata astăzi
        [HttpGet]
        public async Task<IActionResult> HasSpunToday(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            DateTime today = DateTime.UtcNow.Date;

            // Verificăm dacă LastSpinDate este null sau setat la data default
            if (user.LastSpinDate == DateTime.MinValue || user.LastSpinDate.Date != today)
            {
                user.hasSpunWheel = false;
                user.LastSpinDate = today; // Actualizăm cu data curentă doar când e necesar
                await _context.SaveChangesAsync();
            }

            return Json(new { hasSpun = user.hasSpunWheel });
        }




        // Actualizează starea 'hasSpunWheel' după ce utilizatorul a roti roata
        public async Task<IActionResult> UpdateSpinStatus(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Salvează data ultimei rotiri
            user.LastSpinDate = DateTime.UtcNow;
            user.hasSpunWheel = true;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // Resetarea câmpului 'hasSpunWheel' la finalul zilei (poate fi un task programat sau un endpoint care se apelează manual)
        public async Task<IActionResult> ResetSpinStatus()
        {
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                user.hasSpunWheel = false;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}