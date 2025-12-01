# API Design Guidelines and Standards
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | API-GUIDELINES-001 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Standards and Guidelines |
| Target Audience | API Designers, Solution Architects, Development Teams, Technical Leads, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Annual |
| Related Documents | High-Level Architecture (ARCH-HL-001), API Flow (ARCH-API-003), Error Handling (API-ERRORHANDLING-001), Versioning (API-VERSIONING-001) |
| Prerequisites | Understanding of REST principles, HTTP protocol, JSON format |

---

## Executive Summary

This document establishes comprehensive API design guidelines and standards for the .NET Enterprise Architecture Template, providing authoritative direction for building consistent, predictable, and enterprise-grade RESTful APIs. These guidelines ensure uniform implementation patterns across all API endpoints, facilitating client integration, reducing development costs, and supporting long-term maintainability.

**Strategic Business Value:**
- Reduced integration costs through consistent API patterns
- Accelerated time-to-market via standardized development approaches
- Enhanced developer experience leading to increased API adoption
- Minimized support burden through predictable behavior
- Improved system reliability via proven architectural patterns

**Key Technical Capabilities:**
- Resource-oriented REST design principles
- Comprehensive HTTP protocol semantic utilization
- Standardized request/response contract patterns
- RFC 7807 compliant error handling
- Structured pagination, filtering, and sorting mechanisms
- Path-based API versioning strategy
- Security-first authentication and authorization patterns

