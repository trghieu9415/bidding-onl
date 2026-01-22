using FluentValidation;

namespace L2.Application.UseCases.Auth.Bidder.Login;

public class LoginValidator : AbstractValidator<LoginCommand> {
  public LoginValidator() {
    RuleFor(x => x.Email).NotEmpty().EmailAddress();
    RuleFor(x => x.Password).NotEmpty();
  }
}