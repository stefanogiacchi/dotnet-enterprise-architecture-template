# Command Query Responsibility Segregation (CQRS) Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | ARCH-CQRS-003 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Architectural Specification |
| Target Audience | Solution Architects, Development Teams, Technical Leads, Domain Experts, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Annual |
| Related Documents | High-Level Architecture (ARCH-HL-001), CQRS Pipeline (ARCH-CQRS-002), Domain Modeling (ARCH-DOMAIN-004), API Guidelines (API-GUIDELINES-001) |
| Prerequisites | Understanding of CQRS pattern, Domain-Driven Design, MediatR framework |

---

## Executive Summary

This document establishes comprehensive Command Query Responsibility Segregation (CQRS) specifications for the .NET Enterprise Architecture Template, implementing a rigorous separation between operations that modify state (Commands) and operations that retrieve data (Queries). This architectural pattern provides the foundation for building scalable, maintainable, and high-performance enterprise applications with clear boundaries between read and write operations.

**Strategic Business Value:**
- Enhanced system scalability through independent read/write optimization
- Reduced total cost of ownership via simplified maintenance and troubleshooting
- Improved system performance through specialized read and write models
- Increased development velocity via clear separation of concerns
- Better alignment with business operations through explicit command modeling
- Enhanced audit capabilities through command-centric tracking

**Key Technical Capabilities:**
- Strict separation between state-modifying Commands and data-retrieving Queries
- Independent optimization of read and write operations
- MediatR-based implementation with pipeline behaviors
- Domain event integration for Commands
- Flexible query optimization strategies (Dapper, EF Core projections)
- Clear mapping between HTTP methods and CQRS operations
- Comprehensive validation and error handling patterns

**Compliance and Standards:**
- CQRS pattern (Bertrand Meyer, Greg Young)
- Domain-Driven Design tactical patterns (Eric Evans)
- Single Responsibility Principle (SOLID)
- Command-Query Separation principle (Bertrand Meyer)
- REST architectural constraints for API mapping
- ISO/IEC 25010 software quality characteristics

---

## Table of Contents

1. Introduction and Scope
2. CQRS Foundational Principles
3. Command Specification
4. Query Specification
5. Command and Query Distinction
6. HTTP Method Mapping
7. Request Processing Flow
8. Implementation Patterns
9. Performance Optimization Strategies
10. Testing Strategy
11. Project Structure and Organization
12. Anti-Patterns and Pitfalls
13. Scalability Considerations
14. Glossary
15. Recommendations and Next Steps
16. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides authoritative specifications for implementing Command Query Responsibility Segregation (CQRS) within the .NET Enterprise Architecture Template. It defines the strict separation between Commands (write operations) and Queries (read operations), implementation patterns, architectural constraints, and best practices ensuring maintainable, scalable, and performant enterprise applications.

### 1.2. Scope

**In Scope:**
- Command specification and implementation patterns
- Query specification and implementation patterns
- Command/Query distinction rules and enforcement
- HTTP method mapping to CQRS operations
- MediatR integration patterns
- Validation strategies for Commands and Queries
- Performance optimization approaches
- Testing strategies for CQRS operations
- Project organization standards
- Anti-pattern identification and avoidance

**Out of Scope:**
- Event Sourcing implementations (separate specification)
- CQRS with separate read/write databases (advanced pattern)
- Saga pattern implementations
- Distributed transaction coordination
- Message broker configurations
- Specific business domain logic
- Infrastructure deployment patterns

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**Solution Architects:** Architectural patterns and system design decisions

**Development Teams:** Implementation guidelines and code examples

**Technical Leads:** Code review standards and pattern enforcement

**Domain Experts:** Command modeling and business operation representation

**QA Engineers:** Testing strategies and validation approaches

**C-Level Executives:** Business value and operational benefits

### 1.4. CQRS Context in Architecture

CQRS operates within the Application layer, mediating between API and Domain layers:

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer                            │
│  - HTTP Endpoints (Controllers)                         │
│  - Request/Response DTOs                                │
│  - Status Code Selection                                │
└────────────────┬────────────────────────────────────────┘
                 │
                 ↓ IMediator.Send()
