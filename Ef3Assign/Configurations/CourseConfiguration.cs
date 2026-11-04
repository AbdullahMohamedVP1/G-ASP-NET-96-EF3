using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ef3Assign.Models;

namespace Ef3Assign.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Description).HasMaxLength(1000);
            builder.Property(c => c.Price).HasPrecision(18, 2);
            builder.Property(c => c.Level).IsRequired();
            builder.Property(c => c.IsPublished).IsRequired();

            // Instructor 1 --- * Course
            builder.HasOne(c => c.Instructor)
                   .WithMany(i => i.Courses)
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Category 1 --- * Course
            builder.HasOne(c => c.Category)
                   .WithMany(cat => cat.Courses)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
            new Course { Id = 1, Title = "C# Fundamentals", Description = "Learn C# from zero", Price = 300m, Level = CourseLevel.Beginner, IsPublished = true, PublishedAt = new DateTime(2023, 1, 5), InstructorId = 1, CategoryId = 1 },
                new Course { Id = 2, Title = "ASP.NET Core Web API", Description = "Build REST APIs", Price = 800m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedAt = new DateTime(2023, 4, 20), InstructorId = 1, CategoryId = 3 },
                new Course { Id = 3, Title = "EF Core Deep Dive", Description = "Advanced EF Core topics", Price = 950m, Level = CourseLevel.Advanced, IsPublished = true, PublishedAt = new DateTime(2023, 9, 12), InstructorId = 2, CategoryId = 3 },
                new Course { Id = 4, Title = "Advanced SQL Server", Description = "Indexes and performance", Price = 700m, Level = CourseLevel.Advanced, IsPublished = false, PublishedAt = null, InstructorId = 2, CategoryId = 2 },
                new Course { Id = 5, Title = "HTML and CSS Basics", Description = "Your first web pages", Price = 250m, Level = CourseLevel.Beginner, IsPublished = true, PublishedAt = new DateTime(2024, 2, 1), InstructorId = 3, CategoryId = 4 },
                new Course { Id = 6, Title = "JavaScript for Beginners", Description = null, Price = 400m, Level = CourseLevel.Beginner, IsPublished = false, PublishedAt = null, InstructorId = 3, CategoryId = 4 }
            );
        }
    }
}