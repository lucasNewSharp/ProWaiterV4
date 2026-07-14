using NewSharp.Ferramentas;
using Newtonsoft.Json;
using ProWaiter.Licenca.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations.Sql;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Licenca.Gestores
{
    public class GestorLicencas
    {
        private LicencasContext _context = null;

        public GestorLicencas(LicencasContext context)
        {
            _context = context;
        }

        public LicencaProWaiter ObterLicenca()
        {
            return _context.Licencas.FirstOrDefault();
        }

        public bool LicencaExiste()
        {
            return _context.Licencas.FirstOrDefault() != null;
        }

        public bool AtivarLicenca(LicencaDTO licencaDTO, out string msg)
        {
            msg = string.Empty;
            string retorno = Validar(licencaDTO);
            long validacao;
            if (string.IsNullOrWhiteSpace(retorno))
            {
                return false;
            }
            else
            {
                if (!long.TryParse(retorno.Replace("\"", ""), out validacao))
                {
                    msg = retorno;
                    return false;
                }
            }

            LicencaProWaiter novaLicenca = new LicencaProWaiter()
            {
                Nome = licencaDTO.Nome,
                Endereco = licencaDTO.Endereco,
                Cidade = licencaDTO.Cidade,
                Segredo = licencaDTO.Segredo,
                UF = licencaDTO.UF,
                DataAtivacao = DateTime.Today,
                Validacao = validacao,
                Ativo = true
            };

            _context.Licencas.ToList().ForEach(l => _context.Licencas.Remove(l));
            _context.Licencas.Add(novaLicenca);
            _context.SaveChanges();

            return true;
        }

        public bool ValidarLicenca(out string msg)
        {
            msg = string.Empty;
            string retorno = Validar();
            long validacao;
            LicencaProWaiter lic = _context.Licencas.First();
            if (string.IsNullOrWhiteSpace(retorno))
            {
                return false;
            }
            else
            {
                if (!long.TryParse(retorno.Replace("\"", ""), out validacao))
                {
                    msg = retorno;
                    lic.Ativo = false;
                }
                else
                {
                    lic.Validacao = validacao;
                    lic.Ativo = true;
                }
            }
            _context.SaveChanges();
            return true;
        }

        private string Validar(LicencaDTO licencaDTO = null)
        {
            try
            {
                Task<string> ws = RequestValidacao(licencaDTO);
                ws.Wait();
                return ws.Result;
            }
            catch
            {
                throw;
            }
        }

        private async Task<string> RequestValidacao(LicencaDTO licencaDTO = null)
        {
            //Para o azure tem que ter o TSL1.2
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;  

            LicencaProWaiter licencaBD = _context.Licencas.FirstOrDefault();
            LicencaRequest dadosRequest = InstanciarLicencaRequest(licencaBD, licencaDTO);
            Configuracao enderecoValidador = _context.Configuracoes.Where(c => c.Codigo == Configuracao.CodValidador).Single();
            HttpClient httpClient = new HttpClient();            
            httpClient.BaseAddress = new Uri(enderecoValidador.Valor);

            string retorno = string.Empty;
            var objJSon = JsonConvert.SerializeObject(dadosRequest);
            var conteudoJSon = new StringContent(objJSon, Encoding.UTF8, "application/json");

            string url = "Validacao?processorID=" + Validadores.ObterCPUId();
            HttpResponseMessage response = await httpClient.PostAsync(url, conteudoJSon).ConfigureAwait(continueOnCapturedContext: false);

            if (response.IsSuccessStatusCode)
                retorno = await response.Content.ReadAsStringAsync();
            else
                throw new HttpRequestException(response.StatusCode.ToString());

            return retorno;
        }

        private LicencaRequest InstanciarLicencaRequest(LicencaProWaiter licencaBD, LicencaDTO licencaDTO)
        {
            Configuracao versaoSistema = _context.Configuracoes.Where(c => c.Codigo == Configuracao.CodVersao).Single();

            string versaoAPP = "";
            using (StreamReader sr = File.OpenText(@"C:\inetpub\wwwroot\ProWaiter\ProWaiterAPK\Versao.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                VersaoAPP verApp = (VersaoAPP)serializer.Deserialize(sr, typeof(VersaoAPP));
                versaoAPP = verApp.VersionCode;
            }

            LicencaRequest dadosRequest = new LicencaRequest();
            if (licencaDTO != null)
            {
                dadosRequest = new LicencaRequest()
                {
                    Nome = licencaDTO.Nome,
                    Endereco = licencaDTO.Endereco,
                    Cidade = licencaDTO.Cidade,
                    UF = licencaDTO.UF,
                    Segredo = licencaDTO.Segredo,
                    DataAtivacao = DateTime.Today,
                    Validacao = 0
                };
            }
            else
            {
                dadosRequest = new LicencaRequest()
                {
                    Nome = licencaBD.Nome,
                    Endereco = licencaBD.Endereco,
                    Cidade = licencaBD.Cidade,
                    Segredo = licencaBD.Segredo,
                    UF = licencaBD.UF,
                    DataAtivacao = licencaBD.DataAtivacao,
                    Validacao = licencaBD.Validacao
                };
            }

            dadosRequest.VersaoAPP = versaoAPP;
            dadosRequest.VersaoProWaiter = versaoSistema.Valor;
            return dadosRequest;
        }

        [Serializable]
        class LicencaRequest
        {
            public string Nome { get; set; }
            public string Endereco { get; set; }
            public string Cidade { get; set; }
            public string UF { get; set; }
            public string Segredo { get; set; }
            public DateTime? DataAtivacao { get; set; }
            public long Validacao { get; set; }
            public string VersaoProWaiter { get; set; }
            public string VersaoAPP { get; set; }
        }
    }
}
