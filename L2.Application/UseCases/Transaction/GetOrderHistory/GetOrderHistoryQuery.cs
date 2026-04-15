using L2.Application.Abstractions;
using L2.Application.DTOs;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Transaction.GetOrderHistory;

public record GetOrderHistoryQuery(SieveModel SieveModel) : IQuery<GetOrderHistoryResult>;

public record GetOrderHistoryResult(List<OrderDto> Orders, Meta Meta);
