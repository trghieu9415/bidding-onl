using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetOrders;

public record GetOrdersQuery(OrderFilter Filter) : IRequest<GetOrdersResult>;

public record GetOrdersResult(List<OrderDto> Orders, Meta Meta);
