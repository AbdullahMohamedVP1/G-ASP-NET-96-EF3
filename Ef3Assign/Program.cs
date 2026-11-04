using Microsoft.EntityFrameworkCore;
using Ef3Assign.Data;
using Ef3Assign.Models;

namespace Ef3Assign
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SeedData();

            Console.WriteLine("----- Create -----");
            AddInstructor();
            AddCourse("Clean Architecture", 600m, CourseLevel.Advanced, 1, 2);
            AddCourse("Test Course", 100m, CourseLevel.Beginner, 99, 2);   // instructor not found
            AddStudents();
            EnrollStudent(4, 1);    // ok
            EnrollStudent(4, 4);    // course not published
            EnrollStudent(4, 5);    // enrolled
            EnrollStudent(99, 1);   // student not found

            Console.WriteLine("\n----- Read -----");
            FindCourse(1, "ASP.NET Core Web API");
            FindCourse(99, "Not A Course");
            FilterCourses(200m, 800m);
            PrintSubCategories(1);
            PrintTopLevelCategories();
            CourseRoster(1);

            Console.WriteLine("\n----- Update -----");
            ChangePrice(2, 850m);
            UpdateProgress(3, 2, 80);
            UpdateProgress(3, 2, 100);
            UpdateProgress(3, 2, 150);   // invalid
            PublishCourse(4);

            Console.WriteLine("\n----- Delete -----");
            Unenroll(1, 1);
            DeleteCategoryUnsafe(1);
            DeleteCategorySafe(1);   // refused
            DeleteCategorySafe(5);   // deleted
            DeleteCourse(5);

            Console.WriteLine("\n----- Bulk Operations -----");
            BulkPriceRaise(CourseLevel.Beginner);
            BulkDeleteDropped();
        }

        // --4---- Seeding from code ------
        static void SeedData()
        {
            using var context = new AppDbContext();

            if (!context.Students.Any())
            {
                context.Students.AddRange(
                    new Student { FullName = "Abdallah Mohamed", Email = "abdallah@example.com", DateOfBirth = new DateTime(2003, 5, 12) },
                    new Student { FullName = "Youssef Ali", Email = "youssef@example.com", DateOfBirth = new DateTime(2002, 8, 3) },
                    new Student { FullName = "Mariam Adel", Email = "mariam@example.com", DateOfBirth = new DateTime(2003, 1, 25) },
                    new Student { FullName = "Hana Tarek", Email = "hana@example.com", DateOfBirth = new DateTime(2004, 11, 9) },
                    new Student { FullName = "Karim Nabil", Email = "karim@example.com", DateOfBirth = new DateTime(2001, 7, 30) }
                );
                context.SaveChanges();
            }

            if (!context.Enrollments.Any())
            {
                var ids = context.Students.OrderBy(s => s.Id).Select(s => s.Id).ToList();

                context.Enrollments.AddRange(
                    new Enrollment { StudentId = ids[0], CourseId = 1, EnrolledAt = DateTime.Now.AddDays(-60), ProgressPercentage = 30, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = ids[0], CourseId = 2, EnrolledAt = DateTime.Now.AddDays(-90), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 90m },
                    new Enrollment { StudentId = ids[1], CourseId = 1, EnrolledAt = DateTime.Now.AddDays(-80), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 85.5m },
                    new Enrollment { StudentId = ids[1], CourseId = 5, EnrolledAt = DateTime.Now.AddDays(-40), ProgressPercentage = 20, Status = EnrollmentStatus.Dropped },
                    new Enrollment { StudentId = ids[2], CourseId = 2, EnrolledAt = DateTime.Now.AddDays(-30), ProgressPercentage = 55, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = ids[2], CourseId = 3, EnrolledAt = DateTime.Now.AddDays(-25), ProgressPercentage = 10, Status = EnrollmentStatus.Dropped },
                    new Enrollment { StudentId = ids[3], CourseId = 5, EnrolledAt = DateTime.Now.AddDays(-20), ProgressPercentage = 70, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = ids[4], CourseId = 3, EnrolledAt = DateTime.Now.AddDays(-100), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 92m }
                );
                context.SaveChanges();
            }
        }

        // -------- Create ---------
        static void AddInstructor()
        {
            using var context = new AppDbContext();

            var instructor = new Instructor
            {
                FullName = "Sara Mahmoud",
                Email = "sara.mahmoud@example.com",
                Bio = "Full stack developer",
                JoinedAt = DateTime.Now
            };

            context.Instructors.Add(instructor);
            context.SaveChanges();

            Console.WriteLine($"Instructor added with Id = {instructor.Id}");
        }

        static void AddCourse(string title, decimal price, CourseLevel level, int instructorId, int categoryId)
        {
            using var context = new AppDbContext();

            if (!context.Instructors.Any(i => i.Id == instructorId))
            {
                Console.WriteLine($"Cannot add course: instructor {instructorId} does not exist.");
                return;
            }

            if (!context.Categories.Any(c => c.Id == categoryId))
            {
                Console.WriteLine($"Cannot add course: category {categoryId} does not exist.");
                return;
            }

            var course = new Course
            {
                Title = title,
                Price = price,
                Level = level,
                IsPublished = false,
                InstructorId = instructorId,
                CategoryId = categoryId
            };

            context.Courses.Add(course);
            context.SaveChanges();

            Console.WriteLine($"Course '{course.Title}' added with Id = {course.Id}");
        }

        static void AddStudents()
        {
            using var context = new AppDbContext();

            var students = new List<Student>
            {
                new Student { FullName = "Nour Hossam", Email = "nour@example.com", DateOfBirth = new DateTime(2003, 2, 14) },
                new Student { FullName = "Ziad Walid", Email = "ziad@example.com", DateOfBirth = new DateTime(2002, 12, 1) },
                new Student { FullName = "Salma Ehab", Email = "salma@example.com", DateOfBirth = new DateTime(2004, 4, 18) }
            };

            context.Students.AddRange(students);
            context.SaveChanges();

            foreach (var s in students)
                Console.WriteLine($"Student '{s.FullName}' added with Id = {s.Id}");
        }

        static void EnrollStudent(int studentId, int courseId)
        {
            using var context = new AppDbContext();

            var student = context.Students.Find(studentId);
            if (student == null)
            {
                Console.WriteLine($"Enrollment refused: student {studentId} does not exist.");
                return;
            }

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Enrollment refused: course {courseId} does not exist.");
                return;
            }

            if (!course.IsPublished)
            {
                Console.WriteLine($"Enrollment refused: course '{course.Title}' is not published.");
                return;
            }

            if (context.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
            {
                Console.WriteLine($"Enrollment refused: {student.FullName} is already enrolled in '{course.Title}'.");
                return;
            }

            context.Enrollments.Add(new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = DateTime.Now,
                ProgressPercentage = 0,
                Status = EnrollmentStatus.Active
            });
            context.SaveChanges();

            Console.WriteLine($"{student.FullName} enrolled in '{course.Title}'.");
        }

        // ------- Read ------
        static void FindCourse(int id, string title)
        {
            using var context = new AppDbContext();

            var byId = context.Courses.Find(id);
            if (byId == null)
                Console.WriteLine($"Find by Id {id}: not found");
            else
                Console.WriteLine($"Find by Id {id}: {byId.Title}");

            var byTitle = context.Courses.FirstOrDefault(c => c.Title == title);
            if (byTitle == null)
                Console.WriteLine($"Find by title '{title}': not found");
            else
                Console.WriteLine($"Find by title '{title}': Id = {byTitle.Id}");
        }

        static void FilterCourses(decimal min, decimal max)
        {
            using var context = new AppDbContext();

            var courses = context.Courses
                .Where(c => c.Price >= min && c.Price <= max)
                .OrderByDescending(c => c.Price)
                .Select(c => new { c.Title, c.Level, c.Price })
                .ToList();

            Console.WriteLine($"Courses priced between {min} and {max}:");
            foreach (var c in courses)
                Console.WriteLine($" - {c.Title} | {c.Level} | {c.Price}");
        }

        static void PrintSubCategories(int parentId)
        {
            using var context = new AppDbContext();

            var subs = context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .ToList();

            Console.WriteLine($"Sub-categories of category {parentId}:");
            foreach (var c in subs)
                Console.WriteLine($" - {c.Name}");
        }

        static void PrintTopLevelCategories()
        {
            using var context = new AppDbContext();

            var tops = context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            Console.WriteLine("Top-level categories:");
            foreach (var c in tops)
                Console.WriteLine($" - {c.Name}");
        }

        static void CourseRoster(int courseId)
        {
            using var context = new AppDbContext();

            // step 1
            var studentIds = context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToList();

            // step 2
            var students = context.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToList();

            Console.WriteLine($"Course {courseId} has {studentIds.Count} enrolled students:");
            foreach (var s in students)
                Console.WriteLine($" - {s.FullName}");
        }

        // ------- Update ------
        static void ChangePrice(int courseId, decimal newPrice)
        {
            using var context = new AppDbContext();

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            if (newPrice < 0)
            {
                Console.WriteLine("Price cannot be negative.");
                return;
            }

            var oldPrice = course.Price;
            course.Price = newPrice;
            context.SaveChanges();

            Console.WriteLine($"Price of '{course.Title}' changed from {oldPrice} to {newPrice}.");
        }

        static void UpdateProgress(int studentId, int courseId, int progress)
        {
            using var context = new AppDbContext();

            if (progress < 0 || progress > 100)
            {
                Console.WriteLine($"Invalid progress {progress}. It must be between 0 and 100.");
                return;
            }

            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"Enrollment (student {studentId}, course {courseId}) not found.");
                return;
            }

            enrollment.ProgressPercentage = progress;
            if (progress == 100)
                enrollment.Status = EnrollmentStatus.Completed;

            context.SaveChanges();

            Console.WriteLine($"Progress updated to {progress}% (status: {enrollment.Status}).");
        }

        static void PublishCourse(int courseId)
        {
            using var context = new AppDbContext();

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            if (course.IsPublished)
            {
                Console.WriteLine($"Course '{course.Title}' is already published.");
                return;
            }

            course.IsPublished = true;
            course.PublishedAt = DateTime.Now;
            context.SaveChanges();

            Console.WriteLine($"Course '{course.Title}' published.");
        }

        // ---------------- Delete ----------------
        static void Unenroll(int studentId, int courseId){
            using var context = new AppDbContext();

            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null){
                Console.WriteLine($"Enrollment (student {studentId}, course {courseId}) not found.");
                return;}
            context.Enrollments.Remove(enrollment);
            context.SaveChanges();
            Console.WriteLine($"Student {studentId} unenrolled from course {courseId}.");}

        static void DeleteCategoryUnsafe(int categoryId)
        {
            using var context = new AppDbContext();

            var category = context.Categories.Find(categoryId);
            if (category == null){
                Console.WriteLine($"Category {categoryId} not found.");
                return;}

            try{
                context.Categories.Remove(category);
                context.SaveChanges();
                Console.WriteLine("Category deleted.");}
            catch (DbUpdateException ex){
                Console.WriteLine("Delete failed: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner: " + ex.InnerException.Message);}
        }

        static void DeleteCategorySafe(int categoryId)
        {
            using var context = new AppDbContext();

            var category = context.Categories.Find(categoryId);{
                Console.WriteLine($"Category {categoryId} not found.");
                return;}
            if (context.Categories.Any(c => c.ParentCategoryId == categoryId)){
                Console.WriteLine($"Cannot delete '{category.Name}' because it still has sub-categories.");}

            if (context.Courses.Any(c => c.CategoryId == categoryId)){
                Console.WriteLine($"Cannot delete '{category.Name}' because it still has courses.");
                return;
            }

            context.Categories.Remove(category);
            context.SaveChanges();

            Console.WriteLine($"Category '{category.Name}' deleted.");}

        static void DeleteCourse(int courseId)
        {
            using var context = new AppDbContext();

            var course = context.Courses.Find(courseId);
            if (course == null){
                Console.WriteLine($"course {courseId} not found.");
                return;}

            var before = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of course {courseId} before delete: {before}");

            context.Courses.Remove(course);
            context.SaveChanges();

            var after = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of course {courseId} after delete: {after}");

            // The enrollments were removed too because the FK from Enrollment to Course
            // uses Cascade delete (the default for a required relationship),
            // so deleting the course deletes its enrollments automatically.
        }
        static void BulkPriceRaise(CourseLevel level){
            using var context = new AppDbContext();
            var affected = context.Courses
                .Where(c => c.IsPublished && c.Level == level)
                .ExecuteUpdate(s => s.SetProperty(c => c.Price, c => c.Price * 1.10m));

            Console.WriteLine($"Price raised by 10% for {affected} published {level} courses."); }

        static void BulkDeleteDropped(){
            using var context = new AppDbContext();

            var affected = context.Enrollments
                .Where(e => e.Status == EnrollmentStatus.Dropped)
                .ExecuteDelete();
            Console.WriteLine($"{affected} dropped enrollments deleted.");
        }
    }
}