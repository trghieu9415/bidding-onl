using FluentAssertions;
using L1.Core.Domain.Catalog.ValueObjects;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.ValueObjects;

public class CatalogValueObjectTests {
  [Fact]
  public void ImageGallery_RecordStoresMainAndSubImages() {
    // Arrange
    var subImages = new[] { "sub-1.png", "sub-2.png" };

    // Act
    var gallery = new ImageGallery("main.png", subImages);

    // Assert
    gallery.MainImageUrl.Should().Be("main.png");
    gallery.SubImageUrls.Should().BeEquivalentTo(subImages);
  }

  [Fact]
  public void ImageGallery_RecordUsesValueEqualityForSameReferences() {
    // Arrange
    ICollection<string> subImages = ["sub-1.png"];
    var left = new ImageGallery("main.png", subImages);
    var right = new ImageGallery("main.png", subImages);

    // Act
    var isEqual = left.Equals(right);

    // Assert
    isEqual.Should().BeTrue();
    left.Should().Be(right);
  }
}
