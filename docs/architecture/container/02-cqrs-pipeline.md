# CQRS Pipeline and Behavioral Patterns Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | ARCH-CQRS-002 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Technical Specification |
| Target Audience | Solution Architects, Technical Leads, Senior Developers, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Quarterly |
| Related Documents | High-Level Architecture (ARCH-HL-001), API Flow Specification (ARCH-API-003) |
| Prerequisites | Understanding of MediatR pattern, Clean Architecture principles |

---

## Executive Summary

This document specifies the implementation of Command Query Responsibility Segregation (CQRS) pattern and MediatR pipeline behaviors within the .NET Enterprise Architecture Template. The CQRS pattern separates read and write operations into distinct models, enabling independent optimization and scaling strategies for each concern. Pipeline behaviors provide a standardized mechanism for implementing cross-cutting concerns including validation, logging, performance monitoring, and transaction management.

**Strategic Business Value:**
- Enhanced maintainability through separation of read and write concerns
- Improved scalability via independent optimization of commands and queries
- Reduced defect rates through centralized validation and error handling
- Operational excellence through comprehensive logging and performance monitoring
- Simplified testing via isolated handlers and reusable behaviors

**Key Technical Capabilities:**
- Mediator pattern implementation using MediatR library
- Extensible pipeline behavior architecture for cross-cutting concerns
- FluentValidation integration for declarative validation rules
- Structured logging integration with correlation tracking
- Performance monitoring with configurable thresholds
- Optional transaction management for data consistency

**Compliance and Standards:**
- Aligned with SOLID principles (Single Responsibility, Open/Closed)
- Implements TOGAF 10 separation of concerns
- Supports ITIL 4 operational practices
- Conforms to ISO/IEC/IEEE 26515:2018 documentation standards

---

## Table of Contents

1. Introduction and Scope
2. CQRS Pattern Fundamentals
3. MediatR Implementation Architecture
4. Command Pattern Specification
5. Query Pattern Specification
6. Pipeline Behavior Architecture
7. Validation Behavior Specification
8. Logging Behavior Specification
9. Performance Monitoring Behavior Specification
10. Transaction Management Behavior Specification
11. Behavior Execution Order and Composition
12. Testing Strategy for CQRS Pipeline
13. Performance Characteristics and Optimization
14. Glossary
15. Recommendations and Next Steps
16. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides comprehensive technical specifications for the implementation of CQRS pattern and pipeline behaviors in the .NET Enterprise Architecture Template. It defines the architectural patterns, implementation guidelines, and operational characteristics necessary for building maintainable, scalable request processing pipelines.

### 1.2. Scope

**In Scope:**
- CQRS pattern definition and implementation guidelines
- Command and query structure specifications
- Handler implementation patterns
- Pipeline behavior architecture and implementation
- Cross-cutting concern implementations (validation, logging, performance, transactions)
- Behavior composition and execution order
- Testing strategies for CQRS components

**Out of Scope:**
- Specific business domain implementations
- Database query optimization techniques
- Infrastructure-specific configurations
- Authentication and authorization implementations (covered in separate specification)
- Event sourcing patterns (separate advanced topic)

### 1.3. Document Audience

This specification addresses multiple technical stakeholder groups:

**Solution Architects:** Pattern selection and architectural governance

**Technical Leads:** Implementation guidance and code review standards

**Senior Developers:** Detailed implementation specifications and examples

**Development Teams:** Handler and behavior implementation patterns

**C-Level Executives:** Business value and operational benefits

### 1.4. Architectural Context

The CQRS pipeline operates within the Application layer of the Clean Architecture, serving as the orchestration mechanism for all business operations. It provides the bridge between the API layer (presentation) and the Domain/Infrastructure layers (business logic and data access).

---

## 2. CQRS Pattern Fundamentals

### 2.1. Pattern Overview

Command Query Responsibility Segregation (CQRS) is an architectural pattern that separates operations that modify system state (commands) from operations that retrieve data (queries).

#### 2.1.1. Core Principles

**Principle 1: Separation of Concerns**

Commands and queries represent fundamentally different concerns:
- Commands encapsulate business intent to change state
- Queries encapsulate data retrieval requirements
- No operation performs both reading and writing

**Principle 2: Independent Optimization**

Commands and queries can be optimized independently:
- Commands focus on transactional consistency and business rule enforcement
- Queries focus on read performance and data shaping
- Different data access strategies can be employed for each

**Principle 3: Clear Intent**

Operation type is explicit in code structure:
- Command names express business operations (CreateProduct, UpdateUserProfile)
- Query names express data retrieval intent (GetProductById, SearchUsers)
- Return types clearly indicate operation outcome

#### 2.1.2. Benefits

**Maintainability Benefits:**
- Reduced complexity through focused responsibilities
- Clear boundaries between read and write logic
- Simplified testing through isolated components
- Enhanced code readability through explicit intent

**Scalability Benefits:**
- Independent scaling of read and write workloads
- Optimization flexibility for each operation type
- Support for read replicas and caching strategies
- Foundation for eventual consistency patterns

**Performance Benefits:**
- Optimized query implementations using Dapper
- Simplified command logic focused on business rules
- Reduced contention through separation
- Efficient resource utilization

### 2.2. Implementation Strategy

#### 2.2.1. MediatR Library Selection

**Rationale for MediatR:**
- Mature, well-tested implementation of Mediator pattern
- Excellent community support and documentation
- Extensible pipeline behavior architecture
- Minimal performance overhead
- Strong integration with dependency injection

**Alternative Considerations:**
- Custom mediator implementation: Higher maintenance burden
- Direct handler invocation: Loss of behavior pipeline benefits
- Service layer pattern: Reduced clarity of intent

#### 2.2.2. Request-Response Model

All operations follow the request-response pattern:

**Request (Command or Query):**
- Immutable data structure
- Contains all parameters required for operation
- Implements `IRequest<TResponse>` interface
- Validated before handler execution

**Response:**
- Strongly-typed result object
- May be success result, error result, or domain object
- Serializable for API responses
- Mapped to DTOs when crossing layer boundaries

**Handler:**
- Implements `IRequestHandler<TRequest, TResponse>`
- Single responsibility: process one request type
- Orchestrates domain objects and infrastructure services
- Returns structured response

### 2.3. Architectural Positioning

#### 2.3.1. Layer Integration

