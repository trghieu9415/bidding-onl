using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.SearchItem;

public record SearchItemQuery(AuctionSearchFilter SearchFilter) : IRequest<SearchItemResult>;

public record SearchItemResult(List<AuctionSearchDto> Items, Meta Meta);
