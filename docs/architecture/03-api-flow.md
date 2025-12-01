# API Flow and Request Processing Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | ARCH-API-003 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Technical Specification |
| Target Audience | Solution Architects, API Designers, Development Teams, Technical Leads, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Quarterly |
| Related Documents | High-Level Architecture (ARCH-HL-001), CQRS Pipeline Specification (ARCH-CQRS-002) |
| Prerequisites | Understanding of HTTP protocol, RESTful API principles, Clean Architecture |

---

## Executive Summary

This document specifies the complete request processing flow through the API layer of the .NET Enterprise Architecture Template, from HTTP request reception to response delivery. The specification defines layer responsibilities, data transformation patterns, error handling strategies, and observability integration points that ensure consistent, maintainable, and auditable API implementations.

**Strategic Business Value:**
- Operational consistency through standardized request processing patterns
- Reduced incident resolution time via comprehensive correlation tracking
- Enhanced security posture through centralized validation and error handling
- Improved developer productivity via clear separation of concerns
- Audit compliance through structured logging and traceability

**Key Technical Capabilities:**
- End-to-end request lifecycle management with clear layer boundaries
- Standardized error handling using RFC 7807 Problem Details specification
- Comprehensive correlation ID propagation for distributed tracing
- Model binding and validation at appropriate architectural boundaries
- Multi-layer mapping strategy isolating domain models from external contracts
- Integrated observability with structured logging and metrics collection

**Compliance and Standards:**
- Aligned with REST architectural constraints (RFC 7230-7235)
- Implements RFC 7807 Problem Details for HTTP APIs
- Follows TOGAF 10 layer separation principles
- Supports ITIL 4 incident management through correlation tracking
- Conforms to ISO/IEC/IEEE 26515:2018 documentation standards
- Implements NIST cybersecurity framework input validation controls

---

## Table of Contents

1. Introduction and Scope
2. Request Processing Architecture
3. API Layer Responsibilities and Constraints
4. Complete Request Lifecycle
5. HTTP Endpoint Implementation Patterns
6. Model Binding and Validation Strategy
7. Error Handling and Problem Details Specification
8. Correlation ID and Request Tracking
9. Data Transformation and Mapping Strategy
10. Observability Integration
11. Performance Characteristics
12. Security Considerations
13. Testing Strategy
14. Glossary
15. Recommendations and Next Steps
16. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides comprehensive specifications for HTTP request processing within the .NET Enterprise Architecture Template. It defines the complete request lifecycle, layer responsibilities, data transformation patterns, error handling mechanisms, and observability integration necessary for building consistent, maintainable, and enterprise-grade APIs.

### 1.2. Scope

**In Scope:**
- Complete HTTP request processing flow from reception to response
- API layer responsibilities and architectural constraints
- Model binding and validation strategies
- Error handling and standardized error response formats
- Correlation ID generation and propagation
- Data transformation patterns between layers
- Logging and observability integration
- HTTP status code selection guidelines
- Response format standardization

**Out of Scope:**
- Authentication and authorization implementations (separate specification)
- Rate limiting and throttling mechanisms (separate specification)
- API versioning strategies (separate specification)
- Caching strategies (separate specification)
- Specific business domain implementations
- Database optimization techniques
- Infrastructure deployment configurations

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**Solution Architects:** Overall API architecture governance and pattern compliance

**API Designers:** Endpoint design, contract specification, error handling patterns

**Development Teams:** Implementation guidelines, coding standards, testing approaches

**Technical Leads:** Code review standards, quality assurance, pattern enforcement

**Operations Teams:** Logging structure, correlation tracking, incident investigation

**C-Level Executives:** Business value, operational efficiency, risk mitigation

### 1.4. Architectural Context

The API layer serves as the entry point for all external communications within the Clean Architecture:

```
External Clients
       ↓
┌──────────────────────────────────────┐
│         API Layer                    │
│  (HTTP Concerns Only)                │
└──────────────┬───────────────────────┘
               ↓
┌──────────────┴───────────────────────┐
│      Application Layer               │
│  (Use Case Orchestration via CQRS)   │
└──────────────┬───────────────────────┘
               ↓
┌──────────────┴───────────────────────┐
│         Domain Layer                 │
│  (Business Logic)                    │
└──────────────┬───────────────────────┘
               ↓
┌──────────────┴───────────────────────┐
│     Infrastructure Layer             │
│  (Data Access, External Services)    │
└──────────────────────────────────────┘
```

---

## 2. Request Processing Architecture

### 2.1. High-Level Request Flow

