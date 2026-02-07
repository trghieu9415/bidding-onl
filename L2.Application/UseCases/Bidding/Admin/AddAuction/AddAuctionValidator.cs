using FluentValidation;

namespace L2.Application.UseCases.Bidding.Admin.AddAuction;

public class AddAuctionValidator : AbstractValidator<AddAuctionCommand> {
  public AddAuctionValidator() {
    RuleFor(x => x.CatalogItemId).NotEmpty();
    RuleFor(x => x.StepPrice).GreaterThan(0).WithMessage("Bước giá phải lớn hơn 0");
    RuleFor(x => x.ReservePrice).GreaterThanOrEqualTo(0).WithMessage("Giá sàn không được âm");
  }
}
