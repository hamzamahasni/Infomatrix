using Infomatrix.Datos.Repositorio.IRepositorio;
using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio
{
    public class OrdenRepositorio : Repositorio<Orden>, IOrdenRepositorio
    {
        private readonly ApplicationDbContext db;

        public OrdenRepositorio(ApplicationDbContext db): base(db) //Por el padre, aseguramos que estemos trabajando con mismo db
        {
            this.db = db;
        }
        public void Actualizar(Orden orden)
        {
            db.Update(orden);
        }

    }
}
