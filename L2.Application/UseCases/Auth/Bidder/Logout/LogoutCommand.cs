using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.Logout;

public record LogoutCommand(string RefreshToken, bool AllDevices) : ICommand<Unit>;
