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
    public class ShoppingCardConfiguration : IEntityTypeConfiguration<ShoppingCard>
    {
        public void Configure(EntityTypeBuilder<ShoppingCard> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.Count)
                   .IsRequired();

            builder.ToTable("ShoppingCards");
        }
    }
}