**Compliance and Standards:**
- REST architectural constraints (Roy Fielding's dissertation)
- HTTP/1.1 specification (RFC 7230-7235)
- RFC 7807 Problem Details for HTTP APIs
- OpenAPI Specification 3.0 for API documentation
- OAuth 2.0 / OpenID Connect for authentication
- WCAG 2.1 accessibility for API documentation

---

## Table of Contents

1. Introduction and Scope
2. Core Design Principles
3. URL Structure and Conventions
4. Resource Naming Standards
5. HTTP Method Specifications
6. Request and Response Contract Design
7. Pagination, Filtering, and Sorting
8. Error Handling Standards
9. Authentication and Authorization
10. Idempotency and Safety Requirements
11. Versioning and Breaking Changes
12. API Documentation Requirements
13. Testing Standards
14. Performance and Scalability Guidelines
15. Glossary
16. Recommendations and Next Steps
17. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides authoritative API design guidelines for all HTTP APIs developed using the .NET Enterprise Architecture Template. It establishes mandatory standards, recommended practices, and architectural patterns ensuring consistency, maintainability, and professional quality across all API implementations.

### 1.2. Scope

**In Scope:**
- RESTful API design principles and patterns
- HTTP method usage and semantics
- URL structure and naming conventions
- Request and response contract design
- Error handling and status code usage
- Authentication and authorization patterns
- Pagination, filtering, and sorting standards
- API versioning strategy
- Documentation requirements
- Testing approaches

**Out of Scope:**
- GraphQL API design (separate specification if needed)
- gRPC protocol specifications
- WebSocket implementations
- Internal service-to-service communication patterns
- Database design and schema definitions
- Infrastructure deployment configurations
- Specific business domain logic

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**API Designers:** Comprehensive design patterns and standards

**Solution Architects:** Architectural governance and compliance verification

**Development Teams:** Implementation guidelines and code examples

**Technical Leads:** Code review criteria and quality gates

**Frontend Developers:** Integration patterns and contract specifications

**C-Level Executives:** Business value and strategic alignment

### 1.4. Relationship to Other Standards

These guidelines integrate with and reference:

**Internal Standards:**
- High-Level Architecture (ARCH-HL-001)
- API Flow Specification (ARCH-API-003)
- Error Handling Specification (API-ERRORHANDLING-001)
- Versioning Strategy (API-VERSIONING-001)

**External Standards:**
- REST architectural style
- HTTP/1.1 specification
- RFC 7807 Problem Details
- OpenAPI Specification 3.0

---

## 2. Core Design Principles

### 2.1. Fundamental Principles

#### 2.1.1. HTTP as Application Protocol

**Principle:** Utilize HTTP as a true application protocol, respecting its semantics and design.

**Requirements:**
- Use HTTP methods according to their defined semantics
- Leverage HTTP status codes appropriately
- Utilize HTTP headers for metadata
- Respect caching mechanisms
- Follow idempotency and safety definitions

**Anti-Pattern:**
- Tunneling all operations through POST
- Using GET for state-changing operations
- Ignoring HTTP status code meanings
- Custom status code interpretations

**Rationale:** HTTP provides rich semantics designed for distributed systems. Proper usage enables standard tooling, caching, and client libraries to work effectively.

#### 2.1.2. Resource-Oriented Design

**Principle:** Model APIs around resources (nouns) rather than actions (verbs).

**Resource Characteristics:**
- Represents a business entity or concept
- Has a unique identifier
- Can be manipulated through standard operations
- May contain relationships to other resources

**Examples:**

**Correct (Resource-Oriented):**
```
GET    /api/v1/products
POST   /api/v1/products
GET    /api/v1/products/{id}
PUT    /api/v1/products/{id}
DELETE /api/v1/products/{id}
```

**Incorrect (Action-Oriented):**
```
POST /api/v1/createProduct
POST /api/v1/updateProduct
POST /api/v1/deleteProduct
POST /api/v1/getProduct
```

**Exception:** Operations that don't fit CRUD patterns may use action verbs:
```
POST /api/v1/products/{id}/activate
POST /api/v1/orders/{id}/cancel
POST /api/v1/users/{id}/reset-password
```

**Rationale:** Resource-oriented design provides consistency, predictability, and alignment with REST principles.

#### 2.1.3. Predictability and Consistency

**Principle:** Similar operations should exhibit identical patterns across all resources.

**Consistency Requirements:**

| Aspect | Standard | Example |
|--------|----------|---------|
| Collection retrieval | GET /{resource} | GET /products, GET /orders |
| Single resource retrieval | GET /{resource}/{id} | GET /products/{id} |
| Resource creation | POST /{resource} | POST /products |
| Resource replacement | PUT /{resource}/{id} | PUT /products/{id} |
| Resource deletion | DELETE /{resource}/{id} | DELETE /products/{id} |
| Pagination parameters | ?page=X&pageSize=Y | Same for all collections |
| Filtering parameters | ?fieldName=value | Same pattern for all resources |
| Sorting parameters | ?sort=field_direction | Same pattern for all resources |

**Benefits:**
- Reduced learning curve for API consumers
- Simplified client library development
- Easier documentation maintenance
- Predictable behavior reduces integration errors

#### 2.1.4. Separation of Concerns

**Principle:** API layer focuses exclusively on HTTP concerns and contract management.

**Layer Responsibilities:**

**API Layer (Controllers):**
- HTTP request/response handling
- Input validation (format, type)
- Status code selection
- Header management
- Request/response mapping

**Application Layer:**
- Use case orchestration
- Business validation
- Transaction management
- Domain event handling

**Domain Layer:**
- Business logic
- Invariant enforcement
- Domain rules

**Anti-Pattern:**
```csharp
// BAD: Business logic in controller
[HttpPost]
public async Task<IActionResult> CreateProduct(CreateProductRequest request)
{
    // Don't do this in controller!
    if (request.Price < 0)
        return BadRequest("Price cannot be negative");
    
    var product = new Product
    {
        Name = request.Name,
        Price = request.Price
    };
    
    _context.Products.Add(product);
    await _context.SaveChangesAsync();
    
    return Ok(product);
}
```

**Correct Pattern:**
```csharp
// GOOD: Thin controller delegates to application layer
[HttpPost]
public async Task<IActionResult> CreateProduct(CreateProductRequest request)
{
    var command = new CreateProductCommand(
        request.Name,
        request.Price,
        request.Currency);
    
    var result = await _mediator.Send(command);
    
    return CreatedAtAction(
        nameof(GetProduct),
        new { id = result.Id },
        result);
}
```

#### 2.1.5. Backward Compatibility

**Principle:** Never break existing client integrations without explicit versioning.

**Compatibility Rules:**
- Published API contracts are immutable within a version
- Breaking changes require new major version
- Non-breaking changes permissible within version
- Deprecation communicated with minimum 12-month notice

**Non-Breaking Changes:**
- Adding optional request fields
- Adding response fields
- Adding new endpoints
- Adding new HTTP methods to existing resources
- Expanding validation (less strict)

**Breaking Changes:**
- Removing request or response fields
- Renaming fields
- Changing field types
- Changing endpoint behavior
- Removing endpoints
- Stricter validation

---

## 3. URL Structure and Conventions

### 3.1. Base URL Pattern

#### 3.1.1. Standard Structure

**Format:**
```
{scheme}://{hostname}/api/v{major}/{resource-path}
```

**Components:**

| Component | Required | Format | Example |
|-----------|----------|--------|---------|
| scheme | Yes | http or https | https |
| hostname | Yes | Domain name | api.example.com |
| api | Yes | Fixed prefix | api |
| v{major} | Yes | Version number | v1, v2, v3 |
| resource-path | Yes | Resource hierarchy | products, products/{id} |

**Complete Examples:**
```
https://api.example.com/api/v1/products
https://api.example.com/api/v1/products/{id}
https://api.example.com/api/v1/orders/{orderId}/items
https://api.example.com/api/v2/customers/{id}/addresses
```

#### 3.1.2. URL Component Guidelines

**Hostname:**
- Use subdomain for API (api.example.com)
- Separate from main application domain
- Enables independent scaling and routing
- Supports API gateway deployment

**API Prefix:**
- Always include /api prefix
- Distinguishes API routes from other routes
- Enables routing rules and policies
- Conventional across industry

**Version Number:**
- Always include major version
- Place before resource path
- Format: v{integer} (v1, v2, v3)
- Detailed in API-VERSIONING-001

**Resource Path:**
- Use lowercase
- Use hyphens for multi-word resources (compound-resource)
- Never use underscores
- Keep hierarchy shallow (maximum 3 levels recommended)

### 3.2. Resource Path Patterns

#### 3.2.1. Collection Resources

**Pattern:** `/{resource-collection}`

**Examples:**
```
GET /api/v1/products
GET /api/v1/orders
GET /api/v1/customers
```

**Characteristics:**
- Always plural nouns
- Returns collection (typically paginated)
- Supports filtering and sorting via query parameters

#### 3.2.2. Individual Resources

**Pattern:** `/{resource-collection}/{id}`

**Examples:**
```
GET /api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
PUT /api/v1/orders/a1b2c3d4-e5f6-7890-abcd-ef1234567890
DELETE /api/v1/customers/c2d3e4f5-g6h7-8901-ijkl-mn2345678901
```

**Identifier Guidelines:**
- Use GUID format for public APIs (enhances security)
- Consistent format across all resources
- Avoid sequential integers in public APIs (prevents enumeration)

#### 3.2.3. Sub-Resources

**Pattern:** `/{resource-collection}/{id}/{sub-resource-collection}`

**Examples:**
```
GET /api/v1/orders/{orderId}/items
GET /api/v1/products/{productId}/reviews
POST /api/v1/customers/{customerId}/addresses
```

**Guidelines:**
- Use for resources inherently belonging to parent
- Limit nesting depth (prefer flat structure)
- Consider independent top-level resource if relationship not strong

**Decision Framework:**

Use sub-resource when:
- Child cannot exist without parent
- Child always accessed through parent
- Strong ownership relationship

Use independent resource when:
- Child can exist independently
- Child accessed from multiple contexts
- Relationship is reference rather than ownership

#### 3.2.4. Custom Actions

**Pattern:** `/{resource-collection}/{id}/{action}`

**Examples:**
```
POST /api/v1/products/{id}/activate
POST /api/v1/orders/{id}/cancel
POST /api/v1/users/{id}/reset-password
POST /api/v1/documents/{id}/publish
```

**Guidelines:**
- Use only when standard CRUD insufficient
- Always use POST method
- Use verb for action name
- Keep action names short and descriptive
- Document side effects clearly

**When to Use Custom Actions:**
- State transitions not represented by simple update
- Operations with complex side effects
- Business processes that don't map to CRUD
- Operations that feel unnatural as updates

---

## 4. Resource Naming Standards

### 4.1. Collection Naming Rules

#### 4.1.1. Plural Nouns

**Rule:** Always use plural nouns for collection resources.

**Correct:**
```
/api/v1/products
/api/v1/orders
/api/v1/users
/api/v1/categories
```

**Incorrect:**
```
/api/v1/product
/api/v1/order
/api/v1/user
```

**Rationale:** 
- Indicates collection semantics
- Consistent with English language conventions
- Industry standard practice

#### 4.1.2. Lowercase with Hyphens

**Rule:** Use lowercase letters with hyphens for multi-word resources.

**Correct:**
```
/api/v1/product-categories
/api/v1/shipping-addresses
/api/v1/order-items
```

**Incorrect:**
```
/api/v1/ProductCategories      # PascalCase
/api/v1/product_categories     # snake_case
/api/v1/productCategories      # camelCase
```

**Rationale:**
- URLs are case-insensitive in many contexts
- Hyphens improve readability
- Consistent with URL best practices

#### 4.1.3. Business Domain Alignment

**Rule:** Use names that reflect business domain concepts.

**Good Examples:**
- /api/v1/products (not /api/v1/items)
- /api/v1/customers (not /api/v1/users when referring to purchasers)
- /api/v1/orders (not /api/v1/transactions when referring to purchase orders)

**Rationale:** API should reflect ubiquitous language from domain-driven design

### 4.2. Query Parameter Naming

#### 4.2.1. Standard Parameters

| Parameter | Purpose | Format | Example |
|-----------|---------|--------|---------|
| page | Pagination page number | Integer (1-based) | ?page=2 |
| pageSize | Items per page | Integer | ?pageSize=20 |
| sort | Sort field and direction | field_direction | ?sort=price_desc |
| search | Free-text search | String | ?search=phone |
| filter | Generic filtering | Various | ?filter=active |

#### 4.2.2. Field-Specific Filters

**Format:** Use field name as parameter name

**Examples:**
```
?name=widget
?category=electronics
?status=active
?minPrice=10&maxPrice=100
?createdAfter=2025-01-01
```

**Guidelines:**
- Use camelCase for multi-word field names
- Prefix range filters with min/max
- Use consistent naming with response fields

### 4.3. Header Naming

#### 4.3.1. Standard Headers

**Request Headers:**
```
Accept: application/json
Content-Type: application/json
Authorization: Bearer {token}
X-Correlation-ID: {guid}
```

**Response Headers:**
```
Content-Type: application/json; charset=utf-8
X-Correlation-ID: {guid}
Location: /api/v1/products/{id}
API-Version: 1.0
```

#### 4.3.2. Custom Header Convention

**Format:** Prefix custom headers with `X-`

**Examples:**
```
X-Request-ID
X-Client-Version
X-Feature-Flags
```

**Note:** X- prefix is deprecated in RFC 6648 but remains conventional for clarity.

---

## 5. HTTP Method Specifications

### 5.1. GET Method

#### 5.1.1. Specification

**Purpose:** Retrieve resource representation without side effects.

**Characteristics:**
- Safe: Does not modify server state
- Idempotent: Multiple identical requests have same effect as single request
- Cacheable: Responses may be cached

**Usage Patterns:**

| Pattern | URL | Purpose |
|---------|-----|---------|
| Collection retrieval | GET /api/v1/products | List all products (paginated) |
| Single resource | GET /api/v1/products/{id} | Retrieve specific product |
| Filtered collection | GET /api/v1/products?category=electronics | List filtered products |

#### 5.1.2. Request Specification

**Query Parameters Only:**
- No request body permitted
- All parameters in URL query string
- Support pagination, filtering, sorting

**Example:**
```http
GET /api/v1/products?page=1&pageSize=20&category=electronics&sort=price_asc HTTP/1.1
Host: api.example.com
Accept: application/json
Authorization: Bearer {token}
```

#### 5.1.3. Response Specification

**Success Response (200 OK):**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000

{
  "items": [...],
  "page": 1,
  "pageSize": 20,
  "totalItems": 150,
  "totalPages": 8
}
```

**Not Found Response (404 Not Found):**
```http
HTTP/1.1 404 Not Found
Content-Type: application/json; charset=utf-8

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource not found",
  "status": 404,
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

### 5.2. POST Method

#### 5.2.1. Specification

**Purpose:** Create new resource or execute non-idempotent operation.

**Characteristics:**
- Not safe: Modifies server state
- Not idempotent: Multiple requests create multiple resources
- Not cacheable

**Usage Patterns:**

| Pattern | URL | Purpose |
|---------|-----|---------|
| Resource creation | POST /api/v1/products | Create new product |
| Custom action | POST /api/v1/products/{id}/activate | Execute activation |
| Search operation | POST /api/v1/products/search | Complex search (when GET insufficient) |

#### 5.2.2. Request Specification

**Request Body Required:**
```http
POST /api/v1/products HTTP/1.1
Host: api.example.com
Content-Type: application/json
Accept: application/json
Authorization: Bearer {token}

{
  "name": "Widget Pro",
  "description": "Professional widget",
  "price": 99.99,
  "currency": "USD"
}
```

#### 5.2.3. Response Specification

**Success Response (201 Created):**
```http
HTTP/1.1 201 Created
Location: /api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000

{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Widget Pro",
  "description": "Professional widget",
  "price": 99.99,
  "currency": "USD",
  "createdAt": "2025-01-15T10:30:00Z"
}
```

**Location Header:**
- Required for 201 responses
- Contains URI of created resource
- Enables client to immediately retrieve resource

### 5.3. PUT Method

#### 5.3.1. Specification

**Purpose:** Replace entire resource with new representation.

**Characteristics:**
- Not safe: Modifies server state
- Idempotent: Multiple identical requests have same effect
- Full replacement: All fields must be provided

**Usage Pattern:**
```
PUT /api/v1/products/{id}
```

#### 5.3.2. Request Specification

**Complete Resource Representation:**
```http
PUT /api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Widget Pro Max",
  "description": "Professional widget - updated",
  "price": 129.99,
  "currency": "USD",
  "isActive": true
}
```

**Requirements:**
- All required fields must be present
- Omitted optional fields set to default/null
- ID in URL, not in body

#### 5.3.3. Response Specification

**Success Response (200 OK with body):**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Widget Pro Max",
  "description": "Professional widget - updated",
  "price": 129.99,
  "currency": "USD",
  "isActive": true,
  "updatedAt": "2025-01-15T11:00:00Z"
}
```

**Alternative Success Response (204 No Content):**
```http
HTTP/1.1 204 No Content
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

