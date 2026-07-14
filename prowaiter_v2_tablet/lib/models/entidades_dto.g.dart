// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'entidades_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ConfiguracoesAPP _$ConfiguracoesAPPFromJson(Map<String, dynamic> json) {
  return ConfiguracoesAPP()
    ..ipServidor = json['ipServidor'] as String
    ..login = json['login'] as String
    ..senha = json['senha'] as String
    ..dataHoraLogin = json['dataHoraLogin'] == null ? null : DateTime.parse(json['dataHoraLogin'] as String)
    ..mesa = json['mesa'] as String
    ..tema = json['tema'] as String;
}

Map<String, dynamic> _$ConfiguracoesAPPToJson(ConfiguracoesAPP instance) => <String, dynamic>{
      'ipServidor': instance.ipServidor,
      'login': instance.login,
      'senha': instance.senha,
      'dataHoraLogin': instance.dataHoraLogin?.toIso8601String(),
      'mesa': instance.mesa,
      'tema': instance.tema,
    };

ConfiguracoesProWaiterWeb _$ConfiguracoesProWaiterWebFromJson(Map<String, dynamic> json) {
  return ConfiguracoesProWaiterWeb()
    ..utilizaComanda = json['UtilizaComanda'] as bool
    ..requerObservacaoAoAbrirPedidoInterno = json['RequerObservacaoAoAbrirPedidoInterno'] as bool;
}

Map<String, dynamic> _$ConfiguracoesProWaiterWebToJson(ConfiguracoesProWaiterWeb instance) => <String, dynamic>{
      'UtilizaComanda': instance.utilizaComanda,
      'RequerObservacaoAoAbrirPedidoInterno': instance.requerObservacaoAoAbrirPedidoInterno,
    };

TipoBebida _$TipoBebidaFromJson(Map<String, dynamic> json) {
  return TipoBebida(
    codigo: json['Codigo'] as int,
    nome: json['Nome'] as String,
    posicao: json['Posicao'] as int,
    corFundo: json['CorFundo'] as String,
    corFonte: json['CorFonte'] as String,
  );
}

Map<String, dynamic> _$TipoBebidaToJson(TipoBebida instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'Nome': instance.nome,
      'Posicao': instance.posicao,
      'CorFundo': instance.corFundo,
      'CorFonte': instance.corFonte,
    };

TipoRefeicao _$TipoRefeicaoFromJson(Map<String, dynamic> json) {
  return TipoRefeicao(
    codigo: json['Codigo'] as int,
    nome: json['Nome'] as String,
    posicao: json['Posicao'] as int,
    corFundo: json['CorFundo'] as String,
    corFonte: json['CorFonte'] as String,
  );
}

Map<String, dynamic> _$TipoRefeicaoToJson(TipoRefeicao instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'Nome': instance.nome,
      'Posicao': instance.posicao,
      'CorFundo': instance.corFundo,
      'CorFonte': instance.corFonte,
    };

Mesa _$MesaFromJson(Map<String, dynamic> json) {
  return Mesa(
    codigo: json['Codigo'] as int,
    descricao: json['Descricao'] as String,
    codUtilimoPedido: json['CodUltimoPedido'] as int,
    observacoes: json['Observacoes'] as String,
  )..ultimoPedido = json['UltimoPedido'] == null ? null : PedidoInterno.fromJson(json['UltimoPedido'] as Map<String, dynamic>);
}

Map<String, dynamic> _$MesaToJson(Mesa instance) => <String, dynamic>{
      'UltimoPedido': instance.ultimoPedido,
      'Codigo': instance.codigo,
      'Descricao': instance.descricao,
      'CodUltimoPedido': instance.codUtilimoPedido,
      'Observacoes': instance.observacoes,
    };

LocalInterno _$LocalInternoFromJson(Map<String, dynamic> json) {
  return LocalInterno(
    codigo: json['Codigo'] as int,
    nome: json['Nome'] as String,
  );
}

Map<String, dynamic> _$LocalInternoToJson(LocalInterno instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'Nome': instance.nome,
    };

