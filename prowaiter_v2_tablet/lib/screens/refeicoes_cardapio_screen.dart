import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:prowaiter_v2/components/botao_rodape.dart';
import 'package:back_button_interceptor/back_button_interceptor.dart';
import 'package:prowaiter_v2/components/circular_progress_padrao.dart';
import 'package:prowaiter_v2/components/filtro.dart';
import 'package:prowaiter_v2/components/scaffold_padrao.dart';
import 'package:prowaiter_v2/models/entidades_dto.dart';
import 'package:prowaiter_v2/util/app_routes.dart';
import 'package:prowaiter_v2/util/servicos_app_provider.dart';
import 'package:prowaiter_v2/util/util.dart' as util;

class RefeicoesCardapioScreen extends StatefulWidget {
  @override
  _RefeicoesCardapioScreenState createState() => _RefeicoesCardapioScreenState();
}

class _RefeicoesCardapioScreenState extends State<RefeicoesCardapioScreen> {
  ServicosAPPProvider _provider;
  bool _erro = false;
  String _msgErro = "";
  bool _exibirCarregando = true;
  String _filtro = "";

  @override
  initState() {
    super.initState();
    _provider = Provider.of<ServicosAPPProvider>(context, listen: false);
    BackButtonInterceptor.add(_backButton);
    Future.delayed(Duration.zero, () {
      _carregarRefeicoesDoCardapio();
    });
  }

  @override
  void dispose() {
    super.dispose();
    BackButtonInterceptor.remove(_backButton);
  }

  bool _backButton(bool stopDefaultButtonEvent, RouteInfo routeInfo) {
    _voltar();
    return true;
  }

  List<Refeicao> _refeicoes;
  Future<List<Refeicao>> _carregarRefeicoesDoCardapio() async {
    _erro = false;
    _msgErro = "";
    try {
      if (_refeicoes == null) {
        Categoria categoria = ModalRoute.of(context).settings.arguments;
        if (categoria != null) {
          try {
            List<RefeicaoDoCardapio> refeicoesDoCardapio =
                await _provider.obterEntidades<RefeicaoDoCardapio>("RefeicoesDoCardapio", queryStringCodigo: "codTipoRefeicao=${categoria.codigo}");

            _refeicoes = refeicoesDoCardapio.map((e) => e.refeicao).toList().toSet().toList();
          } catch (e, s) {
            throw ("Erro ao tentar obter os tipos de bebidas ${e.toString()}${s.toString()}");
          }
        }
        setState(() {
          _exibirCarregando = false;
        });
      }

      if (!_filtro.isNullOrWhiteSpace()) {
        List<Refeicao> refeicoesFiltradas = _refeicoes.where((e) => e.nome.toUpperCase().contains(_filtro.toUpperCase().trim())).toList();
        setState(() {});
        return refeicoesFiltradas;
      }
    } catch (e, s) {
      setState(() {
        _msgErro = e.toString() + s.toString();
        _erro = true;
      });
    }

    return _refeicoes;
  }

  @override
  Widget build(BuildContext context) {
    return ScaffoldPadrao(
      exibirBackButton: true,
      onBackPress: _voltar,
      titulo: "Refeições do cardápio",
      conteudoBottomAppBar: _criarBotoesRodape(context),
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
      future: _carregarRefeicoesDoCardapio(),
      builder: (BuildContext context, AsyncSnapshot<List<Refeicao>> snapshot) {
        if(snapshot.hasError){
          _msgErro = snapshot.error.toString();
          _erro = true;
          return _criarTelaErro();
        }
        if (snapshot.hasData) {
          List<Refeicao> lista = snapshot.data;
          return Container(
            child: Column(
              children: <Widget>[
                Filtro(
                  onFilterChanged: (valor) {
                    setState(() {
                      _filtro = valor;
                    });
                  },
                ),
                Expanded(
                  child: ListView.builder(
                    scrollDirection: Axis.vertical,
                    shrinkWrap: true,
                    itemCount: lista.length,
                    itemBuilder: (BuildContext context, int i) {
                      return Column(
                        children: <Widget>[
                          Container(
                            margin: EdgeInsets.all(0),
                            decoration: BoxDecoration(border: Border(bottom: BorderSide(color: Colors.grey[300]))),
                            child: ListTile(
                              onTap: () {
                                _irParaProximaTela(lista[i]);
                              },
                              title: Text(
                                lista[i].nome,
                              ),
                            ),
                          ),
                        ],
                      );
                    },
                  ),
                ),
              ],
            ),
          );
        }
        return _erro ? _criarTelaErro() : CircularProgressPadrao();
      },
    );
  }

  Widget _criarBotoesRodape(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: <Widget>[
        BotaoRodape(
          texto: "Voltar",
          onPressed: _exibirCarregando ? null : _voltar,
        ),
        BotaoRodape(texto: "Revisar", onPressed: _exibirCarregando ? null : _irParaDetalhesDoPedido),
      ],
    );
  }

  Widget _criarTelaErro() {
    return util.obterWidgetErroParaTela(context, _msgErro, refresh: () {
      _refeicoes = null;
      setState(() {
        _exibirCarregando = true;
      });
      _carregarRefeicoesDoCardapio();
    });
  }

  void _voltar() {
    if (_exibirCarregando) return;
    Navigator.of(context).pushReplacementNamed(AppRouts.CategoriasRoute);
  }

  void _irParaDetalhesDoPedido() {
    Navigator.of(context).pushReplacementNamed(AppRouts.DetalhesPedidoRoute);
  }

  void _irParaProximaTela(Refeicao ref) {
    Navigator.of(context).pushReplacementNamed(AppRouts.ComponentesRefeicaoRoute, arguments: ref);
  }
}
