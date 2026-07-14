import 'package:http/http.dart' as http;
import 'package:path_provider/path_provider.dart' as path;
import 'package:package_info/package_info.dart';
import '../models/entidades_dto.dart';
import 'dart:async';
import 'dart:io';
import 'dart:convert';
import '../util/gestor_configuracoes.dart';
import '../util/util.dart';
import '../util/constantes.dart';

class ServicosAPPProvider {
  //propriedades publicas
  ConfiguracoesProWaiterWeb configWeb;
  ConfiguracoesAPP configAPP;

  //Atributos privados
  String _login = "";
  String _senha = "";
  int codMesaAtual;
  PedidoInterno pedidoInternoAtual;
  double descontoAtual = 0;
  double acrescimoAtual = 0;

  //Metodos privados
  String _enderecoAPIatr;
  Future<String> _obterEnderecoBaseAPI() async {
    if (_enderecoAPIatr.isNullOrWhiteSpace()) {
      _enderecoAPIatr = "http://${configAPP.ipServidor}/ProWaiter/api/";
    }
    return _enderecoAPIatr;
  }

  //Metodos publicos
  void atualizarEstadoPedido(Mesa mesaAtual) {
    this.pedidoInternoAtual = mesaAtual.ultimoPedido;
    this.codMesaAtual = mesaAtual.codigo;
  }

  //Deleta um apk baixado em atualizações antigas
  Future deletarAPKAntigo() async {
    Directory diretorio = await path.getExternalStorageDirectory();
    String caminhoArquivo = "${diretorio.path}/${Constantes.nomeAPK}";
    File arquivo = File(caminhoArquivo);
    if (arquivo.existsSync()) {
      arquivo.deleteSync();
    }
  }

  Future<bool> appPrecisaAtualizar() async {
    //Como sempre rodamos ao iniciar o app, já carregamos as configurações aqui, para uso posterior
    GestorConfiguracoes gConfig = GestorConfiguracoes();
    configAPP = await gConfig.obterConfiguracoes(); //já carregamos as configs do APP

    //executamos o GET para pegar as informações do servidor para avaliar se existe nova versão
    String url = "http://${configAPP.ipServidor}/prowaiter/prowaiterAPK/versao.json";

    http.Response response = await http.get(
      Uri.parse(url),
      headers: {"X-Requested-With": "XMLHttpRequest"},
    );

    if (response.statusCode == 200) {
      var map = json.decode(response.body);
      String versaoServidorStr = map["VersionCode"];
      int versaoServidor = int.parse(versaoServidorStr);

      PackageInfo packageInfo = await PackageInfo.fromPlatform();
      int versaoLocal = int.parse(packageInfo.buildNumber);

      if (versaoLocal < versaoServidor) {
        return true;
      } else {
        return false;
      }
    } else {
      throw ("Erro ao tentar obter informações sobre no versão para atualização. StatusCode: ${response.statusCode}");
    }
  }

  Future<bool> autenticarUsuario({String login, String senha}) async {
    _login = login.trim();
    _senha = senha.trim();
    bool autenticou = false;

    http.Response response = await _executarGet(controller: "ValidarUsuario");

    if (response.statusCode == 200) {
      autenticou = response.body.toLowerCase() == "true";

      if (autenticou) {
        var gConfig = GestorConfiguracoes();
        await gConfig.salvarLoginSenha(login, senha);

        //Carregamos as configs da web
        response = await _executarGet(controller: "Configuracoes");
        configWeb = ConfiguracoesProWaiterWeb.fromJson(json.decode(response.body));
      } else {
        _login = "";
        _senha = "";
      }
    } else {
      throw ("Erro ao tentar autenticar o usuário: status code: ${response.statusCode}");
    }

    return autenticou;
  }

  Future<bool> tentarAutenticacaoAutomatica() async {
    var gConfig = GestorConfiguracoes();
    ConfiguracoesAPP config = await gConfig.obterConfiguracoes();

    if (config.login.isNullOrWhiteSpace()) {
      return false;
    }

    if (config.dataHoraLogin == null) {
      return false;
    }

    if (config.dataHoraLogin.add(Duration(hours: 4)).isBefore(DateTime.now())) {
      return false;
    }

    bool autenticou = await autenticarUsuario(login: config.login, senha: config.senha);
    return autenticou;
  }

  Future logOff() async {
    _login = "";
    _senha = "";
    var gConfig = GestorConfiguracoes();
    await gConfig.removerLoginSenha();
  }

