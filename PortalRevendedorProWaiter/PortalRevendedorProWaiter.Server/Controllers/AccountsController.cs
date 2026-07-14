using PortalRevendedorProWaiter.Server.Util.Email;
using PortalRevendedorProWaiter.Shared;
using PortalRevendedorProWaiter.Shared.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PortalRevendedorProWaiter.Server.Data;
using PortalRevendedorProWaiter.Shared.Model;
using Microsoft.EntityFrameworkCore;
using PortalRevendedorProWaiter.Shared.ViewModels;
using System.Net.Http;

namespace PortalRevendedorProWaiter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IGestorEmails _gEmails;
        private readonly ApplicationDbContext _context;

        public AccountsController(UserManager<IdentityUser> userManager, ApplicationDbContext context, IGestorEmails gEmails)
        {
            _userManager = userManager;
            _gEmails = gEmails;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<List<UsuarioIndexViewModel>> Get()
        {
            List<IdentityUser> usuarios = _context.Users.ToList();
            List<UsuarioIndexViewModel> usuariosParaExibicao = new List<UsuarioIndexViewModel>();

            foreach (var usr in usuarios)
            {
                Revendedor rev = _context.UsuariosDoRevendedor
                    .Where(u => u.AspNetUserId == usr.Id).SingleOrDefault()?.Revendedor;

                var usuarioIndexVm = new UsuarioIndexViewModel()
                {
                    Email = usr.Email,
                    Id = usr.Id,
                    RazaoSocialRevendedor = rev?.RazaoSocial
                };

                usuariosParaExibicao.Add(usuarioIndexVm);
            }
            return usuariosParaExibicao;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<ActionResult<UsuarioEditViewModel>> Get(string id)
        {
            try
            {
                IdentityUser identityUser = await _context.Users.FindAsync(id);
                if (identityUser == null)
                {
                    return BadRequest("Usuário não encontrado");
                }

                UsuarioDoRevendedor usuarioDoRevendedor = await _context.UsuariosDoRevendedor.Where(u => u.AspNetUserId == id).SingleOrDefaultAsync();
                if (usuarioDoRevendedor == null)
                {
                    return BadRequest("Revendedor não encotrado");
                }

                UsuarioEditViewModel usuarioEditViewModel = new UsuarioEditViewModel()
                {
                    CodRevendedor = usuarioDoRevendedor?.CodRevendedor.ToString(),
                    Email = identityUser.Email,
                    Id = id
                };

                return usuarioEditViewModel;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }




        [HttpPost]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<IActionResult> Post([FromBody]RegisterModel model)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                var newUser = new IdentityUser { UserName = model.Email, Email = model.Email };
                try
                {
                    var resultUser = await _userManager.CreateAsync(newUser, model.Password);
                    if (!resultUser.Succeeded)
                    {
                        var errors = resultUser.Errors.Select(x => x.Description);
                        return Ok(new DefaultRequestResult { Successful = false, Errors = errors });
                    }

                    var resultRole = await _userManager.AddToRoleAsync(newUser, AppRolesConstants.Revendedor);
                    if (!resultRole.Succeeded)
                    {
                        var errors = resultRole.Errors.Select(x => x.Description);
                        return Ok(new DefaultRequestResult { Successful = false, Errors = errors });
                    }

                    UsuarioDoRevendedor usrR = new UsuarioDoRevendedor()
                    {
                        AspNetUserId = newUser.Id,
                        CodRevendedor = int.Parse(model.CodRevendedor)
                    };

                    _context.Attach(usrR);
                    await _context.SaveChangesAsync();

                    trans.Commit();
                    await _gEmails.EnviarEmailConfirmarEmail(model.Email, Url, Request);
                    return Ok(new DefaultRequestResult { Successful = true });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    var errors = new List<string> { ex.ToString() };
                    return BadRequest(new DefaultRequestResult() { Errors = errors, Successful = false });
                }
            }
        }

        [HttpPut]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<IActionResult> Put([FromBody]UsuarioEditViewModel model)
        {
            try
            {
                IdentityUser identityUser = await _context.Users.FindAsync(model.Id);
                if (identityUser == null)
                {
                    return Ok(new DefaultRequestResult() { Errors = new List<string> { "Usuário não encontrado" }, Successful = false });
                }

                if (identityUser.Email != model.Email)
                {
                    return Ok(new DefaultRequestResult() { Errors = new List<string> { "Email não está igual ao cadastro" }, Successful = false });
                }

                UsuarioDoRevendedor usuarioDoRevendedor = await _context.UsuariosDoRevendedor.Where(u => u.AspNetUserId == model.Id).SingleOrDefaultAsync();
                if (usuarioDoRevendedor == null)
                {
                    return Ok(new DefaultRequestResult() { Errors = new List<string> { "Revendedor do cadastro atual não encontrado" }, Successful = false });
                }

                Revendedor rev = _context.Revendedores.Find(int.Parse(model.CodRevendedor));
                if (rev == null)
                {
                    return Ok(new DefaultRequestResult() { Errors = new List<string> { "Revendedor não encontrado" }, Successful = false });
                }

                usuarioDoRevendedor.CodRevendedor = int.Parse(model.CodRevendedor);
                _context.Entry(usuarioDoRevendedor).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new DefaultRequestResult() { Successful = true });

            }
            catch (Exception ex)
            {
                return Ok(new DefaultRequestResult() { Errors = new List<string> { ex.ToString() }, Successful = false });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                IdentityUser identityUser = await _context.Users.FindAsync(id);
                if (identityUser == null)
                {
                    return BadRequest("Usuário não encontrado");
                }

                UsuarioDoRevendedor usuarioDoRevendedor = await _context.UsuariosDoRevendedor.Where(u => u.AspNetUserId == id).SingleOrDefaultAsync();

                if (usuarioDoRevendedor != null)
                    _context.UsuariosDoRevendedor.Remove(usuarioDoRevendedor);
                await _context.SaveChangesAsync();

                _context.Remove(identityUser);
                await _context.SaveChangesAsync();
                

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
