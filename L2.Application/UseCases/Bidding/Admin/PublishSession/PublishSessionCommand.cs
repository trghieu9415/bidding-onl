using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.PublishSession;

public record PublishSessionCommand(Guid Id) : ICommand<Unit>;