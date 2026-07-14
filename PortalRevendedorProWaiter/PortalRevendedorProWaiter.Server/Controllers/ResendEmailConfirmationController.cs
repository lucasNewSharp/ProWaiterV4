using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortalRevendedorProWaiter.Server.Util.Email;
using PortalRevendedorProWaiter.Shared;
using PortalRevendedorProWaiter.Shared.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PortalRevendedorProWaiter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResendEmailConfirmationController : ControllerBase
    {
        private readonly IGestorEmails _gEmails;

        public ResendEmailConfirmationController(IGestorEmails gEmails)
        {
            _gEmails = gEmails;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]LoginModel model)
        {
            try
            {
                if (model.Email == null)
                {
                    var erros = new List<string>
                {
                    "Email nulo"
                };
                    return Ok(new DefaultRequestResult() { Errors = erros, Successful = false });
                }

                await _gEmails.EnviarEmailConfirmarEmail(model.Email, Url, Request);
                return Ok(new DefaultRequestResult() { Successful = true });
            }
            catch (Exception ex)
            {
                var errors = new List<string> { ex.ToString() };
                return Ok(new DefaultRequestResult() { Errors = errors, Successful = false });
            }
        }
    }
}