# Authentication and Authorization Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | API-AUTH-001 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Security Specification |
| Target Audience | Security Architects, Solution Architects, Development Teams, Security Operations, C-Level Executives |
| Classification | Internal/Confidential |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Quarterly |
| Related Documents | API Guidelines (API-GUIDELINES-001), Error Handling (API-ERRORHANDLING-001), High-Level Architecture (ARCH-HL-001) |
| Prerequisites | Understanding of OAuth 2.0, JWT, RBAC, HTTP security |

---

## Executive Summary

This document establishes comprehensive authentication and authorization specifications for the .NET Enterprise Architecture Template, implementing industry-standard security patterns based on OAuth 2.0, OpenID Connect, and JWT bearer token authentication. The specification provides enterprise-grade security controls protecting API endpoints while supporting flexible authorization models including role-based access control (RBAC), policy-based authorization, and claims-based authorization.

**Strategic Business Value:**
- Enhanced security posture through industry-standard authentication protocols
- Reduced security incidents via centralized identity management integration
- Improved compliance with data protection regulations (GDPR, HIPAA, SOC 2)
- Decreased operational costs through standardized security implementations
- Enhanced audit capabilities via comprehensive access logging
- Scalable multi-tenant support with tenant isolation guarantees

**Key Technical Capabilities:**
- OAuth 2.0 / OpenID Connect compliant authentication
- JWT bearer token validation and verification
- Role-Based Access Control (RBAC) for coarse-grained authorization
- Policy-Based Authorization for fine-grained access control
- Claims-Based Authorization for attribute-based decisions
- Multi-tenant context isolation and enforcement
- Stateless API design with token-based identity
- Integration with enterprise identity providers (Azure AD, Auth0, IdentityServer)

**Compliance and Standards:**
- OAuth 2.0 (RFC 6749) authorization framework
- OpenID Connect Core 1.0 specification
- JWT (RFC 7519) token standard
- NIST SP 800-63B Digital Identity Guidelines
- OWASP API Security Top 10 compliance
- ISO/IEC 27001 information security management alignment

---

## Table of Contents

1. Introduction and Scope
2. Security Architecture Principles
3. Authentication Mechanisms
4. Authorization Models
5. Token Management and Validation
6. Endpoint Security Patterns
7. Multi-Tenancy Security
8. Error Handling for Authentication and Authorization
9. Identity Provider Integration
10. Security Testing Requirements
11. Environment-Specific Security Configuration
12. Monitoring and Audit Requirements
13. Glossary
14. Recommendations and Next Steps
15. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides authoritative specifications for implementing authentication and authorization within the .NET Enterprise Architecture Template. It defines security patterns, token validation procedures, authorization models, and integration approaches ensuring consistent, auditable, and defense-in-depth security across all API implementations.

### 1.2. Scope

**In Scope:**
- Authentication mechanism specifications
- Authorization model implementations (RBAC, PBAC, CBAC)
- JWT bearer token validation requirements
- Identity provider integration patterns
- Multi-tenant security isolation
- Security error handling
- Token lifecycle management
- Claims processing and validation
- Security testing approaches
- Audit logging requirements

**Out of Scope:**
- Identity provider infrastructure deployment
- User registration and password management (handled by IdP)
- Certificate management and PKI infrastructure
- Network security configurations (firewalls, WAF)
- Cryptographic key generation procedures
- Data encryption at rest specifications
- Specific compliance framework implementations (HIPAA, PCI-DSS)
- Single Sign-On (SSO) portal implementations

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**Security Architects:** Security architecture patterns and threat modeling

**Solution Architects:** Integration architecture and identity provider selection

**Development Teams:** Implementation guidelines and code examples

**Security Operations:** Monitoring, audit, and incident response requirements

**Compliance Officers:** Regulatory compliance verification points

**C-Level Executives:** Business risk mitigation and security investment justification

### 1.4. Security Architecture Context

Authentication and authorization integrate within the broader security architecture:

```
┌─────────────────────────────────────────────────────────┐
│                External Security Layer                  │
│  - API Gateway                                          │
│  - WAF (Web Application Firewall)                       │
│  - DDoS Protection                                      │
│  - Rate Limiting                                        │
└────────────────┬────────────────────────────────────────┘
                 │
                 ↓
┌────────────────┴────────────────────────────────────────┐
│           Authentication & Authorization Layer          │
│  ┌──────────────────────────────────────────────────┐   │
│  │  ✓ Token Validation                              │   │
│  │  ✓ Claims Extraction                             │   │
│  │  ✓ Authorization Policy Evaluation               │   │
│  │  ✓ Tenant Context Resolution                     │   │
│  └──────────────────────────────────────────────────┘   │
└────────────────┬────────────────────────────────────────┘
                 │
                 ↓
┌────────────────┴────────────────────────────────────────┐
│                  Application Layer                      │
│  - Business Logic                                       │
│  - Domain Rules                                         │
│  - Resource Access                                      │
└─────────────────────────────────────────────────────────┘
```

---

## 2. Security Architecture Principles

### 2.1. Foundational Security Principles

#### 2.1.1. Secure by Default

**Principle:** All API endpoints require authentication unless explicitly designated as public.

