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
    public class ForgotPasswordController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IGestorEmails _gEmails;

        public ForgotPasswordController(UserManager<IdentityUser> userManager, IGestorEmails gEmails)
        {
            _userManager = userManager;
            _gEmails = gEmails;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ForgotPasswordModel forgotPasswordModel)
        {
            try
            {
                List<string> errors = new List<string>();
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                if (user == null)
                {
                    errors.Add("Usuário não cadastrado");
                    return Ok(new DefaultRequestResult() { Errors = errors, Successful = false });
                }

                if (!user.EmailConfirmed)
                {
                    errors.Add("Você ainda não confirmou o seu e-mail");
                    return Ok(new DefaultRequestResult() { Errors = errors, Successful = false });
                }

                await _gEmails.EnviarEmailEsqueciMinhaSenha(forgotPasswordModel.Email, Url, Request);
                return Ok(new DefaultRequestResult() { Successful = true });
            }
            catch (Exception ex)
            {
                var errors = new List<string> { ex.ToString() };
                return BadRequest(new DefaultRequestResult() { Errors = errors, Successful = false });
            }
        }
    }
}