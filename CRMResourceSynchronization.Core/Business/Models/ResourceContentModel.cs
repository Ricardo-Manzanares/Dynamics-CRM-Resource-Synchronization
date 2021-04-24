using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMResourceSynchronization.Core.Business.Models
{
    public enum ResourceContentRowStatus { Difference, Empty, Insert, Delete }

    public class ResourceContentModel
    {
        [Description("Row")]
        public int numRow { get; set; }
        [Description("")]
        public string textRow { get; set; }
        [Browsable(false)]
        public ResourceContentRowStatus statusRow { get; set; }
    }
}
