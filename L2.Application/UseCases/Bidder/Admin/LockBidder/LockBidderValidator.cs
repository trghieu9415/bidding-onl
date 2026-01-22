using FluentValidation;

namespace L2.Application.UseCases.Bidder.Admin.LockBidder;

public class LockBidderValidator : AbstractValidator<LockBidderCommand> {
  public LockBidderValidator() {
    RuleFor(x => x.Id).NotEmpty().WithMessage("Id người dùng không được để trống");
  }
}