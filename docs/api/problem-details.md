# Error Handling and Problem Details Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | API-ERRORHANDLING-001 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Technical Specification |
| Target Audience | Solution Architects, API Developers, Development Teams, Operations Teams, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Quarterly |
| Related Documents | High-Level Architecture (ARCH-HL-001), API Flow (ARCH-API-003), CQRS Pipeline (ARCH-CQRS-002) |
| Prerequisites | Understanding of HTTP protocol, REST APIs, Exception handling |

---

## Executive Summary

This document specifies the comprehensive error handling strategy for the .NET Enterprise Architecture Template, implementing the RFC 7807 Problem Details standard for HTTP APIs. The strategy provides consistent, machine-readable error responses across all API endpoints, enabling reliable client integration, simplified debugging, and enhanced operational diagnostics.

**Strategic Business Value:**
- Reduced integration costs through standardized error formats
- Faster incident resolution via comprehensive error traceability
- Enhanced client experience through clear, actionable error information
- Improved system reliability through consistent error handling patterns
- Reduced support costs via detailed error diagnostics

**Key Technical Capabilities:**
- RFC 7807 Problem Details compliant error responses
- Centralized exception handling with consistent transformation
- Comprehensive exception-to-status-code mapping matrix
- Field-level validation error reporting with structured format
- Correlation ID tracking for distributed tracing integration
- Security-conscious error messaging protecting internal implementation details

**Compliance and Standards:**
- RFC 7807: Problem Details for HTTP APIs
- HTTP/1.1 specification (RFC 7230-7235)
- NIST cybersecurity framework for secure error handling
- ITIL 4 incident management through comprehensive logging
- WCAG 2.1 accessibility through clear error messaging

---

## Table of Contents

1. Introduction and Scope
2. RFC 7807 Problem Details Standard
3. Error Response Structure Specifications
4. Exception Classification and HTTP Status Mapping
5. Centralized Exception Handling Architecture
6. Validation Error Handling
7. Domain Error Handling
8. Authentication and Authorization Error Handling
9. Concurrency and Conflict Error Handling
10. Error Traceability and Correlation
11. Security Considerations
12. Testing Requirements
13. Anti-Patterns and Best Practices
14. Glossary
15. Recommendations and Next Steps
16. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides comprehensive specifications for error handling across all API endpoints in the .NET Enterprise Architecture Template. It defines error response formats, exception handling patterns, HTTP status code mappings, and operational practices ensuring consistent, secure, and debuggable error responses that facilitate client integration and system monitoring.

### 1.2. Scope

**In Scope:**
- RFC 7807 Problem Details response format specifications
- Exception-to-HTTP-status-code mapping definitions
- Centralized exception handling middleware architecture
- Validation error response structures
- Domain error response structures
- Authentication and authorization error handling
- Correlation ID and traceability mechanisms
- Security considerations for error responses
- Testing strategies for error scenarios

**Out of Scope:**
- Specific business domain validation rules (defined per module)
- Authentication mechanism implementations (separate specification)
- Logging infrastructure configuration (separate specification)
- Monitoring and alerting configurations (separate specification)
- Client-side error handling implementations
- Retry and circuit breaker patterns (separate specification)

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**Solution Architects:** Error handling patterns and architectural governance

**API Developers:** Implementation guidelines and code examples

**Frontend Developers:** Error response formats and client integration

**Operations Teams:** Logging structure, correlation tracking, incident investigation

**QA Engineers:** Error scenario testing and validation

**C-Level Executives:** Business value and operational benefits

### 1.4. Error Handling Objectives

The error handling strategy aims to achieve:

**Consistency:**
- Standardized error format across all endpoints
- Predictable structure enabling automated client handling
- Uniform HTTP status code usage

**Machine-Readability:**
- Structured JSON format
- Typed error information
- Parseable field-level errors

**Debuggability:**
- Correlation IDs linking requests across systems
- Comprehensive logging integration
- Detailed error context (where security-appropriate)

**Security:**
- No internal implementation detail leakage
- Sanitized error messages
- Safe stack trace handling

**Usability:**
- Clear, actionable error messages
- Field-specific validation feedback
- Human-readable descriptions

---

## 2. RFC 7807 Problem Details Standard

### 2.1. Standard Overview

#### 2.1.1. RFC 7807 Definition

RFC 7807 defines a standardized, machine-readable format for describing errors in HTTP API responses. The format provides:

**Standard Fields:**
- Consistent structure across APIs
- Extensibility for custom fields
- HTTP status code alignment
- URI-based error type identification

**Benefits:**
- Improved interoperability between systems
- Simplified client error handling
- Enhanced debugging and monitoring
- Industry-standard adoption

#### 2.1.2. Specification Reference

**RFC Document:** RFC 7807 - Problem Details for HTTP APIs

**URL:** https://tools.ietf.org/html/rfc7807

**Publication Date:** March 2016

**Status:** Proposed Standard

### 2.2. Problem Details Schema

#### 2.2.1. Standard Field Definitions

The Problem Details object contains the following standard fields:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| type | string (URI) | No | URI reference identifying the problem type |
| title | string | No | Short, human-readable summary of problem type |
| status | integer | No | HTTP status code for this occurrence |
| detail | string | No | Human-readable explanation specific to this occurrence |
| instance | string (URI) | No | URI reference identifying the specific occurrence |

