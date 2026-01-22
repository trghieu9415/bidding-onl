using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.ChangePassword;

public record ChangePasswordCommand(string OldPassword, string NewPassword) : ICommand<Unit>;