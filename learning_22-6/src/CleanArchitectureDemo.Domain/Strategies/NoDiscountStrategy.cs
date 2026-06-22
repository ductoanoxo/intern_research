namespace CleanArchitectureDemo.Domain.Strategies;

public class NoDiscountStrategy : IBookDiscountStrategy
{
    public decimal ApplyDiscount(decimal originalPrice)
    {
        return originalPrice;
    }
}
