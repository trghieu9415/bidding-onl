using FluentValidation;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.SearchItem;

public record SearchItemQuery(AuctionSearchRequest SearchFilter) : IRequest<SearchItemResult>;

public record AuctionSearchRequest(
  string? Keyword = null,
  List<Guid>? CategoryIds = null,
  decimal? MinPrice = null,
  decimal? MaxPrice = null,
  AuctionStatus? Status = null,
  DateTime? FromDate = null,
  DateTime? ToDate = null,
  int Page = 1,
  int PerPage = 10
);

public sealed class AuctionSearchRequestValidator : AbstractValidator<AuctionSearchRequest> {
  public AuctionSearchRequestValidator() {
    RuleFor(x => x.Keyword)
      .MaximumLength(200)
      .WithMessage("Từ khóa tìm kiếm không được vượt quá 200 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.Keyword));

    RuleFor(x => x.MinPrice)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá tối thiểu không được nhỏ hơn 0.")
      .When(x => x.MinPrice.HasValue);

    RuleFor(x => x.MaxPrice)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá tối đa không được nhỏ hơn 0.")
      .When(x => x.MaxPrice.HasValue);

    RuleFor(x => x)
      .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
      .WithMessage("Giá tối thiểu không được lớn hơn giá tối đa.");

    RuleFor(x => x)
      .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
      .WithMessage("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

    RuleFor(x => x.Page)
      .GreaterThan(0)
      .WithMessage("Trang hiện tại phải lớn hơn 0.");

    RuleFor(x => x.PerPage)
      .InclusiveBetween(1, 100)
      .WithMessage("Số lượng bản ghi mỗi trang phải từ 1 đến 100.");
  }
}

public record SearchItemResult(List<AuctionSearchDto> Items, Meta Meta);
