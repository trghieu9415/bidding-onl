using L2.Application.DTOs;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetBidderOrder;

public record GetBidderOrderQuery(Guid Id) : IRequest<GetOrderResult>;

public record GetOrderResult(OrderDto Order);
