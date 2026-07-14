/* 
using Microsoft.EntityFrameworkCore;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ProWaiter.Web.Controllers
{
    public class RelatoriosController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult RefeicoesMaisVendidas(DateTime? dataInicial = null, DateTime? dataFinal = null, string unificarRefeicoes = "")
        {
            TratarParametros("RelatorioRefeicoesMaisVendidas", ref dataInicial, ref dataFinal, ref unificarRefeicoes);

            List<RefeicaoMaisVendidaDTO> refeicoes = new List<RefeicaoMaisVendidaDTO>();

            IDbConnection con = new SqlConnection(db.Database.Connection.ConnectionString);
            con.Open();
            try
            {
                string sql = null;
                if (unificarRefeicoes == "on")
                    sql = @"SELECT TBTipos.Nome AS Tipo, TBRefs.Nome AS Refeicao, '' AS Tamanho, COUNT(TBAtrRefPed.Codigo) AS Quantidade
                                FROM TBRefeicoes AS TBRefs INNER JOIN
                                                         TBRefeicoesCardapio AS TBRefCard ON TBRefs.Codigo = TBRefCard.CodRefeicao INNER JOIN
                                                         TBTiposRefeicao AS TBTipos ON TBRefs.CodTipo = TBTipos.Codigo LEFT OUTER JOIN
                                                         TBPedidos AS TBPeds INNER JOIN
                                                         TBAtribRefeicoesPedido AS TBAtrRefPed ON TBPeds.Codigo = TBAtrRefPed.CodPedido ON TBRefCard.CodRefeicao = TBAtrRefPed.CodRefeicao AND TBRefCard.CodTamanho = TBAtrRefPed.CodTamanho
                                WHERE (TBPeds.DataInicio BETWEEN @dataInicial AND @dataFinal)
                                GROUP BY TBTipos.Nome, TBRefs.Nome
                                ORDER BY Quantidade DESC, Tipo, Refeicao";
                else
                    sql = @"SELECT TBTipos.Nome AS Tipo, TBRefs.Nome AS Refeicao, TBTams.Nome AS Tamanho, COUNT(TBAtrRefPed.Codigo) AS Quantidade
                                FROM TBRefeicoes AS TBRefs INNER JOIN
                                                         TBRefeicoesCardapio AS TBRefCard INNER JOIN
                                                         TBTamanhosRefeicao AS TBTams ON TBRefCard.CodTamanho = TBTams.Codigo ON TBRefs.Codigo = TBRefCard.CodRefeicao INNER JOIN
                                                         TBTiposRefeicao AS TBTipos ON TBRefs.CodTipo = TBTipos.Codigo LEFT OUTER JOIN
                                                         TBPedidos AS TBPeds INNER JOIN
                                                         TBAtribRefeicoesPedido AS TBAtrRefPed ON TBPeds.Codigo = TBAtrRefPed.CodPedido ON TBRefCard.CodRefeicao = TBAtrRefPed.CodRefeicao AND TBRefCard.CodTamanho = TBAtrRefPed.CodTamanho
                                WHERE (TBPeds.DataInicio BETWEEN @dataInicial AND @dataFinal)
                                GROUP BY TBTipos.Nome, TBRefs.Nome, TBTams.Nome
                                ORDER BY Quantidade DESC, Tipo, Refeicao, Tamanho";
                IDbCommand cmd = new SqlCommand(sql, (SqlConnection)con);
                cmd.Parameters.Add(new SqlParameter("@dataInicial", dataInicial));
                cmd.Parameters.Add(new SqlParameter("@dataFinal", dataFinal.Value.AddDays(1)));

                int rank = 1, qtdTotal = 0;
                using (IDataReader dr = cmd.ExecuteReader())
                    while (dr.Read())
                    {
                        RefeicaoMaisVendidaDTO refeicao = new RefeicaoMaisVendidaDTO
                        {
                            Rank = rank++,
                            Tipo = (string)dr["Tipo"],
                            Refeicao = (string)dr["Refeicao"],
                            Quantidade = (int)dr["Quantidade"],
                            Tamanho = (string)dr["Tamanho"],
                            ClasseCSS = "linDado"
                        };
                        qtdTotal += refeicao.Quantidade;
                        refeicoes.Add(refeicao);
                    }
                refeicoes.Add(new RefeicaoMaisVendidaDTO
                {
                    Tamanho = "TOTAL",
                    Quantidade = qtdTotal,
                    ClasseCSS = "linTotal"
                });
            }
            finally
            {
                con.Close();
            }

            ViewBag.DataInicial = dataInicial.Value.ToString("yyyy-MM-dd");
            ViewBag.DataFinal = dataFinal.Value.ToString("yyyy-MM-dd");
            ViewBag.UnificarRefeicoes = unificarRefeicoes;
            return View(refeicoes);
        }

        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult BebidasMaisVendidas(DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            TratarParametros("RelatorioBebidasMaisVendidas", ref dataInicial, ref dataFinal);

            List<BebidaMaisVendidaDTO> bebidas = new List<BebidaMaisVendidaDTO>();

            IDbConnection con = new SqlConnection(db.Database.Connection.ConnectionString);
            con.Open();
            try
            {
                string sql = @"SELECT TBTiposBebida.Nome as Tipo,
                                      TBBebidas.Nome as Bebida,
	                                  COUNT(TBAtribBebidasPedido.Codigo) Quantidade
                                 FROM TBBebidas
                           INNER JOIN TBTiposBebida ON TBTiposBebida.Codigo = TBBebidas.CodTipo
                           INNER JOIN TBAtribBebidasPedido ON TBAtribBebidasPedido.CodBebida = TBBebidas.Codigo
                           INNER JOIN TBPedidos ON TBAtribBebidasPedido.CodPedido = TBPedidos.Codigo
                                WHERE (TBPedidos.DataInicio BETWEEN @dataInicial AND @dataFinal)
                             GROUP BY TBTiposBebida.Nome, TBBebidas.Nome
                             ORDER BY Quantidade DESC, Tipo, Bebida ";

                IDbCommand cmd = new SqlCommand(sql, (SqlConnection)con);
                cmd.Parameters.Add(new SqlParameter("@dataInicial", dataInicial));
                cmd.Parameters.Add(new SqlParameter("@dataFinal", dataFinal.Value.AddDays(1)));

                int rank = 1, qtdTotal = 0;
                using (IDataReader dr = cmd.ExecuteReader())
                    while (dr.Read())
                    {
                        BebidaMaisVendidaDTO bebida = new BebidaMaisVendidaDTO
                        {
                            Rank = rank++,
                            Tipo = (string)dr["Tipo"],
                            Bebida = (string)dr["Bebida"],
                            Quantidade = (int)dr["Quantidade"],
                            ClasseCSS = "linDado"
                        };
                        qtdTotal += bebida.Quantidade;
                        bebidas.Add(bebida);
                    }
                bebidas.Add(new BebidaMaisVendidaDTO
                {
                    Bebida = "TOTAL",
                    Quantidade = qtdTotal,
                    ClasseCSS = "linTotal"
                });
            }
            finally
            {
                con.Close();
            }

            ViewBag.DataInicial = dataInicial.Value.ToString("yyyy-MM-dd");
            ViewBag.DataFinal = dataFinal.Value.ToString("yyyy-MM-dd");
            return View(bebidas);
        }

        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult FaturamentoPorData(DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            TratarParametros("RelatorioFaturamentoPorData", ref dataInicial, ref dataFinal);

            List<FaturamentoNaDataDTO> faturamentos = new List<FaturamentoNaDataDTO>();

            IDbConnection con = new SqlConnection(db.Database.Connection.ConnectionString);
            con.Open();
            try
            {
                string sql = @"SELECT        CONVERT(DATE, TBPeds.DataInicio) AS Data,
	                            (SELECT ISNULL(SUM(Valor),0) + ISNULL(SUM(Acrescimo),0) FROM TBAtribRefeicoesPedido AS TBA INNER JOIN TBPedidos AS TBP ON TBA.CodPedido = TBP.Codigo WHERE CONVERT(DATE, TBP.DataInicio) = CONVERT(DATE, TBPeds.DataInicio)) AS ValorRefeicoes,
	                            (SELECT ISNULL(SUM(Valor),0) FROM TBAtribBebidasPedido   AS TBA INNER JOIN TBPedidos AS TBP ON TBA.CodPedido = TBP.Codigo WHERE CONVERT(DATE, TBP.DataInicio) = CONVERT(DATE, TBPeds.DataInicio)) AS ValorBebidas,

                                (SELECT ISNULL(SUM(Valor),0) 
									FROM TBAtribItensBalcaoPedido   AS TBI 
										INNER JOIN TBPedidos AS TBP ON TBI.CodPedido = TBP.Codigo 
									WHERE CONVERT(DATE, TBP.DataInicio) = CONVERT(DATE, TBPeds.DataInicio)) AS ValorDiversos,

	                            SUM(Acrescimos) AS Acrescimos, SUM(Descontos) AS Descontos,
                                (SELECT ISNULL(SUM(ValorEntrega), 0) FROM TBPedidosExternos AS TBEXT INNER JOIN TBPedidos as TBP on TBEXT.Codigo = TBP.Codigo WHERE CONVERT(DATE, TBP.DataInicio) = CONVERT(DATE, TBPeds.DataInicio)) AS ValorEntregas 
                            FROM TBPedidos AS TBPeds
                            WHERE DataInicio BETWEEN @dataInicial AND @dataFinal
                            GROUP BY CONVERT(date, TBPeds.DataInicio)
                            ORDER BY Data";
                IDbCommand cmd = new SqlCommand(sql, (SqlConnection)con);
                cmd.Parameters.Add(new SqlParameter("@dataInicial", dataInicial));
                cmd.Parameters.Add(new SqlParameter("@dataFinal", dataFinal.Value.AddDays(1)));

                decimal TotalRefeicoes = 0, TotalBebidas = 0, TotalAcrescimos = 0, TotalDescontos = 0, TotalEntregas = 0, TotalDiversos = 0;
                using (IDataReader dr = cmd.ExecuteReader())
                    while (dr.Read())
                    {
                        TotalRefeicoes += (decimal)dr["ValorRefeicoes"];
                        TotalBebidas += (decimal)dr["ValorBebidas"];
                        TotalAcrescimos += (decimal)dr["Acrescimos"];
                        TotalDescontos += (decimal)dr["Descontos"];
                        TotalEntregas += (decimal)dr["ValorEntregas"];
                        TotalDiversos += (decimal)dr["ValorDiversos"];
                        FaturamentoNaDataDTO faturamento = new FaturamentoNaDataDTO
                        {
                            Data = ((DateTime)dr["Data"]).ToShortDateString(),
                            ValorRefeicoes = (decimal)dr["ValorRefeicoes"],
                            ValorBebidas = (decimal)dr["ValorBebidas"],
                            ValorItensBalcao = (decimal)dr["ValorDiversos"],
                            Acrescimos = (decimal)dr["Acrescimos"],
                            Descontos = (decimal)dr["Descontos"],
                            Entregas = (decimal)dr["ValorEntregas"],
                            ClasseCSS = "linDado"
                        };
                        faturamentos.Add(faturamento);
                    }
                faturamentos.Add(new FaturamentoNaDataDTO
                {
                    Data = "TOTAL",
                    ValorRefeicoes = TotalRefeicoes,
                    ValorBebidas = TotalBebidas,
                    ValorItensBalcao = TotalDiversos,
                    Acrescimos = TotalAcrescimos,
                    Descontos = TotalDescontos,
                    Entregas = TotalEntregas,                    
                    ClasseCSS = "linTotal"
                }); ;
            }
            finally
            {
                con.Close();
            }

            ViewBag.DataInicial = dataInicial.Value.ToString("yyyy-MM-dd");
            ViewBag.DataFinal = dataFinal.Value.ToString("yyyy-MM-dd");
            return View(faturamentos);
        }

        private void TratarParametros(string nomeCookie, ref DateTime? dataInicial, ref DateTime? dataFinal)
        {
            string aux = "";
            TratarParametros(nomeCookie, ref dataInicial, ref dataFinal, ref aux);
        }

        private void TratarParametros(string nomeCookie, ref DateTime? dataInicial, ref DateTime? dataFinal, ref string unificarRefeicoes)
        {
            HttpCookie cookie = Request.Cookies[nomeCookie];
            try
            {
                if (cookie != null)
                {
                    if (!dataInicial.HasValue || !dataFinal.HasValue)
                    {
                        dataInicial = DateTime.Parse(cookie.Values["dataInicial"]);
                        dataFinal = DateTime.Parse(cookie.Values["dataFinal"]);
                        unificarRefeicoes = cookie.Values["unificarRefeicoes"];
                    }
                }
                else
                {
                    dataInicial = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                    dataFinal = DateTime.Today;
                    unificarRefeicoes = "";
                }
            }
            catch
            {
                dataInicial = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                dataFinal = DateTime.Today;
                unificarRefeicoes = "";
            }

            cookie = new HttpCookie(nomeCookie);
            cookie.Values["dataInicial"] = dataInicial.Value.ToString();
            cookie.Values["dataFinal"] = dataFinal.Value.ToString();
            cookie.Values["unificarRefeicoes"] = unificarRefeicoes;
            cookie.Expires = DateTime.Today.AddDays(1);
            HttpContext.Response.Cookies.Set(cookie);
        }
    }
}
 */