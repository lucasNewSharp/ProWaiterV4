# Progresso da Migração: ProWaiter.Web

| Fase | Descrição | Status | Detalhes / Observações |
|---|---|---|---|
| **Fase 1** | Preparação e Setup do Novo Projeto | ⏳ Pendente | Backup, criação do `.csproj` moderno, migração do `wwwroot`. |
| **Fase 2** | DB, EF Core e Remoção de DLLs | ⏳ Pendente | Substituição das libs `NewSharp`, ajuste do DbContext e Mapeamentos. |
| **Fase 3** | Infraestrutura (DI, settings) | ⏳ Pendente | `appsettings.json`, remoção do `Web.config`, configuração no `Program.cs`. |
| **Fase 4** | Identidade e Segurança | ⏳ Pendente | Migrar de ASP.NET Identity 2.2 para ASP.NET Core Identity. |
| **Fase 5** | Refatoração de Controllers e APIs | ⏳ Pendente | Roteamento Endpoint, alteração de namespaces, DI em Controllers. |
| **Fase 6** | Refatoração de Views | ⏳ Pendente | Mudança para Tag Helpers, caminhos estáticos. |
| **Fase 7** | Build e Testes Finais | ⏳ Pendente | Testes locais com Kestrel, validação de integrações com Impressoras. |

### Histórico de Atividades
- **14/07/2026**: Criação da pasta de contexto `.ia_context`, elaboração da análise (`analysis.md`) e definição do plano arquitetural e sequencial da migração (`migration_plan.md`).
