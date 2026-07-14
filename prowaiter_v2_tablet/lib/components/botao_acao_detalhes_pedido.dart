import 'package:flutter/material.dart';
import 'package:prowaiter_v2/util/constantes.dart';

enum AcaoBotaoDetalhes { editar, remover }

class BotaoAcaoDetalhesPedido extends StatelessWidget {
  final AcaoBotaoDetalhes acao;
  final Function onPressed;

  BotaoAcaoDetalhesPedido({this.acao, this.onPressed});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 80,
      height: 40,
      child: Padding(
        padding: const EdgeInsets.all(2),
        child: Container(
          width: 10,
          child: RaisedButton(
            padding: EdgeInsets.all(0),
            onPressed: onPressed,
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(5.0)),
            child: Ink(
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(5.0),
                border: Border.all(color: _obterBordaCorBotao()),
                gradient: LinearGradient(
                  colors: _obterCoresBotaoGradiente(),
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                ),
              ),
              child: Container(
                alignment: Alignment.center,
                child: Text(
                  acao == AcaoBotaoDetalhes.editar ? "Editar" : "Remover",
                  textAlign: TextAlign.center,
                  style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold, color: Colors.white),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Color _obterBordaCorBotao() {
    if (onPressed == null) {
      return Colors.grey;
    }

    return acao == AcaoBotaoDetalhes.editar ? Constantes.corBordaBotaoVerde : Constantes.corBordaBotaoVermelho;
  }

  List<Color> _obterCoresBotaoGradiente() {
    if (onPressed == null) {
      return [Colors.grey, Colors.grey[300]];
    }
    return acao == AcaoBotaoDetalhes.editar ? Constantes.coresBotaoVerde : Constantes.coresBotaoVermelho;
  }
}