```
┌────────────────────────────────────────────┐
│         API Layer (Controllers)            │
│  - Receives HTTP requests                  │
│  - Creates command/query objects           │
│  - Sends to MediatR                        │
└────────────────┬───────────────────────────┘
                 │
                 ↓ IMediator.Send()
┌────────────────┴───────────────────────────┐
│         Application Layer                  │
│  ┌──────────────────────────────────────┐  │
│  │    MediatR Pipeline                  │  │
│  │  ┌────────────────────────────────┐  │  │
│  │  │  Pipeline Behaviors            │  │  │
│  │  │  - Validation                  │  │  │
│  │  │  - Logging                     │  │  │
│  │  │  - Performance                 │  │  │
│  │  │  - Transaction                 │  │  │
│  │  └────────────┬───────────────────┘  │  │
│  │               ↓                      │  │
│  │  ┌────────────────────────────────┐  │  │
│  │  │  Handler                       │  │  │
│  │  │  - Orchestrates domain logic   │  │  │
│  │  │  - Coordinates infrastructure  │  │  │
│  │  └────────────┬───────────────────┘  │  │
│  └───────────────┼──────────────────────┘  │
└────────────────┬─┴───────────────────────┬─┘
                 │                         │
                 ↓                         ↓
┌────────────────┴────────┐  ┌────────────┴──────────┐
│     Domain Layer        │  │  Infrastructure Layer │
│  - Entities             │  │  - Repositories       │
│  - Business Rules       │  │  - DbContext          │
│  - Domain Events        │  │  - External Services  │
└─────────────────────────┘  └───────────────────────┘
```

#### 2.3.2. Dependency Flow

Dependencies flow inward toward the domain:
- API layer depends on Application layer (MediatR abstractions)
- Application layer depends on Domain layer (entities, interfaces)
- Infrastructure layer implements Application/Domain interfaces
- No inner layer depends on outer layers

---

## 3. MediatR Implementation Architecture

### 3.1. Core Components

#### 3.1.1. IRequest Interface

The base interface for all commands and queries:

```csharp
public interface IRequest<out TResponse> { }
```

**Characteristics:**
- Covariant response type
- Marker interface for MediatR registration
- No required methods or properties
- Generic type parameter defines handler return type

#### 3.1.2. IRequestHandler Interface

The interface implemented by all handlers:

```csharp
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken);
}
```

**Characteristics:**
- Contravariant request type
- Covariant response type
- Async operation support via Task
- Cancellation token for operation cancellation

#### 3.1.3. IPipelineBehavior Interface

The interface for implementing cross-cutting concerns:

```csharp
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}
```

**Characteristics:**
- Wraps handler execution
- Can execute logic before and after handler
- Chain of responsibility pattern implementation
- Can short-circuit pipeline by not calling next()

### 3.2. Registration and Configuration

#### 3.2.1. Service Registration

MediatR and related services are registered in the dependency injection container:

```csharp
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly);
});

// Register pipeline behaviors in desired order
services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));
    
services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(LoggingBehavior<,>));
    
services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(PerformanceBehavior<,>));
```

**Registration Guidelines:**
- Register MediatR with assembly scanning
- Register behaviors as open generic types
- Order of registration determines execution order
- Use appropriate service lifetime (typically Transient or Scoped)

#### 3.2.2. Assembly Scanning

MediatR automatically discovers:
- All classes implementing IRequest<TResponse>
- All classes implementing IRequestHandler<TRequest, TResponse>
- All classes implementing IPipelineBehavior<TRequest, TResponse>

**Requirements for Discovery:**
- Classes must be public
- Classes must be in scanned assembly
- Classes must have public constructor
- Dependencies resolved via dependency injection

### 3.3. Execution Model

#### 3.3.1. Request Processing Flow

```
1. Controller creates request object
   ↓
2. Controller calls IMediator.Send(request)
   ↓
3. MediatR locates registered handler
   ↓
4. MediatR constructs behavior pipeline
   ↓
5. First behavior receives request
   ↓
6. Behavior executes pre-handler logic
   ↓
7. Behavior calls next() delegate
   ↓
8. Next behavior in chain executes
   ↓
9. Handler executes when all behaviors complete
   ↓
10. Handler returns response
    ↓
11. Behaviors execute post-handler logic (in reverse order)
    ↓
12. Final response returned to controller
```

#### 3.3.2. Error Propagation

Exceptions propagate through the pipeline:
- Exceptions thrown in behaviors bubble up
- Exceptions thrown in handlers bubble through behaviors
- Behaviors can catch and transform exceptions
- Unhandled exceptions reach global exception handler

---

## 4. Command Pattern Specification

### 4.1. Command Definition

Commands represent operations that change system state and encapsulate all data required for the operation.

#### 4.1.1. Command Structure

**Record Type Implementation:**

```csharp
public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    Guid CategoryId) : IRequest<ProductDto>;
```

**Characteristics:**
- Sealed record type for immutability
- All required parameters in primary constructor
- Implements IRequest<TResponse>
- Response type indicates operation result
- Naming convention: {Verb}{Entity}Command

**Design Principles:**
- Immutable after construction
- Complete parameter set (no partial commands)
- Primitive or value object parameters
- No business logic in command itself
- Clear, business-focused naming

#### 4.1.2. Command Naming Conventions

**Standard Command Patterns:**

| Pattern | Example | Purpose |
|---------|---------|---------|
| Create{Entity}Command | CreateProductCommand | Create new entity |
| Update{Entity}Command | UpdateProductCommand | Modify existing entity |
| Delete{Entity}Command | DeleteProductCommand | Remove entity |
| {Action}{Entity}Command | PublishProductCommand | Domain-specific action |
| {Action}{Entities}Command | ImportProductsCommand | Batch operation |

**Naming Guidelines:**
- Start with action verb (Create, Update, Delete, Activate, etc.)
- Include entity name
- Suffix with "Command"
- Use singular for single entity operations
- Use plural for batch operations

#### 4.1.3. Response Types

**Common Response Patterns:**

**DTO Response:**
```csharp
public sealed record CreateProductCommand(...) : IRequest<ProductDto>;
```
- Returns created/modified entity as DTO
- Suitable for API responses
- Includes all relevant entity data

**Result Object Response:**
```csharp
public sealed record UpdateProductCommand(...) : IRequest<Result<ProductDto>>;
```
- Wraps success/failure state
- Includes validation errors
- Supports error handling without exceptions

**Unit Response:**
```csharp
public sealed record DeleteProductCommand(Guid Id) : IRequest<Unit>;
```
- Indicates operation completion without return value
- MediatR Unit type (similar to void for async)
- Suitable for operations not requiring result data

**Identifier Response:**
```csharp
public sealed record CreateProductCommand(...) : IRequest<Guid>;
```
- Returns only identifier of created entity
- Lightweight response
- Client can query for full data if needed

### 4.2. Command Handler Specification

#### 4.2.1. Handler Structure

