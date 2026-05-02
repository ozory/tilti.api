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

---

## Fluxo de Assinatura e Pagamento do Motorista

### Visão Geral
Motoristas só podem receber corridas se possuírem uma assinatura ativa. O fluxo é 100% digital, recorrente (mensal) e integrado ao Asaas para geração de cobranças e confirmação de pagamento.

### Passos do Fluxo

1. **Criação da Assinatura**
   - Endpoint: `POST /subscriptions/driver`
   - O motorista escolhe um plano e inicia a assinatura.
   - O sistema cria um registro `DriverSubscription` com status `PendingApproval`.
   - É gerado um link de pagamento via Asaas (campo `AsaasPaymentLink`).

2. **Pagamento**
   - O motorista realiza o pagamento pelo link gerado.
   - O Asaas notifica o backend via webhook (`POST /webhooks/asaas`) quando o pagamento é aprovado. O sistema utiliza o `externalReference` para localizar a assinatura interna de forma precisa.

3. **Ativação da Assinatura**
   - O webhook processa o evento e ativa a assinatura (`status = Active`).
   - O motorista passa a poder receber corridas.

4. **Validação de Corridas**
   - Antes de aceitar/atribuir uma corrida, o sistema valida se o motorista tem assinatura ativa (`HasActiveSubscription`).
   - Se não estiver ativa, o pedido é bloqueado.

5. **Cancelamento**
   - Endpoint: `POST /subscriptions/driver/cancel`
   - Permite ao motorista cancelar sua assinatura (status `Canceled`).

6. **Renovação**
   - O Asaas envia novo webhook a cada renovação/pagamento mensal.
   - O sistema atualiza a data de vencimento e mantém o status ativo.

### Endpoints Relacionados
- `POST /subscriptions/driver` — Cria assinatura do motorista
- `POST /subscriptions/driver/activate` — Ativa assinatura (usado internamente/webhook)
- `POST /subscriptions/driver/cancel` — Cancela assinatura
- `GET /subscriptions/driver/{userId}` — Consulta assinatura do motorista
- `POST /webhooks/asaas` — Recebe notificações de pagamento do Asaas

### Entidades Principais
- `DriverSubscription` — Representa a assinatura do motorista
  - Campos: `UserId`, `PlanId`, `Status`, `DueDate`, `AsaasPaymentId`, `AsaasPaymentLink`, `PaidAt`, etc.
- `SubscriptionStatus` — Enum: `Active`, `PendingApproval`, `Canceled`, `Expired`, etc.

### Observações Técnicas
- O repositório `DriverSubscriptionRepository` implementa as queries de status e busca por pagamento.
- O serviço `DriverPaymentService` integra com o Asaas.
- O webhook processa eventos de pagamento e ativa/cancela assinaturas.
- O contexto `TILTContext` possui `DbSet<DriverSubscription>`.
- Há migration e configuração EF para a tabela `driver_subscriptions`.

- Para criar novos fluxos de pagamento, siga o padrão de command/handler + endpoint + integração externa.
- Consulte este README para endpoints e entidades.
- Para detalhes técnicos do fluxo de motoristas, veja [Docs/driver-subscription-pix-flow.md](file:///Users/paulo/Documents/projects/tilt/backend/tilt.api/Docs/driver-subscription-pix-flow.md).
- Para dúvidas ou novas features, peça exemplos de uso ou fluxos detalhados.

---
