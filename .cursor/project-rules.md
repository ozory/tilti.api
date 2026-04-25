# Project Rules - Tilt API

## Objetivo

Manter consistencia tecnica no backend `tilt.api` com mudancas pequenas, seguras e alinhadas ao dominio de mobilidade urbana.

## Regras gerais de implementacao

- Priorizar **baixo impacto**: reutilizar arquivos, handlers, endpoints e contratos existentes antes de criar novos.
- Manter padrao arquitetural atual: `Api` -> `Application` -> `Domain` -> `Infrastructure`.
- Evitar logica de negocio em endpoint; concentrar em handlers/comandos.
- Preservar contratos de API sempre que possivel (evitar breaking changes).
- Seguir nomes e organizacao existentes por feature (`Users`, `Orders`, `Subscriptions`, etc.).

## Regras de dominio (mobilidade/caronas)

- Toda avaliacao (`rate`) deve ser vinculada a um `OrderId`.
- Avaliacao permitida apenas entre participantes da corrida (usuario e motorista).
- Impedir autoavaliacao e duplicidade de rate na mesma combinacao.
- Validar entidades de negocio com status ativo quando aplicavel.

## Validacao e persistencia

- Validacoes simples no `Validator` (campos obrigatorios, formatos, limites).
- Validacoes de regra de negocio no `Handler`.
- Persistir via `UnitOfWork` e repositorios (`CommitAsync` ao final).
- Manter logs objetivos em handlers para trilha de execucao/erro.

## Qualidade minima por mudanca

- Compilar: `dotnet build`
- Testar: `dotnet test`
- Se criar fluxo novo, incluir ou planejar teste de unidade do handler.

## Convencoes de resposta para assistente

- Explicar mudancas de forma curta e direta.
- Sempre listar arquivos alterados.
- Destacar riscos/assuncoes quando houver.
