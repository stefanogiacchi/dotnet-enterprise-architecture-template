# .NET Enterprise Architecture Template  
### Clean Architecture • CQRS • Mediator • DDD • .NET 8 • Serilog • EF Core • OpenTelemetry

A production-ready **.NET 8 enterprise template** that implements modern architectural best practices  
for building scalable, maintainable, testable and cloud-ready applications.

This template is designed for **enterprise systems**, **microservices**, **API backends**,  
and **modular domains**.  
It serves as a strong foundation for real-world projects and as a reference implementation for teams.

---

# 🚀 Purpose

This template provides:
- A **real Clean Architecture implementation**
- A complete **CQRS + Mediator** pipeline
- A **Domain-Driven Design** structure (Entities, Value Objects, Domain Events)
- A consistent **vertical slice architecture**
- Enterprise-grade **logging, telemetry, error handling**
- Well-organized **API endpoints**
- Ready-to-use **DI, configuration, validation**
- **EF Core + Dapper hybrid** data access
- Cloud-friendly structure (Azure, AWS, containers)

---

# 🧱 Architecture Overview

The architecture follows Clean Architecture with domain boundaries and vertical slices:

```txt

src/
│
├── Api/ -> HTTP endpoints, controllers, filters, middlewares
│
├── Application/ -> Use cases, commands, queries, handlers, DTOs
│ ├── Behaviors/ -> Pipeline behaviors (logging, validation, metrics)
│ ├── Common/ -> Shared logic, abstractions
│ ├── Commands/ -> Write operations
│ └── Queries/ -> Read operations
│
├── Domain/ -> Entities, Aggregates, Value Objects, Domain Events
│
├── Infrastructure/ -> EF Core, Dapper, repositories, external services
│ ├── Persistence/
│ ├── Services/
│ └── Migrations/
│
└── Shared/ -> Utility classes, constants, cross-cutting code
````


Key principles:
- **Highly decoupled** layers  
- **Domain at the center**  
- **Application is the orchestration layer**  
- **Infrastructure is replaceable**  
- **API is just the delivery mechanism**

---

# ⚙️ Features

### ✔ Clean Architecture  
Strict separation of concerns and dependency inversion.

### ✔ CQRS + Mediator Pattern  
Commands & Queries handled through Mediator pipelines.

### ✔ Domain-Driven Design  
Aggregates, Value Objects, Domain Events, Specifications.

### ✔ Enterprise Logging  
- Serilog  
- Structured logs  
- Enrichers  
- Correlation ID  

### ✔ Validation Layer  
- FluentValidation  
- Pipeline Behavior pattern  

### ✔ Data Access  
- Entity Framework Core  
- Dapper  
- Repository pattern optional  
- Unit of Work (if needed)

### ✔ Observability  
- OpenTelemetry  
- Request tracing  
- Metrics (Prometheus-ready)

### ✔ API Best Practices  
- Versioning  
- Consistent response models  
- Exception filters  
- ProblemDetails standard

---

# 🛠️ Tech Stack

### Backend
- **.NET 8 WebAPI**
- **MediatR**
- **FluentValidation**
- **Serilog**
- **OpenTelemetry**
- **EF Core / Dapper**
- **Mapster or AutoMapper**

### Optional integrations
- PostgreSQL  
- SQL Server  
- MongoDB  
- Azure SQL  
- Redis  
- Azure Service Bus  
- Kafka  

---

# 📦 Included Examples

This template includes example modules:

- `Users`  
- `Catalog`  
- `Notifications`  

Each module demonstrates:
- Commands, Queries, Handlers  
- Validation  
- Mapping  
- Domain logic  
- Infrastructure implementation  
- API endpoints  

---

# 📂 Folder Structure (full)

````txt
dotnet-enterprise-architecture-template/
│
├── src/
│ ├── Api/
│ ├── Application/
│ ├── Domain/
│ ├── Infrastructure/
│ └── Shared/
│
├── tests/
│ ├── UnitTests/
│ └── IntegrationTests/
│
├── docs/
│ ├── architecture/
│ ├── domain/
│ ├── api/
│ └── README.md
│
└── README.md


````

---

# 🧪 Testing

Testing setup with:
- xUnit  
- FluentAssertions  
- Moq  
- WebApplicationFactory for API testing  
- Database integration tests  

---

# 🧰 DevOps & Deployment

Includes examples for:
- GitHub Actions CI/CD  
- Dockerfile for API  
- docker-compose for local environment  
- Azure deployment hints  

---

# 📘 Documentation

See `/docs` for:

- Architecture diagrams  
- Domain models  
- API guidelines  
- Coding standards  
- Folder structure explanation  

---

# 🤝 Contributions

Contributions are welcome.  
Feel free to open issues, discussions or PRs.

---

# 📜 License

MIT License.





