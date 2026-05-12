using FluentValidation;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.SearchItem;

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

public sealed class AuctionSearchValidator : AbstractValidator<AuctionSearchRequest> {
  public AuctionSearchValidator() {
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

    RuleFor(x => x.MaxPrice)
      .GreaterThanOrEqualTo(x => x.MinPrice!.Value)
      .WithMessage("Giá tối thiểu không được lớn hơn giá tối đa.")
      .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);

    RuleFor(x => x.ToDate)
      .GreaterThanOrEqualTo(x => x.FromDate)
      .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
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
