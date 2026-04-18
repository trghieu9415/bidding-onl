using System.Text.Json;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.VerifyPayment;

public record VerifyPaymentCommand(Guid UserId, VerifyPaymentRequest Data) : IRequest<bool>;

public record VerifyPaymentRequest(Guid Id, JsonElement Payload);