```csharp
public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Create domain entity
        var product = Product.Create(
            name: request.Name,
            description: request.Description,
            price: Money.Create(request.Price, request.Currency),
            categoryId: request.CategoryId);

        // 2. Persist via repository
        await _repository.AddAsync(product, cancellationToken);

        // 3. Log operation
        _logger.LogInformation(
            "Product created with ID: {ProductId}",
            product.Id);

        // 4. Map to DTO and return
        return _mapper.Map<ProductDto>(product);
    }
}
```

**Handler Characteristics:**
- Sealed class (no inheritance intended)
- Single responsibility (handles one command type)
- Dependencies injected via constructor
- Async operation with cancellation support
- Clear step-by-step implementation

#### 4.2.2. Handler Responsibilities

**Mandatory Responsibilities:**

1. **Domain Object Orchestration:**
   - Create or retrieve domain entities
   - Execute domain operations
   - Enforce business rules through domain model

2. **Infrastructure Coordination:**
   - Use repository abstractions for persistence
   - Coordinate external service calls
   - Manage transaction boundaries (if not using behavior)

3. **Response Construction:**
   - Map domain objects to DTOs
   - Construct response objects
   - Include all required response data

4. **Error Handling:**
   - Allow domain exceptions to propagate
   - Transform infrastructure exceptions as needed
   - Log errors with appropriate context

**Prohibited Activities:**
- Direct database access (must use repositories)
- Business logic implementation (belongs in domain)
- HTTP concerns (belongs in API layer)
- Direct dependency on Infrastructure implementations

#### 4.2.3. Handler Design Patterns

**Pattern 1: Simple CRUD Operation**

```csharp
public async Task<ProductDto> Handle(
    CreateProductCommand request,
    CancellationToken cancellationToken)
{
    var entity = _mapper.Map<Product>(request);
    await _repository.AddAsync(entity, cancellationToken);
    return _mapper.Map<ProductDto>(entity);
}
```

**Pattern 2: Complex Business Logic**

```csharp
public async Task<OrderDto> Handle(
    PlaceOrderCommand request,
    CancellationToken cancellationToken)
{
    // Retrieve aggregate
    var customer = await _customerRepository
        .GetByIdAsync(request.CustomerId, cancellationToken);

    // Execute domain logic
    var order = customer.PlaceOrder(
        request.ProductIds,
        request.ShippingAddress);

    // Persist changes
    await _orderRepository.AddAsync(order, cancellationToken);

    // Domain events will be dispatched automatically

    return _mapper.Map<OrderDto>(order);
}
```

**Pattern 3: External Service Integration**

```csharp
public async Task<PaymentDto> Handle(
    ProcessPaymentCommand request,
    CancellationToken cancellationToken)
{
    // Process payment through external service
    var paymentResult = await _paymentService
        .ProcessAsync(request.Amount, request.PaymentMethod);

    // Create domain entity from result
    var payment = Payment.Create(
        orderId: request.OrderId,
        amount: request.Amount,
        transactionId: paymentResult.TransactionId);

    await _paymentRepository.AddAsync(payment, cancellationToken);

    return _mapper.Map<PaymentDto>(payment);
}
```

### 4.3. Command Validation

#### 4.3.1. FluentValidation Integration

All commands should have corresponding validators:

```csharp
public sealed class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a valid 3-letter ISO code");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID is required");
    }
}
```

**Validation Guidelines:**
- One validator class per command
- Inherits from AbstractValidator<T>
- Fluent rule definition API
- Custom error messages
- Automatic discovery and registration

#### 4.3.2. Validation Execution

Validation occurs automatically in ValidationBehavior:
- Executes before handler
- All registered validators run
- Failures collected and thrown as ValidationException
- Handler never executes if validation fails

---

## 5. Query Pattern Specification

### 5.1. Query Definition

Queries represent operations that retrieve data without modifying system state.

#### 5.1.1. Query Structure

**Simple Query Example:**

```csharp
public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
```

**Complex Query with Filtering:**

```csharp
public sealed record SearchProductsQuery(
    string? SearchTerm,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    int PageNumber,
    int PageSize,
    string? SortBy,
    bool SortDescending) : IRequest<PagedResult<ProductDto>>;
```

**Query Characteristics:**
- Sealed record type
- Immutable parameters
- Implements IRequest<TResponse>
- Nullable parameters for optional filters
- Naming convention: {Verb}{Entity}{Qualifier}Query

#### 5.1.2. Query Naming Conventions

**Standard Query Patterns:**

| Pattern | Example | Purpose |
|---------|---------|---------|
| Get{Entity}ById | GetProductByIdQuery | Retrieve single entity |
| Get{Entities} | GetProductsQuery | Retrieve collection |
| Search{Entities} | SearchProductsQuery | Filtered collection retrieval |
| List{Entities} | ListProductsQuery | Simple list without filters |
| Find{Entities}By{Criteria} | FindProductsByCategory | Specific filter criteria |

#### 5.1.3. Response Types for Queries

**Single Entity Response:**
```csharp
public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
```
- Returns entity DTO or null
- Nullable return type for not found scenarios

**Collection Response:**
```csharp
public sealed record GetProductsQuery() : IRequest<List<ProductDto>>;
```
- Returns list of DTOs
- Empty list for no results

**Paged Response:**
```csharp
public sealed record SearchProductsQuery(...) 
    : IRequest<PagedResult<ProductDto>>;

public sealed record PagedResult<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
```
- Includes pagination metadata
- Standardized across all paged queries

### 5.2. Query Handler Specification

#### 5.2.1. Handler Structure for Simple Queries

```csharp
public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductReadRepository _readRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(
        IProductReadRepository readRepository,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _readRepository
            .GetByIdAsync(request.Id, cancellationToken);

        return product is null
            ? null
            : _mapper.Map<ProductDto>(product);
    }
}
```

#### 5.2.2. Handler Structure for Complex Queries

```csharp
public sealed class SearchProductsQueryHandler
    : IRequestHandler<SearchProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductReadRepository _readRepository;
    private readonly IMapper _mapper;

    public SearchProductsQueryHandler(
        IProductReadRepository readRepository,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        // Build query specification
        var specification = new ProductSearchSpecification(
            searchTerm: request.SearchTerm,
            categoryId: request.CategoryId,
            minPrice: request.MinPrice,
            maxPrice: request.MaxPrice);

        // Execute count query
        var totalCount = await _readRepository
            .CountAsync(specification, cancellationToken);

        // Execute data query with pagination
        var products = await _readRepository
            .ListAsync(
                specification,
                request.PageNumber,
                request.PageSize,
                request.SortBy,
                request.SortDescending,
                cancellationToken);

        // Map and return paged result
        return new PagedResult<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(products),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
```

