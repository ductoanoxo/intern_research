using FluentValidation;

namespace DddCqrsJwtDemo.Application.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("ZipCode is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required in the order.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("BookId is required.");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}
