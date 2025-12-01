# Pagination, Filtering, and Sorting Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | API-PAGINATION-001 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Technical Specification |
| Target Audience | API Designers, Development Teams, Integration Engineers, QA Engineers, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Annual |
| Related Documents | API Guidelines (API-GUIDELINES-001), CQRS Specification (ARCH-CQRS-003), API Examples (API-EXAMPLES-001) |
| Prerequisites | Understanding of REST principles, HTTP query parameters, CQRS pattern |

---

## Executive Summary

This document establishes comprehensive specifications for pagination, filtering, and sorting mechanisms across all collection endpoints within the .NET Enterprise Architecture Template. These standardized patterns ensure predictable API behavior, optimal performance, and consistent client integration experiences while supporting scalable data retrieval for enterprise applications.

**Strategic Business Value:**
- Reduced server load through controlled data retrieval and transfer
- Improved user experience via responsive data access patterns
- Decreased bandwidth costs through efficient data pagination
- Enhanced system scalability supporting large dataset management
- Standardized client integration reducing development time and errors
- Better resource utilization through optimized query execution

**Key Technical Capabilities:**
- Offset-based pagination with page and pageSize parameters
- Flexible field-based filtering with multiple criteria support
- Consistent sorting syntax with field and direction specification
- Standardized response envelope with pagination metadata
- Validation framework for query parameters
- CQRS integration with Query objects and Handlers
- Performance optimization through indexed filtering and sorting

**Compliance and Standards:**
- REST architectural constraints for resource collections
- HTTP/1.1 query parameter conventions (RFC 3986)
- ISO 8601 datetime formatting for temporal filters
- JSON:API pagination recommendations
- GraphQL pagination pattern influences

---

## Table of Contents

1. Introduction and Scope
2. Pagination Specification
3. Filtering Specification
4. Sorting Specification
5. Combined Query Operations
6. Response Format Standards
7. Validation Requirements
8. Error Handling
9. Implementation Patterns
10. Performance Optimization
11. Testing Requirements
12. Advanced Patterns
13. Glossary
14. Recommendations and Next Steps
15. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides authoritative specifications for pagination, filtering, and sorting operations across all collection endpoints in the .NET Enterprise Architecture Template. It defines query parameter conventions, response formats, validation rules, and implementation patterns ensuring consistent, performant, and scalable data retrieval mechanisms.

### 1.2. Scope

**In Scope:**
- Pagination parameter specifications (page, pageSize)
- Filtering parameter conventions and patterns
- Sorting parameter syntax and behavior
- Response envelope structure with pagination metadata
- Query parameter validation rules
- Error handling for invalid parameters
- CQRS Query object design for list operations
- Performance optimization strategies
- Testing approaches for paginated endpoints

**Out of Scope:**
- Cursor-based pagination (advanced pattern, separate specification if needed)
- GraphQL-style field selection
- Real-time data streaming
- WebSocket-based data push
- Client-side pagination implementations
- Database-specific optimization techniques
- Caching strategies (covered in separate specification)

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**API Designers:** Query parameter design and endpoint specifications

**Development Teams:** Implementation guidelines and code examples

**Integration Engineers:** Client-side query construction patterns

**QA Engineers:** Test case development and validation scenarios

**Performance Engineers:** Optimization strategies and indexing recommendations

**C-Level Executives:** Business value and scalability benefits

### 1.4. Applicability

These specifications apply to:

**All Collection Endpoints:**
- GET /api/v1/products
- GET /api/v1/orders
- GET /api/v1/customers
- GET /api/v1/logs
- Any endpoint returning multiple resources

**Not Applicable To:**
- Single resource retrieval (GET /api/v1/products/{id})
- Resource creation (POST)
- Resource update (PUT, PATCH)
- Resource deletion (DELETE)

---

## 2. Pagination Specification

### 2.1. Pagination Rationale

#### 2.1.1. Purpose

