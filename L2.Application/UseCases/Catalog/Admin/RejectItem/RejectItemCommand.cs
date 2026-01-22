using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.RejectItem;

public record RejectItemCommand(Guid Id) : ICommand<Unit>;