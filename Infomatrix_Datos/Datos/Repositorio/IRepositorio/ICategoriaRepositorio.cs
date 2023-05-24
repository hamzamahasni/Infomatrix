using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio.IRepositorio
{
    public interface ICategoriaRepositorio : IRepositorio<Categoria>
    {
        void Actualizar (Categoria categoria);

    }
}
