using FluentValidation;

namespace L2.Application.UseCases.Auth.Bidder.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand> {
  public RegisterValidator() {
    RuleFor(x => x.Email).NotEmpty().EmailAddress();
    RuleFor(x => x.FullName).NotEmpty().MinimumLength(3);
    RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
  }
}