using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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
    public class CompaniesController : BaseController
    {

        private UserManager<ApplicationUser> _userManager;
        private readonly ConnectionSettings _connSettings;

        public CompaniesController(UserManager<ApplicationUser> userManager, IOptions<ConnectionSettings> connSettings)
        {
            _userManager = userManager;
            _connSettings = connSettings.Value;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("GetAllCompanies")]
        // GET :  api/GetAllCompanies
        public ActionResult GetAllCompanies()
        {
            try
            {

                List<CompanyModel> companiesList = new List<CompanyModel>();

                string connectionString = _connSettings.IdentityConnection;
                
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetAllCompanies", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {

                        CompanyModel company = new CompanyModel
                        {

                            ID = Convert.ToInt32(sdr["ID"].ToString()),
                            Name = sdr["CompanyName"].ToString(),
                            IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false

                        };

                        companiesList.Add(company);
                    };

                    con.Close();
                }

                var response = new { success = true, responseText = "", data = companiesList };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }        

        }

        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        [Route("GetCompanyDetails")]
        // post :  api/Companies/GetCompanyDetails
        public ActionResult GetCompanyDetails([FromBody] CompanyModel model)
        {

            try
            {

                CompanyDetailsModel company = new CompanyDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetCompanyDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("ID", model.ID);
                    cmd.Parameters.Add(param);

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {
                        company.ID = Convert.ToInt32(sdr["Id"].ToString());
                        company.Name = sdr["CompanyName"].ToString();
                        company.IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false;
                        company.EmailAddresses = sdr["EmailAddresses"].ToString();
                        company.Addresses = GetCompanyAddresses(company.ID);

                        ContentResult cr = (ContentResult)GetCompanySites(company.ID);

                        CompanyActionResultObj caro = new CompanyActionResultObj();

                        using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(cr.Content)))
                        {
                            // Deserialization from JSON  
                            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(CompanyActionResultObj));
                            caro = (CompanyActionResultObj)deserializer.ReadObject(ms);                                             
                        }

                        company.Sites = caro.data;


                    }
                        con.Close();
                }
            
                var response = new { success = true, responseText = "", data = company };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");

            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        [Route("GetCompanySites")]
        // post :  api/Companies/GetCompanySites
        public ActionResult GetCompanySites(int companyID)
        {
            try { 

            List<SiteModel> siteList = new List<SiteModel>();

            string connectionString = _connSettings.IdentityConnection;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetCompanySites", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("ID", companyID);
                cmd.Parameters.Add(param);

                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {

                    SiteModel site = new SiteModel();

                    site.ID = Convert.ToInt32(sdr["ID"].ToString());
                    site.SiteName = sdr["SiteName"].ToString();
                    site.CompanyName = sdr["CompanyName"].ToString();
                    site.IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false;

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
        [Route("GetCompanyAddresses")]
        // post :  api/Companies/GetCompanyAddresses
        public List<Address> GetCompanyAddresses(int companyID)
        {
            
                List<Address> addressList = new List<Address>();    

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetCompanyAddresses", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("CompanyID",companyID);
                    cmd.Parameters.Add(param);

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {

                        Address addressObj = new Address();

                        addressObj.ID = Convert.ToInt32(sdr["ID"].ToString());
                        addressObj.Address_1 = sdr["Address_1"].ToString();
                        addressObj.Address_2 = sdr["Address_2"].ToString();
                        addressObj.Address_3 = sdr["Address_3"].ToString();
                        addressObj.Town = sdr["Town"].ToString();
                        addressObj.County = sdr["County"].ToString();
                        addressObj.PostCode = sdr["PostCode"].ToString();
                        addressObj.PhoneNo = sdr["PhoneNo"].ToString();
                        addressObj.Default = Convert.ToInt32(sdr["Default"]) == 1 ? true : false;
                        addressObj.IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false;

                        addressList.Add(addressObj);

                    };

                    con.Close();
                }

            return addressList;

        }               

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("SaveCompanyDetails")]
        // post :  api/Companies/SaveCompanyDetails
        public ActionResult SaveCompanyDetails([FromBody] CompanyDetailsModel model)
        {
          
            if(model.ID == 0)
            {
                return AddCompanyDetails(model);
            }
            else
            {
                return UpdateCompanyDetails(model);
            }           

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("AddCompanyDetails")]
        // post :  api/Companies/AddCompanyDetails
        public ActionResult AddCompanyDetails(CompanyDetailsModel model)
        {

            try
            {

                CompanyDetailsModel company = new CompanyDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                int newCompanyID = 0;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("AddCompanyDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param1 = new SqlParameter("CompanyName", model.Name);
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter("IsActive", model.IsActive);
                    cmd.Parameters.Add(param2);

                    SqlParameter param3 = new SqlParameter("EmailAddresses", model.EmailAddresses);
                    cmd.Parameters.Add(param3);

                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {
                        newCompanyID = Convert.ToInt32(sdr["ID"].ToString());
                    };

                    con.Close();
                }

                return AddCompanyAddresses(newCompanyID, model.Addresses);

            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("AddCompanyAdresses")]
        // post :  api/Companies/AddCompanyAdresses
        public ActionResult AddCompanyAddresses(int companyID, List<Address> addressesList)
        {

            try
            {

                CompanyDetailsModel company = new CompanyDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    con.Open();

                    foreach (Address addObj in addressesList) { 
                    

                    SqlCommand cmd = new SqlCommand("AddCompanyAddress", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("CompanyID", companyID);
                    cmd.Parameters.Add(param);

                    SqlParameter paramAddress_1 = new SqlParameter("Address_1", addObj.Address_1);
                    cmd.Parameters.Add(paramAddress_1);

                    SqlParameter paramAddress_2 = new SqlParameter("Address_2", addObj.Address_2);
                    cmd.Parameters.Add(paramAddress_2);

                    SqlParameter paramAddress_3 = new SqlParameter("Address_3", addObj.Address_3);
                    cmd.Parameters.Add(paramAddress_3);

                    SqlParameter paramTown = new SqlParameter("Town", addObj.Town);
                    cmd.Parameters.Add(paramTown);

                    SqlParameter paramCounty = new SqlParameter("County", addObj.County);
                    cmd.Parameters.Add(paramCounty);

                    SqlParameter paramPostCode = new SqlParameter("PostCode", addObj.PostCode);
                    cmd.Parameters.Add(paramPostCode);

                    SqlParameter paramPhoneNo = new SqlParameter("PhoneNo", addObj.PhoneNo);
                    cmd.Parameters.Add(paramPhoneNo);

                    SqlParameter paramDefault = new SqlParameter("Default", addObj.Default);
                    cmd.Parameters.Add(paramDefault);

                    SqlParameter paramIsActive = new SqlParameter("IsActive", addObj.IsActive);
                    cmd.Parameters.Add(paramIsActive);
            
                    cmd.ExecuteNonQuery();
                   
                    }

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
        [Route("UpdateCompanyDetails")]
        // post :  api/Companies/UpdateCompanyDetails
        public ActionResult UpdateCompanyDetails( CompanyDetailsModel model)
        {

            try
            {

                CompanyDetailsModel company = new CompanyDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UpdateCompanyDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("ID", model.ID);
                    cmd.Parameters.Add(param);

                    SqlParameter param1 = new SqlParameter("CompanyName", model.Name);
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter("IsActive", model.IsActive);
                    cmd.Parameters.Add(param2);

                    SqlParameter param3 = new SqlParameter("EmailAddresses", model.EmailAddresses);
                    cmd.Parameters.Add(param3);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    //// Remove any companyaddress items for the curent company
                    SqlCommand cmdDelete = new SqlCommand("DELETE FROM CompanyAddresses where Companies_ID =  " + model.ID);
                    cmdDelete.CommandType = CommandType.Text;
                    cmdDelete.Connection = con;
                    cmdDelete.ExecuteNonQuery();

                    con.Close();
                }

                return UpdateCompanyAddresses(model.ID, model.Addresses);
                
            }
            catch (Exception ex)
            {
                var response = new { success = false, responseText = ex.Message };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(response), "application/json");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("UpdateCompanyAddresses")]
        // post :  api/Companies/UpdateCompanyAddresses
        public ActionResult UpdateCompanyAddresses(int companyID, List<Address> addressesList)
        {

            try
            {

                CompanyDetailsModel company = new CompanyDetailsModel();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    con.Open();

                    foreach (Address addObj in addressesList)
                    {


                        SqlCommand cmd = new SqlCommand("UpdateCompanyAddress", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter paramCompanyID = new SqlParameter("CompanyID", companyID);
                        cmd.Parameters.Add(paramCompanyID);

                        SqlParameter paramAddressID = new SqlParameter("AddressID", addObj.ID);
                        cmd.Parameters.Add(paramAddressID);

                        SqlParameter paramAddress_1 = new SqlParameter("Address_1", addObj.Address_1);
                        cmd.Parameters.Add(paramAddress_1);

                        SqlParameter paramAddress_2 = new SqlParameter("Address_2", addObj.Address_2);
                        cmd.Parameters.Add(paramAddress_2);

                        SqlParameter paramAddress_3 = new SqlParameter("Address_3", addObj.Address_3);
                        cmd.Parameters.Add(paramAddress_3);

                        SqlParameter paramTown = new SqlParameter("Town", addObj.Town);
                        cmd.Parameters.Add(paramTown);

                        SqlParameter paramCounty = new SqlParameter("County", addObj.County);
                        cmd.Parameters.Add(paramCounty);

                        SqlParameter paramPostCode = new SqlParameter("PostCode", addObj.PostCode);
                        cmd.Parameters.Add(paramPostCode);

                        SqlParameter paramPhoneNo = new SqlParameter("PhoneNo", addObj.PhoneNo);
                        cmd.Parameters.Add(paramPhoneNo);

                        SqlParameter paramDefault = new SqlParameter("Default", addObj.Default);
                        cmd.Parameters.Add(paramDefault);

                        SqlParameter paramIsActive = new SqlParameter("IsActive", addObj.IsActive);
                        cmd.Parameters.Add(paramIsActive);

                        cmd.ExecuteNonQuery();

                    }

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
        [Authorize(Roles = "Admin, Cust")]
        [Route("GetCompanyNames")]
        // POST :  api/GetCompanyNames
        public ActionResult GetCompanyNames([FromBody] CompanyIDs companyIDs)
        {
            try
            {
                string IDs = string.Join(",", companyIDs.IDs);

                List<CompanyModel> companiesList = new List<CompanyModel>();

                string connectionString = _connSettings.IdentityConnection;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Select * from Companies where ID in (" + IDs + ")");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;                 
                    con.Open();

                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {

                        CompanyModel company = new CompanyModel
                        {

                            ID = Convert.ToInt32(sdr["ID"].ToString()),
                            Name = sdr["CompanyName"].ToString(),
                            IsActive = Convert.ToInt32(sdr["IsActive"]) == 1 ? true : false

                        };

                        companiesList.Add(company);
                    };

                    con.Close();
                }

                var response = new { success = true, responseText = "", data = companiesList };
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