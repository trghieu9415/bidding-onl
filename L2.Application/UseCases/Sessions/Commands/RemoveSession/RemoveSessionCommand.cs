using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.RemoveSession;

public record RemoveSessionCommand(Guid Id) : IRequest<Unit>, ITransactional;
