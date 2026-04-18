using L2.Application.DTOs;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetBidderOrder;

public record GetBidderOrderQuery(Guid Id, Guid UserId) : IRequest<GetBidderOrderResult>;

public record GetBidderOrderResult(OrderDto Order, List<PaymentDto>? Payments);
