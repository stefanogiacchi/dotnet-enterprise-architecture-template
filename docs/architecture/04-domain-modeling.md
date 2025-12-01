# Domain Modeling Specification
**Domain-Driven Design Patterns - .NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | ARCH-DOMAIN-004 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Technical Specification |
| Target Audience | Domain Experts, Solution Architects, Senior Developers, Technical Leads, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Quarterly |
| Related Documents | High-Level Architecture (ARCH-HL-001), CQRS Pipeline (ARCH-CQRS-002), API Flow (ARCH-API-003) |
| Prerequisites | Understanding of Domain-Driven Design principles, Object-Oriented Programming |

---

## Executive Summary

This document specifies the domain modeling approach within the .NET Enterprise Architecture Template, applying Domain-Driven Design (DDD) tactical patterns in a pragmatic manner suitable for enterprise API development. The Domain layer represents the core business logic, encapsulating business rules, entities, value objects, and domain events while maintaining complete independence from infrastructure and framework concerns.

**Strategic Business Value:**
- Protection of business logic investment through infrastructure independence
- Enhanced maintainability via clear business rule encapsulation
- Reduced defect rates through invariant enforcement at the domain level
- Improved team communication through ubiquitous language implementation
- Long-term adaptability through framework-agnostic domain design

**Key Technical Capabilities:**
- Rich domain model with behavior encapsulation
- Entity and value object implementations following DDD patterns
- Aggregate root pattern for consistency boundary enforcement
- Domain event mechanism for decoupled side-effect management
- Business invariant validation at domain level
- Specification pattern for reusable business rules

**Compliance and Standards:**
- Aligned with SOLID principles (Single Responsibility, Dependency Inversion)
- Implements TOGAF 10 separation of concerns
- Follows Domain-Driven Design tactical patterns (Eric Evans)
- Supports ITIL 4 change management through domain event tracking
- Conforms to ISO/IEC/IEEE 26515:2018 documentation standards

---

## Table of Contents

1. Introduction and Scope
2. Domain Layer Architecture
3. Entity Pattern Specification
4. Value Object Pattern Specification
5. Aggregate and Aggregate Root Patterns
6. Domain Event Pattern Specification
7. Business Invariant Enforcement
8. Domain Services and Policies
9. Specification Pattern Implementation
10. Domain Layer Structure and Organization
11. Testing Strategy for Domain Logic
12. Domain Modeling Rules and Constraints
13. Performance Considerations
14. Glossary
15. Recommendations and Next Steps
16. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides comprehensive specifications for domain modeling within the .NET Enterprise Architecture Template. It defines tactical Domain-Driven Design patterns, implementation guidelines, and architectural constraints necessary for building maintainable, testable, and business-focused domain models that serve as the foundation of enterprise applications.

### 1.2. Scope

**In Scope:**
- Domain layer responsibilities and constraints
- Entity pattern implementation and guidelines
- Value object pattern implementation and guidelines
- Aggregate and aggregate root patterns
- Domain event definition and handling
- Business invariant enforcement strategies
- Domain service implementation patterns
- Specification pattern for business rules
- Domain model organization and structure
- Testing strategies for domain logic

**Out of Scope:**
- Infrastructure persistence implementations (EF Core configurations)
- Application layer orchestration patterns (covered in ARCH-CQRS-002)
- API endpoint implementations (covered in ARCH-API-003)
- Authentication and authorization logic
- External service integration patterns
- Caching strategies
- Deployment configurations

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**Domain Experts:** Business rule implementation and validation

**Solution Architects:** Domain modeling patterns and architectural governance

**Senior Developers:** Implementation guidelines and code examples

**Technical Leads:** Code review standards and pattern enforcement

**Development Teams:** Entity, value object, and aggregate implementation

**C-Level Executives:** Business value and risk mitigation through robust domain modeling

### 1.4. Architectural Context

The Domain layer occupies the center of the Clean Architecture, representing the business core:

```
┌─────────────────────────────────────────┐
│         API Layer                       │
│  (HTTP Concerns)                        │
└────────────────┬────────────────────────┘
                 ↓
┌────────────────┴────────────────────────┐
│      Application Layer                  │
│  (Use Case Orchestration)               │
└────────────────┬────────────────────────┘
                 ↓
┌────────────────┴────────────────────────┐
│      ╔══════════════════════════╗       │
│      ║    DOMAIN LAYER          ║       │
│      ║  • Entities              ║       │
│      ║  • Value Objects         ║       │
│      ║  • Aggregates            ║       │
│      ║  • Domain Events         ║       │
│      ║  • Business Rules        ║       │
│      ║  • Invariants            ║       │
│      ╚══════════════════════════╝       │
└────────────────┬────────────────────────┘
                 ↑
┌────────────────┴────────────────────────┐
│     Infrastructure Layer                │
│  (Persistence, External Services)       │
└─────────────────────────────────────────┘
```

**Key Characteristic:** The Domain layer has zero dependencies on any other layer or external framework.

---

## 2. Domain Layer Architecture

### 2.1. Domain Layer Responsibilities

#### 2.1.1. Primary Responsibilities

The Domain layer shall be responsible for:

**Business Logic Encapsulation:**
- Define and enforce business rules
- Implement domain-specific calculations
- Manage entity lifecycle and state transitions
- Validate business constraints

**Domain Model Definition:**
- Define entities with identity and behavior
- Define value objects for domain concepts
- Define aggregates as consistency boundaries
- Define domain events for significant business occurrences

**Invariant Enforcement:**
- Validate business constraints at construction
- Prevent invalid state transitions
- Ensure data consistency within aggregates
- Protect domain integrity

**Business Behavior:**
- Implement domain operations as entity methods
- Encapsulate business workflows within aggregates
- Express business rules through specifications
- Manage domain relationships

#### 2.1.2. Prohibited Activities

The Domain layer must not:

**Infrastructure Concerns:**
- Reference database frameworks (Entity Framework Core, Dapper)
- Contain SQL queries or database operations
- Reference external service SDKs
- Implement HTTP or messaging protocols

**Framework Dependencies:**
- Reference ASP.NET Core
- Use dependency injection frameworks
- Reference configuration libraries
- Use logging frameworks directly

**Application Concerns:**
- Orchestrate use cases (Application layer responsibility)
- Perform data mapping to DTOs
- Handle validation pipeline logic
- Manage transactions

**API Concerns:**
- Handle HTTP requests or responses
- Perform serialization
- Implement authentication or authorization
- Manage API versioning

### 2.2. Domain Layer Architecture Principles

#### 2.2.1. Persistence Ignorance

**Principle Statement:** Domain entities must remain unaware of persistence mechanisms.

**Implementation:**
- No data access annotations (e.g., [Key], [Column], [Table])
- No awareness of ORM behavior
- No change tracking dependencies
- No lazy loading expectations

**Benefits:**
- Pure business logic focus
- Easy testing without mocks
- Framework independence
- Migration flexibility

#### 2.2.2. Framework Independence

**Principle Statement:** Domain logic must not depend on any external framework.

**Implementation:**
- Zero framework references in Domain project
- Standard .NET types only
- Custom base classes for domain patterns
- Interface definitions for abstractions

**Benefits:**
- Long-term maintainability
- Technology stack flexibility
- Simplified testing
- Reduced coupling

#### 2.2.3. Rich Domain Model

**Principle Statement:** Domain entities should encapsulate behavior, not just data.

**Implementation:**
- Methods express business operations
- Private setters prevent invalid modifications
- Constructors enforce invariants
- State changes through meaningful methods

**Anti-Pattern (Anemic Domain Model):**
```csharp
// AVOID: Anemic model - just data
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

**Correct Pattern (Rich Domain Model):**
```csharp
// CORRECT: Rich model - behavior + data
public sealed class Product : Entity<ProductId>
{
    public string Name { get; private set; }
    public Money Price { get; private set; }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new DomainException("Price must be positive");
            
        var oldPrice = Price;
        Price = newPrice;
        
