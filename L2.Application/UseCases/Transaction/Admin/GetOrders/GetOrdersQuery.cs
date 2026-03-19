using L2.Application.Abstractions;
using L2.Application.DTOs;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Transaction.Admin.GetOrders;

public record GetOrdersQuery(SieveModel SieveModel) : IQuery<GetOrdersResult>;

public record GetOrdersResult(List<OrderDto> Orders, Meta Meta);
