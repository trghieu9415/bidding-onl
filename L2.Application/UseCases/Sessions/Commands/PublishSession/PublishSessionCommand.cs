using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.PublishSession;

public record PublishSessionCommand(Guid Id) : IRequest<bool>, ITransactional;
