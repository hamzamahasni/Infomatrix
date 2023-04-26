using Infomatrix.Datos;
using Infomatrix.Models;
using Microsoft.AspNetCore.Mvc;

namespace Infomatrix.Controllers
{
    public class CategoriaController : Controller
    {

        private readonly ApplicationDbContext db;
        
        public CategoriaController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Categoria> lista = db.Categoria;
            return View(lista);
        }
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//Antifalsificaciones sitio seguro
        public IActionResult Crear(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                db.Categoria.Add(categoria);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }
        public IActionResult Editar(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = db.Categoria.Find(id); 
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                db.Categoria.Update(categoria);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }
        public IActionResult Eliminar(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = db.Categoria.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Categoria categoria)
        {
            if (categoria == null)
            {
                return NotFound();
            }
            db.Categoria.Remove(categoria);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}