┌────────────────┴────────────────────────────────────────┐
│              Application Layer (CQRS)                   │
│                                                         │
│  ┌──────────────────────┐   ┌──────────────────────┐   │
│  │   Commands           │   │   Queries            │   │
│  │  (Write Model)       │   │  (Read Model)        │   │
│  │  • CreateProduct     │   │  • GetProducts       │   │
│  │  • UpdateProduct     │   │  • GetProductById    │   │
│  │  • DeleteProduct     │   │  • SearchProducts    │   │
│  └──────────┬───────────┘   └──────────┬───────────┘   │
│             │                           │               │
│             ↓                           ↓               │
│  ┌──────────┴───────────┐   ┌──────────┴───────────┐   │
│  │  Command Handlers    │   │  Query Handlers      │   │
│  │  • Domain Logic      │   │  • Data Retrieval    │   │
│  │  • Invariants        │   │  • Projections       │   │
│  │  • Domain Events     │   │  • Performance       │   │
│  └──────────┬───────────┘   └──────────┬───────────┘   │
└─────────────┼──────────────────────────┼───────────────┘
              │                          │
              ↓                          ↓
┌─────────────┴──────────────────────────┴───────────────┐
│          Domain & Infrastructure Layers                │
│  - Domain Entities & Aggregates                        │
│  - Repositories                                        │
│  - Database Context                                    │
└─────────────────────────────────────────────────────────┘
```

---

## 2. CQRS Foundational Principles

### 2.1. Core Principles

#### 2.1.1. Command-Query Separation

**Principle:** Operations that modify state (Commands) are strictly separated from operations that read state (Queries).

**Origin:** Bertrand Meyer's Command-Query Separation principle

**Application:**
- Commands change system state but do not return domain data
- Queries return data but do not change system state
- No operation performs both functions

**Benefits:**
- Clearer intent and semantics
- Easier reasoning about system behavior
- Simplified testing
- Independent optimization

#### 2.1.2. Single Responsibility per Operation

**Principle:** Each Command or Query has exactly one responsibility and one reason to change.

**Implementation:**
- One Command class per business operation
- One Query class per data retrieval need
- One Handler per Command/Query
- No shared handlers

**Benefits:**
- High cohesion
- Low coupling
- Simplified maintenance
- Clear ownership

#### 2.1.3. Explicit Intent

**Principle:** Commands explicitly represent business intent through naming and structure.

**Command Naming:**
- Imperative verb + noun: CreateProduct, UpdateOrder, CancelPayment
- Reflects business terminology
- Avoids technical jargon

**Benefits:**
- Ubiquitous language alignment
- Self-documenting code
- Clear audit trails
- Business operation traceability

#### 2.1.4. Independent Scalability

**Principle:** Read and write operations can be optimized and scaled independently.

**Scaling Strategies:**

**Commands (Write Model):**
- Vertical scaling for consistency guarantees
- Optimized for correctness and data integrity
- Transaction management priority
- Domain rule enforcement

**Queries (Read Model):**
- Horizontal scaling for throughput
- Optimized for performance and flexibility
- Caching strategies
- Denormalization acceptable
- Read replicas

### 2.2. CQRS vs Traditional Architecture

#### 2.2.1. Comparison Matrix

| Aspect | Traditional CRUD | CQRS |
|--------|------------------|------|
| Read/Write Model | Single unified model | Separate models |
| Optimization | Compromise between read/write | Independent optimization |
| Complexity | Lower initial | Higher initial, lower long-term |
| Scalability | Limited | High (independent scaling) |
| Performance | Moderate | High (specialized) |
| Intent Expression | Implicit (CRUD) | Explicit (Commands) |
| Domain Alignment | Weak | Strong |

---

## 3. Command Specification

### 3.1. Command Definition

#### 3.1.1. Command Characteristics

A Command:

**Represents Intent to Change State:**
- Expresses business operation
- Encapsulates operation parameters
- Carries user context

**Triggers Domain Logic:**
- Enforces business rules
- Validates invariants
- Applies domain constraints

**Produces Side Effects:**
- Modifies database state
- Raises domain events
- Triggers integrations

**Returns Minimal Data:**
- Success/failure indicator
- Created resource identifier
- Operation result metadata

### 3.2. Command Structure

#### 3.2.1. Command Definition Pattern

**Record-Based Command (Recommended):**
```csharp
public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Currency) : IRequest<Guid>;
```

**Benefits:**
- Immutability by default
- Value equality semantics
- Concise syntax
- Clear intent

#### 3.2.2. Command with Complex Data

```csharp
public sealed record CreateOrderCommand : IRequest<OrderResult>
{
    public Guid CustomerId { get; init; }
    public Address ShippingAddress { get; init; }
    public IReadOnlyList<OrderLineItem> Items { get; init; }
    public string PaymentMethod { get; init; }
}

