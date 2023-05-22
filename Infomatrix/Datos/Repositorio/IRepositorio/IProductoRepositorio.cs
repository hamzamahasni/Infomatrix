using Infomatrix.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Infomatrix.Datos.Repositorio.IRepositorio
{
    public interface IProductoRepositorio : IRepositorio<Producto>
    {
        void Actualizar (Producto producto);

        IEnumerable<SelectListItem> ObtenerTodosDropdownList(string obj);
    }
}
