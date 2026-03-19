using L1.Core.Domain.Transaction.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L3.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : BaseConfiguration<Payment> {
  public override void Configure(EntityTypeBuilder<Payment> builder) {
    base.Configure(builder);

    builder.Property(p => p.OrderId)
      .IsRequired();

    builder.Property(p => p.Amount)
      .HasPrecision(18, 2)
      .IsRequired();

    builder.Property(p => p.TransactionId)
      .HasMaxLength(255)
      .IsRequired(false);

    builder.Property(p => p.Method)
      .HasConversion<string>()
      .HasMaxLength(50)
      .IsRequired();

    builder.Property(p => p.Status)
      .HasConversion<string>()
      .HasMaxLength(50)
      .IsRequired();

    builder.Ignore(p => p.DomainEvents);
    builder.HasIndex(p => p.OrderId);
    builder.HasIndex(p => p.TransactionId)
      .IsUnique()
      .HasFilter("[TransactionId] IS NOT NULL");
  }
}