public sealed record OrderLineItem(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice);

public sealed record OrderResult(
    Guid OrderId,
    string OrderNumber,
    decimal TotalAmount);
```

### 3.3. Command Handler Implementation

#### 3.3.1. Standard Handler Pattern

```csharp
public sealed class CreateProductCommandHandler 
    : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating product: {Name}",
            command.Name);

        // Create domain entity with business rules
        var product = Product.Create(
            ProductId.New(),
            command.Name,
            command.Description,
            Money.Create(command.Price, command.Currency),
            ProductCategory.Electronics);

        // Persist
        await _repository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await PublishDomainEventsAsync(product, cancellationToken);

        _logger.LogInformation(
            "Product created: {ProductId}",
            product.Id);

        return product.Id.Value;
    }

    private async Task PublishDomainEventsAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        foreach (var domainEvent in product.DomainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        product.ClearDomainEvents();
    }
}
```

### 3.4. Command Return Types

#### 3.4.1. Recommended Return Types

| Return Type | Use Case | Example |
|-------------|----------|---------|
| void | Operation with no return value | DeleteProductCommand |
| Guid | Created entity identifier | CreateProductCommand |
| Unit | MediatR void equivalent | ArchiveProductCommand |
| Result<T> | Success/failure with data | UpdateProductCommand |
| Custom Result Type | Complex operation result | ProcessOrderCommand → OrderResult |

#### 3.4.2. Anti-Pattern: Returning Domain Entities

**Incorrect:**
```csharp
public sealed record CreateProductCommand : IRequest<Product>; // WRONG
```

**Correct:**
```csharp
public sealed record CreateProductCommand : IRequest<Guid>; // RIGHT
```

**Rationale:**
- Commands should not return domain entities
- Separation of concerns violated
- Domain details leaked to API layer
- Use Query to retrieve entity after creation

### 3.5. Command Validation

#### 3.5.1. FluentValidation Implementation

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
            .WithMessage("Price must be greater than zero")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Price must not exceed 999999.99");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO code")
            .Must(BeValidCurrency)
            .WithMessage("Currency must be a valid ISO 4217 code");
    }

    private bool BeValidCurrency(string currency)
    {
        var validCurrencies = new[] { "USD", "EUR", "GBP", "JPY" };
        return validCurrencies.Contains(currency);
    }
}
```

### 3.6. Command Constraints

#### 3.6.1. Mandatory Rules

**Commands MUST:**
- Represent business operations explicitly
- Trigger domain logic validation
- Use imperative naming (verb + noun)
- Return minimal data (ID, result, void)
- Raise domain events for significant changes
- Be handled by exactly one handler

**Commands MUST NOT:**
- Return domain entities directly
- Perform complex queries
- Return paginated collections
- Skip validation
- Bypass domain logic
- Query data without state modification intent

---

## 4. Query Specification

### 4.1. Query Definition

#### 4.1.1. Query Characteristics

A Query:

**Retrieves Data:**
- Reads database state
- Returns data transfer objects (DTOs)
- Supports filtering, sorting, pagination

**Does Not Modify State:**
- No database writes
- No domain event raising
- No side effects

**Optimizes for Performance:**
- Uses efficient data access
- Applies projections
- Leverages caching
- Uses read replicas

**Returns Structured Data:**
- DTOs tailored to client needs
- Paginated results
- Aggregated data
- Projected views

### 4.2. Query Structure

#### 4.2.1. Simple Query Pattern

```csharp
public sealed record GetProductByIdQuery(Guid Id) 
    : IRequest<ProductResponse>;
```

