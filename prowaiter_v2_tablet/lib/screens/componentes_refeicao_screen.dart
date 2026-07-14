import 'package:back_button_interceptor/back_button_interceptor.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:prowaiter_v2/components/botao_rodape.dart';
import 'package:prowaiter_v2/components/circular_progress_padrao.dart';
import 'package:prowaiter_v2/components/filtro.dart';
import 'package:prowaiter_v2/components/numeric_up_down.dart';
import 'package:prowaiter_v2/components/scaffold_padrao.dart';
import 'package:prowaiter_v2/models/entidades_dto.dart';
import 'package:prowaiter_v2/util/app_routes.dart';
import 'package:prowaiter_v2/util/servicos_app_provider.dart';
import 'package:prowaiter_v2/models/refeicao_do_pedido_dto_view.dart';
import 'package:prowaiter_v2/util/util.dart' as util;

class ComponentesRefeicaoScreen extends StatefulWidget {
  @override
  _ComponentesRefeicaoScreenState createState() => _ComponentesRefeicaoScreenState();
}

class _ComponentesRefeicaoScreenState extends State<ComponentesRefeicaoScreen> {
  ServicosAPPProvider _provider;
  bool _editar = false;
  var _observacoesController = TextEditingController();
  int _quantidade = 1;
  String _codTamanhoSelecionado;
  bool _erro = false;
  String _msgErro = "";
  bool _exibirCarregando = true;

  RefeicaoDoPedidoDTOView _refeicaoDoPedidoDTOView;
  RefeicaoDoPedido _refeicaoDoPedidoParam;
  RefeicaoDoCardapio _refCardapio;
  Map<ComponenteComposicaoRefeicaoCardapio, int> _nudsComponentesComValor = {};
  Map<ComponenteComposicaoRefeicaoCardapio, int> _nudsAExibir = {};

