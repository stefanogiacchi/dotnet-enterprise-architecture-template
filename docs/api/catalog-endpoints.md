# Catalog API Endpoints Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | API-CATALOG-001 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | API Specification |
| Target Audience | API Developers, Frontend Teams, Integration Developers, QA Engineers, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Quarterly |
| Related Documents | High-Level Architecture (ARCH-HL-001), API Flow (ARCH-API-003), Domain Modeling (ARCH-DOMAIN-004) |
| Prerequisites | Understanding of RESTful APIs, HTTP protocol, JSON format |

---

## Executive Summary

This document provides comprehensive API endpoint specifications for the Catalog module within the .NET Enterprise Architecture Template. The Catalog module implements a reference implementation demonstrating complete CRUD (Create, Read, Update, Delete) operations for product management, following RESTful design principles and enterprise API standards. This implementation serves as both a functional module and a pattern reference for developing additional API modules.

**Strategic Business Value:**
- Accelerated API development through proven endpoint patterns
- Consistent client experience through standardized response formats
- Reduced integration costs via comprehensive documentation
- Enhanced maintainability through clear endpoint-to-CQRS mappings
- Simplified testing through well-defined contracts

**Key Technical Capabilities:**
- Complete RESTful product resource management
- Paginated collection retrieval with filtering and sorting
- Standardized error handling using RFC 7807 Problem Details
- Direct mapping to CQRS commands and queries
- Comprehensive validation with structured error responses
- Production-ready security considerations

**Compliance and Standards:**
- REST architectural constraints (RFC 7230-7235)
- RFC 7807 Problem Details for HTTP APIs
- ISO 8601 date/time formatting
- HTTP/1.1 specification compliance
- OpenAPI 3.0 specification compatible

---

## Table of Contents

1. Introduction and Scope
2. API Base Configuration
3. Product Resource Specification
4. Endpoint Specifications
5. CQRS Command and Query Mapping
6. Security and Authorization Requirements
7. Error Handling Specifications
8. Testing Requirements
9. Performance Characteristics
10. Glossary
11. Recommendations and Next Steps
12. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document specifies the API endpoints for the Catalog module, providing detailed technical specifications for HTTP operations, request/response formats, validation rules, error handling, and integration with the underlying CQRS architecture. It serves as the definitive reference for implementing and consuming the Catalog API.

### 1.2. Scope

**In Scope:**
- HTTP endpoint specifications (methods, paths, parameters)
- Request and response payload formats
- Validation rules and error responses
- Query parameter specifications for filtering and pagination
- HTTP status code usage
- CQRS command and query mappings
- Security requirements

**Out of Scope:**
- Implementation details of CQRS handlers (covered in ARCH-CQRS-002)
- Domain model specifications (covered in ARCH-DOMAIN-004)
- Authentication mechanism implementations
- Infrastructure configuration details
- Database schema design
- Rate limiting configurations

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**API Developers:** Complete endpoint implementation specifications

**Frontend Developers:** Request/response formats and integration patterns

**QA Engineers:** Test scenarios, validation rules, error conditions

**Integration Developers:** Contract specifications for system integration

**Technical Writers:** API documentation source material

**C-Level Executives:** Business value and operational capabilities

### 1.4. Catalog Module Overview

The Catalog module provides product management capabilities through RESTful API endpoints. It demonstrates:

**Standard CRUD Operations:**
- Create new products
- Retrieve product lists and individual products
- Update existing products (full and partial)
- Delete products

**Advanced Query Capabilities:**
- Pagination support
- Free-text search
- Sorting options
- Filtering capabilities

**Enterprise Features:**
- Validation with structured error responses
- Consistent error handling
- Correlation ID tracking
- Comprehensive logging

---

## 2. API Base Configuration

### 2.1. Base URL Structure

#### 2.1.1. API Base Path

```
https://{hostname}/api/v1/catalog
```

**Components:**
- `{hostname}`: Environment-specific host (e.g., api.example.com)
- `api`: API namespace prefix
- `v1`: Major version number
- `catalog`: Module identifier

#### 2.1.2. Resource Path

```
/api/v1/catalog/products
```

**Full Resource URL:**
```
https://api.example.com/api/v1/catalog/products
```

### 2.2. HTTP Protocol Requirements

#### 2.2.1. Protocol Version

**Required:** HTTP/1.1 or HTTP/2

**HTTPS Required:** All production endpoints must use HTTPS

#### 2.2.2. Standard Headers

**Request Headers:**

| Header | Required | Description |
|--------|----------|-------------|
| Accept | Yes | Must include `application/json` |
| Content-Type | Yes (for POST/PUT/PATCH) | Must be `application/json` |
| Authorization | Yes (production) | Bearer token or configured auth scheme |
| X-Correlation-ID | No | Client-provided correlation ID |

**Response Headers:**

| Header | Always Present | Description |
|--------|----------------|-------------|
| Content-Type | Yes | `application/json; charset=utf-8` |
| X-Correlation-ID | Yes | Request correlation identifier |
| Location | For 201 responses | URI of created resource |

### 2.3. Content Negotiation

#### 2.3.1. Supported Media Types

**Request Body:**
- `application/json` (required)

