# Contributing to .NET Enterprise Architecture Template

Thank you for considering contributing to the **.NET Enterprise Architecture Template**!
This project aims to provide a **production-grade, architecture-driven foundation** for modern .NET 8 enterprise applications.
Contributions are welcome and highly appreciated — whether they involve reporting bugs, improving documentation, or adding new architectural components.



## 🧭 Contribution Principles

Before contributing, please follow these project principles:

### ✔ Architecture First

Follow Clean Architecture, CQRS, DDD, and SOLID principles.

### ✔ Consistency Over Creativity

Changes should align with the existing style and conventions.

### ✔ Keep It Enterprise-Ready

Each contribution must maintain or enhance production quality.

### ✔ Documentation Is Not Optional

Every feature, decision, or component **must** be documented.



# 📝 How to Contribute

## 1️⃣ Open an Issue

Before submitting a pull request, please **open an issue** to discuss:

* New features
* Improvements
* Bugs
* Documentation updates
* API/architecture changes

Use one of the predefined templates:

* **Bug Report**
* **Feature Request**
* **Architecture Decision Proposal (ADR)**
* **Documentation Update**

👉 Issues help maintain design coherence and avoid duplicated work.



## 2️⃣ Fork the Repository

```bash
git fork https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template
cd dotnet-enterprise-architecture-template
```

Then create a new branch:

```bash
git checkout -b feature/my-new-feature
```

Use consistent naming:

| Type     | Format                 |
| -- | - |
| Feature  | `feature/<name>`       |
| Fix      | `fix/<issue-number>`   |
| Docs     | `docs/<section>`       |
| Refactor | `refactor/<component>` |



## 3️⃣ Development Guidelines

### ✔ Coding Standards

* Use **C# 12** guidelines & naming conventions.
* Follow Microsoft’s official documentation for style.
* Keep files short and focused.
* One class per file.
* Meaningful names and strict domain semantics.

### ✔ Architecture Standards

Each contribution must respect:

* **Clean Architecture Dependency Rule**
* **CQRS separation (Commands vs Queries)**
* **Vertical Slice Foldering**
* **DDD tactical patterns**

### ✔ Testing Requirements

All contributions **must** include tests:

* Unit tests for domain logic and handlers.
* Integration tests for API endpoints or persistence.
* Mock external systems.
* Keep tests deterministic.



## 4️⃣ Commit Standards

Use **conventional commits**:

```
feat: add new Product aggregate to domain
fix: correct null reference in CreateOrderHandler
docs: improve architecture overview diagrams
refactor: extract pipeline behavior logic
test: add integration tests for /api/products
```



## 5️⃣ Pull Request Guidelines

Before opening a PR, ensure:

* All tests pass:

  ```bash
  dotnet test
  ```
* Code compiles with no warnings.
* Documentation is updated (Markdown files, diagrams, examples).
* The PR description explains the **why**, not only the what.

### ✔ Pull Request Must Include

* Clear, concise title
* Linked issue number (e.g., “Fixes #42”)
* Summary of changes
* Architectural reasoning (if applicable)
* Screenshots or diagrams if relevant
* Checklist confirming tests and linting

### ✔ PRs That Will Be Rejected

* Breaking the Clean Architecture rule
* Mixing unrelated changes in a single PR
* Non-documented features
* PRs without tests
* Cosmetic refactoring without value



# 📐 Architecture Contributions

If adding:

### 🧱 New Domain Model

Add:

* Aggregate
* Value Objects
* Domain Events
* Specifications
* Documentation in `docs/domain/...`

### ⚙ New Application Feature (Command/Query)

Add:

* Command or Query
* Handler
* DTOs
* Validator
* Mapping Profile
* Tests (unit + integration)

### 🏗 New API Endpoint

Add:

* Controller method
* Request/Response contracts
* ProblemDetails mapping
* Swagger docs
* Samples in `docs/api/examples.md`

### 🧩 Infrastructure Implementation

Add/update:

* Repository
* EF Core config
* Database migration
* External service client
* Retry policies / Resilience
* Logging + OpenTelemetry instrumentation



# 📄 Documentation Requirements

Each PR must update relevant documentation:

* Architecture: `docs/architecture/*.md`
* API Design: `docs/api/*.md`
* Domain: `docs/domain/*.md`
* Setup/Dev Environment: `docs/setup/*.md`
* Diagrams (Mermaid or images) when needed

> **Rule:** *No feature is considered “complete” unless documented.*



# 🧪 Running the Project

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Api
```



# ⭐ Code of Conduct

By contributing, you agree to respect:

* Professional collaboration
* Architectural consistency
* Respectful communication
* Technical discussion backed by reasoning



# 🙌 Thank You

Your contribution helps create a powerful, enterprise-grade .NET architecture that can benefit thousands of developers and companies.

If you want to collaborate, discuss architecture, or propose major enhancements, feel free to reach out:

---

## About the Author

**Stefano Giacchi**
Cloud Architect • Enterprise Architect Track • AI-Driven Engineer

Architecting large-scale distributed systems for over 20 years, with a focus on
**cloud-native platforms, enterprise integration patterns, AI-first architectures, and modern application design**.

Experienced across **Energy, Manufacturing, Finance, and Public Sector**, with a consistent track record of transforming complex requirements into scalable, maintainable and future-proof solutions.

Creator of advanced architectural accelerators and knowledge frameworks adopted by teams and practitioners across different countries.

Passionate about **AI governance, semantic architectures, intelligent APIs, and the evolution of enterprise software in the age of LLMs**.

### Certifications

[![Azure Architect Expert](https://img.shields.io/badge/Azure_Architect_Expert-0078D4?style=flat-square\&logo=microsoftazure\&logoColor=white)](#)
[![DevOps Engineer Expert](https://img.shields.io/badge/DevOps_Engineer_Expert-0078D4?style=flat-square\&logo=azurepipelines\&logoColor=white)](#)
[![ITIL 4](https://img.shields.io/badge/ITIL_4_Foundation-5C2D91?style=flat-square)](#)
[![PSM I](https://img.shields.io/badge/PSM_I-009FDA?style=flat-square)](#)
[![Neo4j Professional](https://img.shields.io/badge/Neo4j_Professional-008CC1?style=flat-square\&logo=neo4j\&logoColor=white)](#)
![C|FA](https://img.shields.io/badge/Cyber_Forensics_Associate-C%7CFA-critical?style=flat-square)

### Links

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Stefano_Giacchi-0A66C2?style=flat-square\&logo=linkedin)](https://www.linkedin.com/in/stefanogiacchi/)
[![GitHub](https://img.shields.io/badge/GitHub-stefanogiacchi-181717?style=flat-square\&logo=github)](https://github.com/stefanogiacchi)
[![HackerRank](https://img.shields.io/badge/HackerRank-Stefano_Giacchi-2EC866?style=flat-square\&logo=hackerrank)](https://www.hackerrank.com/profile/stefanogiacchi)

> *“I build architectures that scale, govern data, and enable intelligent behavior — transforming software into a strategic asset.”*