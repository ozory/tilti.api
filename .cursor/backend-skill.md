# Backend Skill - Tilt API

## Quando usar

Use esta skill para tarefas de backend no `tilt.api`: novas regras de negocio, ajustes de endpoint, handlers, validacoes e integracoes internas.

## Checklist rapido de execucao

1. Identificar a feature alvo (`Users`, `Orders`, `Subscriptions`, etc.).
2. Reusar comando/handler/endpoint existente sempre que possivel.
3. Aplicar validacoes:
   - sintaticas no `Validator`
   - semanticas/regra no `Handler`
4. Persistir com `UnitOfWork` + repositorio + `CommitAsync`.
5. Validar impacto no contrato HTTP (request/response/status).
6. Rodar `dotnet build` e `dotnet test`.
7. Comunicar resultado com resumo objetivo e proximos passos.

## Mapa de camadas (referencia)

- `Api/Endpoints`: entrada HTTP (Minimal API)
- `Application/Features/*`: commands, queries, validators, handlers
- `Domain/Features/*`: entidades e regras centrais
- `Infrastructure/*`: EF Core, repositórios, integrações, mensageria
- `Tests/*`: testes unitarios/aplicacao

## Padrao de mudanca recomendado

- Mudanca pequena e iterativa.
- Evitar criar novos arquivos sem necessidade real.
- Preferir extensao de fluxo existente ao inves de duplicar features.

## Praticas para este projeto

- Rates devem respeitar participantes do pedido (`Order`).
- Evitar acoplamento entre dominios nao relacionados.
- Manter mensagens de erro claras e consistentes.
- Preservar idioma/padrao ja predominante no trecho alterado.
