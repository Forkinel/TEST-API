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
    public class SitesController : BaseController
    {

        private UserManager<ApplicationUser> _userManager;
        private readonly ConnectionSettings _connSettings;

        public SitesController(UserManager<ApplicationUser> userManager, IOptions<ConnectionSettings> connSettings)
        {
            _userManager = userManager;
            _connSettings = connSettings.Value;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("GetAllSites")]
        // GET :  api/GetAllSites
        public ActionResult GetAllSites()
        {
            try
            {

                List<SiteModel> siteList = new List<SiteModel>();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetAllSites", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {

                        SiteModel site = new SiteModel
                        {

                            ID = Convert.ToInt32(sdr["ID"].ToString()),
                            SiteName = sdr["SiteName"].ToString(),
                            CompanyName = sdr["CompanyName"].ToString(),
                            IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false

                        };

                        siteList.Add(site);
                    };

                    con.Close();
                }

                var response = new { success = true, responseText = "", data = siteList };
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
        [Route("GetSiteDetails")]
        // post :  api/Sites/GetSiteDetails
        public ActionResult GetSiteDetails([FromBody] SiteDetailsModel model)
        {

            try
            {

                SiteDetailsModel site = new SiteDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetSiteDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("ID", model.ID);
                    cmd.Parameters.Add(param);

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {
                        site.ID = Convert.ToInt32(sdr["ID"].ToString());
                        site.Companies_ID = Convert.ToInt32(sdr["Companies_ID"].ToString());
                        site.SiteName = sdr["SiteName"].ToString();
                        site.EmailAddresses = sdr["EmailAddresses"].ToString();
  

                        site.Address1 = sdr["Address1"].ToString();
                        site.Address2 = sdr["Address2"].ToString();
                        site.Address3 = sdr["Address3"].ToString();
                        site.Town = sdr["Town"].ToString();
                        site.County = sdr["County"].ToString();
                        site.PostCode = sdr["PostCode"].ToString();
                        
                        site.IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false;

                    }
                    con.Close();
                }

                var response = new { success = true, responseText = "", data = site };
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
        [Route("SaveSiteDetails")]
        // post :  api/Tests/SaveSiteDetails
        public ActionResult SaveSiteDetails([FromBody] SiteDetailsModel model)
        {

            if (model.ID == 0)
            {
                return AddSiteDetails(model);
            }
            else
            {
                return UpdateSiteDetails(model);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("AddSiteDetails")]
        // post :  api/Companies/AddSiteDetails
        public ActionResult AddSiteDetails(SiteDetailsModel model)
        {

            try
            {

                SiteDetailsModel test = new SiteDetailsModel();

                string connectionString = _connSettings.IdentityConnection;


                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("AddSiteDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param1 = new SqlParameter("Companies_ID", model.Companies_ID);
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter("SiteName", model.SiteName);
                    cmd.Parameters.Add(param2);

                    SqlParameter param3 = new SqlParameter("Address1", (object)model.Address1 ?? DBNull.Value);
                    cmd.Parameters.Add(param3);

                    SqlParameter param4 = new SqlParameter("Address2", (object)model.Address2 ?? DBNull.Value);
                    cmd.Parameters.Add(param4);

                    SqlParameter param5 = new SqlParameter("Address3", (object)model.Address3 ?? DBNull.Value);
                    cmd.Parameters.Add(param5);

                    SqlParameter param6 = new SqlParameter("Town", (object)model.Town ?? DBNull.Value);
                    cmd.Parameters.Add(param6);

                    SqlParameter param7 = new SqlParameter("County", (object)model.County ?? DBNull.Value);
                    cmd.Parameters.Add(param7);

                    SqlParameter param8 = new SqlParameter("PostCode", (object)model.PostCode ?? DBNull.Value);
                    cmd.Parameters.Add(param8);

                    SqlParameter param9 = new SqlParameter("EmailAddresses", (object)model.EmailAddresses ?? DBNull.Value);
                    cmd.Parameters.Add(param9);

                    SqlParameter param10 = new SqlParameter("IsActive", model.IsActive);
                    cmd.Parameters.Add(param10);

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    int newSiteID = 0;

                    while (sdr.Read())
                    {
                        newSiteID = Convert.ToInt32(sdr["ID"].ToString());
                    };
         
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("UpdateSiteDetails")]
        // post :  api/Companies/UpdateSiteDetails
        public ActionResult UpdateSiteDetails(SiteDetailsModel model)
        {

            try
            {

                SiteDetailsModel test = new SiteDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UpdateSiteDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("ID", model.ID);
                    cmd.Parameters.Add(param);

                    SqlParameter param1 = new SqlParameter("Companies_ID", model.Companies_ID);
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter("SiteName", model.SiteName);
                    cmd.Parameters.Add(param2);

                    SqlParameter param3 = new SqlParameter("Address1", (object)model.Address1 ?? DBNull.Value);
                    cmd.Parameters.Add(param3);

                    SqlParameter param4 = new SqlParameter("Address2", (object)model.Address2 ?? DBNull.Value);
                    cmd.Parameters.Add(param4);

                    SqlParameter param5 = new SqlParameter("Address3", (object)model.Address3 ?? DBNull.Value);
                    cmd.Parameters.Add(param5);

                    SqlParameter param6 = new SqlParameter("Town", (object)model.Town ?? DBNull.Value);
                    cmd.Parameters.Add(param6);

                    SqlParameter param7 = new SqlParameter("County", (object)model.County ?? DBNull.Value);
                    cmd.Parameters.Add(param7);

                    SqlParameter param8 = new SqlParameter("PostCode", (object)model.PostCode ?? DBNull.Value);
                    cmd.Parameters.Add(param8);

                    SqlParameter param9 = new SqlParameter("EmailAddresses", (object)model.EmailAddresses ?? DBNull.Value);
                    cmd.Parameters.Add(param9);

                    SqlParameter param10 = new SqlParameter("IsActive", model.IsActive);
                    cmd.Parameters.Add(param10);

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