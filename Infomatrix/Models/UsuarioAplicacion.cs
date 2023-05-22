using Microsoft.AspNetCore.Identity;

namespace Infomatrix.Models
{
    public class UsuarioAplicacion : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}
