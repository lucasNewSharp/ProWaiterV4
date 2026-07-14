import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:json_annotation/json_annotation.dart';
import 'package:prowaiter_v2/models/i_clonable.dart';
import 'package:prowaiter_v2/models/item_pedido.dart';
import 'package:prowaiter_v2/util/util.dart';
part 'entidades_dto.g.dart';

//Rodar:
//flutter packages pub run build_runner build
//para gerar as conversões

@JsonSerializable()
class ConfiguracoesAPP {
  String ipServidor;
  String login;
  String senha;
  DateTime dataHoraLogin;
  String tema;

  ConfiguracoesAPP();

  factory ConfiguracoesAPP.fromJson(Map<String, dynamic> json) => _$ConfiguracoesAPPFromJson(json);
  Map<String, dynamic> toJson() => _$ConfiguracoesAPPToJson(this);
}

@JsonSerializable()
class ConfiguracoesProWaiterWeb {
  @JsonKey(name: "UtilizaComanda")
  bool utilizaComanda;
  @JsonKey(name: "RequerObservacaoAoAbrirPedidoInterno")
  bool requerObservacaoAoAbrirPedidoInterno;

  ConfiguracoesProWaiterWeb();

  factory ConfiguracoesProWaiterWeb.fromJson(Map<String, dynamic> json) => _$ConfiguracoesProWaiterWebFromJson(json);
  Map<String, dynamic> toJson() => _$ConfiguracoesProWaiterWebToJson(this);
}

abstract class Categoria {
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "Nome")
  String nome;
  @JsonKey(name: "Posicao")
  int posicao;
  @JsonKey(name: "CorFundo")
  String corFundo;
  @JsonKey(name: "CorFonte")
  String corFonte;

  Categoria({@required this.codigo, @required this.nome, @required this.posicao, @required this.corFundo, @required this.corFonte});
}

@JsonSerializable()
class TipoBebida extends Categoria {
  TipoBebida({@required int codigo, @required String nome, @required int posicao, @required String corFundo, @required String corFonte})
      : super(codigo: codigo, nome: nome, posicao: posicao, corFundo: corFundo, corFonte: corFonte);

  factory TipoBebida.fromJson(Map<String, dynamic> json) => _$TipoBebidaFromJson(json);
  Map<String, dynamic> toJson() => _$TipoBebidaToJson(this);
}

@JsonSerializable()
class TipoRefeicao extends Categoria {
  TipoRefeicao({@required int codigo, @required String nome, @required int posicao, @required String corFundo, @required String corFonte})
      : super(codigo: codigo, nome: nome, posicao: posicao, corFundo: corFundo, corFonte: corFonte);

  factory TipoRefeicao.fromJson(Map<String, dynamic> json) => _$TipoRefeicaoFromJson(json);
  Map<String, dynamic> toJson() => _$TipoRefeicaoToJson(this);
}

class CategoriaModelo extends Categoria {}

@JsonSerializable()
class Mesa {
  @JsonKey(name: "UltimoPedido")
  PedidoInterno ultimoPedido;
  @JsonKey(name: "Codigo")
  final int codigo;
  @JsonKey(name: "Descricao")
  final String descricao;
  @JsonKey(name: "CodUltimoPedido")
  final int codUtilimoPedido;
  @JsonKey(name: "Observacoes")
  String observacoes;

  Mesa({
    @required this.codigo,
    @required this.descricao,
    @required this.codUtilimoPedido,
    @required this.observacoes,
  });

  factory Mesa.fromJson(Map<String, dynamic> json) => _$MesaFromJson(json);
  Map<String, dynamic> toJson() => _$MesaToJson(this);
}

@JsonSerializable()
class LocalInterno {
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "Nome")
  String nome;

  LocalInterno({
    @required this.codigo,
    @required this.nome,
  });

  factory LocalInterno.fromJson(Map<String, dynamic> json) => _$LocalInternoFromJson(json);
  Map<String, dynamic> toJson() => _$LocalInternoToJson(this);
}

@JsonSerializable()
class ComponenteRefeicao {
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "Nome")
  String nome;

  ComponenteRefeicao({
    @required this.codigo,
    @required this.nome,
  });

  factory ComponenteRefeicao.fromJson(Map<String, dynamic> json) => _$ComponenteRefeicaoFromJson(json);
  Map<String, dynamic> toJson() => _$ComponenteRefeicaoToJson(this);
}

