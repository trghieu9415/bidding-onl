using L2.Application.DTOs;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetOrder;

public record GetOrderQuery(Guid Id) : IRequest<GetOrderResult>;

public record GetOrderResult(OrderDto Order, PaymentDto? Payment);
