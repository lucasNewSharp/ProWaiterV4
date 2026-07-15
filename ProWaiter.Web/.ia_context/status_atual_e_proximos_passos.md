# Status Atual da Migração

## O que foi realizado hoje:
- **Atualização da Arquitetura Base:** Conversão do projeto legado em .NET Framework 4.8 para o padrão moderno SDK-style (.NET 10.0).
- **Substituição do Servidor:** Preparação estrutural para remover a dependência do IIS em favor do Kestrel.
- **Isolamento de Dependências:** Remoção completa das antigas DLLs fechadas `NewSharp.Ferramentas` e `NewSharp.BancoDeDados`. Toda a lógica de acesso a dados foi internalizada e atualizada.
- **Migração de ORM (EF6 para EF Core):** 
  - Conversão massiva de todos os arquivos de configuração para implementar `IEntityTypeConfiguration<T>`.
  - Substituição das antigas rotinas de transação customizadas por `db.Database.BeginTransaction()` nativo.
  - Remoção global do namespace `System.Data.Entity`.
- **Limpeza e Modernização de Controladores e APIs:**
  - Substituição de heranças obsoletas como `ApiController` pelo `ControllerBase`.
  - Correção na captura de parâmetros HTTP (`Request.Form` lidando corretamente com `StringValues`).
  - Atualização do sistema de autenticação para as diretrizes do ASP.NET Core Identity (`SignInManager`, `SignInResult`).
  - Isolamento de componentes de baixo nível/desktop que não são suportados em .NET Core (como o legado `GestorImpressoes`).
- **Resultado Final do Dia:** O código C# do backend foi totalmente validado e compila com **0 erros** no .NET 10.

## Atualizações Recentes (15/07/2026):
- **Nova Arquitetura de Impressão (Sockets/TCP):** A dependência legado do `NewSharp` foi completamente substituída por uma arquitetura nativa. Os comandos ESC/POS são enviados agora via Socket direto do Kestrel em .NET 10. A opção de desenvolvedor `ImpressoraArquivoTexto` foi mantida.
- **Correção da Camada de Banco de Dados:** A configuração de herança do EF Core (TPT) foi ajustada removendo `.HasKey` duplicado nas classes filhas do `Pedido`. Além disso, foi implementado um *fallback* inteligente no `OnConfiguring` do Contexto para ler do `appsettings.json`, dispensando refatorar todos os `new ProWaiterContext()` do código legado.
- **Resgate Visual (CSS/JS):** Os caminhos de empacotamento extintos do ASP.NET Framework (`~/css/css.css`) foram mapeados diretamente aos arquivos reais no `wwwroot/Content` e `wwwroot/Scripts`, consertando a quebra visual completa que estava impedindo o render do Bootstrap e jQuery.
- **Refatoração da Autenticação:** A parte de segurança (`AccountController`) legada travava por usar OWIN. O componente foi integralmente recriado usando injeção asíncrona de `SignInManager<ApplicationUser>` e `UserManager<ApplicationUser>`, restabelecendo o formulário de Login e proteção `[Authorize]`.

## O que falta fazer (Próximos Passos):

1. **Revisar e Limpar Avisos de Renderização Restantes:**
   - O compilador acusa avisos (`MVC1000`) quanto ao uso de `@Html.Partial()` no Razor, alertando risco de *deadlocks*. Devemos refatorar esses pontos para `<partial name="..." />` nas Views de Pedidos e Cardápio para assegurar a máxima estabilidade sob tráfego.

2. **Testes Extensivos da Regra de Negócios:**
   - Uma vez autenticado e com as Views visíveis, é vital validar o fluxo core: a criação de um pedido, transição pelas tabelas filhas (Externo, Interno, etc), o envio para a cozinha, e certificar que as regras antigas fluem como esperado na engine nova do EF Core.

3. **Verificação de Integração Física:**
   - Testar o envio de um pedido e verificar a saída para a "impressora de texto" ou para um IP de impressora na rede local para assegurar que os bytes ESC/POS gerados via Sockets pela nova biblioteca não sofrem corrupção.
