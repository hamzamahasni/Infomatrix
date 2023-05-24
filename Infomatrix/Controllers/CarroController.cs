using Infomatrix_Datos.Datos;
using Infomatrix_Modelos;
using Infomatrix_Modelos.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Infomatrix_Utilidades;

namespace Infomatrix.Controllers
{
    [Authorize]
    public class CarroController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IEmailSender emailSender;


        //Para que pueda ser usada en todo el controlador y no se pierdan sus valores
        [BindProperty]
        public ProductoUsuarioVM productoUsuarioVM { get; set; }

        public CarroController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
            this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            List<CarroCompra> carroComprasList = new List<CarroCompra>();

            if(HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras)!=null &&
               HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count()>0) 
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            List<int> prodEnCarro = carroComprasList.Select(i=>i.ProductoId).ToList();
            IEnumerable<Producto> prodList = db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            //IEnumerable<Producto> prodList = productoRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id));
            return View(prodList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Resumen));
        }

        public IActionResult Resumen()
        {
            //Traer el usuario conectado
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<CarroCompra> carroComprasList = new List<CarroCompra>();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
               HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroComprasList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            List<int> prodEnCarro = carroComprasList.Select(i => i.ProductoId).ToList();
            IEnumerable<Producto> prodList = db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            //IEnumerable<Producto> prodList = productoRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id));
            productoUsuarioVM = new ProductoUsuarioVM()
            {
                UsuarioAplicacion = db.UsuarioAplicacion.FirstOrDefault(u => u.Id == claim.Value),
                //UsuarioAplicacion = usuarioRepo.ObtenerPrimero(u => u.Id == claim.Value),
                ProductoLista = prodList.ToList()
            };
            return View(productoUsuarioVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Resumen")]
        public async Task<IActionResult> ResumenPost(ProductoUsuarioVM productoUsuarioMV)
        {
            //var claimsidentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            //Estamos rellenando el email de los pedidos a los clientes
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
            foreach (var prod in productoUsuarioMV.ProductoLista)
            {
                productoListaSB.Append($" - Nombre : {prod.NombreProducto}<span style='font-size:14px;'> (ID: {prod.Id})</span><br/> ");
            }
            string messageBody = string.Format(htmlBody,
                                                productoUsuarioMV.UsuarioAplicacion.NombreCompleto,
                                                productoUsuarioMV.UsuarioAplicacion.Email,
                                                productoUsuarioMV.UsuarioAplicacion.PhoneNumber,
                                                productoListaSB.ToString());

            await emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

            ////Grabar la Orden y Detalle en la BD
            //Orden orden = new Orden()
            //{
            //    UsuarioAplicacionId = claim.Value,
            //    NombreCompleto = productoUsuarioMV.UsuarioAplicacion.NombreCompleto,
            //    Email = productoUsuarioMV.UsuarioAplicacion.Email,
            //    Telefono = productoUsuarioMV.UsuarioAplicacion.PhoneNumber,
            //    FechaOrden = DateTime.Now
            //};

            //ordenRepo.Agregar(orden);
            //ordenRepo.grabar();

            //foreach (var prod in productoUsuarioMV.ProductoLista)
            //{
            //    OrdenDetalle ordenDetalle = new OrdenDetalle()
            //    {
            //        OrdenId = orden.Id,
            //        ProductoId = prod.Id
            //    };
            //    ordenDetalleRepo.Agregar(ordenDetalle);
            //}
            //ordenDetalleRepo.grabar();

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
    }
}

