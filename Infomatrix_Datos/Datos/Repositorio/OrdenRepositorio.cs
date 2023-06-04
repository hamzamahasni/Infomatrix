using Infomatrix_Datos.Datos.Repositorio.IRepositorio;
using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio
{
    public class VentaRepositorio : Repositorio<Venta>, IVentaRepositorio
    {
        private readonly ApplicationDbContext db;

        public VentaRepositorio(ApplicationDbContext db): base(db) //Por el padre, aseguramos que estemos trabajando con mismo db
        {
            this.db = db;
        }
        public void Actualizar(Venta venta)
        {
            db.Update(venta);
        }

    }
}
