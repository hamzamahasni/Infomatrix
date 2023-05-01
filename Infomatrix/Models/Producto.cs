using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infomatrix.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre del Producto es requerido")]
        public string NombreProducto { get; set; }
        [Required(ErrorMessage = "Descripcion corta es requerida")]
        public string DescripcionCorta { get; set; }
        [Required(ErrorMessage = "Descripcion del producto es requerido")]
        public string DescripcionProducto { get; set; }

        [Required(ErrorMessage = "Precio del producto es requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "Precio del producto debe ser mayor a 0")]
        public double Precio { get; set; }
        public string? ImagenUrl { get; set; }

        //FK
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        public int MarcaId { get; set; }
        [ForeignKey("MarcaId")]
        public virtual Marca? Marca { get; set; }
    }
}
