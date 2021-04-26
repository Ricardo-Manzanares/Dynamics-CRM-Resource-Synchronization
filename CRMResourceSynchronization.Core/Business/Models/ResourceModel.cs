using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRMResourceSynchronization.Core.Business.Models
{
    public enum ResourceContentStatus {[EnumMember(Value = "Equal")][Description("Resources are the same")]Equal, [EnumMember(Value = "Difference")][Description("Resources are different")]Difference, [EnumMember(Value = "LocalResourceMissing")] [Description("Local resource missing")]LocalResourceMissing, [EnumMember(Value = "EmptyResource")] [Description("Empty resource")] EmptyResource }
    public class ResourceModel
    {
        [Browsable(false)]
        public Guid resourceid { get; set; }

        [Description("")]
        public bool selectResource { get; set; }

        [Description("CRM F. Creación")]
        public string createdon { get; set; }

        [Description("Recurso")]
        public string name { get; set; }

        [Description("CRM F. Modificación")]
        public string modifiedon { get; set; }

        [Browsable(false)]
        public string contentCRM { get; set; }

        [Browsable(false)]
        public List<ResourceContentModel> contentRowsCRM = new List<ResourceContentModel>();

        [Browsable(false)]
        public string contentLocal { get; set; }

        [Browsable(false)]
        public List<ResourceContentModel> contentRowsLocal = new List<ResourceContentModel>();
        public string resourcesStatus { get; set; }

        [Description("Proyecto F. Creación")]
        public string localcreatedon { get; set; }

        [Description("Proyecto F. Modificación")]
        public string localmodifiedon { get; set; }

        [Browsable(false)]
        public int webresourcetype { get; set; }

        [Browsable(false)]
        public string pathlocal { get; set; }        
    }
}
