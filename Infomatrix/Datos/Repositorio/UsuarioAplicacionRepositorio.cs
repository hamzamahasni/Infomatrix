using Infomatrix.Datos.Repositorio.IRepositorio;
using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio
{
    public class UsuarioAplicacionRepositorio : Repositorio<UsuarioAplicacion>, IUsuarioAplicacionRepositorio
    {
        private readonly ApplicationDbContext db;

        public UsuarioAplicacionRepositorio(ApplicationDbContext db): base(db) //Por el padre, aseguramos que estemos trabajando con mismo db
        {
            this.db = db;
        }
        public void Actualizar(UsuarioAplicacion usuarioAplicacion)
        {
            db.Update(usuarioAplicacion);
        }

    }
}
