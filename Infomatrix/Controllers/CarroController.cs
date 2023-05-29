using Infomatrix_Datos.Datos;
using Infomatrix_Modelos;
using Infomatrix_Modelos.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Infomatrix_Utilidades;
using Infomatrix_Datos.Datos.Repositorio.IRepositorio;

namespace Infomatrix.Controllers
{
    [Authorize]
    public class CarroController : Controller
    {
        //private readonly ApplicationDbContext db;
        private readonly IProductoRepositorio productoRepo;
        private readonly IUsuarioAplicacionRepositorio usuarioRepo;
        private readonly IOrdenRepositorio ordenRepo;
        private readonly IOrdenDetalleRepositorio ordenDetalleRepo;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IEmailSender emailSender;

        //Para que pueda ser usada en todo el controlador y no se pierdan sus valores
        [BindProperty]
        public ProductoUsuarioVM productoUsuarioVM { get; set; }

        public CarroController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender,
                                IProductoRepositorio productoRepo, IUsuarioAplicacionRepositorio usuarioRepo,
                                IOrdenDetalleRepositorio ordenDetalleRepo, IOrdenRepositorio ordenRepo)
        {
            this.productoRepo = productoRepo;
            this.usuarioRepo = usuarioRepo;
            this.ordenDetalleRepo = ordenDetalleRepo;
            this.ordenRepo = ordenRepo;
            this.webHostEnvironment = webHostEnvironment;
            this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            List<CarroCompra> carroComprasList = new List<CarroCompra>();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
               HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            List<int> prodEnCarro = carroComprasList.Select(i => i.ProductoId).ToList();
            //IEnumerable<Producto> prodList = db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            IEnumerable<Producto> prodList = productoRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id));
            List<Producto> prodListFinal = new List<Producto>();
            foreach (var objCarro in carroComprasList)
            {
                Producto ptem = prodList.FirstOrDefault(p => p.Id == objCarro.ProductoId);
                ptem.TempUnidades = objCarro.Unidades;
                prodListFinal.Add(ptem);
            }
            return View(prodListFinal);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Producto> ProdList)
        {
            List<CarroCompra> carroComprasList = new List<CarroCompra>();

            foreach (Producto prod in ProdList)
            {
                carroComprasList.Add(new CarroCompra
                {
                    ProductoId = prod.Id,
                    Unidades = prod.TempUnidades
                });
            }
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasList);

            return RedirectToAction(nameof(Resumen));
        }

        public IActionResult Resumen()
        {

           UsuarioAplicacion usuario;
            if (User.IsInRole(WC.AdminRole)) { 
                if(HttpContext.Session.Get<int>(WC.SessionOrdenId)  != 0)
                {
                    Orden orden = ordenRepo.ObtenerPrimero(u => u.Id == HttpContext.Session.Get<int>(WC.SessionOrdenId));
                    usuario = new UsuarioAplicacion()
                    {
                        Email = orden.Email,
                        NombreCompleto = orden.NombreCompleto,
                        PhoneNumber = orden.Telefono
                    };
                }
                else // Si no pertenece a una orden
                {
                    usuario = new UsuarioAplicacion();
                }
            } else
            {
                //Traer el usuario conectado
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                usuario = usuarioRepo.ObtenerPrimero(u => u.Id == claim.Value);
            }

            

            List<CarroCompra> carroComprasList = new List<CarroCompra>();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
               HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            List<int> prodEnCarro = carroComprasList.Select(i => i.ProductoId).ToList();
            //IEnumerable<Producto> prodList = db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            IEnumerable<Producto> prodList = productoRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id));
            productoUsuarioVM = new ProductoUsuarioVM()
            {
                //UsuarioAplicacion = db.UsuarioAplicacion.FirstOrDefault(u => u.Id == claim.Value),
                UsuarioAplicacion = usuario,
            };
            //Para llenar la vista de continuar del carro
            foreach (var carro in carroComprasList)
            {
                Producto pTemp = productoRepo.ObtenerPrimero(p => p.Id == carro.ProductoId);
                pTemp.TempUnidades = carro.Unidades;
                productoUsuarioVM.ProductoLista.Add(pTemp);
            }
            return View(productoUsuarioVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Resumen")]
        public async Task<IActionResult> ResumenPost(ProductoUsuarioVM productoUsuarioVM)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
            var rutaTemplate = webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templaces" + Path.DirectorySeparatorChar.ToString() + "PlantillaOrden.html";
            var subject = "Nueva Orden";
            string htmlBody = "";

            using (StreamReader sr = System.IO.File.OpenText(rutaTemplate))
            {
                htmlBody = sr.ReadToEnd();
            }

            //Nombre: { 0}
            //Email: { 1}
            //Telefono: { 2}
            //Productos: { 3}
            StringBuilder productoListaSB = new StringBuilder();
            foreach (var prod in this.productoUsuarioVM.ProductoLista)
            {
                productoListaSB.Append($" - Nombre : {prod.NombreProducto}<span style='font-size:14px;'> (ID: {prod.Id})</span><br/> ");
            }
            string messageBody = string.Format(htmlBody,
                                                productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                                                productoUsuarioVM.UsuarioAplicacion.Email,
                                                productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                                                productoListaSB.ToString());

            await emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

            //Grabar la Orden y Detalle en la BD
            Orden orden = new Orden()
            {
                UsuarioAplicacionId = claim.Value,
                NombreCompleto = this.productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                Email = this.productoUsuarioVM.UsuarioAplicacion.Email,
                Telefono = this.productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                FechaOrden = DateTime.Now
            };

            ordenRepo.Agregar(orden);
            ordenRepo.grabar();

            foreach (var prod in this.productoUsuarioVM.ProductoLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    OrdenId = orden.Id,
                    ProductoId = prod.Id
                };
                ordenDetalleRepo.Agregar(ordenDetalle);
            }
            ordenDetalleRepo.grabar();

            return RedirectToAction(nameof(Confirmacion));
        }

        public IActionResult Confirmacion()
        {
            HttpContext.Session.Clear();
            return View();
        }
        public IActionResult Remover(int Id)
        {
            List<CarroCompra> carroComprasList = new List<CarroCompra>();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            carroComprasList.Remove(carroComprasList.FirstOrDefault(p => p.ProductoId == Id));
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarCarro(IEnumerable<Producto> ProdList)
        {
            List<CarroCompra> carroComprasList = new List<CarroCompra>();

            foreach(Producto prod in ProdList)
            {
                carroComprasList.Add(new CarroCompra
                {
                    ProductoId = prod.Id,
                    Unidades = prod.TempUnidades
                });
            }
            HttpContext.Session.Set(WC.SessionCarroCompras, carroComprasList);

            return RedirectToAction(nameof(Index));
        }

    }
}