        AddDomainEvent(new PriceChangedEvent(Id, oldPrice, newPrice));
    }
}
```

#### 2.2.4. Ubiquitous Language

**Principle Statement:** Domain model should reflect business terminology exactly.

**Implementation:**
- Use business terms for entity names
- Method names express business operations
- Avoid technical jargon in domain layer
- Maintain glossary of domain terms

**Example:**
```csharp
// Business language reflected in code
public class Order : AggregateRoot<OrderId>
{
    public void PlaceOrder() { }      // Business terminology
    public void CancelOrder() { }     // Not "Delete()" or "Remove()"
    public void ShipOrder() { }       // Business operation
    public void RefundOrder() { }     // Business process
}
```

### 2.3. Domain Layer Dependencies

#### 2.3.1. Allowed Dependencies

The Domain layer may only depend on:

**Standard .NET Types:**
- System namespace primitives (string, int, decimal, DateTime, Guid, etc.)
- System.Collections.Generic for collections
- System.Linq for query expressions
- System.Threading for CancellationToken

**Custom Domain Base Classes:**
- Entity base class
- ValueObject base class
- AggregateRoot base class
- DomainEvent base class
- DomainException class

**No External Libraries:**
- No NuGet package references (except .NET standard libraries)
- No framework-specific code
- No infrastructure concerns

#### 2.3.2. Dependency Enforcement

**Project Reference Rules:**
```xml
<!-- Domain.csproj - ONLY standard .NET references -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <!-- NO EXTERNAL PACKAGE REFERENCES -->
  <!-- NO PROJECT REFERENCES -->
</Project>
```

**Validation:**
- Static analysis to detect external dependencies
- Code review verification
- Architecture tests enforcing isolation

---

## 3. Entity Pattern Specification

### 3.1. Entity Definition

#### 3.1.1. Entity Characteristics

An entity is a domain object that:

**Has Identity:**
- Possesses a unique identifier distinguishing it from other instances
- Identity remains stable throughout object lifecycle
- Equality based on identifier, not attributes

**Has Lifecycle:**
- Can be created, modified, and potentially deleted
- State changes over time
- History may be tracked

**Encapsulates Behavior:**
- Contains business logic relevant to the entity
- Exposes methods for state changes
- Protects invariants through controlled access

**Maintains Consistency:**
- Enforces business rules at all times
- Prevents invalid state
- Validates operations before execution

### 3.2. Entity Implementation Pattern

#### 3.2.1. Base Entity Class

```csharp
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public TId Id { get; protected set; }

    protected Entity(TId id)
    {
        Id = id;
    }

    // For EF Core
    protected Entity()
    {
        Id = default!;
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Equals(entity);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TId>.Default.GetHashCode(Id);
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !Equals(left, right);
    }
}
```

#### 3.2.2. Complete Entity Example

```csharp
public sealed class Product : Entity<ProductId>
{
    // Private setters - state changes through methods only
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public ProductCategory Category { get; private set; }
    public ProductStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Private constructor - use factory method or public constructor with validation
    private Product(
        ProductId id,
        string name,
        string description,
        Money price,
        ProductCategory category)
        : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        Status = ProductStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    // Factory method with business rule enforcement
    public static Product Create(
        ProductId id,
        string name,
        string description,
        Money price,
        ProductCategory category)
    {
        // Validate invariants
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty");

        if (name.Length > 200)
            throw new DomainException("Product name cannot exceed 200 characters");

        if (price.Amount <= 0)
            throw new DomainException("Product price must be positive");

        var product = new Product(id, name, description, price, category);
        
        // Raise domain event
        product.AddDomainEvent(new ProductCreatedEvent(id, name, price));

        return product;
    }

    // Business operation methods
    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new DomainException("Price must be positive");

        if (newPrice.Currency != Price.Currency)
            throw new DomainException("Cannot change currency when updating price");

        var oldPrice = Price;
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PriceChangedEvent(Id, oldPrice, newPrice));
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty");

        if (name.Length > 200)
            throw new DomainException("Product name cannot exceed 200 characters");

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductDetailsUpdatedEvent(Id, name, description));
    }

    public void Publish()
    {
        if (Status == ProductStatus.Published)
            throw new DomainException("Product is already published");

        if (Status == ProductStatus.Discontinued)
            throw new DomainException("Cannot publish discontinued product");

        Status = ProductStatus.Published;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductPublishedEvent(Id));
    }

    public void Discontinue()
    {
        if (Status == ProductStatus.Discontinued)
            throw new DomainException("Product is already discontinued");

        Status = ProductStatus.Discontinued;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductDiscontinuedEvent(Id));
    }

    // EF Core constructor
    private Product() : base()
    {
        Name = default!;
        Description = default!;
        Price = default!;
        Category = default!;
    }
}
```

### 3.3. Entity Identity Pattern

#### 3.3.1. Strongly-Typed ID Implementation

```csharp
public sealed class ProductId : ValueObject
{
    public Guid Value { get; }

    public ProductId(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("Product ID cannot be empty");

        Value = value;
    }

    public static ProductId New() => new(Guid.NewGuid());

    public static ProductId From(Guid value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    // Implicit conversion for convenience
    public static implicit operator Guid(ProductId id) => id.Value;
}
```

**Benefits of Strongly-Typed IDs:**
- Type safety preventing ID confusion
- Explicit intent in method signatures
- Better refactoring support
- Domain-specific validation

### 3.4. Entity Design Guidelines

#### 3.4.1. Constructor Design

**Factory Method Pattern (Recommended):**
```csharp
public static Product Create(/* parameters */)
{
    // Validation
    // Construction
    // Domain event
    return product;
}
```

**Public Constructor with Validation:**
```csharp
public Product(ProductId id, string name, Money price)
    : base(id)
{
    // Validation
    // Assignment
    // Domain event
}
```

**Private Constructor + Factory (Complex Scenarios):**
```csharp
private Product(/* parameters */) { }

public static Product CreateStandard(/* params */) { }
public static Product CreatePremium(/* params */) { }
public static Product CreateBundle(/* params */) { }
```

#### 3.4.2. State Modification Guidelines

**Do:**
- Expose methods for state changes
- Validate before changing state
- Raise domain events for significant changes
- Use private setters
- Return void or result objects from methods

**Don't:**
- Expose public setters
- Allow partial updates that violate invariants
- Modify state without validation
- Forget to raise domain events
- Return entities from modification methods (command-query separation)

#### 3.4.3. Method Naming Conventions

**Business Operation Methods:**
- Use action verbs: `PlaceOrder()`, `CancelOrder()`, `ShipOrder()`
- Reflect business language
- Avoid technical terms

**Query Methods:**
- Start with Can/Is/Has: `CanBeCancelled()`, `IsActive()`, `HasInventory()`
- Return boolean or computed values
- No state modification

---

## 4. Value Object Pattern Specification

### 4.1. Value Object Definition

#### 4.1.1. Value Object Characteristics

A value object is a domain object that:

**Has No Identity:**
- Defined entirely by its attributes
- Two instances with same attributes are equal
- Interchangeable when attributes match

**Is Immutable:**
- Cannot be modified after construction
- State changes require creating new instance
- Thread-safe by design

**Is Self-Validating:**
- Enforces constraints at construction
- Cannot exist in invalid state
- Validation embedded in type

**Represents Domain Concept:**
- Models business concepts (Money, Email, Address)
- Encapsulates validation rules
- Avoids primitive obsession

### 4.2. Value Object Implementation Pattern

#### 4.2.1. Base ValueObject Class

```csharp
public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        if (other is null) return false;
        if (GetType() != other.GetType()) return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObject valueObject && Equals(valueObject);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !Equals(left, right);
    }
}
```

#### 4.2.2. Complete Value Object Examples

**Money Value Object:**

```csharp
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required");

        if (currency.Length != 3)
            throw new DomainException("Currency must be a 3-letter ISO code");

        return new Money(amount, currency);
    }

    public static Money Zero(string currency) => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Cannot add money with different currencies");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Cannot subtract money with different currencies");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}
