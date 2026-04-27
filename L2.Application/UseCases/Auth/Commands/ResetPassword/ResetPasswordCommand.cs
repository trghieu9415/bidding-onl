using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<bool>, ITransactional;

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand> {
  public ResetPasswordValidator() {
    RuleFor(x => x.Email)
      .NotEmpty()
      .WithMessage("Email không được để trống.")
      .EmailAddress()
      .WithMessage("Email không hợp lệ.");

    RuleFor(x => x.Token)
      .NotEmpty()
      .WithMessage("Token đặt lại mật khẩu không được để trống.");

    RuleFor(x => x.NewPassword)
      .NotEmpty()
      .WithMessage("Mật khẩu mới không được để trống.")
      .MinimumLength(8)
      .WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự.")
      .Matches(@"[A-Z]")
      .WithMessage("Mật khẩu mới phải chứa ít nhất 1 ký tự in hoa.")
      .Matches(@"[a-z]")
      .WithMessage("Mật khẩu mới phải chứa ít nhất 1 ký tự thường.")
      .Matches(@"[0-9]")
      .WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ số.");
  }
}
