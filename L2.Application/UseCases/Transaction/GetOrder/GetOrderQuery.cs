using L2.Application.Abstractions;
using L2.Application.DTOs;

namespace L2.Application.UseCases.Transaction.GetOrder;

public record GetOrderQuery(Guid Id) : IQuery<GetOrderResult>;

public record GetOrderResult(OrderDto Order, PaymentDto? Payment);
