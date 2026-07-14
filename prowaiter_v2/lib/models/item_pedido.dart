import 'entidades_dto.dart';
import 'package:flutter/material.dart';

abstract class ItemPedido {
  bool get personalizado;
  bool get enviadoACozinha;
  double valor;

  //Chave utilizada para poder remover todos os itens do modelo, caso o usuário remova algum item antes de enviar para cozinha
  GlobalKey chaveParaVincluarModelo;
  ModeloPedido modeloPedido;
}
