using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.System.CancelSession;

public record CancelSessionCommand(Guid Id) : ICommand<Unit>;