**Objective:** Control data transfer volume, maintain API responsiveness, and prevent resource exhaustion.

**Problems Addressed:**
- Excessive memory consumption from large result sets
- Network timeout risks with large responses
- Poor user experience from slow page loads
- Database query performance degradation
- Bandwidth waste transferring unused data

**Benefits:**
- Predictable response times
- Controlled server resource utilization
- Improved client responsiveness
- Better network efficiency
- Scalable to large datasets

### 2.2. Pagination Method: Offset-Based

#### 2.2.1. Method Selection

**Selected Method:** Offset-based pagination using page number and page size

**Rationale:**
- Simple mental model for users and developers
- Easy implementation with SQL OFFSET/LIMIT
- Supports random page access (jump to page N)
- Compatible with total count calculation
- Industry standard for most APIs

**Alternative Considered:**
- Cursor-based pagination (better for very large datasets, real-time data)
- Not selected as primary due to complexity and limited use cases

### 2.3. Query Parameters

#### 2.3.1. Page Parameter

**Parameter Name:** `page`

**Data Type:** Integer

**Required:** No

**Default Value:** 1

**Valid Range:** 1 to maximum integer

**Description:** One-based page index specifying which page of results to retrieve.

**Examples:**
```http
GET /api/v1/products?page=1
GET /api/v1/products?page=5
```

#### 2.3.2. PageSize Parameter

**Parameter Name:** `pageSize`

**Data Type:** Integer

**Required:** No

**Default Value:** 20

**Valid Range:** 1 to 100 (configurable maximum)

**Description:** Number of items to return per page.

**Examples:**
```http
GET /api/v1/products?pageSize=10
GET /api/v1/products?pageSize=50
```

#### 2.3.3. Parameter Specifications Table

| Parameter | Type | Required | Default | Minimum | Maximum | Description |
|-----------|------|----------|---------|---------|---------|-------------|
| page | integer | No | 1 | 1 | 2147483647 | Page number (1-based) |
| pageSize | integer | No | 20 | 1 | 100 | Items per page |

### 2.4. Response Structure

#### 2.4.1. Pagination Envelope

**Standard Response Format:**

```json
{
  "items": [
    {
      "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
      "name": "Product 1"
    },
    {
      "id": "c7b0f5d4-8be3-5ea3-0c10-2e5f7g07a1g2",
      "name": "Product 2"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalItems": 150,
  "totalPages": 8
}
```

#### 2.4.2. Response Field Specifications

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| items | array | Yes | Collection of resource items for current page |
| page | integer | Yes | Current page number (matches request) |
| pageSize | integer | Yes | Items per page (matches request) |
| totalItems | integer | Yes | Total number of items across all pages |
| totalPages | integer | Yes | Total number of pages (calculated: ceiling(totalItems / pageSize)) |

#### 2.4.3. Response DTO Definition

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

### 2.5. Pagination Behavior

#### 2.5.1. First Page Request

**Request:**
```http
GET /api/v1/products?page=1&pageSize=20
```

**Behavior:**
- Return first 20 items
- Calculate total count
- Calculate total pages
- Return pagination metadata

#### 2.5.2. Middle Page Request

**Request:**
```http
GET /api/v1/products?page=3&pageSize=20
```

**Behavior:**
- Skip first 40 items (offset = (page - 1) * pageSize)
- Return next 20 items
- Include same total count and total pages

#### 2.5.3. Last Page Request

**Request:**
```http
GET /api/v1/products?page=8&pageSize=20
```

**Behavior:**
- Return remaining items (may be less than pageSize)
- Indicate this is the last page via totalPages

#### 2.5.4. Beyond Last Page Request

**Request:**
```http
GET /api/v1/products?page=999&pageSize=20
```

**Behavior:**
- Return empty items array
- Return accurate pagination metadata showing page beyond totalPages
- Still 200 OK status (empty result is valid)

**Response:**
```json
{
  "items": [],
  "page": 999,
  "pageSize": 20,
  "totalItems": 150,
  "totalPages": 8
}
```

