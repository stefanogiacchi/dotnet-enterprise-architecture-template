# API Integration Examples and Patterns
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | API-EXAMPLES-001 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Reference Documentation and Examples |
| Target Audience | API Consumers, Integration Developers, QA Engineers, Technical Writers, Solution Architects |
| Classification | Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Quarterly |
| Related Documents | API Guidelines (API-GUIDELINES-001), Error Handling (API-ERRORHANDLING-001), Authentication (API-AUTH-001), Async Operations (API-ASYNC-001) |
| Prerequisites | Understanding of HTTP protocol, REST principles, JSON format |

---

## Executive Summary

This document provides comprehensive, production-ready examples demonstrating complete API integration patterns for the .NET Enterprise Architecture Template. Each example illustrates proper implementation of REST principles, CQRS patterns, RFC 7807 error handling, authentication, pagination, and asynchronous operations. These concrete examples serve as authoritative references for API consumers, integration developers, and quality assurance teams.

**Strategic Business Value:**
- Accelerated integration development through ready-to-use examples
- Reduced integration errors via clear pattern demonstration
- Improved developer experience through comprehensive documentation
- Decreased support costs via self-service integration guidance
- Enhanced API adoption through clear usage patterns
- Standardized error handling reducing troubleshooting time

**Key Technical Capabilities:**
- Complete HTTP request/response examples for all common operations
- CQRS pattern demonstration (Commands vs Queries)
- RFC 7807 Problem Details error responses
- Authentication and authorization examples
- Pagination, filtering, and sorting patterns
- Asynchronous operation implementation (202 Accepted pattern)
- Rate limiting and quota management examples
- Idempotency key usage patterns

**Compliance and Standards:**
- HTTP/1.1 specification (RFC 7230-7235)
- RFC 7807 Problem Details for HTTP APIs
- OAuth 2.0 authentication patterns
- ISO 8601 datetime formatting
- REST architectural constraints
- OpenAPI 3.0 specification alignment

---

## Table of Contents

1. Introduction and Scope
2. Document Conventions
3. Command Operations (State Modification)
4. Query Operations (Data Retrieval)
5. Error Response Patterns
6. Authentication and Authorization Examples
7. Asynchronous Operation Patterns
8. Advanced Query Patterns
9. Rate Limiting and Throttling
10. Idempotency Patterns
11. Correlation and Tracing
12. Example Integration Flows
13. Glossary
14. Recommendations and Next Steps
15. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides authoritative, complete HTTP request and response examples for the .NET Enterprise Architecture Template API. Each example demonstrates proper implementation of established patterns including CQRS, error handling, authentication, pagination, and asynchronous operations. These examples serve as normative references for API consumers and integration developers.

### 1.2. Scope

**In Scope:**
- Complete HTTP request examples with headers and body
- Complete HTTP response examples with status codes, headers, and body
- Command operation examples (POST, PUT, PATCH, DELETE)
- Query operation examples (GET)
- Error response examples for all common scenarios
- Authentication and authorization examples
- Pagination, filtering, and sorting examples
- Asynchronous operation examples (202 Accepted pattern)
- Rate limiting examples
- Idempotency examples

**Out of Scope:**
- Client SDK code examples (language-specific implementations)
- Infrastructure setup and deployment
- Database schema definitions
- Internal implementation details
- Performance benchmarking data
- Load testing scenarios

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**API Consumers:** Complete request/response examples for integration

**Integration Developers:** Reference patterns for client implementation

**QA Engineers:** Test case templates and expected responses

**Technical Writers:** Content source for API documentation

**Solution Architects:** Pattern validation and compliance verification

### 1.4. Example Categories

Examples are organized by operation type:

**Commands (State Modification):**
- Resource creation (POST)
- Resource update (PUT, PATCH)
- Resource deletion (DELETE)
- Custom actions

**Queries (Data Retrieval):**
- Single resource retrieval (GET)
- Collection retrieval with pagination
- Filtered and sorted queries
- Complex search operations

