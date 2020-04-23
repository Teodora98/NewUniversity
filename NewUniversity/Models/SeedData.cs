using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewUniversity.Data;
using System;
using System.Linq;

namespace NewUniversity.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new NewUniversityContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<NewUniversityContext>>()))
            {

                if (context.Course.Any() || context.Enrollment.Any() || context.Student.Any() || context.Teacher.Any())
                {

                    return;

                }
                context.Course.AddRange(
                new Course { Title = "Math", Credits = 6, Semester = 1, FirstTeacherId = 1, SecondTeacherId = 2 },
                new Course { Title = "Computer Science", Credits = 6, Semester = 1, FirstTeacherId = 3, SecondTeacherId = 2 },
                new Course { Title = "Biology", Credits = 6, Semester = 2, FirstTeacherId = 1, SecondTeacherId = 3 },
                new Course { Title = "Chemisty", Credits = 6, Semester = 3, FirstTeacherId = 4, SecondTeacherId = 2 },
                new Course { Title = "Physic", Credits = 6, Semester = 3, FirstTeacherId = 1, SecondTeacherId = 4 },
                new Course { Title = "English", Credits = 6, Semester = 1, FirstTeacherId = 3, SecondTeacherId = 2 }
                );
                context.SaveChanges();

                context.Teacher.AddRange(
                new Teacher { FirstName = "Billy", LastName = "Crystal" },
                new Teacher { FirstName = "Tom", LastName = "Edison" },
                new Teacher { FirstName = "Ana", LastName = "Jan" },
                new Teacher { FirstName = "Tim", LastName = "Tomy" }

                );
                context.SaveChanges();

                context.Student.AddRange(
                new Student
                {
                    Index = "1",
                    FirstName = "Tamara",
                    LastName = "Arsikj"
                },
                new Student
                {
                    Index = "2",
                    FirstName = "Teodora",
                    LastName = "Cvetkovikj"
                },
                new Student
                {
                    Index = "5",
                    FirstName = "Davor",
                    LastName = "Stefanovski"
                },
                new Student
                {
                    Index = "4",
                    FirstName = "Tijana",
                    LastName = "Janeva"
                },
                new Student
                {
                    Index = "6",
                    FirstName = "Kristijan",
                    LastName = "Aleksov"
                },
                new Student
                {
                    Index = "3",
                    FirstName = "Damjan",
                    LastName = "Gjoshev"
                }
                );
                context.SaveChanges(); 
                context.Enrollment.AddRange(
                    new Enrollment { CourseId = 1, StudentId = 1 },
                    new Enrollment { CourseId = 2, StudentId = 2 },
                    new Enrollment { CourseId = 3, StudentId = 3 },
                    new Enrollment { CourseId = 4, StudentId = 4 },
                    new Enrollment { CourseId = 5, StudentId = 5 },
                    new Enrollment { CourseId = 6, StudentId = 2 },
                    new Enrollment { CourseId = 1, StudentId = 3 },
                    new Enrollment { CourseId = 2, StudentId = 4 },
                    new Enrollment { CourseId = 3, StudentId = 5 },
                    new Enrollment { CourseId = 4, StudentId = 1 },
                    new Enrollment { CourseId = 3, StudentId = 2 }
                );
                context.SaveChanges();
            }
        }
    }
}
