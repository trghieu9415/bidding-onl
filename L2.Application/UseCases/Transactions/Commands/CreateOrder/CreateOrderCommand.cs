using FluentValidation;
using L1.Core.Domain.Transaction.ValueObjects;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.CreateOrder;

public record CreateOrderCommand(
  Guid AuctionId,
  Address Address
) : IRequest<CreateOrderResult>, ITransactional;

public record CreateOrderResult(Guid Id);

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand> {
  public CreateOrderValidator() {
    RuleFor(x => x.AuctionId).NotEmpty().WithMessage("Mã đấu giá không được để trống");
    RuleFor(x => x.Address.ReceiverName).NotEmpty().WithMessage("Tên người nhận không được để trống");
    RuleFor(x => x.Address.PhoneNumber).NotEmpty().WithMessage("Số điện thoại không được để trống");
    RuleFor(x => x.Address.ShippingAddress).NotEmpty().WithMessage("Địa chỉ giao hàng không được để trống");
  }
}
