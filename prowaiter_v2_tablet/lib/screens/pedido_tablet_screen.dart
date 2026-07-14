import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:minimize_app/minimize_app.dart';
import 'package:prowaiter_v2/components/botao_mesa.dart';
import '../models/entidades_dto.dart';
import '../util/servicos_app_provider.dart';
import '../components/scaffold_padrao.dart';
import '../util/app_routes.dart';
import '../components/circular_progress_padrao.dart';
import '../util/util.dart' as util;
import 'package:back_button_interceptor/back_button_interceptor.dart';

class PedidoTabletScreen extends StatefulWidget {
  @override
  _PeditoTabletScreenState createState() => _PeditoTabletScreenState();
}

class _PeditoTabletScreenState extends State<PedidoTabletScreen> {
  ServicosAPPProvider _provider;
  bool _erro = false;
  bool _exibirCarregando = true;
  Mesa _mesaClicada;
  String _msgErro;

  @override
  void initState() {
    super.initState();
    BackButtonInterceptor.add(_backButton);
    _provider = Provider.of<ServicosAPPProvider>(context, listen: false);
    _excluirPedidoVazio();
  }

  @override
  void dispose() {
    super.dispose();
    BackButtonInterceptor.remove(_backButton);
  }

  bool _backButton(bool stopDefaultButtonEvent, RouteInfo routeInfo) {
    MinimizeApp.minimizeApp();
    return true;
  }

  void _excluirPedidoVazio() async {
    if (_provider.codMesaAtual != null) {
      try {
        await _provider.excluirPedidoVazio();
      } catch (e, s) {
        util.exibirErro(context, e, s,
            msg: "Erro ao tentar exluir pedido vazio. ");
      }
    }
  }

  void _irParaCategorias() async {
    _exibirScreenCarregando(true);
    try {
      _mesaClicada = await _provider.recuperar<Mesa>(
          "Mesas", _provider.configAPP.mesa.toString());
      if (_mesaClicada == null) {
        util.exibirMensagem(context, "Erro ao tentar obter dados da rede.");
      } else {
        //ja tem pedido
        if (_mesaClicada.codUtilimoPedido != null) {
          //atualizamos o codigo do pedido atual no provider
          _provider.atualizarEstadoPedido(_mesaClicada);
          //vamos para as categorias
          Navigator.of(context).pushReplacementNamed(AppRouts.CategoriasRoute);
          return;
        } else {
          //mesa vazia
          _iniciarCategoriasComPedidoNovo();
          return;
        }
      }
    } catch (e, s) {
      util.exibirErro(context, e, s);
    }
    _exibirScreenCarregando(false);
  }

  void _iniciarCategoriasComPedidoNovo() async {
    _exibirScreenCarregando(true);
    try {
      var retorno = await _provider.inserir<Mesa>(
          controller: "CriarPedidoInterno", objeto: _mesaClicada);
      {
        if (retorno == null) {
          util.exibirMensagem(context, "Erro ao tenter criar peido interno");
        } else {
          _mesaClicada = retorno;
          _provider.atualizarEstadoPedido(_mesaClicada);
          Navigator.of(context).pushReplacementNamed(AppRouts.CategoriasRoute);
          return;
        }
      }
    } catch (e, s) {
      util.exibirErro(context, e, s, msg: "Erro ao tentar criar pedido.");
    }
    _exibirScreenCarregando(false);
  }

  void _exibirScreenCarregando(bool exibir) {
    setState(() {
      _exibirCarregando = exibir;
    });
  }

  @override
  Widget build(BuildContext context) {
    return ScaffoldPadrao(
      //onBackPress: _voltarTelaCategorias,
      exibirBackButton: true,
      titulo: "Pedido",
      //conteudoBottomAppBar: _criarBotoesRodape(),
      body: _criarTela(),
    );
  }

  Widget _criarTela() {
    if (_erro) {
      return util.obterWidgetErroParaTela(context, _msgErro, refresh: () {});
    }

return Stack(
  children: [
    Container(
      decoration: const BoxDecoration(
        image: DecorationImage(
          image: AssetImage("assets/background_login.jpeg"),
          fit: BoxFit.cover,
        ),
      ),
    ),
    Center( // Este Center vai centralizar horizontal e verticalmente
      child: SingleChildScrollView(
        // Removido: margin: const EdgeInsets.only(top: 250),
        child: Padding(
          padding: const EdgeInsets.all(8.0),
          child: Padding(
            padding: const EdgeInsets.all(10),
            child: Padding(
              padding: const EdgeInsets.all(6.0),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center, // Garante que itens na Column se centralizem verticalmente
                children: <Widget>[
                  // O seu botão (ou o Container que o envolve)
                  Container(
                    // Se precisar de algum espaçamento em relação a outros elementos na Column,
                    // adicione um SizedBox aqui, mas não um margin fixo que impeça a centralização geral.
                    // Exemplo: const SizedBox(height: 50),
                    child: Padding(
                      padding: const EdgeInsets.all(10.0),
                      child: ButtonTheme( // Ou ElevatedButton
                        minWidth: 280,
                        height: 45,
                        child: RaisedButton(
                          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(18.0), side: BorderSide(color: Colors.white)),
                          child: Text("Fazer pedido"),
                          color: Colors.white,
                          textColor: Color(0xFFfe4c57),
                          onPressed: _irParaCategorias,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    ),
  ],
);
  }
}
