using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ef3Assign.Models;

namespace Ef3Assign.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(500);

            // self-referencing: Restrict so a parent with sub-categories can't be deleted
            builder.HasOne(c => c.ParentCategory)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(c => c.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ---- Seed Data ----
            builder.HasData(
                new Category { Id = 1, Name = "Programming", Description = "All programming topics", ParentCategoryId = null },
                new Category { Id = 2, Name = "Backend", Description = "Server side development", ParentCategoryId = 1 },
                new Category { Id = 3, Name = "ASP.NET", Description = "ASP.NET Core and related", ParentCategoryId = 2 },
                new Category { Id = 4, Name = "Frontend", Description = "Client side development", ParentCategoryId = 1 },
                new Category { Id = 5, Name = "Design", Description = null, ParentCategoryId = null }
            );
        }
    }
}