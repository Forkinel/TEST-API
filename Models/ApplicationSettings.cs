using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ApplicationSettings
    {
        public string JWT_Secret { get; set; }
        public string Client_URL { get; set; }

        public string SENDGRID_APIKEY { get; set; }

        public string SENDGRID_TEMPLATE_FORGOTTEN_PWD { get; set; }

        public string APPROOT { get; set; }

        public string RESETPWD { get; set; }

        
    }


}