### 2.6. Implementation Example

#### 2.6.1. Query Object

```csharp
public sealed record GetProductsQuery : IRequest<PagedResponse<ProductResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
```

#### 2.6.2. Handler Implementation

```csharp
public sealed class GetProductsQueryHandler 
    : IRequestHandler<GetProductsQuery, PagedResponse<ProductResponse>>
{
    private readonly ApplicationDbContext _context;

    public async Task<PagedResponse<ProductResponse>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        // Calculate offset
        var skip = (query.Page - 1) * query.PageSize;

        // Query with pagination
        var items = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.CreatedAt)
            .Skip(skip)
            .Take(query.PageSize)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .ToListAsync(cancellationToken);

        // Get total count
        var totalItems = await _context.Products
            .CountAsync(cancellationToken);

        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

        return new PagedResponse<ProductResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }
}
```

---

## 3. Filtering Specification

### 3.1. Filtering Approach

#### 3.1.1. Query Parameter Filtering

**Method:** Named query parameters representing filterable fields

**Advantages:**
- Simple and intuitive
- URL-based (cacheable, shareable)
- Standard HTTP practice
- Easy to implement and understand

**Example:**
```http
GET /api/v1/products?category=electronics&minPrice=100&maxPrice=500
```

### 3.2. Filter Parameter Conventions

#### 3.2.1. Simple Equality Filters

**Convention:** Use field name as parameter name

**Examples:**

| Filter | Query Parameter | Example |
|--------|----------------|---------|
| Category | category | ?category=electronics |
| Status | status | ?status=active |
| Color | color | ?color=blue |
| Size | size | ?size=large |

**Request Example:**
```http
GET /api/v1/products?category=electronics&status=active
```

#### 3.2.2. Range Filters

**Convention:** Prefix field name with min/max

**Numeric Ranges:**

| Filter | Query Parameters | Example |
|--------|-----------------|---------|
| Price range | minPrice, maxPrice | ?minPrice=100&maxPrice=500 |
| Quantity range | minQuantity, maxQuantity | ?minQuantity=10&maxQuantity=100 |
| Weight range | minWeight, maxWeight | ?minWeight=1.5&maxWeight=5.0 |

**Date Ranges:**

| Filter | Query Parameters | Example |
|--------|-----------------|---------|
| Created date | createdAfter, createdBefore | ?createdAfter=2025-01-01&createdBefore=2025-12-31 |
| Updated date | updatedAfter, updatedBefore | ?updatedAfter=2025-01-01 |

**Request Example:**
```http
GET /api/v1/orders?minAmount=100&maxAmount=1000&createdAfter=2025-01-01
```

#### 3.2.3. Text Search Filter

**Convention:** Use `search` parameter for free-text search

**Behavior:**
- Searches across multiple text fields (name, description, tags)
- Case-insensitive matching
- Partial match (contains) behavior
- May use database full-text search capabilities

**Example:**
```http
GET /api/v1/products?search=wireless+keyboard
```

**Implementation:**
```csharp
var query = _context.Products.AsQueryable();

if (!string.IsNullOrEmpty(request.Search))
{
    query = query.Where(p => 
        p.Name.Contains(request.Search) ||
        p.Description.Contains(request.Search));
}
```

#### 3.2.4. Boolean Filters

**Convention:** Use field name with boolean value

**Examples:**
```http
GET /api/v1/products?isActive=true
GET /api/v1/products?isDiscounted=false
GET /api/v1/products?inStock=true
```

**Value Formats:**
- true, false (lowercase, recommended)
- True, False (capitalized, acceptable)
- 1, 0 (numeric, acceptable)

### 3.3. Common Filter Examples by Resource

