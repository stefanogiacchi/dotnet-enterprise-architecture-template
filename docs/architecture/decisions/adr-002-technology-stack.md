# ADR-002 – Technology Stack Selection

- **Status:** Approved  
- **Date:** 2025-12-10  
- **Author:** Stefano Giacchi  
- **Project:** dotnet-enterprise-architecture-template  

---

## 1. Context

This project provides an **enterprise-grade architecture template** for modern .NET applications.  
To ensure consistency, maintainability, and long-term scalability, we need a clear and well-defined **technology stack**.

The stack must support:

- Clean Architecture and Vertical Slice Architecture  
- CQRS patterns  
- Cloud-native development  
- Integration with modern DevOps practices  
- AI-ready extension points  
- Deployments in enterprise environments and regulated industries  

Choosing the right technologies at the beginning avoids fragmentation and simplifies future development.

---

## 2. Problem

Without a defined stack:

- teams may use different tools or patterns  
- the template becomes inconsistent  
- maintainability decreases  
- onboarding becomes harder  
- architecture quality varies across features  

We need a **standard, opinionated, but flexible** technology selection that the whole project follows.

---

## 3. Decision

We adopt the following **official technology stack** for the template:

---

### 3.1 Backend (.NET)

| Technology | Reason |
|------------|--------|
| **.NET 8+** | High performance, LTS support, modern framework features. |
| **ASP.NET Core Web API** | Standard for building REST services and microservices. |
| **MediatR** | Clean implementation of CQRS and request pipelines. |
| **FluentValidation** | Clear and reusable validation logic for commands/queries. |
| **Mapster (or AutoMapper)** | Fast and simple object-to-object mapping. |
| **Serilog** | Structured logging and wide support for enterprise sinks. |
| **OpenAPI / Swagger** | Automatic API documentation and client generation. |

---

### 3.2 Data Access

| Technology | Reason |
|------------|--------|
| **Dapper** | Fast, lightweight, ideal for queries and microservices. |
| **EF Core** | Useful for domain-driven aggregates and transactional consistency. |
| **Hybrid approach** | Allows teams to choose the best tool for each scenario. |

---

### 3.3 Cloud & DevOps

| Component | Reason |
|-----------|--------|
| **Azure App Services / Containers** | Modern hosting options for APIs. |
| **Azure SQL / PostgreSQL** | Reliable relational databases. |
| **Azure Key Vault** | Secure secrets management. |
| **Azure Monitor / Application Insights** | Logging, metrics, and distributed tracing. |
| **GitHub Actions / Azure DevOps** | Automated CI/CD pipelines for deployments. |

---

### 3.4 Architecture Principles

| Principle | Description |
|-----------|-------------|
| **Clean Architecture** | Separation of concerns and long-term maintainability. |
| **Vertical Slices** | Features isolated by domain, not by technical layers. |
| **CQRS** | Clear separation of read and write responsibilities. |
| **OpenAPI-first** | APIs documented by default. |
| **Infrastructure-as-Code (optional)** | Recommended for cloud deployments. |

---

### 3.5 AI Integration (Optional but encouraged)

| Component | Reason |
|-----------|--------|
| **External LLM connectors** | Allow integration with OpenAI, Azure OpenAI, or local models. |
| **Vector store support** | Enables semantic search and retrieval-augmented applications. |
| **AI orchestration layer** | Centralized way to add AI-driven features. |

---

## 4. Alternatives Considered

### ❌ Use only EF Core  
Too slow for heavy queries and large-scale enterprise APIs.

### ❌ Use only Dapper  
Good for queries, but not ideal for complex aggregates or domain rules.

### ❌ Do not define a stack  
High risk of inconsistency and chaotic architecture.

---

## 5. Consequences

### Positive

- Consistent and professional project structure  
- Easier onboarding for new developers  
- Strong alignment with industry best practices  
- Smooth evolution toward microservices or AI features  
- Better readiness for cloud and DevOps automation  

### Negative

- Some developers may prefer different libraries  
- Slight initial learning curve for teams new to MediatR or Clean Architecture  

---

## 6. Future Changes

This ADR may be updated if:

- .NET introduces new major architectural features  
- we adopt new AI integration patterns  
- cloud infrastructure requirements change  
- the template adds event sourcing or advanced messaging patterns  

Any major change will be documented with a new ADR.

---
