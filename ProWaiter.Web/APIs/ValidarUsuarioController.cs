using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;

namespace ProWaiter.Web.APIs
{
    public class ValidarUsuarioController : ApiController
    {
        // GET: api/ValidarUsuario
        public async Task<bool> Get()
        {
            if (Request.Headers.Authorization == null)
                return false;
            string parametros = Request.Headers.Authorization.Parameter.ToString();
            var authHeaderValue = Encoding.GetEncoding("ISO-8859-1").GetString(Convert.FromBase64String(parametros));
            string[] valores = authHeaderValue.Split(':');

            valores[0] = RemoverCaracteresScape(valores[0]);
            valores[1] = RemoverCaracteresScape(valores[1]);

            ApplicationSignInManager sm = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationSignInManager>();            

            var result = await sm.PasswordSignInAsync(valores[0], valores[1], true, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return true;                                
                case SignInStatus.Failure:
                    return false;                
            }

            return false;
        }     

        private string RemoverCaracteresScape(string valor)
        {
            return valor.Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }
    }
}
