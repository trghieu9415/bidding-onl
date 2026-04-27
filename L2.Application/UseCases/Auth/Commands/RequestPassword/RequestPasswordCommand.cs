using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.RequestPassword;

public record RequestPasswordCommand(string Email) : IRequest<bool>, ITransactional;

public sealed class RequestPasswordValidator : AbstractValidator<RequestPasswordCommand> {
  public RequestPasswordValidator() {
    RuleFor(x => x.Email)
      .NotEmpty()
      .WithMessage("Email không được để trống.")
      .EmailAddress()
      .WithMessage("Email không hợp lệ.");
  }
}