#### 3.3.1. Product Filters

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| search | string | Free-text search | ?search=phone |
| name | string | Exact or partial name | ?name=iPhone |
| category | string | Product category | ?category=smartphones |
| minPrice | decimal | Minimum price | ?minPrice=200 |
| maxPrice | decimal | Maximum price | ?maxPrice=1000 |
| isActive | boolean | Active status | ?isActive=true |
| manufacturer | string | Manufacturer name | ?manufacturer=Apple |

#### 3.3.2. Order Filters

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| customerId | GUID | Customer identifier | ?customerId={guid} |
| status | string | Order status | ?status=pending |
| minAmount | decimal | Minimum order amount | ?minAmount=50 |
| maxAmount | decimal | Maximum order amount | ?maxAmount=500 |
| createdAfter | date | Created after date | ?createdAfter=2025-01-01 |
| createdBefore | date | Created before date | ?createdBefore=2025-12-31 |

### 3.4. Filter Combination

#### 3.4.1. Multiple Filters (AND Logic)

**Behavior:** Multiple filters combined with AND logic

**Example:**
```http
GET /api/v1/products?category=electronics&minPrice=100&maxPrice=500&isActive=true
```

**Interpretation:** 
- Category equals "electronics" AND
- Price between 100 and 500 AND
- IsActive equals true

#### 3.4.2. Implementation

```csharp
public async Task<PagedResponse<ProductResponse>> Handle(
    GetProductsQuery query,
    CancellationToken cancellationToken)
{
    var queryable = _context.Products.AsQueryable();

    // Apply filters
    if (!string.IsNullOrEmpty(query.Category))
    {
        queryable = queryable.Where(p => p.Category == query.Category);
    }

    if (query.MinPrice.HasValue)
    {
        queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);
    }

    if (query.MaxPrice.HasValue)
    {
        queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);
    }

    if (query.IsActive.HasValue)
    {
        queryable = queryable.Where(p => p.IsActive == query.IsActive.Value);
    }

    // Apply pagination and execute
    var skip = (query.Page - 1) * query.PageSize;
    var items = await queryable
        .OrderBy(p => p.CreatedAt)
        .Skip(skip)
        .Take(query.PageSize)
        .ToListAsync(cancellationToken);

    // ... rest of implementation
}
```

---

## 4. Sorting Specification

### 4.1. Sort Parameter Convention

#### 4.1.1. Parameter Format

**Parameter Name:** `sort`

**Format:** `{field}_{direction}`

**Components:**
- field: Name of the sortable field
- direction: `asc` (ascending) or `desc` (descending)

**Examples:**
```http
GET /api/v1/products?sort=price_asc
GET /api/v1/products?sort=name_desc
GET /api/v1/products?sort=createdAt_desc
```

### 4.2. Sort Direction Specifications

#### 4.2.1. Direction Values

| Direction | Description | Use Case |
|-----------|-------------|----------|
| asc | Ascending order | A to Z, 0 to 9, oldest to newest |
| desc | Descending order | Z to A, 9 to 0, newest to oldest |

### 4.3. Common Sort Fields

#### 4.3.1. Product Sorting

| Sort Value | Description | Example |
|------------|-------------|---------|
| name_asc | Name A to Z | ?sort=name_asc |
| name_desc | Name Z to A | ?sort=name_desc |
| price_asc | Price low to high | ?sort=price_asc |
| price_desc | Price high to low | ?sort=price_desc |
| createdAt_asc | Oldest first | ?sort=createdAt_asc |
| createdAt_desc | Newest first | ?sort=createdAt_desc |

#### 4.3.2. Default Sort Order

**Recommendation:** Define sensible default for each resource

**Common Defaults:**
- Products: createdAt_desc (newest first)
- Orders: orderDate_desc (most recent first)
- Customers: name_asc (alphabetical)
- Logs: timestamp_desc (newest first)

### 4.4. Sort Implementation

#### 4.4.1. Query Object

```csharp
public sealed record GetProductsQuery : IRequest<PagedResponse<ProductResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Sort { get; init; }
}
```

#### 4.4.2. Dynamic Sorting Implementation

