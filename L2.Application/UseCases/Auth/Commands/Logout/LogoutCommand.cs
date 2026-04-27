using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken, bool AllDevices) : IRequest<bool>, ITransactional;

public sealed class LogoutValidator : AbstractValidator<LogoutCommand> {
  public LogoutValidator() {
    RuleFor(x => x.RefreshToken)
      .NotEmpty()
      .WithMessage("Refresh token không được để trống.");
  }
}