The request processing architecture follows a layered approach with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│  1. HTTP Request Reception                                  │
│     - Client sends HTTP request with JSON payload           │
│     - Request enters ASP.NET Core pipeline                  │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌────────────────────┴────────────────────────────────────────┐
│  2. Middleware Pipeline Processing                          │
│     - Correlation ID middleware (add/extract ID)            │
│     - Logging enrichment middleware                         │
│     - Exception handling middleware (outer boundary)        │
│     - Authentication middleware                             │
│     - Authorization middleware                              │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌────────────────────┴────────────────────────────────────────┐
│  3. Controller/Endpoint Execution                           │
│     - Route matching and parameter binding                  │
│     - Model binding (JSON → C# request object)              │
│     - Basic model validation (DataAnnotations if present)   │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌────────────────────┴────────────────────────────────────────┐
│  4. Command/Query Creation                                  │
│     - Map request model to command or query object          │
│     - Include all required parameters                       │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌────────────────────┴────────────────────────────────────────┐
│  5. MediatR Pipeline Dispatch                               │
│     - Send command/query to MediatR                         │
│     - Pipeline behaviors execute (see ARCH-CQRS-002)        │
│       • ValidationBehavior                                  │
│       • LoggingBehavior                                     │
│       • PerformanceBehavior                                 │
│       • TransactionBehavior                                 │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌────────────────────┴────────────────────────────────────────┐
│  6. Handler Execution (Application Layer)                   │
│     - Handler orchestrates domain operations                │
│     - Interacts with domain entities                        │
│     - Coordinates infrastructure services                   │
│     - Returns application DTO                               │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌────────────────────┴────────────────────────────────────────┐
│  7. Response Construction                                   │
│     - Map application DTO to API response model             │
│     - Determine appropriate HTTP status code                │
│     - Set response headers (Location for 201, etc.)         │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌────────────────────┴────────────────────────────────────────┐
│  8. HTTP Response Delivery                                  │
│     - Serialize response to JSON                            │
│     - Include correlation ID in response headers            │
│     - Return to client                                      │
└─────────────────────────────────────────────────────────────┘
```

### 2.2. Request Flow Sequence Diagram

```
┌────────┐         ┌─────────┐         ┌─────────┐         ┌─────────┐         ┌──────────┐
│ Client │         │   API   │         │ MediatR │         │ Handler │         │  Domain  │
│        │         │  Layer  │         │ Pipeline│         │  (App)  │         │  & Infra │
└───┬────┘         └────┬────┘         └────┬────┘         └────┬────┘         └────┬─────┘
    │                   │                   │                   │                   │
    │  HTTP Request     │                   │                   │                   │
    │  (POST /api/...)  │                   │                   │                   │
    ├──────────────────>│                   │                   │                   │
    │                   │                   │                   │                   │
    │                   │ Model Binding &   │                   │                   │
    │                   │ Input Validation  │                   │                   │
    │                   │ ───────────┐      │                   │                   │
    │                   │            │      │                   │                   │
    │                   │ <──────────┘      │                   │                   │
    │                   │                   │                   │                   │
    │                   │ Send Command/Query│                   │                   │
    │                   ├──────────────────>│                   │                   │
    │                   │                   │                   │                   │
    │                   │                   │ Pipeline Behaviors│                   │
    │                   │                   │ • Validation      │                   │
    │                   │                   │ • Logging         │                   │
    │                   │                   │ • Performance     │                   │
    │                   │                   │ • Transaction     │                   │
    │                   │                   │ ───────────┐      │                   │
    │                   │                   │            │      │                   │
    │                   │                   │ <──────────┘      │                   │
    │                   │                   │                   │                   │
    │                   │                   │ Invoke Handler    │                   │
    │                   │                   ├──────────────────>│                   │
    │                   │                   │                   │                   │
    │                   │                   │                   │ Domain Operations │
    │                   │                   │                   │ & Persistence     │
    │                   │                   │                   ├──────────────────>│
    │                   │                   │                   │                   │
    │                   │                   │                   │ Domain Result     │
    │                   │                   │                   │<──────────────────┤
    │                   │                   │                   │                   │
    │                   │                   │ Response DTO      │                   │
    │                   │                   │<──────────────────┤                   │
    │                   │                   │                   │                   │
    │                   │ Result (or        │                   │                   │
    │                   │ Exception)        │                   │                   │
    │                   │<──────────────────┤                   │                   │
    │                   │                   │                   │                   │
    │                   │ Map to API        │                   │                   │
    │                   │ Response Model    │                   │                   │
    │                   │ ───────────┐      │                   │                   │
    │                   │            │      │                   │                   │
    │                   │ <──────────┘      │                   │                   │
    │                   │                   │                   │                   │
    │  HTTP Response    │                   │                   │                   │
    │  (200 OK + JSON)  │                   │                   │                   │
    │<──────────────────┤                   │                   │                   │
    │                   │                   │                   │                   │
```

### 2.3. Layer Interaction Model

#### 2.3.1. Request Flow Through Layers

**Layer 1: Middleware Pipeline**
- Executes before controller
- Cross-cutting concerns (logging, correlation, authentication)
- Exception boundary establishment

**Layer 2: Controller/Endpoint**
- Route handling and parameter extraction
- Model binding and basic validation
- Command/query object creation
- MediatR dispatch

**Layer 3: MediatR Pipeline**
- Pipeline behavior execution
- Cross-cutting concern processing
- Handler invocation

**Layer 4: Application Handler**
- Use case orchestration
- Domain object coordination
- Infrastructure service usage
- DTO construction

**Layer 5: Domain & Infrastructure**
- Business logic execution
- Data persistence
- External service integration

#### 2.3.2. Response Flow Through Layers

Response flows in reverse order:
- Handler returns DTO to MediatR pipeline
- Pipeline behaviors complete post-processing
- Controller receives result and maps to API response model
- Middleware processes response (logging, headers)
- HTTP response serialized and sent to client

---

## 3. API Layer Responsibilities and Constraints

### 3.1. Mandatory Responsibilities

The API layer shall be responsible for the following concerns:

#### 3.1.1. HTTP Protocol Handling

**Request Reception:**
- Accept HTTP requests on configured endpoints
- Extract HTTP headers, query parameters, route values
- Parse request body from JSON or other content types
- Handle content negotiation

**Response Delivery:**
- Serialize response objects to JSON or requested format
- Set appropriate HTTP status codes
- Configure response headers (Content-Type, Location, etc.)
- Handle content compression if configured

#### 3.1.2. Route Management

**Endpoint Definition:**
- Define HTTP routes and methods (GET, POST, PUT, DELETE, PATCH)
- Configure route constraints and parameter patterns
- Implement route versioning if applicable
- Document endpoint contracts

**Parameter Binding:**
- Bind route parameters to method arguments
- Bind query string parameters
- Bind request headers
- Bind request body to model objects

#### 3.1.3. Model Binding and Basic Validation

**Input Binding:**
- Deserialize JSON request bodies to C# objects
- Apply model binding for complex types
- Handle binding errors gracefully

**Format Validation:**
- Validate required fields are present
- Validate data types match expectations
- Validate format constraints (email, URL, etc.)
- Return 400 Bad Request for binding failures

**Note:** Business validation occurs in Application layer via ValidationBehavior.

#### 3.1.4. HTTP Status Code Selection

**Success Responses:**
- 200 OK: Successful GET, PUT, PATCH
- 201 Created: Successful POST creating resource
- 204 No Content: Successful DELETE or operation without return value
- 202 Accepted: Asynchronous operation accepted

**Client Error Responses:**
- 400 Bad Request: Validation failures, malformed requests
- 401 Unauthorized: Authentication required
- 403 Forbidden: Authenticated but not authorized
- 404 Not Found: Resource does not exist
- 409 Conflict: Business rule violation, duplicate resource
- 422 Unprocessable Entity: Semantic validation failure

**Server Error Responses:**
- 500 Internal Server Error: Unhandled exceptions
- 503 Service Unavailable: Dependency unavailable

#### 3.1.5. Response Header Management

**Required Headers:**
- Content-Type: application/json (or appropriate type)
- X-Correlation-ID: Request correlation identifier

**Conditional Headers:**
- Location: URI of created resource (201 responses)
- ETag: Resource version for caching
- Cache-Control: Caching directives for GET responses

### 3.2. Strict Constraints

The API layer must not perform the following activities:

#### 3.2.1. Prohibited Activities

**Business Logic:**
- Must not implement business rules or domain logic
- Must not perform calculations or data transformations beyond mapping
- Must not make business decisions

**Data Access:**
- Must not directly access databases
- Must not create database connections
- Must not execute SQL queries

**Domain Object Manipulation:**
- Must not create or modify domain entities directly
- Must not call domain object methods
- Must not enforce business invariants

**Infrastructure Concerns:**
- Must not implement repository logic
- Must not integrate with external services directly
- Must not manage transactions

#### 3.2.2. Layer Boundary Enforcement

**Dependency Constraints:**
- API layer may depend on Application layer only
- No direct dependency on Infrastructure layer (except for DI configuration)
- No direct dependency on specific implementations

**Communication Pattern:**
- All business operations must flow through MediatR
- All commands and queries must be sent via IMediator interface
- No direct handler instantiation

### 3.3. API Layer Components

#### 3.3.1. Controllers

**Responsibility:**
- Expose HTTP endpoints
- Handle request/response transformation
- Delegate to MediatR for business operations

**Structure:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ResourceController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResourceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Endpoint methods
}
```

#### 3.3.2. Request Models

**Characteristics:**
- Simple data transfer objects
- Contain only data required for operation
- May include DataAnnotations for basic validation
- Separate from command/query objects

**Example:**
```csharp
public sealed record CreateProductRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; }

    [Required]
    public decimal Price { get; init; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; }

    public Guid CategoryId { get; init; }
}
```

#### 3.3.3. Response Models

**Characteristics:**
- Tailored to API contract requirements
- May differ from application DTOs
- Include only data needed by clients
- Versioned with API version

**Example:**
```csharp
public sealed record ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; }
    public string CategoryName { get; init; }
    public DateTime CreatedAt { get; init; }
}
```

---

## 4. Complete Request Lifecycle

### 4.1. Detailed Request Processing Steps

#### 4.1.1. Step 1: HTTP Request Reception

**Process:**
1. ASP.NET Core receives HTTP request
2. Request enters middleware pipeline
3. Kestrel web server parses HTTP headers and body

**Key Activities:**
- TCP connection established
- HTTP request parsed
- Request buffer allocated

**Potential Errors:**
- Malformed HTTP requests (400 Bad Request)
- Request entity too large (413 Payload Too Large)
- Unsupported media type (415 Unsupported Media Type)

#### 4.1.2. Step 2: Middleware Pipeline Processing

**Execution Order:**

1. **Correlation ID Middleware**
   - Extract correlation ID from header (X-Correlation-ID)
   - Generate new ID if not present
   - Add to logging context
   - Add to response headers

2. **Exception Handling Middleware**
   - Establish try-catch boundary
   - Catch all unhandled exceptions
   - Transform to Problem Details responses
   - Log exceptions with correlation ID

3. **Logging Enrichment Middleware**
   - Add request path to logging context
   - Add user identity if authenticated
   - Add timestamp
   - Log request initiation

4. **Authentication Middleware**
   - Validate authentication tokens
   - Establish user identity
   - Return 401 if authentication fails

5. **Authorization Middleware**
   - Evaluate authorization policies
   - Return 403 if authorization fails

6. **Routing Middleware**
   - Match request to endpoint
   - Extract route parameters
   - Return 404 if no route matches

#### 4.1.3. Step 3: Controller Endpoint Execution

**Process:**
1. Controller action method invoked
2. Model binding executes
3. DataAnnotations validation (if present)
4. Action method logic executes

**Model Binding:**
```csharp
[HttpPost]
public async Task<ActionResult<ProductResponse>> Create(
    [FromBody] CreateProductRequest request,  // Bound from request body
    [FromHeader(Name = "X-Api-Version")] string apiVersion,  // Bound from header
    CancellationToken cancellationToken)  // Bound from framework
{
    // Method implementation
}
```

**Binding Sources:**
- FromBody: JSON request body
- FromRoute: URL path segments
- FromQuery: Query string parameters
- FromHeader: HTTP headers
- FromForm: Form data
- FromServices: Dependency injection

#### 4.1.4. Step 4: Command/Query Object Creation

**Mapping Pattern:**
```csharp
var command = new CreateProductCommand(
    Name: request.Name,
    Description: request.Description,
    Price: request.Price,
    Currency: request.Currency,
    CategoryId: request.CategoryId);
```

**Design Principles:**
- Explicit parameter mapping
- Immutable command/query objects
- No business logic in mapping
- Clear data flow

#### 4.1.5. Step 5: MediatR Pipeline Dispatch

**Dispatch Pattern:**
```csharp
var result = await _mediator.Send(command, cancellationToken);
```

**Pipeline Execution:**
- ValidationBehavior validates command
- LoggingBehavior logs request details
- PerformanceBehavior monitors duration
- TransactionBehavior manages database transaction
- Handler executes business logic

**See ARCH-CQRS-002 for detailed pipeline specifications.**

#### 4.1.6. Step 6: Result Processing

**Success Path:**
```csharp
var result = await _mediator.Send(command, cancellationToken);

// Map application DTO to API response
var response = new ProductResponse(
    Id: result.Id,
    Name: result.Name,
    Price: result.Price,
    Currency: result.Currency,
    CategoryName: result.CategoryName,
    CreatedAt: result.CreatedAt);

return CreatedAtAction(
    nameof(GetById),
    new { id = response.Id },
    response);
```

**Error Path:**
- Exceptions propagate through MediatR pipeline
- Caught by exception handling middleware
- Transformed to Problem Details
- Logged with correlation ID
- Returned as appropriate HTTP status code

#### 4.1.7. Step 7: Response Serialization and Delivery

**Serialization:**
- Response object serialized to JSON
- Content-Type header set to application/json
- Response headers added (correlation ID, location, etc.)

**Logging:**
- Response status code logged
- Response size logged
- Request duration logged

**Delivery:**
- HTTP response sent to client
- TCP connection closed or kept alive
- Middleware post-processing completes

### 4.2. Request Lifecycle Timing

#### 4.2.1. Performance Targets

| Phase | Target Duration | Notes |
|-------|----------------|-------|
| Middleware Pipeline | < 5ms | Correlation ID, logging, auth |
| Model Binding | < 10ms | JSON deserialization |
| Validation | < 20ms | FluentValidation rules |
| Handler Execution | < 100ms | Simple operations |
| Handler Execution | < 500ms | Complex operations |
| Response Serialization | < 10ms | JSON serialization |
| **Total (Simple)** | **< 150ms** | GET/POST simple entities |
| **Total (Complex)** | **< 550ms** | Complex business operations |

#### 4.2.2. Monitoring Points

**Key Metrics:**
- Request duration by endpoint
- Model binding failures
- Validation failure rates
- Handler execution time
- Exception rates by type
- Response size distribution

---

## 5. HTTP Endpoint Implementation Patterns

### 5.1. RESTful Endpoint Design

#### 5.1.1. Resource-Based URL Structure

**Standard Patterns:**

| Operation | HTTP Method | URL Pattern | Description |
|-----------|-------------|-------------|-------------|
| List | GET | /api/products | Get collection |
| Get | GET | /api/products/{id} | Get single resource |
| Create | POST | /api/products | Create new resource |
| Update | PUT | /api/products/{id} | Replace resource |
| Partial Update | PATCH | /api/products/{id} | Update specific fields |
| Delete | DELETE | /api/products/{id} | Remove resource |
| Search | GET | /api/products/search?q=term | Search with query |

**Nested Resources:**
```
GET    /api/products/{productId}/reviews
POST   /api/products/{productId}/reviews
GET    /api/products/{productId}/reviews/{reviewId}
```

#### 5.1.2. Query Parameter Conventions

**Filtering:**
```
GET /api/products?categoryId={guid}&minPrice=100&maxPrice=500
```

**Pagination:**
```
GET /api/products?pageNumber=1&pageSize=20
```

**Sorting:**
```
GET /api/products?sortBy=name&sortOrder=asc
```

**Field Selection:**
```
GET /api/products?fields=id,name,price
```

### 5.2. Command Endpoint Implementation

#### 5.2.1. Create Resource (POST)

**Complete Implementation:**

```csharp
/// <summary>
/// Creates a new product
/// </summary>
/// <param name="request">Product creation data</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Created product</returns>
/// <response code="201">Product created successfully</response>
/// <response code="400">Invalid request data</response>
/// <response code="409">Product with same name already exists</response>
[HttpPost]
[ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
public async Task<ActionResult<ProductResponse>> Create(
    [FromBody] CreateProductRequest request,
    CancellationToken cancellationToken)
{
    // Create command from request
    var command = new CreateProductCommand(
        Name: request.Name,
        Description: request.Description,
        Price: request.Price,
        Currency: request.Currency,
        CategoryId: request.CategoryId);

    // Send through MediatR pipeline
    var result = await _mediator.Send(command, cancellationToken);

    // Map to API response model
    var response = MapToResponse(result);

    // Return 201 Created with Location header
    return CreatedAtAction(
        actionName: nameof(GetById),
        routeValues: new { id = response.Id },
        value: response);
}
```

**Key Elements:**
- XML documentation for API documentation generation
- ProducesResponseType attributes for OpenAPI/Swagger
- Explicit status code constants
- Location header via CreatedAtAction
- Clear separation: request → command → handler → response

#### 5.2.2. Update Resource (PUT)

**Complete Implementation:**

```csharp
/// <summary>
/// Updates an existing product
/// </summary>
/// <param name="id">Product identifier</param>
/// <param name="request">Updated product data</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Updated product</returns>
/// <response code="200">Product updated successfully</response>
/// <response code="400">Invalid request data</response>
/// <response code="404">Product not found</response>
[HttpPut("{id:guid}")]
[ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
public async Task<ActionResult<ProductResponse>> Update(
    Guid id,
    [FromBody] UpdateProductRequest request,
    CancellationToken cancellationToken)
{
    // Create command with ID from route
    var command = new UpdateProductCommand(
        Id: id,
        Name: request.Name,
        Description: request.Description,
        Price: request.Price,
        Currency: request.Currency,
        CategoryId: request.CategoryId);

    // Send through MediatR pipeline
    var result = await _mediator.Send(command, cancellationToken);

    // Map to API response model
    var response = MapToResponse(result);

    return Ok(response);
}
```

**Design Considerations:**
- ID from route takes precedence over ID in request body
- PUT replaces entire resource
- Use PATCH for partial updates
- Return updated resource in response body

#### 5.2.3. Delete Resource (DELETE)

**Complete Implementation:**

```csharp
/// <summary>
/// Deletes a product
/// </summary>
/// <param name="id">Product identifier</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>No content</returns>
/// <response code="204">Product deleted successfully</response>
/// <response code="404">Product not found</response>
[HttpDelete("{id:guid}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
public async Task<IActionResult> Delete(
    Guid id,
    CancellationToken cancellationToken)
{
    var command = new DeleteProductCommand(Id: id);

    await _mediator.Send(command, cancellationToken);

    return NoContent();
}
```

**Design Considerations:**
- Returns 204 No Content on success
- No response body required
- Idempotent: repeated deletes return 204
- Use soft delete for audit requirements

### 5.3. Query Endpoint Implementation

#### 5.3.1. Get Single Resource (GET by ID)

**Complete Implementation:**

```csharp
/// <summary>
/// Gets a product by identifier
/// </summary>
/// <param name="id">Product identifier</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Product details</returns>
/// <response code="200">Product found</response>
/// <response code="404">Product not found</response>
[HttpGet("{id:guid}")]
[ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
public async Task<ActionResult<ProductResponse>> GetById(
    Guid id,
    CancellationToken cancellationToken)
{
    var query = new GetProductByIdQuery(Id: id);

    var result = await _mediator.Send(query, cancellationToken);

    if (result is null)
        return NotFound();

    var response = MapToResponse(result);

    return Ok(response);
}
```

**Key Patterns:**
- Explicit null check for not found
- Return 404 for missing resources
- Clear query object creation

#### 5.3.2. List/Search Resources (GET collection)

**Complete Implementation:**

```csharp
/// <summary>
/// Searches products with optional filters
/// </summary>
/// <param name="request">Search criteria</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Paginated list of products</returns>
/// <response code="200">Products retrieved successfully</response>
[HttpGet]
[ProducesResponseType(typeof(PagedResponse<ProductResponse>), StatusCodes.Status200OK)]
public async Task<ActionResult<PagedResponse<ProductResponse>>> Search(
    [FromQuery] SearchProductsRequest request,
    CancellationToken cancellationToken)
{
    var query = new SearchProductsQuery(
        SearchTerm: request.SearchTerm,
        CategoryId: request.CategoryId,
        MinPrice: request.MinPrice,
        MaxPrice: request.MaxPrice,
        PageNumber: request.PageNumber ?? 1,
        PageSize: request.PageSize ?? 20,
        SortBy: request.SortBy,
        SortDescending: request.SortDescending ?? false);

    var result = await _mediator.Send(query, cancellationToken);

    var response = new PagedResponse<ProductResponse>
    {
        Items = result.Items.Select(MapToResponse).ToList(),
        TotalCount = result.TotalCount,
        PageNumber = result.PageNumber,
        PageSize = result.PageSize,
        TotalPages = result.TotalPages
    };

    return Ok(response);
}
```

**Pagination Response Model:**

```csharp
public sealed record PagedResponse<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

---

## 6. Model Binding and Validation Strategy

### 6.1. Model Binding Architecture

#### 6.1.1. Binding Sources

**FromBody Binding:**
- Deserializes JSON request body
- Uses System.Text.Json by default
- Supports complex object graphs
- Single parameter per request

**FromRoute Binding:**
- Extracts values from URL path
- Supports simple types and GUIDs
- Type conversion automatic
- Multiple parameters supported

**FromQuery Binding:**
- Extracts values from query string
- Supports simple types and arrays
- Automatic type conversion
- Multiple parameters supported

**FromHeader Binding:**
- Extracts values from HTTP headers
- Case-insensitive by default
- Supports custom header names
- Type conversion supported

#### 6.1.2. Model Binding Configuration

**JSON Serialization Options:**

```csharp
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
```

**Custom Model Binder (if needed):**

```csharp
public class CustomModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // Custom binding logic
        return Task.CompletedTask;
    }
}
```

### 6.2. Validation Strategy

#### 6.2.1. Two-Layer Validation Approach

**Layer 1: API Layer Format Validation**

Purpose: Validate request format and basic constraints

Implementation: DataAnnotations on request models

Timing: During model binding, before controller action

**Layer 2: Application Layer Business Validation**

Purpose: Validate business rules and complex constraints

Implementation: FluentValidation in ValidationBehavior

Timing: In MediatR pipeline, before handler execution

#### 6.2.2. DataAnnotations for Format Validation

**Request Model Example:**

```csharp
public sealed record CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(200, ErrorMessage = "Product name must not exceed 200 characters")]
    public string Name { get; init; }

    [MaxLength(2000, ErrorMessage = "Description must not exceed 2000 characters")]
    public string? Description { get; init; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; init; }

    [Required(ErrorMessage = "Currency is required")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code")]
    public string Currency { get; init; }

    [Required(ErrorMessage = "Category ID is required")]
    public Guid CategoryId { get; init; }
}
```

**Automatic Validation:**
- ASP.NET Core validates automatically
- Returns 400 Bad Request with validation errors
- No explicit validation call required

#### 6.2.3. FluentValidation for Business Rules

**Validator Implementation:**

```csharp
public sealed class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandValidator(IProductRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero");

        RuleFor(x => x.Currency)
            .Must(BeValidCurrency)
            .WithMessage("Currency must be a valid ISO 4217 code");

        RuleFor(x => x)
            .MustAsync(HaveUniqueName)
            .WithMessage("A product with this name already exists");

        RuleFor(x => x.CategoryId)
            .MustAsync(CategoryExists)
            .WithMessage("Category does not exist");
    }

    private bool BeValidCurrency(string currency)
    {
        var validCurrencies = new[] { "USD", "EUR", "GBP", "JPY" };
        return validCurrencies.Contains(currency);
    }

    private async Task<bool> HaveUniqueName(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        return !await _repository.ExistsWithNameAsync(
            command.Name,
            cancellationToken);
    }

    private async Task<bool> CategoryExists(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        return await _repository.CategoryExistsAsync(
            categoryId,
            cancellationToken);
    }
}
```

**Validation Execution:**
- Automatically executed by ValidationBehavior
- Throws ValidationException on failure
- Exception caught and transformed to Problem Details
- Returns 400 Bad Request with structured errors

### 6.3. Validation Error Response Format

#### 6.3.1. ValidationProblemDetails Structure

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-0f83f3c4d5e6f7a8b9c0d1e2f3a4b5c6-01",
  "errors": {
    "Name": [
      "A product with this name already exists"
    ],
    "CategoryId": [
      "Category does not exist"
    ],
    "Price": [
      "Price must be greater than zero"
    ]
  }
}
```

