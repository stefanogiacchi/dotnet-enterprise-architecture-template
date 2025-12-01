# HTTP Headers and Conventions Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | API-HEADERS-001 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Technical Specification |
| Target Audience | API Designers, Development Teams, Integration Engineers, Operations Teams, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Annual |
| Related Documents | API Guidelines (API-GUIDELINES-001), Error Handling (API-ERRORHANDLING-001), Versioning (API-VERSIONING-001), Async Operations (API-ASYNC-001) |
| Prerequisites | Understanding of HTTP protocol, REST principles, Header semantics |

---

## Executive Summary

This document establishes comprehensive HTTP header specifications for the .NET Enterprise Architecture Template, defining standard headers for correlation tracking, idempotency enforcement, authentication, caching, rate limiting, and API versioning. Proper HTTP header implementation enables enterprise-grade observability, safe retry mechanisms, performance optimization, and transparent client-server communication essential for distributed systems.

**Strategic Business Value:**
- Enhanced system observability through distributed request tracing
- Reduced operational costs via efficient troubleshooting and debugging
- Improved reliability through idempotent operation support
- Optimized performance via intelligent caching strategies
- Decreased infrastructure costs through rate limiting and quota management
- Better client experience through transparent deprecation communication

**Key Technical Capabilities:**
- W3C Trace Context compliant correlation tracking
- Idempotency key support for safe request retries
- RFC 7234 compliant HTTP caching mechanisms
- Standard rate limiting headers with quota information
- API version and deprecation communication
- Conditional request support (ETag, Last-Modified)
- Asynchronous operation coordination headers

**Compliance and Standards:**
- HTTP/1.1 specification (RFC 7230-7235)
- RFC 7234 HTTP caching
- W3C Trace Context specification
- OAuth 2.0 Bearer token authentication (RFC 6750)
- RFC 7807 Problem Details integration
- OpenTelemetry distributed tracing standards

---

## Table of Contents

1. Introduction and Scope
2. Header Classification and Categories
3. Correlation and Distributed Tracing Headers
4. Idempotency Headers
5. Authentication and Authorization Headers
6. Caching Headers
7. Rate Limiting and Throttling Headers
8. API Versioning and Deprecation Headers
9. Content Negotiation Headers
10. Asynchronous Operation Headers
11. Custom Header Conventions
12. Header Propagation Patterns
13. Testing Requirements
14. Security Considerations
15. Glossary
16. Recommendations and Next Steps
17. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides authoritative specifications for HTTP headers used within the .NET Enterprise Architecture Template APIs. It defines standard headers for correlation tracking, idempotency, authentication, caching, rate limiting, versioning, and content negotiation, ensuring consistent implementation across all API endpoints and enabling enterprise-grade observability, reliability, and performance.

### 1.2. Scope

**In Scope:**
- Standard HTTP headers for correlation and tracing
- Idempotency header specifications and implementation
- Authentication and authorization headers
- Caching control headers (request and response)
- Rate limiting and quota management headers
- API versioning and deprecation communication headers
- Content negotiation headers
- Asynchronous operation coordination headers
- Custom header naming conventions
- Header propagation across distributed systems
- Testing requirements for header implementation

**Out of Scope:**
- HTTP/2 and HTTP/3 specific features (HPACK compression, server push)
- WebSocket protocol headers
- Server-Sent Events (SSE) headers
- gRPC metadata specifications
- Infrastructure-level headers (load balancer, CDN)
- Proprietary vendor-specific headers

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**API Designers:** Header selection and implementation patterns

**Development Teams:** Implementation guidelines and code examples

**Integration Engineers:** Client-side header usage and propagation

**Operations Teams:** Monitoring, logging, and troubleshooting using headers

**Security Teams:** Authentication and authorization header validation

**C-Level Executives:** Business value and operational benefits

### 1.4. Header Categories Overview

Headers are organized into functional categories:

