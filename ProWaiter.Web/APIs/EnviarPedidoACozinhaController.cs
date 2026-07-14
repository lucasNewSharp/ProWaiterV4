using ProWaiter.Web.AutenticacaoAPI;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProWaiter.Web.APIs
{

    public class ItensNaoEnviados
    {
        public int CodPedido { get; set; }
        public short CodMesa { get; set; }
        public List<RefeicaoDoPedidoDTO> RefeicoesDoPedido { get; set; }
        public List<BebidaDoPedido> BebidasDoPedido { get; set; }
        public string Mensagem { get; set; }
        public short? CodLocalInternoEntrega { get; set; }
        public decimal Acrescimos { get; set; }
        public decimal Descontos { get; set; }
    }

    public class RefeicaoDoPedidoDTO
    {
        public const int TamMaxObservacoes = int.MaxValue;

        public int Codigo { get; set; }
        public int CodPedido { get; set; }

        public short CodRefeicao { get; set; }
        public string CodTamanho { get; set; }
        public virtual TamanhoRefeicao Tamanho { get; set; }
        public virtual RefeicaoDoCardapio RefeicaoDoCardapio { get; set; }

        public virtual ICollection<ComponenteRefeicaoPedidoDTO> ComponentesRefeicaoPedido { get; set; }
        public bool Enviado { get; set; }

        public string Observacoes { get; set; }
        public decimal Valor { get; set; }
        public decimal Acrescimo { get; set; }
        public string NomeUsuario { get; set; }
        public DateTime? DataHora { get; set; }
    }

    public class ComponenteRefeicaoPedidoDTO
    {
        public short CodComponente { get; set; }
        public byte Quantidade { get; set; }
    }

    [IdentityBasicAuthentication]
    public class EnviarPedidoACozinhaController : ApiController
    {
        private readonly ProWaiterContext db = new ProWaiterContext();

        // POST: api/EnviarRefeicoesACozinha/5
        [ResponseType(typeof(List<ItensNaoEnviados>))]
        public ItensNaoEnviados Post(ItensNaoEnviados itensNaoEnviadas)
        {
            decimal acrescimos = itensNaoEnviadas.Acrescimos;
            decimal descontos = itensNaoEnviadas.Descontos;
            try
            {
                GestorImpressoes.RetornoImpressaoBebidas retImprBebidas = null;
                GestorImpressoes.RetornoImpressaoRefeicoes retImprRefeicoes = null;

                if (itensNaoEnviadas == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                Mesa mesa = db.Mesas.Where(m => m.Codigo == itensNaoEnviadas.CodMesa).SingleOrDefault();
                PedidoInterno pedidoInterno = db.PedidosInternos.Where(p => p.Codigo == itensNaoEnviadas.CodPedido).SingleOrDefault();
                if (pedidoInterno == null || mesa == null || pedidoInterno.DataTermino.HasValue)
                {
                    ItensNaoEnviados retornoPedidoFechado = new ItensNaoEnviados()
                    {
                        CodMesa = itensNaoEnviadas.CodMesa,
                        CodPedido = itensNaoEnviadas.CodPedido,
                        RefeicoesDoPedido = new List<RefeicaoDoPedidoDTO>(),
                        BebidasDoPedido = new List<BebidaDoPedido>(),
                        Mensagem = "O Pedido já não existe mais!",
                        CodLocalInternoEntrega = itensNaoEnviadas.CodLocalInternoEntrega
                    };
                    return retornoPedidoFechado;                    
                }

                ItensNaoEnviados retorno = new ItensNaoEnviados()
                {
                    CodMesa = itensNaoEnviadas.CodMesa,
                    CodPedido = itensNaoEnviadas.CodPedido,
                    RefeicoesDoPedido = new List<RefeicaoDoPedidoDTO>(),
                    BebidasDoPedido = new List<BebidaDoPedido>(),
                    Mensagem = "Itens enviados com sucesso!",
                    CodLocalInternoEntrega = itensNaoEnviadas.CodLocalInternoEntrega
                };

                LocalInterno localInterno = itensNaoEnviadas.CodLocalInternoEntrega.HasValue
                    ? db.LocaisInternos.SingleOrDefault(l => l.Codigo == itensNaoEnviadas.CodLocalInternoEntrega.Value)
                    : null;

                Exception exception = null;
                db.IniciarTransacao();
                try
                {
                    HashSet<short> codBebsAtachadas = new HashSet<short>();
                    foreach (BebidaDoPedido bebDoPedido in itensNaoEnviadas.BebidasDoPedido)
                    {
                        bebDoPedido.Bebida = db.Bebidas.Single(b => b.Codigo == bebDoPedido.CodBebida);
                        if (!codBebsAtachadas.Contains(bebDoPedido.Bebida.Codigo))
                        {
                            db.Bebidas.Attach(bebDoPedido.Bebida);
                            codBebsAtachadas.Add(bebDoPedido.Bebida.Codigo);
                        }
                        bebDoPedido.DataHora = DateTime.Now;
                        bebDoPedido.NomeUsuario = User.Identity.Name;
                        bebDoPedido.AplicarDesconto();
                        bebDoPedido.PercDesconto = bebDoPedido.Bebida.PercDesconto;


                        db.BebidasDosPedidos.Add(bebDoPedido);
                        pedidoInterno.BebidasDoPedido.Add(bebDoPedido);
                    }
                    pedidoInterno.CodLocalInterno = itensNaoEnviadas.CodLocalInternoEntrega;

                    retImprBebidas = GestorImpressoes.Instancia.ImprimirBebidas(pedidoInterno, itensNaoEnviadas.BebidasDoPedido, mesa, localInterno);

                    foreach (BebidaDoPedido beb in retImprBebidas.BebidasEnviadas)
                        beb.Enviado = true;

                    foreach (BebidaDoPedido beb in retImprBebidas.BebidasNaoEnviadas)
                    {
                        beb.Enviado = false;
                        pedidoInterno.BebidasDoPedido.Remove(beb);

                        retorno.BebidasDoPedido.Add(new BebidaDoPedido()
                        {
                            Bebida = beb.Bebida,
                            CodPedido = beb.CodPedido,
                            Enviado = false,
                            Codigo = 0,
                            CodBebida = beb.Bebida.Codigo,
                            Observacoes = beb.Observacoes,
                            Valor = beb.Valor
                        });
                        db.BebidasDosPedidos.Remove(beb);
                    }

                    if (retImprBebidas.BebidasNaoEnviadas != null && retImprBebidas.BebidasNaoEnviadas.Count > 0)
                    {
                        Exception exBeb = retImprBebidas.Erros.FirstOrDefault();
                        if (exBeb != null)
                            retorno.Mensagem = retImprBebidas.ImpressorasComProblema + "\n" + exBeb.Message + "\n";
                    }

                    db.Entry(pedidoInterno).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    db.SetarRollBack();
                    exception = ex;
                }
                if (!db.FinalizarTransacao())
                {
                    //Gambiarra pois ao fazer rollback o objeto fica com código (identity). Objeto inconsistente
                    retorno.BebidasDoPedido = itensNaoEnviadas.BebidasDoPedido.Select(b => new BebidaDoPedido()
                    {
                        Bebida = b.Bebida,
                        CodPedido = b.CodPedido,
                        Enviado = false,
                        Codigo = 0,
                        CodBebida = b.Bebida.Codigo,
                        Observacoes = b.Observacoes,
                        Valor = b.Valor
                    }).ToList();
                    retorno.Mensagem = exception.Message;
                }

                db.IniciarTransacao();
                try
                {
                    HashSet<RefeicaoDoCardapio> refsDoCardapioAtachados = new HashSet<RefeicaoDoCardapio>();
                    HashSet<string> codTamsRefAtachados = new HashSet<string>();

                    List<RefeicaoDoPedido> listaParaImprimir = new List<RefeicaoDoPedido>();
                    foreach (RefeicaoDoPedidoDTO refDoPedido in itensNaoEnviadas.RefeicoesDoPedido)
                    {
                        refDoPedido.Tamanho = db.TamanhosRefeicao.Single(t => t.Codigo == refDoPedido.CodTamanho);
                        refDoPedido.RefeicaoDoCardapio = db.RefeicoesDoCardapio.Single(r => r.CodRefeicao == refDoPedido.CodRefeicao && r.CodTamanho == refDoPedido.CodTamanho);
                        if (!refsDoCardapioAtachados.Contains(refDoPedido.RefeicaoDoCardapio))
                        {
                            db.RefeicoesDoCardapio.Attach(refDoPedido.RefeicaoDoCardapio);
                            refsDoCardapioAtachados.Add(refDoPedido.RefeicaoDoCardapio);
                        }
                        if (!codTamsRefAtachados.Contains(refDoPedido.Tamanho.Codigo))
                        {
                            db.TamanhosRefeicao.Attach(refDoPedido.Tamanho);
                            codTamsRefAtachados.Add(refDoPedido.Tamanho.Codigo);
                        }

                        refDoPedido.DataHora = DateTime.Now;
                        refDoPedido.NomeUsuario = User.Identity.Name;

                        RefeicaoDoPedido refeBD = new RefeicaoDoPedido()
                        {
                            Acrescimo = refDoPedido.Acrescimo,
                            CodPedido = refDoPedido.CodPedido,
                            CodRefeicao = refDoPedido.CodRefeicao,
                            CodTamanho = refDoPedido.CodTamanho,
                            DataHora = refDoPedido.DataHora,
                            Enviado = refDoPedido.Enviado,
                            NomeUsuario = refDoPedido.NomeUsuario,
                            Observacoes = refDoPedido.Observacoes,
                            RefeicaoDoCardapio = refDoPedido.RefeicaoDoCardapio,
                            Tamanho = refDoPedido.Tamanho,
                            Valor = refDoPedido.Valor,
                        };

                        foreach(ComponenteRefeicaoPedidoDTO dto in refDoPedido.ComponentesRefeicaoPedido)
                        {
                            if (dto.Quantidade == 0)
                                continue;
                            ComponenteRefeicaoPedido comp = new ComponenteRefeicaoPedido()
                            {
                                CodComponente = dto.CodComponente,
                                ComponenteRefeicao = db.ComponentesRefeicao.Where(c => c.Codigo == dto.CodComponente).Single(),
                                Quantidade = dto.Quantidade
                            };
                            refeBD.ComponentesRefeicaoPedido.Add(comp);
                        }
                        refeBD.RecalcularValorRefeicao();
                        db.RefeicoesDoPedido.Add(refeBD);
                        pedidoInterno.RefeicoesDoPedido.Add(refeBD);
                        listaParaImprimir.Add(refeBD);

                        //Tenho que executar o saveChanges a cada refeição do pedidio, pois caso
                        //tenha sido adicioanda duas refeições iguais, da erro para salvar por causa da referencia 
                        //da TBAtribComponentessRefeicaoDoPedido a propria refeição do pedidio, e ainda não existe o CodRefeicaoPedido definido, somente apos o SaveChanges
                        db.Entry(pedidoInterno).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //throw new ApplicationException("Teste se vai fazer o rollback"); Já testei aqui, depois do saveChanges da primeira refeição, para ver se ia fazer o rollback
                    }

                    pedidoInterno.CodLocalInterno = itensNaoEnviadas.CodLocalInternoEntrega;
                    retImprRefeicoes = GestorImpressoes.Instancia.ImprimirRefeicoes(pedidoInterno, listaParaImprimir, mesa, localInterno);

                    foreach (RefeicaoDoPedido refe in retImprRefeicoes.RefeicoesEnviadas)
                        refe.Enviado = true;

                    foreach (RefeicaoDoPedido refe in retImprRefeicoes.RefeicoesNaoEnviadas)
                    {
                        refe.Enviado = false;
                        pedidoInterno.RefeicoesDoPedido.Remove(refe);

                        RefeicaoDoPedidoDTO novaRefe = new RefeicaoDoPedidoDTO()
                        {
                            Codigo = 0,
                            CodPedido = refe.CodPedido,
                            CodRefeicao = refe.CodRefeicao,
                            CodTamanho = refe.CodTamanho,
                            Enviado = false,
                            Observacoes = refe.Observacoes,
                            RefeicaoDoCardapio = refe.RefeicaoDoCardapio,
                            ComponentesRefeicaoPedido = new List<ComponenteRefeicaoPedidoDTO>(),
                            Tamanho = refe.Tamanho,
                            Valor = refe.Valor,
                            Acrescimo = refe.Acrescimo
                        };
                        
                        foreach (ComponenteRefeicaoPedido comp in refe.ComponentesRefeicaoPedido)
                            novaRefe.ComponentesRefeicaoPedido.Add(new ComponenteRefeicaoPedidoDTO()
                            {
                                CodComponente = comp.CodComponente,
                                Quantidade = comp.Quantidade
                            });

                        retorno.RefeicoesDoPedido.Add(novaRefe);
                        db.RefeicoesDoPedido.Remove(refe);
                    }

                    if (retImprRefeicoes.RefeicoesNaoEnviadas != null && retImprRefeicoes.RefeicoesNaoEnviadas.Count > 0)
                    {
                        Exception exRef = retImprRefeicoes.Erros.FirstOrDefault();
                        if (exRef != null)
                            retorno.Mensagem = retImprRefeicoes.ImpressorasComProblema + "\n" + exRef.Message + "\n";
                    }

                    db.Entry(pedidoInterno).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    db.SetarRollBack();
                    exception = ex;
                }

                if (!db.FinalizarTransacao())
                {
                    //Gambiarra pois ao fazer rollback o objeto fica com código (identity). Objeto inconsistente                
                    retorno.RefeicoesDoPedido.Clear();
                    foreach (var refPedDTO in itensNaoEnviadas.RefeicoesDoPedido)
                    {
                        var refeDTO = new RefeicaoDoPedidoDTO()
                        {
                            Codigo = 0,
                            CodPedido = refPedDTO.CodPedido,
                            CodRefeicao = refPedDTO.CodRefeicao,
                            CodTamanho = refPedDTO.CodTamanho,
                            ComponentesRefeicaoPedido = refPedDTO.ComponentesRefeicaoPedido,
                            Enviado = false,
                            Observacoes = refPedDTO.Observacoes,
                            RefeicaoDoCardapio = refPedDTO.RefeicaoDoCardapio,
                            Tamanho = refPedDTO.Tamanho,
                            Valor = refPedDTO.Valor,
                            Acrescimo = refPedDTO.Acrescimo
                        };

                        retorno.RefeicoesDoPedido.Add(refeDTO);
                    }                    
                    retorno.Mensagem = exception.Message;
                }

                //Se teve algum item adicionado a partir de modelo, pode ter acrescimo ou desconto, então atualizamos o pedido
                if(acrescimos > 0 || descontos > 0)
                {
                    try
                    {
                        pedidoInterno.Acrescimos += acrescimos;
                        pedidoInterno.Descontos += descontos;
                        db.SaveChanges();
                        //caso não conseguiu enviar alguma coias para as impressoras, avisamos o usuário que o desconto foi adicionado mesmo assim
                        //ao pedido, pois o desconto é do pedido e não dos itens, o usuário do APP deve reenviar os itens que 
                        //não foi possivel imprimir. No proximo reenvio o desconto/acrescimo vira zerado.
                        if (retorno.BebidasDoPedido.Count > 0 || retorno.RefeicoesDoPedido.Count > 0)
                        {
                            retorno.Mensagem += " Alguns itens não foram impressos, porém o desconto foi adicionado ao pedido. Ajuste a impressora e reenvie apenas os itens que não foram impressos.";
                        }
                    }
                    catch(Exception ex)
                    {
                        retorno.Mensagem += ex.Message;
                    }
                }

                return retorno;
            }
            catch
            {
                //Log
            }

            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
