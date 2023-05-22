using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio.IRepositorio
{
    public interface ICategoriaRepositorio : IRepositorio<Categoria>
    {
        void Actualizar (Categoria categoria);

    }
}
