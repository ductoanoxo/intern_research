using FluentValidation;

namespace CleanArchitectureDemo.Application.Books.Commands.ApplyDiscount;

public class ApplyDiscountCommandValidator : AbstractValidator<ApplyDiscountCommand>
{
    public ApplyDiscountCommandValidator()
    {
        RuleFor(v => v.BookId)
            .NotEmpty().WithMessage("BookId is required.");

        RuleFor(v => v.DiscountType)
            .NotEmpty().WithMessage("DiscountType is required.")
            .Must(type => type.ToLower() == "percentage" || type.ToLower() == "fixed")
            .WithMessage("DiscountType must be either 'Percentage' or 'Fixed'.");

        RuleFor(v => v.Value)
            .GreaterThanOrEqualTo(0).WithMessage("Discount value must be greater than or equal to 0.");

        RuleFor(v => v.Value)
            .LessThanOrEqualTo(100).When(v => v.DiscountType.ToLower() == "percentage")
            .WithMessage("Percentage discount cannot exceed 100%.");
    }
}
