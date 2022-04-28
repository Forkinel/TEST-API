
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using System.Net.Mail;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _appSettings;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;

        }

        [HttpPost]
        [Route("ForgotPassword")]
        //Post : /Account/ForgotPassword
        public async Task<IActionResult> ForgotPassword(UserModel userModel)
        {
            try
            {

                ApplicationUser user = await _userManager.FindByNameAsync(userModel.ID);
                var newToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //var confirmationLink = Url.Page("ConfirmPasswordResetEmail", "Account", new { userId = user.Id, token = newToken }, Request.Scheme);                
                var resetLink = _appSettings.APPROOT.ToString() + "user/reset-pwd?userId=" + user.Id + "&token=" + UrlSafeEncode(newToken);

                MailController mailController = new MailController();
                var emailSent = await mailController.SendEmail(resetLink, _appSettings.SENDGRID_APIKEY, _appSettings.SENDGRID_TEMPLATE_FORGOTTEN_PWD, user.Email);

                var response = new { success = true, responseText = "", data = resetLink, emailSent };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }

        [HttpGet]
        [Route("ConfirmPasswordResetEmail")]
        //Get : /Account/ConfirmPasswordResetEmail
        public async Task<IActionResult> ConfirmPasswordResetEmail(string userId, string token)
        {

            if (userId == null || token == null)
            {
                var response = new { success = false, responseText = "Error" };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }

            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {

                var response = new { success = false, responseText = "The User ID " + userId + " is invalid" };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }

            try
            {

                var decodedCode = UrlSafeDecode(token);

                IdentityResult result = await _userManager.ConfirmEmailAsync(user, decodedCode);

                if (result.Succeeded)
                {
                    var response = new { success = true, responseText = "" };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }
                else
                {
                    var response = new { success = false, responseText = "User tokens do not match" };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }

            }
            catch (Exception ex)
            {

                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }

        [HttpGet]
        [Route("ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(string userId, string token, string newpwd)
        {


            if (userId == null || token == null)
            {
                var response = new { success = false, responseText = "There has been an error with your login." };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }

            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {

                var response = new { success = false, responseText = "The User details are invalid" };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }

            try
            {


                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);


                var result = await _userManager.ResetPasswordAsync(user, resetToken, newpwd);

                if (result.Succeeded)
                {
                    var response = new { success = true, responseText = "Password Updated, please login again using your new password." };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }
                else
                {
                    // tokens do not match, probably clicked and old email reset
                    var response = new { success = false, responseText = "There has been an error. Please try to reset your password again via the login screen." };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }

            }
            catch (Exception ex)
            {

                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }


        [HttpGet]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string userId)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {

                var response = new { success = false, responseText = "The User details are invalid" };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }

            try
            {

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);


                var result = await _userManager.ResetPasswordAsync(user, resetToken, _appSettings.RESETPWD);

                if (result.Succeeded)
                {

                    var resetLink = _appSettings.APPROOT.ToString() + "user/reset-pwd?userId=" + user.Id + "&token=" + UrlSafeEncode(resetToken);

                    MailController mailController = new MailController();
                    var emailSent = await mailController.SendEmail(resetLink, _appSettings.SENDGRID_APIKEY, _appSettings.SENDGRID_TEMPLATE_FORGOTTEN_PWD, user.Email);

                    var response = new { success = true, responseText = "", data = resetLink, emailSent };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }
                else
                {
                    // tokens do not match, probably clicked and old email reset
                    var response = new { success = false, responseText = "There has been an error. Please try to reset your password again via the login screen." };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
                }

            }
            catch (Exception ex)
            {

                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }
    }
}