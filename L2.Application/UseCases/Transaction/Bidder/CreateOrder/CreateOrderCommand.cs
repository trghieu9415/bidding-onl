using FluentValidation;
using L2.Application.Abstractions;

namespace L2.Application.UseCases.Transaction.Bidder.CreateOrder;

public record CreateOrderCommand(
  Guid AuctionId,
  string ReceiverName,
  string PhoneNumber,
  string ShippingAddress
) : ICommand<CreateOrderResult>;

public record CreateOrderResult(Guid Id);

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand> {
  public CreateOrderValidator() {
    RuleFor(x => x.AuctionId).NotEmpty().WithMessage("Mã đấu giá không được để trống");
    RuleFor(x => x.ReceiverName).NotEmpty().WithMessage("Tên người nhận không được để trống");
    RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Số điện thoại không được để trống");
    RuleFor(x => x.ShippingAddress).NotEmpty().WithMessage("Địa chỉ giao hàng không được để trống");
  }
}