```csharp
public async Task<PagedResponse<ProductResponse>> Handle(
    GetProductsQuery query,
    CancellationToken cancellationToken)
{
    var queryable = _context.Products.AsQueryable();

    // Apply sorting
    queryable = ApplySorting(queryable, query.Sort);

    // ... rest of implementation
}

private IQueryable<Product> ApplySorting(
    IQueryable<Product> queryable,
    string? sort)
{
    return sort switch
    {
        "name_asc" => queryable.OrderBy(p => p.Name),
        "name_desc" => queryable.OrderByDescending(p => p.Name),
        "price_asc" => queryable.OrderBy(p => p.Price),
        "price_desc" => queryable.OrderByDescending(p => p.Price),
        "createdAt_asc" => queryable.OrderBy(p => p.CreatedAt),
        "createdAt_desc" => queryable.OrderByDescending(p => p.CreatedAt),
        _ => queryable.OrderByDescending(p => p.CreatedAt) // Default
    };
}
```

### 4.5. Multi-Field Sorting (Optional)

#### 4.5.1. Convention

**Format:** Comma-separated sort specifications

**Example:**
```http
GET /api/v1/products?sort=category_asc,price_desc
```

**Interpretation:** Sort by category ascending, then by price descending within each category

**Implementation:**
```csharp
private IQueryable<Product> ApplyMultiFieldSorting(
    IQueryable<Product> queryable,
    string? sort)
{
    if (string.IsNullOrEmpty(sort))
        return queryable.OrderByDescending(p => p.CreatedAt);

    var sortFields = sort.Split(',');
    IOrderedQueryable<Product>? orderedQuery = null;

    foreach (var field in sortFields)
    {
        var parts = field.Split('_');
        if (parts.Length != 2) continue;

        var fieldName = parts[0];
        var direction = parts[1];

        orderedQuery = (fieldName, direction) switch
        {
            ("category", "asc") => orderedQuery?.ThenBy(p => p.Category) 
                ?? queryable.OrderBy(p => p.Category),
            ("category", "desc") => orderedQuery?.ThenByDescending(p => p.Category) 
                ?? queryable.OrderByDescending(p => p.Category),
            ("price", "asc") => orderedQuery?.ThenBy(p => p.Price) 
                ?? queryable.OrderBy(p => p.Price),
            ("price", "desc") => orderedQuery?.ThenByDescending(p => p.Price) 
                ?? queryable.OrderByDescending(p => p.Price),
            _ => orderedQuery
        };
    }

    return orderedQuery ?? queryable.OrderByDescending(p => p.CreatedAt);
}
```

---

## 5. Combined Query Operations

### 5.1. Complete Query Example

#### 5.1.1. Full Query String

**Request:**
```http
GET /api/v1/products?search=phone&category=electronics&minPrice=200&maxPrice=1200&isActive=true&sort=price_desc&page=2&pageSize=20
```

**Query Breakdown:**

| Parameter | Value | Purpose |
|-----------|-------|---------|
| search | phone | Free-text search |
| category | electronics | Category filter |
| minPrice | 200 | Minimum price filter |
| maxPrice | 1200 | Maximum price filter |
| isActive | true | Active status filter |
| sort | price_desc | Sort by price descending |
| page | 2 | Second page |
| pageSize | 20 | 20 items per page |

### 5.2. Operation Execution Order

#### 5.2.1. Standard Processing Sequence

**Mandatory Order:**
1. Filtering (WHERE clauses)
2. Sorting (ORDER BY)
3. Pagination (OFFSET/LIMIT)
4. Total Count (separate query or optimization)

**Rationale:**
- Filtering reduces dataset size first (most efficient)
- Sorting applied to filtered results
- Pagination applied to filtered and sorted results
- Total count reflects filtered results

#### 5.2.2. SQL Translation Example

