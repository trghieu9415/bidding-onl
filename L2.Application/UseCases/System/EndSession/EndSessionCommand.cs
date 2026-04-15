using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.System.EndSession;

public record EndSessionCommand(Guid Id) : IRequest<Unit>, ITransactional;
