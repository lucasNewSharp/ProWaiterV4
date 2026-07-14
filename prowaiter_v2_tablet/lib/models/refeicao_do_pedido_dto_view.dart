import 'package:prowaiter_v2/models/entidades_dto.dart';
import 'package:flutter/foundation.dart';

class ComponenteRefeicaoDoPedidoDTOView {
  final ComponenteRefeicao componente;
  bool checked;
  ComponenteRefeicaoDoPedidoDTOView({this.componente, this.checked});
}

class RefeicaoDoPedidoDTOView {
  String nome;
  int codRefeicao;
  List<TamanhoRefeicao> tamanhos;
  List<ComponenteRefeicaoDoPedidoDTOView> componentes;
  List<ComponenteComposicaoRefeicaoCardapio> componentesComposicao;

  RefeicaoDoPedidoDTOView({
    @required this.nome,
    @required this.tamanhos,    
    @required this.codRefeicao,
    this.componentes,
    this.componentesComposicao
  });
}