@JsonSerializable()
class ComponenteRefeicaoPedido {
  @JsonKey(name: "CodComponente")
  final int codComponente;
  @JsonKey(name: "Quantidade")
  final int quantidade;

  ComponenteRefeicaoPedido({this.codComponente, this.quantidade});

  factory ComponenteRefeicaoPedido.fromJson(Map<String, dynamic> json) => _$ComponenteRefeicaoPedidoFromJson(json);
  Map<String, dynamic> toJson() => _$ComponenteRefeicaoPedidoToJson(this);
}

@JsonSerializable()
class Bebida {
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "Nome")
  String nome;
  @JsonKey(name: "Valor")
  double valor;
  @JsonKey(name: "CodTipo")
  int codTipo;
  @JsonKey(name: "Ativo")
  bool ativo;
  @JsonKey(name: "PercDesconto")
  double percDesconto;

  Bebida(
      {@required this.codigo, @required this.nome, @required this.valor, @required this.codTipo, @required this.ativo, @required this.percDesconto});

  factory Bebida.fromJson(Map<String, dynamic> json) => _$BebidaFromJson(json);
  Map<String, dynamic> toJson() => _$BebidaToJson(this);
}

@JsonSerializable(ignoreUnannotated: true)
class BebidaDoPedido extends ItemPedido implements IClonable {
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "CodPedido")
  int codPedido;
  @JsonKey(name: "CodBebida")
  int codBebida;
  @JsonKey(name: "Bebida")
  Bebida bebida;
  @JsonKey(name: "Observacoes")
  String observacoes;
  @JsonKey(name: "Valor")
  double valor;

  //Chave utilizada para poder remover todos os itens do modelo, caso o usuário remova algum item antes de enviar para cozinha
  GlobalKey chaveParaVincluarModelo;
  ModeloPedido modeloPedido;

  BebidaDoPedido(
      {this.codigo,
      @required this.codPedido,
      @required this.codBebida,
      @required this.bebida,
      this.observacoes,
      @required this.valor,
      this.chaveParaVincluarModelo,
      this.modeloPedido})
      : super();

  bool get enviadoACozinha {
    return codigo != null && codigo != 0;
  }

  bool get personalizado {
    return !observacoes.isNullOrWhiteSpace();
  }

  @override
  BebidaDoPedido clone() {
    if (bebida == null) return null;

    BebidaDoPedido novaBebida = BebidaDoPedido(
        codPedido: this.codPedido,
        codBebida: this.codBebida,
        bebida: this.bebida,
        observacoes: this.observacoes,
        valor: this.valor,
        chaveParaVincluarModelo: this.chaveParaVincluarModelo,
        modeloPedido: this.modeloPedido);

    return novaBebida;
  }

  factory BebidaDoPedido.fromJson(Map<String, dynamic> json) => _$BebidaDoPedidoFromJson(json);
  Map<String, dynamic> toJson() => _$BebidaDoPedidoToJson(this);
}

@JsonSerializable()
class TamanhoRefeicao {
  @JsonKey(name: "Codigo")
  String codigo;
  @JsonKey(name: "Nome")
  String nome;
  @JsonKey(name: "Posicao")
  int posicao;

  TamanhoRefeicao({@required this.codigo, @required this.nome, @required this.posicao});

  factory TamanhoRefeicao.fromJson(Map<String, dynamic> json) => _$TamanhoRefeicaoFromJson(json);
  Map<String, dynamic> toJson() => _$TamanhoRefeicaoToJson(this);
}

@JsonSerializable()
class Refeicao {
  @JsonKey(name: "ComponentesRefeicao")
  List<ComponenteRefeicao> componentesRefeicao;
  @JsonKey(name: "Tipo")
  TipoRefeicao tipo;
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "Nome")
  String nome;
  @JsonKey(name: "CodTipo")
  int codTipo;

  Refeicao({
    @required this.componentesRefeicao,
    @required this.tipo,
    @required this.codigo,
    @required this.nome,
    @required this.codTipo,
  });

  factory Refeicao.fromJson(Map<String, dynamic> json) => _$RefeicaoFromJson(json);
  Map<String, dynamic> toJson() => _$RefeicaoToJson(this);

  @override
  bool operator ==(o) => o is Refeicao && o.codigo == this.codigo;

  @override
  int get hashCode => codigo | 2 | 4 | 8;
}