#### 2.2.2. Standard Problem Details Example

```json
{
  "type": "https://api.example.com/problems/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The request contains invalid data. See errors for details.",
  "instance": "/api/v1/catalog/products"
}
```

### 2.3. Extension Fields

#### 2.3.1. Template-Specific Extensions

This template extends the standard with additional fields:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| traceId | string | Yes | Correlation/trace identifier for logging and tracing |
| errors | object | No | Dictionary of field-specific validation or domain errors |
| timestamp | string (ISO 8601) | No | Error occurrence timestamp |

**Rationale for Extensions:**

**traceId:**
- Essential for distributed tracing
- Links API errors to backend logs
- Supports incident investigation
- Enables end-to-end request tracking

**errors:**
- Provides field-level error detail
- Supports validation error reporting
- Enables client-side field highlighting
- Improves user experience

**timestamp:**
- Documents exact error occurrence time
- Supports audit requirements
- Aids in temporal correlation
- Helps identify time-dependent issues

#### 2.3.2. Complete Extended Example

```json
{
  "type": "https://api.example.com/problems/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The request contains invalid data. See errors for details.",
  "instance": "/api/v1/catalog/products",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "timestamp": "2025-01-15T10:30:00.123Z",
  "errors": {
    "name": [
      "Product name is required.",
      "Product name must not exceed 200 characters."
    ],
    "price": [
      "Price must be greater than zero."
    ]
  }
}
```

---

## 3. Error Response Structure Specifications

### 3.1. Base Error Response

#### 3.1.1. Minimal Error Response

Every error response must include at minimum:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01"
}
```

**Field Requirements:**

**type (Required):**
- Must be a valid URI
- Should reference problem documentation
- Use RFC 7231 URIs for standard HTTP errors
- Use custom URIs for application-specific errors

**title (Required):**
- Concise, human-readable summary
- Should not change for same problem type
- 100 characters or less recommended

**status (Required):**
- Must match HTTP response status code
- Integer value

**traceId (Required):**
- Unique identifier for this error occurrence
- Obtained from Activity.Current.Id or HttpContext.TraceIdentifier
- Format: W3C Trace Context (00-{trace-id}-{span-id}-{flags})

#### 3.1.2. Detailed Error Response

For errors requiring additional context:

```json
{
  "type": "https://api.example.com/problems/business-rule-violation",
  "title": "Business rule violation",
  "status": 422,
  "detail": "The operation violates a business constraint: Product cannot be activated without a valid description.",
  "instance": "/api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1/activate",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "timestamp": "2025-01-15T10:30:00.123Z"
}
```

### 3.2. Validation Error Response

#### 3.2.1. Structure

Validation errors include an `errors` object mapping field names to error message arrays:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The request contains invalid data. See errors for details.",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "errors": {
    "name": [
      "Product name is required.",
      "Product name must not exceed 200 characters."
    ],
    "price": [
      "Price must be greater than zero."
    ],
    "currency": [
      "Currency must be a valid 3-letter ISO 4217 code."
    ]
  }
}
```

#### 3.2.2. Errors Object Specification

**Structure:**
```json
{
  "errors": {
    "fieldName1": ["error message 1", "error message 2"],
    "fieldName2": ["error message 1"],
    "nestedObject.fieldName": ["error message 1"]
  }
}
```

**Field Naming:**
- Use camelCase for field names
- Use dot notation for nested properties (e.g., "address.zipCode")
- Use array index for collection items (e.g., "items[0].quantity")

**Error Messages:**
- Array of strings per field
- Multiple errors per field allowed
- Clear, actionable messages
- No technical jargon
- Localization-ready (future consideration)

### 3.3. HTTP Response Format

#### 3.3.1. Response Headers

**Required Headers:**

```http
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
```

**Header Specifications:**

| Header | Value | Required | Description |
|--------|-------|----------|-------------|
| Content-Type | application/problem+json | Yes | RFC 7807 media type |
| X-Correlation-ID | UUID | Yes | Correlation identifier |
| Cache-Control | no-cache, no-store | Yes | Prevent error caching |

**Note:** Some implementations may use `application/json` instead of `application/problem+json` for broader client compatibility. This is acceptable but should be documented.

#### 3.3.2. Character Encoding

**Requirement:** All error responses must use UTF-8 encoding.

**Header:** `Content-Type: application/problem+json; charset=utf-8`

---

## 4. Exception Classification and HTTP Status Mapping

### 4.1. Exception Hierarchy

#### 4.1.1. Exception Categories

Exceptions are classified into categories based on their nature:

**Client Errors (4xx):**
- Request format errors
- Validation failures
- Authentication failures
- Authorization failures
- Resource not found
- Business rule violations

**Server Errors (5xx):**
- Unhandled exceptions
- Infrastructure failures
- External service failures
- Timeout errors

### 4.2. Exception to HTTP Status Code Mapping

#### 4.2.1. Complete Mapping Matrix

