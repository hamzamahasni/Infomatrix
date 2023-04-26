using Infomatrix.Datos;
using Infomatrix.Models;
using Microsoft.AspNetCore.Mvc;

namespace Infomatrix.Controllers
{
    public class MarcaController : Controller
    {

        private readonly ApplicationDbContext db;
        
        public MarcaController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Marca> lista = db.Marca;
            return View(lista);
        }
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//Antifalsificaciones sitio seguro
        public IActionResult Crear(Marca marca)
        {
            if (ModelState.IsValid)
            {
                db.Marca.Add(marca);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(marca);
        }
    }
}
