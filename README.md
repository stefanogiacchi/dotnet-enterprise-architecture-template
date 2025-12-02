 
# .NET Enterprise Architecture Template

## 🚀 Production-ready .NET 8 Backend Template

Modern, opinionated, and enterprise-grade architecture template for **.NET 8**, built on  
**Clean Architecture, CQRS, and Domain-Driven Design (DDD)**.  
Designed for API-centric systems, microservices, and cloud-native applications.



## 🔖 Status & Platform

| Build | License | .NET |
| :: | :--: | :--: |
| ![Build](https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template/actions/workflows/build-and-test.yml/badge.svg) | ![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg) | ![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4) |

 


## 💡 Why this template?

This is not just a boilerplate project.  
It is a **reference implementation** that encodes years of patterns and lessons learned in enterprise systems:

- **Clean Architecture** with strict layering and dependency inversion
- **CQRS + MediatR** for a clear separation between commands (writes) and queries (reads)
- **Pipeline Behaviors** for cross-cutting concerns (validation, logging, metrics, transactions)
- **DDD-friendly Domain Layer** with **Entities, Value Objects, Aggregates, Domain Events, Specifications**
- **Hybrid persistence model**:
  - **EF Core** for transactional write-side operations
  - **Dapper** for optimized read-side queries
- **Enterprise observability**:
  - **Serilog** for structured logging and correlation IDs
  - **OpenTelemetry** for distributed tracing and metrics
- **API Playbook**:
  - REST conventions
  - RFC 7807 **Problem Details** for error handling
  - Pagination, filtering, sorting
  - Async / long-running operation patterns
- **Cloud & container ready**:
  - Docker-friendly layout
  - Health checks, readiness/liveness probes
  - CI/CD–friendly structure



## 🧱 Architecture at a Glance

The template follows a layered, vertical-slice friendly architecture:

```text
Presentation  →  Application  →  Domain  →  Infrastructure
(API)            (CQRS,       (Core         (EF Core, Dapper,
                  Behaviors)   business)     integrations)
````

See the architecture diagrams and C4 model in:

* `docs/architecture/01-high-level.md`
* `docs/architecture/02-cqrs-pipeline.md`
* `docs/architecture/04-domain-modeling.md`



## ⚙️ Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template.git
cd dotnet-enterprise-architecture-template
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Run the API

```bash
dotnet run --project src/Api
```

By default, the API will be available at:

* `https://localhost:<PORT>`
* Swagger UI at `https://localhost:<PORT>/swagger`

> 📌 Check `appsettings.Development.json` and `launchSettings.json` in `src/Api`
> for the actual port configuration.

 

## 📚 Documentation

This repository includes a full documentation set intended as an **architecture playbook**.

### 🔷 Architecture

* [Architecture Overview](docs/architecture/01-high-level.md)
* [CQRS & Mediator Pipeline](docs/architecture/02-cqrs-pipeline.md)
* [API Request Flow](docs/architecture/03-api-flow.md)
* [Domain Modeling (DDD)](docs/architecture/04-domain-modeling.md)
* [Technical Specification (Full)](docs/technical-specification.md)

### 🔷 API Design

* [API Guidelines & REST Conventions](docs/api/api-guidelines.md)
* [Resource Naming & URL Design](docs/api/resource-naming.md)
* [Versioning Strategy](docs/api/versioning.md)
* [Authentication & Authorization](docs/api/authentication.md)
* [Problem Details & Error Handling](docs/api/problem-details.md)
* [Pagination, Filtering & Sorting](docs/api/pagination-filtering-sorting.md)
* [Commands vs Queries](docs/api/commands-vs-queries.md)
* [Async Operations](docs/api/async-operations.md)
* [Status Codes & Headers](docs/api/status-codes.md)
* [Examples & Payloads](docs/api/examples.md)

### 🔷 Domain

* [Domain Overview](docs/domain/README.md)
* [Catalog Domain](docs/domain/catalog.md)
* [Users Domain](docs/domain/users.md)
* [Notifications Domain](docs/domain/notifications.md)
* [Specifications & Business Rules](docs/domain/specifications.md)

*(Adjust links above to match your actual filenames in `docs/domain`.)*

### 🔷 Setup & Operations

* [Local Setup & Prerequisites](docs/setup/getting-started.md)
* [Docker & Containerization](docs/setup/docker.md)
* [CI/CD & GitHub Actions](docs/setup/ci-cd.md)

 

## 🧩 Project Status & Roadmap

This template is under active evolution.

Planned / in progress:

* ✅ Base architecture skeleton (API, Application, Domain, Infrastructure, Shared)
* ✅ CQRS pipeline with logging behavior
* 🔄 Sample vertical slices:

  * `Catalog` (Products, pricing, search)
  * `Users` (registration, authentication)
  * `Notifications` (event-driven delivery)
* 🔄 GitHub Actions: `build-and-test.yml`
* 🔄 Dockerfile and optional `docker-compose.yml`
* 🔄 ADRs in `docs/architecture/decisions/`

> Contributions, ideas, and feedback are welcome.



## 🤝 Contributing

Contributions are welcome as issues or pull requests.

Suggested next improvements:

* Add new vertical slices
* Improve domain examples (aggregates, events, specs)
* Extend API examples and tests
* Enhance CI/CD workflows

You can also use this repository as inspiration for your own internal company template.


## 📄 License

This project is intended to be licensed under the **MIT License**.

Make sure you have a `LICENSE` file in the repository root with the MIT text.




## 👤 Author — Stefano Giacchi  
Cloud Solutions Architect • Enterprise Architecture Track • Senior .NET Engineer

[![Azure Architect Expert](https://img.shields.io/badge/Azure_Architect_Expert-0078D4?style=flat-square&logo=microsoftazure&logoColor=white)](#)
[![DevOps Engineer Expert](https://img.shields.io/badge/DevOps_Engineer_Expert-0078D4?style=flat-square&logo=azurepipelines&logoColor=white)](#)
[![ITIL 4](https://img.shields.io/badge/ITIL_4_Foundation-5C2D91?style=flat-square)](#)
[![PSM I](https://img.shields.io/badge/PSM_I-009FDA?style=flat-square)](#)
[![Neo4j Professional](https://img.shields.io/badge/Neo4j_Professional-008CC1?style=flat-square&logo=neo4j&logoColor=white)](#)  
![C|FA](https://img.shields.io/badge/Cyber_Forensics_Associate-C%7CFA-critical?style=flat-square)


[![LinkedIn](https://img.shields.io/badge/LinkedIn-Stefano_Giacchi-0A66C2?style=flat-square&logo=linkedin)](https://www.linkedin.com/in/stefanogiacchi/)
[![GitHub](https://img.shields.io/badge/GitHub-stefanogiacchi-181717?style=flat-square&logo=github)](https://github.com/stefanogiacchi)
[![HackerRank](https://img.shields.io/badge/HackerRank-Stefano_Giacchi-2EC866?style=flat-square&logo=hackerrank)](https://www.hackerrank.com/profile/stefanogiacchi)




> *“I design scalable architectures that turn complexity into clarity — enabling teams to build reliable, future-proof systems.”*

