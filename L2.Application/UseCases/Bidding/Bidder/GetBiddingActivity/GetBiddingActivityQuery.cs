using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.Bidder.GetBiddingActivity;

public record GetBiddingActivityQuery(SieveModel SieveModel) : IQuery<GetBiddingActivityResult>;
