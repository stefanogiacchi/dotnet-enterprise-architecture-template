# Resource Naming and URL Structure Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | API-NAMING-001 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Standards and Guidelines |
| Target Audience | API Designers, Development Teams, Solution Architects, Technical Leads, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Annual |
| Related Documents | API Guidelines (API-GUIDELINES-001), Versioning (API-VERSIONING-001), CQRS (ARCH-CQRS-003) |
| Prerequisites | Understanding of REST principles, HTTP methods, URI structure |

---

## Executive Summary

This document establishes comprehensive resource naming conventions and URL structure standards for the .NET Enterprise Architecture Template, ensuring predictable, intuitive, and maintainable API endpoints. These conventions align with REST architectural principles, industry best practices, and OpenAPI specifications, providing a foundation for consistent API design across all enterprise applications.

**Strategic Business Value:**
- Reduced integration complexity through predictable URL patterns
- Accelerated development via standardized naming conventions
- Improved API discoverability and developer experience
- Enhanced long-term maintainability through stable URL structures
- Decreased documentation costs via self-documenting URLs
- Better search engine optimization for public APIs

**Key Technical Capabilities:**
- Lowercase kebab-case naming convention for all resources
- Hierarchical URL structure reflecting domain ownership
- Plural nouns for collection resources
- GUID-based resource identifiers for security and uniqueness
- RESTful state transition patterns using HTTP methods
- Clean separation between resources and actions
- Version-prefixed URL structure

**Compliance and Standards:**
- REST architectural constraints (Roy Fielding)
- URI Generic Syntax (RFC 3986)
- HTTP/1.1 specification (RFC 7230-7235)
- OpenAPI Specification 3.0 compatibility
- ISO/IEC 25010 usability characteristics

---

## Table of Contents

1. Introduction and Scope
2. Core Design Principles
3. Resource Naming Conventions
4. URL Structure Standards
5. Resource Identifier Specifications
6. Hierarchical Relationship Patterns
7. HTTP Method to Resource Mapping
8. State Transition Patterns
9. Query Parameter vs Path Parameter Guidelines
10. Versioning Integration
11. Security Considerations
12. Anti-Patterns and Common Mistakes
13. Validation and Compliance
14. Glossary
15. Recommendations and Next Steps
16. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides authoritative specifications for resource naming and URL structure within the .NET Enterprise Architecture Template. It defines mandatory naming conventions, URL patterns, hierarchical structures, and resource identification standards ensuring consistent, predictable, and RESTful API design across all enterprise applications.

### 1.2. Scope

**In Scope:**
- Resource naming conventions (case, format, pluralization)
- URL structure patterns and hierarchy
- Resource identifier format specifications
- HTTP method to resource operation mapping
- State transition URL patterns
- Relationship representation in URLs
- Versioning integration with URL structure
- Security considerations for URL design
- Anti-pattern identification and avoidance

**Out of Scope:**
- Query parameter specifications (covered in API-PAGINATION-001)
- Request/response body formats
- Authentication mechanisms (covered in API-AUTH-001)
- Error response formats (covered in API-ERRORHANDLING-001)
- API versioning strategy (covered in API-VERSIONING-001)
- Specific business domain modeling

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**API Designers:** URL structure design and resource modeling

**Development Teams:** Implementation standards and code examples

**Solution Architects:** Architectural governance and compliance verification

**Technical Leads:** Code review criteria and pattern enforcement

**Technical Writers:** API documentation standards

**C-Level Executives:** Business value and standardization benefits

### 1.4. Benefits of Standardized Naming

**Developer Benefits:**
- Intuitive URL structures reducing learning curve
- Predictable patterns accelerating development
- Self-documenting APIs reducing documentation needs

**Business Benefits:**
- Reduced integration costs
- Faster time-to-market
- Improved partner/client satisfaction
- Lower maintenance costs

**Technical Benefits:**
- Consistent tooling integration
- Simplified automated testing
- Better API discoverability
- Enhanced caching capabilities

