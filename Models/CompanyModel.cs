using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{ 
    public class CompanyModel
    {

        public int ID { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }       
        

    }

    public class CompanyIDs
    {
        public List<int> IDs { get; set; }
       
    }

    // this will be populated by more detailed information like address etc at a later date
    public class CompanyDetailsModel
    {
        public int ID { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string EmailAddresses { get; set; }
        //public bool AllowReporting { get; set; }
        //public bool UseTVCLogic { get; set; }
        //public bool HighPriority { get; set; }
        //public bool Deleted { get; set; }
        //public string TestReportType { get; set; }
        //public bool ExternalBarcodePrinting { get; set; }
        //public string SageAccountRef { get; set; }
        //public bool SageInterface { get; set; }
        //public bool MandatoryEmailOnOrder { get; set; }
        //public bool AllowEmailBodyOverride { get; set; }
        //public int EmailReceiptTypeId { get; set; }
        //public string Logo { get; set; }
        //public bool showLogo { get; set; }
        //public int InterimResultEmailTypeId { get; set; }
        //public int ResultsDeliveryTypeId { get; set; }
        //public bool ShowLimitCriteria { get; set; }
        //public bool AllowAutoRelease { get; set; }

        public List<Address> Addresses { get; set; }
        public List<SiteModel> Sites { get; set; }
        public List<int> Tests { get; set; }
        //public List<int> Kits { get; set; }

    }

    public class Address
    {
        public int ID { get; set; }
        public string Address_1 { get; set; }
        public string Address_2 { get; set; }
        public string Address_3 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string PhoneNo { get; set; }
        public bool Default { get; set; }
        public bool IsActive { get; set; }

    }

    public class CompanyActionResultObj
    {
        public CompanyActionResultObj() { }
        public bool success { get; set; }
        public string responseText { get; set; }
        public List<SiteModel> data { get; set; }


    }


}
