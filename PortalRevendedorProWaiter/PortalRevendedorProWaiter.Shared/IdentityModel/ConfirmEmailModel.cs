using System;
using System.Collections.Generic;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.IdentityModel
{
    public class ConfirmEmailModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }
}