**Note:** Choose one convention (200 or 204) and apply consistently.

### 5.4. PATCH Method

#### 5.4.1. Specification

**Purpose:** Apply partial modification to resource.

**Characteristics:**
- Not safe: Modifies server state
- Idempotent: Depends on implementation
- Partial update: Only specified fields affected

**Usage Pattern:**
```
PATCH /api/v1/products/{id}
```

#### 5.4.2. Implementation Approaches

**Approach 1: Simple Partial DTO (Recommended for simplicity)**

```http
PATCH /api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Content-Type: application/json

{
  "price": 119.99
}
```

Only price updated, other fields unchanged.

**Approach 2: JSON Patch (RFC 6902) (Advanced)**

```http
PATCH /api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Content-Type: application/json-patch+json

[
  { "op": "replace", "path": "/price", "value": 119.99 },
  { "op": "add", "path": "/tags", "value": ["sale"] }
]
```

**Recommendation:** Use simple partial DTO approach unless JSON Patch operations required.

#### 5.4.3. Response Specification

**Success Response (200 OK):**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Widget Pro Max",
  "price": 119.99,
  "currency": "USD",
  "updatedAt": "2025-01-15T11:30:00Z"
}
```

### 5.5. DELETE Method

#### 5.5.1. Specification

**Purpose:** Remove resource from system.

**Characteristics:**
- Not safe: Modifies server state
- Idempotent: Multiple delete requests have same effect
- May implement soft delete or hard delete

**Usage Pattern:**
```
DELETE /api/v1/products/{id}
```

#### 5.5.2. Request Specification

**No Body:**
```http
DELETE /api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Authorization: Bearer {token}
```

#### 5.5.3. Response Specification

**Success Response (204 No Content):**
```http
HTTP/1.1 204 No Content
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

