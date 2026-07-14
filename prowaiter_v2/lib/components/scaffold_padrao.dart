import 'package:flutter/material.dart';
import 'package:prowaiter_v2/util/servicos_app_provider.dart';
import 'package:provider/provider.dart';
import '../util/app_routes.dart';
import '../util/util.dart' as util;
import 'bottom_app_bar_padrao.dart';

class ScaffoldPadrao extends StatelessWidget {
  final String titulo;
  final Widget body;
  final Function onRefresh;
  final bool exibirRefresh;
  final Widget conteudoBottomAppBar;
  final bool exibirBackButton;
  final Function onBackPress;

  ScaffoldPadrao(
      {this.titulo,
      this.body,
      this.onRefresh,
      this.conteudoBottomAppBar,
      this.onBackPress,
      this.exibirBackButton = false,
      this.exibirRefresh = false});

  @override
  Widget build(BuildContext context) {
    void _loggOf() {
      var provider = Provider.of<ServicosAPPProvider>(context, listen: false);
      provider.logOff().then((_) {
        Navigator.of(context).pushReplacementNamed(AppRouts.LoginRoute);
      });
    }

    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        leading: !exibirBackButton
            ? null
            : new IconButton(
                icon: new Icon(Icons.arrow_back),
                onPressed: onBackPress,
              ),
        title: Text(titulo),
        actions: <Widget>[
          if (exibirRefresh)
            IconButton(
              icon: Icon(Icons.refresh),
              onPressed: onRefresh,
            ),
          PopupMenuButton(
            onSelected: (int valor) {
              if (valor == 0) {
                _loggOf();
              } else if (valor == 1) {
                util.exibirSobre(context);
              }
            },
            itemBuilder: (_) => [
              PopupMenuItem(
                child: Text(
                  "Logout",
                ),
                value: 0,
              ),
              PopupMenuItem(
                child: Text(
                  "Sobre",
                ),
                value: 1,
              )
            ],
          ),
        ],
      ),
      body: body,
      //drawer: DrawerPadrao(),
      bottomNavigationBar: conteudoBottomAppBar != null
          ? BottomAppBarPadrao(
              child: conteudoBottomAppBar,
            )
          : null,
    );
  }
}
