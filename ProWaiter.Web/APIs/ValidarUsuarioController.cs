using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ProWaiter.Web.Models;

namespace ProWaiter.Web.APIs
{
    public class ValidarUsuarioController : ControllerBase
    {
        // GET: api/ValidarUsuario
        public async Task<bool> Get()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return false;
            
            string parametros = Request.Headers["Authorization"].ToString().Replace("Basic ", "");
            var authHeaderValue = Encoding.GetEncoding("ISO-8859-1").GetString(Convert.FromBase64String(parametros));
            string[] valores = authHeaderValue.Split(':');

            valores[0] = RemoverCaracteresScape(valores[0]);
            valores[1] = RemoverCaracteresScape(valores[1]);

            Microsoft.AspNetCore.Identity.SignInManager<ApplicationUser> sm = null;            

            var result = await sm.PasswordSignInAsync(valores[0], valores[1], true, lockoutOnFailure: false);
            if (result.Succeeded) return true;
            return false;
        }     

        private string RemoverCaracteresScape(string valor)
        {
            return valor.Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }
    }
}