**Error Scenarios:**
- Validation errors (400)
- Authorization errors (401, 403)
- Resource not found (404)
- Business rule violations (422)
- Server errors (500)

---

## 2. Document Conventions

### 2.1. Request/Response Format

#### 2.1.1. Structure

Each example follows this structure:

**Description:** Brief explanation of the operation

**Request:**
- HTTP method and URL
- Required and optional headers
- Request body (if applicable)

**Response:**
- HTTP status code and reason phrase
- Response headers
- Response body

#### 2.1.2. Variable Notation

| Notation | Description | Example |
|----------|-------------|---------|
| {variable} | Placeholder for actual value | {access_token} |
| {id} | Resource identifier | {product_id} |
| [...] | Abbreviated content | "items": [...] |

#### 2.1.3. HTTP Version

All examples use HTTP/1.1 unless otherwise specified. HTTP/2 is supported and recommended for production use.

### 2.2. Common Headers

#### 2.2.1. Standard Request Headers

| Header | Required | Description | Example |
|--------|----------|-------------|---------|
| Authorization | Yes (authenticated endpoints) | Bearer token | Bearer eyJhbGc... |
| Content-Type | Yes (with body) | Request content type | application/json |
| Accept | Recommended | Desired response format | application/json |
| X-Correlation-ID | Recommended | Request correlation identifier | 7f5a4b25-6804-4c9d... |
| Idempotency-Key | Recommended (mutations) | Idempotency key | 3f41c17b-f710-4d57... |

#### 2.2.2. Standard Response Headers

| Header | Always Present | Description |
|--------|----------------|-------------|
| Content-Type | Yes | Response content type |
| X-Correlation-ID | Yes (if provided in request) | Echoed correlation ID |
| Location | Yes (201 Created) | URI of created resource |
| ETag | Optional | Entity tag for caching |
| Cache-Control | Optional | Caching directives |

---

## 3. Command Operations (State Modification)

### 3.1. Resource Creation (POST)

#### 3.1.1. Create Product - Success Scenario

**Operation:** Create new product in catalog

**Business Rule:** Product name must be unique within tenant

**HTTP Request:**
```http
POST /api/v1/catalog/products HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
Accept: application/json
X-Correlation-ID: 7f5a4b25-6804-4c9d-9e7d-2f3a9b41d001
Idempotency-Key: 3f41c17b-f710-4d57-8e10-3a02b23b4ec9
```

**Request Body:**
```json
{
  "name": "Phone X",
  "description": "128 GB, black",
  "price": 799.00,
  "currency": "EUR"
}
```

**HTTP Response:**
```http
HTTP/1.1 201 Created
Location: /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 7f5a4b25-6804-4c9d-9e7d-2f3a9b41d001
```

**Response Body:**
```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Phone X",
  "description": "128 GB, black",
  "price": 799.00,
  "currency": "EUR",
  "isActive": true,
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T10:30:00Z"
}
```

**Key Points:**
- Status code 201 indicates successful creation
- Location header provides URI of created resource
- Response includes complete representation of created resource
- Timestamps follow ISO 8601 format with UTC timezone

### 3.2. Resource Update (PUT)

#### 3.2.1. Update Product - Full Replacement

**Operation:** Replace entire product representation

**HTTP Request:**
```http
PUT /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
Accept: application/json
X-Correlation-ID: 8b6f5d32-7915-4e0a-af8e-3g4b0c52e002
If-Match: "686897696a7c876b7e"
```

