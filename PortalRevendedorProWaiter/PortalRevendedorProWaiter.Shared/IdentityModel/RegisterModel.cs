using PortalRevendedorProWaiter.Shared.Atributos;
using System.ComponentModel.DataAnnotations;

namespace PortalRevendedorProWaiter.Shared.IdentityModel
{
    public class RegisterModel
    {
        public string Id { get; set; }

        [CampoRequeridoObrigatorio]
        [EmailAddress(ErrorMessage = "O email é inválido")]
        public string Email { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(100, ErrorMessage = ConstantesAtributosEntidades.TamanhoMinimoMaximo, MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        [Compare("Password", ErrorMessage = "A senha e a confirmação de senha devem ser iguais")]
        public string ConfirmPassword { get; set; }

        [CampoRequeridoObrigatorio]
        [Display(Name = "Revendedor")]
        public string CodRevendedor { get; set; }        
    }
}