| Exception Type | HTTP Status | Status Code | Problem Type URI | Category |
|----------------|-------------|-------------|------------------|----------|
| ValidationException | Bad Request | 400 | https://tools.ietf.org/html/rfc7231#section-6.5.1 | Client Error |
| NotFoundException | Not Found | 404 | https://tools.ietf.org/html/rfc7231#section-6.5.4 | Client Error |
| UnauthorizedException | Unauthorized | 401 | https://tools.ietf.org/html/rfc7235#section-3.1 | Client Error |
| ForbiddenException | Forbidden | 403 | https://tools.ietf.org/html/rfc7231#section-6.5.3 | Client Error |
| ConflictException | Conflict | 409 | https://tools.ietf.org/html/rfc7231#section-6.5.8 | Client Error |
| DomainException | Unprocessable Entity | 422 | https://tools.ietf.org/html/rfc4918#section-11.2 | Client Error |
| ConcurrencyException | Conflict | 409 | https://tools.ietf.org/html/rfc7231#section-6.5.8 | Client Error |
| TimeoutException | Gateway Timeout | 504 | https://tools.ietf.org/html/rfc7231#section-6.6.5 | Server Error |
| DbException | Internal Server Error | 500 | https://tools.ietf.org/html/rfc7231#section-6.6.1 | Server Error |
| Exception (unhandled) | Internal Server Error | 500 | https://tools.ietf.org/html/rfc7231#section-6.6.1 | Server Error |

#### 4.2.2. Status Code Usage Guidelines

**400 Bad Request:**
- Malformed JSON syntax
- Invalid parameter formats
- Validation rule failures
- Missing required fields

**401 Unauthorized:**
- Missing authentication credentials
- Invalid authentication token
- Expired authentication token

**403 Forbidden:**
- Authenticated but insufficient permissions
- Role-based access denial
- Resource access restrictions

**404 Not Found:**
- Requested resource does not exist
- Invalid resource identifier
- Deleted resources

**409 Conflict:**
- Duplicate resource creation attempts
- Optimistic concurrency failures
- State transition conflicts

**422 Unprocessable Entity:**
- Business rule violations
- Domain invariant violations
- Semantic validation failures

**500 Internal Server Error:**
- Unexpected exceptions
- Infrastructure failures
- Database connection errors

**504 Gateway Timeout:**
- External service timeouts
- Database query timeouts
- Distributed operation timeouts

### 4.3. Custom Exception Types

#### 4.3.1. ValidationException

```csharp
public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
```

#### 4.3.2. NotFoundException

```csharp
public sealed class NotFoundException : Exception
{
    public string ResourceType { get; }
    public object ResourceId { get; }

    public NotFoundException(string resourceType, object resourceId)
        : base($"{resourceType} with identifier {resourceId} was not found.")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}
```

#### 4.3.3. DomainException

```csharp
public sealed class DomainException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, object>? ErrorData { get; }

    public DomainException(string message, string errorCode = "DOMAIN_ERROR")
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public DomainException(
        string message,
        string errorCode,
        Dictionary<string, object> errorData)
        : base(message)
    {
        ErrorCode = errorCode;
        ErrorData = errorData;
    }
}
```

#### 4.3.4. ConflictException

```csharp
public sealed class ConflictException : Exception
{
    public string ConflictType { get; }

    public ConflictException(string message, string conflictType = "RESOURCE_CONFLICT")
        : base(message)
    {
        ConflictType = conflictType;
    }
}
```

---

## 5. Centralized Exception Handling Architecture

### 5.1. Exception Handling Middleware

#### 5.1.1. Middleware Architecture

```
┌─────────────────────────────────────────────────────┐
│          HTTP Request Enters Pipeline               │
└────────────────┬────────────────────────────────────┘
                 │
                 ↓
┌────────────────┴────────────────────────────────────┐
│     Exception Handling Middleware                   │
│  ┌──────────────────────────────────────────────┐   │
│  │  try                                         │   │
│  │  {                                           │   │
│  │      await next(context);                    │   │
│  │  }                                           │   │
│  │  catch (Exception ex)                        │   │
│  │  {                                           │   │
│  │      await HandleExceptionAsync(context, ex);│   │
│  │  }                                           │   │
│  └──────────────────────────────────────────────┘   │
└────────────────┬────────────────────────────────────┘
                 │
                 ↓
┌────────────────┴────────────────────────────────────┐
│          Remaining Middleware Pipeline              │
│  - Authentication                                   │
│  - Authorization                                    │
│  - Routing                                          │
│  - Endpoint Execution                               │
└─────────────────────────────────────────────────────┘
```

#### 5.1.2. Complete Implementation

