using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RevokeItem;

public record RevokeItemCommand(Guid Id, Guid UserId) : IRequest<bool>, ITransactional;