#### 5.2.3. Query Optimization Strategies

**Strategy 1: Dapper for Read-Only Operations**

```csharp
public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IDbConnection _connection;

    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT 
                Id, Name, Description, Price, Currency, CategoryId
            FROM Products
            WHERE Id = @Id AND IsDeleted = 0";

        return await _connection.QueryFirstOrDefaultAsync<ProductDto>(
            sql,
            new { request.Id });
    }
}
```

**Benefits:**
- Significant performance improvement over EF Core
- Direct SQL control
- Minimal overhead
- Efficient for read-heavy operations

**Strategy 2: Projection with Entity Framework**

```csharp
public async Task<ProductDto?> Handle(
    GetProductByIdQuery request,
    CancellationToken cancellationToken)
{
    return await _dbContext.Products
        .Where(p => p.Id == request.Id)
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price.Amount,
            Currency = p.Price.Currency
        })
        .FirstOrDefaultAsync(cancellationToken);
}
```

**Benefits:**
- No change tracking overhead
- Optimized SQL queries
- Type-safe projections

**Strategy 3: Specification Pattern**

```csharp
public sealed class ProductSearchSpecification : Specification<Product>
{
    public ProductSearchSpecification(
        string? searchTerm,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
            AddCriteria(p => p.Name.Contains(searchTerm));

        if (categoryId.HasValue)
            AddCriteria(p => p.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            AddCriteria(p => p.Price.Amount >= minPrice.Value);

        if (maxPrice.HasValue)
            AddCriteria(p => p.Price.Amount <= maxPrice.Value);

        AddCriteria(p => !p.IsDeleted);
    }
}
```

**Benefits:**
- Reusable query logic
- Composable criteria
- Testable specifications
- Clear business intent

### 5.3. Query Performance Considerations

#### 5.3.1. Caching Strategy

**Caching Decorator Pattern:**

```csharp
public sealed class CachedProductQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IRequestHandler<GetProductByIdQuery, ProductDto?> _inner;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"product:{request.Id}";
        
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
            return JsonSerializer.Deserialize<ProductDto>(cached);

        var result = await _inner.Handle(request, cancellationToken);

        if (result is not null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                },
                cancellationToken);
        }

        return result;
    }
}
```

#### 5.3.2. Database Query Optimization

**Optimization Guidelines:**
- Use AsNoTracking() for read-only EF Core queries
- Project to DTOs directly in LINQ queries
- Use Dapper for simple, high-performance queries
- Implement proper database indexes
- Monitor query execution plans
- Use pagination for large result sets
- Avoid N+1 query problems with Include()

---

## 6. Pipeline Behavior Architecture

### 6.1. Behavior Fundamentals

Pipeline behaviors provide a mechanism to implement cross-cutting concerns that apply to all or most requests without modifying individual handlers.

#### 6.1.1. Behavior Execution Model

```
Request → [Behavior 1] → [Behavior 2] → [Behavior 3] → Handler → Response
           ↓               ↓               ↓
        Pre-logic      Pre-logic      Pre-logic
           ↑               ↑               ↑
        Post-logic     Post-logic     Post-logic
```

**Execution Characteristics:**
- Behaviors execute in registration order
- Each behavior can execute logic before calling next()
- Each behavior can execute logic after next() returns
- Behaviors form a chain of responsibility
- Any behavior can short-circuit the pipeline

#### 6.1.2. Behavior Base Structure

```csharp
public class CustomBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Pre-handler logic
        // Execute before handler

        var response = await next(); // Execute next behavior or handler

        // Post-handler logic
        // Execute after handler

        return response;
    }
}
```

### 6.2. Behavior Registration and Configuration

#### 6.2.1. Registration Patterns

**Global Behavior (applies to all requests):**

```csharp
services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(LoggingBehavior<,>));
```

**Conditional Behavior (applies to specific requests):**

```csharp
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(); // Skip if no validators

        // Execute validation logic
    }
}
```

#### 6.2.2. Execution Order Configuration

Behaviors execute in the order they are registered:

```csharp
// Recommended order
services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));           // 1. Validate first

services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(AuthorizationBehavior<,>));        // 2. Authorize after validation

services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(LoggingBehavior<,>));              // 3. Log the operation

services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(PerformanceBehavior<,>));          // 4. Monitor performance

services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(TransactionBehavior<,>));          // 5. Manage transactions last
```

---

## 7. Validation Behavior Specification

### 7.1. Purpose and Scope

The Validation Behavior ensures that all incoming commands and queries are validated against defined rules before reaching the handler, providing a centralized validation mechanism.

### 7.2. Implementation Specification

#### 7.2.1. Complete Implementation

```csharp
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Skip if no validators registered
        if (!_validators.Any())
        {
            _logger.LogDebug(
                "No validators found for {RequestName}",
                requestName);
            return await next();
        }

        _logger.LogDebug(
            "Validating {RequestName} with {ValidatorCount} validators",
            requestName,
            _validators.Count());

        // Create validation context
        var context = new ValidationContext<TRequest>(request);

        // Execute all validators in parallel
        var validationResults = await Task.WhenAll(
            _validators.Select(v => 
                v.ValidateAsync(context, cancellationToken)));

        // Collect all failures
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Any())
        {
            _logger.LogWarning(
                "Validation failed for {RequestName} with {ErrorCount} errors",
                requestName,
                failures.Count);

            throw new ValidationException(failures);
        }

        _logger.LogDebug(
            "Validation succeeded for {RequestName}",
            requestName);

        return await next();
    }
}
```

### 7.3. Validation Exception Handling

#### 7.3.1. ValidationException Structure

```csharp
public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(
        IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures occurred.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
```

#### 7.3.2. API Layer Exception Handling

```csharp
public sealed class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ValidationException validationException)
            return;

        var problemDetails = new ValidationProblemDetails(
            validationException.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new BadRequestObjectResult(problemDetails);
        context.ExceptionHandled = true;
    }
}
```

### 7.4. Validation Rules Best Practices

#### 7.4.1. Rule Categories

**Format Validation:**
- Email format
- URL format
- Phone number format
- Date/time format

**Range Validation:**
- Numeric ranges (min/max)
- String length limits
- Collection size limits

**Business Rule Validation:**
- Unique constraints
- Cross-field dependencies
- State transitions
- Referential integrity

**Async Validation:**
- Database existence checks
- External service validation
- Complex business rules requiring data access

#### 7.4.2. Validator Example with Multiple Rules