```
┌─────────────────────────────────────────────────────────┐
│                 HTTP Header Categories                  │
└─────────────────────────────────────────────────────────┘

┌───────────────────┐  ┌───────────────────┐
│   Correlation &   │  │   Idempotency     │
│     Tracing       │  │                   │
│ • X-Correlation-ID│  │ • Idempotency-Key │
│ • Trace Context   │  │                   │
└───────────────────┘  └───────────────────┘

┌───────────────────┐  ┌───────────────────┐
│  Authentication   │  │     Caching       │
│                   │  │                   │
│ • Authorization   │  │ • Cache-Control   │
│ • Bearer Token    │  │ • ETag            │
└───────────────────┘  │ • If-None-Match   │
                       └───────────────────┘

┌───────────────────┐  ┌───────────────────┐
│  Rate Limiting    │  │   Versioning      │
│                   │  │                   │
│ • X-RateLimit-*   │  │ • API-Version     │
│ • Retry-After     │  │ • Deprecation     │
└───────────────────┘  │ • Sunset          │
                       └───────────────────┘
```

---

## 2. Header Classification and Categories

### 2.1. Standard vs Custom Headers

#### 2.1.1. Standard Headers

**Definition:** Headers defined in HTTP specifications (RFC 7230-7235) or widely adopted standards.

**Examples:**
- Authorization
- Content-Type
- Cache-Control
- ETag
- Last-Modified

**Usage:** Use standard headers whenever applicable before creating custom headers.

#### 2.1.2. Custom Headers

**Definition:** Application-specific headers not defined in HTTP specifications.

**Naming Convention:** Prefix with `X-` (legacy convention) or application namespace.

**Examples:**
- X-Correlation-ID
- X-RateLimit-Limit
- X-Idempotency-Replayed

**Guideline:** Minimize custom headers; prefer standard headers when available.

### 2.2. Request vs Response Headers

#### 2.2.1. Request Headers

**Purpose:** Provide metadata about the request or desired response behavior.

**Common Request Headers:**

| Header | Category | Purpose |
|--------|----------|---------|
| Authorization | Security | Authentication credentials |
| Content-Type | Content | Request body format |
| Accept | Content | Desired response format |
| X-Correlation-ID | Tracing | Request correlation identifier |
| Idempotency-Key | Safety | Request idempotency key |
| If-None-Match | Caching | Conditional request (ETag) |
| If-Modified-Since | Caching | Conditional request (timestamp) |

#### 2.2.2. Response Headers

**Purpose:** Provide metadata about the response or server state.

**Common Response Headers:**

| Header | Category | Purpose |
|--------|----------|---------|
| Content-Type | Content | Response body format |
| Location | Navigation | Created resource URI (201) |
| ETag | Caching | Entity tag for resource version |
| Cache-Control | Caching | Caching directives |
| X-Correlation-ID | Tracing | Echoed correlation identifier |
| X-RateLimit-* | Throttling | Rate limit information |
| Retry-After | Throttling | Retry delay specification |

### 2.3. Header Precedence

When multiple headers control the same behavior:

**Precedence Order:**
1. Most specific header
2. Standard header over custom header
3. Request header overrides default behavior
4. Response header confirms actual behavior

---

## 3. Correlation and Distributed Tracing Headers

### 3.1. Correlation Identifier

#### 3.1.1. X-Correlation-ID Header

**Purpose:** Unique identifier tracking a logical request across distributed systems.

**Format:** UUID v4 (recommended) or any unique string

**Direction:** Bidirectional (request and response)

**Behavior:**

**Client Provides Header:**
```http
GET /api/v1/products HTTP/1.1
X-Correlation-ID: 7f5a4b25-6804-4c9d-9e7d-2f3a9b41d001
```

**Server Response:**
```http
HTTP/1.1 200 OK
X-Correlation-ID: 7f5a4b25-6804-4c9d-9e7d-2f3a9b41d001
```

**Server Generates Header (if absent):**
```http
GET /api/v1/products HTTP/1.1
(no X-Correlation-ID provided)
```

**Server Response:**
```http
HTTP/1.1 200 OK
X-Correlation-ID: a8b6c539-7915-5f2b-af0e-4h5c1d63f003
```

