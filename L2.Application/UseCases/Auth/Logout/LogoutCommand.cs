using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Logout;

public record LogoutCommand(string RefreshToken, bool AllDevices) : IRequest<Unit>, ITransactional;