**Idempotency:** Subsequent DELETE of same resource returns 404 or 204 (both acceptable).

### 5.6. HTTP Method Selection Matrix

| Operation | Method | Idempotent | Safe | Cache | Body |
|-----------|--------|-----------|------|-------|------|
| Retrieve collection | GET | Yes | Yes | Yes | No |
| Retrieve resource | GET | Yes | Yes | Yes | No |
| Create resource | POST | No | No | No | Yes |
| Replace resource | PUT | Yes | No | No | Yes |
| Update resource partially | PATCH | Usually | No | No | Yes |
| Delete resource | DELETE | Yes | No | No | No |
| Execute action | POST | No | No | No | Optional |

---

## 6. Request and Response Contract Design

### 6.1. Request Models

#### 6.1.1. Design Principles

**Dedicated DTOs:**
- Create specific DTO for each operation
- Do not expose domain entities directly
- Clear naming convention: {Operation}{Resource}Request

**Examples:**
```csharp
public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    string Currency);

public sealed record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    bool IsActive);

public sealed record PartialUpdateProductRequest
{
    public string? Name { get; init; }
    public decimal? Price { get; init; }
    public string? Currency { get; init; }
}
```

#### 6.1.2. Validation Requirements

**API Layer Validation:**
- Format validation (data types, patterns)
- Required field presence
- Basic constraints (length, range)
- Performed via Data Annotations

