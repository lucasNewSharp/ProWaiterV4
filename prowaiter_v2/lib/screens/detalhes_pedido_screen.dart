import 'package:back_button_interceptor/back_button_interceptor.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:prowaiter_v2/components/botao_rodape.dart';
import 'package:prowaiter_v2/components/card_view_item_pedido.dart';
import 'package:prowaiter_v2/components/circular_progress_padrao.dart';
import 'package:prowaiter_v2/components/scaffold_padrao.dart';
import 'package:prowaiter_v2/models/entidades_dto.dart';
import 'package:prowaiter_v2/models/item_pedido.dart';
import 'package:prowaiter_v2/util/app_routes.dart';
import 'package:prowaiter_v2/util/servicos_app_provider.dart';
import 'package:toast/toast.dart';
import 'package:prowaiter_v2/util/util.dart' as util;

class DetalhesPedidoScreen extends StatefulWidget {
  @override
  _DetalhesPedidoScreenState createState() => _DetalhesPedidoScreenState();
}

class _DetalhesPedidoScreenState extends State<DetalhesPedidoScreen> {
  ServicosAPPProvider _provider;
  PedidoInterno pedido;
  List<CardViewItemPedido> _itensPedido;
  String _codLocalInternoSelecionado;
  bool _erro = false;
  String _msgErro = "";
  bool _exibirCarregando = true;
  double _valorTotal = 0;

  @override
  void initState() {
    super.initState();
    BackButtonInterceptor.add(_backButton);
    _provider = Provider.of<ServicosAPPProvider>(context, listen: false);
    pedido = _provider.pedidoInternoAtual;
    _carregarItensPedido();
  }

  @override
  void dispose() {
    super.dispose();
    BackButtonInterceptor.remove(_backButton);
  }

  bool _backButton(bool stopDefaultButtonEvent, RouteInfo routeInfo) {
    _voltarTelaCategorias();
    return true;
  }

  void _carregarItensPedido() async {
    double _vlTot = 0;
    _erro = false;
    _msgErro = "";
    try {
      _itensPedido = pedido.refeicoesDoPedido
          .map((e) => CardViewItemPedido(
                entidade: e,
                aoEditar: _aoEditarRefeicao,
                aoRemover: _aoRemoverRefeicao,
              ))
          .toList();

      _itensPedido.addAll(pedido.bebidasDoPedido
          .map((e) => CardViewItemPedido(
                entidade: e,
                aoEditar: _aoEditarBebida,
                aoRemover: _aoRemoverBebida,
              ))
          .toList());

      _vlTot = await _calcularValorTotal();

      if (_provider.configWeb.utilizaComanda) {
        _locaisInternos = await _carregarLocaisInternos();
      }
      setState(() {
        _exibirCarregando = false;
        _valorTotal = _vlTot;
      });
    } catch (e, s) {
      setState(() {
        _erro = true;
        _msgErro = e.toString() + s.toString();
      });
    }
  }

  Future<double> _calcularValorTotal() async {
    double vl = 0;
    for (var card in _itensPedido) {
      if (card.entidade is BebidaDoPedido) {
        vl += util.calcularValorBebidaParaExibicao(card.entidade as BebidaDoPedido);
      } else if (card.entidade is RefeicaoDoPedido) {
        var refeicaoDoPedido = card.entidade as RefeicaoDoPedido;
        if (refeicaoDoPedido.refeicaoDoCardapio.deComposicao) {
          List<ComponenteComposicaoRefeicaoCardapio> componentesComposicao = await _provider.obterEntidades<ComponenteComposicaoRefeicaoCardapio>(
              "ComponentesComposicaoRefeicaoCardapio",
              queryStringCodigo: "codRefeicao=${refeicaoDoPedido.codRefeicao}&codTamanho=${refeicaoDoPedido.codTamanho}");

          vl += util.calcularValorRefeicaoDoPedidoParaExibicao(card.entidade as RefeicaoDoPedido, componentesComposicao);
        } else {
          vl += util.calcularValorRefeicaoDoPedidoParaExibicao(card.entidade as RefeicaoDoPedido);
        }
      }
    }

    //verificaçao do modelo se tem desconto ou acrescimo (itens ainda não enviados)
    List<GlobalKey> listaChaves = new List<GlobalKey>();
    double desconto = 0;
    double acrescimo = 0;
    for (var card in _itensPedido) {
      if (card.entidade.chaveParaVincluarModelo != null && !card.entidade.enviadoACozinha) {
        if (!listaChaves.contains(card.entidade.chaveParaVincluarModelo)) {
          desconto += card.entidade.modeloPedido.desconto;
          acrescimo += card.entidade.modeloPedido.acrescimo;
          listaChaves.add(card.entidade.chaveParaVincluarModelo);
        }
      }
    }
    vl += acrescimo;
    vl -= desconto;

    //adicionamos os descontos e acrescimos do PedidoInterno
    if (pedido.acrescimos != null) {
      vl += pedido.acrescimos;
    }
    if (pedido.descontos != null) {
      vl -= pedido.descontos;
    }
    return vl;
  }

