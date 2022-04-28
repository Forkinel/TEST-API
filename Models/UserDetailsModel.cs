using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class UserModel
    {
        public string ID { get; set; }

    }

    
    //public class UserNameCheckModel
    //{
    //    public string userName { get; set; }
    //    public string ID { get; set; }

    //}

    public class UserDetailsModel
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; }
        public List<int> Companies { get; set; }
        public bool AllowedToViewPricing { get; set; }
        public bool ShowAllProjects { get; set; }
        public bool IsStaff { get; set; }
        public bool IsActive { get; set; }
    }

}
