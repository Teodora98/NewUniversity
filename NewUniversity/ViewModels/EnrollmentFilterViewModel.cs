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
        public SelectList SelectYear { get; set; }
        public IList<Enrollment> Enrollments{ get; set; }
        public int EnrollmentYear { get; set; }

    }
}
