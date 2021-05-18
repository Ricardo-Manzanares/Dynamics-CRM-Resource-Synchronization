using DynamicsCRMResourceSynchronization.Core.DiffPlex.DiffBuilder.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DynamicsCRMResourceSynchronization.Core.Business.Models
{
    public enum ResourceContentStatus {[EnumMember(Value = "Equal")][Description("Resources are the same")]Equal, [EnumMember(Value = "LocalResourceMissing")] [Description("Local resource missing")]LocalResourceMissing, [EnumMember(Value = "CRMResourceMissing")] [Description("CRM Resource missing")] CRMResourceMissing, [EnumMember(Value = "DifferencesExist")] [Description("Differences exist")] DifferencesExist, [EnumMember(Value = "DifferencesResolved")] [Description("Differences resolved")] DifferencesResolved }
    public enum ResourceStatusInSolution {[EnumMember(Value = "NoAction")] [Description("No action")] NoAction, [EnumMember(Value = "PendingToAdd")] [Description("Pending to add")] PendingToAdd, [EnumMember(Value = "PendingToRemove")] [Description("Pending to remove")] PendingToRemove}
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
        public string contentLocal { get; set; }

        [Browsable(false)]
        public SideBySideDiffModel resourceCompareStatus { get; set; }

        public ResourceContentStatus resourceDifference { get; set; }

        [Browsable(false)]
        public ResourceStatusInSolution resourceStatusInSolution { get; set; }

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
