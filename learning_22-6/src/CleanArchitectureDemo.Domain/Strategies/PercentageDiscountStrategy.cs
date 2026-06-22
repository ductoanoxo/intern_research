namespace CleanArchitectureDemo.Domain.Strategies;

public class PercentageDiscountStrategy : IBookDiscountStrategy
{
    private readonly decimal _percentage;

    public PercentageDiscountStrategy(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100.");

        _percentage = percentage;
    }

    public decimal ApplyDiscount(decimal originalPrice)
    {
        return originalPrice - (originalPrice * (_percentage / 100));
    }
}
