using L1.Core.Base.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity {
  public virtual void Configure(EntityTypeBuilder<T> builder) {
    builder.HasKey(e => e.Id);
  }
}