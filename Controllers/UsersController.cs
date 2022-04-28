using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {

        private UserManager<ApplicationUser> _userManager;
        private readonly ConnectionSettings _connSettings;
        private readonly ApplicationSettings _appSettings;

        public UsersController(UserManager<ApplicationUser> userManager, IOptions<ConnectionSettings> connSettings, IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _connSettings = connSettings.Value;
            _appSettings = appSettings.Value;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("GetAllUsers")]
        // GET :  api/GetAllUsers
        public IEnumerable<ApplicationUserSmallModel> GetAllUsers()
        {

            List<ApplicationUserSmallModel> usersList = new List<ApplicationUserSmallModel>();

            string connectionString = _connSettings.IdentityConnection;


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllUsers", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {

                    ApplicationUserSmallModel user = new ApplicationUserSmallModel
                    {

                        ID = sdr["Id"].ToString(),
                        FullName = sdr["FullName"].ToString(),
                        UserName = sdr["UserName"].ToString(),
                        Email = sdr["Email"].ToString(),
                        Roles = sdr["Roles"].ToString().Split(','),
                        IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false

                    };

                    usersList.Add(user);
                };

                con.Close();
            }

            return usersList;

        }

        [HttpPost]
        [Authorize(Roles = "Admin, Cust")]
        [Route("GetUserDetails")]
        // GET :  api/Users/GetUserDetails
        public ActionResult GetUserDetails([FromBody] UserModel model)
        {
            try
            {

                UserDetailsModel user = new UserDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetUserDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("ID", model.ID);
                    cmd.Parameters.Add(param);

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {

                        user.ID = sdr["Id"].ToString();
                        user.FullName = sdr["FullName"].ToString();
                        user.UserName = sdr["UserName"].ToString();
                        user.Password = sdr["Password"].ToString();
                        user.Email = sdr["Email"].ToString();

                        if (sdr["Roles"].ToString() != "")
                        {
                            var strArray = sdr["Roles"].ToString().Split(',');
                            user.Roles = strArray;
                        }

                        if (sdr["Companies"].ToString() != "")
                        {
                            var strArray = sdr["Companies"].ToString().Split(',');
                           List<Int32> listInts = new List<int>();
                            foreach (string item in strArray)
                            {
                                listInts.Add(Convert.ToInt32(item));

                            }
                            user.Companies = listInts;
                        }


                        user.IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false;
                        user.IsStaff = Convert.ToInt32(sdr["IsStaff"]) == 1 ? true : false;
                        user.AllowedToViewPricing = Convert.ToInt32(sdr["AllowedToViewPricing"]) == 1 ? true : false;
                        user.ShowAllProjects = Convert.ToInt32(sdr["ShowAllProjects"]) == 1 ? true : false;

                    };

                    con.Close();
                }

                  var response = new { success = true, responseText = "", data = user };
                 return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
        
                }
                catch (Exception ex)
                {

                    var response = new { success = false, responseText = ex.Message };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }

            

        }


        [HttpPost]
        [Authorize(Roles = "Admin, Cust")]
        [Route("GetUserDetailsObj")]
        // GET :  api/Users/GetUserDetailsObj
        public UserDetailsModel GetUserDetailsObj(UserModel model)
        {
           

            UserDetailsModel user = new UserDetailsModel();

            string connectionString = _connSettings.IdentityConnection;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetUserDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("ID", model.ID);
                cmd.Parameters.Add(param);

                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {

                    user.ID = sdr["Id"].ToString();
                    user.FullName = sdr["FullName"].ToString();
                    user.UserName = sdr["UserName"].ToString();
                    user.Password = sdr["Password"].ToString();
                    user.Email = sdr["Email"].ToString();

                    if (sdr["Roles"].ToString() != "")
                    {
                        var strArray = sdr["Roles"].ToString().Split(',');
                        user.Roles = strArray;
                    }

                    if (sdr["Companies"].ToString() != "")
                    {
                        var strArray = sdr["Companies"].ToString().Split(',');
                        List<Int32> listInts = new List<int>();
                        foreach (string item in strArray)
                        {
                            listInts.Add(Convert.ToInt32(item));

                        }
                        user.Companies = listInts;
                    }


                    user.IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false;
                    user.IsStaff = Convert.ToInt32(sdr["IsStaff"]) == 1 ? true : false;
                    user.AllowedToViewPricing = Convert.ToInt32(sdr["AllowedToViewPricing"]) == 1 ? true : false;
                    user.ShowAllProjects = Convert.ToInt32(sdr["ShowAllProjects"]) == 1 ? true : false;


                };

                con.Close();

                return user;
            }

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("SaveUserDetails")]
        // post :  api/Users/SaveUserDetails
        public async Task<ActionResult> SaveUserDetails([FromBody] UserDetailsModel userDetails)
        {

            var user = await _userManager.FindByIdAsync(userDetails.ID);

            if (user == null)
            {
                return await CreateUserDetails(userDetails);
            }
            else
            {
                return await EditUserDetails(userDetails);
            }
        }




        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("CreateUserDetails")]
        // GET :  api/Users/CreateUserDetails
        public async Task<ActionResult> CreateUserDetails(UserDetailsModel userDetails)
        {
          
                var applicationUser = new ApplicationUser();

                applicationUser.UserName = userDetails.UserName;
                applicationUser.Email = userDetails.Email;
                applicationUser.FullName = userDetails.FullName;
                applicationUser.IsActive = userDetails.IsActive;
                applicationUser.IsStaff = userDetails.IsStaff;
                applicationUser.AllowedToViewPricing = userDetails.AllowedToViewPricing;
                applicationUser.ShowAllProjects = userDetails.ShowAllProjects;

            try
                {
                
                    var result = await _userManager.CreateAsync(applicationUser, userDetails.Password);

                    if (result.Succeeded)
                    {
                        // assign a role to the user
                        await _userManager.AddToRolesAsync(applicationUser, userDetails.Roles); ;
                        userDetails.ID = applicationUser.Id;
                        UpdateCompanyUsers(userDetails);


                        ApplicationUser user = await _userManager.FindByNameAsync(userDetails.ID);
                        var newToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        //var confirmationLink = Url.Page("ConfirmPasswordResetEmail", "Account", new { userId = user.Id, token = newToken }, Request.Scheme);                
                        var resetLink = _appSettings.APPROOT + "reset-pwd?userId=" + user.Id + "&token=" + UrlSafeEncode(newToken);

                        MailController mailController = new MailController();
                        var emailSent = await mailController.SendEmail(resetLink, _appSettings.SENDGRID_APIKEY, _appSettings.SENDGRID_TEMPLATE_FORGOTTEN_PWD, user.Email);

                        var response = new { success = true, responseText = "", data = resetLink, emailSent };
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

                    }
                    else
                    {
                        var errorMessages = "";
                        foreach (IdentityError erroritem in result.Errors)
                        {
                            errorMessages += erroritem.Description + " ";
                        }

                        var response = new { success = false, responseText = errorMessages };
                        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                    }

                }
                catch (Exception ex)
                {

                    var response = new { success = false, responseText = ex.Message };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }

            
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("EditUserDetails")]

        public async Task<ActionResult> EditUserDetails(UserDetailsModel userDetails)
        {

            var user = await _userManager.FindByIdAsync(userDetails.ID);
            
            try
            {

                if (!user.UserName.Equals(userDetails.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    user.UserName = userDetails.UserName;
                }

                if (!user.Email.Equals(userDetails.Email, StringComparison.OrdinalIgnoreCase))
                {
                    user.Email = userDetails.Email;
                }

                if (!user.FullName.Equals(userDetails.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    user.FullName = userDetails.FullName;
                }

                if (user.IsActive != userDetails.IsActive)
                {
                    user.IsActive = userDetails.IsActive;
                }

                if (user.IsStaff != userDetails.IsStaff)
                {
                    user.IsStaff = userDetails.IsStaff;
                }

                if (user.AllowedToViewPricing != userDetails.AllowedToViewPricing)
                {
                    user.AllowedToViewPricing = userDetails.AllowedToViewPricing;
                }

                if (user.ShowAllProjects != userDetails.ShowAllProjects)
                {
                    user.ShowAllProjects = userDetails.ShowAllProjects;
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // get the currently save roles of this user
                    var userRoles = await _userManager.GetRolesAsync(user);
                    // remove any roles then assign roles to the user as i dont see how to just update them
                    result = await _userManager.RemoveFromRolesAsync(user, userRoles);
                    result = await _userManager.AddToRolesAsync(user, userDetails.Roles);

                    UpdateCompanyUsers(userDetails);

                    var response = new { success = true, responseText = "", };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }
                else
                {
                    var errorMessages = "";
                    foreach (IdentityError erroritem in result.Errors)
                    {
                        errorMessages += erroritem.Description + " ";
                    }

                    var response = new { success = false, responseText = errorMessages };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }

            }
            catch (Exception ex)
            {

                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }



        public void UpdateCompanyUsers(UserDetailsModel userDetails)
        {

            string connectionString = _connSettings.IdentityConnection;

            using (SqlConnection con = new SqlConnection(connectionString))
            {

                var stringID = userDetails.ID.ToString(); 

                con.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM CompanyUsers where AspNetUsers_ID =  '" + stringID + "'");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.ExecuteNonQuery();

                if (userDetails.Companies.Count > 0)
                {
                    StringBuilder nonQuery = new StringBuilder();
                    foreach (int item in userDetails.Companies)
                    {
                        nonQuery.AppendFormat("INSERT INTO CompanyUsers (AspNetUsers_ID, Companies_ID) VALUES ('{0}', {1});", stringID, item);

                    }
                    cmd = new SqlCommand(nonQuery.ToString());

                    cmd.CommandType = CommandType.Text;

                    cmd.Connection = con;

                    cmd.ExecuteNonQuery();


                }

                con.Close();


            }         

        }

    }

}