**Implementation:**
- Global authentication requirement via `[Authorize]` attribute
- Explicit `[AllowAnonymous]` for public endpoints
- Code review verification of authorization attributes
- Automated tests for unauthorized access attempts

**Default Controller Configuration:**
```csharp
[ApiController]
[Authorize] // Applied globally
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class SecureControllerBase : ControllerBase
{
    // All derived controllers require authentication by default
}
```

#### 2.1.2. Defense in Depth

**Principle:** Implement multiple layers of security controls.

**Layers:**
1. **Network Layer:** TLS/SSL encryption, network segmentation
2. **Gateway Layer:** API gateway authentication, rate limiting
3. **Application Layer:** Token validation, authorization policies
4. **Business Layer:** Domain-specific authorization rules
5. **Data Layer:** Row-level security, encryption at rest

#### 2.1.3. Least Privilege

**Principle:** Grant minimum permissions necessary for operation execution.

**Implementation:**
- Granular permission definitions (read, write, delete per resource)
- Role assignments with minimal scope
- Time-bound access grants where applicable
- Regular permission audits

**Permission Hierarchy Example:**
```
catalog.read        → Read products only
catalog.write       → Create/update products
catalog.delete      → Delete products (Admin only)
catalog.admin       → Full catalog management
```

#### 2.1.4. Zero Trust Architecture

**Principle:** Never trust, always verify every request.

**Requirements:**
- Validate every token on every request
- No implicit trust based on network location
- Verify authorization for each operation
- Audit all access attempts

#### 2.1.5. Statelessness

**Principle:** APIs maintain no session state; tokens carry complete identity context.

**Benefits:**
- Horizontal scalability without session affinity
- Simplified load balancing
- Reduced infrastructure complexity
- Enhanced resilience

**Implementation:**
- JWT tokens contain all necessary claims
- No server-side session storage
- Idempotent request handling

---

## 3. Authentication Mechanisms

### 3.1. Bearer Token Authentication

#### 3.1.1. Specification

**Primary Mechanism:** OAuth 2.0 Bearer Token Authentication

**HTTP Header:**
```http
Authorization: Bearer {access_token}
```

**Token Format:** JSON Web Token (JWT) - RFC 7519

**Token Structure:**
```
Header.Payload.Signature

Example:
eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.
eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

#### 3.1.2. JWT Token Claims

**Standard Claims (RFC 7519):**

| Claim | Name | Required | Description |
|-------|------|----------|-------------|
| iss | Issuer | Yes | Token issuer identifier |
| sub | Subject | Yes | User/principal identifier |
| aud | Audience | Yes | Intended token recipient |
| exp | Expiration Time | Yes | Token expiration timestamp |
| nbf | Not Before | No | Token valid after timestamp |
| iat | Issued At | Yes | Token issue timestamp |
| jti | JWT ID | No | Unique token identifier |

**Application-Specific Claims:**

| Claim | Type | Required | Description |
|-------|------|----------|-------------|
| scope | string or array | Yes | OAuth 2.0 scopes |
| role | string or array | No | User roles |
| tenantId | string | Yes (multi-tenant) | Tenant identifier |
| email | string | No | User email address |
| name | string | No | User display name |
| permissions | array | No | Fine-grained permissions |

**Example JWT Payload:**
```json
{
  "iss": "https://auth.example.com",
  "sub": "user-123456",
  "aud": "api.example.com",
  "exp": 1735689600,
  "iat": 1735686000,
  "scope": ["catalog.read", "catalog.write"],
  "role": ["Editor"],
  "tenantId": "tenant-abc",
  "email": "user@example.com",
  "name": "John Doe"
}
```

### 3.2. OAuth 2.0 Grant Types

#### 3.2.1. Client Credentials Flow

**Use Case:** Machine-to-machine, service-to-service authentication

**Flow:**
```
┌────────┐                                  ┌──────────────┐
│ Client │                                  │ Auth Server  │
│ API    │                                  │              │
└───┬────┘                                  └──────┬───────┘
    │                                              │
    │ POST /token                                  │
    │ grant_type=client_credentials                │
    │ client_id=...                                │
    │ client_secret=...                            │
    ├─────────────────────────────────────────────>│
    │                                              │
    │                                              │ Validate
    │                                              │ credentials
    │                                              │
    │ 200 OK                                       │
    │ {                                            │
    │   "access_token": "...",                     │
    │   "token_type": "Bearer",                    │
    │   "expires_in": 3600                         │
    │ }                                            │
    │<─────────────────────────────────────────────┤
    │                                              │
    │ GET /api/v1/resources                        │
    │ Authorization: Bearer {token}                │
    ├──────────────────────────>                  │
    │                           │                  │
    │                           │ Resource Server  │
    │                           │ validates token  │
    │                           │                  │
    │ 200 OK + Resources        │                  │
    │<──────────────────────────┘                  │
