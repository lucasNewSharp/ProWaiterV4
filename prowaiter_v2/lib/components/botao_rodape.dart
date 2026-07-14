import 'package:flutter/material.dart';
import 'package:prowaiter_v2/util/constantes.dart';

class BotaoRodape extends StatelessWidget {
  final Function onPressed;
  final String texto;

  BotaoRodape({this.texto, this.onPressed});

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Container(
        height: 50,
        child: Padding(
          padding: const EdgeInsets.all(3),
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
                child: Padding(
                  padding: const EdgeInsets.all(0),
                  child: Text(
                    texto,
                    textAlign: TextAlign.center,
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.white),
                  ),
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
    return Constantes.corBordaBotaoRodape;
  }

  List<Color> _obterCoresBotaoGradiente() {
    if (onPressed == null) {
      return [Colors.grey, Colors.grey[300]];
    }
    return Constantes.coresBotaoRodape;
  }
}
