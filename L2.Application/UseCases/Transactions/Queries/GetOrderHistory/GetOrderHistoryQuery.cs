using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetOrderHistory;

public record GetOrderHistoryQuery(Guid UserId, OrderFilter Filter) : IRequest<GetOrderHistoryResult>;

public record GetOrderHistoryResult(List<OrderDto> Orders, Meta Meta);
