using Infomatrix_Datos.Datos;
using Infomatrix_Modelos;
using Infomatrix_Modelos.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Infomatrix_Utilidades;
using Infomatrix_Datos.Datos.Repositorio.IRepositorio;

namespace Infomatrix.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductoController : Controller
    {
        private readonly IProductoRepositorio productoRepo;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductoController(IProductoRepositorio productoRepo, IWebHostEnvironment webHostEnvironment)
        {
            this.productoRepo = productoRepo;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            //IEnumerable<Producto> lista = db.Producto.Include(c => c.Categoria).Include(t => t.Marca);
            IEnumerable<Producto> lista = productoRepo.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            return View(lista);
        }

        public IActionResult Upsert(int? Id)
        {
            //IEnumerable<SelectListItem> categoriaDropDown = db.Categoria.Select(c => new SelectListItem
            //{
            //    Text= c.NombreCategoria,
            //    Value = c.Id.ToString()
            //});

            //ViewBag.categoriaDropDown = categoriaDropDown;

            //Producto producto = new Producto();
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                //CategoriaLista = db.Categoria.Select(c => new SelectListItem
                //{
                //    Text = c.NombreCategoria,
                //    Value = c.Id.ToString()
                //}),
                //MarcaLista = db.Marca.Select(c => new SelectListItem
                //{
                //    Text = c.Nombre,
                //    Value = c.Id.ToString()
                //})
                CategoriaLista = productoRepo.ObtenerTodosDropdownList(WC.CategoriaNombre),
                MarcaLista = productoRepo.ObtenerTodosDropdownList(WC.MarcaNombre)
            };


            if (Id == null)
            {
                //Crear un nuevo producto
                return View(productoVM);
            }
            else
            {
                productoVM.Producto = productoRepo.Obtener(Id.GetValueOrDefault());
                if (productoVM.Producto == null)
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
                    productoRepo.Agregar(productoVM.Producto);
                    TempData[WC.Exitosa] = "Producto creado Exitosamente";
                }
                else
                { //Actualizar

                    //Seleccionamo el producto actual (Problema de que estamos seleccionado un producto en memoria)
                    var objProducto = productoRepo.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking: false);

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
                    productoRepo.Actualizar(productoVM.Producto);
                    TempData[WC.Exitosa] = "Producto actulizado Exitosamente";
                }
                productoRepo.grabar();

                return RedirectToAction("Index");
            }
            //Se llenan nuevamente las listas si algo falla
            //productoVM.CategoriaLista = db.Categoria.Select(c => new SelectListItem
            //{
            //    Text = c.NombreCategoria,
            //    Value = c.Id.ToString()
            //});
            //productoVM.MarcaLista = db.Marca.Select(c => new SelectListItem
            //{
            //    Text = c.Nombre,
            //    Value = c.Id.ToString()
            //});
            productoVM.CategoriaLista = productoRepo.ObtenerTodosDropdownList(WC.CategoriaNombre);
            productoVM.MarcaLista = productoRepo.ObtenerTodosDropdownList(WC.MarcaNombre);

            return View(productoVM);
        }

        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            //Producto producto = db.Producto.Include(c => c.Categoria)
            //                               .Include(t => t.Marca)
            //                               .FirstOrDefault(p => p.Id == Id);
            Producto producto = productoRepo.ObtenerPrimero(p => p.Id == Id, incluirPropiedades: "Categoria,Marca");
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
                TempData[WC.Error] = "Error al eliminar el Producto";
                return NotFound();
            }
            string upload = webHostEnvironment.WebRootPath + WC.imagenRuta;

            Producto pr = productoRepo.ObtenerPrimero(p => p.Id == producto.Id, incluirPropiedades: "Categoria,Marca", isTracking: false);
            //borra la imagen anterior
            var anteriorFile = Path.Combine(upload, pr.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);

            }
            productoRepo.Remover(producto);
            productoRepo.grabar();
            TempData[WC.Exitosa] = "Producto eliminado Exitosamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