**Request Body:**
```json
{
  "name": "Phone X Pro",
  "description": "256 GB, black, upgraded",
  "price": 899.00,
  "currency": "EUR",
  "isActive": true
}
```

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 8b6f5d32-7915-4e0a-af8e-3g4b0c52e002
ETag: "787908707b8d987c8f"
```

**Response Body:**
```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Phone X Pro",
  "description": "256 GB, black, upgraded",
  "price": 899.00,
  "currency": "EUR",
  "isActive": true,
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-16T09:15:00Z"
}
```

**Key Points:**
- If-Match header ensures optimistic concurrency control
- All fields must be provided (full replacement)
- Response includes updated resource representation

### 3.3. Partial Update (PATCH)

#### 3.3.1. Update Product Price

**Operation:** Update only specific fields

**HTTP Request:**
```http
PATCH /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
Accept: application/json
X-Correlation-ID: 9c7g6e43-8026-5f1b-bg9f-4h5c1d63f003
```

**Request Body:**
```json
{
  "price": 849.00
}
```

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 9c7g6e43-8026-5f1b-bg9f-4h5c1d63f003
```

**Response Body:**
```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Phone X Pro",
  "description": "256 GB, black, upgraded",
  "price": 849.00,
  "currency": "EUR",
  "isActive": true,
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-16T11:22:00Z"
}
```

**Key Points:**
- Only specified fields updated
- Unspecified fields remain unchanged
- Response includes complete updated resource

### 3.4. Resource Deletion (DELETE)

#### 3.4.1. Delete Product

**Operation:** Remove product from catalog

**HTTP Request:**
```http
DELETE /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
X-Correlation-ID: 0d8h7f54-9137-6g2c-ch0g-5i6d2e74g004
```

**HTTP Response:**
```http
HTTP/1.1 204 No Content
X-Correlation-ID: 0d8h7f54-9137-6g2c-ch0g-5i6d2e74g004
```

**Key Points:**
- 204 No Content indicates successful deletion
- No response body required
- Subsequent GET requests return 404 Not Found

---

## 4. Query Operations (Data Retrieval)

### 4.1. Single Resource Retrieval

#### 4.1.1. Get Product by ID

**Operation:** Retrieve single product by identifier

**HTTP Request:**
```http
GET /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
X-Correlation-ID: 4f3e7a21-2b56-4ac7-8c98-81ed7ea5c002
```

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 4f3e7a21-2b56-4ac7-8c98-81ed7ea5c002
ETag: "686897696a7c876b7e"
Last-Modified: Wed, 15 Jan 2025 10:30:00 GMT
Cache-Control: public, max-age=300
```

**Response Body:**
```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Phone X",
  "description": "128 GB, black",
  "price": 799.00,
  "currency": "EUR",
  "isActive": true,
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-16T09:00:00Z"
}
```

**Key Points:**
- ETag enables conditional requests and caching
- Last-Modified supports HTTP caching mechanisms
- Cache-Control specifies caching behavior

### 4.2. Collection Retrieval with Pagination

#### 4.2.1. Get Products - Simple Pagination

**Operation:** Retrieve paginated list of products

**HTTP Request:**
```http
GET /api/v1/catalog/products?page=1&pageSize=20 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
X-Correlation-ID: 5g4f8b32-3c67-5bd8-9d09-92fe8fb6d003
```

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 5g4f8b32-3c67-5bd8-9d09-92fe8fb6d003
Cache-Control: public, max-age=60
```

**Response Body:**
```json
{
  "items": [
    {
      "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
      "name": "Phone X",
      "price": 799.00,
      "currency": "EUR",
      "isActive": true
    },
    {
      "id": "c7b0f5d4-8be3-5ea3-0c10-2e5f7g07a1g2",
      "name": "Tablet Pro",
      "price": 1299.00,
      "currency": "EUR",
      "isActive": true
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalItems": 150,
  "totalPages": 8
}
```

**Pagination Metadata:**

| Field | Type | Description |
|-------|------|-------------|
| items | array | Current page items |
| page | integer | Current page number (1-based) |
| pageSize | integer | Items per page |
| totalItems | integer | Total items across all pages |
| totalPages | integer | Total number of pages |

### 4.3. Advanced Query with Filtering and Sorting

#### 4.3.1. Complex Product Search

**Operation:** Search products with multiple criteria

