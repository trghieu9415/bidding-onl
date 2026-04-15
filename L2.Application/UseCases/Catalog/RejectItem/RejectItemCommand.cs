using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.RejectItem;

public record RejectItemCommand(Guid Id, string Reason) : IRequest<Unit>, ITransactional;

public class RejectItemValidator : AbstractValidator<RejectItemCommand> {
  public RejectItemValidator() {
    RuleFor(x => x.Id).NotEmpty().WithMessage("Id sản phẩm là bắt buộc");
  }
}
