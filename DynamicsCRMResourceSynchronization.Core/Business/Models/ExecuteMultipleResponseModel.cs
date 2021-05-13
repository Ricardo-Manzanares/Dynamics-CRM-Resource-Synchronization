using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsCRMResourceSynchronization.Core.Business.Models
{
    public class ExecuteMultipleResponseModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool error { get; set; }
    }
}