ComponenteRefeicao _$ComponenteRefeicaoFromJson(Map<String, dynamic> json) {
  return ComponenteRefeicao(
    codigo: json['Codigo'] as int,
    nome: json['Nome'] as String,
  );
}

Map<String, dynamic> _$ComponenteRefeicaoToJson(ComponenteRefeicao instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'Nome': instance.nome,
    };

ComponenteRefeicaoPedido _$ComponenteRefeicaoPedidoFromJson(Map<String, dynamic> json) {
  return ComponenteRefeicaoPedido(
    codComponente: json['CodComponente'] as int,
    quantidade: json['Quantidade'] as int,
  );
}

Map<String, dynamic> _$ComponenteRefeicaoPedidoToJson(ComponenteRefeicaoPedido instance) => <String, dynamic>{
      'CodComponente': instance.codComponente,
      'Quantidade': instance.quantidade,
    };

Bebida _$BebidaFromJson(Map<String, dynamic> json) {
  return Bebida(
    codigo: json['Codigo'] as int,
    nome: json['Nome'] as String,
    valor: (json['Valor'] as num).toDouble(),
    codTipo: json['CodTipo'] as int,
    ativo: json['Ativo'] as bool,
    percDesconto: (json['PercDesconto'] as num).toDouble(),
  );
}

Map<String, dynamic> _$BebidaToJson(Bebida instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'Nome': instance.nome,
      'Valor': instance.valor,
      'CodTipo': instance.codTipo,
      'Ativo': instance.ativo,
      'PercDesconto': instance.percDesconto,
    };

BebidaDoPedido _$BebidaDoPedidoFromJson(Map<String, dynamic> json) {
  return BebidaDoPedido(
    codigo: json['Codigo'] as int,
    codPedido: json['CodPedido'] as int,
    codBebida: json['CodBebida'] as int,
    bebida: json['Bebida'] == null ? null : Bebida.fromJson(json['Bebida'] as Map<String, dynamic>),
    observacoes: json['Observacoes'] as String,
    valor: (json['Valor'] as num)?.toDouble(),
  );
}

Map<String, dynamic> _$BebidaDoPedidoToJson(BebidaDoPedido instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'CodPedido': instance.codPedido,
      'CodBebida': instance.codBebida,
      'Bebida': instance.bebida,
      'Observacoes': instance.observacoes,
      'Valor': instance.valor,
    };

TamanhoRefeicao _$TamanhoRefeicaoFromJson(Map<String, dynamic> json) {
  return TamanhoRefeicao(
    codigo: json['Codigo'] as String,
    nome: json['Nome'] as String,
    posicao: json['Posicao'] as int,
  );
}

Map<String, dynamic> _$TamanhoRefeicaoToJson(TamanhoRefeicao instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'Nome': instance.nome,
      'Posicao': instance.posicao,
    };

Refeicao _$RefeicaoFromJson(Map<String, dynamic> json) {
  return Refeicao(
    componentesRefeicao: (json['ComponentesRefeicao'] as List).map((e) => ComponenteRefeicao.fromJson(e as Map<String, dynamic>)).toList(),
    tipo: TipoRefeicao.fromJson(json['Tipo'] as Map<String, dynamic>),
    codigo: json['Codigo'] as int,
    nome: json['Nome'] as String,
    codTipo: json['CodTipo'] as int,
  );
}

Map<String, dynamic> _$RefeicaoToJson(Refeicao instance) => <String, dynamic>{
      'ComponentesRefeicao': instance.componentesRefeicao,
      'Tipo': instance.tipo,
      'Codigo': instance.codigo,
      'Nome': instance.nome,
      'CodTipo': instance.codTipo,
    };

RefeicaoDoCardapio _$RefeicaoDoCardapioFromJson(Map<String, dynamic> json) {
  return RefeicaoDoCardapio(
    refeicao: Refeicao.fromJson(json['Refeicao'] as Map<String, dynamic>),
    tamanhoRefeicao: TamanhoRefeicao.fromJson(json['TamanhoRefeicao'] as Map<String, dynamic>),
    codRefeicao: json['CodRefeicao'] as int,
    codTamanho: json['CodTamanho'] as String,
    valor: (json['Valor'] as num).toDouble(),
    ativo: json['Ativo'] as bool,
    deComposicao: json['DeComposicao'] as bool,
    percDesconto: (json['PercDesconto'] as num).toDouble(),
  );
}

