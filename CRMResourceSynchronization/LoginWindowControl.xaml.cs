using CRMResourceSynchronization.Core.Business;
using CRMResourceSynchronization.Core.Dynamics;
using CRMResourceSynchronization.Extensions;
using CRMResourceSynchronization.Properties;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using static CRMResourceSynchronization.Core.Dynamics.CRMClient;

namespace CRMResourceSynchronization
{
    /// <summary>
    /// Interaction logic for LoginWindowControl.
    /// </summary>
    public partial class LoginWindowControl : UserControl
    {
        private AutenticationType typeAuth;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindowControl"/> class.
        /// </summary>
        public LoginWindowControl()
        {
            this.InitializeComponent();
            LoadConfiguration();
        }       

        private void LoadConfiguration()
        {
            ValidateConnectionClose.Visibility = Visibility.Visible;
            ValidateConnectionOpen.Visibility = Visibility.Hidden;

            CRMUrl.Text = Settings.Default.CRMUrl;
            CRMOAuthUserName.Text = Settings.Default.CRMUserName;
            CRMOAuthPassword.Text = Settings.Default.CRMPassword;
            CRMOffice365UserName.Text = Settings.Default.CRMUserName;
            CRMOffice365Password.Text = Settings.Default.CRMPassword;
            CRMOAuthClientId.Text = Settings.Default.ClientId;
            CRMClientSecretClientId.Text = Settings.Default.ClientId;
            CRMOAuthRedirectUri.Text = Settings.Default.RedirectUri;
            CRMOAuthTokenPath.Text = Settings.Default.TokenCacheStorePath;
            if(Settings.Default.IntegratedSecurityPrompt)
                CRMOAuthIntegratedSecurityTrue.IsChecked = true;
            if(!Settings.Default.IntegratedSecurityPrompt)
                CRMOAuthIntegratedSecurityFalse.IsChecked = true;

            switch (Settings.Default.AuthLoginPrompt)
            {
                case "Always":
                    CRMOAuthLoginPrompt.SelectedIndex = 1;
                    break;
                case "Auto":
                    CRMOAuthLoginPrompt.SelectedIndex = 2;
                    break;
                case "Never":
                    CRMOAuthLoginPrompt.SelectedIndex = 3;
                    break;
                default:
                    break;
            }

            CRMCertificateThumprint.Text = Settings.Default.CertificateThumprint;
            CRMClientSecretClientSecret.Text = Settings.Default.ClientSecret;

            if(!string.IsNullOrEmpty(Settings.Default.CRMTypeAuth))
                AutenticationTypePanel(Utils.GetObjectEnumFromDescription<AutenticationType>(Settings.Default.CRMTypeAuth));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            var window = Window.GetWindow(this);
            window.Close();
        }

