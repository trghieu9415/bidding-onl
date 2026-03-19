using L2.Application.Abstractions;

namespace L2.Application.UseCases.Transaction.System.ExpireOrder;

public record ExpireOrderCommand(Guid Id) : ICommand<bool>;