Map<String, dynamic> _$RefeicaoDoCardapioToJson(RefeicaoDoCardapio instance) => <String, dynamic>{
      'Refeicao': instance.refeicao,
      'TamanhoRefeicao': instance.tamanhoRefeicao,
      'CodRefeicao': instance.codRefeicao,
      'CodTamanho': instance.codTamanho,
      'Valor': instance.valor,
      'Ativo': instance.ativo,
      'DeComposicao': instance.deComposicao,
      'PercDesconto': instance.percDesconto,
    };

RefeicaoDoPedido _$RefeicaoDoPedidoFromJson(Map<String, dynamic> json) {
  return RefeicaoDoPedido(
    componentesRefeicaoPedido:
        (json['ComponentesRefeicaoPedido'] as List).map((e) => ComponenteRefeicaoPedido.fromJson(e as Map<String, dynamic>)).toList(),
    refeicaoDoCardapio: RefeicaoDoCardapio.fromJson(json['RefeicaoDoCardapio'] as Map<String, dynamic>),
    tamanho: TamanhoRefeicao.fromJson(json['Tamanho'] as Map<String, dynamic>),
    codigo: json['Codigo'] as int,
    codPedido: json['CodPedido'] as int,
    codRefeicao: json['CodRefeicao'] as int,
    codTamanho: json['CodTamanho'] as String,
    observacoes: json['Observacoes'] as String,
    valor: (json['Valor'] as num).toDouble(),
  );
}

Map<String, dynamic> _$RefeicaoDoPedidoToJson(RefeicaoDoPedido instance) => <String, dynamic>{
      'ComponentesRefeicaoPedido': instance.componentesRefeicaoPedido,
      'RefeicaoDoCardapio': instance.refeicaoDoCardapio,
      'Tamanho': instance.tamanho,
      'Codigo': instance.codigo,
      'CodPedido': instance.codPedido,
      'CodRefeicao': instance.codRefeicao,
      'CodTamanho': instance.codTamanho,
      'Observacoes': instance.observacoes,
      'Valor': instance.valor,
    };

PedidoInterno _$PedidoInternoFromJson(Map<String, dynamic> json) {
  return PedidoInterno(
      bebidasDoPedido: (json['BebidasDoPedido'] as List)?.map((e) => e == null ? null : BebidaDoPedido.fromJson(e as Map<String, dynamic>))?.toList(),
      refeicoesDoPedido:
          (json['RefeicoesDoPedido'] as List)?.map((e) => e == null ? null : RefeicaoDoPedido.fromJson(e as Map<String, dynamic>))?.toList(),
      codigo: json['Codigo'] as int,
      dataTermino: json['DataTermino'] == null ? null : DateTime.parse(json['DataTermino'] as String),
      nomeUsuario: json['NomeUsuario'] as String,
      observacoes: json['Observacoes'] as String,
      valorTotal: json["ValorTotal"] as double,
      acrescimos: json["Acrescimos"] as double,
      descontos: json["Descontos"] as double);
}

Map<String, dynamic> _$PedidoInternoToJson(PedidoInterno instance) => <String, dynamic>{
      'BebidasDoPedido': instance.bebidasDoPedido,
      'RefeicoesDoPedido': instance.refeicoesDoPedido,
      'Codigo': instance.codigo,
      'DataTermino': instance.dataTermino?.toIso8601String(),
      'NomeUsuario': instance.nomeUsuario,
      'Observacoes': instance.observacoes,
      "ValorTotal": instance.valorTotal,
      "Acrescimos": instance.acrescimos,
      "Descontos": instance.descontos
    };

