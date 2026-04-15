using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.PublishSession;

public record PublishSessionCommand(Guid Id) : IRequest<Unit>, ITransactional;