#### 3.1.2. Implementation Requirements

**Server-Side Requirements:**

**Extract or Generate:**
```csharp
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"]
            .FirstOrDefault() ?? Guid.NewGuid().ToString();

        // Add to response
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        // Add to logging context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
```

**Propagation Requirements:**
- Propagate to all downstream HTTP calls
- Include in message queue messages
- Include in database operation logs
- Include in error responses (ProblemDetails traceId)

#### 3.1.3. Logging Integration

**Structured Logging:**
```csharp
_logger.LogInformation(
    "Processing request. CorrelationId: {CorrelationId}, Method: {Method}, Path: {Path}",
    correlationId,
    context.Request.Method,
    context.Request.Path);
```

**Log Output:**
```
[2025-01-15 10:30:00] INFO Processing request. CorrelationId: 7f5a4b25-6804-4c9d-9e7d-2f3a9b41d001, Method: GET, Path: /api/v1/products
```

### 3.2. W3C Trace Context

#### 3.2.1. Traceparent Header

**Purpose:** W3C standard for distributed tracing context propagation.

**Format:** `{version}-{trace-id}-{parent-id}-{trace-flags}`

**Example:**
```http
traceparent: 00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-01
```

**Components:**

| Component | Length | Description | Example |
|-----------|--------|-------------|---------|
| version | 2 hex | Version (00) | 00 |
| trace-id | 32 hex | Trace identifier | 0af7651916cd43dd8448eb211c80319c |
| parent-id | 16 hex | Parent span identifier | b7ad6b7169203331 |
| trace-flags | 2 hex | Trace flags | 01 (sampled) |

**Integration:** OpenTelemetry SDK automatically handles traceparent propagation.

---

## 4. Idempotency Headers

### 4.1. Idempotency-Key Header

#### 4.1.1. Purpose and Use Cases

**Purpose:** Enable safe retries of non-idempotent operations (typically POST requests).

**Use Cases:**
- Payment processing
- Order placement
- Report generation
- Resource creation with side effects
- Critical business operations

**Not Required For:**
- Idempotent methods (GET, PUT, DELETE)
- Read-only operations
- Non-critical operations

#### 4.1.2. Header Specification

**Header Name:** `Idempotency-Key`

**Format:** UUID v4 (recommended) or client-generated unique string

**Direction:** Request only

**Example:**
```http
POST /api/v1/orders HTTP/1.1
Authorization: Bearer {token}
Content-Type: application/json
Idempotency-Key: 3f41c17b-f710-4d57-8e10-3a02b23b4ec9

{
  "customerId": "customer-123",
  "items": [...]
}
```

#### 4.1.3. Server-Side Behavior

**First Request (New Key):**
1. Store idempotency key with request hash
2. Process operation normally
3. Store operation result
4. Return response with status 201 Created

**Duplicate Request (Same Key):**
1. Detect existing idempotency key
2. Retrieve cached operation result
3. Return same response with status 201 Created
4. Add `X-Idempotent-Replayed: true` header

**Example Duplicate Response:**
```http
HTTP/1.1 201 Created
Location: /api/v1/orders/order-id-xyz
X-Idempotent-Replayed: true
X-Correlation-ID: 8b6f5d32-7915-4e0a-af8e-3g4b0c52e002

{
  "orderId": "order-id-xyz",
  "status": "Created",
  "createdAt": "2025-01-15T10:30:00Z"
}
```

#### 4.1.4. Implementation Pattern

```csharp
public async Task<IActionResult> CreateOrder(
    CreateOrderRequest request,
    [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
{
    if (!string.IsNullOrEmpty(idempotencyKey))
    {
        // Check for existing operation
        var existingResult = await _idempotencyStore
            .GetResultAsync(idempotencyKey);

        if (existingResult != null)
        {
            Response.Headers["X-Idempotent-Replayed"] = "true";
            return StatusCode(
                existingResult.StatusCode,
                existingResult.Body);
        }
    }

    // Process new request
    var order = await _orderService.CreateOrderAsync(request);

    // Store result for idempotency
    if (!string.IsNullOrEmpty(idempotencyKey))
    {
        await _idempotencyStore.StoreResultAsync(
            idempotencyKey,
            201,
            order,
            expiresIn: TimeSpan.FromHours(24));
    }

    return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
}
```

