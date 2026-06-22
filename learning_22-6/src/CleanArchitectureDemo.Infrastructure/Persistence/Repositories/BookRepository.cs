using CleanArchitectureDemo.Application.Common.Interfaces;
using CleanArchitectureDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureDemo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Book database operations.
/// Demonstrates:
/// - Dependency Inversion Principle (DIP): Concrete implementation of high-level IBookRepository interface.
/// </summary>
public class BookRepository : IBookRepository
{
    private readonly BookStoreDbContext _dbContext;

    public BookRepository(BookStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        await _dbContext.Books.AddAsync(book, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Book book, CancellationToken cancellationToken = default)
    {
        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
