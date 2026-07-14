using ProWaiter.Web.Models.DTOs;
using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Web;


namespace ProWaiter.Web.Models
{
    public class ConfiguracaoDescontosViewModel
    {
        public abstract class ItemConfiguracaoDesconto
        {
            public string Nome { get; set; }
            public decimal Valor { get; set; }
            [Display(Name = "Perc. desconto (%)")]
            public decimal PercDesconto { get; set; }
            public decimal ValorFinal { get; set; }

            public bool Checked { get; set; }
            public string CSSDestaqueValorFinal { get; set; }
        }

        public class BebidaVM : ItemConfiguracaoDesconto
        {
            public int Codigo { get; set; }

            public BebidaVM() { }
            public BebidaVM(Bebida bebida)
            {
                Codigo = bebida.Codigo;
                Nome = bebida.Nome;
                Valor = bebida.Valor;
                PercDesconto = bebida.PercDesconto;
                ValorFinal = PercDesconto > 0 ? Valor - (Valor * (PercDesconto / 100)) : Valor;
                if (ValorFinal != Valor)
                    CSSDestaqueValorFinal = "bg-info";
            }
        }

        public class ItemBalcaoVM : ItemConfiguracaoDesconto
        {
            public int Codigo { get; set; }

            public ItemBalcaoVM() { }
            public ItemBalcaoVM(ItemBalcao item)
            {
                Codigo = item.Codigo;
                Nome = item.Nome;
                Valor = item.Valor;
                PercDesconto = item.PercDesconto;
                ValorFinal = PercDesconto > 0 ? Valor - (Valor * (PercDesconto / 100)) : Valor;
                if (ValorFinal != Valor)
                    CSSDestaqueValorFinal = "bg-info";
            }
        }


        public class RefeicaoCardapioVM : ItemConfiguracaoDesconto
        {
            public short CodRefeicao { get; set; }
            public string CodTamanho { get; set; }
            public string NomeTamanho { get; set; }

            public RefeicaoCardapioVM() { }
            public RefeicaoCardapioVM(RefeicaoDoCardapio refe)
            {
                CodRefeicao = refe.CodRefeicao;
                Nome = refe.Refeicao.Nome;
                CodTamanho = refe.CodTamanho;
                NomeTamanho = refe.TamanhoRefeicao.Nome;
                Valor = refe.Valor;
                PercDesconto = refe.PercDesconto;
                ValorFinal = PercDesconto > 0 ? Valor - (Valor * (PercDesconto / 100)) : Valor;
                if (ValorFinal != Valor)
                    CSSDestaqueValorFinal = "bg-info";
            }
        }

        public List<BebidaVM> Bebidas { get; set; }
        public List<RefeicaoCardapioVM> Refeicoes { get; set; }
        public List<ItemBalcaoVM> ItensBalcao { get; set; }

        public ConfiguracaoDescontosViewModel()
        {
            Bebidas = new List<BebidaVM>();
            Refeicoes = new List<RefeicaoCardapioVM>();
            ItensBalcao = new List<ItemBalcaoVM>();
        }
    }
}