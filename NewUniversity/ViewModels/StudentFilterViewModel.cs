using Microsoft.AspNetCore.Mvc.Rendering;
using NewUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewUniversity.ViewModels
{
    public class StudentFilterViewModel
    {
        public IList<Student> Students { get; set; }

        public string StudentIndex { get; set; }

        public string SearchFullName { get; set; }

        public string SearchCourse { get; set; } 
    }
}