```

**Email Value Object:**

```csharp
public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email address is required");

        value = value.Trim().ToLowerInvariant();

        if (value.Length > 254)
            throw new DomainException("Email address is too long");

        if (!EmailRegex.IsMatch(value))
            throw new DomainException("Email address format is invalid");

        return new Email(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
```

**Address Value Object:**

```csharp
public sealed class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    private Address(
        string street,
        string city,
        string state,
        string zipCode,
        string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public static Address Create(
        string street,
        string city,
        string state,
        string zipCode,
        string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street is required");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required");

        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State is required");

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new DomainException("Zip code is required");

        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country is required");

        return new Address(street, city, state, zipCode, country);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }
}
```

### 4.3. Value Object Design Guidelines

#### 4.3.1. When to Use Value Objects

**Use Value Objects For:**
- Money and currency
- Email addresses, phone numbers
- Physical measurements (weight, distance, temperature)
- Addresses, coordinates
- Date ranges, time periods
- Quantities, percentages
- SKUs, product codes
- Any concept defined by its attributes

**Benefits:**
- Eliminates primitive obsession
- Centralizes validation
- Expresses domain concepts clearly
- Enables type safety
- Improves testability

#### 4.3.2. Immutability Enforcement

**Correct Pattern:**
```csharp
public sealed class Money : ValueObject
{
    public decimal Amount { get; } // Read-only property

    private Money(decimal amount, string currency)
    {
        Amount = amount; // Set in constructor only
        Currency = currency;
    }

    // Operations return new instances
    public Money Add(Money other) => new Money(Amount + other.Amount, Currency);
}
```

**Incorrect Pattern:**
```csharp
// AVOID: Mutable value object
public class Money
{
    public decimal Amount { get; set; } // WRONG: Public setter
    public string Currency { get; set; }
}
```

#### 4.3.3. Value Object Operations

**Arithmetic Operations (for measurable values):**
```csharp
public Money Add(Money other) { }
public Money Subtract(Money other) { }
public Money Multiply(decimal factor) { }
public Money Divide(decimal divisor) { }
```

**Comparison Operations:**
```csharp
public bool IsGreaterThan(Money other) { }
public bool IsLessThan(Money other) { }
public int CompareTo(Money other) { }
```

**Conversion Operations:**
```csharp
public Money ConvertTo(string targetCurrency, decimal exchangeRate) { }
```

---

## 5. Aggregate and Aggregate Root Patterns

### 5.1. Aggregate Definition

#### 5.1.1. Aggregate Characteristics

An aggregate is:

**Consistency Boundary:**
- Defines scope of transactional consistency
- Groups related entities and value objects
- Enforces invariants within boundary

**Transactional Boundary:**
- One aggregate per database transaction
- Changes persisted atomically
- Prevents partial updates

**Unit of Retrieval:**
- Loaded and saved as complete unit
- No partial loading of aggregate
- Repository methods operate on entire aggregate

**Has Single Root:**
- One entity serves as aggregate root
- External access only through root
- Root coordinates internal changes

### 5.2. Aggregate Root Pattern

#### 5.2.1. AggregateRoot Base Class

```csharp
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected AggregateRoot() : base()
    {
    }
}
```

#### 5.2.2. Complete Aggregate Example

**Order Aggregate:**

```csharp
public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderLine> _lines = new();

    public CustomerId CustomerId { get; private set; }
    public OrderNumber OrderNumber { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public Address ShippingAddress { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ShippedDate { get; private set; }

    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    private Order(
        OrderId id,
        CustomerId customerId,
        OrderNumber orderNumber,
        Address shippingAddress)
        : base(id)
    {
        CustomerId = customerId;
        OrderNumber = orderNumber;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalAmount = Money.Zero("USD");
        OrderDate = DateTime.UtcNow;
    }

    public static Order Create(
        OrderId id,
        CustomerId customerId,
        OrderNumber orderNumber,
        Address shippingAddress)
    {
        var order = new Order(id, customerId, orderNumber, shippingAddress);
        order.AddDomainEvent(new OrderCreatedEvent(id, customerId, orderNumber));
        return order;
    }

    // Aggregate root controls all modifications
    public void AddLine(ProductId productId, Money unitPrice, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify order that is not pending");

        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");

        // Check if line already exists
        var existingLine = _lines.FirstOrDefault(l => l.ProductId == productId);
        if (existingLine != null)
        {
            existingLine.IncreaseQuantity(quantity);
        }
        else
        {
            var line = OrderLine.Create(
                OrderLineId.New(),
                productId,
                unitPrice,
                quantity);
            _lines.Add(line);
        }

        RecalculateTotal();

        AddDomainEvent(new OrderLineAddedEvent(Id, productId, quantity));
    }

    public void RemoveLine(OrderLineId lineId)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify order that is not pending");

        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line == null)
            throw new DomainException("Order line not found");

        _lines.Remove(line);
        RecalculateTotal();

        AddDomainEvent(new OrderLineRemovedEvent(Id, lineId));
    }

    public void UpdateShippingAddress(Address newAddress)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot update address for non-pending order");

        ShippingAddress = newAddress;
        AddDomainEvent(new OrderShippingAddressUpdatedEvent(Id, newAddress));
    }

    public void PlaceOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Order is not in pending status");

        if (!_lines.Any())
            throw new DomainException("Cannot place order without line items");

        Status = OrderStatus.Placed;
        AddDomainEvent(new OrderPlacedEvent(Id, OrderNumber, TotalAmount));
    }

    public void Ship()
    {
        if (Status != OrderStatus.Placed)
            throw new DomainException("Order must be placed before shipping");

        Status = OrderStatus.Shipped;
        ShippedDate = DateTime.UtcNow;
        AddDomainEvent(new OrderShippedEvent(Id, OrderNumber, ShippedDate.Value));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new DomainException("Cannot cancel shipped order");

        if (Status == OrderStatus.Delivered)
            throw new DomainException("Cannot cancel delivered order");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id, OrderNumber));
    }

    private void RecalculateTotal()
    {
        TotalAmount = _lines
            .Select(l => l.LineTotal)
            .Aggregate(Money.Zero("USD"), (sum, lineTotal) => sum.Add(lineTotal));
    }

    // EF Core constructor
    private Order() : base()
    {
        CustomerId = default!;
        OrderNumber = default!;
        ShippingAddress = default!;
        TotalAmount = default!;
    }
}
```

**OrderLine Entity (within Order aggregate):**

```csharp
public sealed class OrderLine : Entity<OrderLineId>
{
    public ProductId ProductId { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Money LineTotal { get; private set; }

    private OrderLine(
        OrderLineId id,
        ProductId productId,
        Money unitPrice,
        int quantity)
        : base(id)
    {
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        LineTotal = unitPrice.Multiply(quantity);
    }

    internal static OrderLine Create(
        OrderLineId id,
        ProductId productId,
        Money unitPrice,
        int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");

        return new OrderLine(id, productId, unitPrice, quantity);
    }

    internal void IncreaseQuantity(int additionalQuantity)
    {
        if (additionalQuantity <= 0)
            throw new DomainException("Additional quantity must be positive");

        Quantity += additionalQuantity;
        LineTotal = UnitPrice.Multiply(Quantity);
    }

    internal void DecreaseQuantity(int decreaseAmount)
    {
        if (decreaseAmount <= 0)
            throw new DomainException("Decrease amount must be positive");

        if (Quantity - decreaseAmount < 1)
            throw new DomainException("Resulting quantity would be less than 1");

        Quantity -= decreaseAmount;
        LineTotal = UnitPrice.Multiply(Quantity);
    }

    // EF Core constructor
    private OrderLine() : base()
    {
        ProductId = default!;
        UnitPrice = default!;
        LineTotal = default!;
    }
}
```

### 5.3. Aggregate Design Guidelines

#### 5.3.1. Aggregate Size

**Guideline:** Keep aggregates small.

**Recommended:**
- Single root entity + related value objects
- Small number of child entities (< 5)
- Focus on transactional consistency needs

**Avoid:**
- Large object graphs
- Entire domain model in one aggregate
- Performance problems from loading large graphs

#### 5.3.2. Aggregate Boundaries

**How to Identify Aggregate Boundaries:**

1. **Transactional Consistency:**
   - What must change together in a transaction?
   - What invariants must be maintained together?

2. **Business Rules:**
   - What business rules span multiple entities?
   - What operations must be atomic?

3. **Lifecycle:**
   - What entities share the same lifecycle?
   - What gets created/deleted together?

**Example Decision:**
- Order + OrderLines = One aggregate (strong consistency needed)
- Customer + Orders = Separate aggregates (eventual consistency acceptable)

#### 5.3.3. Reference Between Aggregates

**Use Identifiers, Not Object References:**

```csharp
// CORRECT: Reference by ID
public sealed class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; } // ID reference
}

