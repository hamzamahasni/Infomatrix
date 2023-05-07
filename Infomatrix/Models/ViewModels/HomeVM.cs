using Microsoft.AspNetCore.Mvc.Rendering;

namespace Infomatrix.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Producto> Productos { get; set; }
        public IEnumerable<Categoria> Categorias { get; set; }
    }
}