**Response Body:**
- `application/json` (default)

**Future Considerations:**
- `application/xml` (if required)
- `application/hal+json` (for hypermedia)

---

## 3. Product Resource Specification

### 3.1. Resource Representation

#### 3.1.1. Product Resource Schema

```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Sample Product",
  "description": "Optional product description",
  "price": 99.90,
  "currency": "EUR",
  "isActive": true,
  "createdAtUtc": "2025-01-15T10:30:00Z",
  "updatedAtUtc": "2025-01-16T09:00:00Z"
}
```

#### 3.1.2. Field Specifications

| Field | Type | Required | Constraints | Description |
|-------|------|----------|-------------|-------------|
| id | string (GUID) | Yes | Valid GUID format | Unique identifier of the product |
| name | string | Yes | 1-200 characters | Product name |
| description | string | No | 0-2000 characters | Optional product description |
| price | decimal | Yes | > 0, max 2 decimal places | Unit price |
| currency | string | Yes | 3-letter ISO 4217 code | Currency code (e.g., EUR, USD, GBP) |
| isActive | boolean | Yes | true or false | Active/visible status flag |
| createdAtUtc | string (ISO 8601) | Yes | Valid ISO 8601 datetime | Creation timestamp in UTC |
| updatedAtUtc | string (ISO 8601) | Yes | Valid ISO 8601 datetime | Last update timestamp in UTC |

#### 3.1.3. Field Validation Rules

**Name Field:**
- Minimum length: 1 character
- Maximum length: 200 characters
- Must not be empty or whitespace only
- Trimmed before validation

**Description Field:**
- Maximum length: 2000 characters
- Nullable/optional
- Trimmed before storage

**Price Field:**
- Must be greater than zero
- Maximum value: 999999999.99
- Precision: 2 decimal places
- Must be a valid decimal number

**Currency Field:**
- Must be exactly 3 characters
- Must be uppercase
- Must be valid ISO 4217 currency code
- Common values: EUR, USD, GBP, JPY, CHF

**IsActive Field:**
- Boolean value only
- Default value on creation: true

### 3.2. Paginated Collection Response

#### 3.2.1. Collection Schema

```json
{
  "items": [
    {
      "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
      "name": "Product 1",
      "description": "Description",
      "price": 99.90,
      "currency": "EUR",
      "isActive": true,
      "createdAtUtc": "2025-01-15T10:30:00Z",
      "updatedAtUtc": "2025-01-16T09:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalItems": 42,
  "totalPages": 3
}
```

#### 3.2.2. Pagination Metadata

| Field | Type | Description |
|-------|------|-------------|
| items | array | Array of product resources |
| page | integer | Current page number (1-based) |
| pageSize | integer | Number of items per page |
| totalItems | integer | Total number of items across all pages |
| totalPages | integer | Total number of pages (calculated) |

**Calculation:**
```
totalPages = ceiling(totalItems / pageSize)
```

---

## 4. Endpoint Specifications

### 4.1. List Products (GET Collection)

#### 4.1.1. Endpoint Definition

**HTTP Method:** GET

**Path:** `/api/v1/catalog/products`

**Purpose:** Retrieve a paginated list of products with optional filtering and sorting.

#### 4.1.2. Request Specification

**HTTP Request Example:**

```http
GET /api/v1/catalog/products?page=1&pageSize=20&search=phone&sort=price_desc HTTP/1.1
Host: api.example.com
Accept: application/json
Authorization: Bearer {token}
```

**Query Parameters:**

| Parameter | Type | Required | Default | Constraints | Description |
|-----------|------|----------|---------|-------------|-------------|
| page | integer | No | 1 | >= 1, <= 1000 | Page index (1-based) |
| pageSize | integer | No | 20 | 1-100 | Number of items per page |
| search | string | No | null | 0-100 characters | Free-text search on name and description |
| sort | string | No | null | Predefined values | Sort order specification |

**Valid Sort Values:**

| Value | Description |
|-------|-------------|
| name_asc | Sort by name ascending (A-Z) |
| name_desc | Sort by name descending (Z-A) |
| price_asc | Sort by price ascending (lowest first) |
| price_desc | Sort by price descending (highest first) |
| created_asc | Sort by creation date ascending (oldest first) |
| created_desc | Sort by creation date descending (newest first) |

**Default Sort Order:** created_desc (newest products first)

#### 4.1.3. Response Specifications

**Success Response (200 OK):**

```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "items": [
    {
      "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
      "name": "Phone X",
      "description": "128 GB, black",
      "price": 799.00,
      "currency": "EUR",
      "isActive": true,
      "createdAtUtc": "2025-01-15T10:30:00Z",
      "updatedAtUtc": "2025-01-16T09:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalItems": 42,
  "totalPages": 3
}
```

