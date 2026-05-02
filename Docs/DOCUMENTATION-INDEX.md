# 📚 Índice de Documentação - Padrões CQRS

## 🎯 Início Rápido

**Novo no projeto?** Comece aqui:

1. Leia: [CQRS-PATTERNS.md](./CQRS-PATTERNS.md) (5 min)
2. Escolha seu caminho:
   - Implementar Command? → [SKILL-command-handlers.md](./.github/SKILL-command-handlers.md)
   - Implementar Query? → [SKILL-query-handlers.md](./.github/SKILL-query-handlers.md)
3. Use como referência: `Application/Features/Orders/Commands/Create/`

---

## 📑 Arquivos de Documentação

### 🔧 Configuração & Instruções

| Arquivo | Propósito | Para Quem |
|---------|-----------|----------|
| [.github/copilot-instructions.md](./.github/copilot-instructions.md) | Instruções para GitHub Copilot | Copilot, Agentes IA |
| [.cursor/rules.md](./.cursor/rules.md) | Regras customizadas para Cursor | Cursor, Regras locais |
| [.agent.md](./.agent.md) | Config para agentes de IA | Claude, Gemini, etc |

### 📖 Guias Específicos

| Arquivo | Conteúdo | Tempo |
|---------|----------|-------|
| [.github/SKILL-command-handlers.md](./.github/SKILL-command-handlers.md) | Pattern de Command Handlers | 10 min |
| [.github/SKILL-query-handlers.md](./.github/SKILL-query-handlers.md) | Pattern de Query Handlers | 10 min |
| [CQRS-PATTERNS.md](./CQRS-PATTERNS.md) | Guia completo CQRS | 15 min |

---

## 🎓 Por Caso de Uso

### "Preciso implementar um Command Handler"
1. Leia: `.github/SKILL-command-handlers.md`
2. Use template: Seção "Template de Implementação"
3. Consulte: `Application/Features/Orders/Commands/Create/CreateOrderCommandHandler.cs`
4. Checklist: Item "Checklist de Implementação"

### "Preciso implementar uma Query"
1. Leia: `.github/SKILL-query-handlers.md`
2. Use template: Seção "Template de Implementação"
3. Note as diferenças: Tabela "Diferenças entre Command e Query Handlers"
4. Exemplo: `Application/Features/Orders/Queries/GetOrderById/GetOrderByIdQueryHandler.cs`

### "Estou usando GitHub Copilot"
1. Arquivo aplicável: `.github/copilot-instructions.md`
2. É carregado automaticamente para qualquer arquivo
3. Mencione: "Siga o padrão do .github/copilot-instructions.md"
4. Help: "@github /help SKILL-command-handlers"

### "Estou usando Cursor"
1. Arquivo aplicável: `.cursor/rules.md`
2. É carregado automaticamente do `.cursor/`
3. Regras são aplicadas a novos arquivos
4. Verifique: Ctrl+Shift+/ para ver regras ativas

### "Estou usando outro agente (Claude, Gemini, etc)"
1. Arquivo aplicável: `.agent.md`
2. Compartilhe este arquivo com o agente
3. Peça para seguir as instruções
4. Valide com: Checklist de Validação

---

## 🔍 Localizar Informações

### Por Tema

**Nomenclatura**
- `.github/copilot-instructions.md` → Seção "Padrão de Nomenclatura"
- `CQRS-PATTERNS.md` → Seção "Nomenclatura" da Referência Rápida

**Estrutura de Pastas**
- `.github/copilot-instructions.md` → Seção "Estrutura de Pastas"
- `CQRS-PATTERNS.md` → Seção "Estrutura de Pastas"
- `.agent.md` → Seção "Template"

**Padrão de Command Handler**
- `.github/SKILL-command-handlers.md` → Seção "Template de Implementação"
- `.agent.md` → Seção "Template 1: CommandHandler Básico"
- `.cursor/rules.md` → Seção "Regra 2: Implementação de Command Handlers"

**Padrão de Query Handler**
- `.github/SKILL-query-handlers.md` → Seção "Template de Implementação"
- `.agent.md` → Seção "Template 2: QueryHandler Básico"

**Proibições & Anti-patterns**
- `.cursor/rules.md` → Seção "Regra 1: Padrão CQRS"
- `.agent.md` → Seção "Anti-patterns (Nunca Faça)"
- `CQRS-PATTERNS.md` → Seção "Proibições"

**Exemplos Reais**
- `.github/SKILL-command-handlers.md` → Seção "Exemplo Real do Projeto"
- `.github/SKILL-query-handlers.md` → Seção "Exemplos Reais do Projeto"
- `.agent.md` → Seção "Exemplos Reais do Projeto"

**Return Pattern (Result<T>)**
- `CQRS-PATTERNS.md` → Seção "Return Pattern"
- `.agent.md` → Seção "Tratamento de Erro"

---

## ✅ Checklist para Implementação

Use este checklist quando implementar novo handler:

### Antes de Começar
- [ ] Li a documentação apropriada (Command/Query)
- [ ] Conheço a pasta onde colocar
- [ ] Conheço o padrão de nomenclatura

