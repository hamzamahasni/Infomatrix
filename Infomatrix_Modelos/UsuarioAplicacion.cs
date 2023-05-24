using Microsoft.AspNetCore.Identity;

namespace Infomatrix_Modelos
{
    public class UsuarioAplicacion : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}
