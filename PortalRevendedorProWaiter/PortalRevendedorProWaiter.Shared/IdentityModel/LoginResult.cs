using System;

namespace PortalRevendedorProWaiter.Shared.IdentityModel
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime Expiry { get; set; }
    }
}