```csharp
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        // Log the exception
        _logger.LogError(
            exception,
            "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}",
            traceId,
            context.Request.Path);

        // Create problem details from exception
        var problemDetails = CreateProblemDetails(exception, context, traceId);

        // Set response properties
        context.Response.StatusCode = problemDetails.Status ?? 500;
        context.Response.ContentType = "application/problem+json; charset=utf-8";

        // Add correlation ID header
        context.Response.Headers["X-Correlation-ID"] = traceId;

        // Write response
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private ProblemDetails CreateProblemDetails(
        Exception exception,
        HttpContext context,
        string traceId)
    {
        return exception switch
        {
            ValidationException validationEx => CreateValidationProblemDetails(
                validationEx,
                context,
                traceId),

            NotFoundException notFoundEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Resource not found",
                Status = StatusCodes.Status404NotFound,
                Detail = ShouldExposeDetails() ? notFoundEx.Message : null,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = traceId,
                    ["timestamp"] = DateTime.UtcNow
                }
            },

            UnauthorizedException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Authentication credentials are required or invalid.",
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = traceId,
                    ["timestamp"] = DateTime.UtcNow
                }
            },

            ForbiddenException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = "You do not have permission to perform this action.",
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = traceId,
                    ["timestamp"] = DateTime.UtcNow
                }
            },

            ConflictException conflictEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = ShouldExposeDetails() ? conflictEx.Message : "The request conflicts with the current state of the resource.",
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = traceId,
                    ["timestamp"] = DateTime.UtcNow
                }
            },

            DomainException domainEx => CreateDomainProblemDetails(
                domainEx,
                context,
                traceId),

            TimeoutException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.5",
                Title = "Gateway Timeout",
                Status = StatusCodes.Status504GatewayTimeout,
                Detail = "The operation did not complete within the expected time.",
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = traceId,
                    ["timestamp"] = DateTime.UtcNow
                }
            },

            _ => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ShouldExposeDetails() ? exception.Message : "An unexpected error occurred.",
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = traceId,
                    ["timestamp"] = DateTime.UtcNow
                }
            }
        };
    }

    private ValidationProblemDetails CreateValidationProblemDetails(
        ValidationException exception,
        HttpContext context,
        string traceId)
    {
        var problemDetails = new ValidationProblemDetails(exception.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Detail = "The request contains invalid data. See errors for details.",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = traceId;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return problemDetails;
    }

    private ProblemDetails CreateDomainProblemDetails(
        DomainException exception,
        HttpContext context,
        string traceId)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            Title = "Business rule violation",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Extensions =
            {
                ["traceId"] = traceId,
                ["timestamp"] = DateTime.UtcNow,
                ["errorCode"] = exception.ErrorCode
            }
        };

        if (exception.ErrorData != null)
        {
            foreach (var kvp in exception.ErrorData)
            {
                problemDetails.Extensions[kvp.Key] = kvp.Value;
            }
        }

        return problemDetails;
    }

    private bool ShouldExposeDetails()
    {
        // Only expose detailed error messages in development
        return _environment.IsDevelopment();
    }
}
```

#### 5.1.3. Middleware Registration

```csharp
// In Program.cs or Startup.cs
app.UseMiddleware<ExceptionHandlingMiddleware>();

// OR using extension method
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

// Usage
app.UseExceptionHandling();
```

**Registration Order:**

```csharp
app.UseExceptionHandling();        // 1. Exception boundary
app.UseCorrelationId();            // 2. Correlation ID
app.UseHttpsRedirection();         // 3. HTTPS enforcement
app.UseAuthentication();           // 4. Authentication
app.UseAuthorization();            // 5. Authorization
app.MapControllers();              // 6. Endpoint routing
```

---

## 6. Validation Error Handling

### 6.1. FluentValidation Integration

#### 6.1.1. Validation Pipeline Behavior

Validation errors are caught in the CQRS pipeline's ValidationBehavior:

```csharp
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => 
                v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

#### 6.1.2. ValidationException Construction

```csharp
public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => ToCamelCase(g.Key), g => g.ToArray());
    }

    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
```

### 6.2. Validation Error Response Examples

#### 6.2.1. Single Field Error

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The request contains invalid data. See errors for details.",
  "instance": "/api/v1/catalog/products",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "errors": {
    "name": [
      "Product name is required."
    ]
  }
}
```

#### 6.2.2. Multiple Field Errors

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The request contains invalid data. See errors for details.",
  "instance": "/api/v1/catalog/products",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "errors": {
    "name": [
      "Product name is required.",
      "Product name must not exceed 200 characters."
    ],
    "price": [
      "Price must be greater than zero."
    ],
    "currency": [
      "Currency must be a valid 3-letter ISO 4217 code."
    ]
  }
}
```

#### 6.2.3. Nested Object Validation

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The request contains invalid data. See errors for details.",
  "instance": "/api/v1/orders",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "errors": {
    "shippingAddress.street": [
      "Street address is required."
    ],
    "shippingAddress.zipCode": [
      "Zip code must be 5 digits."
    ],
    "items[0].quantity": [
      "Quantity must be greater than zero."
    ]
  }
}
```

---

## 7. Domain Error Handling

### 7.1. Domain Exception Patterns

#### 7.1.1. Business Rule Violation

**Scenario:** Operation violates a business rule or domain invariant.

**Exception:**
```csharp
throw new DomainException(
    "Product price cannot be negative.",
    "INVALID_PRICE");
```

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "Business rule violation",
  "status": 422,
  "detail": "Product price cannot be negative.",
  "instance": "/api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "errorCode": "INVALID_PRICE"
}
```

#### 7.1.2. State Transition Error

**Scenario:** Invalid state transition attempt.

**Exception:**
```csharp
throw new DomainException(
    "Cannot ship order that is not in placed status.",
    "INVALID_STATE_TRANSITION",
    new Dictionary<string, object>
    {
        ["currentState"] = "Draft",
        ["requestedState"] = "Shipped",
        ["allowedTransitions"] = new[] { "Placed" }
    });
```

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "Business rule violation",
  "status": 422,
  "detail": "Cannot ship order that is not in placed status.",
  "instance": "/api/v1/orders/a1b2c3d4-e5f6-7890-abcd-ef1234567890/ship",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "errorCode": "INVALID_STATE_TRANSITION",
  "currentState": "Draft",
  "requestedState": "Shipped",
  "allowedTransitions": ["Placed"]
}
```

