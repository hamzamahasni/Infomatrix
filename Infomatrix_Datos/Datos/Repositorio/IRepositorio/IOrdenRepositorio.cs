using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio.IRepositorio
{
    public interface IOrdenRepositorio : IRepositorio<Orden>
    {
        void Actualizar (Orden orden);

    }
}