        private void SaveSettings()
        {
            Settings.Default.CRMUrl = CRMUrl.Text;

            Settings.Default.CRMTypeAuth = typeAuth.ToString();

            switch (typeAuth)
            {
                case AutenticationType.AD:
                    Settings.Default.CRMUserName = CRMADUserName.Text;
                    Settings.Default.CRMPassword = CRMADPassword.Text;

                    Settings.Default.ClientId = "";
                    Settings.Default.RedirectUri = "";
                    Settings.Default.TokenCacheStorePath = "";
                    Settings.Default.IntegratedSecurityPrompt = false;
                    Settings.Default.AuthLoginPrompt = "";
                    Settings.Default.CertificateThumprint = "";
                    Settings.Default.ClientSecret = "";
                    break;
                case AutenticationType.OAuth:
                    Settings.Default.CRMUserName = CRMOAuthUserName.Text;
                    Settings.Default.CRMPassword = CRMOAuthPassword.Text;
                    Settings.Default.ClientId = CRMOAuthClientId.Text;
                    Settings.Default.RedirectUri = CRMOAuthRedirectUri.Text;
                    Settings.Default.TokenCacheStorePath = CRMOAuthTokenPath.Text;
                    Settings.Default.IntegratedSecurityPrompt = CRMOAuthIntegratedSecurityTrue.IsChecked != null ? CRMOAuthIntegratedSecurityTrue.IsChecked.Value : false;
                    if (CRMOAuthLoginPrompt.SelectedIndex > 0)
                        Settings.Default.AuthLoginPrompt = ((ComboBoxItem)CRMOAuthLoginPrompt.SelectedItem).Content.ToString();

                    Settings.Default.CertificateThumprint = "";
                    Settings.Default.ClientSecret = "";
                    break;
                case AutenticationType.Certificate:
                    Settings.Default.CertificateThumprint = CRMCertificateThumprint.Text;

                    Settings.Default.CRMUserName = "";
                    Settings.Default.CRMPassword = "";
                    Settings.Default.ClientId = "";
                    Settings.Default.RedirectUri = "";
                    Settings.Default.TokenCacheStorePath = "";
                    Settings.Default.IntegratedSecurityPrompt = false;
                    Settings.Default.AuthLoginPrompt = "";
                    Settings.Default.ClientSecret = "";
                    break;
                case AutenticationType.ClientSecret:
                    Settings.Default.ClientId = CRMClientSecretClientId.Text;
                    Settings.Default.ClientSecret = CRMClientSecretClientSecret.Text;

                    Settings.Default.CRMUserName = "";
                    Settings.Default.CRMPassword = "";
                    Settings.Default.RedirectUri = "";
                    Settings.Default.TokenCacheStorePath = "";
                    Settings.Default.IntegratedSecurityPrompt = false;
                    Settings.Default.AuthLoginPrompt = "";
                    break;
                case AutenticationType.Office365:
                    Settings.Default.CRMUserName = CRMOffice365UserName.Text;
                    Settings.Default.CRMPassword = CRMOffice365Password.Text;

                    Settings.Default.CertificateThumprint = "";
                    Settings.Default.ClientSecret = "";
                    Settings.Default.RedirectUri = "";
                    Settings.Default.TokenCacheStorePath = "";
                    Settings.Default.IntegratedSecurityPrompt = false;
                    Settings.Default.AuthLoginPrompt = "";
                    break;
                default:
                    break;
            }

            Settings.Default.Save();
            Settings.Default.Reload();
        }

        private void AutenticationType_Click(object sender, RoutedEventArgs e)
        {
            AutenticationTypePanel(Utils.GetObjectEnumFromDescription<AutenticationType>(((RadioButton)sender).Content.ToString()));
        }

        private void AutenticationTypePanel(AutenticationType type)
        {
            switch (type)
            {
                case AutenticationType.AD:
                    CRMAuthenticationParametersAD.Visibility = Visibility.Visible;
                    AuthenticationTypeAD.IsChecked = true;
                    CRMAuthenticationParametersCertificate.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersClientSecret.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersOAuth.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersOffice365.Visibility = Visibility.Hidden;
                    break;
                case AutenticationType.OAuth:
                    CRMAuthenticationParametersAD.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersCertificate.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersClientSecret.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersOAuth.Visibility = Visibility.Visible;
                    AuthenticationTypeOAuth.IsChecked = true;
                    CRMAuthenticationParametersOffice365.Visibility = Visibility.Hidden;
                    break;
                case AutenticationType.Certificate:
                    CRMAuthenticationParametersAD.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersCertificate.Visibility = Visibility.Visible;
                    CRMAuthenticationParametersClientSecret.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersOAuth.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersOffice365.Visibility = Visibility.Hidden;
                    break;
                case AutenticationType.ClientSecret:
                    CRMAuthenticationParametersAD.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersCertificate.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersClientSecret.Visibility = Visibility.Visible;
                    AuthenticationTypeClientSecret.IsChecked = true;
                    CRMAuthenticationParametersOAuth.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersOffice365.Visibility = Visibility.Hidden;
                    break;
                case AutenticationType.Office365:
                    CRMAuthenticationParametersAD.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersCertificate.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersClientSecret.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersOAuth.Visibility = Visibility.Hidden;
                    CRMAuthenticationParametersOffice365.Visibility = Visibility.Visible;
                    AuthenticationTypeOffice365.IsChecked = true;
                    break;
                default:
                    break;
            }

            typeAuth = type;
            ResetAutenticationFields();
        }

