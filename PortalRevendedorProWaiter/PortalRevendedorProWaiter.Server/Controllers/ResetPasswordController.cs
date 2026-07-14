using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortalRevendedorProWaiter.Shared;
using PortalRevendedorProWaiter.Shared.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PortalRevendedorProWaiter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ResetPasswordController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                List<string> errors = new List<string>();

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
                if (user == null)
                {
                    errors.Add("Usuário não cadastrado");
                    return Ok(new DefaultRequestResult() { Errors = errors, Successful = false });
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Code, resetPasswordModel.Password);
                if (!result.Succeeded)
                {
                    return Ok(new DefaultRequestResult() { Errors = result.Errors.Select(e => e.Description).ToList(), Successful = false });
                }

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