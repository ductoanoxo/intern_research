using System.Threading;
using System.Threading.Tasks;
using DddCqrsJwtDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DddCqrsJwtDemo.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Book> Books { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