**Application Layer Validation:**
- Business rules
- Cross-field validation
- Domain-specific constraints
- Performed via FluentValidation

**Example Data Annotations:**
```csharp
public sealed record CreateProductRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; init; }

    [StringLength(2000)]
    public string? Description { get; init; }

    [Range(0.01, 999999.99)]
    public decimal Price { get; init; }

    [Required]
    [RegularExpression("^[A-Z]{3}$")]
    public string Currency { get; init; }
}
```

### 6.2. Response Models

#### 6.2.1. Design Principles

**Dedicated Response DTOs:**
- Create specific DTO for API responses
- Encapsulate internal structure
- Version-specific representations
- Clear naming: {Resource}Response

**Example:**
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

#### 6.2.2. Field Naming Conventions

**Use camelCase:**
```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "productName": "Widget Pro",
  "unitPrice": 99.99,
  "isActive": true,
  "createdAt": "2025-01-15T10:30:00Z"
}
```

**Configuration:**
```csharp
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = 
            JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = 
            JsonIgnoreCondition.WhenWritingNull;
    });
```

#### 6.2.3. Null Handling

**Strategy:** Omit null fields from response (recommended)

**Configuration:**
```csharp
options.JsonSerializerOptions.DefaultIgnoreCondition = 
    JsonIgnoreCondition.WhenWritingNull;
```

**Example:**
```json
{
  "id": "...",
  "name": "Widget",
  "price": 99.99
  // description omitted because null
}
```

**Rationale:**
- Reduces payload size
- Cleaner JSON
- Industry standard practice

### 6.3. Collection Response Pattern

#### 6.3.1. Standard Envelope

**Structure:**
```json
{
  "items": [...],
  "page": 1,
  "pageSize": 20,
  "totalItems": 150,
  "totalPages": 8
}
```

**Schema:**
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

#### 6.3.2. Empty Collection Handling

**Return 200 OK with empty array:**
```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalItems": 0,
  "totalPages": 0
}
```

**Do Not:** Return 404 for empty collections.

**Rationale:** Empty collection is valid state, distinct from resource not found.

---

## 7. Pagination, Filtering, and Sorting

### 7.1. Pagination Standards

#### 7.1.1. Offset-Based Pagination (Recommended)

**Parameters:**
- `page`: Page number (1-based)
- `pageSize`: Items per page

**Request:**
```
GET /api/v1/products?page=2&pageSize=20
```

**Response:**
```json
{
  "items": [...],
  "page": 2,
  "pageSize": 20,
  "totalItems": 150,
  "totalPages": 8
}
```

**Implementation:**
```csharp
var skip = (page - 1) * pageSize;
var items = await _context.Products
    .Skip(skip)
    .Take(pageSize)
    .ToListAsync();

var totalItems = await _context.Products.CountAsync();
var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
```

#### 7.1.2. Default Values

| Parameter | Default | Minimum | Maximum |
|-----------|---------|---------|---------|
| page | 1 | 1 | No limit |
| pageSize | 20 | 1 | 100 |

**Rationale for Maximum:**
- Prevents excessive load
- Encourages appropriate page sizing
- Configurable per resource if needed

#### 7.1.3. Cursor-Based Pagination (Advanced)

**Use When:**
- Very large datasets
- Real-time data (frequent inserts)
- Better performance required

**Parameters:**
- `cursor`: Opaque string identifying position
- `limit`: Items to return

**Request:**
```
GET /api/v1/products?cursor=eyJpZCI6MTAwfQ&limit=20
```

**Response:**
```json
{
  "items": [...],
  "nextCursor": "eyJpZCI6MTIwfQ",
  "hasMore": true
}
```

### 7.2. Filtering Standards