  Future<http.Response> _executarGet({String controller, String queryStringCodigo = ""}) async {
    String token = "Basic " + base64Encode(utf8.encode('$_login:$_senha'));
    String url = await _obterEnderecoBaseAPI();

    url += "$controller/";
    if (!queryStringCodigo.isNullOrWhiteSpace()) {
      url += (queryStringCodigo.contains("=") ? "?" : "") + queryStringCodigo;
    }

    http.Response response = await http.get(Uri.parse(url), headers: {HttpHeaders.authorizationHeader: token, "X-Requested-With": "XMLHttpRequest"});
    return response;
  }

  Future<http.Response> _executarPost({String controller, Map<String, dynamic> dados}) async {
    String token = "Basic " + base64Encode(utf8.encode('$_login:$_senha'));
    String url = await _obterEnderecoBaseAPI();

    url += "$controller/";
    //print(url);

    http.Response response = await http.post(
      Uri.parse(url),
      body: json.encode(dados),
      encoding: utf8,
      headers: {
        HttpHeaders.authorizationHeader: token,
        "X-Requested-With": "XMLHttpRequest",
        "Content-Type": "application/json",
      },
    );
    return response;
  }

  Future<http.Response> _executarDelete({String controller, String chave}) async {
    if (chave.isNullOrWhiteSpace()) throw ("Erro ao tentar excluir objeto com chave $chave na api $controller");

    String token = "Basic " + base64Encode(utf8.encode('$_login:$_senha'));
    String url = await _obterEnderecoBaseAPI();

    url += "$controller/";
    url += (chave.contains("=") ? "?" : "") + chave;
    //print(url);

    http.Response response = await http.delete(
      Uri.parse(url),
      headers: {
        HttpHeaders.authorizationHeader: token,
        "X-Requested-With": "XMLHttpRequest",
        "Content-Type": "application/json",
      },
    );
    return response;
  }

  Future<bool> recuperarBool(String controller) async {
    return recuperar<bool>(controller, null);
  }

  Future<T> recuperar<T>(String controller, String queryStringCodigo) async {
    http.Response response = await _executarGet(controller: controller, queryStringCodigo: queryStringCodigo);

    if (response.statusCode == 200) {
      print(response.body);

      if (T == bool) {
        bool retorno = response.body.toLowerCase() == "true";
        return retorno as T;
      } else {
        var map = json.decode(response.body);
        T entidade = instanciarObjeto<T>(map);
        return entidade;
      }
    } else {
      throw ("Erro ao tentar obter dados da rede: status code: ${response.statusCode}");
    }
  }

  Future<List<T>> obterEntidades<T>(String controller, {String queryStringCodigo}) async {
    http.Response response = await _executarGet(controller: controller, queryStringCodigo: queryStringCodigo);
    print(response.body);
    if (response.statusCode == 200) {
      var map = json.decode(response.body);

      List<T> lista = [];
      map.forEach((element) {
        //print(element);
        lista.add(instanciarObjeto<T>(element));
      });
      return lista;
    } else {
      throw ("Erro ao tentar obter dados da rede: status code: ${response.statusCode}");
    }
  }

  Future<T> inserir<T>({String controller, T objeto}) async {
    http.Response response = await _executarPost(controller: controller, dados: obterJson(objeto));
    if (response.statusCode == 200) {
      T entidade = instanciarObjeto<T>(json.decode(response.body));
      return entidade;
    } else {
      throw ("Erro ao tentar inserir dados no backend: status code: ${response.statusCode}");
    }
  }

  Future<bool> excluir({String controller, String chave}) async {
    http.Response response = await _executarDelete(controller: controller, chave: chave);

    if (response.statusCode == 200) {
      return true;
    } else {
      throw ("Erro ao tentar executar o delete no controller $controller com a chave $chave");
    }
  }

  Future excluirPedidoVazio() async {
    if (codMesaAtual == null) {
      throw ("Pedido vazio não foi excluido pois o codigo da mesa atual está nulo");
    }

    try {
      //mesmo que der erro abaixo zeramos os dados de acrescimos e descontos do contexto
      descontoAtual = 0;
      acrescimoAtual = 0;
      Mesa mesa = await recuperar<Mesa>("Mesas", codMesaAtual.toString());
      if (mesa == null) throw ("Erro ao tentar recuperar os dados da mesa atual (excluirPedidoVazio())");
      codMesaAtual = null;
      pedidoInternoAtual = null;

      PedidoInterno pedido = mesa.ultimoPedido;
      //peidito tem algum item adicionado?
      if (pedido != null) {
        if ((pedido.refeicoesDoPedido == null || pedido.refeicoesDoPedido.length == 0) &&
            (pedido.bebidasDoPedido == null || pedido.bebidasDoPedido.length == 0)) {
          await excluir(controller: "PedidosInternos", chave: pedido.codigo.toString());
        }
      }
    } catch (e, _) {
      rethrow;
    }
  }