```sql
-- Step 1: Filter (WHERE)
SELECT * FROM Products
WHERE 
    (Name LIKE '%phone%' OR Description LIKE '%phone%')
    AND Category = 'electronics'
    AND Price >= 200
    AND Price <= 1200
    AND IsActive = 1

-- Step 2: Sort (ORDER BY)
ORDER BY Price DESC

-- Step 3: Paginate (OFFSET/FETCH)
OFFSET 20 ROWS    -- (page - 1) * pageSize
FETCH NEXT 20 ROWS ONLY;

-- Step 4: Total Count (separate query)
SELECT COUNT(*) FROM Products
WHERE 
    (Name LIKE '%phone%' OR Description LIKE '%phone%')
    AND Category = 'electronics'
    AND Price >= 200
    AND Price <= 1200
    AND IsActive = 1;
```

### 5.3. Complete Implementation

```csharp
public sealed record GetProductsQuery : IRequest<PagedResponse<ProductResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public string? Category { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool? IsActive { get; init; }
    public string? Sort { get; init; }
}

public sealed class GetProductsQueryHandler 
    : IRequestHandler<GetProductsQuery, PagedResponse<ProductResponse>>
{
    public async Task<PagedResponse<ProductResponse>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var queryable = _context.Products.AsNoTracking();

        // Step 1: Apply filters
        queryable = ApplyFilters(queryable, query);

        // Get total count (before pagination)
        var totalItems = await queryable.CountAsync(cancellationToken);

        // Step 2: Apply sorting
        queryable = ApplySorting(queryable, query.Sort);

        // Step 3: Apply pagination
        var skip = (query.Page - 1) * query.PageSize;
        var items = await queryable
            .Skip(skip)
            .Take(query.PageSize)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Currency = p.Currency,
                IsActive = p.IsActive
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

        return new PagedResponse<ProductResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    private IQueryable<Product> ApplyFilters(
        IQueryable<Product> queryable,
        GetProductsQuery query)
    {
        if (!string.IsNullOrEmpty(query.Search))
        {
            queryable = queryable.Where(p => 
                p.Name.Contains(query.Search) ||
                p.Description.Contains(query.Search));
        }

        if (!string.IsNullOrEmpty(query.Category))
        {
            queryable = queryable.Where(p => p.Category == query.Category);
        }

        if (query.MinPrice.HasValue)
        {
            queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);
        }

        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(p => p.IsActive == query.IsActive.Value);
        }

        return queryable;
    }

    private IQueryable<Product> ApplySorting(
        IQueryable<Product> queryable,
        string? sort)
    {
        return sort switch
        {
            "name_asc" => queryable.OrderBy(p => p.Name),
            "name_desc" => queryable.OrderByDescending(p => p.Name),
            "price_asc" => queryable.OrderBy(p => p.Price),
            "price_desc" => queryable.OrderByDescending(p => p.Price),
            "createdAt_asc" => queryable.OrderBy(p => p.CreatedAt),
            "createdAt_desc" => queryable.OrderByDescending(p => p.CreatedAt),
            _ => queryable.OrderByDescending(p => p.CreatedAt)
        };
    }
}
```

---

## 6. Response Format Standards

### 6.1. Successful Response

**HTTP Status:** 200 OK

**Example:**
```json
{
  "items": [
    {
      "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
      "name": "Phone X Pro",
      "price": 899.00,
      "currency": "EUR",
      "isActive": true
    }
  ],
  "page": 2,
  "pageSize": 20,
  "totalItems": 150,
  "totalPages": 8
}
```

### 6.2. Empty Result Response

**HTTP Status:** 200 OK

**Example:**
```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalItems": 0,
  "totalPages": 0
}
```

**Note:** Empty result is valid state, not an error condition.

---

## 7. Validation Requirements

### 7.1. Parameter Validation Rules

#### 7.1.1. Page Validation

| Rule | Constraint | Error Response |
|------|-----------|----------------|
| Minimum value | page >= 1 | 400 Bad Request |
| Data type | Integer | 400 Bad Request |
| Format | Numeric string | 400 Bad Request |