// AVOID: Direct object reference
public sealed class Order : AggregateRoot<OrderId>
{
    public Customer Customer { get; private set; } // WRONG: Navigation property
}
```

**Benefits:**
- Enforces aggregate boundaries
- Prevents accidental modification
- Reduces loading overhead
- Enables independent scaling

#### 5.3.4. Repository Pattern for Aggregates

**One Repository Per Aggregate:**

```csharp
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken);
    Task<Order?> GetByOrderNumberAsync(OrderNumber orderNumber, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
    Task DeleteAsync(Order order, CancellationToken cancellationToken);
}
```

**No Repository for Child Entities:**
- OrderLine does not have its own repository
- Access only through Order aggregate root

---

## 6. Domain Event Pattern Specification

### 6.1. Domain Event Definition

#### 6.1.1. Domain Event Characteristics

A domain event:

**Represents Significant Occurrence:**
- Models something important that happened in the domain
- Past tense naming (ProductCreated, OrderPlaced, PriceChanged)
- Contains data about what happened

**Is Immutable:**
- Created once, never modified
- Record type preferred
- All properties read-only

**Enables Decoupling:**
- Separates cause from effect
- Allows multiple reactions to same event
- Supports eventual consistency

**Supports Event-Driven Architecture:**
- Foundation for CQRS
- Enables event sourcing (if desired)
- Facilitates integration with external systems

### 6.2. Domain Event Implementation

#### 6.2.1. IDomainEvent Interface

```csharp
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
```

#### 6.2.2. Base DomainEvent Class

```csharp
public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
```

#### 6.2.3. Domain Event Examples

```csharp
// Product domain events
public sealed record ProductCreatedEvent(
    ProductId ProductId,
    string Name,
    Money Price) : DomainEvent;

public sealed record PriceChangedEvent(
    ProductId ProductId,
    Money OldPrice,
    Money NewPrice) : DomainEvent;

public sealed record ProductPublishedEvent(
    ProductId ProductId) : DomainEvent;

public sealed record ProductDiscontinuedEvent(
    ProductId ProductId) : DomainEvent;

// Order domain events
public sealed record OrderCreatedEvent(
    OrderId OrderId,
    CustomerId CustomerId,
    OrderNumber OrderNumber) : DomainEvent;

public sealed record OrderPlacedEvent(
    OrderId OrderId,
    OrderNumber OrderNumber,
    Money TotalAmount) : DomainEvent;

public sealed record OrderShippedEvent(
    OrderId OrderId,
    OrderNumber OrderNumber,
    DateTime ShippedDate) : DomainEvent;

public sealed record OrderCancelledEvent(
    OrderId OrderId,
    OrderNumber OrderNumber) : DomainEvent;
```

### 6.3. Domain Event Lifecycle

#### 6.3.1. Event Flow Diagram

```
┌──────────────────────────────────────────────────────────┐
│  1. Aggregate Root Operation                             │
│     Product.UpdatePrice(newPrice)                        │
└────────────────┬─────────────────────────────────────────┘
                 ↓
┌────────────────┴─────────────────────────────────────────┐
│  2. State Change + Event Creation                        │
│     Price = newPrice;                                    │
│     AddDomainEvent(new PriceChangedEvent(...))           │
└────────────────┬─────────────────────────────────────────┘
                 ↓
┌────────────────┴─────────────────────────────────────────┐
│  3. Handler Persists Aggregate                           │
│     await _repository.UpdateAsync(product)               │
│     await _dbContext.SaveChangesAsync()                  │
└────────────────┬─────────────────────────────────────────┘
                 ↓
┌────────────────┴─────────────────────────────────────────┐
│  4. After Transaction - Dispatch Events                  │
│     foreach (var @event in product.DomainEvents)         │
│         await _publisher.Publish(@event)                 │
└────────────────┬─────────────────────────────────────────┘
                 ↓
┌────────────────┴─────────────────────────────────────────┐
│  5. Domain Event Handlers Execute                        │
│     • SendPriceChangeNotification                        │
│     • UpdateProductSearchIndex                           │
│     • RecordPriceHistory                                 │
│     • PublishToExternalSystem                            │
└──────────────────────────────────────────────────────────┘
```

#### 6.3.2. Event Publishing in Application Layer

**Handler Pattern:**

```csharp
public sealed class UpdateProductPriceCommandHandler
    : IRequestHandler<UpdateProductPriceCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IPublisher _publisher;

    public async Task<ProductDto> Handle(
        UpdateProductPriceCommand request,
        CancellationToken cancellationToken)
    {
        // Get aggregate
        var product = await _repository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product == null)
            throw new NotFoundException("Product not found");

        // Execute domain operation (raises domain event)
        var newPrice = Money.Create(request.Price, request.Currency);
        product.UpdatePrice(newPrice);

        // Persist
        await _repository.UpdateAsync(product, cancellationToken);

        // Publish domain events after successful save
        foreach (var domainEvent in product.DomainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        product.ClearDomainEvents();

        return _mapper.Map<ProductDto>(product);
    }
}
```

### 6.4. Domain Event Handlers

#### 6.4.1. Event Handler Implementation

```csharp
public sealed class PriceChangedEventHandler
    : INotificationHandler<PriceChangedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IPriceHistoryRepository _priceHistoryRepository;
    private readonly ILogger<PriceChangedEventHandler> _logger;

    public PriceChangedEventHandler(
        IEmailService emailService,
        IPriceHistoryRepository priceHistoryRepository,
        ILogger<PriceChangedEventHandler> logger)
    {
        _emailService = emailService;
        _priceHistoryRepository = priceHistoryRepository;
        _logger = logger;
    }

    public async Task Handle(
        PriceChangedEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling PriceChangedEvent for Product {ProductId}",
            notification.ProductId);

        try
        {
            // Record price history
            var historyEntry = new PriceHistory(
                id: Guid.NewGuid(),
                productId: notification.ProductId,
                oldPrice: notification.OldPrice,
                newPrice: notification.NewPrice,
                changedAt: notification.OccurredOn);

            await _priceHistoryRepository.AddAsync(historyEntry, cancellationToken);

            // Send notification (non-critical, don't fail if this fails)
            try
            {
                await _emailService.SendPriceChangeNotificationAsync(
                    notification.ProductId,
                    notification.OldPrice,
                    notification.NewPrice,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to send price change notification for Product {ProductId}",
                    notification.ProductId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling PriceChangedEvent for Product {ProductId}",
                notification.ProductId);
            throw;
        }
    }
}
```

#### 6.4.2. Multiple Handlers for Single Event

```csharp
// Handler 1: Update search index
public sealed class ProductPublishedSearchIndexHandler
    : INotificationHandler<ProductPublishedEvent>
{
    public async Task Handle(
        ProductPublishedEvent notification,
        CancellationToken cancellationToken)
    {
        // Update search index
    }
}

// Handler 2: Send notification
public sealed class ProductPublishedNotificationHandler
    : INotificationHandler<ProductPublishedEvent>
{
    public async Task Handle(
        ProductPublishedEvent notification,
        CancellationToken cancellationToken)
    {
        // Send notification
    }
}

