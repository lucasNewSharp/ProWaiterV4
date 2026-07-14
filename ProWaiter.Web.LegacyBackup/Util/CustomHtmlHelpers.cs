using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProWaiter.Web.Util
{
    public enum eTipoBotao { Editar, Detalhes, Excluir, Voltar }

    public static class CustomHtmlHelepers
    {        
        public static IHtmlString BotaoAcaoTexto(this HtmlHelper htmlHelper, string action, string controller, string texto, object routedValues = null, string style = null)
        {
            //<input type="button" class="btn btn-primary" onclick="location.href='@Url.Action("Create", "PedidosExternos")'" value="Para Entrega" />    

            TagBuilder input = new TagBuilder("input");            
            input.AddCssClass("btn btn-primary");
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string onClick = $"location.href='{urlHelper.Action(action, controller, routedValues)}'";
            input.Attributes.Add("onclick", onClick);
            input.Attributes.Add("value", texto);
            input.Attributes.Add("type", "button");
            if (!string.IsNullOrWhiteSpace(style))
            {
                input.Attributes.Add("style", style);
            }
            return MvcHtmlString.Create(input.ToString());
        }

        public static IHtmlString BotaoAcaoImagem(this HtmlHelper htmlHelper, eTipoBotao tipo, string action, object routedValues, string linkToolTipo)
        {
            return BotaoAcaoImagem(htmlHelper, tipo, action, null, routedValues, linkToolTipo);
        }

        public static IHtmlString BotaoAcaoImagem(this HtmlHelper htmlHelper, eTipoBotao tipo, string action, string controller, object routedValues, string linkToolTipo)
        {
            string icone = "glyphicon glyphicon-";
            string cor = "btn btn-";
            switch (tipo)
            {
                case eTipoBotao.Editar:
                    icone += "edit";
                    cor += "warning";
                    break;
                case eTipoBotao.Detalhes:
                    icone += "list";
                    cor += "primary";
                    break;
                case eTipoBotao.Excluir:
                    icone += "remove";
                    cor += "danger";
                    break;
                case eTipoBotao.Voltar:
                    icone += "arrow-left";
                    cor += "primary";
                    break;
            }

            TagBuilder span = new TagBuilder("span");
            span.AddCssClass(icone);

            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var anchor = new TagBuilder("a");
            anchor.InnerHtml = span.ToString();
            anchor.Attributes["title"] = linkToolTipo;
            anchor.AddCssClass(cor);
            anchor.Attributes["href"] = urlHelper.Action(action, controller, routedValues);

            return MvcHtmlString.Create(anchor.ToString());

        }
    }
}