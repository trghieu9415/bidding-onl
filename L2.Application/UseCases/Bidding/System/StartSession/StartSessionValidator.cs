using FluentValidation;

namespace L2.Application.UseCases.Bidding.System.StartSession;

public class StartSessionValidator : AbstractValidator<StartSessionCommand> {
  public StartSessionValidator() {
    RuleFor(x => x.Id).NotEmpty();
  }
}