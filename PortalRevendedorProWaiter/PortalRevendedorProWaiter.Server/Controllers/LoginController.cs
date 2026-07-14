using PortalRevendedorProWaiter.Shared.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using PortalRevendedorProWaiter.Shared.Model;
using PortalRevendedorProWaiter.Server.Data;
using System.Linq;
using PortalRevendedorProWaiter.Shared;

namespace PortalRevendedorProWaiter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _contexto;

        public LoginController(IConfiguration configuration,
                               SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager,
                               ApplicationDbContext contexto)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _contexto = contexto;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            try
            {
                var userTask = _userManager.FindByEmailAsync(login.Email);

                if (userTask.Result == null)
                {
                    return Ok(new LoginResult() { Error = "Usuário não cadastrado", Successful = false, EmailConfirmed = true });
                }

                var user = userTask.Result;

                bool ehAdmin = await _userManager.IsInRoleAsync(user, AppRolesConstants.Administrador);

                if (!ehAdmin)
                {
                    Revendedor revendedor = _contexto.UsuariosDoRevendedor.Where(u => u.AspNetUserId == user.Id).SingleOrDefault()?.Revendedor;

                    if(revendedor == null)
                    {
                        return Ok(new LoginResult() { Error = "Usuário não é administrador e não possui um revendedor vinculado " + user.Id, Successful = false, EmailConfirmed = true });
                    }

                    if (!revendedor.Ativo)
                    {
                        string msg = "O revendedor " + revendedor.RazaoSocial + " está inativo no sistema, consulte os administradores.";
                        return Ok(new LoginResult() { Error = msg, Successful = false, EmailConfirmed = true });
                    }
                }

                bool emailConfirmd = await _userManager.IsEmailConfirmedAsync(user);

                if (!emailConfirmd)
                {
                    return Ok(new LoginResult() { Successful = false, EmailConfirmed = false });
                }

                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

                if (!result.Succeeded)
                {
                    return Ok(new LoginResult { Successful = false, Error = "Usuário ou senha inválidos", EmailConfirmed = true });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                var roles = await _signInManager.UserManager.GetRolesAsync(user);

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

                var token = new JwtSecurityToken(
                    _configuration["JwtIssuer"],
                    _configuration["JwtAudience"],
                    claims,
                    expires: null,
                    signingCredentials: creds
                );

                return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token), EmailConfirmed = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new LoginResult { Successful = false, EmailConfirmed = true, Error = ex.ToString() });
            }
        }
    }
}
