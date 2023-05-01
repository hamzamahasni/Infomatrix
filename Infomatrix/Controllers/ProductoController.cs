using Infomatrix.Datos;
using Infomatrix.Models;
using Infomatrix.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Infomatrix.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment; //para poder recibir imagenes

        public ProductoController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Producto> lista = db.Producto.Include(c=>c.Categoria)
                                                     .Include(m=>m.Marca);
            return View(lista);
        }
        public IActionResult Upsert(int? Id)
        {
            //IEnumerable<SelectListItem> categoriaDropDown = db.Categoria.Select(c => new SelectListItem
            //{
            //    Text = c.NombreCategoria,
            //    Value = c.Id.ToString()
            //});

            //ViewBag.categoriaDropDown = categoriaDropDown;
            //Producto producto = new Producto();

            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.NombreCategoria,
                    Value = c.Id.ToString()
                }),
                MarcaLista = db.Marca.Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                })
            };
            if (Id == null)
            {
                //Crear un nuevo producto
                return View(productoVM);
            }
            else
            {
                productoVM.Producto = db.Producto.Find(Id);
                if (productoVM == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }

        }
        //Conseguir la imagen de la vista
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = webHostEnvironment.WebRootPath;
                if (productoVM.Producto.Id == 0)
                {
                    //Crear
                    string upload = webRootPath + WC.imagenRuta;
                    //asignar una id a la imagen
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    db.Producto.Add(productoVM.Producto);
                }
                else
                {
                    //Actualizar
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productoVM);

        }


    }
}
