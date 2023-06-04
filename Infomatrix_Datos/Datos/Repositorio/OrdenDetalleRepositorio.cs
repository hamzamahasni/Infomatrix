using Infomatrix_Datos.Datos.Repositorio.IRepositorio;
using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio
{
    public class VentaDetalleRepositorio : Repositorio<VentaDetalle>, IVentaDetalleRepositorio
    {
        private readonly ApplicationDbContext db;

        public VentaDetalleRepositorio(ApplicationDbContext db): base(db) //Por el padre, aseguramos que estemos trabajando con mismo db
        {
            this.db = db;
        }
        public void Actualizar(VentaDetalle ventaDetalle)
        {
            db.Update(ventaDetalle);
        }

    }
}
