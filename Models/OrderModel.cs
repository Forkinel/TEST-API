using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class OrderModel
    {

        public List<SiteOrder> Sites { get; set; }
        public List<TestOrder> Tests { get; set; }

    }

    public class SiteOrder
    {
        public int ID { get; set; }
        public string SiteName { get; set; }
    }

    public class TestOrder 
    { 

        public int ID { get; set; }
        public string TestName { get; set; }
    }

}