---

## 2. Core Design Principles

### 2.1. Resource-Oriented Architecture

#### 2.1.1. Resources Not Actions

**Principle:** URLs represent resources (nouns), not actions (verbs).

**Rationale:** REST architectural style centers on resources and their representations, with operations expressed through HTTP methods.

**Correct Examples:**
```
GET    /api/v1/products
POST   /api/v1/products
GET    /api/v1/products/{id}
PUT    /api/v1/products/{id}
DELETE /api/v1/products/{id}
```

**Incorrect Examples:**
```
POST /api/v1/createProduct        # Action verb in URL
GET  /api/v1/getProducts           # Action verb in URL
POST /api/v1/updateProduct/{id}    # Action verb in URL
POST /api/v1/deleteProduct/{id}    # Action verb in URL
```

#### 2.1.2. Uniform Interface

**Principle:** Same resource accessed through consistent URL regardless of operation.

**Example:**
```
Resource: Product with ID abc-123

GET    /api/v1/products/abc-123   # Retrieve
PUT    /api/v1/products/abc-123   # Replace
PATCH  /api/v1/products/abc-123   # Update
DELETE /api/v1/products/abc-123   # Remove
```

### 2.2. Hierarchical Structure

#### 2.2.1. Domain-Driven Hierarchy

**Principle:** URL hierarchy reflects domain ownership and relationships, not technical implementation.

**Correct (Domain Ownership):**
```
/api/v1/orders/{orderId}/items
/api/v1/customers/{customerId}/addresses
/api/v1/projects/{projectId}/tasks
```

**Incorrect (Technical Relationships):**
```
/api/v1/database/orders/items               # Technical structure
/api/v1/tables/customers/addresses          # Technical structure
```

#### 2.2.2. Shallow Hierarchy

**Principle:** Limit URL nesting to maximum 2-3 levels for maintainability.

**Acceptable:**
```
/api/v1/orders/{orderId}/items/{itemId}     # 2 levels
```

**Problematic:**
```
/api/v1/orders/{orderId}/items/{itemId}/discounts/{discountId}/rules/{ruleId}  # Too deep
```

**Alternative:**
```
/api/v1/order-item-discounts/{discountId}/rules
```

### 2.3. Predictability

#### 2.3.1. Consistent Patterns

**Principle:** Apply same conventions across all resources.

**Pattern Consistency:**
```
/api/v1/products
/api/v1/orders
/api/v1/customers
/api/v1/categories
```

**Not:**
```
/api/v1/products          # Lowercase, plural
/api/v1/Order             # PascalCase, singular  ✗
/api/v1/customer_data     # snake_case, plural  ✗
```

### 2.4. Stability

#### 2.4.1. URL Permanence

**Principle:** URLs should remain stable across application lifetime.

**Stable URL Characteristics:**
- Domain concept naming (not implementation details)
- Version prefix isolation
- No business logic encoding
- No technical detail exposure

**Versioning for Breaking Changes:**
```
/api/v1/products/{id}     # Version 1
/api/v2/products/{id}     # Version 2 (breaking changes)
```

### 2.5. Simplicity

#### 2.5.1. Minimal Complexity

**Principle:** Keep URLs as simple as possible while maintaining clarity.

**Good:**
```
/api/v1/products
/api/v1/products/{id}
```

**Unnecessarily Complex:**
```
/api/v1.0/resources/products/collection
/api/v1/products/resource/{id}/representation
```

---

## 3. Resource Naming Conventions

### 3.1. Case Convention

#### 3.1.1. Lowercase Kebab-Case

**Standard:** All URL segments use lowercase letters with hyphens separating words.

**Correct Examples:**
```
/api/v1/products
/api/v1/user-preferences
/api/v1/order-items
/api/v1/shipping-addresses
/api/v1/product-categories
```

**Incorrect Examples:**
```
/api/v1/Products              # PascalCase ✗
/api/v1/userPreferences       # camelCase ✗
/api/v1/user_preferences      # snake_case ✗
/api/v1/USER-PREFERENCES      # UPPERCASE ✗
```

