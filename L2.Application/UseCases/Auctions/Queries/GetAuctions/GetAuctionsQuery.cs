using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.GetAuctions;

public record GetAuctionsQuery(AuctionFilter Filter) : IRequest<GetAuctionsResult>;

public record GetAuctionsResult(List<AuctionDto> Auctions, Meta Meta);
