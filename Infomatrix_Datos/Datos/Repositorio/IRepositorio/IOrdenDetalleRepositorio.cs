using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio.IRepositorio
{
    public interface IOrdenDetalleRepositorio : IRepositorio<OrdenDetalle>
    {
        void Actualizar (OrdenDetalle ordenDetalle);

    }
}