```

**Configuration:**
```csharp
services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = configuration["Authentication:Authority"];
        options.Audience = configuration["Authentication:Audience"];
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });
```

#### 3.2.2. Authorization Code Flow with PKCE

**Use Case:** User-based authentication from web applications, SPAs, mobile apps

**Flow:** (Simplified)
```
User → Frontend → Auth Server (login) → 
Auth Server returns authorization code → 
Frontend exchanges code for token → 
Frontend calls API with token
```

**Security Requirements:**
- PKCE (Proof Key for Code Exchange) mandatory
- State parameter for CSRF protection
- Short-lived authorization codes (< 10 minutes)
- One-time use authorization codes

#### 3.2.3. Refresh Token Flow

**Use Case:** Long-lived sessions with short-lived access tokens

**Configuration:**
- Access token lifetime: 15-60 minutes
- Refresh token lifetime: 7-30 days
- Refresh token rotation on use (recommended)
- Refresh token revocation support

### 3.3. ASP.NET Core Authentication Configuration

#### 3.3.1. Complete Configuration Example

**Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Identity provider configuration
    options.Authority = builder.Configuration["Authentication:Authority"];
    options.Audience = builder.Configuration["Authentication:Audience"];
    
    // Token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Authentication:Audience"],
        
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5),
        
        ValidateIssuerSigningKey = true,
        // Signing key retrieved from Authority metadata endpoint
    };
    
    // HTTPS requirement
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    
    // Event handlers for logging and diagnostics
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning(
                "Authentication failed: {Error}",
                context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information(
                "Token validated for user: {UserId}",
                context.Principal?.FindFirst("sub")?.Value);
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Authentication and authorization middleware
app.UseAuthentication(); // MUST come before UseAuthorization
app.UseAuthorization();

app.MapControllers();
app.Run();
```

**appsettings.json:**
```json
{
  "Authentication": {
    "Authority": "https://auth.example.com",
    "Audience": "api.example.com",
    "Issuer": "https://auth.example.com"
  }
}
```

---

## 4. Authorization Models

### 4.1. Role-Based Access Control (RBAC)

#### 4.1.1. Role Definition

**Purpose:** Coarse-grained access control based on organizational roles

**Standard Roles:**

| Role | Description | Typical Permissions |
|------|-------------|---------------------|
| Admin | System administrator | Full access to all resources |
| Manager | Department manager | Read/write within department |
| Editor | Content editor | Create and modify resources |
| Viewer | Read-only user | View resources only |
| Guest | Limited access user | View public resources only |

#### 4.1.2. Role-Based Authorization Implementation

**Controller-Level Authorization:**
```csharp
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")] // Entire controller requires Admin role
public class AdminController : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        // Only Admin role can access
    }
}
```

**Action-Level Authorization:**
```csharp
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Viewer,Editor,Admin")] // Multiple roles
    public async Task<IActionResult> GetProducts()
    {
        // Viewers, Editors, and Admins can access
    }

    [HttpPost]
    [Authorize(Roles = "Editor,Admin")]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        // Only Editors and Admins can create
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Single role
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        // Only Admin can delete
    }
}
```

#### 4.1.3. Role Claims Configuration

**Token Claim:**
```json
{
  "role": ["Editor", "Viewer"]
}
```

**Or:**
```json
{
  "roles": "Editor,Admin"
}
```

**Claim Mapping Configuration:**
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    RoleClaimType = "role", // Map to "role" claim in token
    // ... other parameters
};
```

### 4.2. Policy-Based Authorization

#### 4.2.1. Policy Definition

**Purpose:** Fine-grained, flexible authorization based on multiple factors

**Policy Components:**
- Requirements: Conditions that must be met
- Handlers: Logic evaluating requirements
- Context: Information about the request and user

#### 4.2.2. Policy Configuration

**Program.cs:**
```csharp
builder.Services.AddAuthorization(options =>
{
    // Scope-based policies
    options.AddPolicy("Catalog.Read", policy =>
        policy.RequireClaim("scope", "catalog.read"));

    options.AddPolicy("Catalog.Write", policy =>
        policy.RequireClaim("scope", "catalog.write"));

    options.AddPolicy("Catalog.Delete", policy =>
        policy.RequireClaim("scope", "catalog.delete"));

    // Role-based policy
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Combined requirements
    options.AddPolicy("Catalog.Admin", policy =>
        policy
            .RequireRole("Admin")
            .RequireClaim("scope", "catalog.admin"));

    // Custom requirement
    options.AddPolicy("TenantOwner", policy =>
        policy.Requirements.Add(new TenantOwnerRequirement()));
});
```

#### 4.2.3. Policy Usage in Controllers

```csharp
[ApiController]
[Route("api/v1/catalog/products")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Catalog.Read")]
    public async Task<IActionResult> GetProducts()
    {
        // Requires catalog.read scope
    }

    [HttpPost]
    [Authorize(Policy = "Catalog.Write")]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        // Requires catalog.write scope
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Catalog.Delete")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        // Requires catalog.delete scope
    }
}
```

#### 4.2.4. Custom Authorization Requirements

**Requirement Definition:**
```csharp
public class TenantOwnerRequirement : IAuthorizationRequirement
{
    // Empty marker interface implementation
}
```

**Handler Implementation:**
```csharp
public class TenantOwnerHandler : AuthorizationHandler<TenantOwnerRequirement>
{
    private readonly ITenantService _tenantService;