**Rationale:**
- URLs are case-insensitive in many systems
- Lowercase prevents ambiguity
- Kebab-case improves readability
- Industry standard convention

#### 3.1.2. Case Consistency Matrix

| Style | Example | Correct |
|-------|---------|---------|
| Lowercase kebab-case | /user-preferences | ✓ Yes |
| camelCase | /userPreferences | ✗ No |
| PascalCase | /UserPreferences | ✗ No |
| snake_case | /user_preferences | ✗ No |
| UPPERCASE | /USER-PREFERENCES | ✗ No |

### 3.2. Pluralization Rules

#### 3.2.1. Collection Resources

**Rule:** Use plural nouns for collection endpoints.

**Examples:**
```
/api/v1/products              # Collection of products
/api/v1/orders                # Collection of orders
/api/v1/users                 # Collection of users
/api/v1/categories            # Collection of categories
/api/v1/addresses             # Collection of addresses
```

**Rationale:**
- Indicates collection semantics
- Consistent with English language
- Industry standard practice

#### 3.2.2. Individual Resources

**Rule:** Access individual resources through collection with identifier.

**Pattern:**
```
/api/v1/{collection}/{id}
```

**Examples:**
```
/api/v1/products/abc-123
/api/v1/orders/def-456
/api/v1/users/ghi-789
```

**Not:**
```
/api/v1/product/abc-123       # Singular collection ✗
```

#### 3.2.3. Irregular Plurals

**Handle Correctly:**

| Singular | Correct Plural | Incorrect |
|----------|---------------|-----------|
| person | people | persons ✗ |
| child | children | childs ✗ |
| category | categories | categorys ✗ |
| company | companies | companys ✗ |

### 3.3. Word Separation

#### 3.3.1. Multi-Word Resources

**Use Hyphens:**
```
/api/v1/user-preferences
/api/v1/order-items
/api/v1/shipping-addresses
/api/v1/product-categories
/api/v1/payment-methods
```

**Not Underscores:**
```
/api/v1/user_preferences      # snake_case ✗
/api/v1/order_items           # snake_case ✗
```

**Not Concatenation:**
```
/api/v1/userpreferences       # No separation ✗
/api/v1/orderitems            # No separation ✗
```

### 3.4. Abbreviation Guidelines

#### 3.4.1. Avoid Abbreviations

**Principle:** Use full words for clarity unless abbreviation is universally recognized.

**Acceptable Abbreviations:**
```
/api/v1/api-keys              # API widely understood
/api/v1/sms-notifications     # SMS widely understood
/api/v1/pdf-reports           # PDF widely understood
```

**Avoid:**
```
/api/v1/usr-prefs             # Unclear ✗
/api/v1/ord-itms              # Unclear ✗
/api/v1/prod-cats             # Unclear ✗
```

**Preferred:**
```
/api/v1/user-preferences      # Clear ✓
/api/v1/order-items           # Clear ✓
/api/v1/product-categories    # Clear ✓
```

---

## 4. URL Structure Standards

### 4.1. Complete URL Anatomy

#### 4.1.1. Standard Structure

**Format:**
```
{scheme}://{hostname}/api/v{version}/{resource-path}
```

**Example:**
```
https://api.example.com/api/v1/products/abc-123/reviews
```

**Components:**

| Component | Example | Required | Description |
|-----------|---------|----------|-------------|
| scheme | https | Yes | Protocol (always HTTPS in production) |
| hostname | api.example.com | Yes | API domain |
| api | api | Yes | API prefix |
| v{version} | v1 | Yes | API version |
| resource-path | products/abc-123/reviews | Yes | Resource hierarchy |

### 4.2. Base URL Pattern

#### 4.2.1. Standard Base URL

**Pattern:**
```
https://{hostname}/api/v{version}
```

**Examples:**
```
https://api.example.com/api/v1
https://api.staging.example.com/api/v1
https://api-dev.example.com/api/v1
```

