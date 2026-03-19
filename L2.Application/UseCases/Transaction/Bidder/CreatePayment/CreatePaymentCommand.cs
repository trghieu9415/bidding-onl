using FluentValidation;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Abstractions;

namespace L2.Application.UseCases.Transaction.Bidder.CreatePayment;

public record CreatePaymentCommand(Guid OrderId, PaymentMethod Method) : ICommand<CreatePaymentResult>;

public record CreatePaymentResult(string PaymentUrl);

public class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand> {
  public CreatePaymentValidator() {
    RuleFor(x => x.OrderId).NotEmpty().WithMessage("Id đơn hàng không được để trống");
    RuleFor(x => x.Method).IsInEnum().WithMessage("Phương thức thanh toán không hợp lệ");
  }
}