    public TenantOwnerHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantOwnerRequirement requirement)
    {
        var tenantIdClaim = context.User.FindFirst("tenantId")?.Value;
        
        if (string.IsNullOrEmpty(tenantIdClaim))
        {
            context.Fail();
            return;
        }

        // Verify user owns the tenant
        var isOwner = await _tenantService.IsOwnerAsync(
            context.User.FindFirst("sub")?.Value,
            tenantIdClaim);

        if (isOwner)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
```

**Handler Registration:**
```csharp
services.AddSingleton<IAuthorizationHandler, TenantOwnerHandler>();
```

### 4.3. Claims-Based Authorization

#### 4.3.1. Claims Overview

**Definition:** Claims are statements about a subject (user) made by an issuer.

**Common Claims:**

| Claim Type | Example Value | Usage |
|------------|---------------|-------|
| sub (Subject) | user-123456 | Unique user identifier |
| email | user@example.com | User email address |
| name | John Doe | User display name |
| role | Admin | User role |
| scope | catalog.read | OAuth 2.0 scope |
| tenantId | tenant-abc | Multi-tenant identifier |
| department | Engineering | Organizational unit |
| subscription | Premium | Subscription tier |

#### 4.3.2. Claims Access in Code

**Controller Access:**
```csharp
[ApiController]
[Authorize]
public class UserContextController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        // Access claims from User.Claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var tenantId = User.FindFirstValue("tenantId");
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

        return Ok(new
        {
            UserId = userId,
            Email = email,
            TenantId = tenantId,
            Roles = roles
        });
    }
}
```

**Service Layer Access:**
```csharp
public class ProductService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Product> GetProductAsync(Guid id)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var tenantId = user?.FindFirstValue("tenantId");

        // Filter by tenant
        return await _repository.GetByIdAsync(id, tenantId);
    }
}
```

#### 4.3.3. Claims Transformation

**Purpose:** Enrich or modify claims after token validation

**Implementation:**
```csharp
public class CustomClaimsTransformation : IClaimsTransformation
{
    private readonly IUserPermissionService _permissionService;

    public CustomClaimsTransformation(IUserPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity;
        var userId = principal.FindFirstValue("sub");

        // Load additional permissions from database
        var permissions = await _permissionService.GetUserPermissionsAsync(userId);

        foreach (var permission in permissions)
        {
            identity.AddClaim(new Claim("permission", permission));
        }

        return principal;
    }
}
```

**Registration:**
```csharp
services.AddScoped<IClaimsTransformation, CustomClaimsTransformation>();
```

---

## 5. Token Management and Validation

### 5.1. Token Validation Requirements

#### 5.1.1. Mandatory Validations

| Validation | Check | Failure Action |
|------------|-------|----------------|
| Signature | Verify cryptographic signature | Reject (401) |
| Issuer | Verify iss claim matches expected | Reject (401) |
| Audience | Verify aud claim matches API | Reject (401) |
| Expiration | Verify exp claim > current time | Reject (401) |
| Not Before | Verify nbf claim <= current time | Reject (401) |
| Format | Valid JWT structure | Reject (401) |

#### 5.1.2. Token Validation Flow

```
┌─────────────────────────────────────────────────────────┐
│  1. Extract Token from Authorization Header             │
└────────────────┬────────────────────────────────────────┘
                 ↓
┌────────────────┴────────────────────────────────────────┐
│  2. Parse JWT Structure (Header.Payload.Signature)      │
└────────────────┬────────────────────────────────────────┘
                 ↓
┌────────────────┴────────────────────────────────────────┐
│  3. Retrieve Signing Keys from Authority (JWKS)         │
└────────────────┬────────────────────────────────────────┘
                 ↓
┌────────────────┴────────────────────────────────────────┐
│  4. Verify Signature                                    │
│     - RSA256, ES256, etc.                               │
└────────────────┬────────────────────────────────────────┘
                 ↓
┌────────────────┴────────────────────────────────────────┐
│  5. Validate Claims                                     │
│     - iss (Issuer)                                      │
│     - aud (Audience)                                    │
│     - exp (Expiration)                                  │
│     - nbf (Not Before)                                  │
└────────────────┬────────────────────────────────────────┘
                 ↓
┌────────────────┴────────────────────────────────────────┐
│  6. Extract Claims and Create ClaimsPrincipal           │
└────────────────┬────────────────────────────────────────┘
                 ↓
┌────────────────┴────────────────────────────────────────┐
│  7. Attach Principal to HttpContext.User                │
└─────────────────────────────────────────────────────────┘
```

### 5.2. Token Lifetime Management

#### 5.2.1. Recommended Token Lifetimes

| Token Type | Recommended Lifetime | Rationale |
|------------|---------------------|-----------|
| Access Token | 15-60 minutes | Balance between security and usability |
| Refresh Token | 7-30 days | Long-lived sessions with rotation |
| ID Token | 15-60 minutes | Matches access token |
| Client Credentials Token | 1-24 hours | Service-to-service scenarios |

#### 5.2.2. Clock Skew Handling

**Configuration:**
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ClockSkew = TimeSpan.FromMinutes(5), // Allow 5 minutes skew
    // ... other parameters
};
```

**Rationale:** Compensates for time synchronization differences between servers

### 5.3. Token Revocation

#### 5.3.1. Revocation Strategies

**Strategy 1: Short-Lived Tokens (Recommended)**
- Access tokens expire quickly (15-60 minutes)
- No active revocation needed
- Revocation effective within token lifetime

