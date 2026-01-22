using FluentValidation;

namespace L2.Application.UseCases.Catalog.Bidder.RegisterItem;

public class RegisterItemValidator : AbstractValidator<RegisterItemCommand> {
  public RegisterItemValidator() {
    RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    RuleFor(x => x.Description).NotEmpty();
    RuleFor(x => x.StartingPrice).GreaterThan(0).WithMessage("Giá khởi điểm phải lớn hơn 0");
    RuleFor(x => x.CategoryIds).NotEmpty().WithMessage("Phải chọn ít nhất một danh mục");
  }
}