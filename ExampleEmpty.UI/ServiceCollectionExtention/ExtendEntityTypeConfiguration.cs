using ExampleEmpty.UI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExampleEmpty.UI.ServiceCollectionExtention
{
    public class ExtendEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("tblCustomer");
            builder.HasKey(c => c.CustomerId);

            builder.Property(c => c.Name)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)")
                      .IsFixedLength()
                      .IsRequired();

            builder.Property(c => c.Address)
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)")
                    .IsFixedLength()
                    .IsRequired();


            builder.Property(c => c.Gender)
                    .HasColumnType("int")
                    .IsFixedLength()
                    .IsRequired();

            builder.Property(c => c.PhotoPath)
                   .HasColumnType("varchar(255)")
                   .IsFixedLength();
                 

        }
    }
}
