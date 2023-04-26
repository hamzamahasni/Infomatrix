using System.ComponentModel.DataAnnotations;

namespace Infomatrix.Models
{
    public class Marca
    {

        [Key]
        public int Id { get; set; }
        //[Required(ErrorMessage = "Nombre de Categoria es Obligatorio.")]
        public string Nombre { get; set; }

        //[Required(ErrorMessage = "Orden es Obligatorio.")]
        //[Range(1, int.MaxValue, ErrorMessage = "El orden debe ser mayor a 0.")]
    }
}