@JsonSerializable()
class RefeicaoDoCardapio {
  @JsonKey(name: "Refeicao")
  Refeicao refeicao;
  @JsonKey(name: "TamanhoRefeicao")
  TamanhoRefeicao tamanhoRefeicao;
  @JsonKey(name: "CodRefeicao")
  int codRefeicao;
  @JsonKey(name: "CodTamanho")
  String codTamanho;
  @JsonKey(name: "Valor")
  double valor;
  @JsonKey(name: "Ativo")
  bool ativo;
  @JsonKey(name: "DeComposicao")
  bool deComposicao;
  @JsonKey(name: "PercDesconto")
  double percDesconto;

  RefeicaoDoCardapio({
    @required this.refeicao,
    @required this.tamanhoRefeicao,
    @required this.codRefeicao,
    @required this.codTamanho,
    @required this.valor,
    @required this.ativo,
    @required this.deComposicao,
    @required this.percDesconto,
  });

  factory RefeicaoDoCardapio.fromJson(Map<String, dynamic> json) => _$RefeicaoDoCardapioFromJson(json);
  Map<String, dynamic> toJson() => _$RefeicaoDoCardapioToJson(this);
}

@JsonSerializable(ignoreUnannotated: true)
class RefeicaoDoPedido extends ItemPedido implements IClonable {
  @JsonKey(name: "ComponentesRefeicaoPedido")
  List<ComponenteRefeicaoPedido> componentesRefeicaoPedido;
  @JsonKey(name: "RefeicaoDoCardapio")
  RefeicaoDoCardapio refeicaoDoCardapio;
  @JsonKey(name: "Tamanho")
  TamanhoRefeicao tamanho;
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "CodPedido")
  int codPedido;
  @JsonKey(name: "CodRefeicao")
  int codRefeicao;
  @JsonKey(name: "CodTamanho")
  String codTamanho;
  @JsonKey(name: "Observacoes")
  String observacoes;
  @JsonKey(name: "Valor")
  double valor;
  bool get enviadoACozinha {
    return codigo != null && codigo != 0;
  }

  //Chave utilizada para poder remover todos os itens do modelo, caso o usuário remova algum item antes de enviar para cozinha
  GlobalKey chaveParaVincluarModelo;
  ModeloPedido modeloPedido;

  bool get personalizado {
    return refeicaoDoCardapio.refeicao.componentesRefeicao.length != componentesRefeicaoPedido.length || !observacoes.isNullOrWhiteSpace();
  }

  String get descricao {
    return refeicaoDoCardapio.refeicao.nome;
  }

  RefeicaoDoPedido(
      {this.componentesRefeicaoPedido,
      this.refeicaoDoCardapio,
      this.tamanho,
      this.codigo,
      @required this.codPedido,
      @required this.codRefeicao,
      @required this.codTamanho,
      @required this.observacoes,
      this.valor,
      this.chaveParaVincluarModelo,
      this.modeloPedido})
      : super();

  factory RefeicaoDoPedido.fromJson(Map<String, dynamic> json) => _$RefeicaoDoPedidoFromJson(json);
  Map<String, dynamic> toJson() => _$RefeicaoDoPedidoToJson(this);

  @override
  RefeicaoDoPedido clone() {
    RefeicaoDoPedido novaRef = RefeicaoDoPedido(
        codPedido: this.codPedido,
        codRefeicao: this.codRefeicao,
        codTamanho: this.codTamanho,
        observacoes: this.observacoes,
        tamanho: this.tamanho,
        valor: this.valor,
        refeicaoDoCardapio: this.refeicaoDoCardapio,
        chaveParaVincluarModelo: this.chaveParaVincluarModelo,
        modeloPedido: this.modeloPedido,
        componentesRefeicaoPedido:
            this.componentesRefeicaoPedido.map((e) => ComponenteRefeicaoPedido(codComponente: e.codComponente, quantidade: e.quantidade)).toList());
    return novaRef;
  }
}

