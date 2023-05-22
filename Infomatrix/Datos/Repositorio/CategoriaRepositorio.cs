using Infomatrix.Datos.Repositorio.IRepositorio;
using Infomatrix.Models;

namespace Infomatrix.Datos.Repositorio
{
    public class MarcaRepositorio : Repositorio<Marca>, IMarcaRepositorio
    {
        private readonly ApplicationDbContext db;

        public MarcaRepositorio(ApplicationDbContext db): base(db) //Por el padre, aseguramos que estemos trabajando con mismo db
        {
            this.db = db;
        }
        public void Actualizar(Marca marca)
        {
            var tipoAnterior = db.Marca.FirstOrDefault(c => c.Id == marca.Id);
            if (tipoAnterior != null)
            {
                tipoAnterior.Nombre = marca.Nombre;
            }
        }
    }
}
