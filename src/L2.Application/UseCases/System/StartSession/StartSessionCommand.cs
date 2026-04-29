using MediatR;

namespace L2.Application.UseCases.System.StartSession;

public record StartSessionCommand(Guid Id) : IRequest<bool>;
