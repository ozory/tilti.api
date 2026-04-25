# Tilt API

Backend da plataforma Tilt, um app de mobilidade no estilo caronas urbanas (similar ao Uber).

## Objetivo

Esta API gerencia o fluxo principal do aplicativo: cadastro e autenticacao de usuarios, criacao e acompanhamento de corridas/pedidos, planos/assinaturas, pagamentos e avaliacoes entre passageiro e motorista.

## Funcionalidades principais

- Cadastro, atualizacao e validacao de usuarios
- Login e refresh token
- Criacao e consulta de pedidos/corridas
- Mensageria e tracking de pedido
- Planos e assinaturas
- Integracoes de pagamento
- Avaliacao bidirecional por corrida:
  - passageiro avalia motorista
  - motorista avalia passageiro

## Arquitetura (resumo)

O projeto segue separacao por camadas:

- `Api/`: endpoints HTTP (Minimal API)
- `Application/`: casos de uso (`Commands`, `Queries`, validacoes, handlers)
- `Domain/`: entidades e regras de negocio
- `Infrastructure/`: banco de dados, repositórios e integracoes externas
- `Tests/`: testes automatizados

Padroes usados:

- `Command/Query` com mediator interno
- `UnitOfWork` + repositorios
- Eventos de dominio e consumo assíncrono (ex.: notificacoes por fila)

## Stack tecnica

- .NET 10 (C#)
- Entity Framework Core
- PostgreSQL
- RabbitMQ (mensageria)

## Observacoes

- O endpoint de avaliacao de usuario/motorista esta em `POST /users/rate`.
- A API foi organizada para evolucao incremental com baixo acoplamento entre dominios.
