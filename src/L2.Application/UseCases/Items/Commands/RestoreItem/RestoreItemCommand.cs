using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RestoreItem;

public record RestoreItemCommand(Guid Id) : IRequest<bool>, ITransactional;