#### 7.1.2. PageSize Validation

| Rule | Constraint | Error Response |
|------|-----------|----------------|
| Minimum value | pageSize >= 1 | 400 Bad Request |
| Maximum value | pageSize <= 100 | 400 Bad Request |
| Data type | Integer | 400 Bad Request |

### 7.2. Validation Implementation

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

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum price must be greater than or equal to 0");

        RuleFor(x => x.MaxPrice)
            .GreaterThan(x => x.MinPrice)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithMessage("Maximum price must be greater than minimum price");
    }
}
```

---

## 8. Error Handling

### 8.1. Validation Errors (400 Bad Request)

**Scenario:** Invalid query parameters

**Example Request:**
```http
GET /api/v1/products?page=0&pageSize=150
```

**Response:**
```http
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Invalid pagination parameters provided.",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "errors": {
    "page": [
      "Page must be greater than or equal to 1"
    ],
    "pageSize": [
      "Page size must be between 1 and 100"
    ]
  }
}
```

### 8.2. Invalid Sort Field

**Example Request:**
```http
GET /api/v1/products?sort=invalid_field_asc
```

**Response:**
```http
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Invalid sort parameter",
  "status": 400,
  "detail": "Sort field 'invalid_field' is not supported. Supported fields: name, price, createdAt",
  "traceId": "00-1a93f4d5c8ab648794d4ef69334b1d51-01"
}
```

---

## 9. Implementation Patterns

### 9.1. Controller Implementation

```csharp
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? sort = null)
    {
        var query = new GetProductsQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            IsActive = isActive,
            Sort = sort
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
```

---

## 10. Performance Optimization

### 10.1. Database Indexing

#### 10.1.1. Recommended Indexes

**Frequently Filtered Fields:**
```sql
CREATE INDEX IX_Products_Category ON Products(Category);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);
CREATE INDEX IX_Products_Price ON Products(Price);
CREATE INDEX IX_Products_CreatedAt ON Products(CreatedAt);
```

**Composite Indexes for Common Queries:**
```sql
CREATE INDEX IX_Products_Category_Price 
ON Products(Category, Price) 
WHERE IsActive = 1;
```

### 10.2. Query Optimization

#### 10.2.1. Count Optimization

**Problem:** Separate COUNT query can be expensive

**Solution 1: Count without pagination**
```csharp
// Execute count before pagination
var totalItems = await queryable.CountAsync();
```

**Solution 2: Approximate count for very large datasets**
```sql
-- Use approximate count for very large tables
SELECT APPROX_COUNT_DISTINCT(*) FROM Products;
```

### 10.3. Caching Strategies

**Cache Stable Filtered Subsets:**
- Category-based product lists
- Static reference data
- Infrequently changing aggregations

**Cache Key Example:**
```csharp
var cacheKey = $"products:page={page}:size={pageSize}:cat={category}:sort={sort}";
```

---

## 11. Testing Requirements

### 11.1. Pagination Tests

```csharp
[Fact]
public async Task GetProducts_FirstPage_ReturnsCorrectPage()
{
    // Arrange
    var query = new GetProductsQuery { Page = 1, PageSize = 20 };

    // Act
    var result = await _handler.Handle(query, CancellationToken.None);

    // Assert
    Assert.Equal(1, result.Page);
    Assert.Equal(20, result.PageSize);
    Assert.True(result.Items.Count() <= 20);
}

