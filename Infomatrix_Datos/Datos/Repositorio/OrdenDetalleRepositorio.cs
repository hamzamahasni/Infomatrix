using Infomatrix_Datos.Datos.Repositorio.IRepositorio;
using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio
{
    public class OrdenDetalleRepositorio : Repositorio<OrdenDetalle>, IOrdenDetalleRepositorio
    {
        private readonly ApplicationDbContext db;

        public OrdenDetalleRepositorio(ApplicationDbContext db): base(db) //Por el padre, aseguramos que estemos trabajando con mismo db
        {
            this.db = db;
        }
        public void Actualizar(OrdenDetalle ordenDetalle)
        {
            db.Update(ordenDetalle);
        }

    }
}
