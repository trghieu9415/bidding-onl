using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Bidding.Admin.GetSessions;

public record GetSessionsResult(List<AuctionSessionDto> Sessions, Meta Meta);