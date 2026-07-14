using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.Atributos
{
    public class CampoRequeridoObrigatorioAttribute : RequiredAttribute
    {
        public CampoRequeridoObrigatorioAttribute()
        {
            ErrorMessage = ConstantesAtributosEntidades.CampoObrigatorio;
        }
    }
}