        private void ResetAutenticationFields()
        {
            ValidateConnectionClose.Visibility = Visibility.Visible;
            switch (typeAuth)
            {
                case AutenticationType.AD:
                    CRMOAuthUserName.Text = "";
                    CRMOAuthPassword.Text = "";
                    CRMOAuthClientId.Text = "";
                    CRMOAuthRedirectUri.Text = "";
                    CRMOAuthTokenPath.Text = "";
                    CRMOAuthIntegratedSecurityTrue.IsChecked = false;
                    CRMOAuthIntegratedSecurityFalse.IsChecked = true;
                    CRMOAuthLoginPrompt.SelectedIndex = 0;
                    CRMCertificateThumprint.Text = "";
                    CRMClientSecretClientId.Text = "";
                    CRMClientSecretClientSecret.Text = "";
                    CRMOffice365UserName.Text = "";
                    CRMOffice365Password.Text = "";                   
                    break;
                case AutenticationType.OAuth:
                    CRMADIFDTrue.IsChecked = false;
                    CRMADIFDFalse.IsChecked = true;
                    CRMADUserName.Text = "";
                    CRMADPassword.Text = "";
                    CRMCertificateThumprint.Text = "";
                    CRMClientSecretClientId.Text = "";
                    CRMClientSecretClientSecret.Text = "";
                    CRMOffice365UserName.Text = "";
                    CRMOffice365Password.Text = "";
                    CRMOffice365UserName.Text = "";
                    CRMOffice365Password.Text = "";
                    break;
                case AutenticationType.Certificate:
                    CRMOAuthUserName.Text = "";
                    CRMOAuthPassword.Text = "";
                    CRMOAuthClientId.Text = "";
                    CRMOAuthRedirectUri.Text = "";
                    CRMOAuthTokenPath.Text = "";
                    CRMOAuthIntegratedSecurityTrue.IsChecked = false;
                    CRMOAuthIntegratedSecurityFalse.IsChecked = true;
                    CRMOAuthLoginPrompt.SelectedIndex = 0;
                    CRMADIFDTrue.IsChecked = false;
                    CRMADIFDFalse.IsChecked = true;
                    CRMADUserName.Text = "";
                    CRMADPassword.Text = "";
                    CRMClientSecretClientId.Text = "";
                    CRMClientSecretClientSecret.Text = "";
                    CRMOffice365UserName.Text = "";
                    CRMOffice365Password.Text = "";
                    break;
                case AutenticationType.ClientSecret:
                    CRMOAuthUserName.Text = "";
                    CRMOAuthPassword.Text = "";
                    CRMOAuthClientId.Text = "";
                    CRMOAuthRedirectUri.Text = "";
                    CRMOAuthTokenPath.Text = "";
                    CRMOAuthIntegratedSecurityTrue.IsChecked = false;
                    CRMOAuthIntegratedSecurityFalse.IsChecked = true;
                    CRMOAuthLoginPrompt.SelectedIndex = 0;
                    CRMADIFDTrue.IsChecked = false;
                    CRMADIFDFalse.IsChecked = true;
                    CRMADUserName.Text = "";
                    CRMADPassword.Text = "";
                    CRMCertificateThumprint.Text = "";
                    CRMOffice365UserName.Text = "";
                    CRMOffice365Password.Text = "";
                    break;
                case AutenticationType.Office365:
                    CRMOAuthUserName.Text = "";
                    CRMOAuthPassword.Text = "";
                    CRMOAuthClientId.Text = "";
                    CRMOAuthRedirectUri.Text = "";
                    CRMOAuthTokenPath.Text = "";
                    CRMOAuthIntegratedSecurityTrue.IsChecked = false;
                    CRMOAuthIntegratedSecurityFalse.IsChecked = true;
                    CRMOAuthLoginPrompt.SelectedIndex = 0;
                    CRMADIFDTrue.IsChecked = false;
                    CRMADIFDFalse.IsChecked = true;
                    CRMADUserName.Text = "";
                    CRMADPassword.Text = "";
                    CRMCertificateThumprint.Text = "";
                    CRMClientSecretClientId.Text = "";
                    CRMClientSecretClientSecret.Text = "";
                    break;
                default:
                    break;
            }
        }