#### 4.2.2. Complex Query with Parameters

```csharp
public sealed record GetProductsQuery : IRequest<PagedResponse<ProductResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public string? Category { get; init; }
    public string? Sort { get; init; }
}
```

### 4.3. Query Handler Implementation

#### 4.3.1. EF Core Projection Handler

```csharp
public sealed class GetProductByIdQueryHandler 
    : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly ApplicationDbContext _context;

    public GetProductByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponse> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == query.Id)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Currency = p.Currency,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
            throw new NotFoundException($"Product {query.Id} not found");

        return product;
    }
}
```

#### 4.3.2. Dapper High-Performance Handler

```csharp
public sealed class GetProductsQueryHandler 
    : IRequestHandler<GetProductsQuery, PagedResponse<ProductResponse>>
{
    private readonly IDbConnection _connection;

    public GetProductsQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<PagedResponse<ProductResponse>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var sql = @"
            SELECT 
                Id, Name, Description, Price, Currency, 
                IsActive, CreatedAt, UpdatedAt
            FROM Products
            WHERE (@Search IS NULL OR Name LIKE @SearchPattern)
              AND (@Category IS NULL OR Category = @Category)
            ORDER BY 
                CASE WHEN @Sort = 'name_asc' THEN Name END ASC,
                CASE WHEN @Sort = 'name_desc' THEN Name END DESC,
                CASE WHEN @Sort = 'price_asc' THEN Price END ASC,
                CASE WHEN @Sort = 'price_desc' THEN Price END DESC,
                CreatedAt DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*) 
            FROM Products
            WHERE (@Search IS NULL OR Name LIKE @SearchPattern)
              AND (@Category IS NULL OR Category = @Category);";

        var parameters = new
        {
            Search = query.Search,
            SearchPattern = query.Search != null ? $"%{query.Search}%" : null,
            Category = query.Category,
            Sort = query.Sort,
            Offset = (query.Page - 1) * query.PageSize,
            PageSize = query.PageSize
        };

        using var multi = await _connection.QueryMultipleAsync(sql, parameters);
        
        var products = await multi.ReadAsync<ProductResponse>();
        var totalCount = await multi.ReadSingleAsync<int>();

        return new PagedResponse<ProductResponse>
        {
            Items = products,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }
}
```

### 4.4. Query Response Types

#### 4.4.1. Single Entity Response

```csharp
public sealed record ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
```

#### 4.4.2. Collection Response

```csharp
public sealed record PagedResponse<T>
{
    public IEnumerable<T> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}
```

### 4.5. Query Validation

#### 4.5.1. Parameter Validation

```csharp
public sealed class GetProductsQueryValidator 
    : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Search))
            .WithMessage("Search term must not exceed 100 characters");
    }
}
```

### 4.6. Query Constraints

#### 4.6.1. Mandatory Rules

**Queries MUST:**
- Be read-only operations
- Return DTOs (never domain entities)
- Support AsNoTracking() when using EF Core
- Implement pagination for collections
- Use descriptive naming (Get/Search/Find + criteria)

**Queries MUST NOT:**
- Modify database state
- Raise domain events
- Execute business logic
- Validate business invariants
- Call repository write methods
- Trigger side effects

---

## 5. Command and Query Distinction

### 5.1. Decision Matrix

| Characteristic | Command | Query |
|----------------|---------|-------|
| Purpose | Modify state | Retrieve data |
| Domain Logic | Yes | No |
| Invariant Enforcement | Yes | No |
| Domain Events | Yes | No |
| Transaction | Required | Optional |
| Return Type | void, ID, Result | DTO, Collection |
| Optimization Priority | Correctness | Performance |
| Caching | No | Yes |
| HTTP Method | POST, PUT, PATCH, DELETE | GET |
| Side Effects | Expected | Forbidden |

### 5.2. Classification Guidelines

#### 5.2.1. When to Use Command

**Use Command If Operation:**
- Creates new entity
- Modifies existing entity
- Deletes entity
- Changes system state
- Triggers workflow
- Requires transaction
- Enforces business rules
- Raises domain events

