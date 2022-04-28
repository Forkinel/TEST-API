using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class RolesController : BaseController
    {

        private readonly ConnectionSettings _connSettings;

        public RolesController( IOptions<ConnectionSettings> connSettings)
        {
     
            _connSettings = connSettings.Value;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("GetAllRoles")]
        // GET :  api/GetAllRoles
        public ActionResult GetAllRoles()
        //public IEnumerable<RoleModel> GetAllRoles()
        {

            try
            {

                List<RoleModel> rolesList = new List<RoleModel>();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetAllRoles", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {

                        RoleModel role = new RoleModel
                        {
                            ID = Convert.ToInt32(sdr["Id"].ToString()),
                            RoleName = sdr["Name"].ToString()
                        };

                        rolesList.Add(role);
                    };

                    con.Close();
                }

                var response = new { success = true, responseText = "", data = rolesList };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }
           
            //return rolesList;

        }


    }
}