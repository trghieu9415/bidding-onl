using AutoMapper;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;

namespace L3.Infrastructure.Mappers;

public class BiddingProfile : Profile {
  public BiddingProfile() {
    CreateMap<Bid, BidDto>();

    CreateMap<Auction, AuctionDto>()
      .ForMember(d => d.StepPrice, o => o.MapFrom(s => s.Rules.StepPrice))
      .ForMember(d => d.ReservePrice, o => o.MapFrom(s => s.Rules.ReservePrice))
      .ForMember(d => d.Bids, o => o.MapFrom(s => s.Bids));

    CreateMap<AuctionSession, AuctionSessionDto>()
      .ForMember(d => d.StartTime, o => o.MapFrom(s => s.TimeFrame != null ? s.TimeFrame.StartTime : (DateTime?)null))
      .ForMember(d => d.EndTime, o => o.MapFrom(s => s.TimeFrame != null ? s.TimeFrame.EndTime : (DateTime?)null))
      .ForMember(d => d.AuctionIds, o => o.MapFrom(s => s.AuctionIds.ToList()));
  }
}
