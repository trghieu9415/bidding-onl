using L2.Application.Abstractions;

namespace L2.Application.UseCases.Transaction.System.MarkOrderAsPaid;

public record MarkOrderAsPaidCommand(Guid Id) : ICommand<bool>;
