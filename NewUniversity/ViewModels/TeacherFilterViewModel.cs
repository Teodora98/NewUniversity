using Microsoft.AspNetCore.Mvc.Rendering;
using NewUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewUniversity.ViewModels
{
    public class TeacherFilterViewModel
    {
        public IList<Teacher> Teachers { get; set; }

        public SelectList AcademicRanges { get; set; }

        public string TeacherAcademicRank { get; set; }

        public SelectList Degrees { get; set; }

        public string TeacherDegree { get; set; }

        public string SearchFullName { get; set; } 
    }
}
