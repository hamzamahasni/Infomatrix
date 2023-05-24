using Infomatrix_Datos.Datos;
using Infomatrix_Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infomatrix_Utilidades;
using Infomatrix_Datos.Datos.Repositorio.IRepositorio;

namespace Infomatrix.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class MarcaController : Controller
    {

        private readonly IMarcaRepositorio marcaRepo;

        public MarcaController(IMarcaRepositorio marcaRepo)
        {
            this.marcaRepo = marcaRepo;
        }
        public IActionResult Index()
        {
            IEnumerable<Marca> lista = marcaRepo.ObtenerTodos();
            return View(lista);
        }
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Marca marca)
        {
            if (ModelState.IsValid)
            {
                marcaRepo.Agregar(marca);
                marcaRepo.grabar();
                TempData[WC.Exitosa] = "Tipo Aplicacion creado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            TempData[WC.Error] = "Error al crear Tipo Aplicacion";
            return View(marca);
        }
        public IActionResult Editar(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = marcaRepo.Obtener(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Marca marca)
        {
            if (ModelState.IsValid)
            {
                marcaRepo.Actualizar(marca);
                marcaRepo.grabar();
                TempData[WC.Exitosa] = "Tipo Aplicacion editado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            TempData[WC.Error] = "Error al editar Tipo Aplicacion";
            return View(marca);
        }
        public IActionResult Eliminar(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = marcaRepo.Obtener(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Marca marca)
        {
            if (marca == null)
            {
                TempData[WC.Error] = "Error al eliminar Tipo Aplicacion";
                return NotFound();
            }
            marcaRepo.Remover(marca);
            marcaRepo.grabar();
            TempData[WC.Exitosa] = "Tipo Aplicacion eliminado Exitosamente";
            return RedirectToAction(nameof(Index));

        }
    }
}
