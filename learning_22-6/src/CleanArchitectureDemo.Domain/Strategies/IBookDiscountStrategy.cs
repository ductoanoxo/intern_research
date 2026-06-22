namespace CleanArchitectureDemo.Domain.Strategies;

/// <summary>
/// Strategy pattern interface for book discounts.
/// Demonstrates:
/// - Open/Closed Principle (OCP): New discount strategies can be added without modifying existing code.
/// - Liskov Substitution Principle (LSP): Any implementation can be used transparently in place of this interface.
/// </summary>
public interface IBookDiscountStrategy
{
    decimal ApplyDiscount(decimal originalPrice);
}
