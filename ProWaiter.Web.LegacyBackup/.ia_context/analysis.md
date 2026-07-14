# Análise do Projeto ProWaiter.Web

## 1. Visão Geral
O projeto **ProWaiter.Web** é uma aplicação web monolítica (ASP.NET MVC 5 + WebAPI 2) construída sobre o .NET Framework 4.8 (conforme TargetFrameworkVersion v4.7.2/4.8 no csproj). A aplicação utiliza IIS para hospedagem e depende de componentes legados de identidade (`Microsoft.AspNet.Identity.Owin`) e ORM (`EntityFramework` 6.4.4).

O foco desta missão é atualizar essa base de código para o estado da arte do ecossistema .NET, migrando-a para o **.NET 10** (ASP.NET Core) rodando no servidor **Kestrel** e adotando nativamente o **Entity Framework Core**.

## 2. Dependências Customizadas
O projeto atualmente depende de duas bibliotecas proprietárias (`NewSharp.BancoDeDados` e `NewSharp.Ferramentas`).
- **Uso do `NewSharp.BancoDeDados`**: 
  - Fornece a classe base `ContextoBD` e interfaces como `IEntidadeBD` para padronização das entidades.
  - Oferece injeção ou controle de contexto e instâncias via `GestoresEntidades` e repositórios genéricos como `GestorEntidadeBD<T>`.
  - Provê extensões de validação para as entidades (ex: `this.ObterMensagemErro(...)`).
- **Objetivo da Migração**: Todas essas dependências serão removidas. O Entity Framework Core puro será utilizado. O padrão de repositório (`GestoresEntidades`) será reescrito para utilizar interfaces padronizadas ou injetado via Dependency Injection (DI) do próprio ASP.NET Core, eliminando a dependência do pacote antigo. Os recursos utilitários (validação, etc.) da `NewSharp.Ferramentas` serão portados internamente ou substituídos por abstrações nativas do .NET 10.

## 3. Componentes a Serem Migrados
1. **Infraestrutura de Hospedagem**: Sair do `System.Web` (IIS dependente) para ASP.NET Core (Kestrel, `Program.cs` e `Startup.cs`).
2. **Sistema de Rotas e WebAPI**: Migrar do antigo `RouteConfig` e `WebApiConfig` para o roteamento do ASP.NET Core (Endpoint Routing). Controladores MVC herdarão de `Controller` (Core) e Controladores API de `ControllerBase`.
3. **Identity e Segurança**: Substituir o `Microsoft.AspNet.Identity.EntityFramework` pelo `Microsoft.AspNetCore.Identity.EntityFrameworkCore`. A autenticação OWIN será migrada para o Middleware de Autenticação do ASP.NET Core.
4. **Camada de Dados (ORM)**:
   - Substituir EF 6 por EF Core.
   - Refatorar as classes de Mapeamento (Fluent API): Substituir `EntityTypeConfiguration<T>` pelo `IEntityTypeConfiguration<T>` suportado pelo EF Core.
   - Trocar as conexões de banco configuradas no `Web.config` para o `appsettings.json`.
5. **Views e Assets**:
   - Ajustar arquivos Razor (`.cshtml`) para utilizar Tag Helpers do ASP.NET Core ao invés dos tradicionais `HtmlHelper` (ex: `Html.ActionLink` -> `<a asp-action="...">`).
   - Transferir assets estáticos (CSS, JS, imagens) da raiz do projeto para o diretório `wwwroot`.

## 4. Riscos Identificados
- **Acoplamento Forte**: Há bastante lógica misturada em controllers e extenso uso das bibliotecas `NewSharp`. A quebra dessas amarras exigirá uma limpeza cirúrgica.
- **Quebras de Views**: As views baseadas no framework antigo irão quebrar se não atualizadas para o padrão Tag Helpers ou sintaxes modernas do Core.
- **Padrão de Autenticação**: A alteração no Identity impactará o login/registro, necessitando cuidado redobrado para manter hashes de senhas compatíveis (o EF Core Identity sabe lidar com Hashes V2 do Identity antigo, mas é preciso validação).
- **Relatórios**: Há menções a componentes de impressão e relatórios (`ImpressoraRDLC.cs`, Bematech, Elgin, EscPos). Será necessário garantir que estes hardwares e pacotes funcionam em .NET 10 cross-platform ou restringi-los a chamadas Windows-only se houver dependência explícita.
