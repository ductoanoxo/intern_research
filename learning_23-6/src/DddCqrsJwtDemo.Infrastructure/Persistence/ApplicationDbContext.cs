using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Application.Abstractions;
using DddCqrsJwtDemo.Application.Events;
using DddCqrsJwtDemo.Domain.Entities;
using DddCqrsJwtDemo.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddCqrsJwtDemo.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IMediator _mediator;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Gather all domain events from tracked AggregateRoots
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        // 2. Clear domain events to prevent duplicate triggers
        domainEntities.ForEach(x => x.Entity.ClearDomainEvents());

        // 3. Dispatch domain events before committing changes
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent);

            if (notification != null)
            {
                // MediatR will route this to INotificationHandler<DomainEventNotification<TEvent>>
                await _mediator.Publish(notification, cancellationToken);
            }
        }

        // 4. Save everything inside a single transaction
        return await base.SaveChangesAsync(cancellationToken);
    }
}
