using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.RestoreSession;

public record RestoreSessionCommand(Guid Id) : IRequest<Unit>, ITransactional;
