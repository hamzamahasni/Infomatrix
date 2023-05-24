using Infomatrix_Modelos;

namespace Infomatrix_Datos.Datos.Repositorio.IRepositorio
{
    public interface IUsuarioAplicacionRepositorio : IRepositorio<UsuarioAplicacion>
    {
        void Actualizar (UsuarioAplicacion usuarioAplicacion);

    }
}
