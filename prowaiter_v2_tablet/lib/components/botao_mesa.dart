import 'package:flutter/material.dart';
import 'package:prowaiter_v2/util/constantes.dart';
import '../models/entidades_dto.dart';

class BotaoMesa extends StatelessWidget {
  final Mesa mesa;
  final Function(Mesa mesa) aoClicarMesa;

  BotaoMesa(this.mesa, this.aoClicarMesa);

  @override
  Widget build(BuildContext context) {    
    return criarBotaoMesa(context, mesa.descricao, mesa.codUtilimoPedido != null);
  }

  Widget criarBotaoMesa(BuildContext context, String descricao, bool ocupada) {
    
    return Container(       
      child: RaisedButton(
        elevation: 5,
        onPressed: () {
          aoClicarMesa(mesa);
        },
        shape:
            RoundedRectangleBorder(borderRadius: BorderRadius.circular(10.0)),
        padding: EdgeInsets.all(0.0),
        child: Ink(          
          decoration: BoxDecoration(            
            borderRadius: BorderRadius.circular(10.0),
            border: Border.all(
                color: ocupada ? Constantes.corBordaBotaoVermelho : Constantes.corBordaBotaoVerde),
            gradient: LinearGradient(
              colors: ocupada ? Constantes.coresBotaoVermelho : Constantes.coresBotaoVerde, // [Color(0xFFE57373), Color(0xFFEF9A9A)],
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
            ),
          ),
          child: Container(
            //constraints: BoxConstraints(maxWidth: 300.0, minHeight: 50.0),
            alignment: Alignment.center,
            child: Padding(
              padding: const EdgeInsets.all(6.0),
              child: Text(
                descricao,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 24,
                  color: Colors.white
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
