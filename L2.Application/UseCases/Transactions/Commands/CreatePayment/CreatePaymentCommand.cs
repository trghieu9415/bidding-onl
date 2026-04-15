using FluentValidation;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.CreatePayment;

public record CreatePaymentCommand(Guid OrderId, PaymentMethod Method) : IRequest<CreatePaymentResult>, ITransactional;

public record CreatePaymentResult(string PaymentUrl);

public class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand> {
  public CreatePaymentValidator() {
    RuleFor(x => x.OrderId).NotEmpty().WithMessage("Id đơn hàng không được để trống");
    RuleFor(x => x.Method).IsInEnum().WithMessage("Phương thức thanh toán không hợp lệ");
  }
}
