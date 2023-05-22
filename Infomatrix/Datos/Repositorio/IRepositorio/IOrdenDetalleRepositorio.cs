using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio.IRepositorio
{
    public interface IOrdenDetalleRepositorio : IRepositorio<OrdenDetalle>
    {
        void Actualizar (OrdenDetalle ordenDetalle);

    }
}
