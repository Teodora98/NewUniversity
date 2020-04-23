using Microsoft.AspNetCore.Mvc.Rendering;
using NewUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewUniversity.ViewModels
{
    public class CourseStudentViewModel
    {
        public Course Course { get; set; }

        public IEnumerable<int> SelectedStudents { get; set; }

        public IEnumerable<SelectListItem> StudentsList { get; set; }
    }
}
