using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio.IRepositorio
{
    public interface IMarcaRepositorio : IRepositorio<Marca>
    {
        void Actualizar (Marca marca);

    }
}
