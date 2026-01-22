using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Bidding.Admin.GetRemovedSessions;

public record GetRemovedSessionsResult(List<AuctionSessionDto> Sessions, Meta Meta);