```csharp
public sealed class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    private readonly IProductRepository _repository;

    public UpdateProductCommandValidator(IProductRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .MustAsync(ProductExists)
            .WithMessage("Product not found");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .When(x => x.Name is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero")
            .When(x => x.Price.HasValue);

        RuleFor(x => x)
            .MustAsync(NameIsUnique)
            .WithMessage("Product name must be unique")
            .When(x => x.Name is not null);
    }

    private async Task<bool> ProductExists(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    private async Task<bool> NameIsUnique(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        return !await _repository.ExistsWithNameAsync(
            command.Name!,
            excludeId: command.Id,
            cancellationToken);
    }
}
```

---

## 8. Logging Behavior Specification

### 8.1. Purpose and Scope

The Logging Behavior provides structured, consistent logging for all requests flowing through the MediatR pipeline, enabling comprehensive audit trails and operational diagnostics.

### 8.2. Implementation Specification

#### 8.2.1. Complete Implementation

```csharp
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId ?? "Anonymous";
        var requestId = Guid.NewGuid();

        _logger.LogInformation(
            "Handling {RequestName} for user {UserId} with RequestId {RequestId}",
            requestName,
            userId,
            requestId);

        try
        {
            var response = await next();

            _logger.LogInformation(
                "Handled {RequestName} successfully for user {UserId} with RequestId {RequestId}",
                requestName,
                userId,
                requestId);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling {RequestName} for user {UserId} with RequestId {RequestId}",
                requestName,
                userId,
                requestId);
            throw;
        }
    }
}
```

### 8.3. Structured Logging Practices

#### 8.3.1. Log Enrichment

**Correlation ID Enrichment:**

```csharp
using Serilog.Context;

public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
{
    var correlationId = _httpContextAccessor.HttpContext?
        .Request.Headers["X-Correlation-ID"].FirstOrDefault()
        ?? Guid.NewGuid().ToString();

    using (LogContext.PushProperty("CorrelationId", correlationId))
    {
        return await next();
    }
}
```

**Request Data Enrichment:**

```csharp
_logger.LogInformation(
    "Processing {RequestName} {@RequestData}",
    requestName,
    new 
    { 
        UserId = userId,
        RequestId = requestId,
        RequestType = typeof(TRequest).FullName,
        Timestamp = DateTime.UtcNow
    });
```

#### 8.3.2. Sensitive Data Protection

**Implementation Guidelines:**
- Never log passwords, tokens, or API keys
- Redact personal identifiable information (PII) when required
- Use structured logging to control serialization
- Implement custom serialization for sensitive types

**Example with Sanitization:**

```csharp
private object SanitizeRequest(TRequest request)
{
    var properties = typeof(TRequest).GetProperties();
    var sanitized = new Dictionary<string, object?>();

    foreach (var prop in properties)
    {
        var name = prop.Name;
        var value = prop.GetValue(request);

        if (IsSensitiveProperty(name))
        {
            sanitized[name] = "***REDACTED***";
        }
        else
        {
            sanitized[name] = value;
        }
    }

    return sanitized;
}

private bool IsSensitiveProperty(string propertyName)
{
    return propertyName.Contains("Password", StringComparison.OrdinalIgnoreCase)
        || propertyName.Contains("Token", StringComparison.OrdinalIgnoreCase)
        || propertyName.Contains("Secret", StringComparison.OrdinalIgnoreCase)
        || propertyName.Contains("ApiKey", StringComparison.OrdinalIgnoreCase);
}
```

### 8.4. Log Level Guidelines

#### 8.4.1. Log Level Matrix

| Level | Usage | Example |
|-------|-------|---------|
| Verbose | Detailed diagnostic information | Individual validator execution details |
| Debug | Internal system events | Request parameter values, handler selection |
| Information | General operational messages | Request start/completion, business operations |
| Warning | Unexpected but handled situations | Slow query performance, retry attempts |
| Error | Errors requiring attention | Validation failures, unhandled exceptions |
| Fatal | Critical failures | Application startup failures, unrecoverable errors |

#### 8.4.2. Contextual Logging Examples

**Information Level:**
```csharp
_logger.LogInformation(
    "Product {ProductId} created by user {UserId}",
    productId,
    userId);
```

**Warning Level:**
```csharp
_logger.LogWarning(
    "External service call to {ServiceName} failed, using fallback",
    serviceName);
```

**Error Level:**
```csharp
_logger.LogError(
    exception,
    "Failed to process {RequestName} for user {UserId}",
    requestName,
    userId);
```

---

## 9. Performance Monitoring Behavior Specification

### 9.1. Purpose and Scope

The Performance Monitoring Behavior measures and logs execution time for all requests, identifying slow operations and enabling performance analysis.

### 9.2. Implementation Specification

#### 9.2.1. Complete Implementation

```csharp
public sealed class PerformanceBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;
    private readonly IMetricsCollector _metricsCollector;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        IMetricsCollector metricsCollector)
    {
        _logger = logger;
        _metricsCollector = metricsCollector;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _timer.Start();

        try
        {
            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            // Record metrics
            _metricsCollector.RecordRequestDuration(
                requestName,
                elapsedMilliseconds);

            // Log if exceeds threshold
            if (elapsedMilliseconds > GetThreshold(requestName))
            {
                _logger.LogWarning(
                    "Long running request: {RequestName} took {ElapsedMilliseconds}ms",
                    requestName,
                    elapsedMilliseconds);
            }
            else
            {
                _logger.LogDebug(
                    "{RequestName} completed in {ElapsedMilliseconds}ms",
                    requestName,
                    elapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            _timer.Stop();

            _logger.LogError(
                ex,
                "{RequestName} failed after {ElapsedMilliseconds}ms",
                requestName,
                _timer.ElapsedMilliseconds);

            _metricsCollector.RecordRequestFailure(requestName);

            throw;
        }
    }

    private static long GetThreshold(string requestName)
    {
        // Commands typically take longer than queries
        return requestName.EndsWith("Command") ? 1000 : 500;
    }
}
```

### 9.3. Performance Metrics Collection

#### 9.3.1. Metrics Interface

```csharp
public interface IMetricsCollector
{
    void RecordRequestDuration(string requestName, long durationMs);
    void RecordRequestFailure(string requestName);
    void RecordValidationFailure(string requestName, int errorCount);
}
```

#### 9.3.2. OpenTelemetry Integration

