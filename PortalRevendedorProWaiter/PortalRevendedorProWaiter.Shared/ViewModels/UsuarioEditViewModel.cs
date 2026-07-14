using PortalRevendedorProWaiter.Shared.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.ViewModels
{
    public class UsuarioEditViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        [CampoRequeridoObrigatorio]
        [Display(Name = "Revendedor")]
        public string CodRevendedor { get; set; }
    }
}