#### 7.2.1. Field-Based Filtering

**Pattern:** Use field name as query parameter

**Examples:**
```
GET /api/v1/products?category=electronics
GET /api/v1/products?isActive=true
GET /api/v1/products?category=electronics&isActive=true
```

#### 7.2.2. Range Filtering

**Pattern:** Prefix with min/max

**Examples:**
```
GET /api/v1/products?minPrice=10&maxPrice=100
GET /api/v1/orders?createdAfter=2025-01-01&createdBefore=2025-01-31
```

#### 7.2.3. Search Filtering

**Pattern:** Use `search` parameter for free-text

**Example:**
```
GET /api/v1/products?search=wireless+keyboard
```

**Implementation:**
- Search across multiple fields (name, description)
- Case-insensitive
- Support partial matching

### 7.3. Sorting Standards

#### 7.3.1. Sort Parameter Format

**Pattern:** `?sort={field}_{direction}`

**Directions:**
- `asc`: Ascending
- `desc`: Descending

**Examples:**
```
GET /api/v1/products?sort=price_asc
GET /api/v1/products?sort=name_desc
GET /api/v1/products?sort=createdAt_desc
```

#### 7.3.2. Multiple Sort Fields

**Pattern:** Comma-separated

**Example:**
```
GET /api/v1/products?sort=category_asc,price_desc
```

**Interpretation:** Sort by category ascending, then by price descending within each category.

#### 7.3.3. Default Sort Order

**Recommendation:** Define sensible default for each resource

**Examples:**
- Products: createdAt_desc (newest first)
- Orders: orderDate_desc (most recent first)
- Customers: name_asc (alphabetical)

---

## 8. Error Handling Standards

### 8.1. RFC 7807 Problem Details

#### 8.1.1. Standard Format

All errors must use RFC 7807 Problem Details format.

**Schema:**
```json
{
  "type": "string (URI)",
  "title": "string",
  "status": integer,
  "detail": "string",
  "instance": "string (URI)",
  "traceId": "string"
}
```

**Reference:** See API-ERRORHANDLING-001 for complete specification.

### 8.2. HTTP Status Code Usage

#### 8.2.1. Success Codes (2xx)

| Code | Name | Usage |
|------|------|-------|
| 200 | OK | Successful GET, PUT, PATCH with body |
| 201 | Created | Successful POST (resource creation) |
| 204 | No Content | Successful DELETE, PUT/PATCH without body |

#### 8.2.2. Client Error Codes (4xx)

| Code | Name | Usage |
|------|------|-------|
| 400 | Bad Request | Validation errors, malformed request |
| 401 | Unauthorized | Missing or invalid authentication |
| 403 | Forbidden | Valid auth, insufficient permissions |
| 404 | Not Found | Resource does not exist |
| 409 | Conflict | State conflict, duplicate resource |
| 422 | Unprocessable Entity | Business rule violation |

#### 8.2.3. Server Error Codes (5xx)

| Code | Name | Usage |
|------|------|-------|
| 500 | Internal Server Error | Unexpected server error |
| 503 | Service Unavailable | Service temporarily unavailable |
| 504 | Gateway Timeout | Upstream service timeout |

---

## 9. Authentication and Authorization

### 9.1. Authentication Mechanisms

#### 9.1.1. Bearer Token Authentication (Primary)

**Implementation:**
```http
GET /api/v1/products HTTP/1.1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Token Types:**
- JWT (JSON Web Tokens)
- OAuth 2.0 Access Tokens
- OpenID Connect ID Tokens

**Configuration:**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://auth.example.com";
        options.Audience = "api.example.com";
        options.RequireHttpsMetadata = true;
    });
```

#### 9.1.2. API Key Authentication (Alternative)

**Implementation:**
```http
GET /api/v1/products HTTP/1.1
X-API-Key: your-api-key-here
```

**Use Cases:**
- Server-to-server integration
- Simplified authentication for trusted clients
- Legacy system integration

### 9.2. Authorization Patterns

#### 9.2.1. Role-Based Access Control (RBAC)

**Controller Decoration:**
```csharp
[Authorize(Roles = "Admin,Manager")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteProduct(Guid id)
{
    // Only Admin or Manager roles can execute
}
```

#### 9.2.2. Policy-Based Authorization

**Policy Definition:**
```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageProducts", policy =>
        policy.RequireClaim("permission", "products:write"));
    
    options.AddPolicy("CanDeleteProducts", policy =>
        policy.RequireRole("Admin"));
});
```

**Controller Usage:**
```csharp
[Authorize(Policy = "CanManageProducts")]
[HttpPost]
public async Task<IActionResult> CreateProduct(...)
{
    // Only users with products:write permission
}
```

### 9.3. Error Responses

#### 9.3.1. Unauthorized (401)