```csharp
public sealed class OpenTelemetryMetricsCollector : IMetricsCollector
{
    private readonly Meter _meter;
    private readonly Histogram<long> _requestDuration;
    private readonly Counter<long> _requestFailures;

    public OpenTelemetryMetricsCollector()
    {
        _meter = new Meter("Application.CQRS");
        _requestDuration = _meter.CreateHistogram<long>(
            "cqrs.request.duration",
            "ms",
            "Duration of CQRS request processing");
        _requestFailures = _meter.CreateCounter<long>(
            "cqrs.request.failures",
            "count",
            "Number of failed CQRS requests");
    }

    public void RecordRequestDuration(string requestName, long durationMs)
    {
        _requestDuration.Record(
            durationMs,
            new KeyValuePair<string, object?>("request.name", requestName));
    }

    public void RecordRequestFailure(string requestName)
    {
        _requestFailures.Add(
            1,
            new KeyValuePair<string, object?>("request.name", requestName));
    }
}
```

### 9.4. Performance Analysis and Alerting

#### 9.4.1. Performance Threshold Configuration

```csharp
public sealed class PerformanceThresholds
{
    public Dictionary<string, long> RequestThresholds { get; init; } = new()
    {
        // Commands
        ["CreateProductCommand"] = 1000,
        ["UpdateProductCommand"] = 800,
        ["DeleteProductCommand"] = 500,
        
        // Queries
        ["GetProductByIdQuery"] = 100,
        ["SearchProductsQuery"] = 500,
        ["GetProductListQuery"] = 300
    };

    public long DefaultCommandThreshold { get; init; } = 1000;
    public long DefaultQueryThreshold { get; init; } = 500;

    public long GetThreshold(string requestName)
    {
        if (RequestThresholds.TryGetValue(requestName, out var threshold))
            return threshold;

        return requestName.EndsWith("Command")
            ? DefaultCommandThreshold
            : DefaultQueryThreshold;
    }
}
```

---

## 10. Transaction Management Behavior Specification

### 10.1. Purpose and Scope

The Transaction Behavior ensures database operations within command handlers execute within a transactional boundary, providing atomicity and consistency.

### 10.2. Implementation Specification

#### 10.2.1. Complete Implementation

```csharp
public sealed class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IDbContext _dbContext;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IDbContext dbContext,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Skip transaction for queries
        if (requestName.EndsWith("Query"))
        {
            return await next();
        }

        // Skip if transaction already exists
        if (_dbContext.HasActiveTransaction)
        {
            _logger.LogDebug(
                "Skipping transaction for {RequestName} - transaction already active",
                requestName);
            return await next();
        }

        _logger.LogDebug(
            "Beginning transaction for {RequestName}",
            requestName);

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext
                .BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();

                await _dbContext.SaveChangesAsync(cancellationToken);
                await _dbContext.CommitTransactionAsync(transaction);

                _logger.LogDebug(
                    "Transaction committed for {RequestName}",
                    requestName);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Transaction rollback for {RequestName}",
                    requestName);

                await _dbContext.RollbackTransactionAsync(transaction);
                throw;
            }
        });
    }
}
```

### 10.3. Transaction Isolation Levels

#### 10.3.1. Isolation Level Configuration

```csharp
public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
{
    var isolationLevel = GetIsolationLevel(typeof(TRequest));

    await using var transaction = await _dbContext
        .BeginTransactionAsync(isolationLevel, cancellationToken);

    // ... rest of implementation
}

private IsolationLevel GetIsolationLevel(Type requestType)
{
    // Check for attribute
    var attribute = requestType
        .GetCustomAttribute<TransactionIsolationAttribute>();

    return attribute?.IsolationLevel ?? IsolationLevel.ReadCommitted;
}
```

#### 10.3.2. Custom Transaction Attribute

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class TransactionIsolationAttribute : Attribute
{
    public IsolationLevel IsolationLevel { get; }

    public TransactionIsolationAttribute(IsolationLevel isolationLevel)
    {
        IsolationLevel = isolationLevel;
    }
}

// Usage
[TransactionIsolation(IsolationLevel.Serializable)]
public sealed record ProcessPaymentCommand(...) : IRequest<PaymentResult>;
```

### 10.4. Distributed Transaction Considerations

#### 10.4.1. Two-Phase Commit Pattern

For operations spanning multiple databases or external systems:

```csharp
public sealed class DistributedTransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ITransactionCoordinator _coordinator;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!RequiresDistributedTransaction(typeof(TRequest)))
            return await next();

        using var scope = new TransactionScope(
            TransactionScopeAsyncFlowOption.Enabled);

        var response = await next();

        scope.Complete();

        return response;
    }
}
```

---

## 11. Behavior Execution Order and Composition

### 11.1. Recommended Execution Sequence

The order of behavior registration determines execution sequence. The recommended order optimizes for fail-fast principles and operational efficiency:

```
1. ValidationBehavior
   ↓ (reject invalid requests immediately)
2. AuthorizationBehavior (optional)
   ↓ (verify permissions before processing)
3. LoggingBehavior
   ↓ (log all valid, authorized requests)
4. PerformanceBehavior
   ↓ (monitor execution time)
5. TransactionBehavior
   ↓ (manage database transactions)
6. Handler Execution
   ↓
7. Behaviors execute post-handler logic (reverse order)
```

### 11.2. Execution Flow Diagram

```
┌─────────────────────────────────────────────┐
│          API Controller                     │
│  Creates Command/Query                      │
└────────────────┬────────────────────────────┘
                 │
                 ↓ IMediator.Send()
┌────────────────┴────────────────────────────┐
│  ValidationBehavior                         │
│  - Execute FluentValidation rules           │
│  - Throw ValidationException if invalid     │
│  - Short-circuit if validation fails        │
└────────────────┬────────────────────────────┘
                 │ ✓ Valid
                 ↓
┌────────────────┴────────────────────────────┐
│  AuthorizationBehavior (Optional)           │
│  - Check user permissions                   │
│  - Throw UnauthorizedException if denied    │
│  - Short-circuit if unauthorized            │
└────────────────┬────────────────────────────┘
                 │ ✓ Authorized
                 ↓
┌────────────────┴────────────────────────────┐
│  LoggingBehavior                            │
│  PRE: Log request initiation                │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌────────────────┴────────────────────────────┐
│  PerformanceBehavior                        │
│  PRE: Start timer                           │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌────────────────┴────────────────────────────┐
│  TransactionBehavior                        │
│  PRE: Begin transaction                     │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌────────────────┴────────────────────────────┐
│  Handler                                    │
│  - Execute business logic                   │
│  - Interact with domain                     │
│  - Use infrastructure services              │
└────────────────┬────────────────────────────┘
                 │
                 ↓ Response
