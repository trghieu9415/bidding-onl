using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Transaction.GetOrders;

public record GetOrdersQuery(SieveModel SieveModel) : IRequest<GetOrdersResult>;

public record GetOrdersResult(List<OrderDto> Orders, Meta Meta);
