using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj.Data;
using proj.Models;



namespace proj.Controllers
{
    public class AdvertisementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdvertisementController> _logger;

        public AdvertisementController(ApplicationDbContext context, ILogger<AdvertisementController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdvertisementModel advertisement)
        {
            try
            {
                // Verifică dacă fișierul foto este furnizat
                if (advertisement.Photo == null || string.IsNullOrEmpty(advertisement.Photo.FileName))
                {
                    advertisement.PhotoPath = "default.jpg"; // Imagine implicită dacă nu este încărcată nicio poză
                }
                else
                {
                    advertisement.PhotoPath = advertisement.Photo.FileName;

                    // Calea către directorul de upload
                    var basePath = "C:\\Users\\Corina\\source\\repos\\Aroma\\proj\\wwwroot\\imag\\"; // Modifică după caz
                    var fullPath = Path.Combine(basePath, advertisement.PhotoPath);

                    // Salvează fișierul în sistemul local
                    using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                    {
                        await advertisement.Photo.CopyToAsync(fileStream);
                    }
                }

                // Verifică dacă link-ul este valid
                if (!string.IsNullOrWhiteSpace(advertisement.Link))
                {
                    if (!Uri.TryCreate(advertisement.Link, UriKind.Absolute, out Uri? uriResult) ||
                        (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                    {
                        TempData["ErrorMessage"] = "Invalid URL format. Please provide a valid link.";
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    advertisement.Link = "https://default.link"; // Link implicit dacă nu este furnizat
                }

                // Adaugă reclama în baza de date
                _context.Advertisements.Add(advertisement);
                await _context.SaveChangesAsync();

                // Mesaj de succes
                TempData["SuccessMessage"] = "The advertisement has been added successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Loghează eroarea
                _logger.LogError(ex, "Error saving advertisement to the database");

                // Mesaj de eroare pentru utilizator
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
            }

            // În cazul unei erori, returnează pagina cu datele curente
            return View(advertisement);
        }



    }
}