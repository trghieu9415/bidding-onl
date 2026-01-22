using L2.Application.Models;

namespace L2.Application.UseCases.Bidder.Admin.GetBidders;

public record GetBiddersResult(List<User> Bidders, Meta Meta);