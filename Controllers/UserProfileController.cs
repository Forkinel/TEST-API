using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : BaseController
    {

        private UserManager<ApplicationUser> _userManager;

        public UserProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        // GET :  api/UserProfile
        public async Task<Object> GetUserProfile() {

            string userID = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userID);
            return new
            {
           
                user.FullName,
                user.Email,
                user.UserName,
                user.IsActive,
                user.IsStaff

            };
        
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("ForAdmin")]
        public string GetForAdmin() {

            return "Web Method for Admin";

        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        [Route("ForCustomer")]
        public string GetForCustomer()
        {

            return "Web Method for Customer";

        }

        [HttpGet]
        [Authorize(Roles = "Admin, Customer")]
        [Route("ForAdminOrCustomer")]
        public string GetForAdminOrCustomer()
        {

            return "Web Method for Admin or Customer";

        }



    }
}