### 4.3. Resource Path Construction

#### 4.3.1. Collection Path

**Pattern:**
```
/api/v1/{collection}
```

**Examples:**
```
/api/v1/products
/api/v1/orders
/api/v1/customers
```

#### 4.3.2. Individual Resource Path

**Pattern:**
```
/api/v1/{collection}/{id}
```

**Examples:**
```
/api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
/api/v1/orders/c7b0f5d4-8be3-5ea3-0c10-2e5f7g07a1g2
```

#### 4.3.3. Sub-Resource Path

**Pattern:**
```
/api/v1/{collection}/{id}/{sub-collection}
/api/v1/{collection}/{id}/{sub-collection}/{sub-id}
```

**Examples:**
```
/api/v1/orders/order-123/items
/api/v1/orders/order-123/items/item-456
/api/v1/customers/customer-789/addresses
/api/v1/customers/customer-789/addresses/address-101
```

### 4.4. Action Sub-Resources

#### 4.4.1. State Transition Pattern

**Pattern:**
```
POST /api/v1/{collection}/{id}/{action}
```

**Examples:**
```
POST /api/v1/orders/order-123/approve
POST /api/v1/orders/order-123/cancel
POST /api/v1/users/user-456/activate
POST /api/v1/products/product-789/publish
```

**Characteristics:**
- Always use POST method
- Action name is verb
- Represents state transition
- Non-idempotent operation

---

## 5. Resource Identifier Specifications

### 5.1. Identifier Format

#### 5.1.1. GUID Standard

**Requirement:** Use GUID (Globally Unique Identifier) for all resource identifiers.

**Format:** UUID Version 4 (random)

**Example:**
```
b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
```

**In URLs:**
```
/api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
```

#### 5.1.2. GUID Advantages

**Security:**
- Non-sequential prevents enumeration attacks
- No predictable patterns
- Large space reduces guessing probability

**Distribution:**
- Generated anywhere without coordination
- No database round-trip required
- Suitable for distributed systems

**Uniqueness:**
- Globally unique across all systems
- No collision risk
- Merge-friendly

### 5.2. Identifier Anti-Patterns

#### 5.2.1. Auto-Increment Integers

**Problem:**
```
/api/v1/products/1
/api/v1/products/2
/api/v1/products/3
```

**Issues:**
- Enumeration attacks (iterate all IDs)
- Information disclosure (ID reveals creation order)
- Distributed system complexity
- Merge conflicts

#### 5.2.2. Human-Readable Slugs

**Limited Use:**
```
/api/v1/products/wireless-keyboard-black
```

**Acceptable For:**
- Public content (blog posts, documentation)
- SEO-critical pages
- Human-friendly URLs

**Not Recommended For:**
- Transactional resources
- User-generated content
- Frequently updated resources

**Best Practice:**
```
/api/v1/products/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
```

### 5.3. Composite Identifiers

#### 5.3.1. Avoid Composite Keys in URLs

**Problem:**
```
/api/v1/order-items/order-123/item-456     # Composite key
```

**Preferred:**
```
/api/v1/order-items/item-abc-def-ghi       # Single GUID
```

**Rationale:**
- Simpler URL structure
- Single point of uniqueness
- Easier caching
- Better API ergonomics

---

## 6. Hierarchical Relationship Patterns

### 6.1. Parent-Child Relationships

#### 6.1.1. When to Use Hierarchy

**Use Nested URLs When:**
- Strong ownership relationship exists
- Child cannot exist without parent
- Child always accessed through parent context
- Relationship is fundamental to domain

**Example (Strong Ownership):**
```
/api/v1/orders/{orderId}/items
/api/v1/customers/{customerId}/addresses
/api/v1/projects/{projectId}/tasks
```

#### 6.1.2. When to Avoid Hierarchy

**Use Flat URLs When:**
- Resources independently accessible
- Weak or many-to-many relationships
- Cross-cutting concerns
- Resource shared across multiple parents

