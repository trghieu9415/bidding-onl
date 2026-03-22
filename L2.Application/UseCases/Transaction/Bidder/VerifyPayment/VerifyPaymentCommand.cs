using L2.Application.Abstractions;

namespace L2.Application.UseCases.Transaction.Bidder.VerifyPayment;

public record VerifyPaymentCommand(Guid Id, object Payload) : ICommand<bool>;
