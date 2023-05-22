using Infomatrix.Datos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc.Rendering;
using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio
{
    public class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        private readonly ApplicationDbContext db;

        public ProductoRepositorio(ApplicationDbContext db): base(db) //Por el padre, aseguramos que estemos trabajando con mismo db
        {
            this.db = db;
        }
        public void Actualizar(Producto producto)
        {
            db.Update(producto);
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropdownList(string obj)
        {
            if(obj == WC.CategoriaNombre)
            {
                return db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.NombreCategoria,
                    Value = c.Id.ToString()
                });
            }
            if (obj == WC.MarcaNombre)
            {
                return db.Marca.Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                });
            }
            return null;
        }
    }
}
