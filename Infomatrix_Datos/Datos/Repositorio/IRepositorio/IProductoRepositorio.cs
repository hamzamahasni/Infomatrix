using Infomatrix_Modelos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Infomatrix_Datos.Datos.Repositorio.IRepositorio
{
    public interface IProductoRepositorio : IRepositorio<Producto>
    {
        void Actualizar (Producto producto);

        IEnumerable<SelectListItem> ObtenerTodosDropdownList(string obj);
    }
}