**Error Response (400 Bad Request):**

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-550e8400e29b41d4a716446655440000-01",
  "errors": {
    "page": ["Page must be greater than 0"],
    "pageSize": ["Page size must be between 1 and 100"]
  }
}
```

#### 4.1.4. CQRS Mapping

**Command/Query:** GetProductsQuery

**Handler:** GetProductsQueryHandler

**Location:** Application.Queries.Products

### 4.2. Get Product by ID (GET Single Resource)

#### 4.2.1. Endpoint Definition

**HTTP Method:** GET

**Path:** `/api/v1/catalog/products/{id}`

**Purpose:** Retrieve a single product by its unique identifier.

#### 4.2.2. Request Specification

**HTTP Request Example:**

```http
GET /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Accept: application/json
Authorization: Bearer {token}
```

**Path Parameters:**

| Parameter | Type | Required | Constraints | Description |
|-----------|------|----------|-------------|-------------|
| id | string (GUID) | Yes | Valid GUID format | Product unique identifier |

#### 4.2.3. Response Specifications

**Success Response (200 OK):**

```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Phone X",
  "description": "128 GB, black",
  "price": 799.00,
  "currency": "EUR",
  "isActive": true,
  "createdAtUtc": "2025-01-15T10:30:00Z",
  "updatedAtUtc": "2025-01-16T09:00:00Z"
}
```

**Error Response (404 Not Found):**

```http
HTTP/1.1 404 Not Found
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource not found.",
  "status": 404,
  "detail": "The requested product was not found.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

**Error Response (400 Bad Request - Invalid GUID):**

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Invalid request.",
  "status": 400,
  "detail": "The provided ID is not a valid GUID format.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

#### 4.2.4. CQRS Mapping

**Command/Query:** GetProductByIdQuery

**Handler:** GetProductByIdQueryHandler

**Location:** Application.Queries.Products

### 4.3. Create Product (POST)

#### 4.3.1. Endpoint Definition

**HTTP Method:** POST

**Path:** `/api/v1/catalog/products`

**Purpose:** Create a new product in the catalog.

#### 4.3.2. Request Specification

**HTTP Request Example:**

```http
POST /api/v1/catalog/products HTTP/1.1
Host: api.example.com
Content-Type: application/json
Accept: application/json
Authorization: Bearer {token}
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

**Request Body Schema:**

| Field | Type | Required | Constraints | Description |
|-------|------|----------|-------------|-------------|
| name | string | Yes | 1-200 characters | Product name |
| description | string | No | 0-2000 characters | Optional description |
| price | decimal | Yes | > 0, max 2 decimals | Unit price |
| currency | string | Yes | 3-letter ISO 4217 | Currency code |

#### 4.3.3. Response Specifications

**Success Response (201 Created):**

```http
HTTP/1.1 201 Created
Location: /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Phone X",
  "description": "128 GB, black",
  "price": 799.00,
  "currency": "EUR",
  "isActive": true,
  "createdAtUtc": "2025-01-15T10:30:00Z",
  "updatedAtUtc": "2025-01-15T10:30:00Z"
}
```

**Error Response (400 Bad Request - Validation):**

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-550e8400e29b41d4a716446655440000-01",
  "errors": {
    "name": ["Product name is required."],
    "price": ["Price must be greater than zero."],
    "currency": ["Currency must be a valid 3-letter ISO code."]
  }
}
```

**Error Response (409 Conflict - Business Rule):**

```http
HTTP/1.1 409 Conflict
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Business rule violation.",
  "status": 409,
  "detail": "A product with this name already exists.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

#### 4.3.4. CQRS Mapping

**Command/Query:** CreateProductCommand

**Handler:** CreateProductCommandHandler

**Location:** Application.Commands.Products

**Validation:** CreateProductCommandValidator

### 4.4. Update Product (PUT)

#### 4.4.1. Endpoint Definition

**HTTP Method:** PUT

**Path:** `/api/v1/catalog/products/{id}`

**Purpose:** Replace an existing product with new data (full update).

#### 4.4.2. Request Specification

**HTTP Request Example:**

```http
PUT /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Content-Type: application/json
Accept: application/json
Authorization: Bearer {token}
```

**Path Parameters:**

| Parameter | Type | Required | Constraints | Description |
|-----------|------|----------|-------------|-------------|
| id | string (GUID) | Yes | Valid GUID format | Product unique identifier |

**Request Body:**

```json
{
  "name": "Phone X Pro",
  "description": "256 GB, black",
  "price": 899.00,
  "currency": "EUR",
  "isActive": true
}
```

**Request Body Schema:**

| Field | Type | Required | Constraints | Description |
|-------|------|----------|-------------|-------------|
| name | string | Yes | 1-200 characters | Product name |
| description | string | No | 0-2000 characters | Optional description |
| price | decimal | Yes | > 0, max 2 decimals | Unit price |
| currency | string | Yes | 3-letter ISO 4217 | Currency code |
| isActive | boolean | Yes | true or false | Active status flag |

#### 4.4.3. Response Specifications

**Success Response (200 OK):**

```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Phone X Pro",
  "description": "256 GB, black",
  "price": 899.00,
  "currency": "EUR",
  "isActive": true,
  "createdAtUtc": "2025-01-15T10:30:00Z",
  "updatedAtUtc": "2025-01-16T09:00:00Z"
}
```

**Alternative Success Response (204 No Content):**

```http
HTTP/1.1 204 No Content
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

**Note:** API may return either 200 OK with body or 204 No Content based on configuration.

**Error Response (404 Not Found):**

