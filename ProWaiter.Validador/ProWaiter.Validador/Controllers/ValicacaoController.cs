using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProWaiter.Validador;
using ProWaiter.Validador.Models;

namespace ProWaiter.Validador.Controllers
{
    [Produces("application/json")]
    [Route("api/Validacao")]
    public class ValidacaoController : Controller
    {
        private readonly ProWaiterValidadorContext _context;

        public ValidacaoController(ProWaiterValidadorContext context)
        {
            _context = context;
        }

        //Validacao
        public string PostLicenca([FromBody] Restaurante restaurante, string processorID)
        {
            if (!ModelState.IsValid)
                return "Erro ao tentar processar a ativação";

            Validacao licencaBD = _context.Licencas.Where(l => l.Segredo == restaurante.Segredo).FirstOrDefault();

            if (licencaBD == null) //não existe licença para o segredo
                return "Licença inválida";

            if (!licencaBD.Ativo) //Conta desativada
                return "Conta desativada, entre em contato com o revendedor";

            if (!licencaBD.DataValidacao.HasValue || restaurante.Validacao == 0) //Primeira ativação ou reativação
            {
                if (licencaBD.QuantidadeAtivacoes == 5)
                    return "Você já ativou o produto 5 vezes, entre em contato com o revendedor";

                licencaBD.Cidade = restaurante.Cidade;
                licencaBD.Endereco = restaurante.Endereco;
                licencaBD.Nome = restaurante.Nome;
                licencaBD.UF = restaurante.UF;
                licencaBD.DataAtivacao = DateTime.UtcNow.Date;
                licencaBD.ProcessorID = processorID;
                licencaBD.QuantidadeAtivacoes++;
            }
            else //Validação
            {
                if (licencaBD.DataValidacao.Value.AddDays(5) < DateTime.FromFileTimeUtc(restaurante.Validacao)) //Alguém tentou alterar a ultima data de validação no arquivo de licença no cliente
                    return "Data da última validação inválida";
                if (licencaBD.ProcessorID != processorID) //Licença valida para outro processador
                    return "Essa licença é valida para outro computador, entre em contato com o revendedor para efetuar a reativação do sistema";
            }

            if (!string.IsNullOrWhiteSpace(restaurante.VersaoProWaiter))
                licencaBD.VersaoProWaiter = restaurante.VersaoProWaiter.Trim();

            if (!string.IsNullOrWhiteSpace(restaurante.VersaoAPP))
                licencaBD.VersaoApp = restaurante.VersaoAPP.Trim();

            licencaBD.DataValidacao = DateTime.UtcNow.Date;
            _context.Entry(licencaBD).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return licencaBD.DataValidacao.Value.ToFileTimeUtc().ToString();
        }
    }
}