# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Implement order cancellation functionality that allows users to cancel orders before driver acceptance, with proper validation, status updates, and RabbitMQ event publishing for refund processing. The system prevents cancellation of active orders and tracks who initiated the cancellation.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# 12 (.NET 8.0)  
**Primary Dependencies**: ASP.NET Core, Entity Framework Core, FluentResults, RabbitMQ.Client  
**Storage**: MongoDB and PostgreSQL  
**Testing**: xUnit  
**Target Platform**: Linux server  
**Project Type**: Web-service (REST API)  
**Performance Goals**: 1000 req/s, <200ms p95 latency  
**Constraints**: <500ms for cancellation processing, ACID transactions for order state changes  
**Scale/Scope**: 10k active users, horizontal scaling capability

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**MANDATORY CHECKS** (per Tilt API Constitution v1.0.0):
1. ✅ CQRS Pattern: All handlers use `ICommandHandler<>` or `IQueryHandler<>` (NO MediatR)
2. ✅ Documentation: AI agent files (`.agent.md`, `.cursor/rules.md`) are synchronized
3. ✅ Templates: This plan follows `.specify/templates/plan-template.md` exactly
4. ✅ Code Examples: Any examples use real project patterns from `Application/Features/`
5. ✅ Language: Documentation in Portuguese (pt-BR) unless technical terms

**Validation**: Run `/speckit.constitution` to verify compliance

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
# Tilt API Structure (as seen in workspace)
Api/
Application/
Domain/
Infrastructure/
Tests/
Resources/
Docs/
specs/
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
