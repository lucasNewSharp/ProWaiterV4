import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:prowaiter_v2/screens/atualizar_app.dart';
import 'package:prowaiter_v2/screens/bebidas_screen.dart';
import 'package:prowaiter_v2/screens/componentes_refeicao_screen.dart';
import 'package:prowaiter_v2/screens/configuracoes_screen.dart';
import 'package:prowaiter_v2/screens/detalhes_pedido_screen.dart';
import 'package:prowaiter_v2/screens/login_screen2.dart';
import 'package:prowaiter_v2/screens/mesas_screen.dart';
import 'package:prowaiter_v2/screens/categorias_screen.dart';
import 'package:prowaiter_v2/screens/refeicoes_cardapio_screen.dart';
import 'package:prowaiter_v2/screens/modelos_screen.dart';
import 'package:prowaiter_v2/util/app_routes.dart';
import 'package:prowaiter_v2/util/servicos_app_provider.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        Provider<ServicosAPPProvider>(
          create: (_) => ServicosAPPProvider(),
        ),
      ],
      child: MaterialApp(
        title: 'ProWaiter',
        theme: ThemeData(
          primarySwatch: Colors.red,
          visualDensity: VisualDensity.adaptivePlatformDensity,
          textTheme: Theme.of(context).textTheme.copyWith(button: TextStyle(color: Colors.white)),
        ),
        home: LoginScreen2(),
        routes: {
          AppRouts.ConfiguracoesRoute: (ctx) => ConfiguracoesScreen(),
          AppRouts.LoginRoute: (ctx) => LoginScreen2(),
          AppRouts.MesasRoute: (ctx) => MesasScreen(),
          AppRouts.CategoriasRoute: (ctx) => CategoriasScreen(),
          AppRouts.RefeicoesCardapioRoute: (ctx) => RefeicoesCardapioScreen(),
          AppRouts.AtualizarAppRoute: (ctx) => AtualizarApp(),
          AppRouts.BebidasRoute: (ctx) => BebidasScreen(),
          AppRouts.DetalhesPedidoRoute: (ctx) => DetalhesPedidoScreen(),
          AppRouts.ComponentesRefeicaoRoute: (ctx) => ComponentesRefeicaoScreen(),
          AppRouts.ModelosRoute: (ctx) => ModelosScreen()
        },
      ),
    );
  }
}
