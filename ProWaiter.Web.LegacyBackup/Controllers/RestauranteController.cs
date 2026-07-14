using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace ProWaiter.Web.Controllers
{
    public class RestauranteController : Controller
    {
        #region Restaurante Atual

        internal Restaurante RecuperarRestaurante()
        {
            Restaurante restaurante = null;
            if (System.IO.File.Exists(NomeArquivo))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Restaurante));
                using (XmlReader reader = new XmlTextReader(NomeArquivo))
                    restaurante = (Restaurante)serializer.Deserialize(reader);
            }
            return restaurante;
        }

        internal void GravarRestaurante(Restaurante restaurante)
        {
            XmlSerializer serializer = new XmlSerializer(restaurante.GetType());
            using (XmlWriter writer = new XmlTextWriter(NomeArquivo, Encoding.UTF8))
                serializer.Serialize(writer, restaurante);
        }

        private string NomeArquivo
        {
            get
            {
                string retorno = Configuracoes.ObterInstancia().ArquivoRestaurante;
                retorno = retorno.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
                string diretorio = Path.GetDirectoryName(retorno);
                if (!Directory.Exists(diretorio))
                    Directory.CreateDirectory(diretorio);

                return retorno;
            }
        }

        #endregion

        // GET: Restaurante
        public ActionResult Index()
        {
            if (Startup.Restaurante == null)
            {
                Startup.Restaurante = RecuperarRestaurante();
                if (Startup.Restaurante == null)
                    return RedirectToAction("Create");
            }

            return RedirectToAction("Details");
        }

        // GET: Restaurante/Details/5
        public ActionResult Details()
        {
            if (Startup.Restaurante == null)
                Startup.Restaurante = RecuperarRestaurante();
            return View(Startup.Restaurante);
        }

        // GET: Restaurante/Create
        public ActionResult Create(bool ativar = false)
        {
            if (!ativar && (Startup.Restaurante != null || System.IO.File.Exists(NomeArquivo)))
                return RedirectToAction("Details");

            Restaurante restaurante = RecuperarRestaurante() ?? new Restaurante();
            return View(restaurante);
        }

        // POST: Restaurante/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Nome,Endereco,Cidade,UF,Segredo")] Restaurante restaurante)
        {
            try
            {
                restaurante.DataAtivacao = DateTime.UtcNow;
                GravarRestaurante(restaurante);
                Startup.Restaurante = restaurante;
                return RedirectToAction("Details");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Não foi possível registrar! " + ex.Message);
                return View(restaurante);
            }
        }

        public ActionResult ErroAtivacaoDias(double dias)
        {
            ViewBag.MensagemValidacao = String.Format(Startup.MsgValidacao, dias);
            return View();
        }

        public ActionResult ErroAtivacao(string erro)
        {
            ViewBag.MensagemValidacao = erro;
            return View();
        }
    }
}