#### 4.1.5. Storage Requirements

**Idempotency Store:**
- Key: Idempotency-Key value
- Value: Response status code, headers, body
- TTL: 24 hours (configurable)
- Technology: Redis, distributed cache, database

**Conflict Handling:**
- Same key + different request body = 409 Conflict
- Same key + same request body = Return cached result

---

## 5. Authentication and Authorization Headers

### 5.1. Authorization Header

#### 5.1.1. Bearer Token Authentication

**Header Name:** `Authorization`

**Format:** `Bearer {access_token}`

**Example:**
```http
GET /api/v1/products HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

**Token Types:**
- JWT (JSON Web Token)
- OAuth 2.0 Access Token
- OpenID Connect ID Token

#### 5.1.2. WWW-Authenticate Header

**Purpose:** Indicate authentication scheme required when 401 returned.

**Response Example:**
```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer realm="api.example.com", error="invalid_token", error_description="Token expired"
```

**Parameters:**

| Parameter | Required | Description |
|-----------|----------|-------------|
| realm | No | Protected realm identifier |
| error | No | Error code (invalid_token, invalid_request) |
| error_description | No | Human-readable error description |

**Reference:** See API-AUTH-001 for complete authentication specification.

---

## 6. Caching Headers

### 6.1. Cache-Control Header

#### 6.1.1. Purpose and Directives

**Purpose:** Control caching behavior for HTTP responses.

**Common Directives:**

| Directive | Description | Use Case |
|-----------|-------------|----------|
| no-store | Do not cache | Sensitive data, personalized content |
| no-cache | Revalidate before use | Dynamic content requiring freshness |
| public | Cacheable by any cache | Public resources |
| private | Cacheable by client only | User-specific content |
| max-age={seconds} | Cache lifetime | Static resources, catalog data |
| must-revalidate | Revalidate when stale | Important but cacheable content |

#### 6.1.2. Example Configurations

**Sensitive Endpoints:**
```http
HTTP/1.1 200 OK
Cache-Control: no-store, no-cache, must-revalidate
```

**Public Catalog (5 minutes):**
```http
HTTP/1.1 200 OK
Cache-Control: public, max-age=300
```

**User-Specific Data (1 minute):**
```http
HTTP/1.1 200 OK
Cache-Control: private, max-age=60
```

### 6.2. ETag Header

#### 6.2.1. Purpose

**Purpose:** Entity tag for resource version identification, enabling conditional requests and optimistic concurrency control.

**Format:** Quoted string (strong) or W/"string" (weak)

**Example:**
```http
HTTP/1.1 200 OK
ETag: "686897696a7c876b7e"
Content-Type: application/json

{...}
```

#### 6.2.2. Conditional Requests

**If-None-Match (Caching):**

**Client Request:**
```http
GET /api/v1/products/product-123 HTTP/1.1
If-None-Match: "686897696a7c876b7e"
```

**Server Response (Not Modified):**
```http
HTTP/1.1 304 Not Modified
ETag: "686897696a7c876b7e"
```

**If-Match (Optimistic Concurrency):**

**Client Request:**
```http
PUT /api/v1/products/product-123 HTTP/1.1
If-Match: "686897696a7c876b7e"
Content-Type: application/json

