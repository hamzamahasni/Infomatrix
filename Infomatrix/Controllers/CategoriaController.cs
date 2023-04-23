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
            return View();
        }
    }
}