ComponenteComposicaoRefeicaoCardapio _$ComponenteComposicaoRefeicaoCardapioFromJson(Map<String, dynamic> json) {
  return ComponenteComposicaoRefeicaoCardapio(
    codRefeicao: json['CodRefeicao'] as int,
    refeicao: json['Refeicao'] == null ? null : Refeicao.fromJson(json['Refeicao'] as Map<String, dynamic>),
    codTamanho: json['CodTamanho'] as String,
    tamanho: json['Tamanho'] == null ? null : TamanhoRefeicao.fromJson(json['Tamanho'] as Map<String, dynamic>),
    codComponente: json['CodComponente'] as int,
    valor: (json['Valor'] as num)?.toDouble(),
    calculoProporcional: json['CalculoProporcional'] as bool,
    ativo: json['Ativo'] as bool,
    codUnidade: json['CodUnidade'] as String,
    unidade: json['Unidade'] == null ? null : UnidadeComponenteComposicao.fromJson(json['Unidade'] as Map<String, dynamic>),
  )..componenteRefeicao = json['ComponenteRefeicao'] == null ? null : ComponenteRefeicao.fromJson(json['ComponenteRefeicao'] as Map<String, dynamic>);
}

Map<String, dynamic> _$ComponenteComposicaoRefeicaoCardapioToJson(ComponenteComposicaoRefeicaoCardapio instance) => <String, dynamic>{
      'CodRefeicao': instance.codRefeicao,
      'Refeicao': instance.refeicao,
      'CodTamanho': instance.codTamanho,
      'Tamanho': instance.tamanho,
      'CodComponente': instance.codComponente,
      'ComponenteRefeicao': instance.componenteRefeicao,
      'Valor': instance.valor,
      'CalculoProporcional': instance.calculoProporcional,
      'Ativo': instance.ativo,
      'CodUnidade': instance.codUnidade,
      'Unidade': instance.unidade,
    };

UnidadeComponenteComposicao _$UnidadeComponenteComposicaoFromJson(Map<String, dynamic> json) {
  return UnidadeComponenteComposicao(
    codigo: json['Codigo'] as String,
    descricao: json['Descricao'] as String,
  );
}

Map<String, dynamic> _$UnidadeComponenteComposicaoToJson(UnidadeComponenteComposicao instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'Descricao': instance.descricao,
    };

ItensNaoEnviados _$ItensNaoEnviadosFromJson(Map<String, dynamic> json) {
  return ItensNaoEnviados(
    codPedido: json['CodPedido'] as int,
    codMesa: json['CodMesa'] as int,
    refeicoesDoPedido:
        (json['RefeicoesDoPedido'] as List)?.map((e) => e == null ? null : RefeicaoDoPedido.fromJson(e as Map<String, dynamic>))?.toList(),
    bebidasDoPedido: (json['BebidasDoPedido'] as List)?.map((e) => e == null ? null : BebidaDoPedido.fromJson(e as Map<String, dynamic>))?.toList(),
    mensagem: json['Mensagem'] as String,
    acrescimos: (json['Acrescimos'] as num)?.toDouble(),
    descontos: (json['Descontos'] as num)?.toDouble(),
    codLocalInternoEntrega: json['CodLocalInternoEntrega'] as int,
  );
}

Map<String, dynamic> _$ItensNaoEnviadosToJson(ItensNaoEnviados instance) => <String, dynamic>{
      'CodPedido': instance.codPedido,
      'CodMesa': instance.codMesa,
      'RefeicoesDoPedido': instance.refeicoesDoPedido,
      'BebidasDoPedido': instance.bebidasDoPedido,
      'Mensagem': instance.mensagem,
      'CodLocalInternoEntrega': instance.codLocalInternoEntrega,
      'Acrescimos': instance.acrescimos,
      'Descontos': instance.descontos,
    };

ModeloComponenteRefeicaoPedido _$ModeloComponenteRefeicaoPedidoFromJson(Map<String, dynamic> json) {
  return ModeloComponenteRefeicaoPedido(
    codModeloRefeicaoPedido: json['CodModeloRefeicaoPedido'] as int,
    modeloRefeicaoDoPedido:
        json['ModeloRefeicaoDoPedido'] == null ? null : ModeloRefeicaoPedido.fromJson(json['ModeloRefeicaoDoPedido'] as Map<String, dynamic>),
    codComponente: json['CodComponente'] as int,
    componenteRefeicao: json['ComponenteRefeicao'] == null ? null : ComponenteRefeicao.fromJson(json['ComponenteRefeicao'] as Map<String, dynamic>),
    quantidade: json['Quantidade'] as int,
  );
}

