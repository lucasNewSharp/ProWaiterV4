using NewSharp.BancoDeDados;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Gestores
{
    public class GestoresEntidades
    {
        public ProWaiterContextBDProvider ContextoBDProvider { get; private set; }        

        #region Gestores
        private Dictionary<Type, object> _gEntidades = new Dictionary<Type, object>();
        public GestorEntidadeBD<TEntidade> ObterGestorEntidadeBD<TEntidade>()
            where TEntidade : class, IValidatableObject
        {
            Type tEntidade = typeof(TEntidade);
            if (_gEntidades.ContainsKey(tEntidade))
                return (GestorEntidadeBD<TEntidade>)_gEntidades[tEntidade];
            else
            {
                Type tGestor = typeof(GestorEntidadeBD<>);
                tGestor = tGestor.MakeGenericType(tEntidade);

                foreach (PropertyInfo pInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    if (pInfo.PropertyType == tGestor)
                    {
                        _gEntidades.Add(tEntidade, pInfo.GetValue(this, null));
                        return (GestorEntidadeBD<TEntidade>)_gEntidades[tEntidade];
                    }
            }
            _gEntidades.Add(tEntidade, null);
            return null;
        }

        private GestorEntidadeBD<Mesa> _gMesas = null;
        public GestorEntidadeBD<Mesa> gMesas
        {
            get
            {
                if (_gMesas == null)
                    _gMesas = new GestorEntidadeBD<Mesa>(ContextoBDProvider);
                return _gMesas;
            }
        }

        private GestorEntidadeBD<Cidade> _gCidades = null;
        public GestorEntidadeBD<Cidade> gCidades
        {
            get
            {
                if (_gCidades == null)
                    _gCidades = new GestorEntidadeBD<Cidade>(ContextoBDProvider);
                return _gCidades;
            }
        }

        private GestorEntidadeBD<ComponenteRefeicao> _gComponentesRefeicao = null;
        public GestorEntidadeBD<ComponenteRefeicao> gComponentesRefeicao
        {
            get
            {
                if (_gComponentesRefeicao == null)
                    _gComponentesRefeicao = new GestorEntidadeBD<ComponenteRefeicao>(ContextoBDProvider);
                return _gComponentesRefeicao;
            }
        }

        private GestorEntidadeBD<Refeicao> _gRefeicoes = null;
        public GestorEntidadeBD<Refeicao> gRefeicoes
        {
            get
            {
                if (_gRefeicoes == null)
                    _gRefeicoes = new GestorEntidadeBD<Refeicao>(ContextoBDProvider);
                return _gRefeicoes;
            }
        }

        private GestorEntidadeBD<TamanhoRefeicao> _gTamanhosRefeicao = null;
        public GestorEntidadeBD<TamanhoRefeicao> gTamanhosRefeicao
        {
            get
            {
                if (_gTamanhosRefeicao == null)
                    _gTamanhosRefeicao = new GestorEntidadeBD<TamanhoRefeicao>(ContextoBDProvider);
                return _gTamanhosRefeicao;
            }
        }

        private GestorEntidadeBD<TipoRefeicao> _gTiposRefeicao = null;
        public GestorEntidadeBD<TipoRefeicao> gTiposRefeicao
        {
            get
            {
                if (_gTiposRefeicao == null)
                    _gTiposRefeicao = new GestorEntidadeBD<TipoRefeicao>(ContextoBDProvider);
                return _gTiposRefeicao;
            }
        }

        private GestorEntidadeBD<UF> _gUFs = null;
        public GestorEntidadeBD<UF> gUFs
        {
            get
            {
                if (_gUFs == null)
                    _gUFs = new GestorEntidadeBD<UF>(ContextoBDProvider);
                return _gUFs;
            }
        }


        private GestorEntidadeBD<Bebida> _gBebidas = null;
        public GestorEntidadeBD<Bebida> gBebidas
        {
            get
            {
                if (_gBebidas == null)
                    _gBebidas = new GestorEntidadeBD<Bebida>(ContextoBDProvider);
                return _gBebidas;
            }
        }
        
        private GestorEntidadeBD<Pedido> _gPedidos = null;
        public GestorEntidadeBD<Pedido> gPedidos
        {
            get
            {
                if (_gPedidos == null)
                    _gPedidos = new GestorEntidadeBD<Pedido>(ContextoBDProvider);
                return _gPedidos;
            }
        }

        private GestorEntidadeBD<Cliente> _gClientes = null;
        public GestorEntidadeBD<Cliente> gClientes
        {
            get
            {
                if (_gClientes == null)
                    _gClientes = new GestorEntidadeBD<Cliente>(ContextoBDProvider);
                return _gClientes;
            }
        }

        private GestorEntidadeBD<BebidaDoPedido> _gBebidasDosPedidos = null;
        public GestorEntidadeBD<BebidaDoPedido> gBebidasDosPedidos
        {
            get
            {
                if (_gBebidasDosPedidos == null)
                    _gBebidasDosPedidos = new GestorEntidadeBD<BebidaDoPedido>(ContextoBDProvider);
                return _gBebidasDosPedidos;
            }
        }

        private GestorEntidadeBD<RefeicaoDoCardapio> _gRefeicoesDoCardapio = null;
        public GestorEntidadeBD<RefeicaoDoCardapio> gRefeicoesDoCardapio
        {
            get
            {
                if (_gRefeicoesDoCardapio == null)
                    _gRefeicoesDoCardapio = new GestorEntidadeBD<RefeicaoDoCardapio>(ContextoBDProvider);
                return _gRefeicoesDoCardapio;
            }
        }

        private GestorEntidadeBD<RefeicaoDoPedido> _gRefeicoesDoPedido = null;
        public GestorEntidadeBD<RefeicaoDoPedido> gRefeicoesDoPedido
        {
            get
            {
                if (_gRefeicoesDoPedido == null)
                    _gRefeicoesDoPedido = new GestorEntidadeBD<RefeicaoDoPedido>(ContextoBDProvider);
                return _gRefeicoesDoPedido;
            }
        }

        private GestorEntidadeBD<TipoBebida> _gTiposBebida = null;
        public GestorEntidadeBD<TipoBebida> gTiposBebida
        {
            get
            {
                if (_gTiposBebida == null)
                    _gTiposBebida = new GestorEntidadeBD<TipoBebida>(ContextoBDProvider);
                return _gTiposBebida;
            }
        }


        private GestorEntidadeBD<Impressora> _gImpressoras = null;
        public GestorEntidadeBD<Impressora> gImpressoras
        {
            get
            {
                if (_gImpressoras == null)
                    _gImpressoras = new GestorEntidadeBD<Impressora>(ContextoBDProvider);
                return _gImpressoras;
            }
        }

        private GestorEntidadeBD<Configuracao> _gConfiguracoes = null;
        public GestorEntidadeBD<Configuracao> gConfiguracoes
        {
            get
            {
                if (_gConfiguracoes == null)
                    _gConfiguracoes = new GestorEntidadeBD<Configuracao>(ContextoBDProvider);
                return _gConfiguracoes;
            }
        }

        private GestorEntidadeBD<ComponenteComposicaoRefeicaoCardapio> _gComponentesComposicaoRefeicaoCardapio = null;
        public GestorEntidadeBD<ComponenteComposicaoRefeicaoCardapio> gComponentesComposicaoRefeicaoCardapio
        {
            get
            {
                if (_gComponentesComposicaoRefeicaoCardapio == null)
                    _gComponentesComposicaoRefeicaoCardapio = new GestorEntidadeBD<ComponenteComposicaoRefeicaoCardapio>(ContextoBDProvider);
                return _gComponentesComposicaoRefeicaoCardapio;
            }
        }


        #endregion

        public GestoresEntidades(ProWaiterContextBDProvider contextoBDProvider)
        {
            if (contextoBDProvider == null)
                throw new ArgumentNullException("contextoBDProvider");
            ContextoBDProvider = contextoBDProvider;            
        }
    }
}
