using System.Text.Json;
using FluentValidation;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.VerifyPayment;

public record VerifyClientPaymentCommand(Guid UserId, VerifyClientPaymentRequest Data) : IRequest<bool>;

public record VerifyClientPaymentRequest(Guid Id, JsonElement Payload);

public sealed class VerifyClientPaymentValidator : AbstractValidator<VerifyClientPaymentRequest> {
  public VerifyClientPaymentValidator() {
    RuleFor(x => x.Id)
      .NotEmpty()
      .WithMessage("Id thanh toán không được để trống.");

    RuleFor(x => x.Payload)
      .Must(x => x.ValueKind != JsonValueKind.Undefined && x.ValueKind != JsonValueKind.Null)
      .WithMessage("Dữ liệu thanh toán không được để trống.");
  }
}
