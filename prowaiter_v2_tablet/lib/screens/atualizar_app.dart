import 'package:flutter/material.dart';
import 'package:flutter_downloader/flutter_downloader.dart';
import 'package:open_file/open_file.dart';
import 'package:prowaiter_v2/components/scaffold_padrao.dart';
import 'package:path_provider/path_provider.dart' as path;
import 'package:prowaiter_v2/util/gestor_configuracoes.dart';
import 'package:prowaiter_v2/util/constantes.dart';
import 'package:prowaiter_v2/util/util.dart' as util;
import 'dart:isolate';
import 'dart:ui';

//Teste misto
//abrindo com o OpenFile caso não funcione tentar com o FlutterDownloader
//Android 5.0   : OK
//Android 5.1.1 : OK
//Android 6.0   : OK
//Android 7.0   : OK
//Android 7.1.1 : OK
//Android 8.0   : OK
//Android 8.1   : OK
//Android 9.0   : OK
//Android 10.0  : OK

//OBS:
//Android 5.1.1 : Faz download mas não abre (abre com o FlutterDownloader.open(taskId: id);)
//Android 6.0   : Faz download mas não abre (abre com o FlutterDownloader.open(taskId: id);)
//Abrindo com o downloader:
//Android 5.1   :Não mostra progresso, mas faz o download, não abre a instalação
//Android 6.0   :OK
//Android 7.0   :Dwonload OK, não abre a instalação
//Android 7.1.1 :Dwonload OK, não abre a instalação
//Android 8.0   :Dwonload OK, não abre a instalação
//Android 8.1   :Dwonload OK, não abre a instalação
//Android 9.0   :Dwonload OK, não abre a instalação
//Android 10.0  :Dwonload OK, não abre a instalação

class AtualizarApp extends StatefulWidget {
  @override
  _AtualizarAppState createState() => _AtualizarAppState();
}

class _AtualizarAppState extends State<AtualizarApp> {
  ReceivePort _port = ReceivePort();
  double _progresso = 0;
  static final String _nomePorta = "pw_downloader_send_port";

  @override
  void initState() {
    super.initState();

    //Não posso chamar as configs do Provedor de serviços pois ainda não carreguei
    //as configs são carregadas ao efetuar login, por isso criamos um novo gestor de configurações e pegamos direamente do arquivo.
    GestorConfiguracoes gConfig = new GestorConfiguracoes();

    WidgetsFlutterBinding.ensureInitialized();
    FlutterDownloader.initialize(debug: true).then((value) {
      IsolateNameServer.registerPortWithName(_port.sendPort, _nomePorta);

      _port.listen((dynamic data) {
        String id = data[0];
        DownloadTaskStatus status = data[1];
        int progress = data[2];
        setState(() {
          _progresso = progress / 100;
        });
        if (status == DownloadTaskStatus.complete)
          setState(() {
            path.getExternalStorageDirectory().then((diretorio) {
              //Tentamos abrir pelo nome do arquivo
              OpenFile.open("${diretorio.path}/${Constantes.nomeAPK}").then((value) {
                if (value.type != ResultType.done) {
                  //Caso não abriu, tentamos abrir pelo taskID
                  FlutterDownloader.open(taskId: id).then((abriu) {
                    if (!abriu) {
                      util.exibirMensagem(context,
                          "Não foi possível efetuar a atualização devido a falta de permissão no seu dispositivo. Faça o download manualmente para atualizar");
                    }
                  });
                }
              });
            });
          });
      });

      FlutterDownloader.registerCallback(_downloadCallback);

      path.getExternalStorageDirectory().then((diretorio) {
        gConfig.obterConfiguracoes().then((configs) {
          try {
            var _ = FlutterDownloader.enqueue(
              url: "http://${configs.ipServidor}/ProWaiter/ProWaiterAPK/${Constantes.nomeAPK}",
              savedDir: "${diretorio.path}/",
              fileName: Constantes.nomeAPK,
              showNotification: true, // show download progress in status bar (for Android)
              openFileFromNotification: true, // click on notification to open downloaded file (for Android)
            );
          } catch (e, s) {
            util.exibirMensagem(context, e.toString() + s.toString());
          }
        });
      });
    });
  }

  @override
  void dispose() {
    IsolateNameServer.removePortNameMapping(_nomePorta);
    super.dispose();
  }

  static void _downloadCallback(String id, DownloadTaskStatus status, int progress) {
    final SendPort send = IsolateNameServer.lookupPortByName(_nomePorta);
    send.send([id, status, progress]);
  }

  @override
  Widget build(BuildContext context) {
    return ScaffoldPadrao(
      titulo: "Atualizando o Prowaiter",
      body: Center(
          child: Container(
        padding: EdgeInsets.all(16.0),
        child: _progresso < 100
            ? Container(
                height: 300,
                child: Column(
                  children: <Widget>[
                    Padding(
                      padding: const EdgeInsets.all(8.0),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: <Widget>[
                          Text(
                            "Atualizando o ProWaiter",
                            style: TextStyle(
                              fontSize: 24,
                              color: Theme.of(context).textTheme.headline6.color,
                            ),
                          ),
                        ],
                      ),
                    ),
                    Expanded(
                      child: Padding(
                        padding: const EdgeInsets.all(8.0),
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          crossAxisAlignment: CrossAxisAlignment.center,
                          mainAxisSize: MainAxisSize.max,
                          children: <Widget>[
                            LinearProgressIndicator(
                              value: _progresso,
                            ),
                            Text(
                              '${(_progresso * 100).round()}%',
                              style: TextStyle(
                                fontSize: 20,
                              ),
                            ),
                            Text("Realizando o download"),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
              )
            : Text("Download finalizado"),
      )),
    );
  }
}
