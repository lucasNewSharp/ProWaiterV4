import 'package:flutter/material.dart';
import 'package:package_info/package_info.dart';
import 'package:prowaiter_v2/models/entidades_dto.dart';

void exibirSobre(BuildContext context) async {
  PackageInfo.fromPlatform().then((packageInfo) {
    String versionName = "Versão: ${packageInfo.version}";
    String versionCode = "Build: ${packageInfo.buildNumber}";
    String texto =
        "© 2014 - ${DateTime.now().year}, Newsharp Sistemas de Informação Ltda - ME.\r\nTodos os direitos reservados.\r\n$versionName\r\n$versionCode\n";

    var _alert = new AlertDialog(
      title: Row(
        children: <Widget>[
          Image.asset(
            "assets/ProWaiter.png",
            width: 35,
          ),
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Text(
              "ProWaiter",
              textAlign: TextAlign.center,
            ),
          ),
        ],
      ),
      content: SingleChildScrollView(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            Text(texto, textAlign: TextAlign.center),
          ],
        ),
      ),
    );

    showDialog(context: context, builder: (context) => _alert);
  });
}

void exibirErro(BuildContext context, dynamic exception, StackTrace stackTrace, {String msg}) {
  String mensagem = "${exception.toString()}\n${stackTrace.toString()}";
  if (!msg.isNullOrWhiteSpace()) mensagem = "$msg\n$mensagem";
  exibirMensagem(context, mensagem);
}

void exibirMensagem(BuildContext context, String mensagem, {Function onPressed}) {
  String conteudo = mensagem;
  var _alert = new AlertDialog(
    title: Text("Aviso!"),
    content: SingleChildScrollView(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: <Widget>[
          Text(conteudo, textAlign: TextAlign.start),
        ],
      ),
    ),
    actions: <Widget>[
      FlatButton(
        child: Text("Fechar"),
        onPressed: onPressed == null ? () => Navigator.pop(context) : onPressed,
      ),
    ],
  );

  showDialog(context: context, builder: (context) => _alert);
}

void dialogoPerguntar({@required BuildContext context, @required String mensagem, @required Function aoClicarSim, Function aoClicarNao}) {
  var _alert = new AlertDialog(
    title: Text("Confirmar"),
    content: SingleChildScrollView(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: <Widget>[
          Text(
            mensagem,
            textAlign: TextAlign.start,
          ),
        ],
      ),
    ),
    actions: <Widget>[
      FlatButton(
        child: Text("Sim"),
        onPressed: () {
          aoClicarSim();
        },
      ),
      FlatButton(
        child: Text("Não"),
        onPressed: aoClicarNao != null ? aoClicarNao : () => Navigator.pop(context),
      )
    ],
  );

  showDialog(context: context, builder: (context) => _alert);
}

Widget obterWidgetErroParaTela(BuildContext context, String msg, {Function refresh}) {
  return SingleChildScrollView(
    child: Column(
      children: <Widget>[
        Padding(
          padding: const EdgeInsets.all(8.0),
          child: RaisedButton.icon(
            icon: Icon(
              Icons.refresh,
              color: Colors.white,
            ),
            label: Text("Recarregar tela", style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
            color: Theme.of(context).primaryColor,
            onPressed: refresh,
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(5.0)),
          ),
        ),
        Text(msg),
      ],
    ),
  );
}

extension StringExtension on String {
  bool isNullOrWhiteSpace() {
    return this == null || this.isEmpty;
  }
}

//Calculos de valores com desconto para exibição
double calcularValorRefeicaoDoPedidoParaExibicao(RefeicaoDoPedido ref, [List<ComponenteComposicaoRefeicaoCardapio> componentesComposicao]) {
  double vl = 0;
  if (ref.enviadoACozinha) {
    vl = ref.valor;
  } else {
    //De composicao
    if (ref.refeicaoDoCardapio.deComposicao) {
      int qtd = 0;
      //Obtemos a quantidade dos componentes que são calculados em partes
      for (var compPedido in ref.componentesRefeicaoPedido) {
        var compCardapio = componentesComposicao.singleWhere((element) => element.codComponente == compPedido.codComponente);
        if (compCardapio.codUnidade != null && compCardapio.codUnidade == UnidadeComponenteComposicao.codPartes) {
          qtd += compPedido.quantidade;
        }
      }

      for (var compPedido in ref.componentesRefeicaoPedido) {
        var compCardapio = componentesComposicao.singleWhere((element) => element.codComponente == compPedido.codComponente);
        if (compCardapio.calculoProporcional) {
          vl += (compCardapio.valor * compPedido.quantidade) / qtd;
        } else {
          vl += compCardapio.valor * compPedido.quantidade;
        }
      }
      if (ref.refeicaoDoCardapio.percDesconto > 0) {
        vl = _calcularDescontoSimples(vl, ref.refeicaoDoCardapio.percDesconto);
      }
    } else {
      //refeição simples
      if (ref.refeicaoDoCardapio.percDesconto > 0) {
        vl = _calcularDescontoSimples(ref.refeicaoDoCardapio.valor, ref.refeicaoDoCardapio.percDesconto);
      } else {
        vl = ref.refeicaoDoCardapio.valor;
      }
    }
  }
  return vl;
}

double calcularValorBebidaParaExibicao(BebidaDoPedido beb) {
  double vl = 0;
  if (beb.enviadoACozinha) {
    vl += beb.valor;
  } else {
    if (beb.bebida.percDesconto > 0)
      vl += _calcularDescontoSimples(beb.bebida.valor, beb.bebida.percDesconto);
    else {
      vl += beb.bebida.valor;
    }
  }
  return vl;
}

double _calcularDescontoSimples(double valor, double percDesconto) {
  return valor - ((valor * percDesconto) / 100);
}
