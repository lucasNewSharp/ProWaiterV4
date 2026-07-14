import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:provider/provider.dart';
import 'package:prowaiter_v2/components/botao_acao_detalhes_pedido.dart';
import 'package:prowaiter_v2/components/circular_progress_padrao.dart';
import 'package:prowaiter_v2/models/entidades_dto.dart';
import 'package:prowaiter_v2/models/item_pedido.dart';
import 'package:prowaiter_v2/util/constantes.dart';
import 'package:prowaiter_v2/util/servicos_app_provider.dart';
import 'package:prowaiter_v2/util/util.dart' as util;

enum TipoCard { bebidaPedido, refeicaoPedido, refeicaoPedidoComposicao }

class CardViewItemPedido extends StatefulWidget {
  final ItemPedido entidade;
  final Function(ItemPedido) aoEditar;
  final Function(ItemPedido) aoRemover;

  CardViewItemPedido({this.entidade, this.aoEditar, this.aoRemover}) : super(key: GlobalKey());

  @override
  CardViewItemPedidoState createState() => CardViewItemPedidoState();
}

class CardViewItemPedidoState extends State<CardViewItemPedido> {
  Color corTitulo;
  BebidaDoPedido bebidaDoPedido;
  RefeicaoDoPedido refeicaoDoPedido;
  TipoCard tipoCard;
  ServicosAPPProvider provider;

  List<String> textoComponentesComposicao;

  @override
  void initState() {
    super.initState();
    provider = Provider.of<ServicosAPPProvider>(context, listen: false);
    corTitulo = Colors.black;
    if (widget.entidade.personalizado) {
      corTitulo = Colors.blue[800];
    }

    if (widget.entidade is BebidaDoPedido) {
      bebidaDoPedido = widget.entidade as BebidaDoPedido;
      tipoCard = TipoCard.bebidaPedido;
    } else {
      refeicaoDoPedido = widget.entidade as RefeicaoDoPedido;
      if (refeicaoDoPedido.refeicaoDoCardapio.deComposicao) {
        tipoCard = TipoCard.refeicaoPedidoComposicao;
      } else {
        tipoCard = TipoCard.refeicaoPedido;
      }
    }
  }

  Future<List<String>> _obterTextosComponentesComposicao() async {
    if (textoComponentesComposicao == null) {
      textoComponentesComposicao = [];
      try {
        List<ComponenteComposicaoRefeicaoCardapio> componentesCardapio = await provider.obterEntidades<ComponenteComposicaoRefeicaoCardapio>(
            "ComponentesComposicaoRefeicaoCardapio",
            queryStringCodigo: "codRefeicao=${refeicaoDoPedido.codRefeicao}&codTamanho=${refeicaoDoPedido.codTamanho}");

        int qtd = 0;
        //Obtemos a quantidade dos componentes que são calculados em partes
        for (var compPedido in refeicaoDoPedido.componentesRefeicaoPedido) {
          var compCardapio = componentesCardapio.singleWhere((element) => element.codComponente == compPedido.codComponente);
          if (compCardapio.codUnidade != null && compCardapio.codUnidade == UnidadeComponenteComposicao.codPartes) {
            qtd += compPedido.quantidade;
          }
        }

        for (var compPedido in refeicaoDoPedido.componentesRefeicaoPedido) {
          var compCardapio = componentesCardapio.singleWhere((element) => element.codComponente == compPedido.codComponente);
          String texto = compCardapio.componenteRefeicao.nome;
          if (compCardapio.calculoProporcional) {
            if (compCardapio.codUnidade == UnidadeComponenteComposicao.codPartes) {
              texto += " (${compPedido.quantidade}/$qtd)";
            }
          } else if (!compCardapio.codUnidade.isNullOrWhiteSpace()) {
            texto += " (${compPedido.quantidade} ${compCardapio.codUnidade})";
          }

          textoComponentesComposicao.add(texto);
        }

        double valor = util.calcularValorRefeicaoDoPedidoParaExibicao(refeicaoDoPedido, componentesCardapio);
        this.widget.entidade.valor = valor;

        textoComponentesComposicao.add("Valor: ${valor.toStringAsFixed(2).replaceFirst(".", ",")}");
      } catch (e, s) {
        util.exibirMensagem(context, e.toString() + s.toString());
      }
    }
    return textoComponentesComposicao;
  }

