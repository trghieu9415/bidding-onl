using FluentValidation;
using L1.Core.Domain.Transaction.ValueObjects;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.CreateOrder;

public record CreateOrderCommand(
  Guid UserId,
  string UserFullName,
  string UserEmail,
  CreateOrderRequest Data
) : IRequest<CreateOrderResult>, ITransactional;

public record CreateOrderRequest(Guid AuctionId, Address Address);

public record CreateOrderResult(Guid Id);

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand> {}