#### 6.3.2. Custom Validation Response

```csharp
public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
            return false;

        var problemDetails = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            }
        };

        foreach (var error in validationException.Errors)
        {
            problemDetails.Errors.Add(error.Key, error.Value);
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
```

---

## 7. Error Handling and Problem Details Specification

### 7.1. RFC 7807 Problem Details Standard

#### 7.1.1. Problem Details Structure

**Standard Fields:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| type | string (URI) | No | URI reference identifying problem type |
| title | string | No | Short, human-readable summary |
| status | integer | No | HTTP status code |
| detail | string | No | Human-readable explanation specific to occurrence |
| instance | string (URI) | No | URI reference identifying specific occurrence |

**Example Problem Details:**

```json
{
  "type": "https://api.example.com/problems/insufficient-funds",
  "title": "Insufficient Funds",
  "status": 422,
  "detail": "Account balance is $50.00, but transaction requires $100.00",
  "instance": "/api/accounts/12345/transactions/67890",
  "traceId": "00-0f83f3c4d5e6f7a8b9c0d1e2f3a4b5c6-01",
  "balance": 50.00,
  "required": 100.00
}
```

### 7.2. Exception to HTTP Status Code Mapping

#### 7.2.1. Standard Exception Mappings

| Exception Type | HTTP Status | Problem Type | Usage |
|----------------|-------------|--------------|-------|
| ValidationException | 400 Bad Request | validation-error | Validation rule failures |
| NotFoundException | 404 Not Found | not-found | Resource does not exist |
| UnauthorizedException | 401 Unauthorized | unauthorized | Authentication required |
| ForbiddenException | 403 Forbidden | forbidden | Insufficient permissions |
| ConflictException | 409 Conflict | conflict | Business rule violation |
| DomainException | 422 Unprocessable Entity | domain-error | Domain constraint violation |
| Exception (unhandled) | 500 Internal Server Error | internal-error | Unexpected system error |

