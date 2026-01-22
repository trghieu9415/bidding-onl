using L1.Core.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : BaseConfiguration<Category> {
  public override void Configure(EntityTypeBuilder<Category> builder) {
    base.Configure(builder);

    builder.Property(x => x.Name)
      .IsRequired()
      .HasMaxLength(100);

    builder.HasOne<Category>()
      .WithMany()
      .HasForeignKey(x => x.ParentId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}