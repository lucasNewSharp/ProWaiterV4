using PortalRevendedorProWaiter.Shared.Atributos;
using System.ComponentModel.DataAnnotations;

namespace PortalRevendedorProWaiter.Shared.IdentityModel
{
    public class LoginModel
    {
        [CampoRequeridoObrigatorio]
        public string Email { get; set; }
        [CampoRequeridoObrigatorio]
        public string Password { get; set; }
    }
}