```http
HTTP/1.1 404 Not Found
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource not found.",
  "status": 404,
  "detail": "The product to update was not found.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

**Error Response (400 Bad Request):**

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-550e8400e29b41d4a716446655440000-01",
  "errors": {
    "price": ["Price must be greater than zero."]
  }
}
```

#### 4.4.4. CQRS Mapping

**Command/Query:** UpdateProductCommand

**Handler:** UpdateProductCommandHandler

**Location:** Application.Commands.Products

**Validation:** UpdateProductCommandValidator

### 4.5. Partial Update Product (PATCH)

#### 4.5.1. Endpoint Definition

**HTTP Method:** PATCH

**Path:** `/api/v1/catalog/products/{id}`

**Purpose:** Partially update specific product fields without replacing the entire resource.

**Implementation Note:** This implementation uses a simple partial DTO approach. JSON Patch (RFC 6902) is an alternative approach for more complex scenarios.

#### 4.5.2. Request Specification

**HTTP Request Example:**

```http
PATCH /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Content-Type: application/json
Accept: application/json
Authorization: Bearer {token}
```

**Path Parameters:**

| Parameter | Type | Required | Constraints | Description |
|-----------|------|----------|-------------|-------------|
| id | string (GUID) | Yes | Valid GUID format | Product unique identifier |

**Request Body (Partial Update):**

```json
{
  "price": 749.00,
  "currency": "EUR"
}
```

**Request Body Schema:**

All fields are optional. Only provided fields will be updated.

| Field | Type | Required | Constraints | Description |
|-------|------|----------|-------------|-------------|
| name | string | No | 1-200 characters | Product name |
| description | string | No | 0-2000 characters | Optional description |
| price | decimal | No | > 0, max 2 decimals | Unit price |
| currency | string | No | 3-letter ISO 4217 | Currency code |
| isActive | boolean | No | true or false | Active status flag |

**Business Rule:** If price is updated, currency must also be provided to maintain consistency.

#### 4.5.3. Response Specifications

**Success Response (200 OK):**

```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "id": "b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "name": "Phone X",
  "description": "128 GB, black",
  "price": 749.00,
  "currency": "EUR",
  "isActive": true,
  "createdAtUtc": "2025-01-15T10:30:00Z",
  "updatedAtUtc": "2025-01-16T10:00:00Z"
}
```

**Error Response (404 Not Found):**

Same format as PUT operation.

**Error Response (400 Bad Request):**

Same format as PUT operation with field-specific validation errors.

#### 4.5.4. CQRS Mapping

**Command/Query:** PartialUpdateProductCommand

**Handler:** PartialUpdateProductCommandHandler

**Location:** Application.Commands.Products

**Validation:** PartialUpdateProductCommandValidator

### 4.6. Delete Product (DELETE)

#### 4.6.1. Endpoint Definition

**HTTP Method:** DELETE

**Path:** `/api/v1/catalog/products/{id}`

**Purpose:** Delete a product from the catalog (hard delete or soft delete based on implementation).

**Implementation Note:** The template may implement either:
- Hard delete: Physical removal from database
- Soft delete: Marking as deleted (isDeleted flag) while preserving data

#### 4.6.2. Request Specification

**HTTP Request Example:**

```http
DELETE /api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1 HTTP/1.1
Host: api.example.com
Accept: application/json
Authorization: Bearer {token}
```

**Path Parameters:**

| Parameter | Type | Required | Constraints | Description |
|-----------|------|----------|-------------|-------------|
| id | string (GUID) | Yes | Valid GUID format | Product unique identifier |

#### 4.6.3. Response Specifications

**Success Response (204 No Content):**

```http
HTTP/1.1 204 No Content
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

**Error Response (404 Not Found):**

```http
HTTP/1.1 404 Not Found
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource not found.",
  "status": 404,
  "detail": "The product to delete was not found.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

**Error Response (409 Conflict - Business Rule):**

