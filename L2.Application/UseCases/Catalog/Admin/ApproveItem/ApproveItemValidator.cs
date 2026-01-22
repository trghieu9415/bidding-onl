using FluentValidation;

namespace L2.Application.UseCases.Catalog.Admin.ApproveItem;

public class ApproveItemValidator : AbstractValidator<ApproveItemCommand> {
  public ApproveItemValidator() {
    RuleFor(x => x.Id).NotEmpty().WithMessage("Id sản phẩm là bắt buộc");
  }
}