#### 7.1.3. Invariant Violation

**Scenario:** Domain invariant protection.

**Exception:**
```csharp
throw new DomainException(
    "Order total must equal sum of line items.",
    "INVARIANT_VIOLATION");
```

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "Business rule violation",
  "status": 422,
  "detail": "Order total must equal sum of line items.",
  "instance": "/api/v1/orders/a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "errorCode": "INVARIANT_VIOLATION"
}
```

### 7.2. Domain Error Best Practices

#### 7.2.1. Error Message Guidelines

**Do:**
- Use clear, business-focused language
- Describe what went wrong and why
- Suggest corrective action when possible
- Use consistent terminology

**Don't:**
- Expose technical implementation details
- Include SQL queries or stack traces
- Use technical jargon
- Blame the user

**Examples:**

**Good:**
- "Product cannot be activated without a valid description."
- "Insufficient inventory. Available quantity: 5, requested: 10."
- "Order cannot be cancelled after shipment."

**Bad:**
- "NullReferenceException in ProductService.Activate()"
- "FK constraint violation on Products table"
- "You made an error in the request"

#### 7.2.2. Error Code Conventions

**Format:** UPPERCASE_SNAKE_CASE

**Categories:**
- VALIDATION_*: Validation errors
- BUSINESS_RULE_*: Business rule violations
- STATE_*: State transition errors
- INVARIANT_*: Invariant violations
- PERMISSION_*: Authorization issues

**Examples:**
- VALIDATION_REQUIRED_FIELD
- BUSINESS_RULE_DUPLICATE_NAME
- STATE_INVALID_TRANSITION
- INVARIANT_TOTAL_MISMATCH
- PERMISSION_INSUFFICIENT_ROLE

---

## 8. Authentication and Authorization Error Handling

### 8.1. Authentication Errors (401 Unauthorized)

#### 8.1.1. Missing Credentials

**Scenario:** No authentication credentials provided.

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication credentials are required to access this resource.",
  "instance": "/api/v1/catalog/products",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01"
}
```

#### 8.1.2. Invalid Credentials

**Scenario:** Invalid or expired authentication token.

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "The provided authentication credentials are invalid or expired.",
  "instance": "/api/v1/catalog/products",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01"
}
```

### 8.2. Authorization Errors (403 Forbidden)

#### 8.2.1. Insufficient Permissions

**Scenario:** Authenticated but lacks required permissions.

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have permission to perform this action.",
  "instance": "/api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01"
}
```

#### 8.2.2. Role-Based Access Denial

**Scenario:** User role insufficient for operation.

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "This action requires the 'Catalog.Admin' role.",
  "instance": "/api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "requiredRole": "Catalog.Admin"
}
```

### 8.3. Security Considerations

#### 8.3.1. Information Disclosure Prevention

**Guidelines:**
- Don't reveal whether users exist
- Don't distinguish between authentication and authorization failures when security-critical
- Don't expose internal system structure
- Use generic messages for security-sensitive operations

**Example - Account Lockout:**

**Bad:**
```json
{
  "detail": "Account locked after 3 failed login attempts. Try again in 15 minutes."
}
```

**Good:**
```json
{
  "detail": "Authentication failed. Please check your credentials."
}
```

---

## 9. Concurrency and Conflict Error Handling

### 9.1. Optimistic Concurrency Failures

#### 9.1.1. ETag Mismatch

**Scenario:** Resource modified by another request.

**Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "The resource has been modified by another request. Please reload and try again.",
  "instance": "/api/v1/catalog/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "conflictType": "CONCURRENCY_CONFLICT"
}
```

### 9.2. Duplicate Resource Conflicts

#### 9.2.1. Unique Constraint Violation

**Scenario:** Attempt to create duplicate resource.

**Response:**
```json
{
  "type": "https://api.example.com/problems/duplicate-resource",
  "title": "Duplicate resource",
  "status": 409,
  "detail": "A product with the name 'Phone X' already exists.",
  "instance": "/api/v1/catalog/products",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "conflictType": "DUPLICATE_RESOURCE",
  "conflictingField": "name",
  "conflictingValue": "Phone X",
  "existingResourceId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

### 9.3. State Conflicts

#### 9.3.1. Invalid State for Operation

**Scenario:** Operation not allowed in current state.

**Response:**
```json
{
  "type": "https://api.example.com/problems/invalid-state",
  "title": "Invalid state",
  "status": 409,
  "detail": "Cannot cancel order that has already been shipped.",
  "instance": "/api/v1/orders/a1b2c3d4-e5f6-7890-abcd-ef1234567890/cancel",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "currentState": "Shipped",
  "requestedAction": "Cancel"
}
```

---

## 10. Error Traceability and Correlation

### 10.1. Correlation ID Generation

#### 10.1.1. W3C Trace Context

**Format:** 00-{trace-id}-{span-id}-{flags}

**Example:** 00-0f83f4d5c8ab648794d4ef69334b1d51-b9c7c989f97918e1-01

**Components:**
- Version: 00
- Trace ID: 32 hexadecimal characters (16 bytes)
- Span ID: 16 hexadecimal characters (8 bytes)
- Flags: 2 hexadecimal characters (1 byte)

#### 10.1.2. Correlation ID Middleware

```csharp
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        // Add to response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        // Add to logging context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        // Check if client provided correlation ID
        if (context.Request.Headers.TryGetValue(
            CorrelationIdHeader,
            out var correlationId) &&
            !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId!;
        }

        // Use Activity ID if available (W3C Trace Context)
        if (Activity.Current?.Id != null)
        {
            return Activity.Current.Id;
        }

        // Fallback to HTTP context trace identifier
        return context.TraceIdentifier;
    }
}
```

### 10.2. Distributed Tracing Integration

#### 10.2.1. OpenTelemetry Integration

```csharp
public sealed class ExceptionHandlingMiddleware
{
    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var activity = Activity.Current;
        
