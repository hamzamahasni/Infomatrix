using Infomatrix.Datos.Repositorio.IRepositorio;
using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio
{
    public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
    {
        private readonly ApplicationDbContext db;

        public CategoriaRepositorio(ApplicationDbContext db): base(db) //Por el padre, aseguramos que estemos trabajando con mismo db
        {
            this.db = db;
        }
        public void Actualizar(Categoria categoria)
        {
            var catAnterior = db.Categoria.FirstOrDefault(c=>c.Id == categoria.Id);
            if (catAnterior != null) {
                catAnterior.NombreCategoria = categoria.NombreCategoria;
                catAnterior.MostrarOrden = categoria.MostrarOrden;
            }
        }
    }
}
