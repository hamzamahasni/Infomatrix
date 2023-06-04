using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio.IRepositorio
{
    public interface IVentaDetalleRepositorio : IRepositorio<VentaDetalle>
    {
        void Actualizar (VentaDetalle ventaDetalle);

    }
}
