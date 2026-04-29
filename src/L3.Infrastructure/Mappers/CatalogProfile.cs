using AutoMapper;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;

namespace L3.Infrastructure.Mappers;

public class CatalogProfile : Profile {
  public CatalogProfile() {
    CreateMap<Category, CategoryDto>();

    CreateMap<CatalogItem, CatalogItemDto>()
      .ForMember(d => d.MainImageUrl, o => o.MapFrom(s => s.Images.MainImageUrl))
      .ForMember(d => d.SubImageUrls, o => o.MapFrom(s => s.Images.SubImageUrls))
      .ForMember(d => d.CategoryIds, o => o.MapFrom(s => s.CategoryIds.ToList()));
  }
}
