import 'package:back_button_interceptor/back_button_interceptor.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:prowaiter_v2/components/botao_rodape.dart';
import 'package:prowaiter_v2/components/circular_progress_padrao.dart';
import 'package:prowaiter_v2/components/numeric_up_down.dart';
import 'package:prowaiter_v2/components/scaffold_padrao.dart';
import 'package:prowaiter_v2/models/bebida_dto_vew.dart';
import 'package:prowaiter_v2/models/entidades_dto.dart';
import 'package:prowaiter_v2/util/app_routes.dart';
import 'package:prowaiter_v2/util/servicos_app_provider.dart';
import 'package:prowaiter_v2/util/util.dart' as util;

class BebidasScreen extends StatefulWidget {
  @override
  _BebidasScreenState createState() => _BebidasScreenState();
}

class _BebidasScreenState extends State<BebidasScreen> {
  ServicosAPPProvider _provider;
  var _observacoesController = TextEditingController();
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
      _carregarBebidas();
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

  List<BebidaDTOView> _bebidasDTOView;
  BebidaDoPedido bebidaDoPedidoAtual;
  Future<List<BebidaDTOView>> _carregarBebidas() async {
    _erro = false;
    _msgErro = "";
    try {
      if (_bebidasDTOView == null) {
        Object argumento = ModalRoute.of(context).settings.arguments;
        String codTipoBebida;
        if (argumento is Categoria) {
          Categoria categoria = ModalRoute.of(context).settings.arguments;
          codTipoBebida = categoria.codigo.toString();
        } else {
          bebidaDoPedidoAtual = ModalRoute.of(context).settings.arguments;
          _observacoesController.text = bebidaDoPedidoAtual.observacoes;
          codTipoBebida = bebidaDoPedidoAtual.bebida.codTipo.toString();
        }

        if (codTipoBebida != null) {
          try {
            List<Bebida> bebidas = await _provider.obterEntidades<Bebida>("Bebidas", queryStringCodigo: "codTipoBebida=$codTipoBebida");

            _bebidasDTOView = bebidas
                .map((e) => BebidaDTOView(
                      e,
                      checked: bebidaDoPedidoAtual == null ? false : e.codigo == bebidaDoPedidoAtual.bebida.codigo,
                    ))
                .toList();
          } catch (e, s) {
            throw ("Erro ao tentar obter os tipos de bebidas." + e.toString() + s.toString());
          }
        }
        setState(() {
          _exibirCarregando = false;
        });
      }
    } catch (e, s) {
      setState(() {
        _erro = true;
        _msgErro = e.toString() + s.toString();
      });
    }
    return _bebidasDTOView;
  }

  void _adicionarBebidasDoPedido() {
    try {
      int indice = -1;
      //Se veio uma edição removemos o atual e criamos novamente
      if (bebidaDoPedidoAtual != null) {
        indice = _provider.pedidoInternoAtual.bebidasDoPedido.indexOf(bebidaDoPedidoAtual);
        _provider.pedidoInternoAtual.bebidasDoPedido.remove(bebidaDoPedidoAtual);
      }

      if (_bebidasDTOView != null) {
        List<BebidaDTOView> bebidasSelecionadas = _bebidasDTOView.where((e) => e.checked).toList();
        if (bebidasSelecionadas.length == 0) {
          util.exibirMensagem(context, "Selecione pelo menos uma bebida");
        } else {
          String obs = _observacoesController.text.isNullOrWhiteSpace() ? null : _observacoesController.text.trim();

          for (int i = 0; i < bebidasSelecionadas.length; i++) {
            var bebidaDoPedido = BebidaDoPedido(
                codPedido: _provider.pedidoInternoAtual.codigo,
                bebida: bebidasSelecionadas[i].bebida,
                codBebida: bebidasSelecionadas[i].bebida.codigo,
                valor: bebidasSelecionadas[i].bebida.valor,
                observacoes: obs,
                chaveParaVincluarModelo: bebidaDoPedidoAtual != null ? bebidaDoPedidoAtual.chaveParaVincluarModelo : null,
                modeloPedido: bebidaDoPedidoAtual != null ? bebidaDoPedidoAtual.modeloPedido : null);

            for (int j = 0; j < _quantidade; j++) {
              if (indice != -1) {
                //se foi uma edição inserimos na mesma posição, caso tenha mais de um, vamos incrementando o indice
                _provider.pedidoInternoAtual.bebidasDoPedido.insert(indice + j, bebidaDoPedido.clone());
              } else {
                _provider.pedidoInternoAtual.bebidasDoPedido.add(bebidaDoPedido.clone());
              }
            }
          }
          Navigator.of(context).pushReplacementNamed(AppRouts.CategoriasRoute);
        }
      }
    } catch (e, s) {
      util.exibirErro(context, e, s);
    }
  }

  @override
  Widget build(BuildContext context) {
    return ScaffoldPadrao(
        exibirBackButton: true, onBackPress: _voltar, titulo: "Bebidas", conteudoBottomAppBar: _criarBotoesRodape(context), body: _criarTela());
  }

  Widget _criarTela() {
    if (_erro) {
      return _criarTelaErro();
    }
    if (_exibirCarregando) {
      return CircularProgressPadrao();
    }
    return FutureBuilder(
        future: _carregarBebidas(),
        builder: (BuildContext context, AsyncSnapshot<List<BebidaDTOView>> snapshot) {
          if (snapshot.hasError) {
            _msgErro = snapshot.error.toString();
            _erro = true;
            return _criarTelaErro();
          } else if (snapshot.hasData) {
            List<BebidaDTOView> lista = snapshot.data;
            return Container(
              padding: const EdgeInsets.all(5),
              child: Column(
                children: <Widget>[
                  Row(
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
                  Padding(
                    padding: const EdgeInsets.fromLTRB(10, 0, 10, 10),
                    child: TextField(
                      controller: _observacoesController,
                      textCapitalization: TextCapitalization.none,
                      decoration: InputDecoration(labelText: "Observacoes"),
                    ),
                  ),
                  Divider(),
                  Expanded(
                    child: ListView.builder(
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
                                    bool checked = lista[i].checked;
                                    if (bebidaDoPedidoAtual != null) {
                                      lista.forEach((e) {
                                        e.checked = false;
                                      });
                                    }
                                    lista[i].checked = !checked;
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
                                            if (bebidaDoPedidoAtual != null) {
                                              lista.forEach((e) {
                                                e.checked = false;
                                              });
                                            }

                                            lista[i].checked = newValue;
                                          });
                                        },
                                      ),
                                      Expanded(child: Text(lista[i].bebida.nome)),
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
          } else {
            return _erro ? _criarTelaErro() : CircularProgressPadrao();
          }
        });
  }

  Widget _criarTelaErro() {
    return util.obterWidgetErroParaTela(context, _msgErro, refresh: () {
      _bebidasDTOView = null;
      setState(() {
        _exibirCarregando = true;
      });
      _carregarBebidas();
    });
  }

  Widget _criarBotoesRodape(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: <Widget>[
        BotaoRodape(
          texto: "Voltar",
          onPressed: _exibirCarregando ? null : _voltar,
        ),
        BotaoRodape(texto: "OK", onPressed: _exibirCarregando ? null : _adicionarBebidasDoPedido),
      ],
    );
  }

  void _voltar() {
    if (_exibirCarregando) return;
    Navigator.of(context).pushReplacementNamed(AppRouts.CategoriasRoute);
  }
}