```http
HTTP/1.1 409 Conflict
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Business rule violation.",
  "status": 409,
  "detail": "Cannot delete product that has active orders.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

#### 4.6.4. CQRS Mapping

**Command/Query:** DeleteProductCommand

**Handler:** DeleteProductCommandHandler

**Location:** Application.Commands.Products

**Validation:** DeleteProductCommandValidator

---

## 5. CQRS Command and Query Mapping

### 5.1. Endpoint to CQRS Mapping Matrix

#### 5.1.1. Complete Mapping Table

| HTTP Method | Endpoint | CQRS Type | Command/Query Name | Handler Name |
|-------------|----------|-----------|-------------------|--------------|
| GET | /products | Query | GetProductsQuery | GetProductsQueryHandler |
| GET | /products/{id} | Query | GetProductByIdQuery | GetProductByIdQueryHandler |
| POST | /products | Command | CreateProductCommand | CreateProductCommandHandler |
| PUT | /products/{id} | Command | UpdateProductCommand | UpdateProductCommandHandler |
| PATCH | /products/{id} | Command | PartialUpdateProductCommand | PartialUpdateProductCommandHandler |
| DELETE | /products/{id} | Command | DeleteProductCommand | DeleteProductCommandHandler |

### 5.2. Architecture Flow Diagram

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer                            │
│                                                         │
│  ┌──────────────────────────────────────────────────┐  │
│  │  GET /products                                   │  │
│  │  GET /products/{id}                              │  │
│  │  POST /products                                  │  │
│  │  PUT /products/{id}                              │  │
│  │  PATCH /products/{id}                            │  │
│  │  DELETE /products/{id}                           │  │
│  └────────────┬─────────────────────────────────────┘  │
└───────────────┼─────────────────────────────────────────┘
                │
                ↓ IMediator.Send()
┌───────────────┴─────────────────────────────────────────┐
│              Application Layer (CQRS)                   │
│                                                         │
│  ┌─────────────────────┐    ┌──────────────────────┐   │
│  │   Queries           │    │   Commands           │   │
│  │                     │    │                      │   │
│  │ GetProductsQuery    │    │ CreateProductCommand │   │
│  │ GetProductByIdQuery │    │ UpdateProductCommand │   │
│  │                     │    │ PartialUpdateProduct │   │
│  │                     │    │   Command            │   │
│  │                     │    │ DeleteProductCommand │   │
│  └──────────┬──────────┘    └──────────┬───────────┘   │
│             │                           │               │
│             ↓                           ↓               │
│  ┌─────────────────────┐    ┌──────────────────────┐   │
│  │   Query Handlers    │    │   Command Handlers   │   │
│  │                     │    │                      │   │
│  │ GetProductsQuery    │    │ CreateProduct        │   │
│  │   Handler           │    │   CommandHandler     │   │
│  │ GetProductById      │    │ UpdateProduct        │   │
│  │   QueryHandler      │    │   CommandHandler     │   │
│  │                     │    │ PartialUpdateProduct │   │
│  │                     │    │   CommandHandler     │   │
│  │                     │    │ DeleteProduct        │   │
│  │                     │    │   CommandHandler     │   │
│  └──────────┬──────────┘    └──────────┬───────────┘   │
└─────────────┼──────────────────────────┼───────────────┘
              │                          │
              ↓                          ↓
┌─────────────┴──────────────────────────┴───────────────┐
│              Domain & Infrastructure                    │
│                                                         │
│  - Product Entity (Domain)                              │
│  - Product Repository (Infrastructure)                  │
│  - Database Context (Infrastructure)                    │
└─────────────────────────────────────────────────────────┘
```

### 5.3. Request Flow Examples

#### 5.3.1. Query Flow (GET /products)

```
1. Client sends HTTP GET request
   ↓
2. API Controller receives request
   ↓
3. Controller creates GetProductsQuery with parameters
   ↓
4. Controller sends query to MediatR
   ↓
5. MediatR pipeline behaviors execute (logging, validation)
   ↓
6. GetProductsQueryHandler processes query
   ↓
7. Handler retrieves data via repository (Dapper for performance)
   ↓
8. Handler maps domain entities to DTOs
   ↓
9. Handler returns paginated result
   ↓
10. Controller returns HTTP 200 OK with JSON response
```

#### 5.3.2. Command Flow (POST /products)

```
1. Client sends HTTP POST request with JSON body
   ↓
2. API Controller receives request
   ↓
3. Controller creates CreateProductCommand from request
   ↓
4. Controller sends command to MediatR
   ↓
5. MediatR pipeline behaviors execute:
   - ValidationBehavior validates command
   - LoggingBehavior logs operation
   - PerformanceBehavior monitors duration
   - TransactionBehavior manages database transaction
   ↓
6. CreateProductCommandHandler processes command
   ↓
7. Handler creates Product entity (domain)
   ↓
8. Handler persists via repository (EF Core)
   ↓
9. Domain events published (if any)
   ↓
10. Handler maps entity to DTO
   ↓
11. Controller returns HTTP 201 Created with Location header
```

---

## 6. Security and Authorization Requirements

### 6.1. Authentication Requirements

#### 6.1.1. Authentication Scheme

**Production Requirement:** All endpoints must be authenticated.

**Supported Schemes:**
- Bearer Token (JWT) - Primary
- API Key - Alternative
- OAuth 2.0 - For third-party integrations

**Development/Testing:** Authentication may be disabled for local development.

#### 6.1.2. Authentication Header

```http
Authorization: Bearer {jwt_token}
```

**Token Validation:**
- Token must be valid and not expired
- Token signature must be verified
- Token must contain required claims

**Failure Response (401 Unauthorized):**