#### 7.2.2. Exception Handling Middleware

**Global Exception Handler:**

```csharp
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, problemType, title) = exception switch
        {
            ValidationException => (
                StatusCodes.Status400BadRequest,
                "validation-error",
                "One or more validation errors occurred"),

            NotFoundException => (
                StatusCodes.Status404NotFound,
                "not-found",
                "Resource not found"),

            UnauthorizedException => (
                StatusCodes.Status401Unauthorized,
                "unauthorized",
                "Authentication required"),

            ForbiddenException => (
                StatusCodes.Status403Forbidden,
                "forbidden",
                "Insufficient permissions"),

            ConflictException => (
                StatusCodes.Status409Conflict,
                "conflict",
                "Operation conflicts with current state"),

            DomainException => (
                StatusCodes.Status422UnprocessableEntity,
                "domain-error",
                "Domain constraint violation"),

            _ => (
                StatusCodes.Status500InternalServer Error,
                "internal-error",
                "An internal error occurred")
        };

        _logger.LogError(
            exception,
            "Exception occurred: {ExceptionType} - {Message}",
            exception.GetType().Name,
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Type = $"https://api.example.com/problems/{problemType}",
            Title = title,
            Status = statusCode,
            Detail = ShouldExposeDetail(statusCode) ? exception.Message : null,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            }
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static bool ShouldExposeDetail(int statusCode)
    {
        // Only expose details for client errors (4xx), not server errors (5xx)
        return statusCode >= 400 && statusCode < 500;
    }
}
```

