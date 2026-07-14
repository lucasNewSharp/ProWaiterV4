import 'dart:io';
import 'dart:async';
import 'dart:convert' as convert;
import 'package:path_provider/path_provider.dart';
import '../models/entidades_dto.dart';

class GestorConfiguracoes {
  static const _nomeArquivo = "configuracoes.json";

  Future<String> get _caminhoCompletoArquivo async {
    final _diretorio = await getApplicationDocumentsDirectory();
    final _caminhoArquivo = "${_diretorio.path}/$_nomeArquivo";
    return _caminhoArquivo;
  }

  Future<bool> arquivoConfigExiste() async {        
    final _caminhoArquivo = await _caminhoCompletoArquivo;    
    return File(_caminhoArquivo).exists();
  }

  Future salvarIpServidor(String ip) async {
    ConfiguracoesAPP config = await obterConfiguracoes();
    if (config == null) {
      config = ConfiguracoesAPP();
      config.login = "";
      config.senha = "";      
    }
    config.ipServidor = ip;
    gravarConfiguracoes(config);
  }

  Future salvarLoginSenha(String login, String senha) async{
    ConfiguracoesAPP config = await obterConfiguracoes();
    config.login = login;
    config.senha = senha;
    config.dataHoraLogin = DateTime.now();
    await gravarConfiguracoes(config);
  }

  Future removerLoginSenha() async{
    await salvarLoginSenha("", "");
  }

  Future<ConfiguracoesAPP> obterConfiguracoes() async {    
    bool existe = await arquivoConfigExiste();    
    if (existe) {
      final _caminhoArquivo = await _caminhoCompletoArquivo;
      File arquivo = new File(_caminhoArquivo);
      String dados = arquivo.readAsStringSync();
      var json = convert.json.decode(dados);
      ConfiguracoesAPP config = ConfiguracoesAPP.fromJson(json);
      return config;
    }
    return null;
  }

  Future gravarConfiguracoes(ConfiguracoesAPP config) async {
    if (config == null) {
      throw Exception("O objeto configuração está nulo");
    }

    Map<String, dynamic> dados = config.toJson();
    String dadosString = convert.json.encode(dados);

    String caminhoArquivo = await _caminhoCompletoArquivo;
    var arquivo = new File(caminhoArquivo);
    arquivo.writeAsStringSync(dadosString);
  }
}
