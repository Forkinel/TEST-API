using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class MailController : BaseController
    {



        public MailController()
        {



        }
        private class ExampleTemplateData
        {
            [JsonProperty("{{ name }}")]
            public string name { get; set; }
 
        }

        public async Task<IActionResult> SendEmail(string pwdLink, string apiKey, string templateID, string Email)
        {

            try
            {
                var client = new SendGridClient(apiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("NoReply@2030lims.com"),
                    Subject = "Reset Password Request",
                    HtmlContent = pwdLink
                   
                };

                msg.SetTemplateData(new {link = pwdLink });
       

                msg.SetTemplateId(templateID);
                msg.AddTo(new EmailAddress(Email));

                var response = await client.SendEmailAsync(msg);

                var rtn = new { success = true, responseText = "password reset sent" };
                //var response = await client.SendEmailAsync(msg);
                return  Content(Newtonsoft.Json.JsonConvert.SerializeObject(rtn), "application/json");

            }
            catch (Exception ex)
            {

                //var response = await client.SendEmailAsync(msg);

                var rtn = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rtn), "application/json");

            }
         

        }
    }


}