**Example (Independent Access):**
```
/api/v1/products                            # Not under category
/api/v1/users                               # Not under organization
/api/v1/tags                                # Not under any resource
```

### 6.2. Maximum Nesting Depth

#### 6.2.1. Two-Level Recommendation

**Recommended Maximum:**
```
/api/v1/{collection}/{id}/{sub-collection}/{sub-id}
```

**Example:**
```
/api/v1/orders/order-123/items/item-456
```

#### 6.2.2. Excessive Nesting Problem

**Avoid:**
```
/api/v1/orders/order-123/items/item-456/discounts/discount-789/rules/rule-101
```

**Refactor To:**
```
/api/v1/order-item-discounts/discount-789/rules/rule-101
```

**Benefits:**
- Simpler URLs
- Easier caching
- Better performance
- Reduced coupling

### 6.3. Sibling Resources

#### 6.3.1. Same-Level Resources

**Pattern:** Multiple collections at same level

**Example:**
```
/api/v1/products
/api/v1/categories
/api/v1/manufacturers
/api/v1/reviews
```

**Not:**
```
/api/v1/products
/api/v1/products/categories          # Categories not under products ✗
```

---

## 7. HTTP Method to Resource Mapping

### 7.1. Standard CRUD Operations

#### 7.1.1. Complete Mapping Table

| Operation | HTTP Method | URL Pattern | Example |
|-----------|-------------|-------------|---------|
| List | GET | /{collection} | GET /api/v1/products |
| Retrieve | GET | /{collection}/{id} | GET /api/v1/products/abc-123 |
| Create | POST | /{collection} | POST /api/v1/products |
| Replace | PUT | /{collection}/{id} | PUT /api/v1/products/abc-123 |
| Update | PATCH | /{collection}/{id} | PATCH /api/v1/products/abc-123 |
| Delete | DELETE | /{collection}/{id} | DELETE /api/v1/products/abc-123 |

#### 7.1.2. URL Consistency

**Same URL, Different Methods:**
```
GET    /api/v1/products/abc-123    # Read
PUT    /api/v1/products/abc-123    # Replace
PATCH  /api/v1/products/abc-123    # Update
DELETE /api/v1/products/abc-123    # Remove
```

**Key Principle:** HTTP method determines operation, URL identifies resource.

### 7.2. Custom Actions

#### 7.2.1. State Transition Actions

**Pattern:**
```
POST /api/v1/{collection}/{id}/{action}
```

**Examples:**
```
POST /api/v1/orders/order-123/approve
POST /api/v1/orders/order-123/cancel
POST /api/v1/orders/order-123/ship
POST /api/v1/users/user-456/activate
POST /api/v1/users/user-456/deactivate
POST /api/v1/documents/doc-789/publish
```

**Characteristics:**
- POST method (non-idempotent)
- Action verb in URL
- Represents state change
- May have side effects

---

## 8. State Transition Patterns

### 8.1. Action Sub-Resources

#### 8.1.1. When to Use Action Sub-Resources

**Use When:**
- Operation changes state
- Operation has side effects
- Operation not captured by standard CRUD
- Operation represents domain-specific workflow

**Examples:**

**Order Processing:**
```
POST /api/v1/orders/{id}/approve
POST /api/v1/orders/{id}/reject
POST /api/v1/orders/{id}/cancel
POST /api/v1/orders/{id}/ship
POST /api/v1/orders/{id}/complete
```

**User Management:**
```
POST /api/v1/users/{id}/activate
POST /api/v1/users/{id}/deactivate
POST /api/v1/users/{id}/reset-password
POST /api/v1/users/{id}/verify-email
```

**Document Workflow:**
```
POST /api/v1/documents/{id}/publish
POST /api/v1/documents/{id}/archive
POST /api/v1/documents/{id}/restore
```

#### 8.1.2. Request/Response Pattern

**Request:**
```http
POST /api/v1/orders/order-123/approve HTTP/1.1
Content-Type: application/json

{
  "approvedBy": "user-456",
  "comments": "Approved for processing"
}
```

**Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "orderId": "order-123",
  "status": "Approved",
  "approvedAt": "2025-01-15T10:30:00Z",
  "approvedBy": "user-456"
}
```

### 8.2. Alternative: PATCH with Status

#### 8.2.1. Status Field Update

**Alternative Pattern:**
```http
PATCH /api/v1/orders/order-123 HTTP/1.1
Content-Type: application/json

{
  "status": "Approved"
}
```

**When Acceptable:**
- Simple status transitions
- No additional business logic
- No side effects
- Direct state change

**When to Use Action Sub-Resource Instead:**
- Complex business rules
- Side effects (notifications, integrations)
- Domain-specific semantics
- Audit requirements

---

## 9. Query Parameter vs Path Parameter Guidelines

### 9.1. Path Parameters

#### 9.1.1. Use Path Parameters For

**Resource Identification:**
```
/api/v1/products/{productId}
/api/v1/orders/{orderId}/items/{itemId}
```

**Hierarchical Navigation:**
```
/api/v1/customers/{customerId}/orders
```

**Required Parameters:**
- Parameters that identify the resource
- Parameters that define the resource scope

### 9.2. Query Parameters

#### 9.2.1. Use Query Parameters For

**Filtering:**
```
/api/v1/products?category=electronics&minPrice=100
```

**Sorting:**
```
/api/v1/products?sort=price_desc
```

**Pagination:**
```
/api/v1/products?page=2&pageSize=20
```

**Search:**
```
/api/v1/products?search=wireless+keyboard
```

**Optional Parameters:**
- Parameters that modify behavior
- Parameters that filter results
- Parameters that control presentation

### 9.3. Anti-Pattern: Search in Path

**Incorrect:**
```
/api/v1/products/search/wireless-keyboard    # Search in path ✗
/api/v1/products/findByCategory/electronics  # Action in path ✗
```

**Correct:**
```
/api/v1/products?search=wireless-keyboard    # Search as query param ✓
/api/v1/products?category=electronics        # Filter as query param ✓
```

---

## 10. Versioning Integration

### 10.1. Version Placement

#### 10.1.1. Path-Based Versioning (Recommended)

**Pattern:**
```
/api/v{major}/{resource-path}
```

**Examples:**
```
/api/v1/products
/api/v2/products
/api/v3/products
```

**Placement Rules:**
- Version after /api prefix
- Version before resource path
- Never after resource

**Incorrect Placements:**
```
/api/products/v1                    # After resource ✗
/api/products?version=1             # Query parameter ✗
```

**Reference:** See API-VERSIONING-001 for complete versioning strategy.

### 10.2. Version Impact on URLs

#### 10.2.1. Breaking Changes

**Version 1:**
```
/api/v1/products/{id}
```

**Version 2 (Breaking Change):**
```
/api/v2/products/{id}
```

**Both Coexist:**
```
/api/v1/products/{id}    # Old version maintained
/api/v2/products/{id}    # New version available
```

---

## 11. Security Considerations

### 11.1. Identifier Security

#### 11.1.1. GUID vs Sequential IDs

**Security Issue with Sequential IDs:**
```
/api/v1/orders/1
/api/v1/orders/2
/api/v1/orders/3
```

**Vulnerabilities:**
- Enumeration attacks (iterate all IDs)
- Information disclosure (order count, timing)
- Predictability enables guessing

**Secure with GUIDs:**
```
/api/v1/orders/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
/api/v1/orders/c7b0f5d4-8be3-5ea3-0c10-2e5f7g07a1g2
```

**Benefits:**
- Non-enumerable
- Unpredictable
- No information disclosure

### 11.2. User-Scoped Resources

#### 11.2.1. Explicit User Context

**Secure Pattern:**
```
/api/v1/users/{userId}/sessions
/api/v1/users/{userId}/preferences
/api/v1/users/{userId}/orders
```

**Security Benefit:**
- Authorization check required
- Clear ownership
- Prevents cross-user access

**Insecure Alternative:**
```
/api/v1/sessions/{sessionId}          # Whose session? ✗
/api/v1/sessions?userId={userId}      # Session ID leak risk ✗
```

### 11.3. Sensitive Information in URLs

#### 11.3.1. Avoid Sensitive Data

**Never Include:**
- Passwords
- API keys
- Personal identifiable information (PII)
- Confidential business data

**Incorrect:**
```
/api/v1/users?password=secret123          # Password in URL ✗
/api/v1/users?ssn=123-45-6789             # PII in URL ✗
```

**Correct:**
```
POST /api/v1/users/authenticate           # Credentials in body ✓
Content-Type: application/json

