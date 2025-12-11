# ADR-001 – Adopting CQRS vs. Traditional CRUD Approach

- **Status**: Approved
- **Date**: 2025-12-10
- **Version**: 1.0
- **Author**: Stefano Giacchi
- **Related to**: dotnet-enterprise-architecture-template

---

## 1. Context

The goal of the `dotnet-enterprise-architecture-template` project is to provide an enterprise architecture template:

- modular, extensible, and vertically slice-friendly
- suitable for complex scenarios (regulated domains, public administration, enterprise, AI integration, microservices)
- with a strong emphasis on maintainability, testability, evolvability, and observability

In this context, it is necessary to choose a data access and operations modeling pattern:

- Traditional CRUD (Create/Read/Update/Delete) approach with generic controllers and services
- CQRS (Command and Query Responsibility Segregation) pattern, with clear separation between:
- read operations (Query)
- write operations (Command)

The template must:

- be clear for development teams
- support domain growth over time
- allow cross-cutting pipeline integration (logging, auditing, validation, security, metrics)
- facilitate the adoption of advanced patterns (event sourcing, AI integration, orchestrations, workflows) without requiring a complete rewrite.

---

## 2. Problem / Motivating Force

The CRUD approach, while simple to understand and implement, quickly leads to problems in enterprise scenarios:

- Bloated and unclear controllers and services ("god classes")
- Mixing of read and write logic in the same pipeline
- Difficulty consistently introducing cross-cutting behaviors (logging, tracing, authorization, validation, retry policy, etc.)
- Reduced adherence to real use cases (*Use Case Driven Design*)
- Increasing complexity in maintaining complex business rules as the domain evolves

We need a model that:

- Maintains a clear scope of responsibility
- Facilitates the composition of cross-functional behaviors
- Integrates well with MediatR / pipeline behaviors / request–response patterns
- Can be applied in API, microservices, LLM/AI integration, and workflow contexts.

---

## 3. Decision

The template will adopt **CQRS** as the basic pattern** for managing application operations, with:

1. **Clear separation between Command and Query**
- *Command* = operations that **modify the state** (create, update, delete, process operations, orchestrations, etc.)
- *Query* = operations that **read the state** (searches, lists, details, export, reports, AI suggestions, etc.)

2. **Use of a mediator (e.g., MediatR)**
- Each Command/Query is modeled as a **request** with its own **dedicated handler**
- Cross-cutting pipelines (logging, validation, authorization, metrics, performance) are implemented as mediator **behaviors**

3. **"Vertical Slices"-oriented structure**
- Each application feature contains its own Command, Query, Handler, Validator, DTO, mapping
- Reduce vertical coupling (controller → application → domain) and greater cohesion around use cases

4. **Persistence and data access aligned with CQRS**
- Ability to use:
- the same data model for read/write in simple solutions
- differentiated models (optimized read models, denormalized views) in complex scenarios, without breaking the basic structure

The traditional CRUD approach will be considered **only for simple, internal, or support modules**, where the cost of CQRS would not be justified.

---

## 4. Main Motivations

### 4.1 Alignment with Real-World Use Cases

- CQRS encourages modeling code around Use Cases, not database tables.
- Improves readability: "What does this piece of code do?" is explicitly stated in the Command/Query name.

### 4.2 Maintainability and Evolvability

- Each Command/Query has its own handler → Isolated responsibility → Easier changes.
- Reduces the risk of regressions because each flow is independently testable.
- Easier to manage complex business rules and regulatory changes.

### 4.3 Structured Cross-cutting Concerns

- CQRS and mediator allow you to centralize:
- logging
- tracing/distributed tracing
- validation
- authorizations
- metrics and performance
- All without polluting the logic of individual handlers with duplicate code.

### 4.4 Functional and Architectural Scalability

- Read/write separation makes it easier to:
- optimize queries for performance (dedicated read models, cache, denormalization)
- isolate write domain models to ensure invariants and consistency
- Accelerates future evolution towards event sourcing, microservices, or hybrid systems.

### 4.5 Consistency with a "modern" .NET architecture

- CQRS is now a de facto standard in modern enterprise templates in .NET.
- Facilitates the adoption of other best practices: Clean Architecture, Vertical Slice Architecture, DDD light.

---

## 5. Alternatives Considered

### 5.1 Traditional CRUD (Generic Controller + Services)

**Pros:**

- Easier for junior teams to understand
- Fewer files/classes initially
- Good for very simple data access-only APIs

**Cons:**

- Controllers and services tend to grow uncontrollably
- Business logic often duplicated or mixed between layers
- Difficult to apply cross-cutting concerns consistently
- Not aligned with the complexity of the template's target enterprise domains

**Conclusion:**
Rejected as a core pattern. It can still be used in very simple modules, but is not suitable as a template architectural standard.

---

### 5.2 CRUD + some Use Case Services (a "light" hybrid approach)

**Pros:**

- Reduces complexity somewhat compared to full CQRS
- Maintains a structure relatively close to classic CRUD
- Allows for a gradual introduction of use case concepts

**Cons:**

- Risk of inconsistency: some features are CQRS-like, others are not
- Cross-cutting often still handled in a haphazard manner
- However, it tends to degenerate as the domain grows

**Conclusion:**

Considered but not chosen as a standard because it doesn't offer a clear and unambiguous guideline. Better suited to existing projects in refactoring than to a new template.

---

### 5.3 Event Sourcing + CQRS from the start

**Pros:**

- Maximum traceability of domain status and events
- Ideal for some specific domains (finance, logistics, audit, critical systems)

**Cons:**

- Much added complexity (event infrastructure, store, replay, projections)
- Not suitable as a default for a template that needs to be reusable across various contexts
- Oversized for many "classic" enterprise projects

**Conclusion:**
Not adopted as a default. It may be introduced as an **advanced extension** in future ADRs, for specific use cases.

---

## 6. Consequences

### 6.1 Positive Consequences

- More **modular, readable, testable** code.
- Easier for multiple teams to work in parallel on different features.

- Well-structured cross-cutting, especially with MediatR pipeline behaviors.

- Greater alignment with modern .NET best practices and enterprise architectures.
- Template perceived as "enterprise-ready" and not a simple CRUD starter.

### 6.2 Negative Consequences / Trade-offs

- Greater number of files/classes to manage (Command, Query, Handler, DTO, Validator, etc.).
- Steeper learning curve for developers accustomed to CRUD alone.
- In very simple use cases, CQRS may appear "overkill."

---

## 7. Operational Guidelines

To maintain consistency in the template:

1. **All new application functionality** must be modeled as:
- `Command` + `CommandHandler` for write operations
- `Query` + `QueryHandler` for read operations

2. API controllers must be:
- thin, delegating immediately to the mediator (`IMediator`/`ISender`)
- oriented to a specific use case per endpoint

3. Cross-cutting logic should preferably be added via:
- **Pipeline Behaviors** (e.g., `LoggingBehavior`, `ValidationBehavior`, `AuthorizationBehavior`, `PerformanceBehavior`)

4. Where the application is **very simple**:
- it is possible to have “light” Command/Query that directly encapsulate the CRUD logic, while maintaining the CQRS structure.

---

## 8. Future Review

This decision p