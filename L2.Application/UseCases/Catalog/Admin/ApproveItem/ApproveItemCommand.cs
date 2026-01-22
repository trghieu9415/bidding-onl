using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.ApproveItem;

public record ApproveItemCommand(Guid Id) : ICommand<Unit>;