{...}
```

**Server Response (Conflict):**
```http
HTTP/1.1 412 Precondition Failed
ETag: "787908707b8d987c8f"
```

#### 6.2.3. ETag Generation

**Strategies:**

**Hash-Based:**
```csharp
public string GenerateETag(object resource)
{
    var json = JsonSerializer.Serialize(resource);
    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
    return $"\"{Convert.ToBase64String(hash)}\"";
}
```

**Version-Based:**
```csharp
public string GenerateETag(Product product)
{
    return $"\"{product.Version}\"";
}
```

### 6.3. Last-Modified Header

#### 6.3.1. Purpose

**Purpose:** Timestamp-based alternative to ETag for conditional requests.

**Format:** HTTP-date (RFC 7231)

**Example:**
```http
HTTP/1.1 200 OK
Last-Modified: Wed, 15 Jan 2025 10:30:00 GMT
```

#### 6.3.2. Conditional Request

**Client Request:**
```http
GET /api/v1/products/product-123 HTTP/1.1
If-Modified-Since: Wed, 15 Jan 2025 10:30:00 GMT
```

**Server Response (Not Modified):**
```http
HTTP/1.1 304 Not Modified
Last-Modified: Wed, 15 Jan 2025 10:30:00 GMT
```

---

## 7. Rate Limiting and Throttling Headers

### 7.1. Rate Limit Information Headers

#### 7.1.1. Standard Rate Limit Headers

**Header Set:**

| Header | Purpose | Format |
|--------|---------|--------|
| X-RateLimit-Limit | Total requests allowed | Integer |
| X-RateLimit-Remaining | Requests remaining in window | Integer |
| X-RateLimit-Reset | Window reset time | Unix timestamp |

**Example Response:**
```http
HTTP/1.1 200 OK
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 847
X-RateLimit-Reset: 1700001234
```

#### 7.1.2. Rate Limit Exceeded Response

**Status Code:** 429 Too Many Requests

**Example:**
```http
HTTP/1.1 429 Too Many Requests
Content-Type: application/problem+json
Retry-After: 60
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1700001234

{
  "type": "https://tools.ietf.org/html/rfc6585#section-4",
  "title": "Too Many Requests",
  "status": 429,
  "detail": "Rate limit exceeded. Maximum 1000 requests per hour allowed.",
  "retryAfter": 60
}
```

### 7.2. Retry-After Header

#### 7.2.1. Purpose and Usage

**Purpose:** Indicate when client should retry request.

**Used With:**
- 429 Too Many Requests
- 503 Service Unavailable
- 202 Accepted (async operations)

**Format Options:**

**Delay in Seconds:**
```http
Retry-After: 60
```

**HTTP-Date:**
```http
Retry-After: Wed, 15 Jan 2025 11:30:00 GMT
```

#### 7.2.2. Client Behavior

**Client Implementation:**
```typescript
async function fetchWithRetry(url: string, options: RequestInit) {
  const response = await fetch(url, options);
  
  if (response.status === 429) {
    const retryAfter = response.headers.get('Retry-After');
    const delaySeconds = parseInt(retryAfter || '60', 10);
    
    await delay(delaySeconds * 1000);
    return fetchWithRetry(url, options);
  }
  
  return response;
}
```

---

## 8. API Versioning and Deprecation Headers

### 8.1. API Version Headers

#### 8.1.1. API-Version Header

**Purpose:** Communicate API version in response.

**Example:**
```http
HTTP/1.1 200 OK
API-Version: 1.0
```

#### 8.1.2. Supported Versions Headers

**Purpose:** Communicate all supported and deprecated versions.

**Headers:**
- `api-supported-versions`: All currently supported versions
- `api-deprecated-versions`: Versions in deprecation period

**Example:**
```http
HTTP/1.1 200 OK
api-supported-versions: 1.0, 2.0
api-deprecated-versions: 1.0
```

### 8.2. Deprecation Headers

#### 8.2.1. Deprecation Header

**Purpose:** Indicate that API version or endpoint is deprecated.

**Format:** Boolean or date

**Examples:**

**Boolean:**
```http
Deprecation: true
```

**Date:**
```http
Deprecation: Sun, 01 Jan 2026 00:00:00 GMT
```

#### 8.2.2. Sunset Header

**Purpose:** Indicate when API version will be removed.

**Format:** HTTP-date

**Example:**
```http
HTTP/1.1 200 OK
Deprecation: true
Sunset: Sat, 01 Jun 2027 00:00:00 GMT
```

#### 8.2.3. Successor Version Link

**Purpose:** Point to replacement version or endpoint.

**Format:** Link header with rel="successor-version"

**Example:**
```http
HTTP/1.1 200 OK
Deprecation: true
Sunset: Sat, 01 Jun 2027 00:00:00 GMT
Link: </api/v2/products>; rel="successor-version"; title="Upgrade to V2"
```

**Complete Deprecation Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json
API-Version: 1.0
api-supported-versions: 1.0, 2.0
api-deprecated-versions: 1.0
Deprecation: Sun, 01 Jan 2026 00:00:00 GMT
Sunset: Sat, 01 Jun 2027 00:00:00 GMT
Link: </api/v2/products>; rel="successor-version"
Warning: 299 - "This API version is deprecated and will be removed on 2027-06-01"
```