@JsonSerializable()
class PedidoInterno {
  @JsonKey(name: "BebidasDoPedido")
  List<BebidaDoPedido> bebidasDoPedido;
  @JsonKey(name: "RefeicoesDoPedido")
  List<RefeicaoDoPedido> refeicoesDoPedido;
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "DataTermino")
  DateTime dataTermino;
  @JsonKey(name: "NomeUsuario")
  String nomeUsuario;
  @JsonKey(name: "Observacoes")
  String observacoes;
  @JsonKey(name: "ValorTotal")
  double valorTotal;
  @JsonKey(name: "Acrescimos")
  double acrescimos;
  @JsonKey(name: "Descontos")
  double descontos;

  bool get possuiItensNaoEnviados {
    return refeicoesDoPedido != null && refeicoesDoPedido.any((r) => !r.enviadoACozinha) ||
        bebidasDoPedido != null && bebidasDoPedido.any((b) => !b.enviadoACozinha);
  }

  PedidoInterno(
      {@required this.bebidasDoPedido,
      @required this.refeicoesDoPedido,
      @required this.codigo,
      @required this.dataTermino,
      @required this.nomeUsuario,
      @required this.observacoes,
      @required this.valorTotal,
      @required this.acrescimos,
      @required this.descontos});

  factory PedidoInterno.fromJson(Map<String, dynamic> json) => _$PedidoInternoFromJson(json);
  Map<String, dynamic> toJson() => _$PedidoInternoToJson(this);
}

@JsonSerializable() //Precisa ser nullable pela recursão dos relacionamentos
class ComponenteComposicaoRefeicaoCardapio {
  @JsonKey(name: "CodRefeicao")
  int codRefeicao;
  @JsonKey(name: "Refeicao")
  Refeicao refeicao;
  @JsonKey(name: "CodTamanho")
  String codTamanho;
  @JsonKey(name: "Tamanho")
  TamanhoRefeicao tamanho;
  @JsonKey(name: "CodComponente")
  int codComponente;
  @JsonKey(name: "ComponenteRefeicao")
  ComponenteRefeicao componenteRefeicao;
  @JsonKey(name: "Valor")
  double valor;
  @JsonKey(name: "CalculoProporcional")
  bool calculoProporcional;
  @JsonKey(name: "Ativo")
  bool ativo;
  @JsonKey(name: "CodUnidade")
  String codUnidade;
  @JsonKey(name: "Unidade")
  UnidadeComponenteComposicao unidade;

  ComponenteComposicaoRefeicaoCardapio(
      {@required this.codRefeicao,
      @required this.refeicao,
      @required this.codTamanho,
      @required this.tamanho,
      @required this.codComponente,
      @required this.valor,
      @required this.calculoProporcional,
      @required this.ativo,
      @required this.codUnidade,
      @required this.unidade});

  factory ComponenteComposicaoRefeicaoCardapio.fromJson(Map<String, dynamic> json) => _$ComponenteComposicaoRefeicaoCardapioFromJson(json);
  Map<String, dynamic> toJson() => _$ComponenteComposicaoRefeicaoCardapioToJson(this);
}

@JsonSerializable()
class UnidadeComponenteComposicao {
  static const String codPartes = "PR";
  static const String codPorcao = "PÇ";
  static const String codUnidade = "UN";

  @JsonKey(name: "Codigo")
  String codigo;
  @JsonKey(name: "Descricao")
  String descricao;

  UnidadeComponenteComposicao({@required this.codigo, @required this.descricao});

  factory UnidadeComponenteComposicao.fromJson(Map<String, dynamic> json) => _$UnidadeComponenteComposicaoFromJson(json);
  Map<String, dynamic> toJson() => _$UnidadeComponenteComposicaoToJson(this);
}

@JsonSerializable()
class ItensNaoEnviados {
  @JsonKey(name: "CodPedido")
  int codPedido;
  @JsonKey(name: "CodMesa")
  int codMesa;
  @JsonKey(name: "RefeicoesDoPedido")
  List<RefeicaoDoPedido> refeicoesDoPedido;
  @JsonKey(name: "BebidasDoPedido")
  List<BebidaDoPedido> bebidasDoPedido;
  @JsonKey(name: "Mensagem")
  String mensagem;
  @JsonKey(name: "CodLocalInternoEntrega")
  int codLocalInternoEntrega;
  @JsonKey(name: "Acrescimos")
  double acrescimos;
  @JsonKey(name: "Descontos")
  double descontos;

  ItensNaoEnviados(
      {@required this.codPedido,
      @required this.codMesa,
      @required this.refeicoesDoPedido,
      @required this.bebidasDoPedido,
      @required this.mensagem,
      @required this.acrescimos,
      @required this.descontos,
      this.codLocalInternoEntrega});