        if (activity != null)
        {
            // Set error status on span
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            
            // Record exception details
            activity.RecordException(exception);
            
            // Add custom tags
            activity.SetTag("error.type", exception.GetType().Name);
            activity.SetTag("http.status_code", GetStatusCode(exception));
        }

        var traceId = activity?.Id ?? context.TraceIdentifier;
        
        // Create and send problem details
        // ...
    }
}
```

### 10.3. Logging Integration

#### 10.3.1. Structured Error Logging

```csharp
_logger.LogError(
    exception,
    "Unhandled exception occurred. " +
    "TraceId: {TraceId}, " +
    "Path: {Path}, " +
    "Method: {Method}, " +
    "StatusCode: {StatusCode}, " +
    "ExceptionType: {ExceptionType}",
    traceId,
    context.Request.Path,
    context.Request.Method,
    statusCode,
    exception.GetType().Name);
```

#### 10.3.2. Log Enrichment

```csharp
using (LogContext.PushProperty("CorrelationId", traceId))
using (LogContext.PushProperty("UserId", userId))
using (LogContext.PushProperty("RequestPath", context.Request.Path))
{
    _logger.LogError(exception, "Error processing request");
}
```

---

## 11. Security Considerations

### 11.1. Information Disclosure Prevention

#### 11.1.1. Environment-Based Detail Exposure

```csharp
private bool ShouldExposeDetails()
{
    return _environment.IsDevelopment();
}

private string GetErrorDetail(Exception exception)
{
    if (ShouldExposeDetails())
    {
        return exception.Message;
    }

    return "An error occurred while processing your request.";
}
```

#### 11.1.2. Stack Trace Protection

**Never Expose:**
- Full stack traces
- Internal file paths
- Database connection strings
- Internal service URLs
- Framework version details

**Development Environment:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "Object reference not set to an instance of an object.",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01"
}
```

**Production Environment:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred.",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01"
}
```

### 11.2. SQL Injection Protection

#### 11.2.1. Error Message Sanitization

**Never Include:**
- SQL query text
- Parameter values
- Table or column names
- Database schema information

**Bad:**
```json
{
  "detail": "Invalid query: SELECT * FROM Products WHERE Name = 'Robert'); DROP TABLE Products;--'"
}
```

**Good:**
```json
{
  "detail": "Invalid product search criteria."
}
```

### 11.3. Sensitive Data Protection

#### 11.3.1. PII Redaction

**Redact in Error Messages:**
- Email addresses
- Phone numbers
- Credit card numbers
- Social security numbers
- Passwords
- Authentication tokens

**Example Redaction:**

```csharp
private static string RedactSensitiveData(string message)
{
    // Redact email addresses
    message = Regex.Replace(
        message,
        @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
        "[EMAIL_REDACTED]");

    // Redact phone numbers
    message = Regex.Replace(
        message,
        @"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b",
        "[PHONE_REDACTED]");

    return message;
}
```

---

## 12. Testing Requirements

### 12.1. Unit Testing

#### 12.1.1. Exception Mapping Tests

```csharp
public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task HandleException_ValidationException_Returns400WithErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            ["name"] = new[] { "Name is required." }
        };
        var exception = new ValidationException(errors);

        // Act
        var result = await HandleExceptionAsync(exception);

        // Assert
        Assert.Equal(400, result.Status);
        Assert.Equal("One or more validation errors occurred.", result.Title);
        Assert.Contains("name", result.Errors.Keys);
    }

    [Fact]
    public async Task HandleException_NotFoundException_Returns404()
    {
        // Arrange
        var exception = new NotFoundException("Product", Guid.NewGuid());

        // Act
        var result = await HandleExceptionAsync(exception);

        // Assert
        Assert.Equal(404, result.Status);
        Assert.Equal("Resource not found", result.Title);
    }

    [Fact]
    public async Task HandleException_UnhandledException_Returns500()
    {
        // Arrange
        var exception = new InvalidOperationException("Something went wrong");

        // Act
        var result = await HandleExceptionAsync(exception);

        // Assert
        Assert.Equal(500, result.Status);
        Assert.Equal("Internal Server Error", result.Title);
    }
}
```

### 12.2. Integration Testing

#### 12.2.1. End-to-End Error Scenarios

```csharp
public sealed class ErrorHandlingIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Post_InvalidProduct_Returns400WithValidationErrors()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "", // Invalid
            Price = -10 // Invalid
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/catalog/products",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var problemDetails = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Equal(400, problemDetails.Status);
        Assert.Contains("name", problemDetails.Errors.Keys);
        Assert.Contains("price", problemDetails.Errors.Keys);
        Assert.NotNull(problemDetails.Extensions["traceId"]);
    }

    [Fact]
    public async Task Get_NonExistentProduct_Returns404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/catalog/products/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var problemDetails = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Equal(404, problemDetails.Status);
        Assert.Equal("Resource not found", problemDetails.Title);
    }

    [Fact]
    public async Task Post_WithoutAuthentication_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            Price = 99.99m
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/catalog/products",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        
        var problemDetails = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Equal(401, problemDetails.Status);
    }
}
```

### 12.3. Error Response Schema Validation

#### 12.3.1. JSON Schema Validation

```csharp
[Fact]
public async Task ErrorResponse_MustMatchProblemDetailsSchema()
{
    // Arrange
    var request = new CreateProductRequest { Name = "" };

    // Act
    var response = await _client.PostAsJsonAsync(
        "/api/v1/catalog/products",
        request);

    // Assert
    var json = await response.Content.ReadAsStringAsync();
    var schema = GetProblemDetailsSchema();
    
    var isValid = ValidateJsonSchema(json, schema);
    Assert.True(isValid, "Response does not match Problem Details schema");
}
```

---

## 13. Anti-Patterns and Best Practices

### 13.1. Anti-Patterns to Avoid

#### 13.1.1. Inconsistent Error Formats

**Anti-Pattern:**
```csharp
// Different endpoints returning different error formats
// Endpoint 1
return BadRequest(new { error = "Invalid data" });

