using MediatR;
using OrderManagement.Application.Discounts;
using OrderManagement.Domain.Orders.Discounts;

namespace OrderManagement.Application.Orders.Queries.GetDiscountPreview;

public sealed record GetDiscountPreviewQuery(DiscountType DiscountType, decimal DiscountValue, decimal SubTotal)
    : IRequest<DiscountResultDto>;
