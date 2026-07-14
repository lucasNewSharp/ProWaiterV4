using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public class RefeicaoMaisVendidaDTO
    {
        public int? Rank { get; set; }

        public string Tipo { get; set; }

        [Display(Name = "Refeição")]
        public string Refeicao { get; set; }

        public string Tamanho { get; set; }

        public int Quantidade { get; set; }

        public string ClasseCSS { get; set; }
    }

    public class BebidaMaisVendidaDTO
    {
        public int? Rank { get; set; }

        public string Tipo { get; set; }

        public string Bebida { get; set; }

        public int Quantidade { get; set; }

        public string ClasseCSS { get; set; }
    }

    public class FaturamentoNaDataDTO
    {
        public string Data { get; set; }

        [Display(Name = "Refeições")]
        public decimal ValorRefeicoes { get; set; }

        [Display(Name = "Bebidas")]
        public decimal ValorBebidas { get; set; }

        [Display(Name = "Diversos")]
        public decimal ValorItensBalcao { get; set; }

        [Display(Name = "Acréscimos")]
        public decimal Acrescimos { get; set; }

        public decimal Descontos { get; set; }

        public decimal Entregas { get; set; }

        [Display(Name = "Total")]
        public decimal Total
        {
            get { return ValorRefeicoes + ValorBebidas + ValorItensBalcao + Entregas + Acrescimos - Descontos; }
        }

        public string ClasseCSS { get; set; }
    }
}