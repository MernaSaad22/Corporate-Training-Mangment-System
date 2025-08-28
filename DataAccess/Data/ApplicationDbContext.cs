using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { 
        
        }
       
        public DbSet<Company>Compaines {  get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Exam> Exames { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<EmployeeCourse> EmployeeCourses { get; set; }
        public DbSet<EmployeeAssignment> EmployeeAssignments { get; set; }
      public DbSet<Employee>Employees { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        public DbSet<EmployeeAssignmentSubmission> EmployeeAssignmentSubmissions { get; set; }

        public DbSet<ExamSubmission> ExamSubmissions { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public DbSet<EmployeeLessonProgress> EmployeeLessonProgresses { get; set; }
        public DbSet<EmployeeCourseProgress> EmployeeCourseProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Instructor - Course
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany()
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict); // ❗ مهم

            // Company - Course
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Company)
                .WithMany(c => c.Courses)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Company - Employees
            modelBuilder.Entity<Employee>()
                .HasOne<ApplicationUser>(e => e.ApplicationUser)
                .WithMany()
                .HasForeignKey(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment - Instructor
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Instructor)
                .WithMany()
                .HasForeignKey(a => a.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
            // Company - Owner User
            modelBuilder.Entity<Company>()
                .HasOne(c => c.ApplicationUser)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Instructor - ApplicationUser
            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.ApplicationUser)
                .WithMany()
                .HasForeignKey(i => i.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeAssignment>()
       .HasOne(ea => ea.Employee)
       .WithMany()
       .HasForeignKey(ea => ea.EmployeeId)
       .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeCourse>()
       .HasOne(ec => ec.Employee)
       .WithMany()
       .HasForeignKey(ec => ec.EmployeeId)
       .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Plan>()
    .Property(p => p.Cost)
    .HasPrecision(18, 2); // Use 18,2 for money-like values



            modelBuilder.Entity<Assignment>()
    .HasOne(a => a.Lesson)
    .WithMany(l => l.Assignments)
    .HasForeignKey(a => a.LessonId)
    .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Chapter>()
    .HasOne(c => c.Exam)
    .WithOne(e => e.Chapter)
    .HasForeignKey<Exam>(e => e.ChapterId)
    .OnDelete(DeleteBehavior.Cascade);




            modelBuilder.Entity<EmployeeAssignmentSubmission>()
    .HasOne(sub => sub.Employee)
    .WithMany()
    .HasForeignKey(sub => sub.EmployeeId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeAssignmentSubmission>()
                .HasOne(sub => sub.Assignment)
                .WithMany()
                .HasForeignKey(sub => sub.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ❗ Unique constraint to ensure one submission per employee per assignment
            modelBuilder.Entity<EmployeeAssignmentSubmission>()
                .HasIndex(sub => new { sub.EmployeeId, sub.AssignmentId })
                .IsUnique();




            // ExamSubmission - Employee relationship (many ExamSubmissions to one Employee)
            modelBuilder.Entity<ExamSubmission>()
                .HasOne(es => es.Employee)
                .WithMany() // or .WithMany(e => e.ExamSubmissions) if you have a navigation property in Employee
                .HasForeignKey(es => es.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamSubmission - Exam relationship (many ExamSubmissions to one Exam)
            modelBuilder.Entity<ExamSubmission>()
                .HasOne(es => es.Exam)
                .WithMany() // or .WithMany(e => e.ExamSubmissions) if you have a nav property in Exam
                .HasForeignKey(es => es.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            // QuestionAnswer - ExamSubmission relationship (many QuestionAnswers to one ExamSubmission)
            modelBuilder.Entity<QuestionAnswer>()
                .HasOne(qa => qa.ExamSubmission)
                .WithMany(es => es.QuestionAnswers)
                .HasForeignKey(qa => qa.ExamSubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // QuestionAnswer - Question relationship (many QuestionAnswers to one Question)
            modelBuilder.Entity<QuestionAnswer>()
                .HasOne(qa => qa.Question)
                .WithMany() // or .WithMany(q => q.QuestionAnswers) if you have navigation property
                .HasForeignKey(qa => qa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional: Prevent multiple submissions of the same exam by the same employee
            modelBuilder.Entity<ExamSubmission>()
                .HasIndex(es => new { es.ExamId, es.EmployeeId })
                .IsUnique();

            modelBuilder.Entity<EmployeeLessonProgress>()
     .HasOne(e => e.Employee)
     .WithMany()
     .HasForeignKey(e => e.EmployeeId)
     .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeLessonProgress>()
                .HasOne(e => e.Lesson)
                .WithMany()
                .HasForeignKey(e => e.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: prevent duplicate progress entries
            modelBuilder.Entity<EmployeeLessonProgress>()
                .HasIndex(p => new { p.EmployeeId, p.LessonId })
                .IsUnique();



            modelBuilder.Entity<EmployeeCourseProgress>()
    .HasOne(p => p.Employee)
    .WithMany()
    .HasForeignKey(p => p.EmployeeId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeCourseProgress>()
                .HasOne(p => p.Course)
                .WithMany()
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Ensure one progress entry per employee per course
            modelBuilder.Entity<EmployeeCourseProgress>()
                .HasIndex(p => new { p.EmployeeId, p.CourseId })
                .IsUnique();

        }






    }
}
