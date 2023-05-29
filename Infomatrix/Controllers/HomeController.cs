using Infomatrix_Datos.Datos;
using Infomatrix_Datos.Datos.Repositorio.IRepositorio;
using Infomatrix_Modelos;
using Infomatrix_Modelos.ViewModels;
using Infomatrix_Utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Infomatrix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductoRepositorio productoRepo;
        private readonly ICategoriaRepositorio categoriaRepo;

        public HomeController(ILogger<HomeController> logger, IProductoRepositorio productoRepo, ICategoriaRepositorio categoriaRepo)
        {
            _logger = logger;
            this.productoRepo = productoRepo;
            this.categoriaRepo = categoriaRepo;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                //Productos = db.Producto.Include(c=>c.Categoria).Include(t=>t.Marca),
                //Categorias = db.Categoria
                Productos = productoRepo.ObtenerTodos(incluirPropiedades: "Categoria,Marca"),
                Categorias = categoriaRepo.ObtenerTodos()
            };
            return View(homeVM);
        }

        public IActionResult Detalle(int Id)
        {
            List<CarroCompra> carroComprasLista = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null
                && HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasLista = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            DetalleVM detalleVM = new DetalleVM()
            {
                //Producto = db.Producto.Include(c => c.Categoria).Include(t => t.Marca).Where(p => p.Id == Id).FirstOrDefault(),
                Producto = productoRepo.ObtenerPrimero(p => p.Id == Id, incluirPropiedades: "Categoria,Marca"),
                ExisteEnCarro = false
            };

            foreach (var item in carroComprasLista)
            {
                if (item.ProductoId == Id)
                {
                    detalleVM.ExisteEnCarro = true;
                }
            }
            return View(detalleVM);
        }


        [HttpPost, ActionName("Detalle")]
        [ValidateAntiForgeryToken]
        public IActionResult DetallePost(int Id, DetalleVM detalleVM)
        {
            List<CarroCompra> carroComprasLista = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null
                && HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasLista = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }
            carroComprasLista.Add(new CarroCompra { ProductoId = Id, Unidades = detalleVM.Producto.TempUnidades});
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasLista);
            TempData[WC.Exitosa] = "Producto Agregado";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoverDeCarro(int Id)
        {
            List<CarroCompra> carroComprasLista = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null
                && HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasLista = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            var productoARemover = carroComprasLista.SingleOrDefault(x => x.ProductoId == Id);
            if (productoARemover != null)
            {
                carroComprasLista.Remove(productoARemover);
                TempData[WC.Exitosa] = "Producto Removido";

            }

            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasLista);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}