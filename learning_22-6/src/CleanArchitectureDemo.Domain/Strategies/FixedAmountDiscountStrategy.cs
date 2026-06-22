namespace CleanArchitectureDemo.Domain.Strategies;

public class FixedAmountDiscountStrategy : IBookDiscountStrategy
{
    private readonly decimal _amount;

    public FixedAmountDiscountStrategy(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Discount amount cannot be negative.");

        _amount = amount;
    }

    public decimal ApplyDiscount(decimal originalPrice)
    {
        return Math.Max(0, originalPrice - _amount);
    }
}
