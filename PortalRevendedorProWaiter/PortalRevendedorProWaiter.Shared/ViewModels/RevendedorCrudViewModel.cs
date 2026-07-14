using PortalRevendedorProWaiter.Shared.Atributos;
using PortalRevendedorProWaiter.Shared.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.ViewModels
{
    public class RevendedorCrudViewModel
    {
        public const int MaxLengthCNPJ = 18;
        public const int MaxLengthTelefone = 14;

        public RevendedorCrudViewModel() { }

        public RevendedorCrudViewModel(Revendedor rev)
        {
            Codigo = rev.Codigo;
            CNPJ = rev.CNPJ;
            RazaoSocial = rev.RazaoSocial;
            Endereco = rev.Endereco;
            Responsavel = rev.Responsavel;
            Telefone1 = rev.Telefone1;
            Telefone2 = rev.Telefone2;
            Ativo = rev.Ativo;
        }

        public int Codigo { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(18, ErrorMessage = ConstantesAtributosEntidades.TamanhoFixo, MinimumLength = 18)]
        public string CNPJ { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(500, ErrorMessage = ConstantesAtributosEntidades.TamanhoFixo)]
        public string RazaoSocial { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(500, ErrorMessage = ConstantesAtributosEntidades.TamanhoFixo)]
        public string Endereco { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(200, ErrorMessage = ConstantesAtributosEntidades.TamanhoFixo)]
        public string Responsavel { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(14, ErrorMessage = ConstantesAtributosEntidades.TamanhoFixo, MinimumLength = 14)]
        public string Telefone1 { get; set; }

        [StringLength(14, ErrorMessage = ConstantesAtributosEntidades.TamanhoFixo, MinimumLength = 14)]
        public string Telefone2 { get; set; }

        [CampoRequeridoObrigatorio]
        public bool Ativo { get; set; }

    }
}
