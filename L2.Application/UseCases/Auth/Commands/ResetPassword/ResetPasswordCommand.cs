using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Unit>, ITransactional;
