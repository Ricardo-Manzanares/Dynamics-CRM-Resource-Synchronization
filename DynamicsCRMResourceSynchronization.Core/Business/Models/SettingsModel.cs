using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsCRMResourceSynchronization.Core.Business.Models
{
    public class SettingsModel
    {
        public string CRMUrl { get; set; }
        public string CRMUserName { get; set; }
        public string CRMPassword { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string TokenCacheStorePath { get; set; }
        public bool IntegratedSecurityPrompt { get; set; }
        public string AuthLoginPrompt { get; set; }
        public string CertificateThumprint { get; set; }
        public string ClientSecret { get; set; }
        public string CRMTypeAuth { get; set; }
        public string PathHTML { get; set; }
        public string PathCSS { get; set; }
        public string PathJS { get; set; }
        public string PathXML { get; set; }
        public string PathPNG { get; set; }
        public string PathJPG { get; set; }
        public string PathGIF { get; set; }
        public string PathXAP { get; set; }
        public string PathXSL { get; set; }
        public string PathICO { get; set; }
        public string PathSVG { get; set; }
        public string PathRESX { get; set; }
    }
}
