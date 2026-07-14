# Análise de Migração do Projeto ProWaiter.Web

## 1. Visão Geral do Projeto Legado
O projeto original **ProWaiter.Web** era uma aplicação web monolítica baseada no .NET Framework 4.8. 
A arquitetura dependia de:
- **Hospedagem:** IIS (Internet Information Services).
- **Framework Web:** ASP.NET MVC 5 (utilizando `System.Web.Mvc`).
- **Acesso a Dados:** Entity Framework 6 (`System.Data.Entity`), encapsulado parcialmente em bibliotecas customizadas ("NewSharp").
- **Bibliotecas Externas:** Dependência de duas DLLs legadas externas (`NewSharp.Ferramentas` e `NewSharp.BancoDeDados`), que traziam funcionalidades customizadas de repositório e manipulação de utilitários.

## 2. Objetivos da Migração
A missão principal foi modernizar a stack tecnológica para garantir melhor performance, suporte multiplataforma e segurança, através dos seguintes objetivos:
1. **Migrar de .NET 4.8 para .NET 10.0 (SDK-style project).**
2. **Substituir o IIS pelo Kestrel** como servidor web de alta performance.
3. **Remover a dependência das DLLs legadas da NewSharp**, internalizando e modernizando a lógica para dentro do próprio projeto.
4. **Substituir o Entity Framework 6 pelo Entity Framework Core**, eliminando todas as referências ao `System.Data.Entity`.
5. **Atualizar os Controllers** de `ApiController` e `Controller` legado para o padrão unificado `ControllerBase` e `Controller` do ASP.NET Core.

## 3. Plano de Implementação Executado

### Fase 1: Atualização da Estrutura do Projeto
- O arquivo `ProWaiter.Web.csproj` foi completamente reescrito para utilizar o formato `<Project Sdk="Microsoft.NET.Sdk.Web">`.
- O `TargetFramework` foi atualizado para `net10.0`.
- Foram removidas todas as referências diretas às DLLs da `NewSharp`.
- Criação dos arquivos estruturais do ASP.NET Core: `Program.cs` e `appsettings.json`, configurando o pipeline do Kestrel e a injeção de dependência.

### Fase 2: Migração do Acesso a Dados (EF6 para EF Core)
- **ProWaiterContext:** O contexto do banco de dados foi atualizado para herdar de `DbContext` do `Microsoft.EntityFrameworkCore`. A string de conexão foi movida para o `appsettings.json` e o contexto foi registrado no container de DI em `Program.cs`.
- **Remoção de Código Legado:** As classes como `ContextoBDProvider` e `MapeadorDeEntidadeNewSharp`, provindas da DLL legada, foram removidas e a lógica de mapeamento foi convertida para usar `IEntityTypeConfiguration<T>`.
- **Substituição de Namespaces:** Remoção global de `using System.Data.Entity` em favor de `using Microsoft.EntityFrameworkCore`.
- **Identity:** O controle de usuários foi migrado de `Microsoft.AspNet.Identity.EntityFramework` para `Microsoft.AspNetCore.Identity.EntityFrameworkCore`.

### Fase 3: Modernização de Controllers e Web API
- A classe base `ApiController` foi substituída por `ControllerBase`.
- Os retornos de métodos de API (como `IHttpActionResult` e métodos customizados do projeto legado) foram adaptados para retornar `IActionResult` padrão do .NET Core (ex: `Ok()`, `NotFound()`, `BadRequest()`).
- O sistema de rotas foi atualizado para utilizar o roteamento por atributos do ASP.NET Core (`[Route("api/[controller]")]`).
- As validações de `HttpCookie` e estados de requisição baseados em `System.Web` foram identificadas para refatoração.

### Fase 4: Resolução de Conflitos e Compilação
- Desativação temporária da compilação das Views (`.cshtml`) na verificação do backend, visto que as views legadas dependem de TagHelpers e HtmlHelpers incompatíveis (ex: `IHtmlHelper.BeginForm`). A refatoração das views ocorrerá em uma etapa subsequente focada no frontend.
- Correção massiva de sintaxes obsoletas em todos os Controllers e APis.
- Alteração do formato das transações em banco de dados (`.BeginTransaction`, `.Commit`, `.Rollback`).
- Substituição de tipos de request (como `Request.Form` retornando `StringValues`).
- Configurações do Identity Core ajustadas (`SignInManager`, `ApplicationUser`).
- **Resultado:** Atingimos **0 Erros de Compilação no Backend!**

## 4. Próximos Passos (Plano Contínuo)
1. **Refatoração do Frontend (Views Razor):** Reabilitar a compilação das Views no `.csproj`, analisar e adaptar os arquivos `.cshtml` para utilizarem ASP.NET Core TagHelpers, injetar dependências nas views (se necessário) e reconstruir componentes que dependiam fortemente de `System.Web.Mvc`.
2. **Validação e Testes:** Testar os pipelines de injeção de dependência, rotas de APIs e certificar-se de que a conexão real ao banco de dados com o EF Core (incluindo as relações complexas) está funcionando sem bugs no runtime.
3. **Reimplementação de Recursos de Hardware:** Como o antigo `GestorImpressoes` dependia de arquitetura WinForms/WPF, arquitetar uma nova abordagem (possivelmente uma API para agentes locais de impressão) para lidar com esse gargalo na migração web moderna.
