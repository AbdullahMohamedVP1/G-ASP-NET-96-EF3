using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ef3Assign.Models;

namespace Ef3Assign.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.FullName).IsRequired().HasMaxLength(100);
            builder.Property(i => i.Email).IsRequired().HasMaxLength(150);
            builder.Property(i => i.Bio).HasMaxLength(500);
            builder.Property(i => i.JoinedAt).IsRequired();

            builder.HasData(
            new Instructor { Id = 1, FullName = "Ahmed Hassan", Email = "ahmed.hassan@example.com", Bio = "Backend developer with 10 years of experience", JoinedAt = new DateTime(2022, 1, 10) },
                new Instructor { Id = 2, FullName = "Mona Samir", Email = "mona.samir@example.com", Bio = "Database and data engineering instructor", JoinedAt = new DateTime(2022, 6, 15) },
                new Instructor { Id = 3, FullName = "Omar Khaled", Email = "omar.khaled@example.com", Bio = null, JoinedAt = new DateTime(2023, 3, 1) }
            );
        }
    }
}