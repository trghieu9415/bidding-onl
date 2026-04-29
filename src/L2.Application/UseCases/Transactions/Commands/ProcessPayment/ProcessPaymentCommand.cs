using System.Text.Json;
using FluentValidation;
using L1.Core.Domain.Transaction.Enums;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.ProcessPayment;

public record ProcessPaymentCommand(ProcessPaymentRequest Data) : IRequest<bool>;

public record ProcessPaymentRequest(PaymentMethod Method, JsonElement Payload);

public sealed class ProcessPaymentValidator : AbstractValidator<ProcessPaymentRequest> {
  public ProcessPaymentValidator() {
    RuleFor(x => x.Method)
      .IsInEnum()
      .WithMessage("Phương thức thanh toán không hợp lệ.");

    RuleFor(x => x.Payload)
      .Must(x => x.ValueKind != JsonValueKind.Undefined && x.ValueKind != JsonValueKind.Null)
      .WithMessage("Dữ liệu Webhook không được để trống.");
  }
}
