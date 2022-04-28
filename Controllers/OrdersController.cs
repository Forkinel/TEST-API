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
    public class OrdersController : BaseController
    {

        private UserManager<ApplicationUser> _userManager;
        private readonly ConnectionSettings _connSettings;

        public OrdersController(UserManager<ApplicationUser> userManager, IOptions<ConnectionSettings> connSettings)
        {
            _userManager = userManager;
            _connSettings = connSettings.Value;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Cust")]
        [Route("SaveOrderDetails")]
        // post :  api/Orders/SaveOrderDetails
        public ActionResult SaveOrderDetails([FromBody] OrderModel model)
        {

            var serializedOrderJson = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            var response = new { success = true, responseText = "", data = serializedOrderJson };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

        }   

    }

}