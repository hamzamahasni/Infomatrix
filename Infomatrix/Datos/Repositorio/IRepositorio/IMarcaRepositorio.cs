using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio.IRepositorio
{
    public interface IMarcaRepositorio : IRepositorio<Marca>
    {
        void Actualizar (Marca marca);

    }
}
