using MediatR;

namespace CleanArchitectureDemo.Application.Books.Commands.ApplyDiscount;

public record ApplyDiscountCommand : IRequest<bool>
{
    public Guid BookId { get; init; }
    public string DiscountType { get; init; } = string.Empty; // "Percentage", "Fixed"
    public decimal Value { get; init; }
}
