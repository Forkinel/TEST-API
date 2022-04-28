using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Column (TypeName = "nvarchar(150)")]
        public string FullName { get; set; }

        [Column(TypeName = "bit")]
        public bool IsActive { get; set; }

        [Column(TypeName = "bit")]
        public bool IsStaff { get; set; }

        [Column(TypeName = "bit")]
        public bool AllowedToViewPricing { get; set; }

        [Column(TypeName = "bit")]
        public bool ShowAllProjects { get; set; }

    

    }
}
