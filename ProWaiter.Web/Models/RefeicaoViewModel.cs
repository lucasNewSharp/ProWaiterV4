using ProWaiter.Web.Models.Entidades;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProWaiter.Web.Models
{
    public class RefeicaoViewModel
    {
        public RefeicaoViewModel()
        {
            Componentes = new BootstrapDualListModelView();
        }

        public RefeicaoViewModel(Refeicao refeicao, IEnumerable<ComponenteRefeicao> todosComponentes) : this()
        {
            Codigo = refeicao.Codigo;
            Nome = refeicao.Nome;
            CodTipo = refeicao.CodTipo;
            NomeTipo = refeicao.Tipo.Nome;

            foreach (ComponenteRefeicao componente in refeicao.ComponentesRefeicao.OrderBy(c => c.Nome))
                Componentes.ListaSelecionados.Add(new KeyValuePair<string, string>(componente.Codigo.ToString(), componente.Nome));

            foreach (ComponenteRefeicao componente in todosComponentes.Except(refeicao.ComponentesRefeicao).OrderBy(c => c.Nome))
                Componentes.ListaDisponiveis.Add(new KeyValuePair<string, string>(componente.Codigo.ToString(), componente.Nome));
        }

        [Display(Name = "Código")]
        public short Codigo { get; set; }
        public string Nome { get; set; }
        public short CodTipo { get; set; }

        [Display(Name = "Tipo")]
        public string NomeTipo { get; set; }

        public BootstrapDualListModelView Componentes { get; set; }

        public override string ToString()
        {
            return Nome;
        }
    }
}