**Examples:**
- CreateProduct
- UpdateProductPrice
- ActivateProduct
- DeleteProduct
- PlaceOrder
- CancelOrder
- ApproveRequest

#### 5.2.2. When to Use Query

**Use Query If Operation:**
- Retrieves single entity
- Retrieves collection of entities
- Performs search
- Aggregates data
- Projects data view
- No state modification

**Examples:**
- GetProductById
- GetProducts
- SearchProducts
- GetOrderSummary
- GetDashboardMetrics
- FindCustomersByEmail

### 5.3. Boundary Cases

#### 5.3.1. Read-After-Write Operations

**Problem:** Need to return entity data after Command execution

**Incorrect Approach:**
```csharp
// ANTI-PATTERN: Command returns entity
public sealed record CreateProductCommand : IRequest<ProductResponse>;
```

**Correct Approach:**
```csharp
// Command returns ID
public sealed record CreateProductCommand : IRequest<Guid>;

// Controller orchestrates
[HttpPost]
public async Task<IActionResult> CreateProduct(CreateProductRequest request)
{
    var command = new CreateProductCommand(...);
    var productId = await _mediator.Send(command);

    var query = new GetProductByIdQuery(productId);
    var product = await _mediator.Send(query);

    return CreatedAtAction(nameof(GetProduct), new { id = productId }, product);
}
```

---

## 6. HTTP Method Mapping

### 6.1. Command to HTTP Method Mapping

#### 6.1.1. Standard Mappings

| Operation Type | Command Example | HTTP Method | Endpoint Pattern |
|----------------|----------------|-------------|------------------|
| Create | CreateProductCommand | POST | /api/v1/products |
| Update (Full) | UpdateProductCommand | PUT | /api/v1/products/{id} |
| Update (Partial) | PatchProductCommand | PATCH | /api/v1/products/{id} |
| Delete | DeleteProductCommand | DELETE | /api/v1/products/{id} |
| Action | ActivateProductCommand | POST | /api/v1/products/{id}/activate |

#### 6.1.2. Controller Implementation

```csharp
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Command: Create
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.Currency);

        var productId = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = productId },
            await _mediator.Send(new GetProductByIdQuery(productId)));
    }

    // Command: Update
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        UpdateProductRequest request)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.Price,
            request.Currency);

        await _mediator.Send(command);

        return Ok();
    }

    // Command: Delete
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var command = new DeleteProductCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
```

### 6.2. Query to HTTP Method Mapping

#### 6.2.1. Standard Mappings

| Operation Type | Query Example | HTTP Method | Endpoint Pattern |
|----------------|--------------|-------------|------------------|
| Get Single | GetProductByIdQuery | GET | /api/v1/products/{id} |
| Get Collection | GetProductsQuery | GET | /api/v1/products |
| Search | SearchProductsQuery | GET | /api/v1/products/search?q=... |
| Get Aggregation | GetProductSummaryQuery | GET | /api/v1/products/{id}/summary |

#### 6.2.2. Controller Implementation

```csharp
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    // Query: Get Single
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var product = await _mediator.Send(query);
        return Ok(product);
    }

    // Query: Get Collection
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null)
    {
        var query = new GetProductsQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Sort = sort
        };

        var products = await _mediator.Send(query);
        return Ok(products);
    }
}
```

---

## 7. Request Processing Flow

### 7.1. Complete CQRS Flow Diagram

```
┌────────────────────────────────────────────────────────────┐
│                   Client Request                           │
└────────────────┬───────────────────────────────────────────┘
                 │
                 ↓
┌────────────────┴───────────────────────────────────────────┐
│               API Controller                               │
│  - Request validation (format)                             │
│  - Request to Command/Query mapping                        │
│  - IMediator.Send()                                        │
└────────────────┬───────────────────────────────────────────┘
                 │
                 ↓
┌────────────────┴───────────────────────────────────────────┐
│             MediatR Pipeline                               │
│  1. ValidationBehavior                                     │
│  2. LoggingBehavior                                        │
│  3. PerformanceBehavior                                    │
│  4. TransactionBehavior (Commands only)                    │
└────────────────┬───────────────────────────────────────────┘
                 │
            ┌────┴────┐
            │         │
            ↓         ↓
┌───────────┴──┐  ┌───┴────────────┐
│   Command    │  │   Query        │
│   Handler    │  │   Handler      │
│              │  │                │
│  - Domain    │  │  - Read DB     │
│  - Rules     │  │  - Project     │
│  - Events    │  │  - Return DTO  │
│  - Save      │  │                │
└───────────┬──┘  └───┬────────────┘
            │         │
            └────┬────┘
                 │
                 ↓
┌────────────────┴───────────────────────────────────────────┐
│                Return Response                             │
│  - Command: ID, Result, void                               │
│  - Query: DTO, Collection                                  │
└─────────────────────────────────────────────────────────────┘
```