```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication credentials were not provided or are invalid.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

### 6.2. Authorization Requirements

#### 6.2.1. Operation-Level Authorization

| Operation | HTTP Method | Required Permission | Notes |
|-----------|-------------|-------------------|-------|
| List Products | GET | Catalog.Read | All authenticated users |
| Get Product | GET | Catalog.Read | All authenticated users |
| Create Product | POST | Catalog.Write | Admin or Editor role |
| Update Product | PUT | Catalog.Write | Admin or Editor role |
| Partial Update | PATCH | Catalog.Write | Admin or Editor role |
| Delete Product | DELETE | Catalog.Delete | Admin role only |

#### 6.2.2. Role-Based Access Control

**Recommended Roles:**

**Catalog.Reader:**
- Can list and view products
- Read-only access
- Suitable for general users

**Catalog.Editor:**
- All Reader permissions
- Can create and update products
- Cannot delete products

**Catalog.Admin:**
- All Editor permissions
- Can delete products
- Can manage product visibility

#### 6.2.3. Policy-Based Authorization

**Alternative Approach:** Use policy-based authorization for fine-grained control.

**Example Policies:**
- CanViewProducts
- CanManageProducts
- CanDeleteProducts

**Failure Response (403 Forbidden):**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have permission to perform this action.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

### 6.3. Data Security Considerations

#### 6.3.1. Sensitive Data Handling

**Fields to Protect:**
- Internal cost information (not exposed in API)
- Supplier details (not exposed in API)
- Internal notes (not exposed in API)

**Recommendations:**
- Use separate DTOs for internal vs. external representation
- Implement field-level authorization if needed
- Audit access to sensitive operations

#### 6.3.2. Input Validation Security

**Protection Against:**
- SQL Injection: Parameterized queries, ORM usage
- XSS: Output encoding (automatic in JSON serialization)
- Mass Assignment: Explicit DTO mapping, no direct entity binding

---

## 7. Error Handling Specifications

### 7.1. Standard Error Response Format

#### 7.1.1. RFC 7807 Problem Details

All error responses follow RFC 7807 Problem Details specification.

**Standard Error Schema:**

```json
{
  "type": "string (URI)",
  "title": "string",
  "status": "integer",
  "detail": "string",
  "instance": "string (URI)",
  "traceId": "string"
}
```

#### 7.1.2. Validation Error Extension

Validation errors include an additional `errors` object:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-550e8400e29b41d4a716446655440000-01",
  "errors": {
    "fieldName": ["Error message 1", "Error message 2"]
  }
}
```

### 7.2. HTTP Status Code Usage

#### 7.2.1. Success Status Codes

| Status Code | Meaning | Usage |
|-------------|---------|-------|
| 200 OK | Success with response body | GET, PUT, PATCH with body |
| 201 Created | Resource created | POST successful creation |
| 204 No Content | Success without response body | DELETE, PUT without body |

#### 7.2.2. Client Error Status Codes

| Status Code | Meaning | Usage |
|-------------|---------|-------|
| 400 Bad Request | Invalid request format or validation failure | Malformed JSON, validation errors |
| 401 Unauthorized | Authentication required | Missing or invalid credentials |
| 403 Forbidden | Insufficient permissions | Authenticated but not authorized |
| 404 Not Found | Resource not found | Invalid product ID |
| 409 Conflict | Business rule violation | Duplicate name, constraint violation |
| 422 Unprocessable Entity | Semantic validation failure | Business logic validation errors |

#### 7.2.3. Server Error Status Codes

| Status Code | Meaning | Usage |
|-------------|---------|-------|
| 500 Internal Server Error | Unexpected server error | Unhandled exceptions |
| 503 Service Unavailable | Service temporarily unavailable | Database connection failure, maintenance |

### 7.3. Error Scenarios and Responses

#### 7.3.1. Validation Error Example

**Scenario:** Client submits product with invalid data.

