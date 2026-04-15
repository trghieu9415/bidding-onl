using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Transactions.Queries.GetOrderHistory;

public record GetOrderHistoryQuery(SieveModel SieveModel) : IRequest<GetOrderHistoryResult>;

public record GetOrderHistoryResult(List<OrderDto> Orders, Meta Meta);
