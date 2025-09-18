using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.entities;

namespace Ecommerce.Infrastructure.Persistence
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.UserName).IsUnique();
                b.HasIndex(u => u.Email).IsUnique();
                b.Property(u => u.UserName).HasMaxLength(100).IsRequired();
                b.Property(u => u.Email).HasMaxLength(200).IsRequired();
                b.Property(u => u.PasswordHash).IsRequired();
                b.Property(u => u.Role).HasMaxLength(50).HasDefaultValue("User");
            });

            modelBuilder.Entity<Product>(b =>
            {
                b.HasIndex(p => p.ProductCode).IsUnique();
                b.Property(p => p.Price).HasColumnType("decimal(18,2)");
                b.Property(p => p.DiscountRate).HasColumnType("decimal(5,2)");
                b.Property(p => p.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.HasIndex(r => r.Token).IsUnique();
                b.HasOne(r => r.User).WithMany(u => u.RefreshTokens).HasForeignKey(r => r.UserId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
