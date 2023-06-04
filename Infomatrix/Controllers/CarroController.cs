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
using Microsoft.AspNetCore.Http;
using PayPal.Api;


namespace Infomatrix.Controllers
{
    [Authorize]
    public class CarroController : Controller
    {
        //private readonly ApplicationDbContext db;
        private readonly IProductoRepositorio productoRepo;
        private readonly IUsuarioAplicacionRepositorio usuarioRepo;
        private readonly IVentaRepositorio ventaRepo;
        private readonly IVentaDetalleRepositorio ventaDetalleRepo;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IEmailSender emailSender;
        IConfiguration configuration;
        private IHttpContextAccessor httpContextAccessor;
        private static double totalapagar = 0.00;
        private static string payerId;
        private static APIContext apiContext;
        private static string paypalRedirectUrl;
        private static ProductoUsuarioVM productoUsuarioVM;

        public CarroController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender,
                                IProductoRepositorio productoRepo, IUsuarioAplicacionRepositorio usuarioRepo,
                                IVentaDetalleRepositorio ventaDetalleRepo, IVentaRepositorio ventaRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.productoRepo = productoRepo;
            this.usuarioRepo = usuarioRepo;
            this.ventaDetalleRepo = ventaDetalleRepo;
            this.ventaRepo = ventaRepo;
            this.webHostEnvironment = webHostEnvironment;
            this.emailSender = emailSender;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;

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
            if (User.IsInRole(WC.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WC.SessionVentaId) != 0)
                {
                    Venta venta = ventaRepo.ObtenerPrimero(u => u.Id == HttpContext.Session.Get<int>(WC.SessionVentaId));
                    usuario = new UsuarioAplicacion()
                    {
                        Email = venta.Email,
                        NombreCompleto = venta.NombreCompleto,
                        PhoneNumber = venta.Telefono
                    };
                }
                else // Si no pertenece a una venta
                {
                    usuario = new UsuarioAplicacion();
                }
            }
            else
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
                UsuarioAplicacion = usuario
            };
            //Para llenar la vista de continuar del carro
            foreach (var carro in carroComprasList)
            {
                Producto pTemp = productoRepo.ObtenerPrimero(p => p.Id == carro.ProductoId);
                pTemp.TempUnidades = carro.Unidades;
                productoUsuarioVM.ProductoLista.Add(pTemp);

            }
            totalapagar = 0.00;
            for (int i = 0; i < productoUsuarioVM.ProductoLista.Count(); i++)
            {
                totalapagar += Convert.ToDouble(productoUsuarioVM.ProductoLista[i].Precio) * productoUsuarioVM.ProductoLista[i].TempUnidades;
            }

            return View(productoUsuarioVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Resumen")]
        public async Task<IActionResult> ResumenPost(ProductoUsuarioVM productoUsuario)
        {
            productoUsuarioVM = productoUsuario;
            if (!User.IsInRole(WC.AdminRole))
            {
                await PaymentWithPaypalAsync();
                return Redirect(paypalRedirectUrl);
            }
            else {
                await CorreoYGrabarAsync();
                return RedirectToAction(nameof(Confirmacion));
            }

        }

        public async Task<ActionResult> PaymentWithPaypalAsync(string Cancel = null, string blogId = "", string PayerID = "", string guid = "")
        {
            //getting the apiContext  
            var ClientID = configuration["PayPal:Key"];
            var ClientSecret = configuration["PayPal:Secret"];
            var mode = configuration.GetValue<string>("PayPal:mode");
            apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);
            try
            {

                payerId = PayerID;
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Carro/PaymentWithPayPal?";
                    var guidd = Convert.ToString((new Random()).Next(100000));
                    guid = guidd;
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, blogId);
                    var links = createdPayment.links.GetEnumerator();
                    paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // Tiene que pagar o es admin
                    if (Paypal()) {

                        await CorreoYGrabarAsync();
                        payerId = null;
                        return RedirectToAction(nameof(Confirmacion));
                    } else
                    {
                        return RedirectToAction(nameof(Resumen));
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Resumen));
            }
            //on successful payment, show success page to user.  

            return RedirectToAction(nameof(Confirmacion));
        }

        public async Task CorreoYGrabarAsync()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
            var rutaTemplate = webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templaces" + Path.DirectorySeparatorChar.ToString() + "PlantillaVenta.html";
            var subject = "Nueva Venta";
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
            foreach (var prod in productoUsuarioVM.ProductoLista)
            {
                productoListaSB.Append($" - Nombre del producto: {prod.NombreProducto} <br> <span style='font-size:14px;'> ID: {prod.Id} | Precio {prod.Precio} € | Cantidad: {prod.TempUnidades}</span><br/> ");
            }
            string messageBody = string.Format(htmlBody,
                                                productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                                                productoUsuarioVM.UsuarioAplicacion.Email,
                                                productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                                                productoListaSB.ToString());

            await emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);
            await emailSender.SendEmailAsync(productoUsuarioVM.UsuarioAplicacion.Email, subject, messageBody);

            //Grabar la Venta y Detalle en la BD
            Venta venta = new Venta()
            {
                UsuarioAplicacionId = claim.Value,
                NombreCompleto = productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                Email = productoUsuarioVM.UsuarioAplicacion.Email,
                Telefono = productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                FechaVenta = DateTime.Now
            };

            ventaRepo.Agregar(venta);
            ventaRepo.grabar();

            for (int i = 0; i < productoUsuarioVM.ProductoLista.Count(); i++)
            {
                for (int x = 0; x < productoUsuarioVM.ProductoLista[i].TempUnidades; x++)
                {
                    VentaDetalle ventaDetalle = new VentaDetalle()
                    {
                        VentaId = venta.Id,
                        ProductoId = productoUsuarioVM.ProductoLista[i].Id
                    };
                    ventaDetalleRepo.Agregar(ventaDetalle);
                }
            }


            ventaDetalleRepo.grabar();
        }

        public bool Paypal()
        {
            var paymentId = httpContextAccessor.HttpContext.Session.GetString("payment");
            var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);
            //If executed payment failed then we will show payment failure message to user  
            if (executedPayment.state.ToLower() != "approved")
            {
                return false;
                //return RedirectToAction(nameof(Resumen));
            }
            var blogIds = executedPayment.transactions[0].item_list.items[0].sku;
            return true;

            //return RedirectToAction(nameof(Confirmacion));

        }


        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string blogId)
        {
            //create itemlist and add item objects to it  

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            string texto = totalapagar.ToString() + ".00";
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Item Detail",
                currency = "EUR",
                price = texto,
                quantity = "1",
                sku = "asd"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            //var details = new Details()
            //{
            //    tax = "1",
            //    shipping = "1",
            //    subtotal = "1"
            //};
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "EUR",
                total = texto, // Total must be equal to sum of tax, shipping and subtotal.  
                //details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Guid.NewGuid().ToString(), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
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

            foreach (Producto prod in ProdList)
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



