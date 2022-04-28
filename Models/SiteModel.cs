using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class SiteModel
    {
        public int ID { get; set; }
        public string SiteName { get; set; }
        public string CompanyName { get; set; }        
        public bool IsActive { get; set; }

    }


    public class SiteDetailsModel
    {
        public int ID { get; set; }
        public string SiteName { get; set; }
        public int Companies_ID { get; set; }
        public string? EmailAddresses { get; set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Town { get; set; }
        public string? County { get; set; }
        public string? PostCode { get; set; }
        
        public bool IsActive { get; set; }
    
    }

}