// Endpoint 2
return BadRequest(new { message = "Validation failed", errors = [] });

// Endpoint 3
return BadRequest("Bad request");
```

**Correct:**
```csharp
// All endpoints use Problem Details
throw new ValidationException(errors);
// Middleware transforms to consistent Problem Details
```

#### 13.1.2. Information Leakage

**Anti-Pattern:**
```csharp
catch (SqlException ex)
{
    return StatusCode(500, new
    {
        error = ex.Message, // Exposes SQL details!
        query = ex.Procedure,
        connectionString = _connectionString
    });
}
```

**Correct:**
```csharp
catch (SqlException ex)
{
    _logger.LogError(ex, "Database error occurred");
    throw; // Let middleware handle with safe message
}
```

#### 13.1.3. Swallowing Exceptions

**Anti-Pattern:**
```csharp
try
{
    await _service.ProcessAsync();
}
catch (Exception)
{
    return Ok(); // Hiding errors!
}
```

**Correct:**
```csharp
// Let exceptions propagate to middleware
await _service.ProcessAsync();
// Or handle specifically
try
{
    await _service.ProcessAsync();
}
catch (SpecificException ex)
{
    _logger.LogWarning(ex, "Handled specific error");
    throw new DomainException("User-friendly message", "ERROR_CODE");
}
```

#### 13.1.4. Generic Error Messages

**Anti-Pattern:**
```json
{
  "error": "Error",
  "message": "Something went wrong"
}
```

**Correct:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The request contains invalid data. See errors for details.",
  "traceId": "00-0f83f4d5c8ab648794d4ef69334b1d51-01",
  "errors": {
    "name": ["Product name is required."]
  }
}
```

### 13.2. Best Practices

#### 13.2.1. Centralize Error Handling

**Do:**
- Use single exception handling middleware
- Define clear exception-to-status mappings
- Maintain consistent error format

**Don't:**
- Handle exceptions in multiple places
- Return different formats per controller
- Duplicate error handling logic

#### 13.2.2. Log Comprehensively

**Do:**
- Log all exceptions with trace IDs
- Include relevant context (user, request path, parameters)
- Use structured logging
- Set appropriate log levels

**Don't:**
- Log sensitive data (passwords, tokens)
- Log client IP addresses without consent
- Duplicate logging across layers

#### 13.2.3. Provide Actionable Messages

**Do:**
- Explain what went wrong
- Suggest corrective action
- Use business terminology
- Be specific about requirements

**Don't:**
- Use technical jargon
- Blame the user
- Be vague
- Expose implementation details

---

## 14. Glossary

**HTTP Status Code:** A three-digit code in HTTP response indicating the status of the request.

**Problem Details:** RFC 7807 standardized format for describing HTTP API errors in machine-readable format.

**RFC 7807:** IETF specification "Problem Details for HTTP APIs" defining standard error response format.

**Trace ID:** Unique identifier assigned to a request for tracking across distributed systems.

**W3C Trace Context:** Standardized HTTP header format for propagating trace context across system boundaries.

**Correlation ID:** Identifier linking related operations across multiple services or components.

**ValidationException:** Exception thrown when request data fails validation rules.

**DomainException:** Exception thrown when business rule or domain invariant is violated.

**NotFoundException:** Exception thrown when requested resource does not exist.

**ConflictException:** Exception thrown when operation conflicts with current resource state.

**Information Disclosure:** Security vulnerability where system reveals sensitive implementation details.

**PII (Personally Identifiable Information):** Data that can identify a specific individual.

**Middleware:** Software component in ASP.NET Core pipeline that processes HTTP requests and responses.

**Activity:** .NET distributed tracing context tracking request flow across system components.

**OpenTelemetry:** Open-source observability framework for generating and collecting telemetry data.

---

## 15. Recommendations and Next Steps

### 15.1. For Development Teams

#### 15.1.1. Implementation Checklist