**Strategy 2: Token Introspection**
- Check token validity with authorization server on each request
- Performance impact: additional network call
- Real-time revocation support

**Strategy 3: Revocation List**
- Maintain list of revoked token IDs (jti claim)
- Check against list during validation
- Requires distributed cache (Redis)

**Implementation (Revocation List):**
```csharp
public class TokenRevocationService
{
    private readonly IDistributedCache _cache;

    public async Task RevokeTokenAsync(string tokenId, TimeSpan expiration)
    {
        await _cache.SetStringAsync(
            $"revoked:token:{tokenId}",
            "true",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(expiration)
            });
    }

    public async Task<bool> IsTokenRevokedAsync(string tokenId)
    {
        var result = await _cache.GetStringAsync($"revoked:token:{tokenId}");
        return result == "true";
    }
}
```

---

## 6. Endpoint Security Patterns

### 6.1. Public Endpoints

#### 6.1.1. Anonymous Access

**Use Cases:**
- Public API documentation
- Health check endpoints
- Public catalog or content

**Implementation:**
```csharp
[ApiController]
[Route("api/v1/public")]
public class PublicController : ControllerBase
{
    [HttpGet("catalog")]
    [AllowAnonymous] // Explicitly allow anonymous access
    public async Task<IActionResult> GetPublicCatalog()
    {
        // No authentication required
        var products = await _catalogService.GetPublicProductsAsync();
        return Ok(products);
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "healthy" });
    }
}
```

**Security Consideration:** Even public endpoints should implement rate limiting to prevent abuse.

### 6.2. Authenticated Endpoints

#### 6.2.1. Read Operations

**Pattern:** Require authentication, potentially different permissions for different data

```csharp
[HttpGet]
[Authorize(Policy = "Catalog.Read")]
public async Task<IActionResult> GetProducts()
{
    var tenantId = User.FindFirstValue("tenantId");
    var products = await _catalogService.GetProductsAsync(tenantId);
    return Ok(products);
}
```

#### 6.2.2. Write Operations

**Pattern:** Always require authentication and appropriate write permissions

```csharp
[HttpPost]
[Authorize(Policy = "Catalog.Write")]
public async Task<IActionResult> CreateProduct(CreateProductRequest request)
{
    var userId = User.FindFirstValue("sub");
    var tenantId = User.FindFirstValue("tenantId");
    
    var command = new CreateProductCommand
    {
        Name = request.Name,
        Price = request.Price,
        CreatedBy = userId,
        TenantId = tenantId
    };

    var result = await _mediator.Send(command);
    
    return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
}
```

#### 6.2.3. Delete Operations

**Pattern:** Highest privilege requirement, often admin-only

```csharp
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteProduct(Guid id)
{
    var tenantId = User.FindFirstValue("tenantId");
    
    var command = new DeleteProductCommand
    {
        ProductId = id,
        TenantId = tenantId
    };

    await _mediator.Send(command);
    
    return NoContent();
}
```

### 6.3. Resource-Level Authorization

#### 6.3.1. Ownership Verification

**Pattern:** Verify user owns or has access to specific resource

```csharp
[HttpPut("{id}")]
[Authorize]
public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductRequest request)
{
    var userId = User.FindFirstValue("sub");
    var tenantId = User.FindFirstValue("tenantId");

    // Retrieve product
    var product = await _productRepository.GetByIdAsync(id);

    if (product == null)
        return NotFound();

    // Verify ownership
    if (product.TenantId != tenantId)
        return Forbid(); // 403 Forbidden

    // Additional authorization check
    if (product.CreatedBy != userId && !User.IsInRole("Admin"))
        return Forbid();

    // Proceed with update
    var command = new UpdateProductCommand
    {
        ProductId = id,
        Name = request.Name,
        Price = request.Price
    };

    await _mediator.Send(command);
    
    return Ok();
}
```

---

## 7. Multi-Tenancy Security

### 7.1. Tenant Identification

#### 7.1.1. Tenant Resolution Strategies

**Strategy 1: Token Claim (Recommended)**
```json
{
  "sub": "user-123",
  "tenantId": "tenant-abc"
}
```

**Strategy 2: Path Parameter**
```
GET /api/v1/tenants/{tenantId}/products
```

**Strategy 3: Subdomain**
```
https://tenant-abc.api.example.com/api/v1/products
```

**Strategy 4: Header**
```http
X-Tenant-ID: tenant-abc
```

#### 7.1.2. Tenant Context Implementation

**Tenant Context Service:**
```csharp
public interface ITenantContext
{
    string TenantId { get; }
    bool IsResolved { get; }
}

public class TenantContext : ITenantContext
{
    public string TenantId { get; private set; }
    public bool IsResolved => !string.IsNullOrEmpty(TenantId);

    public void SetTenant(string tenantId)
    {
        if (IsResolved)
            throw new InvalidOperationException("Tenant already resolved");

        TenantId = tenantId;
    }
}
```

**Registration:**
```csharp
services.AddScoped<ITenantContext, TenantContext>();
```

#### 7.1.3. Tenant Resolution Middleware

