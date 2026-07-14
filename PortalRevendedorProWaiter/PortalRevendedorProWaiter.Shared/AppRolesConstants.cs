using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalRevendedorProWaiter.Shared
{
    public class AppRolesConstants
    {
        public const string Administrador = "Administrador";
        public const string Revendedor = "Revendedor";
        public const string AdministradorRevendor = Administrador + "," + Revendedor;
    }
}
