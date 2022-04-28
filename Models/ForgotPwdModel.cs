using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ForgotPwdModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
