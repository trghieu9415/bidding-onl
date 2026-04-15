using System.Text.Json;
using MediatR;

namespace L2.Application.UseCases.Transaction.VerifyPayment;

public record VerifyPaymentCommand(Guid Id, JsonElement Payload) : IRequest<bool>;
