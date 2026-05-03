<!--
Sync Impact Report:
Version change: NEW → 1.0.0
Modified principles: N/A (initial creation)
Added sections: Core Principles (I-VI), Documentation Standards, Development Workflow, Governance
Removed sections: N/A
Templates requiring updates:
  - ✅ .specify/templates/constitution-template.md (updated)
  - ⚠ .specify/templates/plan-template.md (Constitution Check section needs alignment)
  - ⚠ .specify/templates/spec-template.md (needs review)
  - ⚠ .specify/templates/tasks-template.md (needs review)
TODOs: Verify template alignment in next iteration
-->

# Tilt API Constitution

## Core Principles

### I. Documentation Consistency (MANDATORY)
All documentation files MUST follow consistent patterns and structures. Every `.md` file must serve a clear purpose and be maintained in sync with related documents. Documentation MUST be written in Portuguese (pt-BR) unless explicitly specified otherwise for technical terms.

### II. CQRS Pattern Enforcement (NON-NEGOTIABLE)
All Command and Query Handler documentation MUST explicitly reference the CQRS pattern used in this project:
- Commands: MUST use `ICommandHandler<TCommand, TResponse>`
- Queries: MUST use `IQueryHandler<TQuery, TResponse>`
- MediatR: STRICTLY PROHIBITED in all documentation and code
- Return type: MUST be `Task<Result<TResponse>>` using FluentResults

### III. AI Agent Instruction Standards (MANDATORY)
All AI agent configuration files (`.agent.md`, `.cursor/rules.md`, `.github/copilot-instructions.md`) MUST:
- Explicitly prohibit MediatR usage
- Reference the correct interfaces (`ICommandHandler`/`IQueryHandler`)
- Include code examples from the project
- Provide clear anti-patterns section
- Link to relevant documentation indices

### IV. Template Compliance (MANDATORY)
All template files in `.specify/templates/` MUST:
- Be followed exactly by generated documents
- Include placeholder tokens in `[BRACKETS]`
- Provide clear examples in comments
- Be validated during constitution checks

### V. Documentation Synchronization (REQUIRED)
When updating one documentation file, related files MUST be checked and updated:
- `.agent.md` changes → check `.cursor/rules.md`, `.github/copilot-instructions.md`
- `README.md` changes → check `Docs/DOCUMENTATION-INDEX.md`
- Template changes → validate all generated docs still comply

### VI. Code Example Accuracy (MANDATORY)
All code examples in documentation MUST:
- Compile without errors
- Follow the exact patterns in `Application/Features/`
- Use real project namespaces and classes
- Be tested against actual project code

## Documentation Standards

### File-Specific Requirements

**.agent.md / .cursor/rules.md / .github/copilot-instructions.md:**
- MUST include "Proibição Absoluta de MediatR" section
- MUST reference `ICommandHandler<>` and `IQueryHandler<>`
- MUST include real code examples from project
- MUST have anti-patterns section

**README.md:**
- MUST describe project purpose clearly
- MUST list all major features
- MUST document architecture decisions
- MUST include technology stack

**Docs/*.md:**
- MUST be indexed in `Docs/DOCUMENTATION-INDEX.md`
- MUST follow `CQRS-PATTERNS.md` as primary guide
- MUST be cross-referenced with related docs

**.specify/templates/*.md:**
- MUST use `[PLACEHOLDER]` format for variables
- MUST include comment examples
- MUST be referenced in constitution

## Development Workflow

### Constitution Check
Every `/speckit.plan` command MUST verify:
1. Documentation follows CQRS patterns
2. No MediatR references exist
3. Templates are properly followed
4. AI agent files are synchronized

### Documentation Updates
When modifying handlers or patterns:
1. Update `.agent.md`
2. Update `.cursor/rules.md`
3. Update `.github/copilot-instructions.md`
4. Update relevant `Docs/*.md`
5. Verify template compliance

## Governance

This Constitution supersedes all other documentation practices. Any PR that violates these principles MUST be rejected.

**Version**: 1.0.0 | **Ratified**: 2026-05-03 | **Last Amended**: 2026-05-03
