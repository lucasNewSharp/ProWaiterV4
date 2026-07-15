using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ProWaiter.Web.Util
{
    public enum eTipoBotao { Editar, Detalhes, Excluir, Voltar }

    public static class CustomHtmlHelepers
    {        
        public static IHtmlContent BotaoAcaoTexto(this IHtmlHelper htmlHelper, string action, string controller, string texto, object routedValues = null, string style = null)
        {
            var tagBuilder = new TagBuilder("input");            
            tagBuilder.AddCssClass("btn btn-primary");
            
            var urlHelperFactory = (IUrlHelperFactory)htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory));
            var urlHelper = urlHelperFactory.GetUrlHelper(htmlHelper.ViewContext);
            string url = urlHelper.Action(action, controller, routedValues);
            string onClick = $"location.href='{url}'";
            
            tagBuilder.Attributes.Add("onclick", onClick);
            tagBuilder.Attributes.Add("value", texto);
            tagBuilder.Attributes.Add("type", "button");
            if (!string.IsNullOrWhiteSpace(style))
            {
                tagBuilder.Attributes.Add("style", style);
            }
            return tagBuilder;
        }

        public static IHtmlContent BotaoAcaoImagem(this IHtmlHelper htmlHelper, eTipoBotao tipo, string action, object routedValues, string linkToolTipo)
        {
            return BotaoAcaoImagem(htmlHelper, tipo, action, null, routedValues, linkToolTipo);
        }

        public static IHtmlContent BotaoAcaoImagem(this IHtmlHelper htmlHelper, eTipoBotao tipo, string action, string controller, object routedValues, string linkToolTipo)
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

            var anchor = new TagBuilder("a");
            anchor.InnerHtml.AppendHtml(span);
            anchor.Attributes["title"] = linkToolTipo;
            anchor.AddCssClass(cor);
            var urlHelperFactory = (IUrlHelperFactory)htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory));
            var urlHelper = urlHelperFactory.GetUrlHelper(htmlHelper.ViewContext);
            string url = urlHelper.Action(action, controller, routedValues);
            
            anchor.Attributes["href"] = url;

            return anchor;
        }
    }
}