using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NewUniversity.Models
{
    public class User
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        

    }
}
