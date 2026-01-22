using L2.Application.Models;

namespace L2.Application.UseCases.Bidder.Admin.GetLockedBidders;

public record GetLockedBiddersResult(List<User> Bidders, Meta Meta);