### 7.3. Domain-Specific Problem Types

#### 7.3.1. Custom Problem Details

**Example: Business Rule Violation:**

```csharp
public sealed record InsufficientInventoryProblemDetails : ProblemDetails
{
    public InsufficientInventoryProblemDetails(
        Guid productId,
        int requestedQuantity,
        int availableQuantity)
    {
        Type = "https://api.example.com/problems/insufficient-inventory";
        Title = "Insufficient Inventory";
        Status = StatusCodes.Status422UnprocessableEntity;
        Detail = $"Requested {requestedQuantity} units but only {availableQuantity} available";
        Extensions["productId"] = productId;
        Extensions["requestedQuantity"] = requestedQuantity;
        Extensions["availableQuantity"] = availableQuantity;
    }
}
```

**Usage in Exception Handler:**

```csharp
public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
{
    if (exception is InsufficientInventoryException inventoryEx)
    {
        var problemDetails = new InsufficientInventoryProblemDetails(
            inventoryEx.ProductId,
            inventoryEx.RequestedQuantity,
            inventoryEx.AvailableQuantity);

        problemDetails.Instance = httpContext.Request.Path;
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    return false;
}
```

### 7.4. Error Logging Strategy

#### 7.4.1. Structured Error Logging

**Log Level Guidelines:**

| Status Code Range | Log Level | Rationale |
|-------------------|-----------|-----------|
| 400-499 (Client Errors) | Warning | Client issue, not system failure |
| 500-599 (Server Errors) | Error | System failure requiring attention |
| 503 (Service Unavailable) | Warning | Temporary condition |

**Logging Implementation:**

```csharp
_logger.LogWarning(
    exception,
    "Client error {StatusCode}: {ExceptionType} - {Message}. Path: {Path}, TraceId: {TraceId}",
    statusCode,
    exception.GetType().Name,
    exception.Message,
    httpContext.Request.Path,
    Activity.Current?.Id ?? httpContext.TraceIdentifier);
```

#### 7.4.2. Sensitive Information Protection

**Guidelines:**
- Never log passwords, tokens, or API keys
- Redact personal identifiable information (PII) when required
- Sanitize user input before logging
- Log exception type and message, not full stack traces in production

**Example Sanitization:**

```csharp
private static string SanitizeMessage(string message)
{
    // Remove potential sensitive patterns
    message = Regex.Replace(message, @"password[=:]\s*\S+", "password=***", RegexOptions.IgnoreCase);
    message = Regex.Replace(message, @"token[=:]\s*\S+", "token=***", RegexOptions.IgnoreCase);
    message = Regex.Replace(message, @"apikey[=:]\s*\S+", "apikey=***", RegexOptions.IgnoreCase);
    return message;
}
```

---

## 8. Correlation ID and Request Tracking

### 8.1. Correlation ID Implementation

#### 8.1.1. Correlation ID Middleware

**Purpose:**
- Track requests across distributed system components
- Correlate logs, traces, and metrics
- Support incident investigation
- Enable end-to-end request visibility

**Implementation:**

```csharp
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract or generate correlation ID
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        // Add to response headers
        context.Response.Headers.Append(CorrelationIdHeader, correlationId);

        // Add to logging context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.LogInformation(
                "Request started: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await _next(context);

            _logger.LogInformation(
                "Request completed: {Method} {Path} - {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode);
        }
    }
}
```

**Registration:**

```csharp
app.UseMiddleware<CorrelationIdMiddleware>();
```

#### 8.1.2. Correlation ID Propagation

**To Application Layer:**

```csharp
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = _httpContextAccessor.HttpContext?
            .Request.Headers["X-Correlation-ID"].FirstOrDefault();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.LogInformation(
                "Handling {RequestName}",
                typeof(TRequest).Name);

            var response = await next();

            return response;
        }
    }
}
```

**To External Services:**

```csharp
public sealed class ExternalServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task<T> CallExternalServiceAsync<T>(
        string endpoint,
        CancellationToken cancellationToken)
    {
        var correlationId = _httpContextAccessor.HttpContext?
            .Request.Headers["X-Correlation-ID"].FirstOrDefault();

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        
        if (!string.IsNullOrEmpty(correlationId))
        {
            request.Headers.Add("X-Correlation-ID", correlationId);
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);
        // Process response
    }
}
```

### 8.2. Distributed Tracing Integration

#### 8.2.1. OpenTelemetry Correlation

**Activity-Based Tracing:**

```csharp
using var activity = Activity.StartActivity("ProcessOrder");
activity?.SetTag("order.id", orderId);
activity?.SetTag("customer.id", customerId);

try
{
    // Process order
    activity?.SetTag("order.status", "completed");
}
catch (Exception ex)
{
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    throw;
}
```