```csharp
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContext tenantContext)
    {
        // Extract tenant from token claim
        var tenantId = context.User.FindFirstValue("tenantId");

        if (!string.IsNullOrEmpty(tenantId))
        {
            ((TenantContext)tenantContext).SetTenant(tenantId);
        }
        else if (context.User.Identity?.IsAuthenticated == true)
        {
            // User authenticated but no tenant claim
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                title = "Forbidden",
                status = 403,
                detail = "No tenant context available for this user."
            });
            return;
        }

        await _next(context);
    }
}
```

**Registration:**
```csharp
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantResolutionMiddleware>(); // After auth
```

### 7.2. Tenant Data Isolation

#### 7.2.1. Query Filtering

**Automatic Tenant Filtering:**
```csharp
public class TenantFilteredRepository<T> : IRepository<T> where T : ITenantEntity
{
    private readonly DbContext _context;
    private readonly ITenantContext _tenantContext;

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>()
            .Where(e => e.TenantId == _tenantContext.TenantId)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>()
            .Where(e => e.TenantId == _tenantContext.TenantId)
            .ToListAsync();
    }
}
```

#### 7.2.2. Global Query Filters (EF Core)

```csharp
public class ApplicationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply tenant filter globally
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => p.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<Order>()
            .HasQueryFilter(o => o.TenantId == _tenantContext.TenantId);
    }
}
```

#### 7.2.3. Tenant Validation on Write

```csharp
public async Task<Product> CreateProductAsync(Product product)
{
    // Enforce tenant context on create
    product.TenantId = _tenantContext.TenantId;

    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    return product;
}

public async Task UpdateProductAsync(Product product)
{
    // Verify tenant ownership before update
    if (product.TenantId != _tenantContext.TenantId)
        throw new UnauthorizedAccessException("Tenant mismatch");

    _context.Products.Update(product);
    await _context.SaveChangesAsync();
}
```

---

## 8. Error Handling for Authentication and Authorization

### 8.1. Authentication Errors

#### 8.1.1. Unauthorized (401)

**Scenario:** Missing, invalid, or expired token

**HTTP Response:**
```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer error="invalid_token", error_description="The token is expired"
Content-Type: application/json; charset=utf-8
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication credentials are required or invalid.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

**WWW-Authenticate Header Values:**

| Error Code | Description |
|------------|-------------|
| invalid_token | Token is malformed, expired, or invalid |
| invalid_request | Missing or malformed Authorization header |
| insufficient_scope | Token lacks required scope |

### 8.2. Authorization Errors

#### 8.2.1. Forbidden (403)

**Scenario:** Authenticated but insufficient permissions

**HTTP Response:**
```http
HTTP/1.1 403 Forbidden
Content-Type: application/json; charset=utf-8
```

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have permission to perform this action.",
  "traceId": "00-550e8400e29b41d4a716446655440000-01",
  "requiredPermission": "catalog.delete"
}
```

### 8.3. Custom Authorization Failure Handling

```csharp
public class CustomAuthorizationMiddlewareResultHandler 
    : IAuthorizationMiddlewareResultHandler
{
    private readonly IAuthorizationMiddlewareResultHandler _defaultHandler;
    private readonly ILogger<CustomAuthorizationMiddlewareResultHandler> _logger;

    public CustomAuthorizationMiddlewareResultHandler(
        IAuthorizationMiddlewareResultHandler defaultHandler,
        ILogger<CustomAuthorizationMiddlewareResultHandler> logger)
    {
        _defaultHandler = defaultHandler;
        _logger = logger;
    }

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Succeeded)
        {
            var userId = context.User.FindFirstValue("sub");
            var endpoint = context.GetEndpoint()?.DisplayName;

            _logger.LogWarning(
                "Authorization failed for user {UserId} accessing {Endpoint}. " +
                "Failed requirements: {FailedRequirements}",
                userId,
                endpoint,
                string.Join(", ", authorizeResult.AuthorizationFailure?.FailedRequirements
                    .Select(r => r.GetType().Name) ?? Enumerable.Empty<string>()));
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
```

---

## 9. Identity Provider Integration

### 9.1. Azure Active Directory (Azure AD / Entra ID)

#### 9.1.1. Configuration

**appsettings.json:**
```json
{
  "Authentication": {
    "Authority": "https://login.microsoftonline.com/{tenant-id}",
    "Audience": "api://your-api-identifier",
    "Issuer": "https://login.microsoftonline.com/{tenant-id}/v2.0"
  }
}
```

**Program.cs:**
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        configuration.Bind("Authentication", options);
        options.TokenValidationParameters.RoleClaimType = "roles";
    },
    options =>
    {
        configuration.Bind("Authentication", options);
    });
```

### 9.2. IdentityServer

#### 9.2.1. Configuration

**appsettings.json:**
```json
{
  "Authentication": {
    "Authority": "https://identity.example.com",
    "Audience": "catalog-api",
    "RequireHttpsMetadata": true
  }
}
```

### 9.3. Auth0

#### 9.3.1. Configuration

**appsettings.json:**
```json
{
  "Authentication": {
    "Authority": "https://your-tenant.auth0.com/",
    "Audience": "https://api.example.com"
  }
}
```

---

## 10. Security Testing Requirements

### 10.1. Authentication Tests

#### 10.1.1. Unit Tests

```csharp
[Fact]
public async Task GetProducts_NoToken_Returns401()
{
    // Arrange
    var client = _factory.CreateClient();
    // No Authorization header

    // Act
    var response = await client.GetAsync("/api/v1/products");

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}

