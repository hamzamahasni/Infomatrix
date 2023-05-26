using Infomatrix_Datos.Datos.Repositorio.IRepositorio;
using Infomatrix_Modelos;
using Infomatrix_Modelos.ViewModels;
using Infomatrix_Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infomatrix.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class OrdenController : Controller
    {
        private readonly IOrdenRepositorio ordenRepo;
        private readonly IOrdenDetalleRepositorio ordenDetalleRepo;

        [BindProperty]//que sea accesible en todo el controlador
        public OrdenMV OrdenMV { get; set; }
        public OrdenController(IOrdenRepositorio ordenRepo, IOrdenDetalleRepositorio ordenDetalleRepo)
        {
            this.ordenRepo = ordenRepo;
            this.ordenDetalleRepo = ordenDetalleRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detalle(int id)
        {
            OrdenMV = new OrdenMV()
            {
                Orden = ordenRepo.ObtenerPrimero(o => o.Id == id),
                OrdenDetalle = ordenDetalleRepo.ObtenerTodos(d => d.Id == id, incluirPropiedades:"Producto")
            };
            return View(OrdenMV);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detalle()
        {
            List<CarroCompra> carroCompraLista = new List<CarroCompra>();
            OrdenMV.OrdenDetalle = ordenDetalleRepo.ObtenerTodos(d => d.OrdenId == OrdenMV.Orden.Id);
            foreach (var detalle in OrdenMV.OrdenDetalle)
            {
                CarroCompra carroCompra = new CarroCompra()
                {
                    ProductoId = detalle.ProductoId
                };
            carroCompraLista.Add(carroCompra);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCarroCompras, carroCompraLista);
            HttpContext.Session.Set(WC.SessionOrdenId,OrdenMV.Orden.Id);
            return RedirectToAction("Index","Carro");
        }

        [HttpPost]
        public IActionResult Eliminar()
        {
            Orden orden = ordenRepo.ObtenerPrimero(o => o.Id == OrdenMV.Orden.Id);
            IEnumerable<OrdenDetalle> ordenDetalle = ordenDetalleRepo.ObtenerTodos(d=>d.OrdenId ==OrdenMV.Orden.Id);

            ordenDetalleRepo.RemoverRango(ordenDetalle);
            ordenRepo.Remover(orden);
            ordenRepo.grabar();
            
            return RedirectToAction(nameof(Index));
        }

        #region APIs

        [HttpGet]
        public IActionResult ObtenerListaOrdenes() {

            return Json(new { data = ordenRepo.ObtenerTodos() });

        }


        #endregion
    }
}
