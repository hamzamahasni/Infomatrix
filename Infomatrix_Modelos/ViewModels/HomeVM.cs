using Microsoft.AspNetCore.Mvc.Rendering;

namespace Infomatrix_Modelos.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Producto> Productos { get; set; }
        public IEnumerable<Categoria> Categorias { get; set; }
    }
}
