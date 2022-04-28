using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class TestModel
    {
        public int ID { get; set; }
        public string TestName { get; set; }
        public bool IsActive { get; set; }

    }

    public class TestDetailsModel
    {
        public int ID { get; set; }
        public string TestName { get; set; }
        public bool IsActive { get; set; }

    
    }

}
