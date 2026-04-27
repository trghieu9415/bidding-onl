using FluentValidation;
using L2.Application.Abstractions;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.Register;

public record RegisterCommand(string Email, string FullName, string Password, string? PhoneNumber)
  : IRequest<RegisterResult>, ITransactional;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand> {
  public RegisterValidator() {
    RuleFor(x => x.Email)
      .NotEmpty()
      .WithMessage("Email không được để trống.")
      .EmailAddress()
      .WithMessage("Email không hợp lệ.");

    RuleFor(x => x.FullName)
      .NotEmpty()
      .WithMessage("Họ và tên không được để trống.")
      .MaximumLength(200)
      .WithMessage("Họ và tên không được vượt quá 200 ký tự.");

    RuleFor(x => x.Password)
      .NotEmpty()
      .WithMessage("Mật khẩu không được để trống.")
      .MinimumLength(8)
      .WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
      .Matches(@"[A-Z]")
      .WithMessage("Mật khẩu phải chứa ít nhất 1 ký tự in hoa.")
      .Matches(@"[a-z]")
      .WithMessage("Mật khẩu phải chứa ít nhất 1 ký tự thường.")
      .Matches(@"[0-9]")
      .WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số.");

    RuleFor(x => x.PhoneNumber)
      .Matches(@"^(0|\+84)[0-9]{9,10}$")
      .WithMessage("Số điện thoại không hợp lệ.")
      .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
  }
}

public record RegisterResult(AuthTokens Tokens);
