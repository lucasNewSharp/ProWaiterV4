import 'dart:io';
import 'package:back_button_interceptor/back_button_interceptor.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:prowaiter_v2/util/app_routes.dart';
import 'package:provider/provider.dart';
import '../components/circular_progress_padrao.dart';
import '../util/gestor_configuracoes.dart';
import '../util/servicos_app_provider.dart';
import '../util/app_routes.dart';
import 'package:toast/toast.dart' as toast;
import '../util/util.dart' as util;

class LoginScreen2 extends StatefulWidget {
  @override
  _LoginScreenState createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen2> {
  final GestorConfiguracoes _gConfig = GestorConfiguracoes();
  ServicosAPPProvider _provider;
  var _loginController = TextEditingController();
  var _senhaConhtroller = TextEditingController();
  bool _efetuandoLogin = false;
  bool _efetuandoAutoLogin = true;
  bool _erro;
  String _msgErro;
  DateTime _momentoCliqueSair;

  @override
  void initState() {
    super.initState();
    BackButtonInterceptor.add(_backButton);
    _provider = Provider.of<ServicosAPPProvider>(context, listen: false);
    _inicializar();
  }

  @override
  void dispose() {
    super.dispose();
    BackButtonInterceptor.remove(_backButton);
  }

  bool _backButton(bool stopDefaultButtonEvent, RouteInfo routeInfo) {
    DateTime agora = DateTime.now();
    if (_momentoCliqueSair != null && agora.difference(_momentoCliqueSair) > Duration(seconds: 2)) {
      _momentoCliqueSair = null;
    }

    if (_momentoCliqueSair == null) {
      _momentoCliqueSair = DateTime.now();
      Fluttertoast.showToast(msg: "Duplo clique para sair", backgroundColor: Colors.black, textColor: Colors.white);
      return true;
    }
    exit(0);
  }

  void _inicializar() async {
    _erro = false;
    _msgErro = "";
    try {
      //primeiro testamos se o arquivo de configuração existe, se não existe tem que setar o IP do servidor
      bool arqConfigExiste = await _gConfig.arquivoConfigExiste();
      if (!arqConfigExiste) {
        Navigator.of(context).pushReplacementNamed(AppRouts.ConfiguracoesRoute, arguments: null);
        return;
      }

      //Deletamos o APK antigo caso ele exista no sistema interno de arquivos do APP (app baixado para atualização)
      await _provider.deletarAPKAntigo();

      //após testamos se o aplicativo precisa de atualização
      //se precisa redirecionamos para o Widget de atualização
      bool precisaAtualizar = await _provider.appPrecisaAtualizar();
      if (precisaAtualizar) {
        Navigator.of(context).pushReplacementNamed(AppRouts.AtualizarAppRoute);
        return;
      }

      //tentamos realizar a autenticação automática
      //caso consiga, redirecionamos para a tela de mesas diretamente
      bool autenticou = await _provider.tentarAutenticacaoAutomatica();
      if (autenticou) {
        Navigator.of(context).pushReplacementNamed(AppRouts.MesasRoute);
        return;
      }

      //Setamos o flag de autoLogin para falso para redesenhar a interface e permitir que o usuário faça login
      setState(() {
        _efetuandoAutoLogin = false;
      });
    } catch (e, s) {
      _msgErro = e.toString() + s.toString();
      setState(() {
        _efetuandoAutoLogin = false;
        _erro = true;
      });
    }
  }

  void _submitForm() async {
    try {
      setState(() {
        _efetuandoLogin = true;
      });
      String login = _loginController.text;
      String senha = _senhaConhtroller.text;

      if (login == null || login.isEmpty || senha == null || senha.isEmpty) {
        toast.Toast.show("Digite o login e senha", context, duration: toast.Toast.LENGTH_LONG, gravity: toast.Toast.TOP);
        setState(() {
          _efetuandoLogin = false;
        });
        return;
      }

      //tentamos autenticar o usuario
      bool autenticou = await _provider.autenticarUsuario(login: login, senha: senha);
      if (autenticou) {
        Navigator.of(context).pushReplacementNamed(AppRouts.MesasRoute);
        return;
      } else {
        toast.Toast.show("Usuário e/ou senha inválidos", context, duration: toast.Toast.LENGTH_LONG, gravity: toast.Toast.TOP);
        setState(() {
          _efetuandoLogin = false;
        });
      }
    } catch (e, s) {
      _msgErro = e.toString() + s.toString();
      setState(() {
        _efetuandoLogin = false;
        _erro = true;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Login"),
      ),
      body: _criarTela(),
      drawer: Drawer(
        child: Column(
          children: <Widget>[
            AppBar(
              title: Text("Opções"),
              automaticallyImplyLeading: false,
            ),
            Divider(),
            ListTile(
              leading: Icon(Icons.settings),
              title: Text("Configurações"),
              onTap: () {
                _gConfig
                    .obterConfiguracoes()
                    .then((value) => {Navigator.of(context).pushReplacementNamed(AppRouts.ConfiguracoesRoute, arguments: value)});
              },
            ),
            Divider(),
            ListTile(
              leading: Icon(Icons.info),
              title: Text("Sobre"),
              onTap: () {
                util.exibirSobre(context);
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _criarTela() {
    if (_efetuandoAutoLogin || _efetuandoLogin) {
      return CircularProgressPadrao();
    }

    if (_erro) {
      return util.obterWidgetErroParaTela(context, _msgErro, refresh: () {
        _inicializar();
      });
    }

    return Stack(children: [
      Container(
        decoration: BoxDecoration(image: DecorationImage(image: AssetImage("assets/background_login.jpeg"), fit: BoxFit.cover)),
      ),
      SingleChildScrollView(
        child: Container(
          margin: const EdgeInsets.only(top: 250),
          child: Padding(
            padding: const EdgeInsets.all(8.0),
            child: Padding(
              padding: EdgeInsets.all(10),
              child: Padding(
                padding: const EdgeInsets.all(6.0),
                child: Column(
                  children: <Widget>[
                    Center(
                      child: Container(
                        width: 280,
                        child: TextField(
                          controller: _loginController,
                          textCapitalization: TextCapitalization.none,
                          style: TextStyle(color: Colors.white),
                          decoration: InputDecoration(
                            labelText: "LOGIN",
                            labelStyle: TextStyle(color: Colors.white, fontWeight: FontWeight.w500),
                            enabledBorder: UnderlineInputBorder(borderSide: BorderSide(color: Colors.white)),
                            focusedBorder: UnderlineInputBorder(borderSide: BorderSide(color: Colors.white)),
                          ),
                        ),
                      ),
                    ),
                    Container(
                      width: 280,
                      child: TextField(
                        controller: _senhaConhtroller,
                        textCapitalization: TextCapitalization.none,
                        style: TextStyle(color: Colors.white),
                        decoration: InputDecoration(
                          labelText: "SENHA",
                          labelStyle: TextStyle(color: Colors.white, fontWeight: FontWeight.w500),
                          enabledBorder: UnderlineInputBorder(borderSide: BorderSide(color: Colors.white)),
                          focusedBorder: UnderlineInputBorder(borderSide: BorderSide(color: Colors.white)),
                        ),
                        obscureText: true,
                      ),
                    ),
                    Container(
                      margin: EdgeInsets.only(top: 25),
                      child: Padding(
                        padding: const EdgeInsets.all(10.0),
                        child: ButtonTheme(
                          minWidth: 280,
                          height: 45,
                          child: RaisedButton(
                            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(18.0), side: BorderSide(color: Colors.white)),
                            child: Text("LOGIN"),
                            color: Colors.white,
                            textColor: Color(0xFFfe4c57),
                            onPressed: _submitForm,
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
    ]);
  }
}
