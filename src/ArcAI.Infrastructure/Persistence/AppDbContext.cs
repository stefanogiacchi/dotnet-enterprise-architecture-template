using ArcAI.Application.Common.Interfaces;
using ArcAI.Domain.Common;
using ArcAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ArcAI.Infrastructure.Persistence;

/// <summary>
/// Main application database context.
/// Handles entity configuration, change tracking, and domain event dispatching.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly IDateTimeService? _dateTimeService;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService? currentUserService = null,
        IDateTimeService? dateTimeService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    // DbSets for entities
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply any additional custom configurations
        ApplyCustomConfigurations(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Apply audit information to entities before saving
        ApplyAuditInformation();

        // Collect domain events before saving
        var domainEvents = CollectDomainEvents();

        // Save changes to database
        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events after successful save
        await DispatchDomainEventsAsync(domainEvents, cancellationToken);

        return result;
    }

    public override int SaveChanges()
    {
        // Apply audit information
        ApplyAuditInformation();

        // Note: Synchronous SaveChanges doesn't dispatch domain events
        // Use SaveChangesAsync for full domain event support
        return base.SaveChanges();
    }

    /// <summary>
    /// Applies audit information to tracked entities that implement IAuditableEntity.
    /// </summary>
    private void ApplyAuditInformation()
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();
        var currentUserId = _currentUserService?.UserId;
        var currentTime = _dateTimeService?.UtcNow ?? DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                entry.Entity.CreatedAt = currentTime;
                entry.Entity.CreatedBy = currentUserId;
                break;

                case EntityState.Modified:
                entry.Entity.UpdatedAt = currentTime;
                entry.Entity.UpdatedBy = currentUserId;
                break;

                case EntityState.Deleted when entry.Entity is ISoftDeletable softDeletable:
                // Soft delete implementation
                entry.State = EntityState.Modified;
                softDeletable.DeletedAt = currentTime;
                softDeletable.DeletedBy = currentUserId;
                softDeletable.IsDeleted = true;
                break;
            }
        }
    }

    /// <summary>
    /// Collects domain events from aggregate roots.
    /// </summary>
    private List<IDomainEvent> CollectDomainEvents()
    {
        var domainEvents = new List<IDomainEvent>();

        var aggregateRoots = ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregateRoot in aggregateRoots)
        {
            domainEvents.AddRange(aggregateRoot.DomainEvents);
            aggregateRoot.ClearDomainEvents();
        }

        return domainEvents;
    }

    /// <summary>
    /// Dispatches domain events to registered handlers.
    /// </summary>
    private async Task DispatchDomainEventsAsync(
        List<IDomainEvent> domainEvents,
        CancellationToken cancellationToken)
    {
        // Note: This is a simplified implementation.
        // In production, you would inject IMediator here and publish events.
        // For now, we'll just log that events were collected.

        // TODO: Inject IMediator and publish events
        // foreach (var domainEvent in domainEvents)
        // {
        //     await _mediator.Publish(domainEvent, cancellationToken);
        // }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Applies custom configurations that aren't in separate IEntityTypeConfiguration classes.
    /// </summary>
    private void ApplyCustomConfigurations(ModelBuilder modelBuilder)
    {
        // Apply global query filters
        ApplyGlobalQueryFilters(modelBuilder);

        // Configure value conversions if needed
        ConfigureValueConversions(modelBuilder);

        // Configure any other global settings
    }

    /// <summary>
    /// Applies global query filters (e.g., for soft delete).
    /// </summary>
    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // Apply soft delete filter to all entities implementing ISoftDeletable
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var filter = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Equal(
                        property,
                        System.Linq.Expressions.Expression.Constant(false)),
                    parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    /// <summary>
    /// Configures value conversions for value objects and other types.
    /// </summary>
    private void ConfigureValueConversions(ModelBuilder modelBuilder)
    {
        // Value conversions are typically handled in IEntityTypeConfiguration classes
        // This method is here for any global value conversions if needed
    }
}

/// <summary>
/// Interface for entities that support soft deletion.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}