### 7.2. Command Processing Steps

**Step 1: API Request Reception**
- Controller receives HTTP request
- Request body deserialized to request DTO

**Step 2: Command Creation**
- Map request DTO to Command object
- Include user context (claims, tenant)

**Step 3: MediatR Pipeline**
- ValidationBehavior validates command
- LoggingBehavior logs operation
- TransactionBehavior begins transaction

**Step 4: Handler Execution**
- Domain logic execution
- Invariant enforcement
- Entity creation/modification
- Domain event generation

**Step 5: Persistence**
- Repository save
- Transaction commit
- Domain event publishing

**Step 6: Response**
- Return ID or result
- Map to response DTO if needed
- Return appropriate HTTP status code

### 7.3. Query Processing Steps

**Step 1: API Request Reception**
- Controller receives HTTP GET request
- Query parameters extracted

**Step 2: Query Creation**
- Map parameters to Query object
- Include filtering, sorting, pagination

**Step 3: MediatR Pipeline**
- ValidationBehavior validates query parameters
- LoggingBehavior logs operation
- Performance tracking

**Step 4: Handler Execution**
- Database query execution
- Projection to DTO
- Filtering and sorting applied
- Pagination applied

**Step 5: Response**
- Return DTO or collection
- Return 200 OK

---

## 8. Implementation Patterns

### 8.1. Project Structure

#### 8.1.1. Recommended Organization

```
/Application
├── /Products
│   ├── /Commands
│   │   ├── /CreateProduct
│   │   │   ├── CreateProductCommand.cs
│   │   │   ├── CreateProductCommandHandler.cs
│   │   │   ├── CreateProductCommandValidator.cs
│   │   │   └── ProductCreatedEvent.cs
│   │   ├── /UpdateProduct
│   │   │   ├── UpdateProductCommand.cs
│   │   │   ├── UpdateProductCommandHandler.cs
│   │   │   └── UpdateProductCommandValidator.cs
│   │   └── /DeleteProduct
│   │       ├── DeleteProductCommand.cs
│   │       └── DeleteProductCommandHandler.cs
│   │
│   └── /Queries
│       ├── /GetProductById
│       │   ├── GetProductByIdQuery.cs
│       │   ├── GetProductByIdQueryHandler.cs
│       │   └── ProductResponse.cs
│       └── /GetProducts
│           ├── GetProductsQuery.cs
│           ├── GetProductsQueryHandler.cs
│           ├── GetProductsQueryValidator.cs
│           └── ProductListResponse.cs
│
└── /Common
    ├── /Behaviors
    │   ├── ValidationBehavior.cs
    │   ├── LoggingBehavior.cs
    │   └── TransactionBehavior.cs
    └── /Exceptions
        ├── ValidationException.cs
        └── NotFoundException.cs
```

### 8.2. MediatR Configuration

```csharp
// Program.cs
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
});

services.AddValidatorsFromAssembly(typeof(CreateProductCommand).Assembly);

services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
```

---

## 9. Performance Optimization Strategies

### 9.1. Command Optimization

#### 9.1.1. Strategies

**Batch Operations:**
- Process multiple commands in single transaction
- Reduce database round trips

**Asynchronous Processing:**
- Use 202 Accepted pattern for long-running commands
- Background job processing

**Optimistic Concurrency:**
- ETag-based updates
- Timestamp-based conflict detection

### 9.2. Query Optimization

#### 9.2.1. Database Optimization

**Use Dapper for Read-Heavy Operations:**
- 2-5x faster than EF Core
- Direct SQL control
- Minimal overhead

