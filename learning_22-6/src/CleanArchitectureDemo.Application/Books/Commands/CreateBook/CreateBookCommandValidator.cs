using FluentValidation;

namespace CleanArchitectureDemo.Application.Books.Commands.CreateBook;

/// <summary>
/// Validator for CreateBookCommand.
/// Demonstrates:
/// - Single Responsibility Principle (SRP): Isolates validation logic from request handling.
/// </summary>
public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MaximumLength(100).WithMessage("Author must not exceed 100 characters.");

        RuleFor(v => v.Isbn)
            .NotEmpty().WithMessage("ISBN is required.")
            .Matches(@"^(?:97[89]-?)?\d{1,5}-?\d{1,7}-?\d{1,6}-?[\dXx]$")
            .WithMessage("Invalid ISBN format. Example: 978-3-16-148410-0 or 0-12-345678-9.");

        RuleFor(v => v.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

        RuleFor(v => v.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(v => v.Category)
            .IsInEnum().WithMessage("Invalid book category.");
    }
}
