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

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderRequest> {
  public CreateOrderValidator() {
    RuleFor(x => x.AuctionId)
      .NotEmpty()
      .WithMessage("Id phiên đấu giá không được để trống.");

    RuleFor(x => x.Address)
      .NotNull()
      .WithMessage("Địa chỉ giao hàng không được để trống.");
  }
}

public record CreateOrderResult(Guid Id);
