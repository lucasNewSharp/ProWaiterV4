using PortalRevendedorProWaiter.Shared.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.IdentityModel
{
    public class ResetPasswordModel
    {
        [CampoRequeridoObrigatorio]
        [EmailAddress(ErrorMessage = "O email é inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(100, ErrorMessage = "A {0} deve conter pelo menos {2} e no máximo {1} caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        [Compare("Password", ErrorMessage = "A senha e a confirmação de senha devem ser iguais")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
