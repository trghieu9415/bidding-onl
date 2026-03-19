using L2.Application.Abstractions;

namespace L2.Application.UseCases.Transaction.System.RefundOrder;

public record RefundOrderCommand(Guid Id) : ICommand<bool>;
