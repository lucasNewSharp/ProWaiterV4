import 'package:back_button_interceptor/back_button_interceptor.dart';
import 'package:flutter/material.dart';
import 'package:prowaiter_v2/components/scaffold_padrao.dart';
import '../models/entidades_dto.dart';
import 'package:prowaiter_v2/util/app_routes.dart';
import 'package:prowaiter_v2/util/gestor_configuracoes.dart';

class ConfiguracoesScreen extends StatefulWidget {
  @override
  _ConfiguracoesScreenState createState() => _ConfiguracoesScreenState();
}

class _ConfiguracoesScreenState extends State<ConfiguracoesScreen> {
  TextEditingController _ipServidorController = new TextEditingController();

  @override
  void initState() {
    super.initState();
    BackButtonInterceptor.add(_backButton);
  }
   @override
  void dispose(){
    super.dispose();
    BackButtonInterceptor.remove(_backButton);
  }

  bool _backButton(bool stopDefaultButtonEvent, RouteInfo routeInfo) {
    _voltarTelaLogin();
    return true;
  }

  void _submitForm() {
    var gConfig = GestorConfiguracoes();
    gConfig
        .salvarIpServidor(_ipServidorController.text.trim())
        .then((value) {
      _voltarTelaLogin();
    });
  }

  void _voltarTelaLogin(){
    Navigator.of(context).pushReplacementNamed(AppRouts.LoginRoute);
  }

  @override
  Widget build(BuildContext context) {
    ConfiguracoesAPP config = ModalRoute.of(context).settings.arguments;
    if (config != null) {
      setState(() {
        _ipServidorController.text = config.ipServidor;
      });
    }

    return ScaffoldPadrao(
      titulo: "Configurações",
      exibirBackButton: true,
      onBackPress: _voltarTelaLogin,
      body: Card(
        elevation: 5,
        child: Padding(
          padding: EdgeInsets.all(10),
          child: Column(
            children: <Widget>[
              TextField(
                controller: _ipServidorController,
                decoration: InputDecoration(labelText: "IP servidor"),
                onSubmitted: (_) => _submitForm(),
              ),              
              Padding(
                padding: const EdgeInsets.all(8.0),
                child: RaisedButton(
                  child: Text("Salvar"),
                  color: Theme.of(context).primaryColor,
                  textColor: Theme.of(context).textTheme.button.color,
                  onPressed: _submitForm,
                ),
              )
            ],
          ),
        ),
      ),
    );
  }
}