  void _aoEditarBebida(ItemPedido bebidaDoPedido) {
    Navigator.of(context).pushReplacementNamed(AppRouts.BebidasRoute, arguments: bebidaDoPedido);
  }

  void _aoRemoverBebida(ItemPedido itemPedido) {
    if (itemPedido.modeloPedido != null) {
      _removerItensCombo(itemPedido);
    } else {
      pedido.bebidasDoPedido.remove(itemPedido);
      setState(() {
        _carregarItensPedido();
      });
    }
  }

  void _aoEditarRefeicao(ItemPedido refeicaoDoPedido) {
    Navigator.of(context).pushReplacementNamed(AppRouts.ComponentesRefeicaoRoute, arguments: refeicaoDoPedido);
  }

  void _aoRemoverRefeicao(ItemPedido itemPedido) {
    if (itemPedido.modeloPedido != null) {
      _removerItensCombo(itemPedido);
    } else {
      pedido.refeicoesDoPedido.remove(itemPedido);
      setState(() {
        _carregarItensPedido();
      });
    }
  }

  _removerItensCombo(ItemPedido itemPedido) {
    if (itemPedido.modeloPedido != null) {
      util.dialogoPerguntar(
          context: context,
          mensagem: "Você está removendo um item do modelo, os demais itens referentes a este modelo serão removidos, deseja continuar?",
          aoClicarSim: () {
            Navigator.pop(context);
            pedido.refeicoesDoPedido.remove(itemPedido);

            pedido.bebidasDoPedido.removeWhere((e) => e.chaveParaVincluarModelo == itemPedido.chaveParaVincluarModelo);
            pedido.refeicoesDoPedido.removeWhere((e) => e.chaveParaVincluarModelo == itemPedido.chaveParaVincluarModelo);
            //já removemos os acrescimos ou descontos do modelo no valor geral
            _provider.acrescimoAtual -= itemPedido.modeloPedido.acrescimo;
            _provider.descontoAtual -= itemPedido.modeloPedido.desconto;
            setState(() {
              _carregarItensPedido();
            });
          });
    }
  }

  void _enviarItens() async {
    if (!pedido.possuiItensNaoEnviados) {
      util.exibirMensagem(context, "Não existem novos itens adicionados ao pedido");
      return;
    } else {
      if (_provider.configWeb.utilizaComanda) {
        if (_codLocalInternoSelecionado == null) {
          util.exibirMensagem(context, "Escolha o local de entrega!");
          return;
        }
      }
    }

    setState(() {
      _exibirCarregando = true;
    });

    //envio
    List<BebidaDoPedido> bebidasNaoEnviadas = pedido.bebidasDoPedido.where((element) => !element.enviadoACozinha).toList();
    List<RefeicaoDoPedido> refeicoesNaoEnviadas = pedido.refeicoesDoPedido.where((element) => !element.enviadoACozinha).toList();

    ItensNaoEnviados itens = new ItensNaoEnviados(
        codPedido: pedido.codigo,
        codMesa: _provider.codMesaAtual,
        refeicoesDoPedido: refeicoesNaoEnviadas,
        bebidasDoPedido: bebidasNaoEnviadas,
        mensagem: "",
        acrescimos: _provider.acrescimoAtual,
        descontos: _provider.descontoAtual,
        codLocalInternoEntrega: _codLocalInternoSelecionado.isNullOrWhiteSpace() ? null : int.parse(_codLocalInternoSelecionado));

    try {
      ItensNaoEnviados retorno = await _provider.inserir<ItensNaoEnviados>(controller: "EnviarPedidoACozinha", objeto: itens);

      if (retorno.bebidasDoPedido.length == 0 && retorno.refeicoesDoPedido.length == 0) {
        Toast.show(retorno.mensagem, context, duration: Toast.LENGTH_LONG, gravity: Toast.BOTTOM);
        Navigator.of(context).pushReplacementNamed(AppRouts.MesasRoute);
        return;
      } else {
        util.exibirMensagem(context, retorno.mensagem);
        _atualizarCache(retorno);
      }
    } catch (e, s) {
      util.exibirErro(context, e, s);
    }
    setState(() {
      _exibirCarregando = false;
    });
  }

  void _atualizarCache(ItensNaoEnviados retorno) async {
    if (retorno == null) return;

    try {
      PedidoInterno pedidoAtualizado = await _provider.recuperar<PedidoInterno>("PedidosInternos", pedido.codigo.toString());
      _provider.acrescimoAtual = 0; //zeramos o desconto/acrescimo, pois se existiu foi adiconado ao pedido
      _provider.descontoAtual = 0;
      pedido.bebidasDoPedido.clear();
      pedido.bebidasDoPedido.addAll(pedidoAtualizado.bebidasDoPedido); //Adicionamos o que já foi enviado
      pedido.bebidasDoPedido.addAll(retorno.bebidasDoPedido); //Adicionamos o que não foi possível ser enviado

      pedido.refeicoesDoPedido.clear();
      pedido.refeicoesDoPedido.addAll(pedidoAtualizado.refeicoesDoPedido);
      pedido.refeicoesDoPedido.addAll(retorno.refeicoesDoPedido);
      _carregarItensPedido();
      setState(() {});
    } catch (e, s) {
      util.exibirMensagem(context, e.toString() + s.toString());
    }
  }