**EF Core Optimizations:**
```csharp
// AsNoTracking for read-only queries
var products = await _context.Products
    .AsNoTracking()
    .Where(p => p.IsActive)
    .ToListAsync();

// Select projection
var products = await _context.Products
    .Select(p => new ProductResponse
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
    })
    .ToListAsync();

// Split queries for collections
var order = await _context.Orders
    .Include(o => o.Items)
    .AsSplitQuery()
    .FirstAsync(o => o.Id == orderId);
```

#### 9.2.2. Caching Strategies

**Response Caching:**
```csharp
[HttpGet("{id}")]
[ResponseCache(Duration = 60)] // Cache for 60 seconds
public async Task<IActionResult> GetProduct(Guid id)
{
    var query = new GetProductByIdQuery(id);
    var product = await _mediator.Send(query);
    return Ok(product);
}
```

**Distributed Caching:**
```csharp
public async Task<ProductResponse> Handle(
    GetProductByIdQuery query,
    CancellationToken cancellationToken)
{
    var cacheKey = $"product:{query.Id}";
    
    var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
    if (cached != null)
    {
        return JsonSerializer.Deserialize<ProductResponse>(cached);
    }

    var product = await _context.Products
        .AsNoTracking()
        .Where(p => p.Id == query.Id)
        .Select(p => new ProductResponse { ... })
        .FirstOrDefaultAsync(cancellationToken);

    if (product != null)
    {
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);
    }

    return product;
}
```

---

## 10. Testing Strategy

### 10.1. Command Testing

#### 10.1.1. Unit Tests

```csharp
public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesProduct()
    {
        // Arrange
        var repository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateProductCommandHandler(
            repository.Object,
            unitOfWork.Object);

        var command = new CreateProductCommand(
            "Test Product",
            "Description",
            99.99m,
            "USD");

        // Act
        var productId = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, productId);
        repository.Verify(r => r.AddAsync(
            It.IsAny<Product>(),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidPrice_ThrowsDomainException()
    {
        // Arrange
        var handler = CreateHandler();
        var command = new CreateProductCommand(
            "Test Product",
            "Description",
            -10m, // Invalid
            "USD");

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
```

### 10.2. Query Testing

#### 10.2.1. Unit Tests

```csharp
public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingProduct_ReturnsProduct()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var productId = Guid.NewGuid();
        context.Products.Add(new Product
        {
            Id = productId,
            Name = "Test Product",
            Price = 99.99m
        });
        await context.SaveChangesAsync();

        var handler = new GetProductByIdQueryHandler(context);
        var query = new GetProductByIdQuery(productId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ThrowsNotFoundException()
    {
        // Arrange
        var context = CreateEmptyContext();
        var handler = new GetProductByIdQueryHandler(context);
        var query = new GetProductByIdQuery(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(query, CancellationToken.None));
    }
}
```

### 10.3. Integration Testing

```csharp
public class ProductsIntegrationTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CreateAndRetrieveProduct_EndToEnd_Succeeds()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateProductRequest
        {
            Name = "Integration Test Product",
            Price = 99.99m,
            Currency = "USD"
        };

        // Act - Create
        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/products",
            createRequest);
        
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        
        var product = await createResponse.Content
            .ReadFromJsonAsync<ProductResponse>();

        // Act - Retrieve
        var getResponse = await client.GetAsync(
            $"/api/v1/products/{product.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrievedProduct = await getResponse.Content
            .ReadFromJsonAsync<ProductResponse>();
        Assert.Equal(createRequest.Name, retrievedProduct.Name);
    }
}
```

---

## 11. Project Structure and Organization

### 11.1. Feature-Based Organization

**Benefits:**
- High cohesion
- Easy navigation
- Clear ownership
- Simplified testing

**Structure:**
```
Application/
├── Products/
│   ├── Commands/
│   └── Queries/
├── Orders/
│   ├── Commands/
│   └── Queries/
└── Customers/
    ├── Commands/
    └── Queries/
```

---

## 12. Anti-Patterns and Pitfalls

### 12.1. Command Anti-Patterns

#### 12.1.1. Returning Domain Entities

