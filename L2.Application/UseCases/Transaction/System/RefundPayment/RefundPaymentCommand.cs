using L2.Application.Abstractions;

namespace L2.Application.UseCases.Transaction.System.RefundPayment;

public record RefundPaymentCommand(Guid Id) : ICommand<bool>;