// Handler 3: Update analytics
public sealed class ProductPublishedAnalyticsHandler
    : INotificationHandler<ProductPublishedEvent>
{
    public async Task Handle(
        ProductPublishedEvent notification,
        CancellationToken cancellationToken)
    {
        // Record analytics event
    }
}
```

### 6.5. Domain Event Best Practices

#### 6.5.1. Event Naming Conventions

**Use Past Tense:**
- ProductCreated (not ProductCreate)
- OrderPlaced (not PlaceOrder)
- PriceChanged (not ChangePrice)

**Be Specific:**
- PriceChanged (better than ProductUpdated)
- OrderShipped (better than OrderStatusChanged)
- CustomerEmailVerified (better than CustomerUpdated)

#### 6.5.2. Event Data Guidelines

**Include Relevant Data:**
- Event ID and timestamp (from base class)
- Aggregate ID
- Data needed by handlers
- Previous state (for change events)

**Don't Include:**
- Complete aggregate state
- Sensitive data unnecessarily
- Computed values that can be derived

**Example:**
```csharp
// GOOD: Focused event data
public sealed record PriceChangedEvent(
    ProductId ProductId,
    Money OldPrice,
    Money NewPrice) : DomainEvent;

// AVOID: Too much data
public sealed record PriceChangedEvent(
    ProductId ProductId,
    string ProductName,
    string ProductDescription,
    Money OldPrice,
    Money NewPrice,
    CategoryId CategoryId,
    string CategoryName,
    DateTime CreatedAt,
    DateTime UpdatedAt) : DomainEvent; // Too much!
```

---

## 7. Business Invariant Enforcement

### 7.1. Invariant Definition

#### 7.1.1. Invariant Characteristics

A business invariant is:

**Always True Rule:**
- Must hold at all times
- Never violated, even temporarily
- Enforced at domain level

**Business Constraint:**
- Derived from business requirements
- Reflects business policy
- Protects business integrity

**Validation Rule:**
- Checked at construction
- Checked before state changes
- Throws exception if violated

### 7.2. Invariant Implementation Patterns

#### 7.2.1. Constructor Validation

```csharp
public sealed class Product : Entity<ProductId>
{
    public Product(ProductId id, string name, Money price)
        : base(id)
    {
        // Invariant: Product must have non-empty name
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty");

        // Invariant: Product name cannot exceed 200 characters
        if (name.Length > 200)
            throw new DomainException("Product name cannot exceed 200 characters");

        // Invariant: Product price must be positive
        if (price.Amount <= 0)
            throw new DomainException("Product price must be positive");

        Name = name;
        Price = price;
    }
}
```

#### 7.2.2. Method Validation

```csharp
public void UpdatePrice(Money newPrice)
{
    // Invariant: Price must be positive
    if (newPrice.Amount <= 0)
        throw new DomainException("Price must be positive");

    // Invariant: Cannot change currency
    if (newPrice.Currency != Price.Currency)
        throw new DomainException("Cannot change currency when updating price");

    Price = newPrice;
}
```

#### 7.2.3. Cross-Property Invariants

```csharp
public sealed class Order : AggregateRoot<OrderId>
{
    public void PlaceOrder()
    {
        // Invariant: Order must have at least one line item
        if (!_lines.Any())
            throw new DomainException("Cannot place order without line items");

        // Invariant: Order must have valid shipping address
        if (ShippingAddress == null)
            throw new DomainException("Order must have shipping address");

        // Invariant: Total amount must match sum of line items
        var calculatedTotal = _lines
            .Select(l => l.LineTotal)
            .Aggregate(Money.Zero("USD"), (sum, total) => sum.Add(total));

        if (TotalAmount != calculatedTotal)
            throw new DomainException("Order total does not match line items");

        Status = OrderStatus.Placed;
    }
}
```

### 7.3. Common Invariant Patterns

#### 7.3.1. Value Range Invariants

```csharp
// Quantity must be positive
if (quantity <= 0)
    throw new DomainException("Quantity must be positive");

// Age must be between bounds
if (age < 0 || age > 150)
    throw new DomainException("Age must be between 0 and 150");

// Discount percentage must be 0-100
if (discountPercentage < 0 || discountPercentage > 100)
    throw new DomainException("Discount must be between 0% and 100%");
```

#### 7.3.2. State Transition Invariants

```csharp
public void Ship()
{
    // Invariant: Can only ship placed orders
    if (Status != OrderStatus.Placed)
        throw new DomainException("Order must be placed before shipping");

    Status = OrderStatus.Shipped;
    ShippedDate = DateTime.UtcNow;
}

public void Cancel()
{
    // Invariant: Cannot cancel shipped or delivered orders
    if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
        throw new DomainException("Cannot cancel shipped or delivered order");

    Status = OrderStatus.Cancelled;
}
```

#### 7.3.3. Relationship Invariants

```csharp
public void AddReview(Review review)
{
    // Invariant: Customer must have purchased product
    if (!HasPurchasedProduct(review.CustomerId))
        throw new DomainException("Only customers who purchased can review");

    // Invariant: Customer can only review once
    if (_reviews.Any(r => r.CustomerId == review.CustomerId))
        throw new DomainException("Customer has already reviewed this product");

    _reviews.Add(review);
}
```

### 7.4. DomainException Implementation

```csharp
public sealed class DomainException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, object>? ErrorData { get; }

    public DomainException(string message)
        : base(message)
    {
        ErrorCode = "DOMAIN_ERROR";
    }

    public DomainException(string message, string errorCode)
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

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = "DOMAIN_ERROR";
    }
}
```

**Usage:**

```csharp
throw new DomainException(
    "Insufficient inventory",
    "INSUFFICIENT_INVENTORY",
    new Dictionary<string, object>
    {
        ["ProductId"] = productId,
        ["RequestedQuantity"] = requestedQuantity,
        ["AvailableQuantity"] = availableQuantity
    });
```

---

## 8. Domain Services and Policies

### 8.1. Domain Service Definition

#### 8.1.1. When to Use Domain Services

Use domain services when:

**Logic Doesn't Belong to Single Entity:**
- Operation involves multiple aggregates
- Calculation requires data from multiple sources
- Behavior is stateless

**Examples:**
- Transfer funds between accounts
- Calculate shipping cost based on multiple factors
- Validate uniqueness across aggregates
- Complex pricing calculations

### 8.2. Domain Service Implementation

#### 8.2.1. Domain Service Interface

```csharp
public interface IPricingService
{
    Money CalculatePrice(
        Product product,
        Customer customer,
        int quantity);

    Money CalculateDiscount(
        Money originalPrice,
        Customer customer,
        DateTime orderDate);
}
```

#### 8.2.2. Domain Service Implementation

```csharp
public sealed class PricingService : IPricingService
{
    public Money CalculatePrice(
        Product product,
        Customer customer,
        int quantity)
    {
        var basePrice = product.Price.Multiply(quantity);

        // Apply volume discount
        if (quantity >= 100)
            basePrice = basePrice.Multiply(0.9m); // 10% discount
        else if (quantity >= 50)
            basePrice = basePrice.Multiply(0.95m); // 5% discount

        // Apply customer tier discount
        var tierDiscount = customer.Tier switch
        {
            CustomerTier.Gold => 0.15m,
            CustomerTier.Silver => 0.10m,
            CustomerTier.Bronze => 0.05m,
            _ => 0m
        };

        if (tierDiscount > 0)
            basePrice = basePrice.Multiply(1 - tierDiscount);

        return basePrice;
    }

    public Money CalculateDiscount(
        Money originalPrice,
        Customer customer,
        DateTime orderDate)
    {
        // Business logic for discount calculation
        // ...
        return Money.Zero(originalPrice.Currency);
    }
}
```

### 8.3. Policy Pattern

#### 8.3.1. Policy Interface

```csharp
public interface IRefundPolicy
{
    bool CanRefund(Order order);
    Money CalculateRefundAmount(Order order);
}
```

#### 8.3.2. Policy Implementation

```csharp
public sealed class StandardRefundPolicy : IRefundPolicy
{
    private const int RefundWindowDays = 30;