**Problem:**
```csharp
public sealed record CreateProductCommand : IRequest<Product>; // WRONG
```

**Solution:**
```csharp
public sealed record CreateProductCommand : IRequest<Guid>; // RIGHT
```

#### 12.1.2. Commands Performing Queries

**Problem:**
```csharp
public async Task<Guid> Handle(CreateProductCommand command, ...)
{
    // WRONG: Complex query in command
    var existingProducts = await _context.Products
        .Include(p => p.Reviews)
        .Where(p => p.Category == command.Category)
        .ToListAsync();
    
    // ...
}
```

**Solution:** Use Query to check state before Command

### 12.2. Query Anti-Patterns

#### 12.2.1. Queries Modifying State

**Problem:**
```csharp
public async Task<ProductResponse> Handle(GetProductByIdQuery query, ...)
{
    var product = await _context.Products.FindAsync(query.Id);
    product.Views++; // WRONG: Modifying state in query!
    await _context.SaveChangesAsync();
    return Map(product);
}
```

**Solution:** Use Command for state changes, Query only reads

#### 12.2.2. Returning Tracked Entities

**Problem:**
```csharp
var product = await _context.Products // Missing AsNoTracking()
    .FirstOrDefaultAsync(p => p.Id == query.Id);
```

**Solution:**
```csharp
var product = await _context.Products
    .AsNoTracking() // Add this!
    .FirstOrDefaultAsync(p => p.Id == query.Id);
```

---

## 13. Scalability Considerations

### 13.1. Independent Scaling

**Commands (Write Model):**
- Scale vertically for consistency
- Use master database
- Transaction support critical

**Queries (Read Model):**
- Scale horizontally for throughput
- Use read replicas
- Caching layers
- CDN for static projections

### 13.2. Advanced Patterns

**Read Model Separation:**
- Denormalized read database
- Event-driven synchronization
- Eventual consistency

**Event Sourcing Integration:**
- Commands produce events
- Queries read projections
- Complete audit trail

---

## 14. Glossary

**Command:** An operation that modifies system state, representing business intent.

**Query:** An operation that retrieves data without modifying system state.

**CQRS:** Command Query Responsibility Segregation - architectural pattern separating read and write operations.

**Handler:** A class responsible for executing a single Command or Query.

**MediatR:** A .NET library implementing the Mediator pattern for CQRS.

**Pipeline Behavior:** Cross-cutting concern executed before/after Command/Query handlers.

**DTO (Data Transfer Object):** Object carrying data between layers without business logic.

**Projection:** Transforming domain entities into specific DTO shapes optimized for clients.

---

## 15. Recommendations and Next Steps

### 15.1. For Development Teams

**Implementation Checklist:**
- [ ] Separate Commands and Queries clearly
- [ ] Use MediatR for all operations
- [ ] Implement validation for all Commands and Queries
- [ ] Never return domain entities from Commands
- [ ] Use AsNoTracking() for all Queries
- [ ] Apply appropriate HTTP methods
- [ ] Write comprehensive unit tests
- [ ] Write integration tests for end-to-end flows

### 15.2. For Architects

**Governance Activities:**
- Review Command/Query separation
- Validate handler implementations
- Ensure no anti-patterns
- Monitor performance metrics
- Review scalability approach

### 15.3. Related Documentation

**Must Read:**
- High-Level Architecture (ARCH-HL-001)
- CQRS Pipeline (ARCH-CQRS-002)
- Domain Modeling (ARCH-DOMAIN-004)
- API Guidelines (API-GUIDELINES-001)

---

## 16. References

### 16.1. CQRS Pattern

**Greg Young - CQRS Documents**
- https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf

**Martin Fowler - CQRS**
- https://martinfowler.com/bliki/CQRS.html

**Bertrand Meyer - Command-Query Separation**
- Object-Oriented Software Construction

### 16.2. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- ARCH-HL-001: High-Level Architecture
- ARCH-CQRS-002: CQRS Pipeline Specification
- ARCH-DOMAIN-004: Domain Modeling
- API-GUIDELINES-001: API Design Guidelines

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial CQRS specification with comprehensive Command/Query separation patterns |

---

**END OF DOCUMENT**