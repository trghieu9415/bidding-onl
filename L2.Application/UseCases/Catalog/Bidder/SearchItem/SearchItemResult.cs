using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Catalog.Bidder.SearchItem;

public record SearchItemResult(List<AuctionSearchDto> Items, Meta Meta);