    public bool CanRefund(Order order)
    {
        // Cannot refund if not delivered
        if (order.Status != OrderStatus.Delivered)
            return false;

        // Cannot refund if outside window
        var daysSinceDelivery = (DateTime.UtcNow - order.DeliveredDate!.Value).Days;
        if (daysSinceDelivery > RefundWindowDays)
            return false;

        // Cannot refund if already refunded
        if (order.IsRefunded)
            return false;

        return true;
    }

    public Money CalculateRefundAmount(Order order)
    {
        if (!CanRefund(order))
            throw new DomainException("Order is not eligible for refund");

        // Full refund within 7 days
        var daysSinceDelivery = (DateTime.UtcNow - order.DeliveredDate!.Value).Days;
        if (daysSinceDelivery <= 7)
            return order.TotalAmount;

        // 80% refund within 14 days
        if (daysSinceDelivery <= 14)
            return order.TotalAmount.Multiply(0.8m);

        // 50% refund within 30 days
        return order.TotalAmount.Multiply(0.5m);
    }
}
```

---

## 9. Specification Pattern Implementation

### 9.1. Specification Definition

#### 9.1.1. Specification Characteristics

A specification:

**Encapsulates Business Rule:**
- Represents testable business condition
- Reusable across use cases
- Composable with other specifications

**Returns Boolean:**
- Evaluates to true or false
- Can be used for validation
- Can be used for querying

### 9.2. Specification Base Class

```csharp
public abstract class Specification<T>
{
    public abstract bool IsSatisfiedBy(T candidate);

    public Specification<T> And(Specification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }

    public Specification<T> Or(Specification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}
```

### 9.3. Specification Implementations

#### 9.3.1. Composite Specifications

```csharp
internal sealed class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T candidate)
    {
        return _left.IsSatisfiedBy(candidate) && _right.IsSatisfiedBy(candidate);
    }
}

internal sealed class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T candidate)
    {
        return _left.IsSatisfiedBy(candidate) || _right.IsSatisfiedBy(candidate);
    }
}

internal sealed class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _specification;

    public NotSpecification(Specification<T> specification)
    {
        _specification = specification;
    }

    public override bool IsSatisfiedBy(T candidate)
    {
        return !_specification.IsSatisfiedBy(candidate);
    }
}
```

#### 9.3.2. Domain Specification Examples

```csharp
public sealed class EligibleForDiscountSpecification : Specification<Customer>
{
    public override bool IsSatisfiedBy(Customer customer)
    {
        return customer.Tier == CustomerTier.Gold ||
               customer.Tier == CustomerTier.Silver ||
               customer.TotalPurchases >= Money.Create(1000, "USD");
    }
}

public sealed class ActiveProductSpecification : Specification<Product>
{
    public override bool IsSatisfiedBy(Product product)
    {
        return product.Status == ProductStatus.Published &&
               !product.IsDiscontinued &&
               product.Price.Amount > 0;
    }
}

public sealed class EligibleForFreeShippingSpecification : Specification<Order>
{
    private readonly Money _threshold;

    public EligibleForFreeShippingSpecification(Money threshold)
    {
        _threshold = threshold;
    }

    public override bool IsSatisfiedBy(Order order)
    {
        return order.TotalAmount.Amount >= _threshold.Amount &&
               order.ShippingAddress.Country == "USA";
    }
}
```

#### 9.3.3. Specification Usage

**In Entity:**

```csharp
public sealed class Order : AggregateRoot<OrderId>
{
    public bool IsFreeShippingEligible()
    {
        var spec = new EligibleForFreeShippingSpecification(
            Money.Create(100, "USD"));
        return spec.IsSatisfiedBy(this);
    }
}
```

**Composed Specifications:**

```csharp
var activeProductSpec = new ActiveProductSpecification();
var discountEligibleSpec = new EligibleForDiscountSpecification();

// Products that are active AND customer eligible for discount
var eligibleForPromotionSpec = activeProductSpec.And(discountEligibleSpec);

if (eligibleForPromotionSpec.IsSatisfiedBy(product, customer))
{
    // Apply promotion
}
```

---

## 10. Domain Layer Structure and Organization

### 10.1. Folder Structure

#### 10.1.1. Recommended Organization

```
/Domain
├── /Products
│   ├── Product.cs                    (Aggregate Root)
│   ├── ProductId.cs                  (Value Object - ID)
│   ├── ProductCategory.cs            (Value Object)
│   ├── ProductStatus.cs              (Enum)
│   ├── Money.cs                      (Value Object)
│   ├── /Events
│   │   ├── ProductCreatedEvent.cs
│   │   ├── PriceChangedEvent.cs
│   │   └── ProductPublishedEvent.cs
│   ├── /Specifications
│   │   ├── ActiveProductSpecification.cs
│   │   └── DiscountedProductSpecification.cs
│   └── /Exceptions
│       └── ProductDomainException.cs
│
├── /Orders
│   ├── Order.cs                      (Aggregate Root)
│   ├── OrderId.cs                    (Value Object - ID)
│   ├── OrderLine.cs                  (Entity)
│   ├── OrderLineId.cs                (Value Object - ID)
│   ├── OrderNumber.cs                (Value Object)
│   ├── OrderStatus.cs                (Enum)
│   ├── /Events
│   │   ├── OrderCreatedEvent.cs
│   │   ├── OrderPlacedEvent.cs
│   │   ├── OrderShippedEvent.cs
│   │   └── OrderCancelledEvent.cs
│   └── /Services
│       └── IOrderPricingService.cs
│
├── /Customers
│   ├── Customer.cs                   (Aggregate Root)
│   ├── CustomerId.cs                 (Value Object - ID)
│   ├── Email.cs                      (Value Object)
│   ├── Address.cs                    (Value Object)
│   ├── CustomerTier.cs               (Enum)
│   └── /Events
│       ├── CustomerRegisteredEvent.cs
│       └── CustomerEmailVerifiedEvent.cs
│
├── /Common
│   ├── Entity.cs                     (Base class)
│   ├── ValueObject.cs                (Base class)
│   ├── AggregateRoot.cs              (Base class)
│   ├── DomainEvent.cs                (Base class)
│   ├── Specification.cs              (Base class)
│   ├── DomainException.cs            (Base exception)
│   └── IDomainEvent.cs               (Interface)
│
└── /Repositories
    ├── IProductRepository.cs         (Interface)
    ├── IOrderRepository.cs           (Interface)
    └── ICustomerRepository.cs        (Interface)
```

### 10.2. Module Organization Principles

#### 10.2.1. Vertical Slice by Bounded Context

**Guideline:** Organize by business domain, not by technical type.

**Correct:**
```
/Domain
  /Products      (Bounded context)
  /Orders        (Bounded context)
  /Customers     (Bounded context)
```

**Avoid:**
```
/Domain
  /Entities      (Technical grouping - AVOID)
  /ValueObjects  (Technical grouping - AVOID)
  /Events        (Technical grouping - AVOID)
```

#### 10.2.2. Cohesion Guidelines

**Keep Related Concepts Together:**
- All product-related types in /Products
- Product events in /Products/Events
- Product specifications in /Products/Specifications

**Benefits:**
- Easy to find related code
- Clear ownership and boundaries
- Simplified testing
- Better team organization

---

## 11. Testing Strategy for Domain Logic

### 11.1. Unit Testing Approach

#### 11.1.1. Entity Testing

```csharp
public sealed class ProductTests
{
    [Fact]
    public void Create_ValidParameters_CreatesProduct()
    {
        // Arrange
        var id = ProductId.New();
        var name = "Test Product";
        var description = "Test Description";
        var price = Money.Create(99.99m, "USD");
        var category = ProductCategory.Electronics;

        // Act
        var product = Product.Create(id, name, description, price, category);

        // Assert
        product.Should().NotBeNull();
        product.Id.Should().Be(id);
        product.Name.Should().Be(name);
        product.Price.Should().Be(price);
        product.Status.Should().Be(ProductStatus.Draft);
        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductCreatedEvent>();
    }

