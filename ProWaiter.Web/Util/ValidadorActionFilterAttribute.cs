using Microsoft.AspNetCore.Mvc.Filters;
using ProWaiter.Web.Controllers;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using System;
using System.ComponentModel.DataAnnotations;

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

    public class ValidadorActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
    }
}