  void _remover() {
    widget.aoRemover(widget.entidade);
  }

  void _editar() {
    widget.aoEditar(widget.entidade);
  }

  @override
  Widget build(BuildContext context) {
    Widget corpo;
    switch (tipoCard) {
      case TipoCard.bebidaPedido:
        corpo = _obterCardBebidaPedido();
        break;
      case TipoCard.refeicaoPedido:
        corpo = _obterCardRefeicaoPedido();
        break;
      case TipoCard.refeicaoPedidoComposicao:
        corpo = _obterCardRefeicaoCompostaPedido();
        break;
    }

    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(6.0),
      ),
      elevation: 10,
      child: corpo,
    );
  }

  Widget _obterCardBebidaPedido() {
    return Container(
      //height: 60,
      padding: const EdgeInsets.all(8),
      child: Column(
        children: <Widget>[
          _criarRowBotoes(bebidaDoPedido.bebida.nome, true),
        ],
      ),
    );
  }

  Widget _obterCardRefeicaoPedido() {
    return Container(
      //height: 100,
      padding: const EdgeInsets.all(8),
      child: Column(
        children: <Widget>[
          Row(
            children: <Widget>[
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Text(
                    refeicaoDoPedido.refeicaoDoCardapio.refeicao.nome,
                    style: TextStyle(
                      color: corTitulo,
                    ),
                  ),
                ),
              ),
            ],
          ),
          _criarRowBotoes(refeicaoDoPedido.refeicaoDoCardapio.tamanhoRefeicao.nome, false),
        ],
      ),
    );
  }

  Widget _obterCardRefeicaoCompostaPedido() {
    return Container(
      padding: const EdgeInsets.all(8),
      child: Column(
        children: <Widget>[
          Row(
            children: <Widget>[
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Text(
                    refeicaoDoPedido.refeicaoDoCardapio.refeicao.nome,
                    style: TextStyle(
                      color: corTitulo,
                    ),
                  ),
                ),
              ),
            ],
          ),
          _criarRowBotoes(refeicaoDoPedido.refeicaoDoCardapio.tamanhoRefeicao.nome, false),
          Row(
            children: <Widget>[
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.all(8),
                  child: FutureBuilder(
                    future: _obterTextosComponentesComposicao(),
                    builder: (BuildContext ctx, AsyncSnapshot<List<String>> snapshot) {
                      if (snapshot.hasData) {
                        return ListView.builder(
                            itemCount: snapshot.data.length,
                            physics: NeverScrollableScrollPhysics(),
                            shrinkWrap: true,
                            itemBuilder: (BuildContext context, int i) {
                              return Padding(
                                padding: const EdgeInsets.all(1),
                                child: Text(
                                  snapshot.data[i],
                                  style: TextStyle(fontSize: 14, color: Constantes.cinzaTexto),
                                ),
                              );
                            });
                      }
                      return CircularProgressPadrao();
                    },
                  ),
                ),
              )
            ],
          )
        ],
      ),
    );
  }

  Widget _criarRowBotoes(String texto, bool corTextoPadrao) {
    return Row(children: <Widget>[
      Expanded(
        child: Padding(
          padding: const EdgeInsets.all(8.0),
          child: Text(
            texto,
            style: TextStyle(
              color: corTextoPadrao ? Colors.black : Constantes.cinzaTexto,
            ),
          ),
        ),
      ),
      BotaoAcaoDetalhesPedido(acao: AcaoBotaoDetalhes.editar, onPressed: widget.entidade.enviadoACozinha ? null : _editar),
      BotaoAcaoDetalhesPedido(acao: AcaoBotaoDetalhes.remover, onPressed: widget.entidade.enviadoACozinha ? null : _remover),
    ]);
  }
}