**Integration with Correlation ID:**

```csharp
var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
```

#### 8.2.2. Trace Context Propagation

**W3C Trace Context Standard:**

```csharp
public sealed class TraceContextMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract traceparent header
        var traceParent = context.Request.Headers["traceparent"].FirstOrDefault();
        
        if (traceParent != null)
        {
            // Parse and set Activity context
            // traceparent format: 00-{trace-id}-{parent-id}-{trace-flags}
        }

        await _next(context);

        // Add traceparent to response
        if (Activity.Current != null)
        {
            context.Response.Headers.Append(
                "traceparent",
                $"00-{Activity.Current.TraceId}-{Activity.Current.SpanId}-{Activity.Current.ActivityTraceFlags:x2}");
        }
    }
}
```

---

## 9. Data Transformation and Mapping Strategy

### 9.1. Three-Layer Mapping Architecture

#### 9.1.1. Layer Definitions

**API Layer Models:**
- Purpose: External API contract
- Characteristics: Tailored to client needs, versioned with API
- Naming: {Operation}{Resource}Request/Response
- Location: API project

**Application Layer DTOs:**
- Purpose: Internal data transfer between layers
- Characteristics: Technology-agnostic, focused on use case
- Naming: {Resource}Dto
- Location: Application project

**Domain Layer Entities:**
- Purpose: Business logic encapsulation
- Characteristics: Rich behavior, invariant enforcement
- Naming: {Resource} (entity name)
- Location: Domain project

#### 9.1.2. Mapping Flow

```
Client Request (JSON)
       ↓ Deserialization
API Request Model (CreateProductRequest)
       ↓ Manual Mapping in Controller
Command/Query (CreateProductCommand)
       ↓ Handler Processing
Application DTO (ProductDto)
       ↓ Manual Mapping in Controller
API Response Model (ProductResponse)
       ↓ Serialization
Client Response (JSON)
```

### 9.2. Mapping Implementation Patterns

#### 9.2.1. Manual Mapping (Recommended)

**Request to Command:**

```csharp
var command = new CreateProductCommand(
    Name: request.Name,
    Description: request.Description,
    Price: request.Price,
    Currency: request.Currency,
    CategoryId: request.CategoryId);
```

**DTO to Response:**

```csharp
private static ProductResponse MapToResponse(ProductDto dto)
{
    return new ProductResponse
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        Currency = dto.Currency,
        CategoryId = dto.CategoryId,
        CategoryName = dto.CategoryName,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };
}
```

**Benefits:**
- Explicit and type-safe
- Easy to debug
- Clear data flow
- No hidden magic
- IDE refactoring support

#### 9.2.2. AutoMapper Integration (Optional)

**Configuration:**

```csharp
public sealed class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Domain to DTO
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));

        // DTO to Response
        CreateMap<ProductDto, ProductResponse>();

        // Request to Command (if using AutoMapper)
        CreateMap<CreateProductRequest, CreateProductCommand>();
    }
}
```

**Usage:**

```csharp
private readonly IMapper _mapper;

var response = _mapper.Map<ProductResponse>(dto);
```

**Trade-offs:**
- Pros: Less boilerplate code, convention-based mapping
- Cons: Runtime errors, harder to debug, hidden behavior, performance overhead

#### 9.2.3. Mapping Extensions

**Extension Method Pattern:**

```csharp
public static class ProductMappingExtensions
{
    public static ProductResponse ToResponse(this ProductDto dto)
    {
        return new ProductResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Currency = dto.Currency,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    public static CreateProductCommand ToCommand(this CreateProductRequest request)
    {
        return new CreateProductCommand(
            Name: request.Name,
            Description: request.Description,
            Price: request.Price,
            Currency: request.Currency,
            CategoryId: request.CategoryId);
    }
}
```

**Usage:**

```csharp
var command = request.ToCommand();
var response = dto.ToResponse();
```

### 9.3. Complex Mapping Scenarios

#### 9.3.1. Collection Mapping

**Manual Collection Mapping:**

```csharp
var responses = dtos
    .Select(dto => MapToResponse(dto))
    .ToList();
```

**Paged Collection Mapping:**

```csharp
var pagedResponse = new PagedResponse<ProductResponse>
{
    Items = result.Items.Select(MapToResponse).ToList(),
    TotalCount = result.TotalCount,
    PageNumber = result.PageNumber,
    PageSize = result.PageSize,
    TotalPages = result.TotalPages
};
```

#### 9.3.2. Nested Object Mapping

**Flattening:**

```csharp
public static ProductResponse MapToResponse(ProductDto dto)
{
    return new ProductResponse
    {
        Id = dto.Id,
        Name = dto.Name,
        // Flatten nested category
        CategoryId = dto.Category.Id,
        CategoryName = dto.Category.Name,
        CategoryCode = dto.Category.Code
    };
}
```

**Composition:**

```csharp
public static OrderResponse MapToResponse(OrderDto dto)
{
    return new OrderResponse
    {
        Id = dto.Id,
        OrderNumber = dto.OrderNumber,
        // Compose nested objects
        Customer = new CustomerSummary
        {
            Id = dto.CustomerId,
            Name = dto.CustomerName,
            Email = dto.CustomerEmail
        },
        Items = dto.Items.Select(MapToOrderItemResponse).ToList()
    };
}
```

#### 9.3.3. Conditional Mapping

**Field Selection:**

```csharp
public static ProductResponse MapToResponse(
    ProductDto dto,
    bool includeDetails = true)
{
    var response = new ProductResponse
    {
        Id = dto.Id,
        Name = dto.Name,
        Price = dto.Price
    };

    if (includeDetails)
    {
        response.Description = dto.Description;
        response.Specifications = dto.Specifications;
        response.Reviews = dto.Reviews?.Select(MapToReviewResponse).ToList();
    }

    return response;
}
```

### 9.4. Mapping Best Practices

#### 9.4.1. Guidelines

**Do:**
- Keep mapping logic simple and focused
- Use explicit property assignments for clarity
- Create separate mapping methods for different use cases
- Test mapping logic thoroughly
- Document complex mappings

**Don't:**
- Include business logic in mapping methods
- Perform database queries during mapping
- Mix mapping concerns with validation
- Create circular mapping dependencies
- Over-engineer mapping infrastructure

#### 9.4.2. Performance Considerations

**Optimization Strategies:**
- Avoid N+1 mapping scenarios
- Use projection in queries when possible
- Cache complex mapping configurations
- Profile mapping performance in critical paths
- Consider compiled expressions for AutoMapper

---

## 10. Observability Integration

### 10.1. Structured Logging

#### 10.1.1. Serilog Configuration

**Program.cs Setup:**

```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "ProductApi")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Seq(configuration["Seq:ServerUrl"])
    .CreateLogger();

builder.Host.UseSerilog();
```

#### 10.1.2. Request Logging

**Middleware Logging:**

```csharp
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        using (LogContext.PushProperty("RequestMethod", context.Request.Method))
        using (LogContext.PushProperty("UserId", context.User?.FindFirst("sub")?.Value))
        {
            _logger.LogInformation(
                "Request started: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            try
            {
                await _next(context);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Request completed: {Method} {Path} - {StatusCode} in {ElapsedMs}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Request failed: {Method} {Path} after {ElapsedMs}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
```

#### 10.1.3. Controller Action Logging

**Action Filter:**

