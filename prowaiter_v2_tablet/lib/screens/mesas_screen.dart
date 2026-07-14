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

class MesasScreen extends StatefulWidget {
  @override
  _MesasScreenState createState() => _MesasScreenState();
}

class _MesasScreenState extends State<MesasScreen> {
  TextEditingController _obsController = new TextEditingController();
  String _textoTitulo;
  ServicosAPPProvider _provider;
  List<Mesa> _mesas;
  bool _erro = false;
  bool _exibirCarregando = true;
  Mesa _mesaClicada;
  String _msgErro;

  @override
  void initState() {
    super.initState();    
    BackButtonInterceptor.add(_backButton);
     _provider = Provider.of<ServicosAPPProvider>(context, listen: false);
     _textoTitulo = _provider.configWeb.utilizaComanda ? "Comandas" : "Mesas";
     _carregarMesas();    
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

  void _carregarMesas() async {
    try {
      setState(() {
        _msgErro = "";
        _erro = false;
        _exibirCarregando = true;
      });

      if (_provider.codMesaAtual != null) {
        try {
          await _provider.excluirPedidoVazio();
        } catch (e, s) {
          util.exibirErro(context, e, s, msg: "Erro ao tentar exluir pedido vazio. ");
        }
      }     

      _mesas = await _provider.obterEntidades<Mesa>("Mesas");
      setState(() {
        _exibirCarregando = false;
      });
    } catch (e, s) {
      _msgErro = "Erro ao tentar carregar mesas." + e.toString() + s.toString();
      setState(() {        
        _erro = true;
        _exibirCarregando = false;
      });
    }
  }

  void _solicitarObservacao(BuildContext context) {    
    var _alert = new AlertDialog(
      title: Text("Observação"),
      content: SingleChildScrollView(
        child: Column(
          children: <Widget>[
            TextField(
              controller: _obsController,
            ),
          ],
        ),
      ),
      actions: <Widget>[
        FlatButton(
          child: Text("Ok"),
          onPressed: () {
            String texto = _obsController.text;
            if (!texto.isNullOrWhiteSpace()) {
              _mesaClicada.observacoes = texto;
              _iniciarCategoriasComPedidoNovo();
              Navigator.pop(context);
            } else {
              Navigator.pop(context);
              util.exibirMensagem(context, "Digite uma observação");
            }
          },
        )
      ],
    );

    showDialog(context: context, builder: (context) => _alert);
  }

  void _irParaCategorias(Mesa mesa) async {
    _exibirScreenCarregando(true);
    try {
      _mesaClicada = await _provider.recuperar<Mesa>("Mesas", mesa.codigo.toString());
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
          if (_provider.configWeb.requerObservacaoAoAbrirPedidoInterno) {
            _solicitarObservacao(context);
          } else {
            _iniciarCategoriasComPedidoNovo();
            return;
          }
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
      var retorno = await _provider.inserir<Mesa>(controller: "CriarPedidoInterno", objeto: _mesaClicada);
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
      titulo: _textoTitulo,
      exibirRefresh: true,
      onRefresh: _carregarMesas,
      body: _criarTela(),
    );
  }

  Widget _criarTela() {
    if (_erro) {
      return util.obterWidgetErroParaTela(context, _msgErro, refresh: (){
        _carregarMesas();
      });
    }
    if (_exibirCarregando) {
      return CircularProgressPadrao();
    }

    double width = MediaQuery.of(context).size.width;

    return GridView.builder(
      padding: const EdgeInsets.all(10),
      itemCount: _mesas.length,
      itemBuilder: (ctx, i) {
        return BotaoMesa(_mesas[i], _irParaCategorias);
      },
      gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: width ~/ 100,
        crossAxisSpacing: 20,
        mainAxisSpacing: 20,
        childAspectRatio: 1,
      ),
    );
  }
}
