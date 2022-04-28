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
    public class TestsController : BaseController
    {

        private UserManager<ApplicationUser> _userManager;
        private readonly ConnectionSettings _connSettings;

        public TestsController(UserManager<ApplicationUser> userManager, IOptions<ConnectionSettings> connSettings)
        {
            _userManager = userManager;
            _connSettings = connSettings.Value;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("GetAllTests")]
        // GET :  api/GetAllTests
        public ActionResult GetAllTests()
        {
            try
            {

                List<TestModel> testsList = new List<TestModel>();

                string connectionString = _connSettings.IdentityConnection;
                
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetAllTests", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {

                        TestModel test = new TestModel
                        {

                            ID = Convert.ToInt32(sdr["ID"].ToString()),
                            TestName = sdr["TestName"].ToString(),
                            IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false

                        };

                        testsList.Add(test);
                    };

                    con.Close();
                }

                var response = new { success = true, responseText = "", data = testsList };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }        

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("GetTestDetails")]
        // post :  api/Companies/GetTestDetails
        public ActionResult GetTestDetails([FromBody] TestDetailsModel model)
        {

            try
            {

                TestDetailsModel test = new TestDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetTestDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("ID", model.ID);
                    cmd.Parameters.Add(param);

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {
                        test.ID = Convert.ToInt32(sdr["Id"].ToString());
                        test.TestName = sdr["TestName"].ToString();
                        test.IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false;
                    };

                    con.Close();
                }

                var response = new { success = true, responseText = "", data = test };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("SaveTestDetails")]
        // post :  api/Tests/SaveTestDetails
        public ActionResult SaveTestDetails([FromBody] TestDetailsModel model)
        {

            if (model.ID == 0)
            {
                return AddTestDetails(model);
            }
            else
            {
                return UpdateTestDetails(model);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("AddTestDetails")]
        // post :  api/Companies/AddTestDetails
        public ActionResult AddTestDetails(TestDetailsModel model)
        {

            try
            {

                TestDetailsModel test = new TestDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                int newTestID = 0;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("AddTestDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param1 = new SqlParameter("TestName", model.TestName);
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter("IsActive", model.IsActive);
                    cmd.Parameters.Add(param2);

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {
                        newTestID = Convert.ToInt32(sdr["ID"].ToString());
                    };

 
                }

                var response = new { success = true, responseText = "" };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("UpdateTestDetails")]
        // post :  api/Companies/UpdateTestDetails
        public ActionResult UpdateTestDetails(TestDetailsModel model)
        {

            try
            {

                TestDetailsModel test = new TestDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UpdateTestDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("ID", model.ID);
                    cmd.Parameters.Add(param);

                    SqlParameter param1 = new SqlParameter("TestName", model.TestName);
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter("IsActive", model.IsActive);
                    cmd.Parameters.Add(param2);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }

                var response = new { success = true, responseText = "" };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }


      

    }

}