### Durante a Implementação
- [ ] Herda de `ICommandHandler<>` ou `IQueryHandler<>`
- [ ] Injeta dependências via construtor
- [ ] Método `Handle()` retorna `Task<Result<TResponse>>`
- [ ] Command: Valida, Processa, Persiste
- [ ] Query: Busca, Valida nulo, Retorna
- [ ] Exceções capturadas e convertidas em `Result.Fail()`
- [ ] Logging com `_className`
- [ ] Sem MediatR
- [ ] Sem interfaces específicas

### Após Implementação
- [ ] Código compila sem erros
- [ ] Segue nomenclatura (sem typos)
- [ ] Localizado em pasta correta
- [ ] Pode ser executado sem exceções
- [ ] Logging funciona
- [ ] Valida casos de erro

---

## 🚀 Exemplos de Prompt

### Para GitHub Copilot

```
"Implemente CreateSubscriptionCommandHandler seguindo o padrão CQRS 
do projeto. Use ICommandHandler<CreateSubscriptionCommand, SubscriptionResponse>.
Consulte .github/SKILL-command-handlers.md como referência.
Valide e trate erros retornando Result.Fail()."
```

### Para Cursor

```
"Crie um novo QueryHandler para GetSubscriptionById.
Classe: GetSubscriptionByIdQueryHandler
Interface: IQueryHandler<GetSubscriptionByIdQuery, SubscriptionResponse>
Siga as regras definidas em .cursor/rules.md"
```

### Para Claude/Gemini

```
"Implemente um Command Handler seguindo estas regras:
1. Use ICommandHandler<SubscriptionCommand, SubscriptionResponse>
2. Retorne Task<Result<SubscriptionResponse>>
3. Valide entrada com IValidator<>
4. Trate exceções sem lançar, retorne Result.Fail()
5. Logue com _className

Arquivo: Application/Features/Subscriptions/Commands/Create/CreateSubscriptionCommandHandler.cs
Referência: Leia o arquivo .agent.md do projeto"
```

---

## 🔗 Relações entre Documentos

```
copilot-instructions.md (PRINCIPAL)
    ↓
    ├─→ CQRS-PATTERNS.md (GUIA GERAL)
    │       ├─→ Referencia SKILL-command-handlers.md
    │       ├─→ Referencia SKILL-query-handlers.md
    │       └─→ Exemplo: CreateOrderCommandHandler
    │
    ├─→ .cursor/rules.md (REGRAS ESPECÍFICAS)
    │       └─→ Aplicadas automaticamente por Cursor
    │
    ├─→ .agent.md (CONFIG DE AGENTES)
    │       ├─→ Templates de implementação
    │       ├─→ Anti-patterns
    │       └─→ Exemplos reais
    │
    ├─→ SKILL-command-handlers.md (HANDLERS DE COMANDO)
    │       ├─→ Template
    │       ├─→ Checklist
    │       └─→ Exemplo real
    │
    └─→ SKILL-query-handlers.md (HANDLERS DE CONSULTA)
            ├─→ Template
            ├─→ Diferenças
            └─→ Exemplos

Projeto Real (REFERÊNCIA)
    ├─→ Application/Features/Orders/Commands/Create/CreateOrderCommandHandler.cs
    ├─→ Application/Features/Subscriptions/Queries/GetById/GetSubscriptionByIdQueryHandler.cs
    └─→ Segue todos os padrões descritos
```

---

## 📞 Suporte & Troubleshooting

### "Agente sugere MediatR"
- **Causa:** Instrução não foi lida ou ignorada
- **Solução:** Compartilhe `.agent.md` - Seção "Anti-patterns"
- **Ou:** Mencione explicitamente "MediatR é proibido"

### "Agente cria interface específica"
- **Causa:** Não conhece padrão de interfaces genéricas
- **Solução:** Compartilhe `.github/SKILL-command-handlers.md`
- **Ou:** Peça para usar template específico

### "Não sei onde colocar o arquivo"
- **Solução:** Veja `CQRS-PATTERNS.md` - Seção "Estrutura de Pastas"
- **Ou:** Peça ao agente: "Siga a pasta em CQRS-PATTERNS.md"

### "Agente não está seguindo o padrão"
- **Solução 1:** Compartilhe o arquivo `.agent.md` completo
- **Solução 2:** Use Cursor e confie em `.cursor/rules.md`
- **Solução 3:** Valide com checklist de `CQRS-PATTERNS.md`

---

## 📊 Estatísticas

| Métrica | Valor |
|---------|-------|
| Arquivos de documentação | 6 |
| Templates fornecidos | 4+ |
| Regras definidas | 8 |
| Anti-patterns listados | 7 |
| Exemplos reais | 5+ |
| Tempo para ler tudo | ~45 min |

---

## 🎖️ Qualidade Esperada

Após implementar com esta documentação, você deve ter:

✅ **Padrão CQRS consistente**
✅ **Sem MediatR**
✅ **100% type-safe com Result<T>**
✅ **Logging estruturado**
✅ **Tratamento de erro robusto**
✅ **Código limpo e legível**
✅ **Fácil de testar**
✅ **Pronto para production**

---

## 📝 Histórico de Atualizações

| Data | Versão | Mudanças |
|------|--------|----------|
| Mai 2026 | 1.0 | Criação inicial |

---

**Próximas etapas:**
1. Salve este arquivo para referência
2. Compartilhe com seu time
3. Use os arquivos apropriados para suas ferramentas
4. Reporte issues em `.agent.md`

**Dúvidas?** Verifique `CQRS-PATTERNS.md` ou consulte arquivo específico conforme tema.
