using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.RemoveSession;

public record RemoveSessionCommand(Guid Id) : ICommand<Unit>;