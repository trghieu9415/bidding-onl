using FluentValidation;

namespace L2.Application.UseCases.Catalog.Admin.RejectItem;

public class RejectItemValidator : AbstractValidator<RejectItemCommand> {
  public RejectItemValidator() {
    RuleFor(x => x.Id).NotEmpty().WithMessage("Id sản phẩm là bắt buộc");
  }
}