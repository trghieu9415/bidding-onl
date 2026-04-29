using FluentValidation;
using L2.Application.Abstractions;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.Login;

public record LoginCommand(LoginRequest Data, UserRole Role) : IRequest<LoginResult>, ITransactional;

public record LoginRequest(string Email, string Password);

public sealed class LoginValidator : AbstractValidator<LoginRequest> {
  public LoginValidator() {
    RuleFor(x => x.Email)
      .NotEmpty()
      .WithMessage("Email không được để trống.")
      .EmailAddress()
      .WithMessage("Email không hợp lệ.");

    RuleFor(x => x.Password)
      .NotEmpty()
      .WithMessage("Mật khẩu không được để trống.");
  }
}

public record LoginResult(AuthTokens Tokens);