**Reference:** See API-VERSIONING-001 for complete versioning specification.

---

## 9. Content Negotiation Headers

### 9.1. Content-Type Header

#### 9.1.1. Request Content-Type

**Purpose:** Specify format of request body.

**Common Values:**

| Content-Type | Use Case |
|--------------|----------|
| application/json | Standard API request body |
| application/xml | XML request body |
| multipart/form-data | File upload with metadata |
| application/x-www-form-urlencoded | Form submission |

**Example:**
```http
POST /api/v1/products HTTP/1.1
Content-Type: application/json

{"name": "Product X"}
```

#### 9.1.2. Response Content-Type

**Purpose:** Specify format of response body.

**Common Values:**

| Content-Type | Use Case |
|--------------|----------|
| application/json | Standard API response |
| application/problem+json | RFC 7807 error response |
| application/pdf | Document download |
| text/csv | Data export |

**Example:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{...}
```

### 9.2. Accept Header

#### 9.2.1. Purpose

**Purpose:** Client specifies desired response format.

**Example:**
```http
GET /api/v1/reports/report-123 HTTP/1.1
Accept: application/pdf
```

#### 9.2.2. Multiple Formats

**Quality Values:**
```http
Accept: application/json, application/xml;q=0.8, text/html;q=0.5
```

**Interpretation:**
- application/json: Priority 1.0 (default)
- application/xml: Priority 0.8
- text/html: Priority 0.5

#### 9.2.3. Unsupported Format Response

**Status Code:** 406 Not Acceptable

**Example:**
```http
HTTP/1.1 406 Not Acceptable
Content-Type: application/problem+json

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.6",
  "title": "Not Acceptable",
  "status": 406,
  "detail": "Requested format 'application/xml' is not supported. Supported formats: application/json, application/pdf"
}
```

---

## 10. Asynchronous Operation Headers

### 10.1. Location Header

#### 10.1.1. Purpose

**Purpose:** Provide URI of created resource or job status endpoint.

**Used With:**
- 201 Created (resource creation)
- 202 Accepted (async operation)
- 303 See Other (redirect)

**Examples:**

**201 Created:**
```http
HTTP/1.1 201 Created
Location: /api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
```

**202 Accepted:**
```http
HTTP/1.1 202 Accepted
Location: /api/v1/jobs/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91
Retry-After: 5
```

### 10.2. Async Operation Headers

#### 10.2.1. Job Submission Response

**Complete Header Set:**
```http
HTTP/1.1 202 Accepted
Location: /api/v1/jobs/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91
Retry-After: 5
Content-Type: application/json
X-Correlation-ID: 3f5ea98e-5f4c-4df2-b828-27e6844a7007

{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Queued",
  "statusUrl": "/api/v1/jobs/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91"
}
```

#### 10.2.2. Job Status Polling Response

**Running:**
```http
HTTP/1.1 202 Accepted
Retry-After: 5
X-Correlation-ID: 5c7f3a11-9e34-4a1b-a4b1-672e014f0008

{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Running",
  "progress": 45
}
```

**Completed:**
```http
HTTP/1.1 200 OK
X-Correlation-ID: 6d8g4b22-0f45-5b2c-b5c2-783f125g0009