┌────────────────┴────────────────────────────┐
│  TransactionBehavior                        │
│  POST: Commit/Rollback transaction          │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌────────────────┴────────────────────────────┐
│  PerformanceBehavior                        │
│  POST: Stop timer, log duration             │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌────────────────┴────────────────────────────┐
│  LoggingBehavior                            │
│  POST: Log request completion               │
└────────────────┬────────────────────────────┘
                 │
                 ↓ Response
┌────────────────┴────────────────────────────┐
│  API Controller                             │
│  Returns HTTP response                      │
└─────────────────────────────────────────────┘
```

### 11.3. Conditional Behavior Execution

#### 11.3.1. Skip Logic Implementation

```csharp
public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
{
    // Skip logic for specific conditions
    if (ShouldSkip(request))
    {
        return await next();
    }

    // Execute behavior logic
    // ...
}

private bool ShouldSkip(TRequest request)
{
    // Skip for queries in transaction behavior
    if (typeof(TRequest).Name.EndsWith("Query"))
        return true;

    // Skip if no validators in validation behavior
    if (!_validators.Any())
        return true;

    return false;
}
```

---

## 12. Testing Strategy for CQRS Pipeline

### 12.1. Unit Testing Approach

#### 12.1.1. Handler Testing

```csharp
public sealed class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateProductCommandHandler(
            _repositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesProduct()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: "Test Description",
            Price: 99.99m,
            Currency: "USD",
            CategoryId: Guid.NewGuid());

        var expectedDto = new ProductDto { Id = Guid.NewGuid() };

        _mapperMock
            .Setup(x => x.Map<ProductDto>(It.IsAny<Product>()))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        
        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var command = new CreateProductCommand(...);
        
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
```

#### 12.1.2. Behavior Testing

```csharp
public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestCommand>>();
        var logger = Mock.Of<ILogger<ValidationBehavior<TestCommand, Unit>>>();
        var behavior = new ValidationBehavior<TestCommand, Unit>(validators, logger);

        var called = false;
        Task<Unit> Next() 
        {
            called = true;
            return Task.FromResult(Unit.Value);
        }

        // Act
        await behavior.Handle(new TestCommand(), Next, CancellationToken.None);

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidationFails_ThrowsValidationException()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(x => x.ValidateAsync(
                It.IsAny<ValidationContext<TestCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Property", "Error message")
            }));

        var validators = new[] { validatorMock.Object };
        var logger = Mock.Of<ILogger<ValidationBehavior<TestCommand, Unit>>>();
        var behavior = new ValidationBehavior<TestCommand, Unit>(validators, logger);

        Task<Unit> Next() => Task.FromResult(Unit.Value);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(new TestCommand(), Next, CancellationToken.None));
    }
}
```

### 12.2. Integration Testing Approach

#### 12.2.1. Full Pipeline Testing

```csharp
public sealed class CreateProductCommandIntegrationTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CreateProductCommandIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ValidCommand_ReturnsCreatedProduct()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Integration Test Product",
            Description: "Test",
            Price: 99.99m,
            Currency: "USD",
            CategoryId: Guid.NewGuid());

        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<ProductDto>();
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task CreateProduct_InvalidCommand_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "", // Invalid: empty name
            Description: "Test",
            Price: -10m, // Invalid: negative price
            Currency: "USD",
            CategoryId: Guid.Empty); // Invalid: empty GUID

        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var problemDetails = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().ContainKey("Name");
        problemDetails.Errors.Should().ContainKey("Price");
    }
}
```

### 12.3. Performance Testing

#### 12.3.1. Benchmark Testing

```csharp
[MemoryDiagnoser]
public class CommandHandlerBenchmarks
{
    private IMediator _mediator;
    private CreateProductCommand _command;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        // Configure services
        var provider = services.BuildServiceProvider();
        _mediator = provider.GetRequiredService<IMediator>();
        
        _command = new CreateProductCommand(...);
    }

    [Benchmark]
    public async Task<ProductDto> HandleCommand()
    {
        return await _mediator.Send(_command);
    }
}
```

---

## 13. Performance Characteristics and Optimization

### 13.1. Performance Metrics

#### 13.1.1. Expected Performance Characteristics

| Operation Type | Target Latency (p95) | Target Throughput |
|----------------|----------------------|-------------------|
| Simple Query | < 50ms | 2000+ req/s |
| Complex Query | < 200ms | 500+ req/s |
| Simple Command | < 100ms | 1000+ req/s |
| Complex Command | < 500ms | 200+ req/s |

#### 13.1.2. Overhead Analysis

**MediatR Pipeline Overhead:**
- Negligible (< 1ms) for typical request processing
- Primarily reflection-based type resolution at startup
- Runtime performance comparable to direct handler invocation

**Behavior Overhead:**
- Validation: 1-10ms depending on rule complexity
- Logging: < 1ms for structured logging
- Performance monitoring: < 1ms
- Transaction: 5-20ms for begin/commit operations

### 13.2. Optimization Strategies

#### 13.2.1. Query Optimization

**Use Dapper for Simple Queries:**
```csharp
// Instead of EF Core
var products = await _dbContext.Products
    .Where(p => p.CategoryId == categoryId)
    .ToListAsync();

// Use Dapper
var products = await _connection.QueryAsync<Product>(
    "SELECT * FROM Products WHERE CategoryId = @CategoryId",
    new { CategoryId = categoryId });
```

**Performance Improvement:** 2-5x faster for simple queries

**Project to DTOs Directly:**
```csharp
// Avoid
var products = await _dbContext.Products.ToListAsync();
return _mapper.Map<List<ProductDto>>(products);

// Optimize
return await _dbContext.Products
    .Select(p => new ProductDto 
    { 
        Id = p.Id, 
        Name = p.Name,
        // ... other properties
    })
    .ToListAsync();
```

**Performance Improvement:** Eliminates mapping overhead and reduces memory allocation

#### 13.2.2. Command Optimization

**Batch Operations:**
```csharp
// Instead of multiple commands
foreach (var item in items)
{
    await _mediator.Send(new CreateItemCommand(item));
}

// Use batch command
await _mediator.Send(new CreateItemsBatchCommand(items));
```

**Async Operations for Independent Work:**
```csharp
// Execute independent operations in parallel
await Task.WhenAll(
    _emailService.SendAsync(email),
    _notificationService.NotifyAsync(notification),
    _auditService.LogAsync(auditEntry));
