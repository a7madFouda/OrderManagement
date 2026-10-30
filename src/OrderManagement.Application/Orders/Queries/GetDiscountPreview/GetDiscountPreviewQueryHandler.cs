using MediatR;
using OrderManagement.Application.Discounts;

namespace OrderManagement.Application.Orders.Queries.GetDiscountPreview;

internal sealed class GetDiscountPreviewQueryHandler(IDiscountService discountService)
    : IRequestHandler<GetDiscountPreviewQuery, DiscountResultDto>
{
    public Task<DiscountResultDto> Handle(GetDiscountPreviewQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(discountService.Calculate(request.DiscountType, request.DiscountValue, request.SubTotal));
    }
}
