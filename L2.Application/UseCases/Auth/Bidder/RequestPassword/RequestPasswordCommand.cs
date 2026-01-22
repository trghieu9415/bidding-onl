using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.RequestPassword;

public record RequestPasswordCommand(string Email) : ICommand<Unit>;