Map<String, dynamic> _$ModeloComponenteRefeicaoPedidoToJson(ModeloComponenteRefeicaoPedido instance) => <String, dynamic>{
      'CodModeloRefeicaoPedido': instance.codModeloRefeicaoPedido,
      'ModeloRefeicaoDoPedido': instance.modeloRefeicaoDoPedido,
      'CodComponente': instance.codComponente,
      'ComponenteRefeicao': instance.componenteRefeicao,
      'Quantidade': instance.quantidade,
    };

ModeloRefeicaoPedido _$ModeloRefeicaoPedidoFromJson(Map<String, dynamic> json) {
  return ModeloRefeicaoPedido(
    codigo: json['Codigo'] as int,
    codModeloPedido: json['CodModeloPedido'] as int,
    codRefeicao: json['CodRefeicao'] as int,
    codTamanho: json['CodTamanho'] as String,
    modeloComponentesRefeicaoPedido: (json['ModeloComponentesRefeicaoPedido'] as List)
        ?.map((e) => e == null ? null : ModeloComponenteRefeicaoPedido.fromJson(e as Map<String, dynamic>))
        ?.toList(),
    observacoes: json['Observacoes'] as String,
  );
}

Map<String, dynamic> _$ModeloRefeicaoPedidoToJson(ModeloRefeicaoPedido instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'CodModeloPedido': instance.codModeloPedido,
      'CodRefeicao': instance.codRefeicao,
      'CodTamanho': instance.codTamanho,
      'ModeloComponentesRefeicaoPedido': instance.modeloComponentesRefeicaoPedido,
      'Observacoes': instance.observacoes,
    };

ModeloBebidaPedido _$ModeloBebidaPedidoFromJson(Map<String, dynamic> json) {
  return ModeloBebidaPedido(
    codigo: json['Codigo'] as int,
    codModeloPedido: json['CodModeloPedido'] as int,
    codBebida: json['CodBebida'] as int,
    bebida: json['Bebida'] == null ? null : Bebida.fromJson(json['Bebida'] as Map<String, dynamic>),
    observacoes: json['Observacoes'] as String,
  );
}

Map<String, dynamic> _$ModeloBebidaPedidoToJson(ModeloBebidaPedido instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'CodModeloPedido': instance.codModeloPedido,
      'CodBebida': instance.codBebida,
      'Bebida': instance.bebida,
      'Observacoes': instance.observacoes,
    };

ModeloPedido _$ModeloPedidoFromJson(Map<String, dynamic> json) {
  return ModeloPedido(
    codigo: json['Codigo'] as int,
    nome: json['Nome'] as String,
    desconto: (json['Desconto'] as num)?.toDouble(),
    acrescimo: (json['Acrescimo'] as num)?.toDouble(),
    observacoes: json['Observacoes'] as String,
    modelosBebidaPedido:
        (json['ModelosBebidaPedido'] as List)?.map((e) => e == null ? null : ModeloBebidaPedido.fromJson(e as Map<String, dynamic>))?.toList(),
    modelosRefeicaoPedidos:
        (json['ModelosRefeicaoPedidos'] as List)?.map((e) => e == null ? null : ModeloRefeicaoPedido.fromJson(e as Map<String, dynamic>))?.toList(),
  );
}

Map<String, dynamic> _$ModeloPedidoToJson(ModeloPedido instance) => <String, dynamic>{
      'Codigo': instance.codigo,
      'Nome': instance.nome,
      'Desconto': instance.desconto,
      'Acrescimo': instance.acrescimo,
      'Observacoes': instance.observacoes,
      'ModelosBebidaPedido': instance.modelosBebidaPedido,
      'ModelosRefeicaoPedidos': instance.modelosRefeicaoPedidos,
    };
