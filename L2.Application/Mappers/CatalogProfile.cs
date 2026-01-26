using AutoMapper;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;

namespace L2.Application.Mappers;

public class CatalogProfile : Profile {
  public CatalogProfile() {
    CreateMap<Category, CategoryDto>();

    CreateMap<CatalogItem, CatalogItemDto>()
      .ForMember(d => d.MainImageUrl, o => o.MapFrom(s => s.Images.MainImageUrl))
      .ForMember(d => d.SubImageUrls, o => o.MapFrom(s => s.Images.SubImageUrls ?? new List<string>()))
      .ForMember(d => d.CategoryIds, o => o.MapFrom(s => s.CategoryIds.ToList()));
  }
}
