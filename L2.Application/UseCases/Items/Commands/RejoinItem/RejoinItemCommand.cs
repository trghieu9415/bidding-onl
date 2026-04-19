using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RejoinItem;

public record RejoinItemCommand(Guid Id, Guid UserId) : IRequest<bool>, ITransactional;
