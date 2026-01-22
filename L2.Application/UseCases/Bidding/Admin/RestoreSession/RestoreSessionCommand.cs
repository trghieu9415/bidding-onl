using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.RestoreSession;

public record RestoreSessionCommand(Guid Id) : ICommand<Unit>;