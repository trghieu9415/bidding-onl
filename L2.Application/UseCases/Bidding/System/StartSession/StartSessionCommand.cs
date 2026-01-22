using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.System.StartSession;

public record StartSessionCommand(Guid Id) : ICommand<Unit>;