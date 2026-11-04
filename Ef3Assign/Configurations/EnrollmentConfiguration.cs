using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ef3Assign.Models;

namespace Ef3Assign.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // composite key => a student can enroll in the same course only once
            builder.HasKey(e => new { e.StudentId, e.CourseId });

            builder.Property(e => e.EnrolledAt).IsRequired();
            builder.Property(e => e.ProgressPercentage).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.FinalGrade).HasPrecision(5, 2);

            builder.HasOne(e => e.Student)
                   .WithMany(s => s.Enrollments)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}