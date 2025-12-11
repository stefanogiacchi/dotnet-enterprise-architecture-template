# ADR-003 – CQRS vs Traditional Layered Architecture

- **Status:** Approved  
- **Date:** 2025-12-10  
- **Author:** Stefano Giacchi  
- **Project:** dotnet-enterprise-architecture-template  

---

## 1. Context

This project uses a **Clean Architecture + Vertical Slice Architecture** approach.  
A key architectural decision is choosing how to structure application logic:

- **Traditional Layered Architecture** (Controllers → Services → Repositories → Database)  
- **CQRS (Command and Query Responsibility Segregation)**  
  with MediatR and separated read/write models  

Both models are common in .NET projects.  
However, the template must support:

- high scalability  
- clear separation of concerns  
- independent evolution of features  
- testability  
- enterprise-level complexity  

We need a pattern that simplifies growth and keeps the codebase clean.

---

## 2. Problem

Traditional layered architecture often causes:

- large service classes («God Services»)  
- mixing of read and write concerns  
- unclear domain boundaries  
- difficulty adding cross-cutting behaviors  
- hard-to-test business logic  
- slow evolution as the project grows  

This template aims to provide **best practices**, not legacy patterns.

Therefore, we must decide whether to adopt CQRS as the default model.

---

## 3. Decision

We adopt **CQRS as the default application pattern**.

### This means:

1. **Commands** handle operations that change state  
2. **Queries** retrieve data  
3. **Each Command/Query has its own Handler**  
4. **MediatR pipeline behaviors** handle logging, validation, security, and performance  
5. Features follow **Vertical Slice Architecture**, not technical layers  
6. Domain logic stays inside the Application/Domain layers  

Layered architecture is **not used** as the default approach, except for very simple modules where CQRS would be excessive.

---

## 4. Reasons

### 4.1 Better alignment with real use cases  
CQRS models the application around **business actions**, not database tables.

### 4.2 Improved maintainability  
Each feature has its own folder and its own request/handler.  
No huge service classes. No hidden coupling.

### 4.3 Cross-cutting logic becomes simple  
Pipeline behaviors allow:

- unified logging  
- centralized exception handling  
- consistent validation  
- request-level performance tracking  

Without duplicating code.

### 4.4 Easier testing  
Commands and Queries are small, isolated, and testable.

### 4.5 Read and write paths can evolve independently  
Queries can use:

- Dapper  
- custom SQL  
- cached projections  

Commands can use:

- EF Core  
- domain rules  
- transactions  

This flexibility is not possible in a classic layered approach.

### 4.6 Fits perfectly with Clean Architecture  
CQRS reinforces:

- Dependency inversion  
- Separation of concerns  
- Domain-driven design principles  

---

## 5. Alternatives Considered

### ❌ Traditional Layered Architecture

**Pros:**
- Familiar to many developers  
- Fewer files at the beginning  

**Cons:**
- Hard to scale  
- Business logic spreads across layers  
- Poor modularity  
- Difficult to test  
- Risk of “service bloat”  

**Conclusion:** Not suitable for a long-term enterprise template.

---

### ❌ Hybrid (Layered + Partial CQRS)

**Pros:**  
- Softer learning curve  
- Slightly less boilerplate  

**Cons:**  
- Inconsistent architecture  
- Confusing for teams  
- Not predictable  
- Hard to enforce standards  

**Conclusion:** Rejected for inconsistency.

---

## 6. Consequences

### Positive
- Clear and scalable architecture  
- Predictable code organization  
- Very easy onboarding for developers  
- Clean separation of responsibilities  
- Strong foundation for DDD, microservices, and AI workflows  

### Negative
- More files to manage  
- Requires basic understanding of CQRS and MediatR  
- Might feel “overkill” for tiny applications  

---

## 7. Future Considerations

This ADR may evolve when:

- event sourcing is introduced  
- advanced workflows (Sagas, orchestrators) are added  
- the template integrates messaging systems (Azure Service Bus, Kafka)  
- domain complexity grows  

Any major change will be documented in a new ADR.

---
