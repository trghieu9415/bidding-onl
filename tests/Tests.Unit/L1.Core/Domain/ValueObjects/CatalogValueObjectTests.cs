using L1.Core.Domain.Catalog.ValueObjects;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.ValueObjects;

public class CatalogValueObjectTests {
  [Fact]
  public void ImageGallery_RecordStoresMainAndSubImages() {
    var subImages = new[] { "sub-1.png", "sub-2.png" };

    var gallery = new ImageGallery("main.png", subImages);

    Assert.Equal("main.png", gallery.MainImageUrl);
    Assert.Equal(subImages, gallery.SubImageUrls);
  }

  [Fact]
  public void ImageGallery_RecordUsesValueEqualityForSameReferences() {
    ICollection<string> subImages = ["sub-1.png"];
    var left = new ImageGallery("main.png", subImages);
    var right = new ImageGallery("main.png", subImages);

    Assert.Equal(left, right);
  }
}