  void _aoClicarCancelar() {
    if (pedido.possuiItensNaoEnviados) {
      util.dialogoPerguntar(
        context: context,
        mensagem: "Você tem certeza que deseja cancelar os itens ainda não enviados?",
        aoClicarSim: _cancelarItensNaoEnviados,
      );
    } else {
      util.exibirMensagem(context, "O pedido não possui itens não enviados para ser cancelado!");
    }
  }

  void _cancelarItensNaoEnviados() {
    Navigator.pop(context);
    pedido.bebidasDoPedido.removeWhere((element) => !element.enviadoACozinha);
    pedido.refeicoesDoPedido.removeWhere((element) => !element.enviadoACozinha);
    Navigator.of(context).pushReplacementNamed(AppRouts.MesasRoute);
  }

  @override
  Widget build(BuildContext context) {
    return ScaffoldPadrao(
      onBackPress: _voltarTelaCategorias,
      exibirBackButton: true,
      titulo: "Detalhes do pedido",
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

    String vlTotalStr = "TOTAL: R\$ ${_valorTotal.toStringAsFixed(2).replaceFirst(".", ",")}";
    return Padding(
      padding: const EdgeInsets.all(8.0),
      child: SingleChildScrollView(
        child: Column(
          children: <Widget>[
            ListView.builder(
                physics: NeverScrollableScrollPhysics(),
                shrinkWrap: true,
                itemCount: _itensPedido.length,
                itemBuilder: (BuildContext context, int i) {
                  return _itensPedido[i];
                }),
            Card(
              elevation: 10,
              child: Container(
                padding: EdgeInsets.all(20.0),
                child: Column(
                  children: <Widget>[
                    Row(
                      children: [
                        Expanded(
                          child: Text(
                            vlTotalStr,
                            style: TextStyle(fontSize: 18, fontWeight: FontWeight.w700),
                          ),
                        )
                      ],
                    ),
                  ],
                ),
              ),
            ),
            if (_provider.configWeb.utilizaComanda) _criarWidgetLocalEntrega()
          ],
        ),
      ),
    );
  }

  Widget _criarWidgetLocalEntrega() {
    return Column(
      children: <Widget>[
        Padding(
          padding: const EdgeInsets.all(8.0),
          child: Container(
            alignment: Alignment.center,
            padding: const EdgeInsets.all(3),
            width: double.infinity,
            color: Theme.of(context).primaryColorDark,
            child: Text("Local da entrega", style: TextStyle(fontSize: 16, fontWeight: FontWeight.normal, color: Colors.white)),
          ),
        ),
        FutureBuilder(
          future: _carregarLocaisInternos(),
          builder: (BuildContext ctx, AsyncSnapshot<List<LocalInterno>> snapshot) {
            if (snapshot.hasData) {
              return Padding(
                padding: const EdgeInsets.all(6.0),
                child: DropdownButton<String>(
                  isExpanded: true,
                  value: _codLocalInternoSelecionado,
                  iconSize: 24,
                  elevation: 16,
                  onChanged: (String newValue) {
                    setState(() {
                      _codLocalInternoSelecionado = newValue;
                    });
                  },
                  items: snapshot.data.map((e) {
                    return DropdownMenuItem<String>(
                      value: e.codigo.toString(),
                      child: Text(e.nome, style: TextStyle(fontSize: 20)),
                    );
                  }).toList(),
                ),
              );
            }
            return CircularProgressPadrao();
          },
        ),
      ],
    );
  }

  Widget _criarTelaErro() {
    return util.obterWidgetErroParaTela(context, _msgErro, refresh: () {
      _itensPedido = null;
      setState(() {
        _exibirCarregando = true;
      });
      _carregarItensPedido();
    });
  }

  Widget _criarBotoesRodape() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: <Widget>[
        BotaoRodape(texto: "Voltar", onPressed: _exibirCarregando ? null : _voltarTelaCategorias),
        BotaoRodape(texto: "Enviar", onPressed: _exibirCarregando ? null : _enviarItens),
        BotaoRodape(texto: "Cancelar", onPressed: _exibirCarregando ? null : _aoClicarCancelar),
      ],
    );
  }

  void _voltarTelaCategorias() {
    if (_exibirCarregando) return;
    Navigator.of(context).pushReplacementNamed(AppRouts.CategoriasRoute);
  }

  List<LocalInterno> _locaisInternos;
  Future<List<LocalInterno>> _carregarLocaisInternos() async {
    if (_locaisInternos == null) {
      try {
        _locaisInternos = await _provider.obterEntidades<LocalInterno>("LocaisInternos");
      } catch (e, s) {
        throw ("Erro ao tentar obter os locais internos. " + e.toString() + s.toString());
      }
    }
    return _locaisInternos;
  }
}
