using CleanArchitectureDemo.Domain.Entities;

namespace CleanArchitectureDemo.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Book persistence operations.
/// Demonstrates:
/// - Dependency Inversion Principle (DIP): The high-level Application layer defines the interface, 
///   which the low-level Infrastructure layer will implement.
/// - Interface Segregation Principle (ISP): Keeps the interface specific and focused on Book needs.
/// </summary>
public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    Task UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task DeleteAsync(Book book, CancellationToken cancellationToken = default);
}
