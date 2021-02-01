using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Infrastructure.Data.DbMapping.Users
{
    public static class UserMap
    {
        public static ModelBuilder MapUser(this ModelBuilder modelBuilder)
        {
            EntityTypeBuilder<User> entity = modelBuilder.Entity<User>();

            entity.Property(e => e.Name)
                .IsRequired();
            entity.Property(e => e.Email)
                .IsRequired();

            return modelBuilder;
        }
    }
}
