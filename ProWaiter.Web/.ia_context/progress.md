# Progresso da Migração: ProWaiter.Web

| Fase | Descrição | Status | Detalhes / Observações |
|---|---|---|---|
| **Fase 1** | Preparação e Setup do Novo Projeto | ✅ Concluído | Backup, criação do `.csproj` moderno, migração do `wwwroot`. |
| **Fase 2** | DB, EF Core e Remoção de DLLs | ✅ Concluído | Abstrações locais criadas, script rodado para mappings e `.csproj` limpo. |
| **Fase 3** | Infraestrutura (DI, settings) | ✅ Concluído | `appsettings.json` gerado, remoção de lixo antigo, configuração no `Program.cs`. |
| **Fase 4** | Identidade e Segurança | ✅ Concluído | Migrado de ASP.NET Identity 2.2 para ASP.NET Core Identity nativo (IdentityDbContext). |
| **Fase 5** | Refatoração de Controllers e APIs | ✅ Concluído | Identidade (AccountController) refatorada para injetar SignInManager. Conexões de DB centralizadas. |
| **Fase 6** | Refatoração de Views | ✅ Concluído | Caminhos dos arquivos estáticos corrigidos. Formulário de login atualizado para Tag Helpers. |
| **Fase 7** | Build e Testes Finais | ✅ Concluído | TPT do Entity Framework corrigido. Conexão DB movida para appsettings. Impressoras via Sockets/TCP nativo criadas em substituição à NewSharp. |

### Histórico de Atividades
- **14/07/2026**: Criação da pasta de contexto `.ia_context`, elaboração da análise (`analysis.md`) e definição do plano arquitetural e sequencial da migração (`migration_plan.md`).
- **15/07/2026**: 
  - Criação da **Nova Arquitetura de Impressão** TCP/IP via `System.Net.Sockets` (substituindo DLL proprietária `NewSharp`).
  - Correção de herança TPT no Entity Framework Core para as entidades filhas de `Pedido` (remoção de HasKey duplicadas).
  - Configuração do contexto de banco de dados para ler o conection string diretamente do `appsettings.json` nos instanciamentos diretos.
  - Ajustes de paths estáticos (`~/Content` e `~/Scripts`) nas Views para o Bootstrap e jQuery funcionarem pós-bundling legado.
  - Refatoração completa do `AccountController.cs` (Autenticação) utilizando .NET Core Identity de forma assíncrona, e ajustes das Views relacionadas.
  - Build alcançando 100% de sucesso sem quebras. Testes de renderização iniciados.
  - **Correção do Schema do Identity Core**: Criação de script SQL local (`ScriptsMigration/01_Migracao_Identity_Core.sql`) para adicionar colunas `NormalizedUserName`, `NormalizedEmail`, `ConcurrencyStamp`, e criar as tabelas `AspNetUserTokens` e `AspNetRoleClaims` no banco de dados, suprindo as exigências do EF Core Identity.
  - **Correção de Lazy Loading**: Ajuste no `ConfiguracoesController` (e aplicável como padrão de projeto) para utilizar o `.Include()` (Eager Loading) nas propriedades de navegação (`cidadeSelecionada.UF`), evitando `NullReferenceException` visto que o EF Core desabilita Lazy Loading por padrão.
  - **Correção de Tipagem de Chave (Unboxing)**: Ajuste nas chamadas `db.Entity.Find(id)` em controllers como `TiposBebidaController` para prever `id.Value` em chaves anuláveis, mitigando `ArgumentException` pela tipagem estrita de PKs no EF Core.
  - **Correção de Deadlocks e Rotas de Helpers**: Substituição das renderizações síncronas legadas (`@Html.Partial()`) por `<partial name="..." />` nas views, e injeção do `IUrlHelperFactory` na nossa classe customizada `CustomHtmlHelpers.cs` garantindo que atributos `href` preservem e gerem o `id` da rota corretamente na grid.
