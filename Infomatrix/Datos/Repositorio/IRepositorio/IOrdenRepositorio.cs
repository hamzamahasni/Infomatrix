using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio.IRepositorio
{
    public interface IOrdenRepositorio : IRepositorio<Orden>
    {
        void Actualizar (Orden orden);

    }
}
