using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio.IRepositorio
{
    public interface IVentaRepositorio : IRepositorio<Venta>
    {
        void Actualizar (Venta venta);

    }
}