```csharp
public sealed class LoggingActionFilter : IAsyncActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;

    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var controllerName = context.Controller.GetType().Name;
        var actionName = context.ActionDescriptor.DisplayName;

        _logger.LogInformation(
            "Executing action: {Controller}.{Action}",
            controllerName,
            actionName);

        var executedContext = await next();

        if (executedContext.Exception != null)
        {
            _logger.LogError(
                executedContext.Exception,
                "Action failed: {Controller}.{Action}",
                controllerName,
                actionName);
        }
        else
        {
            _logger.LogInformation(
                "Action executed: {Controller}.{Action} - Result: {ResultType}",
                controllerName,
                actionName,
                executedContext.Result?.GetType().Name);
        }
    }
}
```

### 10.2. Metrics Collection

#### 10.2.1. OpenTelemetry Metrics

**Configuration:**

```csharp
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

app.UseOpenTelemetryPrometheusScrapingEndpoint();
```

#### 10.2.2. Custom Metrics

**Metrics Collector:**

```csharp
public sealed class ApiMetrics
{
    private readonly Meter _meter;
    private readonly Counter<long> _requestCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly Counter<long> _errorCounter;

    public ApiMetrics()
    {
        _meter = new Meter("Api.Metrics");
        
        _requestCounter = _meter.CreateCounter<long>(
            "api.requests.total",
            description: "Total number of API requests");
            
        _requestDuration = _meter.CreateHistogram<double>(
            "api.request.duration",
            unit: "ms",
            description: "API request duration");
            
        _errorCounter = _meter.CreateCounter<long>(
            "api.errors.total",
            description: "Total number of API errors");
    }

    public void RecordRequest(string endpoint, string method)
    {
        _requestCounter.Add(1, 
            new KeyValuePair<string, object?>("endpoint", endpoint),
            new KeyValuePair<string, object?>("method", method));
    }

    public void RecordDuration(string endpoint, double durationMs)
    {
        _requestDuration.Record(durationMs,
            new KeyValuePair<string, object?>("endpoint", endpoint));
    }

    public void RecordError(string endpoint, string errorType)
    {
        _errorCounter.Add(1,
            new KeyValuePair<string, object?>("endpoint", endpoint),
            new KeyValuePair<string, object?>("error_type", errorType));
    }
}
```

**Usage in Middleware:**

```csharp
public async Task InvokeAsync(HttpContext context)
{
    var stopwatch = Stopwatch.StartNew();
    var endpoint = $"{context.Request.Method} {context.Request.Path}";

    _metrics.RecordRequest(endpoint, context.Request.Method);

    try
    {
        await _next(context);
        
        stopwatch.Stop();
        _metrics.RecordDuration(endpoint, stopwatch.Elapsed.TotalMilliseconds);
    }
    catch (Exception ex)
    {
        _metrics.RecordError(endpoint, ex.GetType().Name);
        throw;
    }
}
```

### 10.3. Distributed Tracing

#### 10.3.1. OpenTelemetry Tracing

**Configuration:**

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.Filter = (httpContext) =>
                {
                    // Exclude health checks from tracing
                    return !httpContext.Request.Path.StartsWithSegments("/health");
                };
            })
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource("Application.*")
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("ProductApi")
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = builder.Environment.EnvironmentName
                }))
            .AddJaegerExporter()
            .AddConsoleExporter();
    });
```

#### 10.3.2. Custom Spans

**Creating Custom Spans:**

```csharp
using var activity = Activity.StartActivity("ProcessComplexOperation");
activity?.SetTag("operation.type", "data-processing");
activity?.SetTag("record.count", recordCount);

try
{
    // Perform operation
    
    activity?.SetTag("operation.status", "success");
    activity?.SetTag("records.processed", processedCount);
}
catch (Exception ex)
{
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    activity?.RecordException(ex);
    throw;
}
```

### 10.4. Health Checks

#### 10.4.1. Health Check Implementation

**Configuration:**

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database")
    .AddUrlGroup(new Uri(configuration["ExternalService:Url"]), "external-service")
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

#### 10.4.2. Custom Health Checks

```csharp
public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;

    public DatabaseHealthCheck(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            
            return HealthCheckResult.Healthy("Database is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Database is not accessible",
                ex);
        }
    }
}
```

---

## 11. Performance Characteristics

### 11.1. Performance Targets

#### 11.1.1. Response Time Targets

| Operation Type | Target (p50) | Target (p95) | Target (p99) |
|----------------|--------------|--------------|--------------|
| Simple GET | < 20ms | < 50ms | < 100ms |
| Complex GET | < 100ms | < 200ms | < 500ms |
| Simple POST/PUT | < 50ms | < 100ms | < 200ms |
| Complex POST/PUT | < 200ms | < 500ms | < 1000ms |
| DELETE | < 30ms | < 100ms | < 200ms |

#### 11.1.2. Throughput Targets

| Metric | Target | Notes |
|--------|--------|-------|
| Requests per second | > 1000 | Per instance, simple operations |
| Concurrent requests | > 100 | Sustained load |
| Peak throughput | > 2000 | Short duration bursts |

### 11.2. Performance Optimization

#### 11.2.1. Response Caching

**Output Cache Configuration:**

```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder
        .Expire(TimeSpan.FromMinutes(5))
        .Tag("api"));
        
    options.AddPolicy("products", builder => builder
        .Expire(TimeSpan.FromMinutes(10))
        .SetVaryByQuery("categoryId", "searchTerm")
        .Tag("products"));
});

// Usage
[HttpGet]
[OutputCache(PolicyName = "products")]
public async Task<ActionResult<List<ProductResponse>>> GetProducts(
    [FromQuery] string? categoryId,
    [FromQuery] string? searchTerm)
{
    // Implementation
}
```

#### 11.2.2. Response Compression

```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

app.UseResponseCompression();
```

#### 11.2.3. Asynchronous Processing

**Background Job Pattern:**

```csharp
[HttpPost("process")]
public async Task<ActionResult<ProcessingResponse>> StartProcessing(
    [FromBody] ProcessRequest request)
{
    var jobId = Guid.NewGuid();
    
    // Queue background job
    _backgroundJobQueue.QueueBackgroundWorkItem(async token =>
    {
        await ProcessDataAsync(jobId, request, token);
    });

    return Accepted(new ProcessingResponse
    {
        JobId = jobId,
        Status = "Processing",
        StatusUrl = Url.Action(nameof(GetProcessingStatus), new { jobId })
    });
}

[HttpGet("process/{jobId}/status")]
public async Task<ActionResult<ProcessingStatusResponse>> GetProcessingStatus(
    Guid jobId)
{
    var status = await _jobStatusRepository.GetStatusAsync(jobId);
    
    if (status == null)
        return NotFound();
        
    return Ok(status);
}
```

---

## 12. Security Considerations

### 12.1. Input Validation and Sanitization

#### 12.1.1. Validation Requirements

**All Inputs Must Be Validated:**
- Request body parameters
- Route parameters
- Query string parameters
- HTTP headers

**Validation Types:**
- Format validation (type, length, pattern)
- Range validation (min/max values)
- Business rule validation
- Cross-field validation

#### 12.1.2. SQL Injection Prevention

**Use Parameterized Queries:**

```csharp
// Correct: Parameterized query
var products = await _connection.QueryAsync<Product>(
    "SELECT * FROM Products WHERE CategoryId = @CategoryId",
    new { CategoryId = categoryId });

// NEVER: String concatenation
// var sql = $"SELECT * FROM Products WHERE CategoryId = '{categoryId}'";
```

**Entity Framework Core Safety:**
- EF Core automatically parameterizes queries
- LINQ queries are safe from SQL injection
- Always use parameters for raw SQL

#### 12.1.3. XSS Prevention

**Output Encoding:**
- ASP.NET Core automatically encodes output
- JSON serialization escapes special characters
- HTML content should use HTML encoding

**Content Security Policy:**

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "default-src 'self'; script-src 'self'; style-src 'self'");
    await next();
});
```