**When Implementing Error Handling:**
- [ ] Register exception handling middleware first in pipeline
- [ ] Implement correlation ID middleware
- [ ] Create custom exception types for domain errors
- [ ] Implement consistent exception-to-status mapping
- [ ] Add comprehensive logging with trace IDs
- [ ] Test all error scenarios
- [ ] Validate Problem Details format compliance
- [ ] Document custom error codes
- [ ] Implement environment-based detail exposure

**When Adding New Exception Types:**
- [ ] Define clear exception class with meaningful properties
- [ ] Add mapping in exception handling middleware
- [ ] Choose appropriate HTTP status code
- [ ] Define problem type URI
- [ ] Write unit tests for mapping
- [ ] Document in API specification
- [ ] Add integration tests

#### 15.1.2. Code Quality Standards

**Exception Handling:**
- Let exceptions propagate to middleware
- Don't catch and rethrow without adding value
- Use specific exception types
- Include relevant context in exceptions
- Log before throwing custom exceptions

**Error Messages:**
- Write for end users, not developers
- Be specific and actionable
- Use consistent terminology
- Avoid technical jargon
- Consider localization needs

### 15.2. For Operations Teams

#### 15.2.1. Monitoring and Alerting

**Key Metrics:**
- Error rate by status code
- Error rate by endpoint
- Validation error frequency
- Exception type distribution
- Mean time to resolution by error type

**Alerting Thresholds:**
- 5xx error rate > 1% for 5 minutes
- 4xx error rate > 10% for 10 minutes
- Specific error code spike (3x normal)
- Timeout rate increase

#### 15.2.2. Incident Response

**Error Investigation Process:**
1. Locate error in logs using trace ID
2. Identify error category (validation, domain, infrastructure)
3. Review structured log context
4. Check distributed trace for complete request flow
5. Identify root cause
6. Implement fix
7. Verify resolution
8. Update runbooks

**Log Queries:**
```
// Find errors by trace ID
CorrelationId:"00-0f83f4d5c8ab648794d4ef69334b1d51-01"

// Find errors by type
ExceptionType:"ValidationException"

// Find errors by status code
StatusCode:500 AND Level:Error
```

### 15.3. For QA Teams

#### 15.3.1. Error Testing Scenarios

**Mandatory Test Cases:**
- Submit invalid data (validation errors)
- Request non-existent resources (404)
- Attempt unauthorized operations (401/403)
- Trigger business rule violations (422)
- Force concurrency conflicts (409)
- Simulate timeout scenarios (504)
- Test error response format compliance

**Boundary Testing:**
- Empty required fields
- Maximum length violations
- Invalid data types
- Out-of-range values
- Invalid formats (email, phone, currency)

**Security Testing:**
- Verify no stack traces in production
- Verify no sensitive data in errors
- Verify consistent error messages
- Test error message tampering

#### 15.3.2. Automated Testing

**Error Response Validation:**
```csharp
public static class ProblemDetailsAssertions
{
    public static void AssertValidProblemDetails(
        ProblemDetails problemDetails,
        int expectedStatus)
    {
        Assert.NotNull(problemDetails);
        Assert.NotNull(problemDetails.Type);
        Assert.NotNull(problemDetails.Title);
        Assert.Equal(expectedStatus, problemDetails.Status);
        Assert.True(problemDetails.Extensions.ContainsKey("traceId"));
    }
}
```

### 15.4. For Architects

#### 15.4.1. Architecture Governance

**Periodic Reviews:**
- Error handling pattern consistency
- Exception type proliferation
- Error message quality
- Security compliance
- Performance impact of error handling

**Documentation Maintenance:**
- Update exception catalog
- Document new error codes
- Maintain Problem Details examples
- Update API specifications

#### 15.4.2. Evolution Planning

**Future Enhancements:**
- Error message localization
- Custom problem type URIs
- Enhanced error analytics
- Client-side error handling SDKs
- Error recovery suggestions

### 15.5. Related Documentation

**Must Read:**
- High-Level Architecture (ARCH-HL-001)
- API Flow Specification (ARCH-API-003)
- CQRS Pipeline Specification (ARCH-CQRS-002)

**Recommended Reading:**
- API Guidelines and Conventions
- Security Implementation Guide
- Logging and Monitoring Guide
- Testing Strategy Document

---

## 16. References

### 16.1. Standards and Specifications

**RFC 7807 - Problem Details for HTTP APIs**
- https://tools.ietf.org/html/rfc7807

**RFC 7231 - HTTP/1.1 Semantics and Content**
- https://tools.ietf.org/html/rfc7231

**RFC 7235 - HTTP/1.1 Authentication**
- https://tools.ietf.org/html/rfc7235

**W3C Trace Context**
- https://www.w3.org/TR/trace-context/

**OWASP API Security Top 10**
- https://owasp.org/www-project-api-security/

### 16.2. Framework Documentation

**ASP.NET Core Error Handling**
- https://docs.microsoft.com/aspnet/core/fundamentals/error-handling

**FluentValidation**
- https://fluentvalidation.net/

**Serilog**
- https://serilog.net/

**OpenTelemetry .NET**
- https://opentelemetry.io/docs/instrumentation/net/

### 16.3. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- ARCH-HL-001: High-Level Architecture
- ARCH-API-003: API Flow Specification
- ARCH-CQRS-002: CQRS Pipeline Specification
- API-CATALOG-001: Catalog API Endpoints

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial error handling and Problem Details specification with standardized structure |

---

**END OF DOCUMENT**