[Fact]
public async Task GetProducts_InvalidPage_ReturnsValidationError()
{
    // Arrange
    var query = new GetProductsQuery { Page = 0, PageSize = 20 };

    // Act & Assert
    await Assert.ThrowsAsync<ValidationException>(
        () => _handler.Handle(query, CancellationToken.None));
}
```

### 11.2. Filtering Tests

```csharp
[Fact]
public async Task GetProducts_WithCategoryFilter_ReturnsFilteredResults()
{
    // Arrange
    var query = new GetProductsQuery 
    { 
        Page = 1, 
        PageSize = 20,
        Category = "electronics"
    };

    // Act
    var result = await _handler.Handle(query, CancellationToken.None);

    // Assert
    Assert.All(result.Items, item => 
        Assert.Equal("electronics", item.Category));
}
```

### 11.3. Sorting Tests

```csharp
[Fact]
public async Task GetProducts_SortByPriceAsc_ReturnsSortedResults()
{
    // Arrange
    var query = new GetProductsQuery 
    { 
        Page = 1, 
        PageSize = 20,
        Sort = "price_asc"
    };

    // Act
    var result = await _handler.Handle(query, CancellationToken.None);

    // Assert
    var prices = result.Items.Select(i => i.Price).ToList();
    Assert.Equal(prices.OrderBy(p => p), prices);
}
```

---

## 12. Advanced Patterns

### 12.1. Cursor-Based Pagination

**Use Cases:**
- Very large datasets (millions of records)
- Real-time data with frequent inserts
- Infinite scroll UX patterns

**Not Covered:** See separate specification if needed

### 12.2. Field Selection (Sparse Fieldsets)

**Pattern:**
```http
GET /api/v1/products?fields=id,name,price
```

**Not Covered:** See API Guidelines for field selection specification

---

## 13. Glossary

**Cursor-Based Pagination:** Pagination method using opaque cursor tokens instead of page numbers.

**Offset-Based Pagination:** Pagination method using page number and page size to calculate offset.

**Page:** One-based index representing which subset of results to retrieve.

**PageSize:** Number of items included in a single page response.

**Pagination Envelope:** Standard response wrapper containing items and pagination metadata.

**Query Parameter:** URL parameter specifying filter, sort, or pagination criteria.

**Sparse Fieldsets:** Pattern allowing clients to request specific fields only.

**Total Count:** Total number of items across all pages matching filter criteria.

**Total Pages:** Calculated number of pages (ceiling of totalItems / pageSize).

---

## 14. Recommendations and Next Steps

### 14.1. For Development Teams

**Implementation Checklist:**
- [ ] Implement PagedResponse generic type
- [ ] Add pagination parameters to all collection endpoints
- [ ] Implement filter parameters for common search criteria
- [ ] Add sort parameter with multiple sort options
- [ ] Validate all query parameters
- [ ] Return RFC 7807 errors for invalid parameters
- [ ] Index frequently filtered fields
- [ ] Write comprehensive tests for pagination, filtering, sorting
- [ ] Document all supported filters and sort fields

### 14.2. For API Designers

**Design Considerations:**
- Define sensible default page size
- Determine maximum page size
- Identify frequently filtered fields
- Define default sort order
- Document all filter parameters
- Consider multi-field sorting needs

### 14.3. For Performance Engineers

**Optimization Tasks:**
- Create indexes on filtered fields
- Optimize count queries
- Consider caching strategies
- Monitor slow query performance
- Implement approximate counts for very large datasets

### 14.4. Related Documentation

**Must Read:**
- API Guidelines (API-GUIDELINES-001)
- CQRS Specification (ARCH-CQRS-003)
- Error Handling Specification (API-ERRORHANDLING-001)

---

## 15. References

### 15.1. Standards and Specifications

**REST Architectural Style**
- Roy Fielding's Dissertation

**HTTP URI Specification**
- RFC 3986: https://tools.ietf.org/html/rfc3986

**JSON:API Pagination**
- https://jsonapi.org/format/#fetching-pagination

### 15.2. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- API-GUIDELINES-001: API Design Guidelines
- ARCH-CQRS-003: Command Query Responsibility Segregation
- API-ERRORHANDLING-001: Error Handling and Problem Details
- API-EXAMPLES-001: API Integration Examples

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial pagination, filtering, and sorting specification with comprehensive implementation guidelines |

---

**END OF DOCUMENT**