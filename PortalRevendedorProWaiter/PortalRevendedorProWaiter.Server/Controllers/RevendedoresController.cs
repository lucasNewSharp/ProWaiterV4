using Microsoft.EntityFrameworkCore;
using PortalRevendedorProWaiter.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PortalRevendedorProWaiter.Server.Data;
using PortalRevendedorProWaiter.Shared.ViewModels;
using PortalRevendedorProWaiter.Shared.Model;

namespace PortalRevendedorProWaiter.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class RevendedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RevendedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<ActionResult<List<RevendedorIndexViewModel>>> Get()
        {
            try
            {
                List<Revendedor> revendedoresBD = await _context.Revendedores.ToListAsync();
                List<RevendedorIndexViewModel> revendedoresVM = new List<RevendedorIndexViewModel>();

                revendedoresBD.ForEach(r => revendedoresVM.Add(new RevendedorIndexViewModel()
                {
                    Ativo = r.Ativo,
                    CNPJ = r.CNPJ,
                    Codigo = r.Codigo,
                    Endereco = r.Endereco,
                    RazaoSocial = r.RazaoSocial,
                    Responsavel = r.Responsavel,
                    Telefone1 = r.Telefone1,
                    Telefone2 = r.Telefone2
                }));

                return revendedoresVM;
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
      
        [HttpGet("{id}", Name = "GetById")]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<ActionResult<RevendedorCrudViewModel>> GetById(int id)
        {
            try
            {
                Revendedor revendedoresBD = await _context.Revendedores.FindAsync(id);

                if(revendedoresBD == null)
                {
                    ModelState.AddModelError("", "Revendedor não encontrado");
                    return NotFound();
                }

                RevendedorCrudViewModel revVM = new RevendedorCrudViewModel(revendedoresBD);
                return revVM;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<ActionResult> Post([FromBody]RevendedorCrudViewModel viewModel)
        {
            try
            {
                Revendedor revendedor = new Revendedor();
                AtualizarRevendedorComModel(revendedor, viewModel);

                _context.Add(revendedor);
                await _context.SaveChangesAsync();
                return Ok(new DefaultRequestResult { Successful = true });
            }
            catch(Exception ex)
            {
                return BadRequest(new DefaultRequestResult() { Errors = new List<string> { ex.ToString() }, Successful = false });
            }
        }

        [HttpPut]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<ActionResult> Put(RevendedorCrudViewModel viewModel)
        {
            try
            {
                Revendedor revBd = await _context.Revendedores.FindAsync(viewModel.Codigo);
                if (revBd == null)
                {
                    ModelState.AddModelError("", "Revendedor não encontrado");
                    return NotFound();
                }
                AtualizarRevendedorComModel(revBd, viewModel);
                _context.Update(revBd);
                await _context.SaveChangesAsync();
                return Ok(new DefaultRequestResult { Successful = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultRequestResult() { Errors = new List<string> { ex.ToString() }, Successful = false });
            }
        }

        private void AtualizarRevendedorComModel(Revendedor rev, RevendedorCrudViewModel vm)
        {
            rev.Ativo = vm.Ativo;
            rev.CNPJ = vm.CNPJ;
            rev.Endereco = vm.Endereco;
            rev.RazaoSocial = vm.RazaoSocial;
            rev.Responsavel = vm.Responsavel;
            rev.Telefone1 = vm.Telefone1;
            rev.Telefone2 = vm.Telefone2;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRolesConstants.Administrador)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Revendedor revBd = await _context.Revendedores.FindAsync(id);
                if (revBd == null)
                {
                    ModelState.AddModelError("", "Revendedor não encontrado");
                    return NotFound();
                }

                _context.Remove(revBd);
                await _context.SaveChangesAsync();

                return Ok(new DefaultRequestResult { Successful = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultRequestResult() { Errors = new List<string> { ex.ToString() }, Successful = false });
            }
        }
    }
}