[Fact]
public async Task GetProducts_InvalidToken_Returns401()
{
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "invalid-token");

    // Act
    var response = await client.GetAsync("/api/v1/products");

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}

[Fact]
public async Task GetProducts_ValidToken_Returns200()
{
    // Arrange
    var client = _factory.CreateClient();
    var token = GenerateTestToken(claims: new[]
    {
        new Claim("sub", "test-user"),
        new Claim("scope", "catalog.read")
    });
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);

    // Act
    var response = await client.GetAsync("/api/v1/products");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### 10.2. Authorization Tests

#### 10.2.1. Role Tests

```csharp
[Fact]
public async Task DeleteProduct_EditorRole_Returns403()
{
    // Arrange
    var token = GenerateTestToken(roles: new[] { "Editor" });
    var client = CreateAuthenticatedClient(token);

    // Act
    var response = await client.DeleteAsync("/api/v1/products/test-id");

    // Assert - Editor cannot delete
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}

[Fact]
public async Task DeleteProduct_AdminRole_Returns204()
{
    // Arrange
    var token = GenerateTestToken(roles: new[] { "Admin" });
    var client = CreateAuthenticatedClient(token);

    // Act
    var response = await client.DeleteAsync("/api/v1/products/test-id");

    // Assert - Admin can delete
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
}
```

### 10.3. Security Scanning

#### 10.3.1. OWASP ZAP Integration

**Automated Security Scan:**
- Scan authentication endpoints
- Test for common vulnerabilities
- Verify security headers
- Test token validation

#### 10.3.2. Static Analysis

**Tools:**
- SonarQube security rules
- Roslyn security analyzers
- .NET Security Guard

---

## 11. Environment-Specific Security Configuration

### 11.1. Development Environment

**Characteristics:**
- Relaxed security for developer productivity
- Test identity providers acceptable
- Detailed logging enabled
- HTTPS not strictly enforced (localhost)

**Configuration:**
```json
{
  "Authentication": {
    "Authority": "https://localhost:5001",
    "RequireHttpsMetadata": false,
    "ValidateLifetime": true,
    "ClockSkew": "00:05:00"
  }
}
```

### 11.2. Staging Environment

**Characteristics:**
- Production-like security
- HTTPS required
- Production identity provider (test tenant)
- Audit logging enabled

**Configuration:**
```json
{
  "Authentication": {
    "Authority": "https://auth.staging.example.com",
    "RequireHttpsMetadata": true,
    "ValidateLifetime": true,
    "ClockSkew": "00:05:00"
  }
}
```

### 11.3. Production Environment

**Characteristics:**
- Maximum security enforcement
- HTTPS mandatory
- Short token lifetimes
- Comprehensive audit logging
- Key rotation policies
- Certificate pinning (if applicable)

**Configuration:**
```json
{
  "Authentication": {
    "Authority": "https://auth.example.com",
    "RequireHttpsMetadata": true,
    "ValidateLifetime": true,
    "ClockSkew": "00:02:00"
  },
  "Security": {
    "EnforceHttps": true,
    "RequireAuthenticatedConnections": true,
    "TokenLifetime": "00:15:00"
  }
}
```

---

## 12. Monitoring and Audit Requirements

### 12.1. Authentication Events

#### 12.1.1. Events to Log

| Event | Severity | Data to Capture |
|-------|----------|-----------------|
| Successful authentication | Information | User ID, IP address, timestamp |
| Failed authentication | Warning | Reason, IP address, timestamp, attempted user |
| Token validation failure | Warning | Token ID (if available), reason, IP |
| Authorization failure | Warning | User ID, resource, required permission |
| Successful authorization | Debug | User ID, resource, granted permission |

#### 12.1.2. Logging Implementation

```csharp
options.Events = new JwtBearerEvents
{
    OnTokenValidated = context =>
    {
        var userId = context.Principal?.FindFirst("sub")?.Value;
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress;

        Log.Information(
            "Token validated successfully. UserId: {UserId}, IP: {IpAddress}",
            userId,
            ipAddress);

        return Task.CompletedTask;
    },
    OnAuthenticationFailed = context =>
    {
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress;

        Log.Warning(
            context.Exception,
            "Authentication failed. IP: {IpAddress}, Reason: {Reason}",
            ipAddress,
            context.Exception.Message);

        return Task.CompletedTask;
    },
    OnChallenge = context =>
    {
        Log.Warning(
            "Authentication challenge issued. Path: {Path}, Error: {Error}",
            context.Request.Path,
            context.Error);

        return Task.CompletedTask;
    }
};
```

### 12.2. Audit Trail Requirements

#### 12.2.1. Required Audit Information

**For Each Authenticated Request:**
- User identifier (sub claim)
- Tenant identifier (if multi-tenant)
- Request timestamp
- HTTP method and path
- Response status code
- Duration
- IP address
- User agent

**For Sensitive Operations:**
- Resource identifier
- Operation type (create, update, delete)
- Previous state (for updates)
- New state (for creates/updates)

