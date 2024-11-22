using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace EFCoreManyToManyApp
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public List<Subject> Subjects { get; set; } = new();
    }

    public class Subject
    {
        public int SubjectId { get; set; }
        public string Title { get; set; }
        public int MaximumCapacity { get; set; }
        public List<Student> Students { get; set; } = new();
    }

    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=School.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                        .HasMany(s => s.Subjects)
                        .WithMany(s => s.Students);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var subject1 = new Subject { Title = "Math", MaximumCapacity = 30 };
                var subject2 = new Subject { Title = "Science", MaximumCapacity = 25 };
                context.Subjects.AddRange(subject1, subject2);

                var student1 = new Student { Name = "Alice", EnrollmentDate = DateTime.Now };
                var student2 = new Student { Name = "Bob", EnrollmentDate = DateTime.Now };
                context.Students.AddRange(student1, student2);

                subject1.Students.Add(student1);
                subject1.Students.Add(student2);
                subject2.Students.Add(student2);

                context.SaveChanges();

                var subjects = context.Subjects.Include(s => s.Students).ToList();
                foreach (var subj in subjects)
                {
                    Console.WriteLine($"Subject: {subj.Title}, Enrolled Students: {subj.Students.Count}");
                    foreach (var stud in subj.Students)
                    {
                        Console.WriteLine($"  - {stud.Name}");
                    }
                }
            }
        }
    }
}
