import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:prowaiter_v2/components/botao_rodape.dart';
import 'package:back_button_interceptor/back_button_interceptor.dart';
import 'package:prowaiter_v2/components/circular_progress_padrao.dart';
import 'package:prowaiter_v2/components/numeric_up_down.dart';
import 'package:prowaiter_v2/components/scaffold_padrao.dart';
import 'package:prowaiter_v2/models/entidades_dto.dart';
import 'package:prowaiter_v2/models/modelo_dto_view.dart';
import 'package:prowaiter_v2/util/app_routes.dart';
import 'package:prowaiter_v2/util/servicos_app_provider.dart';
import 'package:prowaiter_v2/util/util.dart' as util;

class ModelosScreen extends StatefulWidget {
  @override
  _ModelosScreenState createState() => _ModelosScreenState();
}

class _ModelosScreenState extends State<ModelosScreen> {
  ServicosAPPProvider _provider;
  int _quantidade = 1;
  bool _erro = false;
  String _msgErro = "";
  bool _exibirCarregando = true;

  @override
  void initState() {
    super.initState();
    _provider = Provider.of<ServicosAPPProvider>(context, listen: false);
    BackButtonInterceptor.add(_backButton);
    Future.delayed(Duration.zero, () {
      _carregarModelos();
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

  List<ModeloDTOView> _modelos;
  Future<List<ModeloDTOView>> _carregarModelos() async {
    _erro = false;
    _msgErro = "";
    try {
      if (_modelos == null) {
        try {
          List<ModeloPedido> modelos = await _provider.obterEntidades<ModeloPedido>("ModelosPedido");          
          _modelos = modelos.map((e) => ModeloDTOView(e, checked: false)).toList();
        } catch (e, s) {
          throw ("Erro ao tentar obter os modelos de pedidos ${e.toString()}${s.toString()}");
        }
        setState(() {
          _exibirCarregando = false;
        });
      }
    } catch (e, s) {
      setState(() {
        _msgErro = e.toString() + s.toString();
        _erro = true;
      });
    }
    return _modelos;
  }

  void _adicionarItensDoModelo() async {
    try {
      for (var modelo in _modelos.where((e) => e.checked)) {
        for (int i = 0; i < _quantidade; i++) {
          _provider.acrescimoAtual += modelo.modeloPedido.acrescimo;
          _provider.descontoAtual += modelo.modeloPedido.desconto;
          
          var key = GlobalKey();
          for (var beb in modelo.modeloPedido.modelosBebidaPedido) {            
            _provider.pedidoInternoAtual.bebidasDoPedido.add(BebidaDoPedido(
              bebida: beb.bebida,
              codBebida: beb.codBebida,
              codPedido: _provider.pedidoInternoAtual.codigo,
              valor: beb.bebida.valor,
              observacoes: beb.observacoes,
              chaveParaVincluarModelo: key,
              modeloPedido: modelo.modeloPedido,
            ));
          }

          for (var ref in modelo.modeloPedido.modelosRefeicaoPedidos) {  

            RefeicaoDoCardapio refCardapio = await  _provider.recuperar<RefeicaoDoCardapio>(
              "RefeicoesDoCardapio", "codRefeicao=${ref.codRefeicao}&codTamanho=${ref.codTamanho}");

            var novaRefPedido = RefeicaoDoPedido(
              codPedido: _provider.pedidoInternoAtual.codigo,
              codRefeicao: ref.codRefeicao,
              codTamanho: ref.codTamanho,
              observacoes: ref.observacoes,                            
              valor: refCardapio.valor,
              refeicaoDoCardapio: refCardapio,
              componentesRefeicaoPedido:[],
              chaveParaVincluarModelo: key,
              modeloPedido: modelo.modeloPedido,
            );
            for (var comp in ref.modeloComponentesRefeicaoPedido) {
              novaRefPedido.componentesRefeicaoPedido.add(ComponenteRefeicaoPedido(
                codComponente: comp.codComponente,
                quantidade: comp.quantidade,
              ));
            }
            _provider.pedidoInternoAtual.refeicoesDoPedido.add(novaRefPedido);
          }
        }
      }
      Navigator.of(context).popAndPushNamed(AppRouts.CategoriasRoute);
    } catch (e, s) {
      util.exibirErro(context, e, s);
    }
  }

  @override
  Widget build(BuildContext context) {
    return ScaffoldPadrao(
      exibirBackButton: true,
      onBackPress: _voltar,
      titulo: "Modelos",
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
      future: _carregarModelos(),
      builder: (BuildContext context, AsyncSnapshot<List<ModeloDTOView>> snapshot) {
        if (snapshot.hasError) {
          _msgErro = snapshot.error.toString();
          _erro = true;
          return _criarTelaErro();
        }
        if (snapshot.hasData) {
          List<ModeloDTOView> lista = snapshot.data;
          return Container(
            child: Column(
              children: <Widget>[
                Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Row(
                    children: <Widget>[
                      Text("Quantidade:", style: TextStyle(fontSize: 20)),
                      NumericUpDown(
                        decimalPlaces: 0,
                        initialValue: _quantidade,
                        maxValue: 99,
                        minValue: 1,
                        onChanged: (args) {
                          _quantidade = args.quantidade;
                        },
                      ),
                    ],
                  ),
                ),
                Divider(),
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
                            child: InkWell(
                              onTap: () {
                                setState(() {
                                  lista[i].checked = !lista[i].checked;
                                });
                              },
                              child: Padding(
                                padding: EdgeInsets.all(5),
                                child: Row(
                                  children: <Widget>[
                                    Checkbox(
                                      value: lista[i].checked,
                                      onChanged: (bool newValue) {
                                        setState(() {
                                          lista[i].checked = newValue;
                                        });
                                      },
                                    ),
                                    Expanded(child: Text(lista[i].modeloPedido.nome)),
                                  ],
                                ),
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
        BotaoRodape(texto: "Adicionar", onPressed: _exibirCarregando ? null : _adicionarItensDoModelo),
      ],
    );
  }

  Widget _criarTelaErro() {
    return util.obterWidgetErroParaTela(context, _msgErro, refresh: () {
      _modelos = null;
      setState(() {
        _exibirCarregando = true;
      });
      _carregarModelos();
    });
  }

  void _voltar() {
    if (_exibirCarregando) return;
    Navigator.of(context).pushReplacementNamed(AppRouts.CategoriasRoute);
  }
}