        private void ValidateConnection_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            ValidateConnectionClose.Visibility = Visibility.Visible;
            ValidateConnectionOpen.Visibility = Visibility.Hidden;

            AuthenticationParameters authenticationParameters = new AuthenticationParameters();
            authenticationParameters._CRMUrl = Settings.Default.CRMUrl;
            authenticationParameters._CRMUserName = Settings.Default.CRMUserName;
            authenticationParameters._CRMPassword = Settings.Default.CRMPassword;
            authenticationParameters._ClientId = Settings.Default.ClientId;
            authenticationParameters._RedirectUri = Settings.Default.RedirectUri;
            authenticationParameters._TokenCacheStorePath = Settings.Default.TokenCacheStorePath;
            authenticationParameters._TokenCacheStorePath = Settings.Default.TokenCacheStorePath;
            authenticationParameters._IntegratedSecurityPrompt = Settings.Default.IntegratedSecurityPrompt;
            authenticationParameters._AuthenticationType = typeAuth;

            if (!string.IsNullOrEmpty(Settings.Default.AuthLoginPrompt))
                authenticationParameters._AuthLoginPrompt = Settings.Default.AuthLoginPrompt;
            else
                authenticationParameters._AuthLoginPrompt = "";

            authenticationParameters._CertificateThumprint = Settings.Default.CertificateThumprint;
            authenticationParameters._ClientSecret = Settings.Default.ClientSecret;

            CRMClient CRMClient = new CRMClient(authenticationParameters);
            if (CRMClient.GetOrganizationService())
            {
                CRMStatusConnection.Text = "Correct CRM connection";
                ValidateConnectionClose.Visibility = Visibility.Hidden;
                ValidateConnectionOpen.Visibility = Visibility.Visible;
            }
            else
            {
                ValidateConnectionClose.Visibility = Visibility.Visible;
                ValidateConnectionOpen.Visibility = Visibility.Hidden;
                CRMStatusConnection.Text = "Pending validate connection to CRM";
            }
        }

        private void CRMOAuthIntegratedSecurity_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Name == "CRMOAuthIntegratedSecurityTrue" && rb.IsChecked != null && rb.IsChecked.Value)
            {
                CRMOAuthPassword.IsEnabled = false;
                CRMOAuthPassword.Text = "";
            }
            else
            {
                CRMOAuthPassword.IsEnabled = true;
            }
        }

        private void CRMOAuthLoginPrompt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CRMOAuthLoginPrompt.SelectedIndex > 1)
            {
                CRMOAuthIntegratedSecurityTrue.Visibility = Visibility.Visible;
                CRMOAuthIntegratedSecurityFalse.Visibility = Visibility.Visible;
            }
            else
            {
                CRMOAuthIntegratedSecurityTrue.Visibility = Visibility.Hidden;
                CRMOAuthIntegratedSecurityFalse.Visibility = Visibility.Hidden;
            }
        }

        private void CRMOAuthPassword_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if(tb.Text.Length > 0)
            {
                CRMOAuthIntegratedSecurityTrue.IsEnabled = false;
                CRMOAuthIntegratedSecurityFalse.IsEnabled = false;
            }
            else
            {
                CRMOAuthIntegratedSecurityTrue.IsEnabled = true;
                CRMOAuthIntegratedSecurityFalse.IsEnabled = true;
            }
        }
    }
}