  factory ItensNaoEnviados.fromJson(Map<String, dynamic> json) => _$ItensNaoEnviadosFromJson(json);
  Map<String, dynamic> toJson() => _$ItensNaoEnviadosToJson(this);
}

//Modelos

@JsonSerializable()
class ModeloComponenteRefeicaoPedido {
  @JsonKey(name: "CodModeloRefeicaoPedido")
  int codModeloRefeicaoPedido;
  @JsonKey(name: "ModeloRefeicaoDoPedido")
  ModeloRefeicaoPedido modeloRefeicaoDoPedido;
  @JsonKey(name: "CodComponente")
  int codComponente;
  @JsonKey(name: "ComponenteRefeicao")
  ComponenteRefeicao componenteRefeicao;
  @JsonKey(name: "Quantidade")
  int quantidade;

  ModeloComponenteRefeicaoPedido(
      {@required this.codModeloRefeicaoPedido,
      @required this.modeloRefeicaoDoPedido,
      @required this.codComponente,
      @required this.componenteRefeicao,
      @required this.quantidade});

  factory ModeloComponenteRefeicaoPedido.fromJson(Map<String, dynamic> json) => _$ModeloComponenteRefeicaoPedidoFromJson(json);
  Map<String, dynamic> toJson() => _$ModeloComponenteRefeicaoPedidoToJson(this);
}

@JsonSerializable()
class ModeloRefeicaoPedido {
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "CodModeloPedido")
  int codModeloPedido;
  @JsonKey(name: "CodRefeicao")
  int codRefeicao;
  @JsonKey(name: "CodTamanho")
  String codTamanho;
  @JsonKey(name: "ModeloComponentesRefeicaoPedido")
  List<ModeloComponenteRefeicaoPedido> modeloComponentesRefeicaoPedido;
  @JsonKey(name: "Observacoes")
  String observacoes;

  ModeloRefeicaoPedido(
      {@required this.codigo,
      @required this.codModeloPedido,
      @required this.codRefeicao,
      @required this.codTamanho,
      @required this.modeloComponentesRefeicaoPedido,
      @required this.observacoes});

  factory ModeloRefeicaoPedido.fromJson(Map<String, dynamic> json) => _$ModeloRefeicaoPedidoFromJson(json);
  Map<String, dynamic> toJson() => _$ModeloRefeicaoPedidoToJson(this);
}

@JsonSerializable()
class ModeloBebidaPedido {
  @JsonKey(name: "Codigo")
  int codigo;
  @JsonKey(name: "CodModeloPedido")
  int codModeloPedido;
  @JsonKey(name: "CodBebida")
  int codBebida;
  @JsonKey(name: "Bebida")
  Bebida bebida;
  @JsonKey(name: "Observacoes")
  String observacoes;

  ModeloBebidaPedido(
      {@required this.codigo, @required this.codModeloPedido, @required this.codBebida, @required this.bebida, @required this.observacoes});

  factory ModeloBebidaPedido.fromJson(Map<String, dynamic> json) => _$ModeloBebidaPedidoFromJson(json);
  Map<String, dynamic> toJson() => _$ModeloBebidaPedidoToJson(this);
}

@JsonSerializable()
class ModeloPedido {
  @JsonKey(name: "Codigo")
  
  int codigo;
  @JsonKey(name: "Nome")
  String nome;
  @JsonKey(name: "Desconto")
  double desconto;
  @JsonKey(name: "Acrescimo")
  double acrescimo;
  @JsonKey(name: "Observacoes")
  String observacoes;
  @JsonKey(name: "ModelosBebidaPedido")
  List<ModeloBebidaPedido> modelosBebidaPedido;
  @JsonKey(name: "ModelosRefeicaoPedidos")
  List<ModeloRefeicaoPedido> modelosRefeicaoPedidos;

  ModeloPedido({
    this.codigo,
    this.nome,
    this.desconto,
    this.acrescimo,
    this.observacoes,
    this.modelosBebidaPedido,
    this.modelosRefeicaoPedidos,
  });

  factory ModeloPedido.fromJson(Map<String, dynamic> json) => _$ModeloPedidoFromJson(json);
  Map<String, dynamic> toJson() => _$ModeloPedidoToJson(this);
}