**Response:**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-550e8400e29b41d4a716446655440000-01",
  "errors": {
    "name": ["Product name is required.", "Product name must not exceed 200 characters."],
    "price": ["Price must be greater than zero."],
    "currency": ["Currency must be a valid 3-letter ISO code."]
  }
}
```

#### 7.3.2. Business Rule Violation Example

**Scenario:** Client attempts to create product with duplicate name.

**Response:**

```json
{
  "type": "https://api.example.com/problems/duplicate-product-name",
  "title": "Duplicate product name",
  "status": 409,
  "detail": "A product with the name 'Phone X' already exists in the catalog.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01",
  "existingProductId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

#### 7.3.3. Not Found Example

**Scenario:** Client requests product that doesn't exist.

**Response:**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource not found.",
  "status": 404,
  "detail": "The requested product with ID 'b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1' was not found.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

---

## 8. Testing Requirements

### 8.1. Unit Testing

#### 8.1.1. Controller Unit Tests

**Test Scope:** API Controller logic in isolation.

**Test Categories:**

**Request Mapping Tests:**
- Verify request models map correctly to commands/queries
- Test parameter binding from route, query, and body
- Validate null handling

**Response Mapping Tests:**
- Verify DTOs map correctly to response models
- Test status code selection logic
- Validate Location header generation

**Error Handling Tests:**
- Test exception transformation to Problem Details
- Verify status code mapping
- Validate error response format

**Example Test:**

```csharp
[Fact]
public async Task GetById_ProductExists_Returns200WithProduct()
{
    // Arrange
    var productId = Guid.NewGuid();
    var productDto = new ProductDto { Id = productId, Name = "Test" };
    
    _mediatorMock
        .Setup(m => m.Send(
            It.Is<GetProductByIdQuery>(q => q.Id == productId),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(productDto);
    
    // Act
    var result = await _controller.GetById(productId, CancellationToken.None);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    Assert.Equal(200, okResult.StatusCode);
    var response = Assert.IsType<ProductResponse>(okResult.Value);
    Assert.Equal(productId, response.Id);
}
```

### 8.2. Integration Testing

#### 8.2.1. End-to-End API Tests

**Test Scope:** Complete request flow from HTTP to database.

**Test Infrastructure:**
- WebApplicationFactory for API hosting
- Test containers or in-memory database
- Real HTTP requests

**Test Categories:**

**Happy Path Tests:**
- Create product successfully
- Retrieve product successfully
- Update product successfully
- Delete product successfully
- List products with pagination

**Validation Tests:**
- Submit invalid product data
- Verify validation error responses
- Test boundary conditions

**Error Scenario Tests:**
- Request non-existent product
- Attempt duplicate product creation
- Test unauthorized access
- Test forbidden operations

**Example Test:**

```csharp
[Fact]
public async Task CreateProduct_ValidRequest_Returns201Created()
{
    // Arrange
    var request = new CreateProductRequest
    {
        Name = "Test Product",
        Description = "Description",
        Price = 99.99m,
        Currency = "EUR"
    };
    
    var content = new StringContent(
        JsonSerializer.Serialize(request),
        Encoding.UTF8,
        "application/json");
    
    // Act
    var response = await _client.PostAsync("/api/v1/catalog/products", content);
    
    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.NotNull(response.Headers.Location);
    
    var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
    Assert.NotNull(product);
    Assert.Equal(request.Name, product.Name);
    Assert.Equal(request.Price, product.Price);
}
```

### 8.3. Contract Testing

#### 8.3.1. OpenAPI Specification Validation

**Purpose:** Ensure API implementation matches OpenAPI specification.

**Tools:**
- Swagger/OpenAPI validation tools
- Contract testing frameworks (Pact, Spring Cloud Contract)

**Validation Points:**
- Request schema validation
- Response schema validation
- Status code verification
- Header verification

### 8.4. Performance Testing

#### 8.4.1. Load Testing

**Test Scenarios:**
- Sustained load: 100 requests/second for 10 minutes
- Peak load: 500 requests/second for 1 minute
- Spike test: Sudden increase from 50 to 500 requests/second

**Performance Targets:**
- p95 response time < 200ms for GET requests
- p95 response time < 500ms for POST/PUT requests
- Error rate < 0.1% under normal load

**Tools:**
- k6, JMeter, or Gatling for load testing
- Application Performance Monitoring (APM) for metrics

---

## 9. Performance Characteristics

### 9.1. Response Time Targets

#### 9.1.1. Target Latencies

| Operation | Target p50 | Target p95 | Target p99 |
|-----------|-----------|-----------|-----------|
| GET /products | < 50ms | < 100ms | < 200ms |
| GET /products/{id} | < 20ms | < 50ms | < 100ms |
| POST /products | < 100ms | < 200ms | < 500ms |
| PUT /products/{id} | < 100ms | < 200ms | < 500ms |
| PATCH /products/{id} | < 100ms | < 200ms | < 500ms |
| DELETE /products/{id} | < 50ms | < 100ms | < 200ms |

#### 9.1.2. Throughput Targets

**Target Capacity:**
- Minimum: 1000 requests/second per instance
- Sustained: 500 requests/second per instance
- Peak: 2000 requests/second per instance (short duration)

### 9.2. Optimization Strategies

#### 9.2.1. Query Optimization

**List Products (GET /products):**
- Use Dapper for high-performance reads
- Implement database indexing on searchable fields
- Apply pagination to limit result sets
- Cache frequent queries (optional)

**Get Product (GET /products/{id}):**
- Use Dapper for optimal performance
- Implement caching for frequently accessed products
- Database index on ID field (primary key)

#### 9.2.2. Command Optimization

**Create/Update Operations:**
- Use Entity Framework Core for transactional integrity
- Batch operations where applicable
- Optimize database writes with proper indexing

#### 9.2.3. Caching Strategy

**Cache Candidates:**
- Individual product details (short TTL)
- Product list results (very short TTL due to frequent changes)

**Cache Invalidation:**
- Invalidate on product updates
- Invalidate on product deletions
- Time-based expiration as fallback

---

## 10. Glossary

**API (Application Programming Interface):** A set of protocols and tools for building and integrating software applications.

**CQRS (Command Query Responsibility Segregation):** A pattern separating read operations (queries) from write operations (commands).

**DTO (Data Transfer Object):** An object carrying data between API and application layers without business logic.

**GUID (Globally Unique Identifier):** A 128-bit unique identifier, typically displayed as 32 hexadecimal digits.

**HTTP Status Code:** A three-digit code indicating the status of an HTTP response.

**ISO 4217:** International standard for currency codes (e.g., EUR, USD, GBP).

**ISO 8601:** International standard for date and time representation.

**JSON (JavaScript Object Notation):** A lightweight data-interchange format.

**Pagination:** The process of dividing data into discrete pages for improved performance and usability.

**Problem Details:** RFC 7807 standardized format for HTTP API error responses.

**RESTful API:** An API following REST architectural constraints using HTTP methods semantically.

**RFC 7807:** IETF specification for Problem Details for HTTP APIs.

**Soft Delete:** A deletion strategy where records are marked as deleted rather than physically removed.

**Validation:** The process of ensuring data meets specified constraints and business rules.

---

## 11. Recommendations and Next Steps

### 11.1. For Development Teams

#### 11.1.1. Implementation Checklist

**When Implementing Catalog Endpoints:**
- [ ] Create CQRS commands and queries as specified
- [ ] Implement command and query handlers
- [ ] Create FluentValidation validators
- [ ] Implement ProductsController with all endpoints
- [ ] Add XML documentation comments
- [ ] Configure OpenAPI/Swagger documentation
- [ ] Write unit tests for controllers
- [ ] Write integration tests for complete flows
- [ ] Implement authorization policies
- [ ] Configure correlation ID middleware
- [ ] Test all error scenarios

**When Extending Catalog Module:**
- [ ] Follow established patterns for new endpoints
- [ ] Maintain consistency with existing endpoints
- [ ] Update OpenAPI documentation
- [ ] Add corresponding tests
- [ ] Update this specification document

#### 11.1.2. Code Quality Standards

**Controller Implementation:**
- Keep controllers thin (no business logic)
- Use proper HTTP status codes
- Include comprehensive XML documentation
- Add ProducesResponseType attributes
- Implement proper error handling

**CQRS Implementation:**
- One command/query per operation
- Clear naming conventions
- Comprehensive validation
- Proper logging
- Transaction management where needed

### 11.2. For Frontend Teams

#### 11.2.1. Integration Guidelines

**HTTP Client Configuration:**
- Set Accept header to `application/json`
- Include Authorization header with bearer token
- Include X-Correlation-ID for request tracking
- Handle all documented status codes

**Error Handling:**
- Parse Problem Details responses
- Display validation errors to users
- Log correlation IDs for support
- Implement retry logic for transient failures

**Pagination Implementation:**
- Use page and pageSize parameters
- Display totalPages information
- Implement next/previous navigation
- Handle empty result sets

#### 11.2.2. Testing Recommendations

**Mock API Responses:**
- Use documented response formats
- Test all status codes
- Simulate validation errors
- Test pagination edge cases

### 11.3. For QA Teams

#### 11.3.1. Test Scenarios

**Functional Testing:**
- Verify all CRUD operations
- Test pagination boundary conditions
- Validate search functionality
- Test sorting options
- Verify filtering accuracy

**Security Testing:**
- Test authentication requirements
- Verify authorization rules
- Test input validation thoroughly
- Attempt SQL injection
- Test XSS prevention

**Performance Testing:**
- Load test all endpoints
- Verify response time targets
- Test under concurrent load
- Measure database query performance

#### 11.3.2. Test Data Management

**Setup:**
- Create reusable test products
- Establish baseline data sets
- Document test data creation scripts

**Cleanup:**
- Remove test data after runs
- Reset database state between tests
- Maintain test data isolation

### 11.4. For Architects

#### 11.4.1. Architecture Governance

**Periodic Reviews:**
- API design consistency
- CQRS pattern adherence
- Error handling compliance
- Performance characteristics
- Security posture

**Documentation Maintenance:**
- Keep specification current
- Document architectural decisions
- Update OpenAPI specifications
- Maintain example collections

#### 11.4.2. Evolution Planning

**API Versioning:**
- Plan for v2 requirements
- Define breaking change policy
- Establish deprecation timeline
- Communicate changes to consumers

**Feature Enhancements:**
- Advanced filtering capabilities
- Bulk operations support
- Export functionality
- Integration with external systems

### 11.5. Related Documentation

**Must Read:**
- High-Level Architecture (ARCH-HL-001)
- API Flow Specification (ARCH-API-003)
- CQRS Pipeline Specification (ARCH-CQRS-002)
- Domain Modeling Guide (ARCH-DOMAIN-004)

**Recommended Reading:**
- API Guidelines and Conventions
- Authentication and Authorization Guide
- Performance Optimization Guide
- Testing Strategy Document

---

## 12. References

### 12.1. Standards and Specifications

**RFC 7807 - Problem Details for HTTP APIs**
- https://tools.ietf.org/html/rfc7807

**RFC 7230-7235 - HTTP/1.1 Specification**
- https://tools.ietf.org/html/rfc7230

**ISO 4217 - Currency Codes**
- https://www.iso.org/iso-4217-currency-codes.html

**ISO 8601 - Date and Time Format**
- https://www.iso.org/iso-8601-date-and-time-format.html

**OpenAPI Specification 3.0**
- https://swagger.io/specification/

### 12.2. Framework Documentation

**ASP.NET Core Web API**
- https://docs.microsoft.com/aspnet/core/web-api/

**MediatR**
- https://github.com/jbogard/MediatR

**FluentValidation**
- https://fluentvalidation.net/

### 12.3. Testing Tools

**xUnit**
- https://xunit.net/

**WebApplicationFactory**
- https://docs.microsoft.com/aspnet/core/test/integration-tests

**k6 Load Testing**
- https://k6.io/

### 12.4. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- ARCH-HL-001: High-Level Architecture
- ARCH-CQRS-002: CQRS Pipeline Specification
- ARCH-API-003: API Flow Specification
- ARCH-DOMAIN-004: Domain Modeling Guide

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial Catalog API endpoints specification with standardized structure |

---

**END OF DOCUMENT**