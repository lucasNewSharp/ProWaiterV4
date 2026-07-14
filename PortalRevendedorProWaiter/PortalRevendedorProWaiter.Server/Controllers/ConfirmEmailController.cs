using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PortalRevendedorProWaiter.Shared;
using PortalRevendedorProWaiter.Shared.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PortalRevendedorProWaiter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmEmailController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmEmailController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> PostConfirmEmail([FromBody]ConfirmEmailModel model)
        {
            try
            {
                if (model.UserId == null || model.Code == null)
                {
                    var erros = new List<string> { "Usuário não encontrado" };
                    return Ok(new DefaultRequestResult() { Errors = erros, Successful = false });
                }

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    var erros = new List<string> { "Usuário não encontrado" };
                    return Ok(new DefaultRequestResult() { Errors = erros, Successful = false });
                }

                var result = await _userManager.ConfirmEmailAsync(user, model.Code);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return Ok(new DefaultRequestResult() { Errors = errors, Successful = false });
                }

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