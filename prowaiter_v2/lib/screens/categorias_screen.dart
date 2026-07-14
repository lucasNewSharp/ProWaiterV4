import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:back_button_interceptor/back_button_interceptor.dart';
import 'package:prowaiter_v2/components/botao_rodape.dart';
import '../util/servicos_app_provider.dart';
import '../models/entidades_dto.dart';
import '../components/scaffold_padrao.dart';
import '../util/app_routes.dart';
import "../components/circular_progress_padrao.dart";
import '../util/util.dart' as util;

class CategoriasScreen extends StatefulWidget {
  @override
  _CategoriasScreenState createState() => _CategoriasScreenState();
}

class _CategoriasScreenState extends State<CategoriasScreen> {
  List<Categoria> _categorias;
  ServicosAPPProvider _provider;
  bool _erro = false;
  String _msgErro = "";
  bool _exibirCarregando = true;

  @override
  void initState() {
    super.initState();
    BackButtonInterceptor.add(_backButton);
    _provider = Provider.of<ServicosAPPProvider>(context, listen: false);
    _carregarCategorias();
  }

  @override
  void dispose() {
    super.dispose();
    BackButtonInterceptor.remove(_backButton);
  }

  bool _backButton(bool stopDefaultButtonEvent, RouteInfo routeInfo) {
    _voltarTelaMesas();
    return true;
  }

  Future<List<Categoria>> _carregarCategorias() async {
    _erro = false;
    _msgErro = "";
    try {
      if (_categorias == null) {
        List<TipoRefeicao> tiposRefeicao;
        List<TipoBebida> tiposBebida;
        try {
          tiposBebida = await _provider.obterEntidades<TipoBebida>("TipoBebidas");
        } catch (e, s) {
          throw ("Erro ao tentar obter os tipos de bebidas. " + e.toString() + s.toString());
        }
        try {
          tiposRefeicao = await _provider.obterEntidades<TipoRefeicao>("TiposRefeicoes");
        } catch (e, s) {
          throw ("Erro ao tentar obter os tipos de refeição." + e.toString() + s.toString());
        }

        CategoriaModelo categoriaModelo;
        try {
          bool existeModelo = await _provider.recuperarBool("ModelosPedido/ExisteModelo");
          if (existeModelo) {
            categoriaModelo = new CategoriaModelo();
            categoriaModelo.nome = "Modelos";
            categoriaModelo.corFundo = "#000000";
            categoriaModelo.corFonte = "#FFFFFF";
            categoriaModelo.codigo = -1;
          }
        } catch (e, s) {
          throw ("Erro ao tentar obter informação sobre modelo." + e.toString() + s.toString());
        }

        _categorias = [];
        _categorias.addAll(tiposRefeicao.where((element) => element.posicao != null));
        _categorias.addAll(tiposBebida.where((element) => element.posicao != null));
        _categorias.sort((a, b) => (a.posicao < b.posicao ? -1 : 1));
        _categorias.addAll(tiposRefeicao.where((element) => element.posicao == null));
        _categorias.addAll(tiposBebida.where((element) => element.posicao == null));
        if (categoriaModelo != null) {
          _categorias.add(categoriaModelo);
        }
        setState(() {
          _exibirCarregando = false;
        });
      }
    } catch (e, s) {
      _categorias = null;
      _msgErro = e.toString() + s.toString();
      setState(() {
        _erro = true;
      });
    }

    return _categorias;
  }

  @override
  Widget build(BuildContext context) {
    return ScaffoldPadrao(
      onBackPress: _voltarTelaMesas,
      exibirBackButton: true,
      titulo: "Categorias" + (!_provider.pedidoInternoAtual.observacoes.isNullOrWhiteSpace() ? " - ${_provider.pedidoInternoAtual.observacoes}" : ""),
      conteudoBottomAppBar: _criarBotoesRodape(),
      body: _criarTela(),
    );
  }

  Widget _criarTela() {
    if (_erro) {
      return _criarTelaErro();
    }
    if (_exibirCarregando) {
      return CircularProgressPadrao();
    }
    return FutureBuilder(
      future: _carregarCategorias(),
      builder: (BuildContext context, AsyncSnapshot<List<Categoria>> snapshot) {
        if (snapshot.hasError) {
          return _criarTelaErro();
        } else if (snapshot.hasData) {
          List<Categoria> lista = snapshot.data;
          return ListView.builder(
            itemCount: lista.length,
            itemBuilder: (BuildContext context, int i) {
              int corFundo = int.parse("0xFF${lista[i].corFundo.replaceFirst("#", "")}");
              int corFonte = int.parse("0xFF${lista[i].corFonte.replaceFirst("#", "")}");
              return Column(
                children: <Widget>[
                  Container(
                    margin: EdgeInsets.all(0),
                    decoration: BoxDecoration(color: Color(corFundo), border: Border(bottom: BorderSide(color: Colors.grey[300]))),
                    child: ListTile(
                      onTap: () {
                        _irParaProximaTela(lista[i]);
                      },
                      title: Text(
                        lista[i].nome,
                        style: TextStyle(color: Color(corFonte)),
                      ),
                    ),
                  ),
                ],
              );
            },
          );
        }
        return _erro ? _criarTelaErro() : CircularProgressPadrao();
      },
    );
  }

  Widget _criarBotoesRodape() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: <Widget>[
        BotaoRodape(texto: "Voltar", onPressed: _exibirCarregando ? null : _voltarTelaMesas),
        BotaoRodape(
          texto: "Revisar",
          onPressed: _exibirCarregando ? null : _irParaTelaDetalhesPedido,
        )
      ],
    );
  }

  Widget _criarTelaErro() {
    return util.obterWidgetErroParaTela(context, _msgErro, refresh: () {
      _categorias = null;
      setState(() {
        _exibirCarregando = true;
      });
      _carregarCategorias();
    });
  }

  void _irParaTelaDetalhesPedido() {
    Navigator.of(context).pushReplacementNamed(AppRouts.DetalhesPedidoRoute);
  }

  void _voltarTelaMesas() {
    if (!_exibirCarregando) {
      if (_provider.pedidoInternoAtual.possuiItensNaoEnviados) {
        util.dialogoPerguntar(
            context: context,
            mensagem: "Existem itens não enviados a cozinha. Deseja exibir os detalhes do pedido?",
            aoClicarSim: _irParaDetalhesPedido);
      } else {
        Navigator.of(context).pushReplacementNamed(AppRouts.MesasRoute);
      }
    }
  }

  void _irParaDetalhesPedido() {
    Navigator.of(context).pushReplacementNamed(AppRouts.DetalhesPedidoRoute);
  }

  void _irParaProximaTela(Categoria categoria) {
    String rota;
    if (categoria is TipoRefeicao) {
      rota = AppRouts.RefeicoesCardapioRoute;
    } else if (categoria is TipoBebida) {
      rota = AppRouts.BebidasRoute;
    } else {
      rota = AppRouts.ModelosRoute;
    }

    Navigator.of(context).pushReplacementNamed(rota, arguments: categoria);
  }
}