{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Completed",
  "resultUrl": "/api/v1/reports/result/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91"
}
```

**Reference:** See API-ASYNC-001 for complete asynchronous operations specification.

---

## 11. Custom Header Conventions

### 11.1. Naming Conventions

#### 11.1.1. Prefix Rules

**Recommended Prefixes:**
- `X-`: Legacy convention for custom headers
- No prefix: Modern convention (if name descriptive)
- Application namespace: `MyApp-Feature-Name`

**Examples:**
```
X-Correlation-ID          (legacy but widely used)
X-RateLimit-Limit         (widely adopted standard)
Idempotency-Key           (modern, no prefix)
```

#### 11.1.2. Naming Style

**Rules:**
- Use PascalCase with hyphens: `X-Custom-Header`
- Avoid underscores: `X-Custom_Header` (incorrect)
- Keep names concise but descriptive
- Avoid abbreviations unless well-known

### 11.2. Custom Header Documentation

**Requirements:**
- Document purpose and format
- Specify directionality (request/response/both)
- Provide examples
- Indicate if optional or required
- Describe behavior when absent

---

## 12. Header Propagation Patterns

### 12.1. Downstream Service Calls

#### 12.1.1. Headers to Propagate

**Always Propagate:**
- X-Correlation-ID (or equivalent trace context)
- Authorization (if service-to-service auth required)
- Accept-Language (for localization)

**Consider Propagating:**
- Custom business context headers
- Tenant identifiers

**Never Propagate:**
- Caching headers (Cache-Control, ETag)
- Rate limit headers
- Content-Length, Transfer-Encoding

#### 12.1.2. Implementation Pattern

```csharp
public class DownstreamHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _contextAccessor;

    public async Task<HttpResponseMessage> CallDownstreamAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Propagate correlation ID
        var correlationId = _contextAccessor.HttpContext?
            .Request.Headers["X-Correlation-ID"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(correlationId))
        {
            request.Headers.Add("X-Correlation-ID", correlationId);
        }

        // Propagate authorization
        var authHeader = _contextAccessor.HttpContext?
            .Request.Headers["Authorization"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(authHeader))
        {
            request.Headers.Add("Authorization", authHeader);
        }

        return await _httpClient.SendAsync(request);
    }
}
```

### 12.2. Message Queue Integration

#### 12.2.1. Headers in Messages

**Include in Message Metadata:**
- Correlation ID
- Timestamp
- User context (if applicable)
- Tenant ID (multi-tenant systems)

**Example Message:**
```json
{
  "messageId": "msg-12345",
  "correlationId": "7f5a4b25-6804-4c9d-9e7d-2f3a9b41d001",
  "timestamp": "2025-01-15T10:30:00Z",
  "userId": "user-123",
  "payload": {...}
}
```

---

## 13. Testing Requirements

### 13.1. Header Presence Tests

#### 13.1.1. Response Header Verification

```csharp
[Fact]
public async Task GetProducts_ShouldReturnCorrelationIdHeader()
{
    // Arrange
    var client = _factory.CreateClient();
    var correlationId = Guid.NewGuid().ToString();
    client.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

    // Act
    var response = await client.GetAsync("/api/v1/products");

    // Assert
    Assert.True(response.Headers.Contains("X-Correlation-ID"));
    Assert.Equal(correlationId, 
        response.Headers.GetValues("X-Correlation-ID").First());
}
```

#### 13.1.2. Required Headers Test

```csharp
[Theory]
[InlineData("POST", "/api/v1/products", "Location")]
[InlineData("POST", "/api/v1/reports/generate", "Retry-After")]
public async Task Operation_ShouldReturnRequiredHeaders(
    string method,
    string url,
    string requiredHeader)
{
    // Test that required headers are present
}
```

### 13.2. Caching Behavior Tests

```csharp
[Fact]
public async Task GetSensitiveData_ShouldHaveNoStoreCache()
{
    // Arrange
    var response = await _client.GetAsync("/api/v1/users/profile");

    // Assert
    var cacheControl = response.Headers.CacheControl;
    Assert.True(cacheControl.NoStore);
    Assert.True(cacheControl.NoCache);
}
```

### 13.3. Rate Limiting Tests

```csharp
[Fact]
public async Task MultipleRequests_ShouldReturnRateLimitHeaders()
{
    // Make multiple requests and verify rate limit headers present
}
```

---

## 14. Security Considerations

### 14.1. Sensitive Header Protection

#### 14.1.1. Headers Never to Log

**Prohibited from Logs:**
- Authorization (contains credentials)
- Cookie (contains session tokens)
- Any header containing passwords or secrets

**Logging Redaction:**
```csharp
public class HeaderLoggingMiddleware
{
    private static readonly string[] SensitiveHeaders = 
    {
        "Authorization",
        "Cookie",
        "X-API-Key"
    };

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Request.Headers
            .Where(h => !SensitiveHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        _logger.LogInformation("Request headers: {@Headers}", headers);

        await _next(context);
    }
}
```

### 14.2. Header Injection Prevention

**Validation:**
- Validate header values before processing
- Reject headers with newline characters
- Sanitize user-controlled header values
- Enforce maximum header length

---

## 15. Glossary

**Bearer Token:** Authentication token carried in Authorization header using Bearer scheme.

**Conditional Request:** HTTP request with headers (If-Match, If-None-Match) controlling execution based on resource state.

**Correlation ID:** Unique identifier tracking a request across distributed systems.

**ETag:** Entity tag uniquely identifying a specific version of a resource.

**Idempotency:** Property where multiple identical requests have same effect as single request.

**Idempotency Key:** Client-provided unique identifier ensuring operation executed only once.

**Rate Limiting:** Controlling number of requests a client can make within time window.

**Retry-After:** Header indicating when client should retry request after rate limit or temporary unavailability.

**W3C Trace Context:** Standard format for distributed tracing context propagation (traceparent header).

---

## 16. Recommendations and Next Steps

### 16.1. For Development Teams

**Implementation Checklist:**
- [ ] Implement correlation ID middleware
- [ ] Add idempotency support for POST endpoints
- [ ] Configure appropriate caching headers
- [ ] Implement rate limiting with standard headers
- [ ] Add deprecation headers for old versions
- [ ] Propagate correlation ID to downstream services
- [ ] Write tests for header presence and behavior
- [ ] Document custom headers clearly

### 16.2. For Operations Teams

**Monitoring Requirements:**
- Track correlation IDs in centralized logging
- Monitor rate limit hit rates
- Alert on deprecated version usage spikes
- Verify header propagation in distributed traces

### 16.3. For Security Teams

**Security Review:**
- Verify Authorization header validation
- Ensure sensitive headers not logged
- Review custom header security implications
- Validate header injection prevention

### 16.4. Related Documentation

**Must Read:**
- API Guidelines (API-GUIDELINES-001)
- Error Handling Specification (API-ERRORHANDLING-001)
- Versioning Strategy (API-VERSIONING-001)
- Asynchronous Operations (API-ASYNC-001)

---

## 17. References

### 17.1. HTTP Standards

**HTTP/1.1 Specification**
- RFC 7230-7235: https://tools.ietf.org/html/rfc7230

**HTTP Caching**
- RFC 7234: https://tools.ietf.org/html/rfc7234

**Bearer Token Usage**
- RFC 6750: https://tools.ietf.org/html/rfc6750

**Rate Limiting**
- RFC 6585: https://tools.ietf.org/html/rfc6585

### 17.2. Standards Organizations

**W3C Trace Context**
- https://www.w3.org/TR/trace-context/

**OpenTelemetry**
- https://opentelemetry.io/

### 17.3. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- API-GUIDELINES-001: API Design Guidelines
- API-ERRORHANDLING-001: Error Handling and Problem Details
- API-AUTH-001: Authentication and Authorization
- API-VERSIONING-001: API Versioning Strategy
- API-ASYNC-001: Asynchronous Operations

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial HTTP headers specification with comprehensive header categories and implementation guidelines |

---

**END OF DOCUMENT**