using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio.IRepositorio
{
    public interface IUsuarioAplicacionRepositorio : IRepositorio<UsuarioAplicacion>
    {
        void Actualizar (UsuarioAplicacion usuarioAplicacion);

    }
}