**HTTP Request:**
```http
GET /api/v1/catalog/products?search=phone&minPrice=200&maxPrice=1200&sort=price_desc&page=1&pageSize=20 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
X-Correlation-ID: 8ac45f9c-3f92-4b9a-9a6b-6f1d20c9f003
```

**Query Parameters:**

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| search | string | Free-text search | phone |
| minPrice | decimal | Minimum price filter | 200 |
| maxPrice | decimal | Maximum price filter | 1200 |
| sort | string | Sort specification | price_desc |
| page | integer | Page number | 1 |
| pageSize | integer | Items per page | 20 |

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 8ac45f9c-3f92-4b9a-9a6b-6f1d20c9f003
Cache-Control: public, max-age=60
```

**Response Body:**
```json
{
  "items": [
    {
      "id": "d8c1g6e5-9cf4-6fb4-1d21-3f6g8h18b2h3",
      "name": "Phone Z Ultra",
      "description": "512 GB, Premium",
      "price": 1199.00,
      "currency": "EUR",
      "isActive": true,
      "category": {
        "id": "cat-001",
        "name": "Smartphones"
      }
    },
    {
      "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
      "name": "Phone X Pro",
      "description": "256 GB, black",
      "price": 899.00,
      "currency": "EUR",
      "isActive": true,
      "category": {
        "id": "cat-001",
        "name": "Smartphones"
      }
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalItems": 42,
  "totalPages": 3
}
```

**Key Points:**
- Results sorted by price descending
- Filtering applied before pagination
- Category information included via expansion

---

## 5. Error Response Patterns

### 5.1. Validation Errors (400 Bad Request)

#### 5.1.1. Multiple Validation Failures

**Operation:** Create product with invalid data

**HTTP Request:**
```http
POST /api/v1/catalog/products HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
Accept: application/problem+json
X-Correlation-ID: 2e6c522a-afad-4f23-a6d4-29a961c2f004
```

**Request Body:**
```json
{
  "name": "",
  "description": "Test product",
  "price": -10.00,
  "currency": "EURO"
}
```

**HTTP Response:**
```http
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json; charset=utf-8
X-Correlation-ID: 2e6c522a-afad-4f23-a6d4-29a961c2f004
```

**Response Body:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "See errors property for field-level validation details.",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-b4c3d5e6f7a8b9c0-01",
  "errors": {
    "name": [
      "Product name is required.",
      "Product name must not be empty."
    ],
    "price": [
      "Price must be greater than zero."
    ],
    "currency": [
      "Currency must be a valid ISO 4217 code (e.g., EUR, USD, GBP)."
    ]
  }
}
```

**Error Structure:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| type | string (URI) | Yes | Problem type identifier |
| title | string | Yes | Short problem summary |
| status | integer | Yes | HTTP status code |
| detail | string | No | Detailed explanation |
| traceId | string | Yes | Distributed trace identifier |
| errors | object | Yes (400) | Field-level error messages |

### 5.2. Business Rule Violations (422 Unprocessable Entity)

#### 5.2.1. Domain Invariant Violation

**Operation:** Attempt to deactivate product in active campaign

**HTTP Request:**
```http
PATCH /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
Accept: application/problem+json
X-Correlation-ID: a9a1b9d1-7dc2-4bef-b7e7-7b9973a1c005
```

**Request Body:**
```json
{
  "isActive": false
}
```

**HTTP Response:**
```http
HTTP/1.1 422 Unprocessable Entity
Content-Type: application/problem+json; charset=utf-8
X-Correlation-ID: a9a1b9d1-7dc2-4bef-b7e7-7b9973a1c005
```

**Response Body:**
```json
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "One or more business rules were violated.",
  "status": 422,
  "detail": "The product cannot be deactivated because it is part of an active marketing campaign.",
  "traceId": "00-1a93f4d5c8ab648794d4ef69334b1d51-c5d4e6f8g9h0i1j2-01",
  "errorCode": "PRODUCT_IN_ACTIVE_CAMPAIGN",
  "errors": {
    "isActive": [
      "Product is part of active campaign 'Summer Sale 2025' and cannot be deactivated until campaign ends."
    ]
  }
}
```

**Key Points:**
- 422 indicates syntactically valid but semantically invalid request
- Business rule violation clearly explained
- Error code provided for programmatic handling

### 5.3. Resource Not Found (404)

#### 5.3.1. Non-Existent Product

**Operation:** Retrieve product that does not exist

**HTTP Request:**
```http
GET /api/v1/catalog/products/00000000-0000-0000-0000-000000000000 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
X-Correlation-ID: 6b5a5e9d-ef32-4d9a-9c33-5f7d1234a006
```

**HTTP Response:**
```http
HTTP/1.1 404 Not Found
Content-Type: application/problem+json; charset=utf-8
X-Correlation-ID: 6b5a5e9d-ef32-4d9a-9c33-5f7d1234a006
```

**Response Body:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource not found.",
  "status": 404,
  "detail": "Product with ID '00000000-0000-0000-0000-000000000000' was not found.",
  "traceId": "00-7773f4d5c8ab648794d4ef69334b1d51-d6e5f7g9h0i1j2k3-01"
}
```

---

## 6. Authentication and Authorization Examples

### 6.1. Unauthenticated Request (401)

#### 6.1.1. Missing Authentication Token

**Operation:** Access protected resource without token

**HTTP Request:**
```http
GET /api/v1/catalog/products HTTP/1.1
Host: api.example.com
Accept: application/json
X-Correlation-ID: 7c6b6fa0-fg43-5ea0-ad44-6g8e2345b007
```

**HTTP Response:**
```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer
Content-Type: application/problem+json; charset=utf-8
X-Correlation-ID: 7c6b6fa0-fg43-5ea0-ad44-6g8e2345b007
```

**Response Body:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication is required to access this resource. Provide a valid Bearer token in the Authorization header.",
  "traceId": "00-8884g5e6d9bc759805e5fg70445c2e62-e7f6g8h0i1j2k3l4-01"
}
```

**Key Points:**
- WWW-Authenticate header indicates authentication scheme
- Clear guidance on required authentication

### 6.2. Insufficient Permissions (403)

#### 6.2.1. Unauthorized Delete Operation

**Operation:** Attempt to delete product without admin role

**HTTP Request:**
```http
DELETE /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiRWRpdG9yIn0...
X-Correlation-ID: 8d7c7gb1-gh54-6fb1-be55-7h9f3456c008
```

**HTTP Response:**
```http
HTTP/1.1 403 Forbidden
Content-Type: application/problem+json; charset=utf-8
X-Correlation-ID: 8d7c7gb1-gh54-6fb1-be55-7h9f3456c008
```

**Response Body:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have permission to delete products. This operation requires 'Admin' role.",
  "traceId": "00-9995h6f7e0cd860916f6gh81556d3f73-f8g7h9i1j2k3l4m5-01",
  "requiredPermission": "catalog.delete",
  "userRoles": ["Editor"]
}
```

**Key Points:**
- Authenticated but insufficient permissions
- Required permission clearly stated
- Current user roles disclosed

---

## 7. Asynchronous Operation Patterns

### 7.1. Job Submission (202 Accepted)

#### 7.1.1. Initiate Report Generation

**Operation:** Start long-running report generation

**HTTP Request:**
```http
POST /api/v1/reports/generate HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
Accept: application/json
X-Correlation-ID: 3f5ea98e-5f4c-4df2-b828-27e6844a7007
Idempotency-Key: 2b7c5da2-3d12-43d8-a960-06180f975555
```

**Request Body:**
```json
{
  "type": "market-analysis",
  "parameters": {
    "region": "EU",
    "year": 2025,
    "includeForecasts": true
  }
}
```

**HTTP Response:**
```http
HTTP/1.1 202 Accepted
Location: /api/v1/jobs/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91
Retry-After: 5
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 3f5ea98e-5f4c-4df2-b828-27e6844a7007
```

**Response Body:**
```json
{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Queued",
  "statusUrl": "/api/v1/jobs/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "submittedAt": "2025-11-30T10:32:10Z",
  "estimatedDuration": 120
}
```

**Key Points:**
- 202 Accepted indicates asynchronous processing
- Location header provides job status URL
- Retry-After suggests polling interval

### 7.2. Job Status Polling

#### 7.2.1. Job Running Status

**Operation:** Check job status while processing

**HTTP Request:**
```http
GET /api/v1/jobs/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
X-Correlation-ID: 5c7f3a11-9e34-4a1b-a4b1-672e014f0008
```

**HTTP Response:**
```http
HTTP/1.1 202 Accepted
Content-Type: application/json; charset=utf-8
Retry-After: 5
X-Correlation-ID: 5c7f3a11-9e34-4a1b-a4b1-672e014f0008
```

**Response Body:**
```json
{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Running",
  "progress": 45,
  "message": "Collecting data from external systems...",
  "submittedAt": "2025-11-30T10:32:10Z",
  "startedAt": "2025-11-30T10:32:15Z",
  "updatedAt": "2025-11-30T10:32:45Z",
  "estimatedCompletionAt": "2025-11-30T10:34:10Z"
}
```

#### 7.2.2. Job Completed Status

**Operation:** Check completed job

**HTTP Request:**
```http
GET /api/v1/jobs/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
X-Correlation-ID: 6d8g4b22-0f45-5b2c-b5c2-783f125g0009
```

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 6d8g4b22-0f45-5b2c-b5c2-783f125g0009
```

