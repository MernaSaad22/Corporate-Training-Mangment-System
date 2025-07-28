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

        }






    }
}