  T instanciarObjeto<T>(Map<String, dynamic> element) {
    if (T == Mesa) {
      return Mesa.fromJson(element) as T;
    } else if (T == TipoRefeicao) {
      return TipoRefeicao.fromJson(element) as T;
    } else if (T == TipoBebida) {
      return TipoBebida.fromJson(element) as T;
    } else if (T == LocalInterno) {
      return LocalInterno.fromJson(element) as T;
    } else if (T == ComponenteRefeicao) {
      return ComponenteRefeicao.fromJson(element) as T;
    } else if (T == Bebida) {
      return Bebida.fromJson(element) as T;
    } else if (T == BebidaDoPedido) {
      return BebidaDoPedido.fromJson(element) as T;
    } else if (T == TamanhoRefeicao) {
      return TamanhoRefeicao.fromJson(element) as T;
    } else if (T == Refeicao) {
      return Refeicao.fromJson(element) as T;
    } else if (T == RefeicaoDoCardapio) {
      return RefeicaoDoCardapio.fromJson(element) as T;
    } else if (T == RefeicaoDoPedido) {
      return RefeicaoDoPedido.fromJson(element) as T;
    } else if (T == PedidoInterno) {
      return PedidoInterno.fromJson(element) as T;
    } else if (T == ComponenteRefeicaoPedido) {
      return ComponenteRefeicaoPedido.fromJson(element) as T;
    } else if (T == ComponenteComposicaoRefeicaoCardapio) {
      return ComponenteComposicaoRefeicaoCardapio.fromJson(element) as T;
    } else if (T == UnidadeComponenteComposicao) {
      return UnidadeComponenteComposicao.fromJson(element) as T;
    } else if (T == ItensNaoEnviados) {
      return ItensNaoEnviados.fromJson(element) as T;
    } else if (T == ModeloComponenteRefeicaoPedido) {
      return ModeloComponenteRefeicaoPedido.fromJson(element) as T;
    } else if (T == ModeloRefeicaoPedido) {
      return ModeloRefeicaoPedido.fromJson(element) as T;
    } else if (T == ModeloBebidaPedido) {
      return ModeloBebidaPedido.fromJson(element) as T;
    } else if (T == ModeloPedido) {
      return ModeloPedido.fromJson(element) as T;
    }
    throw ("instanciarObjeto<T> tipo ${T.toString()} desconhecido");
  }

  Map<String, dynamic> obterJson<T>(T element) {
    if (T == Mesa) {
      return (element as Mesa).toJson();
    } else if (T == TipoRefeicao) {
      return (element as TipoRefeicao).toJson();
    } else if (T == TipoBebida) {
      return (element as TipoBebida).toJson();
    } else if (T == LocalInterno) {
      return (element as LocalInterno).toJson();
    } else if (T == ComponenteRefeicao) {
      return (element as ComponenteRefeicao).toJson();
    } else if (T == Bebida) {
      return (element as Bebida).toJson();
    } else if (T == BebidaDoPedido) {
      return (element as BebidaDoPedido).toJson();
    } else if (T == TamanhoRefeicao) {
      return (element as TamanhoRefeicao).toJson();
    } else if (T == Refeicao) {
      return (element as Refeicao).toJson();
    } else if (T == RefeicaoDoCardapio) {
      return (element as RefeicaoDoCardapio).toJson();
    } else if (T == RefeicaoDoPedido) {
      return (element as RefeicaoDoPedido).toJson();
    } else if (T == PedidoInterno) {
      return (element as PedidoInterno).toJson();
    } else if (T == ComponenteRefeicaoPedido) {
      return (element as ComponenteRefeicaoPedido).toJson();
    } else if (T == ComponenteComposicaoRefeicaoCardapio) {
      return (element as ComponenteComposicaoRefeicaoCardapio).toJson();
    } else if (T == UnidadeComponenteComposicao) {
      return (element as UnidadeComponenteComposicao).toJson();
    } else if (T == ItensNaoEnviados) {
      return (element as ItensNaoEnviados).toJson();
    } else if (T == ModeloComponenteRefeicaoPedido) {
      return (element as ModeloComponenteRefeicaoPedido).toJson();
    } else if (T == ModeloRefeicaoPedido) {
      return (element as ModeloRefeicaoPedido).toJson();
    } else if (T == ModeloBebidaPedido) {
      return (element as ModeloBebidaPedido).toJson();
    } else if (T == ModeloPedido) {
      return (element as ModeloPedido).toJson();
    }

    throw ("obterJson<T> tipo  ${T.toString()} desconhecido");
  }
}