**Response Body:**
```json
{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Completed",
  "submittedAt": "2025-11-30T10:32:10Z",
  "startedAt": "2025-11-30T10:32:15Z",
  "completedAt": "2025-11-30T10:34:05Z",
  "duration": 115,
  "resultUrl": "/api/v1/reports/result/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91"
}
```

**Key Points:**
- Status code changes to 200 OK when complete
- Result URL provided for download

---

## 8. Advanced Query Patterns

### 8.1. Field Selection (Sparse Fieldsets)

#### 8.1.1. Retrieve Specific Fields Only

**Operation:** Get products with only selected fields

**HTTP Request:**
```http
GET /api/v1/catalog/products?fields=id,name,price,currency HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
X-Correlation-ID: 7e9h5c33-1g56-6c3d-c6d3-894g236h1010
```

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 7e9h5c33-1g56-6c3d-c6d3-894g236h1010
```

**Response Body:**
```json
{
  "items": [
    {
      "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
      "name": "Phone X",
      "price": 799.00,
      "currency": "EUR"
    },
    {
      "id": "c7b0f5d4-8be3-5ea3-0c10-2e5f7g07a1g2",
      "name": "Tablet Pro",
      "price": 1299.00,
      "currency": "EUR"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalItems": 150,
  "totalPages": 8
}
```

**Benefits:**
- Reduced payload size
- Improved network performance
- Lower bandwidth consumption

### 8.2. Resource Expansion

#### 8.2.1. Include Related Resources

**Operation:** Expand category information inline

**HTTP Request:**
```http
GET /api/v1/catalog/products?include=category,manufacturer&page=1&pageSize=10 HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
X-Correlation-ID: 8f0i6d44-2h67-7d4e-d7e4-905h347i2011
```

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 8f0i6d44-2h67-7d4e-d7e4-905h347i2011
```

**Response Body:**
```json
{
  "items": [
    {
      "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
      "name": "Phone X",
      "price": 799.00,
      "currency": "EUR",
      "category": {
        "id": "cat-001",
        "name": "Smartphones",
        "slug": "smartphones"
      },
      "manufacturer": {
        "id": "mfg-001",
        "name": "TechCorp",
        "country": "US"
      }
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalItems": 150,
  "totalPages": 15
}
```

**Key Points:**
- Related resources embedded inline
- Reduces need for additional requests
- Client controls expansion via query parameter

---

## 9. Rate Limiting and Throttling

### 9.1. Rate Limit Exceeded (429)

#### 9.1.1. Quota Exhausted

**Operation:** Request when rate limit exceeded

**HTTP Request:**
```http
GET /api/v1/catalog/products HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
X-Correlation-ID: 9g1j7e55-3i78-8e5f-e8f5-016i458j3012
```

**HTTP Response:**
```http
HTTP/1.1 429 Too Many Requests
Content-Type: application/problem+json; charset=utf-8
Retry-After: 60
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1700001234
X-Correlation-ID: 9g1j7e55-3i78-8e5f-e8f5-016i458j3012
```

**Response Body:**
```json
{
  "type": "https://tools.ietf.org/html/rfc6585#section-4",
  "title": "Too Many Requests",
  "status": 429,
  "detail": "Rate limit exceeded. You have made 1000 requests in the current time window. Please retry after 60 seconds.",
  "traceId": "00-0116j7g8f1de971027g7hi92667e4g84-g9h8i0j1k2l3m4n5-01",
  "retryAfter": 60,
  "rateLimitWindow": "1 hour"
}
```

**Rate Limit Headers:**

| Header | Description | Example |
|--------|-------------|---------|
| X-RateLimit-Limit | Total requests allowed per window | 1000 |
| X-RateLimit-Remaining | Requests remaining in current window | 0 |
| X-RateLimit-Reset | Unix timestamp when limit resets | 1700001234 |
| Retry-After | Seconds to wait before retry | 60 |

---

## 10. Idempotency Patterns

### 10.1. Idempotent Request

#### 10.1.1. Duplicate Request with Same Key

**Operation:** Retry request with same idempotency key

**First Request:**
```http
POST /api/v1/catalog/products HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
Idempotency-Key: unique-key-12345
X-Correlation-ID: 0h2k8f66-4j89-9f6g-f9g6-127j569k4013
```

**Request Body:**
```json
{
  "name": "Test Product",
  "price": 99.99,
  "currency": "USD"
}
```

**First Response:**
```http
HTTP/1.1 201 Created
Location: /api/v1/catalog/products/product-id-xyz
X-Correlation-ID: 0h2k8f66-4j89-9f6g-f9g6-127j569k4013
```

**Retry Request (Same Idempotency Key):**
```http
POST /api/v1/catalog/products HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
Idempotency-Key: unique-key-12345
X-Correlation-ID: 1i3l9g77-5k90-0g7h-g0h7-238k670l5014
```

**Retry Response (Cached):**
```http
HTTP/1.1 201 Created
Location: /api/v1/catalog/products/product-id-xyz
X-Idempotent-Replayed: true
X-Correlation-ID: 1i3l9g77-5k90-0g7h-g0h7-238k670l5014
```

**Key Points:**
- Same idempotency key returns cached result
- No duplicate resource created
- X-Idempotent-Replayed header indicates cached response

---

## 11. Correlation and Tracing

### 11.1. Distributed Tracing

#### 11.1.1. Request with Correlation ID

**Purpose:** Track requests across microservices

**HTTP Request:**
```http
POST /api/v1/catalog/products HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
X-Correlation-ID: 2j4m0h88-6l01-1h8i-h1i8-349l781m6015
```

**HTTP Response:**
```http
HTTP/1.1 201 Created
X-Correlation-ID: 2j4m0h88-6l01-1h8i-h1i8-349l781m6015
```

**Correlation ID Format:**
- UUID v4 format
- Client-generated or server-generated
- Propagated across all service calls
- Included in all log entries
- Enables end-to-end request tracing

---

## 12. Example Integration Flows

### 12.1. Complete CRUD Workflow

#### 12.1.1. End-to-End Product Lifecycle

**Scenario:** Create, retrieve, update, and delete product

**Step 1: Create Product**
```
POST /api/v1/catalog/products
→ 201 Created
→ Location: /api/v1/catalog/products/{id}
```

**Step 2: Retrieve Product**
```
GET /api/v1/catalog/products/{id}
→ 200 OK
→ Returns product details
```

**Step 3: Update Product**
```
PUT /api/v1/catalog/products/{id}
→ 200 OK
→ Returns updated product
```

**Step 4: Delete Product**
```
DELETE /api/v1/catalog/products/{id}
→ 204 No Content
```

**Step 5: Verify Deletion**
```
GET /api/v1/catalog/products/{id}
→ 404 Not Found
```

### 12.2. Search and Filter Workflow

**Scenario:** Search products, filter results, navigate pages

**Step 1: Initial Search**
```
GET /api/v1/catalog/products?search=phone&page=1&pageSize=20
→ 200 OK (20 items, page 1 of 5)
```

**Step 2: Apply Price Filter**
```
GET /api/v1/catalog/products?search=phone&minPrice=500&maxPrice=1000&page=1&pageSize=20
→ 200 OK (15 items, page 1 of 1)
```

**Step 3: Sort Results**
```
GET /api/v1/catalog/products?search=phone&minPrice=500&maxPrice=1000&sort=price_asc
→ 200 OK (results sorted by price ascending)
```

---

## 13. Glossary

**202 Accepted:** HTTP status code indicating asynchronous processing initiated.

**CQRS:** Command Query Responsibility Segregation - separation of read and write operations.

**Correlation ID:** Unique identifier tracking requests across distributed systems.

**ETag:** Entity tag for HTTP caching and optimistic concurrency control.

**Idempotency Key:** Unique identifier ensuring duplicate requests produce identical results.

**Problem Details:** RFC 7807 standardized error response format.

**Rate Limiting:** Controlling request frequency to protect system resources.

**Sparse Fieldsets:** Selecting specific fields to reduce response payload size.

---

## 14. Recommendations and Next Steps

### 14.1. For API Consumers

**Integration Checklist:**
- [ ] Implement proper error handling for all status codes
- [ ] Use correlation IDs for request tracing
- [ ] Implement idempotency keys for mutation operations
- [ ] Respect rate limits and implement exponential backoff
- [ ] Cache responses appropriately using ETags
- [ ] Handle asynchronous operations with polling
- [ ] Validate responses against expected schema

### 14.2. For Development Teams

**Implementation Guidelines:**
- Use examples as template for new endpoints
- Ensure all responses follow RFC 7807 format
- Implement consistent pagination patterns
- Support field selection and resource expansion
- Add comprehensive integration tests based on examples

### 14.3. For QA Engineers

**Testing Checklist:**
- Verify all success scenarios match examples
- Test all error scenarios documented
- Validate HTTP headers present and correct
- Test pagination edge cases
- Verify idempotency behavior
- Test rate limiting enforcement

### 14.4. Related Documentation

**Must Read:**
- API Guidelines (API-GUIDELINES-001)
- Error Handling Specification (API-ERRORHANDLING-001)
- Authentication Specification (API-AUTH-001)
- Asynchronous Operations (API-ASYNC-001)

---

## 15. References

### 15.1. HTTP Standards

**HTTP/1.1 Specification**
- RFC 7230-7235: https://tools.ietf.org/html/rfc7230

**Problem Details for HTTP APIs**
- RFC 7807: https://tools.ietf.org/html/rfc7807

**Rate Limiting**
- RFC 6585: https://tools.ietf.org/html/rfc6585

### 15.2. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- API-GUIDELINES-001: API Design Guidelines
- API-ERRORHANDLING-001: Error Handling and Problem Details
- API-AUTH-001: Authentication and Authorization
- API-ASYNC-001: Asynchronous Operations
- ARCH-CQRS-003: Command Query Responsibility Segregation

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial API integration examples with comprehensive request/response patterns |

---

**END OF DOCUMENT**