```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication credentials are required or invalid.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

#### 9.3.2. Forbidden (403)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have permission to perform this action.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

---

## 10. Idempotency and Safety Requirements

### 10.1. Safe Methods

#### 10.1.1. Definition

**Safe Method:** Does not modify resource state on server.

**Safe Methods:** GET, HEAD, OPTIONS

**Requirement:** These methods MUST NOT cause side effects.

**Anti-Pattern:**
```csharp
// WRONG: Side effect in GET
[HttpGet("process")]
public IActionResult ProcessData()
{
    _service.UpdateDatabase(); // Violates safety!
    return Ok();
}
```

### 10.2. Idempotent Methods

#### 10.2.1. Definition

**Idempotent Method:** Multiple identical requests have same effect as single request.

**Idempotent Methods:** GET, PUT, DELETE, HEAD, OPTIONS

#### 10.2.2. PUT Idempotency

**Example:**
```
PUT /api/v1/products/{id}
{ "name": "Widget", "price": 99.99 }
```

Result after 1 request = Result after 10 identical requests

#### 10.2.3. DELETE Idempotency

**First Request:**
```
DELETE /api/v1/products/{id}
→ 204 No Content (resource deleted)
```

**Subsequent Requests:**
```
DELETE /api/v1/products/{id}
→ 404 Not Found (already deleted)
```

Both acceptable responses maintaining idempotency.

### 10.3. Non-Idempotent Methods

#### 10.3.1. POST Non-Idempotency

**Example:**
```
POST /api/v1/products
{ "name": "Widget", "price": 99.99 }
```

Each request creates new resource with different ID.

#### 10.3.2. Idempotency Keys (Advanced)

**For Critical Operations:**

**Request:**
```http
POST /api/v1/payments HTTP/1.1
Idempotency-Key: a1b2c3d4-e5f6-7890-abcd-ef1234567890
Content-Type: application/json

{
  "amount": 100.00,
  "currency": "USD"
}
```

**Implementation:**
- Server stores idempotency key
- Duplicate requests with same key return cached response
- Prevents duplicate payments
- Key expires after 24 hours

---

## 11. Versioning and Breaking Changes

### 11.1. Versioning Strategy

**Complete specification in API-VERSIONING-001**

**Summary:**
- Path-based versioning: /api/v{major}/...
- Major version increments for breaking changes
- Non-breaking changes within version
- Minimum 12-month deprecation period

### 11.2. Breaking Change Classification

#### 11.2.1. Breaking Changes (Require New Version)

- Removing response field
- Renaming field
- Changing field type
- Removing endpoint
- Changing endpoint behavior
- Adding required request field
- Stricter validation

#### 11.2.2. Non-Breaking Changes (Allowed in Version)

- Adding optional response field
- Adding new endpoint
- Adding optional request field
- Adding new enum value
- Expanding validation (less strict)
- Performance improvements

---

## 12. API Documentation Requirements

### 12.1. OpenAPI Specification

#### 12.1.1. Requirements

**Mandatory:**
- All endpoints documented in OpenAPI 3.0 format
- Request and response schemas defined
- HTTP status codes documented
- Error response schemas included
- Authentication requirements specified

**Tool:** Swashbuckle for ASP.NET Core

**Configuration:**
```csharp
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product API",
        Version = "v1",
        Description = "API for product management",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "api-support@example.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Add authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});
```

### 12.2. XML Documentation Comments

#### 12.2.1. Controller Documentation

```csharp
/// <summary>
/// Manages product catalog operations
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of products
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page (max 100)</param>
    /// <param name="search">Optional search term</param>
    /// <returns>Paginated list of products</returns>
    /// <response code="200">Products retrieved successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<ProductResponse>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        // Implementation
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="request">Product creation details</param>
    /// <returns>Created product</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid product data</response>
    /// <response code="409">Product with same name already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        [FromBody] CreateProductRequest request)
    {
        // Implementation
    }
}
```

### 12.3. Swagger UI Configuration

#### 12.3.1. Endpoint Configuration

```csharp
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Product API Documentation";
    options.DisplayRequestDuration();
});
```

**Access:** https://api.example.com/swagger

---

## 13. Testing Standards

### 13.1. Controller Unit Tests

#### 13.1.1. Test Scope

**Test Coverage:**
- Request mapping to commands/queries
- Response mapping from results
- Status code selection
- Error handling

**Example:**
```csharp
public class ProductsControllerTests
{
    [Fact]
    public async Task GetProducts_ValidParameters_Returns200WithProducts()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var expectedResult = new PagedResponse<ProductDto>
        {
            Items = new[] { new ProductDto { Id = Guid.NewGuid() } },
            Page = 1,
            PageSize = 20,
            TotalItems = 1,
            TotalPages = 1
        };

        mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetProductsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var controller = new ProductsController(mediatorMock.Object);

