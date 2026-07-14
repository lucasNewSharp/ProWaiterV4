using NewSharp.Ferramentas;
using Newtonsoft.Json;
using ProWaiter.Web.Controllers;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace ProWaiter.Web.Util
{
    [Serializable]
    class RestauranteDTO
    {
        public RestauranteDTO(Restaurante restaurante)
        {
            Nome = restaurante.Nome;
            Endereco = restaurante.Endereco;
            Cidade = restaurante.Cidade;
            UF = restaurante.UF;
            Segredo = restaurante.Segredo;
            DataAtivacao = restaurante.DataAtivacao;
            Validacao = restaurante.Validacao;
        }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(100)]
        public string Endereco { get; set; }

        [Required]
        [StringLength(100)]
        public string Cidade { get; set; }

        [Required]
        [StringLength(2)]
        public string UF { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string Segredo { get; set; }

        public DateTime? DataAtivacao { get; set; }

        public long Validacao { get; set; }

        public string VersaoProWaiter { get; set; }
        public string VersaoAPP { get; set; }
    }

    public class ValidadorActionFilterAttribute : ActionFilterAttribute, IActionFilter
    {
        //private string SemInternet = "SemInternet";                

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if ((filterContext.Controller is PedidosController))
            //{
            //    ProWaiterContext context = new ProWaiterContext();
            //    Licenca lic = context.Licencas.FirstOrDefault();

            //    if (lic == null)
            //    {
            //        throw new HttpException("Não existe licença configurada no sistema, contate o revendedor");
            //    }

            //    if (!lic.Ativo)
            //    {
            //        throw new HttpException("Licença inativada, contate o revendedor");
            //    }

            //    Process[] processes = Process.GetProcessesByName("ProWaiter.Licenca");
            //    if (processes.Length == 0)
            //    {
            //        throw new HttpException("Validador da licença não está ativo");
            //    }

            //    double dias = (DateTime.UtcNow.Date - DateTime.FromFileTimeUtc(lic.Validacao)).TotalDays;
            //    if (dias > 45)
            //    {
            //        throw new ApplicationException($"A validação expirou, você está {dias} sem validar a sua licença");
            //    }
            //}
            base.OnActionExecuting(filterContext);
        }

        /*public override void OnActionExecuting(ActionExecutingContext filterContext)
        {        
            if (!(filterContext.Controller is RestauranteController))
            {
                Restaurante restaurante = Startup.Restaurante;
                if (Startup.Restaurante == null)
                {
                    RestauranteController rc = new RestauranteController();
                    Startup.Restaurante = rc.RecuperarRestaurante();
                    if (Startup.Restaurante == null)
                        filterContext.Result = new RedirectResult("~/Restaurante/Create");
                }
                else //tenho o restaurante
                {
                    double dias = Math.Abs((DateTime.UtcNow.Date - DateTime.FromFileTimeUtc(restaurante.Validacao)).TotalDays);
                    if (dias > 0 && diaTentativa != DateTime.Today.Date.Day)
                    {                        
                        diaTentativa = DateTime.Now.Day;
                        if (restaurante.Validacao != 0 && dias > 45)
                            throw new HttpException(Startup.MsgValicadaoExpirou);

                        string validacao = ObterValidacao(restaurante);
                        long retornoWS = 0;
                        if (validacao == SemInternet)
                        {
                            if (restaurante.Validacao == 0)
                            {
                                filterContext.Result = new RedirectResult("~/Restaurante/ErroAtivacaoDias?dias=" + dias);
                                return;
                            }
                            if (dias > 30)
                                Startup.MsgResultadoValidacao = String.Format(" - " + Startup.MsgValidacao, Math.Abs(45 - dias));
                        }
                        else if (!long.TryParse(validacao.Replace("\"", ""), out retornoWS))
                        {
                            filterContext.Result = new RedirectResult("~/Restaurante/ErroAtivacao?erro=" + HttpUtility.UrlEncode(validacao));
                            return;
                        }
                        else
                        {
                            Startup.MsgResultadoValidacao = "";
                            restaurante.Validacao = retornoWS;
                            RestauranteController rc = new RestauranteController();
                            rc.GravarRestaurante(restaurante);
                        }
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }*/

        /*private string ObterValidacao(Restaurante restaurante)
        {
            try
            {
                Task<string> ws = Validar<string>(restaurante);
                ws.Wait();
                return ws.Result;
            }
            catch { return SemInternet; }
        }

        private async Task<string> Validar<T>(Restaurante restaurante)
        {
            Configuracoes config = Configuracoes.ObterInstancia();

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(config.Validador);

            RestauranteDTO restauranteDTO = new RestauranteDTO(restaurante);

            //Pegamos a versão do ProWaiter
            restauranteDTO.VersaoProWaiter = config.Versao;
            //Pegamos a versão do APP
            restauranteDTO.VersaoAPP = config.VersaoAPP;


            string retorno = string.Empty;
            var objJSon = JsonConvert.SerializeObject(restauranteDTO);
            var conteudoJSon = new StringContent(objJSon, Encoding.UTF8, "application/json");

            string url = "Validacao?processorID=" + Validadores.ObterCPUId();            
            HttpResponseMessage response = await httpClient.PostAsync(url, conteudoJSon).ConfigureAwait(continueOnCapturedContext: false);

            if (response.IsSuccessStatusCode)
                retorno = await response.Content.ReadAsStringAsync();
            else
                throw new HttpRequestException(response.StatusCode.ToString());

            return retorno;
        }*/

    }
}