using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProWaiter.Web.Models
{
    public class RefeicaoDoCardapioViewModel
    {
        public RefeicaoDoCardapioViewModel()
        {
        }

        public short CodRefeicao { get; set; }
        public string NomeRefeicao { get; set; }
        public string CodTamanho { get; set; }
        public string NomeTamanho { get; set; }
        public decimal Valor { get; set; }
        public bool Ativo { get; set; }
        public byte CodImpressora { get; set; }
        public bool DeComposicao { get; set; }
        public decimal PercDesconto { get; set; }
        [Display(Name = "Código de barras")]
        public string CodBarras { get; set; }

        public List<ComponenteDeComposicaoViewModel> ComponentesDeComposicao { get; set; }     
        
        public string RefeicaoParaCargaSelecionada { get; set; }
        public SelectList RefeicoesJaCadastradas { get; set; }
    }

    public class RefeicaoParaCarga
    {
        public string Codigo { get; set; }        
        public string NomeRefeicao { get; set; }        
    }

    public class ComponenteDeComposicaoViewModel
    {
        public short CodComponente { get; set; }
        public string NomeComponente { get; set; }
        public decimal Valor { get; set; }
        public bool CalculoProporcional { get; set; }
        public bool Ativo { get; set; }

        public string CodUnidade { get; set; }
        public SelectList ListaUnidades { get; set; }
    }
}