        // Act
        var result = await controller.GetProducts(1, 20);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        
        var response = Assert.IsType<PagedResponse<ProductResponse>>(okResult.Value);
        Assert.Single(response.Items);
    }
}
```

### 13.2. Integration Tests

#### 13.2.1. Test Infrastructure

```csharp
public class ProductsIntegrationTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ValidRequest_Returns201Created()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            Price = 99.99m,
            Currency = "USD"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/products",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        
        var product = await response.Content
            .ReadFromJsonAsync<ProductResponse>();
        
        Assert.NotNull(product);
        Assert.Equal(request.Name, product.Name);
    }
}
```

### 13.3. Contract Tests

#### 13.3.1. OpenAPI Schema Validation

**Purpose:** Ensure responses match documented schema

**Tool:** OpenAPI validation libraries

**Example:**
```csharp
[Fact]
public async Task GetProduct_ResponseMatchesSchema()
{
    var response = await _client.GetAsync("/api/v1/products/test-id");
    var json = await response.Content.ReadAsStringAsync();
    
    var isValid = _schemaValidator.Validate(json, "ProductResponse");
    Assert.True(isValid, "Response does not match OpenAPI schema");
}
```

---

## 14. Performance and Scalability Guidelines

### 14.1. Response Time Targets

| Operation Type | Target p50 | Target p95 | Target p99 |
|----------------|-----------|-----------|-----------|
| Simple GET | < 20ms | < 50ms | < 100ms |
| Collection GET | < 50ms | < 100ms | < 200ms |
| POST/PUT/PATCH | < 100ms | < 200ms | < 500ms |
| DELETE | < 30ms | < 100ms | < 200ms |

### 14.2. Optimization Strategies

#### 14.2.1. Query Optimization

- Use Dapper for read-heavy operations
- Implement proper database indexing
- Apply pagination to all collections
- Use query result caching where appropriate

#### 14.2.2. Payload Optimization

- Omit null fields in responses
- Use compression (gzip)
- Implement field selection (sparse fieldsets) for large resources
- Consider GraphQL for complex scenarios

#### 14.2.3. Caching Strategy

**Cache-Control Headers:**
```http
Cache-Control: public, max-age=300
ETag: "686897696a7c876b7e"
```

**Cacheable Responses:**
- GET requests only
- Public data
- Stable resources

---

## 15. Glossary

**API (Application Programming Interface):** A set of protocols and tools for building and integrating software applications.

**DTO (Data Transfer Object):** An object carrying data between API and application layers without business logic.

**Idempotent:** Property where multiple identical requests have same effect as single request.

**OpenAPI:** Specification for describing RESTful APIs in machine-readable format.

**Problem Details:** RFC 7807 standardized format for HTTP API error responses.

**Resource:** An object or representation of a domain concept accessible via URL.

**REST (Representational State Transfer):** Architectural style for distributed hypermedia systems.

**Safe Method:** HTTP method that does not modify resource state.

---

## 16. Recommendations and Next Steps

### 16.1. For Development Teams

#### 16.1.1. Implementation Checklist

**When Implementing New Endpoints:**
- [ ] Follow RESTful resource naming
- [ ] Use appropriate HTTP methods
- [ ] Return correct status codes
- [ ] Implement RFC 7807 error responses
- [ ] Add XML documentation comments
- [ ] Include ProducesResponseType attributes
- [ ] Write unit tests for controllers
- [ ] Write integration tests
- [ ] Document in OpenAPI/Swagger
- [ ] Follow authentication/authorization patterns

**Code Quality Standards:**
- Thin controllers (no business logic)
- Proper separation of concerns
- Consistent naming conventions
- Comprehensive error handling
- Appropriate logging

### 16.2. For Architects

#### 16.2.1. Governance Activities

**Periodic Reviews:**
- API design consistency
- Standards compliance
- Performance metrics
- Security posture
- Documentation quality

**Architecture Tests:**
```csharp
[Fact]
public void Controllers_ShouldHaveApiControllerAttribute()
{
    var controllers = typeof(ProductsController).Assembly
        .GetTypes()
        .Where(t => t.Name.EndsWith("Controller"));
    
    foreach (var controller in controllers)
    {
        Assert.True(
            controller.GetCustomAttributes(typeof(ApiControllerAttribute)).Any(),
            $"{controller.Name} missing [ApiController] attribute");
    }
}
```

### 16.3. Related Documentation

**Must Read:**
- High-Level Architecture (ARCH-HL-001)
- API Flow Specification (ARCH-API-003)
- Error Handling Specification (API-ERRORHANDLING-001)
- Versioning Strategy (API-VERSIONING-001)

**Recommended Reading:**
- OpenAPI Specification Standards
- HTTP/1.1 Specification (RFC 7230-7235)
- REST API Design Best Practices
- Security Implementation Guide

---

## 17. References

### 17.1. Standards and Specifications

**REST Architectural Style**
- Roy Fielding's Dissertation: https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm

**HTTP/1.1 Specification**
- RFC 7230-7235: https://tools.ietf.org/html/rfc7230

**RFC 7807 - Problem Details for HTTP APIs**
- https://tools.ietf.org/html/rfc7807

**OpenAPI Specification 3.0**
- https://swagger.io/specification/

### 17.2. Industry Guidelines

**Microsoft REST API Guidelines**
- https://github.com/microsoft/api-guidelines

**Google API Design Guide**
- https://cloud.google.com/apis/design

**Zalando RESTful API Guidelines**
- https://opensource.zalando.com/restful-api-guidelines/

### 17.3. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- ARCH-HL-001: High-Level Architecture
- ARCH-API-003: API Flow Specification
- API-ERRORHANDLING-001: Error Handling and Problem Details
- API-VERSIONING-001: API Versioning Strategy
- API-CATALOG-001: Catalog API Endpoints

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial API design guidelines and standards specification |

---

**END OF DOCUMENT**