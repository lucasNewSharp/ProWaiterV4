using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalRevendedorProWaiter.Server.Data;
using PortalRevendedorProWaiter.Shared;
using PortalRevendedorProWaiter.Shared.Model;
using PortalRevendedorProWaiter.Shared.ViewModels;

namespace PortalRevendedorProWaiter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicencasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LicencasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Licencas
        [HttpGet]
        [Authorize(Roles = AppRolesConstants.AdministradorRevendor)]
        public async Task<ActionResult<LicencaIndexViewModel[]>> GetLicencas()
        {
            List<Licenca> licencasBD = new List<Licenca>();
            if (User.IsInRole(AppRolesConstants.Administrador))
                licencasBD = await _context.Licencas.ToListAsync();
            else
            {
                string userId = _userManager.GetUserId(User);
                //Obtemos todas as licenças do revendedor logado
                var usuarioDoRevendedor = await _context.UsuariosDoRevendedor.SingleOrDefaultAsync(u => u.AspNetUserId == userId);

                if (usuarioDoRevendedor == null)
                    return BadRequest("Revendedor não encontrado para o usuário");

                licencasBD = await _context.Licencas.Where(l => l.CodRevendedor == usuarioDoRevendedor.CodRevendedor).ToListAsync();
            }

            var licencasViewModel = new List<LicencaIndexViewModel>();

            licencasBD.OrderBy(l => l.Nome).ToList().ForEach(l => licencasViewModel.Add(InstanciarLicencaIndexViewModel(l)));

            return licencasViewModel.ToArray();

        }

        private LicencaIndexViewModel InstanciarLicencaIndexViewModel(Licenca l)
        {
            var lic = new LicencaIndexViewModel()
            {
                Ativo = l.Ativo,
                Codigo = l.Codigo,
                Nome = l.Nome,
                Segredo = l.Segredo,
                VersaoAPP = l.VersaoAPP,
                VersaoProWaiter = l.VersaoProWaiter,
                Revendedor = l.Revendedor?.RazaoSocial
            };
            return lic;
        }

        // GET: api/Licencas/5
        [HttpGet("{id}")]
        [Authorize(Roles = AppRolesConstants.AdministradorRevendor)]
        public async Task<ActionResult<LicencaDetailsViewModel>> GetLicenca(int id)
        {
            var licenca = await _context.Licencas.FindAsync(id);

            if (licenca == null)
            {
                return NotFound();
            }

            LicencaDetailsViewModel details = new LicencaDetailsViewModel()
            {
                Ativo = licenca.Ativo,
                Nome = licenca.Nome,
                Cidade = licenca.Cidade + "-" + licenca.UF,
                Codigo = licenca.Codigo,
                DataAtivacao = licenca.DataAtivacao,
                DataValidacao = licenca.DataValidacao,
                Endereco = licenca.Endereco,
                ProcessorID = licenca.ProcessorID,
                QuantidadeAtivacoes = licenca.QuantidadeAtivacoes,
                Segredo = licenca.Segredo,
                VersaoAPP = licenca.VersaoAPP,
                VersaoProWaiter = licenca.VersaoProWaiter,
                Revendedor = licenca.Revendedor?.RazaoSocial
            };

            return details;
        }

        [HttpPut()]
        [Route("[action]")]
        [Authorize(Roles = AppRolesConstants.AdministradorRevendor)]
        public async Task<IActionResult> AtualizarLicencaAtiva([FromBody] LicencaDetailsViewModel lic)
        {
            var licenca = await _context.Licencas.FindAsync(lic.Codigo);

            if (licenca == null)
            {
                return NotFound();
            }

            licenca.Ativo = lic.Ativo;
            _context.Entry(licenca).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut()]
        [Route("[action]")]
        [Authorize(Roles = AppRolesConstants.AdministradorRevendor)]
        public async Task<ActionResult<LicencaIndexViewModel>> CriarLicenca()
        {
            Revendedor revendedor = null;
            if (User.IsInRole(AppRolesConstants.Revendedor))            
            {
                string userId = _userManager.GetUserId(User);
                //Obtemos todas as licenças do revendedor logado
                var usuarioDoRevendedor = await _context.UsuariosDoRevendedor.SingleOrDefaultAsync(u => u.AspNetUserId == userId);

                if (usuarioDoRevendedor == null)
                    return BadRequest("Revendedor não encontrado para o usuário");

                revendedor = usuarioDoRevendedor.Revendedor;
            }

            Licenca licenca = new Licenca()
            {
                Ativo = true,
                CodRevendedor = revendedor?.Codigo,
                QuantidadeAtivacoes = 0,
                Segredo = Guid.NewGuid().ToString().Replace("-", "")
            };

            _context.Licencas.Add(licenca);
            await _context.SaveChangesAsync();

            LicencaIndexViewModel licVM = InstanciarLicencaIndexViewModel(licenca);
            return licVM;
        }       

        // DELETE: api/Licencas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRolesConstants.AdministradorRevendor)]
        public async Task<IActionResult> DeleteLicenca(int id)
        {
            var licenca = await _context.Licencas.FindAsync(id);
            if (licenca == null)
            {
                return NotFound();
            }

            _context.Licencas.Remove(licenca);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