```

#### 13.2.3. Caching Strategy

**Response Caching for Queries:**
```csharp
public sealed class CachingQueryBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheable
{
    private readonly IDistributedCache _cache;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var cacheKey = request.GetCacheKey();
        
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<TResponse>(cached)!;
        }

        var response = await next();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = request.CacheDuration
            },
            cancellationToken);

        return response;
    }
}
```

---

## 14. Glossary

**Behavior:** A component in the MediatR pipeline that implements cross-cutting concerns by wrapping handler execution.

**Command:** An object representing an intent to change system state, implementing IRequest<TResponse>.

**CQRS (Command Query Responsibility Segregation):** An architectural pattern that separates read operations (queries) from write operations (commands).

**FluentValidation:** A .NET library for building strongly-typed validation rules using a fluent interface.

**Handler:** A component that processes a specific command or query, implementing IRequestHandler<TRequest, TResponse>.

**MediatR:** A .NET library implementing the Mediator pattern for in-process messaging and request handling.

**Mediator Pattern:** A behavioral design pattern that reduces coupling between components by centralizing their communication through a mediator object.

**Pipeline Behavior:** Implementation of IPipelineBehavior<TRequest, TResponse> that executes logic before and/or after handler execution.

**Query:** An object representing a request for data retrieval without state modification, implementing IRequest<TResponse>.

**Request:** Generic term for command or query object sent through MediatR pipeline.

**Response:** The result returned from a handler after processing a request.

**Specification:** A pattern that encapsulates business rules or query criteria in reusable, composable objects.

**Transaction Boundary:** The scope within which database operations are treated as an atomic unit.

**Validation Exception:** An exception thrown when validation rules fail, containing structured error information.

**Vertical Slice:** A complete feature implementation cutting across all architectural layers from API to infrastructure.

---

## 15. Recommendations and Next Steps

### 15.1. For Development Teams

#### 15.1.1. Implementation Checklist

**When Creating New Commands:**
- [ ] Create command record with all required parameters
- [ ] Implement IRequest<TResponse> with appropriate response type
- [ ] Create command handler implementing IRequestHandler
- [ ] Create FluentValidation validator
- [ ] Write unit tests for handler
- [ ] Write integration tests for complete flow
- [ ] Document any special considerations

**When Creating New Queries:**
- [ ] Create query record with filter parameters
- [ ] Implement IRequest<TResponse> with DTO response type
- [ ] Create query handler with optimized data access
- [ ] Consider using Dapper for simple queries
- [ ] Implement pagination for collection queries
- [ ] Create validator if query has complex parameters
- [ ] Write unit tests for handler
- [ ] Optimize database queries and indexes

**When Creating New Behaviors:**
- [ ] Identify cross-cutting concern requiring standardization
- [ ] Implement IPipelineBehavior<TRequest, TResponse>
- [ ] Consider conditional execution logic
- [ ] Determine appropriate execution order
- [ ] Write unit tests for behavior
- [ ] Document behavior purpose and configuration
- [ ] Register in dependency injection with correct order

#### 15.1.2. Common Patterns and Anti-Patterns

**Recommended Patterns:**
- One handler per command/query (Single Responsibility)
- Immutable command/query objects
- Clear naming conventions
- Structured validation rules
- DTO mapping at handler boundary
- Repository abstraction for data access

**Anti-Patterns to Avoid:**
- Business logic in command/query objects
- Direct database access in handlers
- Fat handlers with multiple responsibilities
- Skipping validation for "simple" operations
- Ignoring cancellation tokens
- Mixing commands and queries in single operation

### 15.2. For Technical Leads

#### 15.2.1. Code Review Focus Areas

**Command/Query Review:**
- Immutability verification
- Appropriate response type selection
- Clear, business-focused naming
- Complete parameter sets

**Handler Review:**
- Single responsibility adherence
- Proper use of repository abstractions
- Domain logic in domain layer, not handler
- Appropriate error handling
- Cancellation token usage
- Async/await best practices

**Behavior Review:**
- Clear purpose and necessity
- Appropriate execution order
- Conditional execution logic
- Performance impact consideration
- Comprehensive error handling

#### 15.2.2. Performance Monitoring

**Key Metrics to Track:**
- Request duration by command/query type
- Validation failure rates
- Database query performance
- Memory allocation per request
- Exception rates by request type

**Alerting Thresholds:**
- p95 latency exceeding 2x target
- Error rate exceeding 1%
- Validation failure rate exceeding 10%
- Memory growth trends

### 15.3. For Architects

#### 15.3.1. Architecture Evolution

**Short-Term Considerations:**
- Monitor behavior execution overhead
- Evaluate query performance patterns
- Assess validation rule complexity
- Review handler responsibility boundaries

**Long-Term Considerations:**
- Event-driven architecture integration
- Read/write database separation for CQRS
- Message bus integration for commands
- Event sourcing evaluation
- Distributed transaction requirements

#### 15.3.2. Governance and Standards

**Establish Team Standards:**
- Command/query naming conventions
- Response type patterns
- Validation rule organization
- Error handling patterns
- Testing requirements

**Enforcement Mechanisms:**
- Code review checklists
- Automated architecture tests
- Static analysis rules
- Template generators

### 15.4. Related Documentation

**Must Read:**
- High-Level Architecture (ARCH-HL-001)
- API Flow Specification (ARCH-API-003)
- Domain Modeling Guide (ARCH-DOMAIN-004)

**Recommended Reading:**
- Testing Strategy Document
- Performance Optimization Guide
- Security Implementation Guide
- Deployment Architecture Specification

---

## 16. References

### 16.1. Standards and Frameworks

**ISO/IEC/IEEE 26515:2018**
- Developing Information for Users
- Software and System Documentation

**TOGAF 10**
- Architecture patterns and governance
- Separation of concerns principles

**ITIL 4**
- Service management practices
- Operational excellence

### 16.2. Pattern References

**CQRS Pattern:**
- Martin Fowler: CQRS - https://martinfowler.com/bliki/CQRS.html
- Greg Young: CQRS Documents - https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf

**Mediator Pattern:**
- Gang of Four Design Patterns
- Martin Fowler: Patterns of Enterprise Application Architecture

**Specification Pattern:**
- Eric Evans: Domain-Driven Design
- Martin Fowler: Specification Pattern

### 16.3. Technology Documentation

**MediatR:**
- GitHub Repository: https://github.com/jbogard/MediatR
- Documentation: https://github.com/jbogard/MediatR/wiki

**FluentValidation:**
- Official Documentation: https://fluentvalidation.net/
- GitHub Repository: https://github.com/FluentValidation/FluentValidation

**Serilog:**
- Official Website: https://serilog.net/
- Structured Logging: https://nblumhardt.com/2016/06/structured-logging-concepts/

### 16.4. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- ARCH-HL-001: High-Level Architecture
- ARCH-API-003: API Flow Specification
- ARCH-DOMAIN-004: Domain Modeling Guide

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial CQRS pipeline and behaviors specification with standardized structure |

---

**END OF DOCUMENT**