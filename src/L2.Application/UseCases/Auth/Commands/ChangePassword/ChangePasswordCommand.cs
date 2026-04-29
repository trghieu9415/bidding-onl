using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Data) : IRequest<bool>, ITransactional;

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest> {
  public ChangePasswordValidator() {
    RuleFor(x => x.OldPassword)
      .NotEmpty()
      .WithMessage("Mật khẩu cũ không được để trống.");

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

    RuleFor(x => x.NewPassword)
      .NotEqual(x => x.OldPassword)
      .WithMessage("Mật khẩu mới không được trùng với mật khẩu cũ.");
  }
}

public record ChangePasswordRequest(string OldPassword, string NewPassword);
