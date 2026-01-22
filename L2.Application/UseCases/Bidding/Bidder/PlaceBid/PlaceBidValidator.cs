using FluentValidation;

namespace L2.Application.UseCases.Bidding.Bidder.PlaceBid;

public class PlaceBidValidator : AbstractValidator<PlaceBidCommand> {
  public PlaceBidValidator() {
    RuleFor(x => x.AuctionId).NotEmpty();
    RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Giá đặt phải lớn hơn 0");
  }
}