# Plano de Migração: ProWaiter.Web (.NET 4.8 para .NET 10)

## Fase 1: Preparação e Setup do Novo Projeto
1. **Backup e Segurança**:
   - Fazer um backup completo da pasta `ProWaiter.Web` legada para um diretório seguro (ex: `ProWaiter.Web.LegacyBackup`).
2. **Criação do Novo Projeto**:
   - Excluir ou limpar o arquivo `.csproj` antigo.
   - Criar um novo projeto web vazio com o template ASP.NET Core Web App (MVC + API) rodando em .NET 10 (ou a versão Core mais recente).
   - Ajustar o novo `.csproj` para incluir referências do EF Core, Identity Core, entre outros pacotes base.
3. **Mapeamento de Arquivos Estáticos**:
   - Criar a pasta `wwwroot` no novo projeto.
   - Mover todo o conteúdo das antigas pastas `Content`, `Scripts`, `fonts` e `favicon.ico` para `wwwroot`.

## Fase 2: Substituição da Camada de Banco de Dados e Remoção de DLLs Legadas
1. **Abstração das Interfaces**:
   - Criar as interfaces locais `IEntidadeBD` e afins para não quebrar a tipagem das entidades.
   - Recriar métodos de extensão ou funções utilitárias que vinham das bibliotecas `NewSharp.BancoDeDados` e `NewSharp.Ferramentas`.
2. **Atualização do Contexto (EF Core)**:
   - Alterar `ProWaiterContext` para herdar de `Microsoft.EntityFrameworkCore.DbContext`.
   - Adicionar injeção de dependência (`DbContextOptions<ProWaiterContext>`).
   - Refatorar a aplicação das configurações (`IEntityTypeConfiguration<T>`).
3. **Refatoração dos Mapeamentos Fluent API**:
   - Atualizar a pasta `Models/Mapeamento` para seguir a especificação do EF Core (ex: `HasRequired`/`WithMany` -> `HasOne`/`WithMany`).
4. **Remoção das Bibliotecas Customizadas**:
   - Eliminar definitivamente as referências `NewSharp.BancoDeDados` e `NewSharp.Ferramentas` do projeto.
   - Refatorar `GestoresEntidades.cs` para utilizar a estrutura de repositórios do EF Core ou DI do ASP.NET Core, resolvendo todos os erros de compilação na camada de Dados.

## Fase 3: Infraestrutura (Injeção de Dependências e Configurações)
1. **Arquivos de Configuração**:
   - Remover o arquivo `Web.config` e `Global.asax`.
   - Mover as *ConnectionStrings* e *AppSettings* para o `appsettings.json`.
2. **Program.cs e Middlewares**:
   - Configurar Kestrel, banco de dados, injeção de dependência, pipelines HTTP, e rotas no `Program.cs`.

## Fase 4: Identidade e Autenticação
1. **Migrar Identity**:
   - Atualizar `IdentityModels.cs` para suportar as tabelas nativas de Identity do EF Core.
   - Configurar o esquema de cookies, login, registro e tokens no ASP.NET Core.

## Fase 5: Refatoração dos Controladores e APIs
1. **Adaptar os Controladores (MVC)**:
   - Trocar referências `System.Web.Mvc` para `Microsoft.AspNetCore.Mvc`.
   - Substituir variáveis e objetos nativos como `HttpPostedFileBase` para `IFormFile`, `HttpStatusCodeResult` para `StatusCodeResult`, etc.
   - Corrigir a injeção do contexto do banco de dados (que passará a vir por construtor, idealmente, ou acesso direto temporário adaptado).
2. **Adaptar as WebAPIs**:
   - Converter atributos do `System.Web.Http` (`[HttpGet]`, `[HttpPost]`, `[Route]`) para as equivalentes modernas (`[HttpGet]`, mas no namespace Core).
   - Ajustar os retornos (`IHttpActionResult` -> `IActionResult`).

## Fase 6: Refatoração das Views (Razor) e Conclusão
1. **Views**:
   - Refatorar o `_Layout.cshtml` referenciando caminhos de `wwwroot`.
   - Atualizar métodos de formulários (`@Html.BeginForm`) e inputs (`@Html.EditorFor`) para ASP.NET Core Tag Helpers (`<form asp-action="...">`, `<input asp-for="...">`).
   - Atualizar `@Html.ActionLink` para âncoras comuns com Tag Helpers.
2. **Testes e Build**:
   - Compilar a solução e corrigir falhas sintáticas restantes.
   - Garantir que a comunicação do hardware legado (ex: Impressoras) funcione no novo runtime ou tenha substitutos válidos.
   - Rodar localmente via Kestrel para validação final.
