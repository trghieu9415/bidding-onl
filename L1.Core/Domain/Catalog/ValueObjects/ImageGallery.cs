namespace L1.Core.Domain.Catalog.ValueObjects;

public record ImageGallery(string? MainImageUrl, ICollection<string>? SubImageUrls);