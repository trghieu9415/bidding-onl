using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.SearchItem;

public record SearchItemQuery(AuctionSearchModel SearchModel) : IRequest<SearchItemResult>;

public record SearchItemResult(List<AuctionSearchDto> Items, Meta Meta);
