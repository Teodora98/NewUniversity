using Microsoft.AspNetCore.Mvc.Rendering;
using NewUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewUniversity.ViewModels
{
    public class CourseFilterViewModel
    {
        public IList<Course> Courses { get; set; }
        public string SearchString { get; set; }  
        public int CourseSemester { get; set; } 

        public SelectList Semesters { get; set; }  
        public string CourseProgramme { get; set; }
        public SelectList Programmes { get; set; } 

        public string TeacherString { get; set; }
    }
}
