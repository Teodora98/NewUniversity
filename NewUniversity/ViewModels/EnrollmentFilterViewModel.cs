using Microsoft.AspNetCore.Mvc.Rendering;
using NewUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewUniversity.ViewModels
{
    public class EnrollmentFilterViewModel
    {
        public IList<Enrollment> Enrollments{ get; set; }
        public string SeacrhFullName { get; set; }
        public string SearchCourse { get; set; }
        public SelectList Courses { get; set; }

    }
}
