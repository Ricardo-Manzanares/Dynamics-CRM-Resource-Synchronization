﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMResourceSynchronization.Core.Business.Models
{
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
        public string content { get; set; }

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
