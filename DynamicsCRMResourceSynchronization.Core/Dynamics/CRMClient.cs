using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsCRMResourceSynchronization.Core.Dynamics
{
    public class CRMClient
    {
        public enum AutenticationType {[Description("AD")] [EnumMember(Value = "AD")] AD, [Description("OAuth")] [EnumMember(Value = "OAuth")] OAuth, [Description("Certificate")] [EnumMember(Value = "Certificate")] Certificate, [Description("ClientSecret")] [EnumMember(Value = "ClientSecret")] ClientSecret, [Description("Office365")] [EnumMember(Value = "Office365")] Office365 }
        private AuthenticationParameters _Parameters { get; set; }
        public IOrganizationService _Client { get; set; }

        public CRMClient(AuthenticationParameters parameters)
        {
            _Parameters = parameters;
        }

        public bool GetOrganizationService()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                CrmServiceClient conn = null;

                switch (_Parameters._AuthenticationType)
                {
                    case AutenticationType.AD:
                        conn = new CrmServiceClient($@"AuthType=AD;url={_Parameters._CRMUrl};");
                        break;
                    case AutenticationType.OAuth:
                        string connecString = $@"AuthType=OAuth;Username={_Parameters._CRMUserName};url={_Parameters._CRMUrl};AppId={_Parameters._ClientId};RedirectUri={_Parameters._RedirectUri};LoginPrompt={_Parameters._AuthLoginPrompt};";
                        if (!String.IsNullOrEmpty(_Parameters._CRMPassword))
                        {
                            connecString += $@"Password={_Parameters._CRMPassword};";
                        }
                        if (_Parameters._IntegratedSecurityPrompt)
                        {
                            connecString += $@"Integrated Security={_Parameters._IntegratedSecurityPrompt};";
                        }
                        if (!String.IsNullOrEmpty(_Parameters._TokenCacheStorePath))
                        {
                            connecString += $@"TokenCacheStorePath={_Parameters._TokenCacheStorePath};";
                        }
                        conn = new CrmServiceClient(connecString);
                        break;
                    case AutenticationType.Certificate:

                        break;
                    case AutenticationType.ClientSecret:
                        conn = new CrmServiceClient($@"AuthType=ClientSecret;url={_Parameters._CRMUrl};ClientId={_Parameters._ClientId};ClientSecret={_Parameters._ClientSecret}");
                        break;
                    case AutenticationType.Office365:
                        conn = new CrmServiceClient($@"AuthType=Office365;url={_Parameters._CRMUrl};Username={_Parameters._CRMUserName};Password={_Parameters._CRMPassword}");
                        break;
                    default:
                        break;
                }

                if (conn == null)
                    return false;

                _Client = conn.OrganizationWebProxyClient != null ? conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;

                if (_Client != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}