{
  "username": "user@example.com",
  "password": "secret123"
}
```

**Rationale:**
- URLs logged in server logs
- URLs stored in browser history
- URLs visible in network traces

---

## 12. Anti-Patterns and Common Mistakes

### 12.1. Naming Anti-Patterns

#### 12.1.1. Action Verbs in URLs

**Incorrect:**
```
POST /api/v1/createProduct
POST /api/v1/updateProduct/{id}
GET  /api/v1/getProducts
POST /api/v1/deleteProduct/{id}
```

**Correct:**
```
POST   /api/v1/products
PUT    /api/v1/products/{id}
GET    /api/v1/products
DELETE /api/v1/products/{id}
```

#### 12.1.2. Inconsistent Case

**Incorrect:**
```
/api/v1/Products              # PascalCase
/api/v1/userPreferences       # camelCase
/api/v1/order_items           # snake_case
```

**Correct:**
```
/api/v1/products              # All lowercase kebab-case
/api/v1/user-preferences
/api/v1/order-items
```

#### 12.1.3. Technical Terminology

**Incorrect:**
```
/api/v1/database/products                  # Technical detail ✗
/api/v1/tables/orders                      # Technical detail ✗
/api/v1/entities/customers                 # Technical detail ✗
```

**Correct:**
```
/api/v1/products                           # Domain concept ✓
/api/v1/orders                             # Domain concept ✓
/api/v1/customers                          # Domain concept ✓
```

### 12.2. Structure Anti-Patterns

#### 12.2.1. Excessive Nesting

**Incorrect:**
```
/api/v1/customers/{cid}/orders/{oid}/items/{iid}/discounts/{did}/rules/{rid}
```

**Correct:**
```
/api/v1/order-item-discounts/{did}/rules/{rid}
```

#### 12.2.2. Business Logic in URLs

**Incorrect:**
```
/api/v1/products/status/active            # Status in path ✗
/api/v1/orders/price-range/100-500        # Range in path ✗
```

**Correct:**
```
/api/v1/products?status=active            # Status as filter ✓
/api/v1/orders?minPrice=100&maxPrice=500  # Range as filters ✓
```

### 12.3. Security Anti-Patterns

#### 12.3.1. Sequential Identifiers

**Incorrect:**
```
/api/v1/users/1
/api/v1/users/2
/api/v1/users/3
```

**Correct:**
```
/api/v1/users/b6a9e4c3-7ad2-4d92-9b09-1d4e6a6f90f1
```

#### 12.3.2. Cross-User Data Leakage

**Incorrect:**
```
/api/v1/sessions?userId={userId}          # Session enumeration risk ✗
```

**Correct:**
```
/api/v1/users/{userId}/sessions           # Explicit ownership ✓
```

---

## 13. Validation and Compliance

### 13.1. Naming Validation Rules

#### 13.1.1. Automated Validation

**Validation Checks:**

| Rule | Pattern | Example Valid | Example Invalid |
|------|---------|---------------|-----------------|
| Lowercase | ^[a-z0-9-/]+$ | /api/v1/products | /api/v1/Products |
| No underscores | No _ character | /user-preferences | /user_preferences |
| Plural collections | ends with 's' or special plural | /products | /product |
| GUID format | UUID v4 pattern | /{guid} | /123 |

#### 13.1.2. Linting Rules

**ESLint/Custom Rules:**
```javascript
{
  "rules": {
    "api-lowercase-urls": "error",
    "api-kebab-case": "error",
    "api-plural-collections": "error",
    "api-no-verbs": "error",
    "api-guid-identifiers": "error"
  }
}
```

### 13.2. Code Review Checklist

**URL Design Review Points:**
- [ ] All URL segments lowercase
- [ ] Kebab-case for multi-word resources
- [ ] Plural nouns for collections
- [ ] GUIDs for resource identifiers
- [ ] No action verbs in URLs (except state transitions)
- [ ] Maximum 2-3 levels of nesting
- [ ] Consistent versioning placement
- [ ] No sensitive data in URLs
- [ ] Domain terminology (not technical)
- [ ] RESTful HTTP method usage

---

## 14. Glossary

**Action Sub-Resource:** URL pattern representing state transition (e.g., /orders/{id}/approve).

**Collection Resource:** URL representing multiple resources (e.g., /products).

**GUID:** Globally Unique Identifier, 128-bit identifier (UUID format).

**Hierarchical URL:** URL structure reflecting parent-child relationships.

**Kebab-Case:** Lowercase words separated by hyphens (e.g., user-preferences).

**Path Parameter:** URL segment identifying a resource (e.g., {id} in /products/{id}).

**Query Parameter:** URL parameter for filtering, sorting, or pagination (e.g., ?page=1).

**Resource Identifier:** Unique identifier for a specific resource instance.

**RESTful:** Adhering to REST architectural principles.

**State Transition:** Operation changing resource state (approve, cancel, activate).

---

## 15. Recommendations and Next Steps

### 15.1. For Development Teams

**Implementation Checklist:**
- [ ] Adopt lowercase kebab-case for all URLs
- [ ] Use plural nouns for all collections
- [ ] Implement GUID-based resource identifiers
- [ ] Map HTTP methods correctly to operations
- [ ] Limit URL nesting to 2-3 levels maximum
- [ ] Use action sub-resources for state transitions
- [ ] Validate URL patterns in code reviews
- [ ] Document all resource naming decisions
- [ ] Implement automated URL validation

### 15.2. For API Designers

**Design Guidelines:**
- Start with domain model, not implementation
- Model resources as nouns, not operations
- Keep hierarchy shallow and meaningful
- Use consistent patterns across all endpoints
- Document resource relationships clearly
- Consider long-term URL stability

### 15.3. For Architects

**Governance Activities:**
- Establish URL naming standards
- Create validation tools
- Conduct periodic audits
- Maintain naming registry
- Enforce compliance through CI/CD

### 15.4. Related Documentation

**Must Read:**
- API Guidelines (API-GUIDELINES-001)
- Versioning Strategy (API-VERSIONING-001)
- CQRS Specification (ARCH-CQRS-003)
- Pagination Specification (API-PAGINATION-001)

---

## 16. References

### 16.1. Standards and Specifications

**REST Architectural Style**
- Roy Fielding's Dissertation: https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm

**URI Generic Syntax**
- RFC 3986: https://tools.ietf.org/html/rfc3986

**HTTP/1.1 Specification**
- RFC 7230-7235: https://tools.ietf.org/html/rfc7230

**OpenAPI Specification**
- https://swagger.io/specification/

### 16.2. Industry Guidelines

**Microsoft REST API Guidelines**
- https://github.com/microsoft/api-guidelines

**Google API Design Guide**
- https://cloud.google.com/apis/design

**Zalando RESTful API Guidelines**
- https://opensource.zalando.com/restful-api-guidelines/

### 16.3. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- API-GUIDELINES-001: API Design Guidelines
- API-VERSIONING-001: API Versioning Strategy
- ARCH-CQRS-003: Command Query Responsibility Segregation
- API-PAGINATION-001: Pagination, Filtering, and Sorting

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial resource naming and URL structure specification with comprehensive standards and guidelines |

---

**END OF DOCUMENT**