---

## 13. Glossary

**Access Token:** A credential used to access protected resources, typically short-lived.

**Authorization Code:** A temporary code returned after user authentication, exchanged for tokens.

**Bearer Token:** A token that grants access to the bearer (holder) without further proof of identity.

**Claim:** A piece of information about a subject (user) asserted by an issuer.

**Client Credentials Flow:** OAuth 2.0 grant type for machine-to-machine authentication.

**Identity Provider (IdP):** A service that authenticates users and issues tokens (Azure AD, Auth0, etc.).

**JWT (JSON Web Token):** A compact, URL-safe token format containing claims, digitally signed.

**OAuth 2.0:** Authorization framework enabling third-party applications to access resources.

**OpenID Connect:** Identity layer built on OAuth 2.0 for user authentication.

**PKCE (Proof Key for Code Exchange):** Security extension for OAuth 2.0 authorization code flow.

**Policy-Based Authorization:** Authorization model evaluating policies with multiple requirements.

**RBAC (Role-Based Access Control):** Authorization model based on user roles.

**Refresh Token:** Long-lived token used to obtain new access tokens without re-authentication.

**Scope:** OAuth 2.0 mechanism defining access level or permission.

---

## 14. Recommendations and Next Steps

### 14.1. For Development Teams

#### 14.1.1. Implementation Checklist

**Authentication Setup:**
- [ ] Configure JWT bearer authentication
- [ ] Set appropriate token validation parameters
- [ ] Implement HTTPS enforcement (production)
- [ ] Add authentication event logging
- [ ] Test token validation with valid/invalid/expired tokens

**Authorization Setup:**
- [ ] Define roles for application
- [ ] Create authorization policies
- [ ] Apply [Authorize] attributes to controllers
- [ ] Implement resource-level authorization checks
- [ ] Test role and policy enforcement

**Multi-Tenancy (if applicable):**
- [ ] Implement tenant context service
- [ ] Create tenant resolution middleware
- [ ] Add tenant filtering to repositories
- [ ] Test tenant isolation thoroughly

### 14.2. For Security Teams

#### 14.2.1. Security Review Checklist

**Configuration Review:**
- [ ] Verify HTTPS enforced in production
- [ ] Confirm appropriate token lifetimes
- [ ] Review clock skew tolerance
- [ ] Validate signing key rotation procedures

**Code Review:**
- [ ] All sensitive endpoints have [Authorize]
- [ ] No hardcoded credentials or secrets
- [ ] Proper error handling without information disclosure
- [ ] Audit logging implemented correctly

**Penetration Testing:**
- [ ] Test with missing tokens
- [ ] Test with expired tokens
- [ ] Test with forged tokens
- [ ] Test authorization bypass attempts
- [ ] Test tenant isolation (if applicable)

### 14.3. For Operations Teams

#### 14.3.1. Operational Procedures

**Monitoring:**
- Configure alerts for authentication failure spikes
- Monitor token validation error rates
- Track authorization denial patterns
- Set up dashboards for security metrics

**Incident Response:**
- Define procedures for compromised tokens
- Document token revocation process
- Establish communication channels for security incidents
- Create runbooks for common security scenarios

### 14.4. Related Documentation

**Must Read:**
- API Guidelines (API-GUIDELINES-001)
- Error Handling Specification (API-ERRORHANDLING-001)
- High-Level Architecture (ARCH-HL-001)

**Recommended Reading:**
- OAuth 2.0 RFC 6749
- OpenID Connect Core 1.0 Specification
- JWT RFC 7519
- OWASP API Security Top 10

---

## 15. References

### 15.1. Standards and Specifications

**OAuth 2.0 - RFC 6749**
- https://tools.ietf.org/html/rfc6749

**OpenID Connect Core 1.0**
- https://openid.net/specs/openid-connect-core-1_0.html

**JSON Web Token (JWT) - RFC 7519**
- https://tools.ietf.org/html/rfc7519

**PKCE - RFC 7636**
- https://tools.ietf.org/html/rfc7636

**Bearer Token Usage - RFC 6750**
- https://tools.ietf.org/html/rfc6750

### 15.2. Security Guidelines

**OWASP API Security Top 10**
- https://owasp.org/www-project-api-security/

**NIST Digital Identity Guidelines (SP 800-63B)**
- https://pages.nist.gov/800-63-3/sp800-63b.html

**Microsoft Identity Platform Best Practices**
- https://docs.microsoft.com/azure/active-directory/develop/identity-platform-integration-checklist

### 15.3. Framework Documentation

**ASP.NET Core Authentication**
- https://docs.microsoft.com/aspnet/core/security/authentication/

**ASP.NET Core Authorization**
- https://docs.microsoft.com/aspnet/core/security/authorization/

**Microsoft.Identity.Web**
- https://github.com/AzureAD/microsoft-identity-web

### 15.4. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- ARCH-HL-001: High-Level Architecture
- API-GUIDELINES-001: API Design Guidelines
- API-ERRORHANDLING-001: Error Handling and Problem Details
- API-VERSIONING-001: API Versioning Strategy

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial authentication and authorization specification with standardized structure and comprehensive security controls |

---

**END OF DOCUMENT**