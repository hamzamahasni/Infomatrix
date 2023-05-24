using Infomatrix_Datos.Datos;
using Infomatrix_Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Infomatrix_Utilidades;
using Infomatrix_Datos.Datos.Repositorio.IRepositorio;

namespace Infomatrix.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoriaController : Controller
    {

        private readonly ICategoriaRepositorio catRepo;

        public CategoriaController(ICategoriaRepositorio catRepo)
        {
            this.catRepo = catRepo;
        }
        public IActionResult Index()
        {
            IEnumerable<Categoria> lista = catRepo.ObtenerTodos();
            return View(lista);
        }
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                catRepo.Agregar(categoria);
                catRepo.grabar();
                TempData[WC.Exitosa] = "Categoria creada Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            TempData[WC.Error] = "Error al crear Categoria";
            return View(categoria);
        }
        public IActionResult Editar(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = catRepo.Obtener(id.GetValueOrDefault()); //para asegurar que no sea nulo
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
                catRepo.Actualizar(categoria);
                catRepo.grabar();
                TempData[WC.Exitosa] = "Categoria editada Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            TempData[WC.Error] = "Error al crear la Categoria";
            return View(categoria);
        }
        public IActionResult Eliminar(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = catRepo.Obtener(id.GetValueOrDefault());
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
                TempData[WC.Error] = "Error al intentar elimninar la Categoria";
                return NotFound();
            }
            catRepo.Remover(categoria);
            catRepo.grabar();
            TempData[WC.Exitosa] = "Categoria eliminada Exitosamente";
            return RedirectToAction(nameof(Index));


        }
    }
}
