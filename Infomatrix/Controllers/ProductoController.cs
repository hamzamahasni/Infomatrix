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
                    //Seleccionamo el producto actual (Problema de que estamos seleccionado un producto en memoria)
                    var objProducto = db.Producto.AsNoTracking().FirstOrDefault(p => p.Id == productoVM.Producto.Id);

                    if (files.Count > 0) // Usuario carga nueva imagen para actualizar
                    {
                        string upload = webRootPath + WC.imagenRuta;
                        //Creamos la imagen
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        //borra la imagen anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);

                        }

                        //Insertamos la imagen nueva
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productoVM.Producto.ImagenUrl = fileName + extension;

                    }
                    else //La imagen es la misma
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    db.Producto.Update(productoVM.Producto);
                    //TempData[WC.Exitosa] = "Producto actulizado Exitosamente";
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //Se llenan nuevamente las listas si algo falla
            productoVM.CategoriaLista = db.Categoria.Select(c => new SelectListItem
            {
                Text = c.NombreCategoria,
                Value = c.Id.ToString()
            });
            productoVM.MarcaLista = db.Marca.Select(c => new SelectListItem
            {
                Text = c.Nombre,
                Value = c.Id.ToString()
            });

            return View(productoVM);

        }
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Producto producto = db.Producto.Include(c => c.Categoria)
                                           .Include(t => t.Marca)
                                           .FirstOrDefault(p => p.Id == Id);
            //Producto producto = productoRepo.ObtenerPrimero(p => p.Id == Id, incluirPropiedades: "Categoria,TipoAplicacion");
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Producto producto)
        {
            if (producto == null)
            {
                //TempData[WC.Error] = "Error al eliminar el Producto";
                return NotFound();
            }
            string upload = webHostEnvironment.WebRootPath + WC.imagenRuta;

            //Producto pr = productoRepo.ObtenerPrimero(p => p.Id == producto.Id, incluirPropiedades: "Categoria,TipoAplicacion", isTracking: false);
            //borra la imagen anterior
            var anteriorFile = Path.Combine(upload, producto.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);

            }
            db.Producto.Remove(producto);
            db.SaveChanges();
            //TempData[WC.Exitosa] = "Producto eliminado Exitosamente";
            return RedirectToAction(nameof(Index));
        }

    }
}
