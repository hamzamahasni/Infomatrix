using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infomatrix_Modelos
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }
        public string UsuarioAplicacionId { get; set; }
        [ForeignKey("UsuarioAplicacionId")]
        public UsuarioAplicacion UsuarioAplicacion { get; set; }
        public DateTime FechaVenta { get; set; }
        [Required(ErrorMessage = "Telefono es requerido")]
        public string Telefono { get; set; }
        [Required]
        public string NombreCompleto { get; set; }
        [Required(ErrorMessage = "Email es requerido")]
        public string Email { get; set; }
    }
}
