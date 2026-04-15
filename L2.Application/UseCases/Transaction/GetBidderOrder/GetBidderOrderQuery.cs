using L2.Application.Abstractions;
using L2.Application.DTOs;

namespace L2.Application.UseCases.Transaction.GetBidderOrder;

public record GetBidderOrderQuery(Guid Id) : IQuery<GetOrderResult>;

public record GetOrderResult(OrderDto Order);
