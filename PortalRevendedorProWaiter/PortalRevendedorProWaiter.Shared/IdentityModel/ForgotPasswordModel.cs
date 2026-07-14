using PortalRevendedorProWaiter.Shared.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.IdentityModel
{
    public class ForgotPasswordModel
    {
        [CampoRequeridoObrigatorio]
        public string Email { get; set; }
    }
}
