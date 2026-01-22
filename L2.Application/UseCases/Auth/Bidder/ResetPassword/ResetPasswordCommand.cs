using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : ICommand<Unit>;