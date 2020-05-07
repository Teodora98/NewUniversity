using Microsoft.AspNetCore.Http;
using NewUniversity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewUniversity.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Required]

        [StringLength(10)]

        public string Index { get; set; }

        [Required]

        [StringLength(50)]

        public string FirstName { get; set; }

        [Required]

        [StringLength(50)]

        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }
        public int? AcquiredCredits { get; set; }
        public int? CurrentSemestar { get; set; }
        public string FullName
        {

            get { return FirstName + " " + LastName; }

        }
        [StringLength(25)]
        public string EducationLevel { get; set; }
        public ICollection<Enrollment> Courses { get; set; }
        public IFormFile UserProfileImage { get; set; }
    }
}