### 12.2. Rate Limiting

#### 12.2.1. Rate Limiting Configuration

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userId = context.User?.FindFirst("sub")?.Value ?? "anonymous";
        
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        
        await context.HttpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Too Many Requests",
                Detail = "Rate limit exceeded. Please try again later."
            },
            cancellationToken: token);
    };
});

app.UseRateLimiter();
```

### 12.3. Security Headers

#### 12.3.1. Standard Security Headers

```csharp
app.Use(async (context, next) =>
{
    // Prevent clickjacking
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    
    // Prevent MIME sniffing
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    
    // Enable XSS protection
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    
    // Enforce HTTPS
    context.Response.Headers.Append(
        "Strict-Transport-Security",
        "max-age=31536000; includeSubDomains");
    
    // Referrer policy
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    
    await next();
});
```

---

## 13. Testing Strategy

### 13.1. API Integration Testing

#### 13.1.1. Test Infrastructure

```csharp
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ValidRequest_ReturnsCreatedProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Currency = "USD",
            CategoryId = Guid.NewGuid()
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        product.Should().NotBeNull();
        product!.Name.Should().Be(request.Name);
        product.Price.Should().Be(request.Price);
    }

    [Fact]
    public async Task CreateProduct_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "", // Invalid: empty name
            Price = -10, // Invalid: negative price
            Currency = "INVALID", // Invalid: not 3 letters
            CategoryId = Guid.Empty // Invalid: empty guid
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
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

### 13.2. Controller Unit Testing

#### 13.2.1. Controller Test Example

```csharp
public class ProductsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ProductsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            Price = 99.99m,
            Currency = "USD",
            CategoryId = Guid.NewGuid()
        };

        var expectedDto = new ProductDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Currency = request.Currency
        };

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(_controller.GetById));

        var response = createdResult.Value.Should().BeOfType<ProductResponse>().Subject;
        response.Id.Should().Be(expectedDto.Id);
        response.Name.Should().Be(expectedDto.Name);
    }

    [Fact]
    public async Task GetById_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mediatorMock
            .Setup(x => x.Send(
                It.Is<GetProductByIdQuery>(q => q.Id == productId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.GetById(productId, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
```

---

## 14. Glossary

**API (Application Programming Interface):** A set of definitions and protocols for building and integrating application software.

**Binding:** The process of converting HTTP request data into C# objects.

**Content Negotiation:** The mechanism for serving different representations of a resource based on client preferences.

**Correlation ID:** A unique identifier assigned to a request and propagated through all system components for tracing purposes.

**DTO (Data Transfer Object):** An object that carries data between processes or layers without containing business logic.

**HTTP Status Code:** A three-digit code indicating the status of an HTTP response.

**MediatR:** A .NET library implementing the Mediator pattern for in-process messaging.

**Middleware:** Software components in the ASP.NET Core pipeline that process HTTP requests and responses.

**Model Binding:** The process of converting HTTP request data into action method parameters.

**OpenTelemetry:** An open-source observability framework for generating, collecting, and exporting telemetry data.

**Problem Details:** RFC 7807 standardized format for HTTP API error responses.

**RESTful API:** An API that follows REST architectural constraints and uses HTTP methods semantically.

**Serialization:** The process of converting an object into a format (typically JSON) for transmission.

**Structured Logging:** Logging approach where log entries are treated as data structures rather than text strings.

**Trace Context:** Information propagated across service boundaries to correlate operations in distributed systems.

**Validation:** The process of ensuring data meets specified constraints and business rules.

---

## 15. Recommendations and Next Steps

### 15.1. For Development Teams

#### 15.1.1. Implementation Checklist

**When Creating New Endpoints:**
- [ ] Define RESTful URL structure following conventions
- [ ] Create request and response models in API project
- [ ] Implement controller action with proper HTTP method
- [ ] Add XML documentation comments
- [ ] Add ProducesResponseType attributes
- [ ] Map request to command/query
- [ ] Implement response mapping
- [ ] Add correlation ID to logs
- [ ] Write integration tests
- [ ] Update API documentation

**When Handling Errors:**
- [ ] Use appropriate exception types
- [ ] Include correlation ID in logs
- [ ] Return Problem Details format
- [ ] Use correct HTTP status codes
- [ ] Protect sensitive information
- [ ] Test error scenarios

#### 15.1.2. Code Review Focus Areas

**API Layer Review:**
- No business logic in controllers
- Proper HTTP status code usage
- Consistent error handling
- Appropriate logging
- Request/response mapping correctness
- Documentation completeness

**Request Processing Review:**
- Model binding correctness
- Validation completeness
- Correlation ID propagation
- Security considerations
- Performance implications

### 15.2. For Technical Leads

#### 15.2.1. Quality Gates

**Pre-Deployment Checklist:**
- [ ] All endpoints have integration tests
- [ ] Error scenarios covered in tests
- [ ] Logging includes correlation IDs
- [ ] Performance targets met
- [ ] Security headers configured
- [ ] Rate limiting implemented
- [ ] Health checks operational
- [ ] OpenAPI documentation generated

#### 15.2.2. Monitoring Setup

**Operational Metrics:**
- Request rate by endpoint
- Response time percentiles (p50, p95, p99)
- Error rate by status code
- Validation failure rate
- Exception frequency by type

**Alert Configuration:**
- Error rate > 1% for 5 minutes
- p95 latency > 2x target for 10 minutes
- Health check failures
- Rate limit threshold exceeded

### 15.3. For Architects

#### 15.3.1. Architecture Governance

**Periodic Reviews:**
- API design consistency
- Error handling patterns
- Performance characteristics
- Security posture
- Observability coverage

**Documentation Updates:**
- API guidelines
- Error catalog
- Performance baselines
- Security policies

#### 15.3.2. Evolution Planning

**API Versioning:**
- Establish versioning strategy
- Plan breaking change management
- Define deprecation process
- Communicate changes to consumers

**Performance Optimization:**
- Identify bottlenecks
- Implement caching strategies
- Optimize database queries
- Consider CDN for static content

### 15.4. Related Documentation

**Must Read:**
- High-Level Architecture (ARCH-HL-001)
- CQRS Pipeline Specification (ARCH-CQRS-002)
- Domain Modeling Guide (ARCH-DOMAIN-004)

**Recommended Reading:**
- API Versioning Strategy
- Authentication and Authorization Specification
- Rate Limiting and Throttling Guide
- Performance Optimization Guide
- Security Best Practices

---

## 16. References

### 16.1. Standards and Specifications

**RFC 7807 - Problem Details for HTTP APIs**
- https://tools.ietf.org/html/rfc7807

**RFC 7230-7235 - HTTP/1.1 Specification**
- https://tools.ietf.org/html/rfc7230

**W3C Trace Context**
- https://www.w3.org/TR/trace-context/

**OpenAPI Specification 3.0**
- https://swagger.io/specification/

### 16.2. Framework Documentation

**ASP.NET Core**
- https://docs.microsoft.com/aspnet/core/

**MediatR**
- https://github.com/jbogard/MediatR

**Serilog**
- https://serilog.net/

**OpenTelemetry .NET**
- https://opentelemetry.io/docs/instrumentation/net/

### 16.3. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- ARCH-HL-001: High-Level Architecture
- ARCH-CQRS-002: CQRS Pipeline Specification
- ARCH-DOMAIN-004: Domain Modeling Guide

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial API flow and request processing specification with standardized structure |

 