  @override
  void initState() {
    super.initState();
    _provider = Provider.of<ServicosAPPProvider>(context, listen: false);
    BackButtonInterceptor.add(_backButton);
    Future.delayed(Duration.zero, () {
      _carregarRefeicaoDoCardapio();
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

  Future<RefeicaoDoPedidoDTOView> _carregarRefeicaoDoCardapio() async {
    _erro = false;
    _msgErro = "";

    try {
      if (_refeicaoDoPedidoDTOView == null) {
        Refeicao refeicaoParam;
        Object argumento = ModalRoute.of(context).settings.arguments;
        if (argumento is RefeicaoDoPedido) {
          //Se no argumento veio uma refeião do 'PEDIDO' estamos editando
          _editar = true;
          _refeicaoDoPedidoParam = argumento;
          refeicaoParam = _refeicaoDoPedidoParam.refeicaoDoCardapio.refeicao;
        } else {
          refeicaoParam = argumento;
        }

        List<TamanhoRefeicao> tamanhos = await _obterTamanhos(refeicaoParam);

        if (tamanhos != null && tamanhos.length > 0) {
          if (_codTamanhoSelecionado == null) {
            _codTamanhoSelecionado = tamanhos[0].codigo;
          }

          _refCardapio = await _provider.recuperar<RefeicaoDoCardapio>(
              "RefeicoesDoCardapio", "codRefeicao=${refeicaoParam.codigo}&codTamanho=$_codTamanhoSelecionado");

          _refeicaoDoPedidoDTOView = RefeicaoDoPedidoDTOView(
            codRefeicao: refeicaoParam.codigo,
            nome: refeicaoParam.nome,
            tamanhos: tamanhos,
          );

          if (!_refCardapio.deComposicao) {
            _refeicaoDoPedidoDTOView.componentes = await _obterComponentes(_refCardapio);
          } else {
            _refeicaoDoPedidoDTOView.componentesComposicao = await _obterComponentesComposicao(_refCardapio);
            _nudsAExibir = {for (var item in _refeicaoDoPedidoDTOView.componentesComposicao) item: 0};
            //caso nos exista algo nas nuds com valor, que não existe mais no novo tamanho temos que remover
          }

          //Carregamos os dados caso esteja editando algo
          if (_editar) {
            _codTamanhoSelecionado = _refeicaoDoPedidoParam.codTamanho;
            _observacoesController.text = _refeicaoDoPedidoParam.observacoes;

            if (_refeicaoDoPedidoParam.refeicaoDoCardapio.deComposicao) {
              if (_nudsComponentesComValor.length == 0) {
                //só carregamos na primeira vez, mesmo trocando o tamanho a cache dos itens vai estar na memória
                for (var item in _refeicaoDoPedidoDTOView.componentesComposicao) {
                  var comp =
                      _refeicaoDoPedidoParam.componentesRefeicaoPedido.singleWhere((i) => i.codComponente == item.codComponente, orElse: () => null);
                  if (comp != null && comp.quantidade > 0) {
                    _nudsComponentesComValor.putIfAbsent(item, () => comp.quantidade);
                  }
                }
              }
            } else {
              List<int> codComponentesPedido = _refeicaoDoPedidoParam.componentesRefeicaoPedido.map((e) => e.codComponente).toList();
              for (var item in _refeicaoDoPedidoDTOView.componentes) {
                if (codComponentesPedido.contains(item.componente.codigo)) {
                  item.checked = true;
                } else {
                  item.checked = false;
                }
              }
            }
          }
          _limparCache();
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
    return _refeicaoDoPedidoDTOView;
  }

  void _limparCache() {
    //caso nos exista algo nas nuds com valor, que não existe mais no novo tamanho temos que remover
    if (_nudsComponentesComValor == null) {
      return;
    }

    List<ComponenteComposicaoRefeicaoCardapio> remover = [];
    for (var item in _nudsComponentesComValor.keys) {
      bool existe = false;
      for (var novoItem in _nudsAExibir.keys) {
        if (item.codComponente == novoItem.codComponente) {
          existe = true;
          break;
        }
      }
      if (!existe) {
        remover.add(item);
      }
    }
    for (var item in remover) {
      _nudsComponentesComValor.remove(item);
    }
  }

  Map<ComponenteComposicaoRefeicaoCardapio, int> _ordenarComponentesComposicaoComValor(Map<ComponenteComposicaoRefeicaoCardapio, int> map) {
    List<ComponenteComposicaoRefeicaoCardapio> lista = map.keys.toList();
    lista.sort((a, b) => a.componenteRefeicao.nome.compareTo(b.componenteRefeicao.nome));

    Map<ComponenteComposicaoRefeicaoCardapio, int> novoMap = {};
    for (var comp in lista) {
      novoMap.putIfAbsent(comp, () => map[comp]);
    }

    return novoMap;
  }

  void _adicionarRefeicaoDoPedido() async {
    try {
      setState(() {
        _exibirCarregando = true;
      });
      if (_refeicaoDoPedidoDTOView != null) {
        //se for edição excluimos o que existe e adiconamos na mesma posição um novo
        int indice = -1;
        if (_editar) {
          indice = _provider.pedidoInternoAtual.refeicoesDoPedido.indexOf(_refeicaoDoPedidoParam);
          _provider.pedidoInternoAtual.refeicoesDoPedido.remove(_refeicaoDoPedidoParam);
        }

        RefeicaoDoCardapio refDoCardapio = await _provider.recuperar<RefeicaoDoCardapio>(
            "RefeicoesDoCardapio", "codRefeicao=${_refeicaoDoPedidoDTOView.codRefeicao}&codTamanho=$_codTamanhoSelecionado");

        String obs = _observacoesController.text.isNullOrWhiteSpace() ? null : _observacoesController.text.trim();
        RefeicaoDoPedido novaRefeicao = RefeicaoDoPedido(
            codPedido: _provider.pedidoInternoAtual.codigo,
            codRefeicao: _refeicaoDoPedidoDTOView.codRefeicao,
            codTamanho: _codTamanhoSelecionado,
            observacoes: obs,
            valor: refDoCardapio.valor,
            refeicaoDoCardapio: refDoCardapio,
            chaveParaVincluarModelo: _refeicaoDoPedidoParam != null ? _refeicaoDoPedidoParam.chaveParaVincluarModelo : null,
            modeloPedido: _refeicaoDoPedidoParam != null ? _refeicaoDoPedidoParam.modeloPedido : null,
            componentesRefeicaoPedido: []);

        if (_refCardapio.deComposicao) {
          _nudsComponentesComValor.forEach((key, value) {
            novaRefeicao.componentesRefeicaoPedido.add(ComponenteRefeicaoPedido(codComponente: key.componenteRefeicao.codigo, quantidade: value));
          });
        } else {
          novaRefeicao.componentesRefeicaoPedido = _refeicaoDoPedidoDTOView.componentes
              .where((element) => element.checked)
              .map((e) => ComponenteRefeicaoPedido(
                    codComponente: e.componente.codigo,
                    quantidade: 1,
                  ))
              .toList();
        }

        for (int i = 0; i < _quantidade; i++) {
          if (indice != -1) {
            _provider.pedidoInternoAtual.refeicoesDoPedido.insert(indice + i, novaRefeicao.clone());
          } else {
            _provider.pedidoInternoAtual.refeicoesDoPedido.add(novaRefeicao.clone());
          }
        }

        Navigator.of(context).pushReplacementNamed(AppRouts.CategoriasRoute);
      }
    } catch (e, s) {
      util.exibirErro(context, e, s);
    }
    setState(() {
      _exibirCarregando = false;
    });
  }

  NumericUpDown _criarNudComposicao(int valorInicial, ComponenteComposicaoRefeicaoCardapio comp) {
    //se existe um item na memória com valor setado, temos que ajustar o valor inicial
    if (_nudsComponentesComValor != null) {
      for (var itemMemoria in _nudsComponentesComValor.keys) {
        if (comp.codComponente == itemMemoria.codComponente) {
          valorInicial = _nudsComponentesComValor[itemMemoria];
        }
      }
    }

    return NumericUpDown(
      decimalPlaces: 0,
      initialValue: valorInicial,
      maxValue: comp.codUnidade == null ? 1 : 99,
      minValue: 0,
      step: 1,
      texto: comp.componenteRefeicao.nome,
      objetoReferenciado: comp,
      onChanged: (args) {
        _nudsComponentesComValor.remove(args.objetoReferenciado);
        if (args.quantidade > 0) {
          _nudsComponentesComValor.putIfAbsent(args.objetoReferenciado, () => args.quantidade);
        }
        _nudsComponentesComValor = _ordenarComponentesComposicaoComValor(_nudsComponentesComValor);
        setState(() {});
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return ScaffoldPadrao(
        exibirBackButton: true,
        onBackPress: _voltar,
        titulo: "Componentes da refeição",
        conteudoBottomAppBar: _criarBotoesRodape(context),
        body: _criarTela());
  }

  Widget _criarTela() {
    if (_erro) {
      return _criarTelaErro();
    }
    if (_exibirCarregando) {
      return CircularProgressPadrao();
    }
    return FutureBuilder(
      future: _carregarRefeicaoDoCardapio(),
      builder: (BuildContext context, AsyncSnapshot<RefeicaoDoPedidoDTOView> snapshot) {
        if (snapshot.hasError) {
          _erro = true;
          _msgErro = snapshot.error.toString();
          return _criarTelaErro();
        } else if (snapshot.hasData) {
          RefeicaoDoPedidoDTOView refeicao = snapshot.data;
          return SingleChildScrollView(
            child: Padding(
              padding: const EdgeInsets.all(10),
              child: Column(
                children: <Widget>[
                  Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: Center(
                      child: Text(
                        refeicao.nome,
                        style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
                      ),
                    ),
                  ),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: <Widget>[
                      Text("Tamanho:", style: TextStyle(fontSize: 20)),
                      Expanded(
                        child: Padding(
                          padding: const EdgeInsets.fromLTRB(10, 0, 0, 0),
                          child: DropdownButton<String>(
                            isExpanded: true,
                            value: _codTamanhoSelecionado,
                            iconSize: 24,
                            elevation: 16,
                            onChanged: (String newValue) {
                              if (_refCardapio.deComposicao) {
                                _refeicaoDoPedidoDTOView =
                                    null; //setamos nulo para recarregar os componentes, pois os deComposição podem ter componentes diferentes
                              }
                              setState(() {
                                _codTamanhoSelecionado = newValue;
                              });
                            },
                            items: refeicao.tamanhos.map((e) {
                              return DropdownMenuItem<String>(
                                value: e.codigo,
                                child: Text(e.nome, style: TextStyle(fontSize: 20)),
                              );
                            }).toList(),
                          ),
                        ),
                      ),
                    ],
                  ),
                  Row(
                    children: <Widget>[
                      Text("Quantidade:", style: TextStyle(fontSize: 20)),
                      NumericUpDown(
                        decimalPlaces: 0,
                        initialValue: _quantidade,
                        maxValue: 99,
                        minValue: 1,
                        onChanged: (estadoNud) {
                          _quantidade = estadoNud.quantidade;
                        },
                      ),
                    ],
                  ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(0, 0, 10, 0),
                    child: TextField(
                      controller: _observacoesController,
                      textCapitalization: TextCapitalization.none,
                      decoration: InputDecoration(labelText: "Observacoes"),
                    ),
                  ),
                  Divider(),
                  if (_refCardapio.deComposicao)
                    Filtro(onFilterChanged: (valor) {
                      if (valor.isNullOrWhiteSpace()) {
                        _nudsAExibir = {for (var item in _refeicaoDoPedidoDTOView.componentesComposicao) item: 0};
                      } else {
                        _nudsAExibir.clear();

                        for (var item in _refeicaoDoPedidoDTOView.componentesComposicao) {
                          if (item.componenteRefeicao.nome.toUpperCase().contains(valor.toUpperCase())) {
                            _nudsAExibir.putIfAbsent(item, () => 0);
                          }
                        }
                      }
                      setState(() {});
                    }),
                  _obterTelaComponentesPeloTipoRefeicao(refeicao),
                  if (_refCardapio.deComposicao)
                    Row(
                      children: <Widget>[
                        Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Text("Componentes Selecionados:", style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                        ),
                      ],
                    ),
                  if (_refCardapio.deComposicao)
                    ListView.builder(
                        physics: NeverScrollableScrollPhysics(),
                        shrinkWrap: true,
                        itemCount: _nudsComponentesComValor.length,
                        itemBuilder: (BuildContext context, int i) {
                          List<ComponenteComposicaoRefeicaoCardapio> chaves = _nudsComponentesComValor.keys.toList();
                          int tot = 0;
                          for (int j = 0; j < _nudsComponentesComValor.length; j++) {
                            var comp = chaves[j];
                            if (comp.codUnidade != null && comp.codUnidade == UnidadeComponenteComposicao.codPartes) {
                              tot += _nudsComponentesComValor[comp];
                            }
                          }
                          var comp = chaves[i];
                          String texto = comp.componenteRefeicao.nome;
                          if (comp.codUnidade != null) {
                            if (comp.codUnidade == UnidadeComponenteComposicao.codPartes) {
                              texto += " (${_nudsComponentesComValor[comp]}/$tot)";
                            } else {
                              texto += " $_quantidade ${comp.codUnidade}";
                            }
                          }
                          return Padding(
                            padding: const EdgeInsets.all(3.0),
                            child: Text(texto, style: TextStyle(fontSize: 16)),
                          );
                        }),
                ],
              ),
            ),
          );
        }
        return _erro ? _criarTelaErro() : CircularProgressPadrao();
      },
    );
  }

  //Retorna parte da tela referente aos componentes
  Widget _obterTelaComponentesPeloTipoRefeicao(RefeicaoDoPedidoDTOView refeicao) {
    if (_refCardapio.deComposicao) {
      return ListView.builder(
          physics: NeverScrollableScrollPhysics(),
          shrinkWrap: true,
          itemCount: _nudsAExibir.length,
          itemBuilder: (BuildContext context, int i) {
            return Row(
              children: <Widget>[
                _criarNudComposicao(_nudsAExibir.values.toList()[i], _nudsAExibir.keys.toList()[i]),
              ],
            );
          });
    } else {
      return ListView.builder(
        physics: NeverScrollableScrollPhysics(),
        shrinkWrap: true,
        itemCount: refeicao.componentes.length,
        itemBuilder: (BuildContext context, int i) {
          return Column(
            children: <Widget>[
              Container(
                decoration: BoxDecoration(border: Border(bottom: BorderSide(color: Colors.grey[300]))),
                child: InkWell(
                  onTap: () {
                    setState(() {
                      refeicao.componentes[i].checked = !refeicao.componentes[i].checked;
                    });
                  },
                  child: Row(
                    children: <Widget>[
                      Checkbox(
                        value: refeicao.componentes[i].checked,
                        onChanged: (bool newValue) {
                          setState(() {
                            refeicao.componentes[i].checked = newValue;
                          });
                        },
                      ),
                      Expanded(child: Text(refeicao.componentes[i].componente.nome)),
                    ],
                  ),
                ),
              ),
            ],
          );
        },
      );
    }
  }

  Widget _criarBotoesRodape(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: <Widget>[
        BotaoRodape(
          texto: "Voltar",
          onPressed: _exibirCarregando ? null : _voltar,
        ),
        BotaoRodape(texto: "OK", onPressed: _exibirCarregando ? null : _adicionarRefeicaoDoPedido),
      ],
    );
  }

  Widget _criarTelaErro() {
    return util.obterWidgetErroParaTela(context, _msgErro, refresh: () {
      _refeicaoDoPedidoDTOView = null;
      setState(() {
        _exibirCarregando = true;
      });
      _carregarRefeicaoDoCardapio();
    });
  }

  void _voltar() {
    if (_exibirCarregando) return;
    Navigator.of(context).pushReplacementNamed(AppRouts.CategoriasRoute);
  }

  Future<List<TamanhoRefeicao>> _obterTamanhos(Refeicao refeicao) async {
    List<TamanhoRefeicao> tamanhos = [];
    try {
      tamanhos = await _provider.obterEntidades<TamanhoRefeicao>("TamanhosRefeicao",
          queryStringCodigo: "codRefeicao=${refeicao.codigo}&codTipoRefeicao=${refeicao.codTipo}");
    } catch (e, s) {
      throw ("Erro ao tentar obter os tamanhos de refeição. " + e.toString() + s.toString());
    }
    tamanhos.sort((a, b) => a.posicao.compareTo(b.posicao));
    return tamanhos;
  }

  Future<List<ComponenteRefeicaoDoPedidoDTOView>> _obterComponentes(RefeicaoDoCardapio refCardapio) async {
    List<ComponenteRefeicao> componentes = [];
    List<ComponenteRefeicaoDoPedidoDTOView> listaDTO = [];
    try {
      componentes =
          await _provider.obterEntidades<ComponenteRefeicao>("ComponentesRefeicao", queryStringCodigo: "codRefeicao=${refCardapio.refeicao.codigo}");

      listaDTO = componentes.map((e) {
        return new ComponenteRefeicaoDoPedidoDTOView(checked: true, componente: e);
      }).toList();
    } catch (e, s) {
      throw ("Erro ao tentar obter os componentes da refeição. " + e.toString() + s.toString());
    }
    return listaDTO;
  }

  Future<List<ComponenteComposicaoRefeicaoCardapio>> _obterComponentesComposicao(RefeicaoDoCardapio refCardapio) async {
    List<ComponenteComposicaoRefeicaoCardapio> componentesComposicao = [];

    try {
      componentesComposicao = await _provider.obterEntidades<ComponenteComposicaoRefeicaoCardapio>("ComponentesComposicaoRefeicaoCardapio",
          queryStringCodigo: "codRefeicao=${refCardapio.refeicao.codigo}&codTamanho=$_codTamanhoSelecionado");

      return componentesComposicao;
    } catch (e, s) {
      throw ("Erro ao tentar obter os componentes de composição da refeição. " + e.toString() + s.toString());
    }
  }
}