    [Fact]
    public void Create_EmptyName_ThrowsDomainException()
    {
        // Arrange
        var id = ProductId.New();
        var name = "";
        var price = Money.Create(99.99m, "USD");

        // Act & Assert
        var act = () => Product.Create(id, name, "", price, ProductCategory.Electronics);
        
        act.Should().Throw<DomainException>()
            .WithMessage("Product name cannot be empty");
    }

    [Fact]
    public void UpdatePrice_ValidPrice_UpdatesPriceAndRaisesEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        var newPrice = Money.Create(149.99m, "USD");

        // Act
        product.UpdatePrice(newPrice);

        // Assert
        product.Price.Should().Be(newPrice);
        product.DomainEvents.Should().Contain(e =>
            e is PriceChangedEvent priceEvent &&
            priceEvent.ProductId == product.Id &&
            priceEvent.NewPrice == newPrice);
    }

    [Fact]
    public void UpdatePrice_NegativePrice_ThrowsDomainException()
    {
        // Arrange
        var product = CreateTestProduct();
        var invalidPrice = Money.Create(-10m, "USD");

        // Act & Assert
        var act = () => product.UpdatePrice(invalidPrice);
        
        act.Should().Throw<DomainException>()
            .WithMessage("Price must be positive");
    }

    private static Product CreateTestProduct()
    {
        return Product.Create(
            ProductId.New(),
            "Test Product",
            "Description",
            Money.Create(99.99m, "USD"),
            ProductCategory.Electronics);
    }
}
```

#### 11.1.2. Value Object Testing

```csharp
public sealed class MoneyTests
{
    [Fact]
    public void Create_ValidAmount_CreatesMoney()
    {
        // Arrange & Act
        var money = Money.Create(100m, "USD");

        // Assert
        money.Amount.Should().Be(100m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_NegativeAmount_ThrowsDomainException()
    {
        // Act & Assert
        var act = () => Money.Create(-10m, "USD");
        
        act.Should().Throw<DomainException>()
            .WithMessage("Amount cannot be negative");
    }

    [Fact]
    public void Add_SameCurrency_ReturnsSum()
    {
        // Arrange
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(50m, "USD");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_DifferentCurrency_ThrowsDomainException()
    {
        // Arrange
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(50m, "EUR");

        // Act & Assert
        var act = () => money1.Add(money2);
        
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot add money with different currencies");
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(100m, "USD");

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(200m, "USD");

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 == money2).Should().BeFalse();
    }
}
```

#### 11.1.3. Aggregate Testing

```csharp
public sealed class OrderTests
{
    [Fact]
    public void AddLine_ValidLine_AddsLineAndRecalculatesTotal()
    {
        // Arrange
        var order = CreateTestOrder();
        var productId = ProductId.New();
        var unitPrice = Money.Create(50m, "USD");
        var quantity = 2;

        // Act
        order.AddLine(productId, unitPrice, quantity);

        // Assert
        order.Lines.Should().HaveCount(1);
        order.Lines.First().ProductId.Should().Be(productId);
        order.Lines.First().Quantity.Should().Be(quantity);
        order.TotalAmount.Should().Be(Money.Create(100m, "USD"));
    }

    [Fact]
    public void PlaceOrder_NoLines_ThrowsDomainException()
    {
        // Arrange
        var order = CreateTestOrder();

        // Act & Assert
        var act = () => order.PlaceOrder();
        
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot place order without line items");
    }

    [Fact]
    public void PlaceOrder_WithLines_ChangesStatusAndRaisesEvent()
    {
        // Arrange
        var order = CreateTestOrder();
        order.AddLine(
            ProductId.New(),
            Money.Create(50m, "USD"),
            2);

        // Act
        order.PlaceOrder();

        // Assert
        order.Status.Should().Be(OrderStatus.Placed);
        order.DomainEvents.Should().Contain(e => e is OrderPlacedEvent);
    }

    private static Order CreateTestOrder()
    {
        return Order.Create(
            OrderId.New(),
            CustomerId.New(),
            OrderNumber.Generate(),
            Address.Create("123 Main St", "City", "State", "12345", "USA"));
    }
}
```

### 11.2. Testing Best Practices

#### 11.2.1. Test Organization

**One Test Class Per Domain Class:**
```
ProductTests.cs          -> Product.cs
MoneyTests.cs           -> Money.cs
OrderTests.cs           -> Order.cs
```

**Naming Convention:**
```
MethodName_Scenario_ExpectedBehavior
```

**Examples:**
- `Create_ValidParameters_CreatesProduct`
- `UpdatePrice_NegativePrice_ThrowsDomainException`
- `PlaceOrder_NoLines_ThrowsDomainException`

#### 11.2.2. Test Coverage Goals

**Minimum Coverage:**
- 90%+ for domain entities
- 100% for value objects
- 100% for business rule validation
- All exception paths tested

**Focus Areas:**
- Invariant enforcement
- State transitions
- Domain event raising
- Business logic correctness

---

## 12. Domain Modeling Rules and Constraints

### 12.1. Mandatory Rules

#### 12.1.1. Dependency Rules

| Rule | Requirement | Rationale |
|------|------------|-----------|
| Zero External Dependencies | Domain project references no external packages | Framework independence |
| No Infrastructure References | No EF Core, Dapper, Azure SDK, etc. | Persistence ignorance |
| No Framework Attributes | No [Key], [Column], [Table], [Required] | Clean domain model |
| Standard .NET Only | Only System.* namespaces | Technology agnostic |

#### 12.1.2. Design Rules

| Rule | Requirement | Rationale |
|------|------------|-----------|
| Private Setters | All entity properties use private set | Controlled state changes |
| Factory Methods | Complex construction via factory methods | Invariant enforcement |
| Behavior Encapsulation | Methods express operations, not just getters | Rich domain model |
| Immutable Value Objects | Value objects cannot be modified | Value semantics |

#### 12.1.3. Naming Rules

| Rule | Requirement | Example |
|------|------------|---------|
| Entity Classes | Singular noun | Product, Order, Customer |
| Value Object Classes | Singular noun | Money, Email, Address |
| Domain Events | Past tense verb + entity | ProductCreated, OrderPlaced |
| Methods | Action verbs | PlaceOrder(), UpdatePrice(), Cancel() |

### 12.2. Anti-Patterns to Avoid

#### 12.2.1. Anemic Domain Model

**Problem:**
```csharp
// AVOID: Anemic model - just data containers
public class Order
{
    public Guid Id { get; set; }
    public List<OrderLine> Lines { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
}

// Business logic in service layer
public class OrderService
{
    public void PlaceOrder(Order order)
    {
        if (order.Lines.Count == 0)
            throw new Exception("No lines");
        
        order.Status = "Placed";
        order.Total = order.Lines.Sum(l => l.Price * l.Quantity);
    }
}
```

**Solution:**
```csharp
// CORRECT: Rich domain model
public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderLine> _lines = new();
    public OrderStatus Status { get; private set; }

    public void PlaceOrder()
    {
        if (!_lines.Any())
            throw new DomainException("Cannot place order without line items");
        
        Status = OrderStatus.Placed;
        RecalculateTotal();
    }
}
```

#### 12.2.2. Primitive Obsession

**Problem:**
```csharp
// AVOID: Using primitives for domain concepts
public class Product
{
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public string Email { get; set; }
}
```

**Solution:**
```csharp
// CORRECT: Use value objects
public class Product
{
    public Money Price { get; private set; }
    public Email ContactEmail { get; private set; }
}
```

#### 12.2.3. Public Setters

**Problem:**
```csharp
// AVOID: Public setters bypass validation
public class Product
{
    public string Name { get; set; } // Can be set to null or empty!
    public decimal Price { get; set; } // Can be set to negative!
}
```

**Solution:**
```csharp
// CORRECT: Private setters + methods
public class Product
{
    public string Name { get; private set; }
    public Money Price { get; private set; }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new DomainException("Price must be positive");
        
        Price = newPrice;
    }
}
```

---

## 13. Performance Considerations

### 13.1. Domain Model Performance

#### 13.1.1. Value Object Performance

**Optimization:**
- Use structs for small, frequently used value objects
- Cache hash codes for complex value objects
- Use record types for automatic equality implementation

**Example:**
```csharp
// Struct for performance-critical value object
public readonly struct ProductCode : IEquatable<ProductCode>
{
    private readonly string _value;

    public ProductCode(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool Equals(ProductCode other) => _value == other._value;
    public override bool Equals(object? obj) => obj is ProductCode other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
}
```

#### 13.1.2. Collection Performance

**Use Read-Only Collections:**
```csharp
private readonly List<OrderLine> _lines = new();
public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
```

**Benefits:**
- Prevents external modification
- No performance overhead (AsReadOnly is a wrapper)
- Clear intent

#### 13.1.3. Domain Event Performance

**Avoid:**
- Creating events for every property change
- Large event payloads
- Synchronous event processing in domain

**Recommend:**
- Events for significant business occurrences only
- Minimal event data
- Asynchronous event processing after persistence

---

## 14. Glossary

**Aggregate:** A cluster of domain objects (entities and value objects) treated as a single unit for data changes, with one aggregate root managing consistency.

**Aggregate Root:** The entry point entity for an aggregate that enforces invariants and coordinates changes to entities within the aggregate boundary.

**Anemic Domain Model:** An anti-pattern where domain objects contain only data with no behavior, with business logic residing in service layers.

**Domain Event:** An immutable object representing a significant business occurrence within the domain, used to decouple domain logic and trigger side effects.

**Domain Service:** A stateless service containing domain logic that doesn't naturally fit within a single entity or value object.

**Entity:** A domain object with a unique identity that persists throughout its lifecycle, distinguished by its identifier rather than attributes.

**Invariant:** A business rule or constraint that must always be true for a domain object to be in a valid state.

**Persistence Ignorance:** The principle that domain objects should be unaware of how they are persisted, containing no database or ORM-specific code.

**Primitive Obsession:** An anti-pattern of using primitive types (string, int, decimal) instead of value objects to represent domain concepts.

**Rich Domain Model:** A domain model where entities and value objects contain both data and behavior, encapsulating business logic.

**Specification:** A pattern that encapsulates business rules or query criteria in reusable, composable, and testable objects.

**Ubiquitous Language:** A shared vocabulary between developers and domain experts that is reflected directly in the code.

**Value Object:** An immutable domain object defined by its attributes rather than identity, implementing value equality semantics.

---

## 15. Recommendations and Next Steps

### 15.1. For Development Teams

#### 15.1.1. Implementation Checklist

**When Creating New Entities:**
- [ ] Inherit from Entity<TId> base class
- [ ] Use strongly-typed ID (value object)
- [ ] Implement private setters
- [ ] Create factory method with validation
- [ ] Add domain events for significant changes
- [ ] Write comprehensive unit tests
- [ ] Document business rules

**When Creating Value Objects:**
- [ ] Inherit from ValueObject base class
- [ ] Make all properties read-only
- [ ] Validate in constructor or factory method
- [ ] Implement GetEqualityComponents
- [ ] Consider using record types
- [ ] Add operation methods (Add, Subtract, etc.) if applicable
- [ ] Write equality and validation tests

**When Creating Aggregates:**
- [ ] Identify consistency boundary
- [ ] Define single aggregate root
- [ ] Keep aggregate small
- [ ] Control access through root only
- [ ] Use IDs for references to other aggregates
- [ ] Define clear invariants
- [ ] Test aggregate as a unit

#### 15.1.2. Code Review Focus Areas

**Domain Model Review:**
- No framework dependencies
- No public setters on entities
- Invariants enforced in constructors and methods
- Domain events raised for significant changes
- Value objects used instead of primitives
- Rich behavior, not anemic model

**Aggregate Review:**
- Clear aggregate boundaries
- Single root entity
- Child entities accessed through root
- References to other aggregates via IDs
- Invariants maintained within aggregate

### 15.2. For Technical Leads

#### 15.2.1. Quality Gates

**Pre-Deployment Checklist:**
- [ ] Domain project has zero external dependencies
- [ ] All entities have private setters
- [ ] Value objects are immutable
- [ ] Business rules encapsulated in domain
- [ ] Domain events for significant operations
- [ ] 90%+ test coverage on domain logic
- [ ] Architecture tests enforce domain isolation

#### 15.2.2. Metrics and Monitoring

**Code Quality Metrics:**
- Domain project dependency count (should be 0)
- Public setter count in entities (should be 0)
- Test coverage percentage (target: 90%+)
- Cyclomatic complexity of entity methods

**Pattern Usage Metrics:**
- Number of value objects vs. primitives
- Domain event usage frequency
- Specification pattern adoption
- Factory method usage

### 15.3. For Architects

#### 15.3.1. Architecture Governance

**Periodic Reviews:**
- Domain model richness (behavior vs. data)
- Aggregate boundary appropriateness
- Domain event usage patterns
- Value object adoption rate
- Test coverage and quality

**Architecture Tests:**
```csharp
[Fact]
public void Domain_Should_NotHaveExternalDependencies()
{
    var assembly = typeof(Product).Assembly;
    var dependencies = assembly.GetReferencedAssemblies();
    
    dependencies.Should().OnlyContain(a => 
        a.Name.StartsWith("System") ||
        a.Name.StartsWith("netstandard"));
}

[Fact]
public void Entities_Should_HavePrivateSetters()
{
    var entityTypes = typeof(Product).Assembly
        .GetTypes()
        .Where(t => t.IsSubclassOf(typeof(Entity<>)));
    
    foreach (var entityType in entityTypes)
    {
        var publicSetters = entityType.GetProperties()
            .Where(p => p.SetMethod?.IsPublic == true)
            .ToList();
            
        publicSetters.Should().BeEmpty(
            $"{entityType.Name} should not have public setters");
    }
}
```

#### 15.3.2. Evolution Planning

**Domain Model Refinement:**
- Identify anemic areas for enrichment
- Extract value objects from primitives
- Define missing domain events
- Refine aggregate boundaries
- Add specifications for complex rules

**Advanced Patterns:**
- Event sourcing evaluation
- CQRS with separate read models
- Domain event outbox pattern
- Saga pattern for distributed transactions

### 15.4. Related Documentation

**Must Read:**
- High-Level Architecture (ARCH-HL-001)
- CQRS Pipeline Specification (ARCH-CQRS-002)
- API Flow Specification (ARCH-API-003)

**Recommended Reading:**
- Entity Framework Core Configuration Guide
- Repository Implementation Patterns
- Testing Strategy for Domain Models
- Domain Event Infrastructure Setup

---

## 16. References

### 16.1. Domain-Driven Design

**Books:**
- Eric Evans: "Domain-Driven Design: Tackling Complexity in the Heart of Software"
- Vaughn Vernon: "Implementing Domain-Driven Design"
- Vaughn Vernon: "Domain-Driven Design Distilled"
- Scott Millett: "Patterns, Principles, and Practices of Domain-Driven Design"

**Online Resources:**
- Martin Fowler: Domain-Driven Design - https://martinfowler.com/tags/domain%20driven%20design.html
- DDD Community: https://www.dddcommunity.org/

### 16.2. Architectural Patterns

**SOLID Principles:**
- Robert C. Martin: Clean Architecture
- Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion

**Design Patterns:**
- Gang of Four: Design Patterns
- Martin Fowler: Patterns of Enterprise Application Architecture

### 16.3. .NET Resources

**Microsoft Documentation:**
- .NET Design Guidelines: https://docs.microsoft.com/dotnet/standard/design-guidelines/
- C# Coding Conventions: https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions

### 16.4. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- ARCH-HL-001: High-Level Architecture
- ARCH-CQRS-002: CQRS Pipeline Specification
- ARCH-API-003: API Flow Specification

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial domain modeling specification with standardized structure and DDD patterns |

---

**END OF DOCUMENT**