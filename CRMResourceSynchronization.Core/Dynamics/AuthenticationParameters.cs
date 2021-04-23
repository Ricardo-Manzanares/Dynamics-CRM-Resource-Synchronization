using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CRMResourceSynchronization.Core.Dynamics.CRMClient;

namespace CRMResourceSynchronization.Core.Dynamics
{
    public class AuthenticationParameters
    {
        public string _CRMUrl { get; set; }
        public string _CRMUserName { get; set; }
        public string _CRMPassword { get; set; }
        public string _ClientId { get; set; }
        public string _RedirectUri { get; set; }
        public string _TokenCacheStorePath { get; set; }
        public bool _IntegratedSecurityPrompt { get; set; }
        public string _AuthLoginPrompt { get; set; }
        public string _CertificateThumprint { get; set; }
        public string _ClientSecret { get; set; }
        public AutenticationType _AuthenticationType { get; set; }
    }
}
