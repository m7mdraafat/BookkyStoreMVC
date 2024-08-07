using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Data.Config
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.Id).IsRequired();

            builder.Property(x => x.Name)
                   .HasColumnType("NVARCHAR").IsRequired();

            builder.Property(x => x.StreetAddress)
                   .HasColumnType("NVARCHAR");

            builder.Property(x => x.State)
                   .HasColumnType("NVARCHAR");

            builder.Property(x => x.City)
                   .HasColumnType("NVARCHAR");

            builder.Property(x => x.PostalCode)
                   .HasColumnType("NVARCHAR");


            builder.Property(x => x.PhoneNumber)
                   .HasColumnType("NVARCHAR");

            builder.ToTable("Companies");
        }
    }
}
