using Infomatrix_Datos.Datos.Repositorio.IRepositorio;
using Infomatrix_Modelos;
using Infomatrix_Modelos.ViewModels;
using Infomatrix_Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infomatrix.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class VentaController : Controller
    {
        private readonly IVentaRepositorio ventaRepo;
        private readonly IVentaDetalleRepositorio ventaDetalleRepo;

        [BindProperty]//que sea accesible en todo el controlador
        public VentaVM VentaVM { get; set; }
        public VentaController(IVentaRepositorio ventaRepo, IVentaDetalleRepositorio ventaDetalleRepo)
        {
            this.ventaRepo = ventaRepo;
            this.ventaDetalleRepo = ventaDetalleRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detalle(int id)
        {
            VentaVM = new VentaVM()
            {
                Venta = ventaRepo.ObtenerPrimero(o => o.Id == id),
                VentaDetalle = ventaDetalleRepo.ObtenerTodos(d => d.VentaId == id, incluirPropiedades:"Producto")
            };
            return View(VentaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detalle()
        {
            List<CarroCompra> carroCompraLista = new List<CarroCompra>();
            VentaVM.VentaDetalle = ventaDetalleRepo.ObtenerTodos(d => d.VentaId == VentaVM.Venta.Id);
            foreach (var detalle in VentaVM.VentaDetalle)
            {
                CarroCompra carroCompra = new CarroCompra()
                {
                    ProductoId = detalle.ProductoId
                };
            carroCompraLista.Add(carroCompra);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCarroCompras, carroCompraLista);
            HttpContext.Session.Set(WC.SessionVentaId,VentaVM.Venta.Id);
            return RedirectToAction("Index","Carro");
        }

        [HttpPost]
        public IActionResult Eliminar()
        {
            Venta venta = ventaRepo.ObtenerPrimero(o => o.Id == VentaVM.Venta.Id);
            IEnumerable<VentaDetalle> ventaDetalle = ventaDetalleRepo.ObtenerTodos(d=>d.VentaId ==VentaVM.Venta.Id);

            ventaDetalleRepo.RemoverRango(ventaDetalle);
            ventaRepo.Remover(venta);
            ventaRepo.grabar();
            
            return RedirectToAction(nameof(Index));
        }

        #region APIs

        [HttpGet]
        public IActionResult ObtenerListaVentas() {

            return Json(new { data = ventaRepo.ObtenerTodos() });

        }


        #endregion
    }
}
