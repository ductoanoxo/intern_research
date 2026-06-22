using CleanArchitectureDemo.Application.Common.Interfaces;
using CleanArchitectureDemo.Domain.Strategies;
using MediatR;

namespace CleanArchitectureDemo.Application.Books.Commands.ApplyDiscount;

public class ApplyDiscountCommandHandler : IRequestHandler<ApplyDiscountCommand, bool>
{
    private readonly IBookRepository _bookRepository;

    public ApplyDiscountCommandHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<bool> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book == null) return false;

        IBookDiscountStrategy strategy = request.DiscountType.ToLower() switch
        {
            "percentage" => new PercentageDiscountStrategy(request.Value),
            "fixed" => new FixedAmountDiscountStrategy(request.Value),
            _ => new NoDiscountStrategy()
        };

        // DIP & LSP: We pass the strategy abstraction, which is correctly substituted by a concrete strategy
        book.ApplyDiscount(strategy);

        await _bookRepository.UpdateAsync(book, cancellationToken);
        return true;
    }
}
