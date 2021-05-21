using DynamicsCRMResourceSynchronization.Core.Business;
using DynamicsCRMResourceSynchronization.Core.Business.Models;
using DynamicsCRMResourceSynchronization.Core.Dynamics;
using DynamicsCRMResourceSynchronization.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static DynamicsCRMResourceSynchronization.Core.Dynamics.CRMClient;

namespace DynamicsCRMResourceSynchronization
{
    /// <summary>
    /// Interaction logic for MainControl.
    /// </summary>
    public partial class CompareResourceWindowControl : UserControl
    {
        private CRMClient CRMClient = null;
        private SolutionsBusiness solutions { get; set; }
        private List<SolutionModel> listSolutions = new List<SolutionModel>();
        private ResourcesBusiness resourcesBusiness { get; set; }
        private List<ResourceModel> listResources = new List<ResourceModel>();
        private List<ResourceModel> listInitResources = new List<ResourceModel>();

        private enum VisibilityType { Visible, Hidden, Disabled}

        private int pageIndex = 1;
        private int numberOfRecPerPage;
        //To check the paging direction according to use selection.
        private enum PagingMode  { First = 1, Next = 2, Previous = 3, Last = 4, PageCountChange = 5, Refresh = 6};

        private int CRMTypeResourceSelected = 0;
        private string CRMNameSearchResourceSelected = "";

        private string CRMNameSearchResourceDefaultText = "Search by resource name";

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareResourceWindowControl"/> class.
        /// </summary>
        public CompareResourceWindowControl()
        {
            this.InitializeComponent();           

            ActionsOfSolutions(VisibilityType.Hidden);            
            ActionsOfResources(VisibilityType.Hidden);
            ActionsOfEnvironment(VisibilityType.Visible);
            SetEnvironment();
        }

        private bool connectCRM()
        {
            AuthenticationParameters authenticationParameters = new AuthenticationParameters();

            authenticationParameters._CRMUrl = Settings.Default.CRMUrl;
            authenticationParameters._CRMUserName = Settings.Default.CRMUserName;
            authenticationParameters._CRMPassword = Settings.Default.CRMPassword;
            authenticationParameters._ClientId = Settings.Default.ClientId;
            authenticationParameters._RedirectUri = Settings.Default.RedirectUri;
            authenticationParameters._TokenCacheStorePath = Settings.Default.TokenCacheStorePath;
            authenticationParameters._TokenCacheStorePath = Settings.Default.TokenCacheStorePath;
            authenticationParameters._IntegratedSecurityPrompt = Settings.Default.IntegratedSecurityPrompt;
            authenticationParameters._AuthLoginPrompt = Settings.Default.AuthLoginPrompt;
            authenticationParameters._CertificateThumprint = Settings.Default.CertificateThumprint;
            authenticationParameters._ClientSecret = Settings.Default.ClientSecret;
            authenticationParameters._AuthenticationType = Utils.GetObjectEnumFromDescription<AutenticationType>(Settings.Default.CRMTypeAuth);

            CRMClient = new CRMClient(authenticationParameters);
            if(CRMClient.GetOrganizationService())
                return true;
            else
                return false;
        }

        private void ConfiEnvironment_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            w.Title = "Setting up the CRM environment";
            w.Content = new LoginWindowControl();
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.ShowDialog();
        }

        private void ConfigPaths_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            w.Title = "Paths local of resources type";
            w.Content = new PathsWindowControl();
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.ShowDialog();
        }

        private void CRMLoadSolutions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connectCRM())
                {
                    CRMSolutions.ItemsSource = null;
                    CRMLoadSolutionsAsync();                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to retrieve CRM solutions : '{0}'", ex.Message));
            }
        }

        private async Task<bool> CRMLoadSolutionsAsync()
        {
            bool finish = false;
            ActionsOfLoading(Visibility.Visible, "Loading solutions from the environment");
            ActionsOfSolutions(VisibilityType.Disabled);
            ActionsOfResources(VisibilityType.Disabled);
            ActionsOfEnvironment(VisibilityType.Disabled);

            await Task.Run(() =>
            {
                solutions = new SolutionsBusiness(CRMClient, reloadSettingsToModel());
                listSolutions = solutions.GetSolutionsManaged();
                if (listSolutions.Count > 0)
                {
                    listSolutions.Insert(0, new SolutionModel() { solutionid = "", friendlyname = "-- Select a solution --" });
                    finish = true;
                }                
            }).ContinueWith(resp => {
                
            });

            ActionsOfLoading(Visibility.Hidden);
            ActionsOfEnvironment(VisibilityType.Visible);

            if (finish)
            {
                CRMSolutions.ItemsSource = listSolutions.OrderBy(k => k.friendlyname);
                CRMSolutions.SelectedValue = "solutionid";
                CRMSolutions.SelectedItem = "friendlyname";
                CRMSolutions.DisplayMemberPath = "friendlyname";
                CRMSolutions.SelectedValuePath = "solutionid";
                CRMSolutions.SelectedIndex = 0;
                ActionsOfSolutions(VisibilityType.Visible);
            }
            else
            {
                CRMSolutions.SelectedIndex = 0;
                ActionsOfSolutions(VisibilityType.Hidden);
            }

            return finish;
        }

        private void CRMSolutions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CRMLoadResourcesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to retrieve CRM solution resources : {0} - {0}", ((SolutionModel)CRMSolutions.SelectedItem).solutionid, ex.Message));
            }
        }

        private async Task<bool> CRMLoadResourcesAsync()
        {
            bool finish = false;
            ActionsOfLoading(Visibility.Visible, "Loading solution resources");
            ActionsOfSolutions(VisibilityType.Disabled);
            ActionsOfResources(VisibilityType.Hidden);
            ActionsOfEnvironment(VisibilityType.Disabled);

            listResources.Clear();

            if (CRMSolutions.SelectedItem != null && CRMSolutions.SelectedIndex > 0)
            {
                Guid solutionParse = Guid.Empty;
                if (Guid.TryParse(((SolutionModel)CRMSolutions.SelectedItem).solutionid, out solutionParse))
                {
                    await Task.Run(() =>
                    {
                        if (connectCRM())
                        {
                            resourcesBusiness = new ResourcesBusiness(CRMClient, reloadSettingsToModel());
                            listResources = resourcesBusiness.GetResourcesFromSolution(solutionParse);
                            listInitResources = listResources;
                            listResources = listResources.OrderBy(k => k.name).ToList();
                            if (listResources.Count > 0)
                            {
                                finish = true;
                            }
                        }
                    }).ContinueWith(resp => {

                    });                   
                }
                else
                {
                    ActionsOfResources(VisibilityType.Hidden);
                }
            }
            else
            {
                ActionsOfResources(VisibilityType.Hidden);
            }

            ActionsOfLoading(Visibility.Hidden);
            ActionsOfEnvironment(VisibilityType.Visible);
            ActionsOfSolutions(VisibilityType.Visible);

            if (finish)
            {
                Navigate((int)PagingMode.PageCountChange);
                ActionsOfResources(VisibilityType.Visible);
                CRMTypeResource.SelectedIndex = 0;
            }
            else
            {
                ActionsOfResources(VisibilityType.Hidden);
                CRMSolutions.SelectedIndex = 0;
            }

            return finish;
        }

        private SettingsModel reloadSettingsToModel()
        {
            SettingsModel sm = new SettingsModel();
            sm.CRMUrl = Settings.Default.CRMUrl;
            sm.CRMUserName = Settings.Default.CRMUserName;
            sm.CRMPassword = Settings.Default.CRMPassword;
            sm.ClientId = Settings.Default.ClientId;
            sm.RedirectUri = Settings.Default.RedirectUri;
            sm.TokenCacheStorePath = Settings.Default.TokenCacheStorePath;
            sm.IntegratedSecurityPrompt = Settings.Default.IntegratedSecurityPrompt;
            sm.AuthLoginPrompt = Settings.Default.AuthLoginPrompt;
            sm.CertificateThumprint = Settings.Default.CertificateThumprint;
            sm.ClientSecret = Settings.Default.ClientSecret;
            sm.CRMTypeAuth = Settings.Default.CRMTypeAuth;
            sm.PathHTML = Settings.Default.PathHTML;
            sm.PathCSS = Settings.Default.PathCSS;
            sm.PathJS = Settings.Default.PathJS;
            sm.PathXML = Settings.Default.PathXML;
            sm.PathPNG = Settings.Default.PathPNG;
            sm.PathJPG = Settings.Default.PathJPG;
            sm.PathGIF = Settings.Default.PathGIF;
            sm.PathXAP = Settings.Default.PathXAP;
            sm.PathXSL = Settings.Default.PathXSL;
            sm.PathICO = Settings.Default.PathICO;
            sm.PathSVG = Settings.Default.PathSVG;
            sm.PathRESX = Settings.Default.PathRESX;

            return sm;
        }

        private void ActionsOfResources(VisibilityType type)
        {
            switch (type)
            {
                case VisibilityType.Visible:
                    CRMNameSearchResource.IsEnabled = true;
                    CRMTypeResource.IsEnabled = true;
                    CRMTypeResource.Opacity = 1;
                    CRMNameSearchResource.IsEnabled = true;
                    CRMNameSearchResource.Opacity = 1;
                    CRMFilterResources.IsEnabled = true;
                    CRMFilterResources.Opacity = 1;
                    CRMFilterResources.Visibility = Visibility.Visible;
                    GridResourceActions.Visibility = Visibility.Visible;
                    TextResourceActions.Visibility = Visibility.Visible;
                    GridPagination.Visibility = Visibility.Visible;
                    DataResources.Visibility = Visibility.Visible;

                    CRMUploadResource.IsEnabled = true;
                    CRMUploadResource.Opacity = 1;
                    CRMUploadResource.Visibility = Visibility.Visible;
                    CRMPublishResource.IsEnabled = true;
                    CRMPublishResource.Opacity = 1;
                    CRMPublishResource.Visibility = Visibility.Visible;
                    CRMCompareResources.IsEnabled = true;
                    CRMCompareResources.Opacity = 1;
                    CRMCompareResources.Visibility = Visibility.Visible;
                    CRMDownloadResource.IsEnabled = true;
                    CRMDownloadResource.Opacity = 1;
                    CRMDownloadResource.Visibility = Visibility.Visible;
                    cbNumberOfRecords.Opacity = 1;
                    cbNumberOfRecords.IsEnabled = true;
                    cbNumberOfRecords.Visibility = Visibility.Visible;
                    break;
                case VisibilityType.Hidden:
                    DataResources.ItemsSource = null;
                    CRMNameSearchResource.IsEnabled = false;
                    CRMTypeResource.IsEnabled = false;
                    CRMNameSearchResource.IsEnabled = false;
                    CRMFilterResources.IsEnabled = false;
                    CRMFilterResources.Visibility = Visibility.Hidden;
                    GridResourceActions.Visibility = Visibility.Hidden;
                    TextResourceActions.Visibility = Visibility.Hidden;
                    GridPagination.Visibility = Visibility.Hidden;
                    DataResources.Visibility = Visibility.Hidden;

                    CRMUploadResource.IsEnabled = false;
                    CRMUploadResource.Visibility = Visibility.Hidden;
                    CRMPublishResource.IsEnabled = false;
                    CRMPublishResource.Visibility = Visibility.Hidden;
                    CRMCompareResources.IsEnabled = false;
                    CRMCompareResources.Visibility = Visibility.Hidden;
                    CRMDownloadResource.IsEnabled = false;
                    CRMDownloadResource.Visibility = Visibility.Hidden;
                    cbNumberOfRecords.IsEnabled = false;
                    cbNumberOfRecords.Visibility = Visibility.Hidden;
                    break;
                case VisibilityType.Disabled:
                    CRMNameSearchResource.IsEnabled = false;
                    CRMTypeResource.IsEnabled = false;
                    CRMTypeResource.Opacity = 0.4;
                    CRMNameSearchResource.IsEnabled = false;
                    CRMNameSearchResource.Opacity = 0.4;
                    CRMFilterResources.IsEnabled = false;
                    CRMFilterResources.Opacity = 0.4;
                    CRMFilterResources.Visibility = Visibility.Visible;

                    CRMUploadResource.IsEnabled = false;
                    CRMUploadResource.Opacity = 0.4;
                    CRMUploadResource.Visibility = Visibility.Visible;
                    CRMPublishResource.IsEnabled = false;
                    CRMPublishResource.Opacity = 0.4;
                    CRMPublishResource.Visibility = Visibility.Visible;
                    CRMCompareResources.IsEnabled = false;
                    CRMCompareResources.Opacity = 0.4;
                    CRMCompareResources.Visibility = Visibility.Visible;
                    CRMDownloadResource.IsEnabled = false;
                    CRMDownloadResource.Opacity = 0.4;
                    CRMDownloadResource.Visibility = Visibility.Visible;
                    cbNumberOfRecords.Opacity = 0.4;
                    cbNumberOfRecords.IsEnabled = false;
                    cbNumberOfRecords.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

            ActionsOfActionsOfResources();
        }

        private void ActionsOfActionsOfResources()
        {
            bool enableBtnUploadAndPublishResource = true;
            bool enableBtnCompareResource = true;

            foreach (var resource in listResources.Where(k => k.selectResource))
            {
                if (enableBtnUploadAndPublishResource && String.IsNullOrEmpty(resource.pathlocal))
                    enableBtnUploadAndPublishResource = false;

                if (enableBtnCompareResource && String.IsNullOrEmpty(resource.contentLocal))
                    enableBtnCompareResource = false;
            }

            //Enabled/Disabled button donwload resources
            if (listResources.Where(k => k.selectResource).Count() > 0)
            {
                CRMDownloadResource.Opacity = 1;
                CRMDownloadResource.IsEnabled = true;
                CRMDownloadResource.Visibility = Visibility.Visible;
                if (enableBtnCompareResource)
                {
                    CRMCompareResources.Opacity = 1;
                    CRMCompareResources.IsEnabled = true;
                }
                else
                {
                    CRMCompareResources.Opacity = 0.4;
                    CRMCompareResources.IsEnabled = false;
                }
            }
            else
            {
                CRMDownloadResource.Opacity = 0.4;
                CRMDownloadResource.IsEnabled = false;
                CRMCompareResources.Opacity = 0.4;
                CRMCompareResources.IsEnabled = false;                
            }

            //Enabled/Disabled button upload and publish resources
            if (listResources.Where(k => k.selectResource).Count() > 0 && enableBtnUploadAndPublishResource)
            {
                CRMUploadResource.IsEnabled = true;
                CRMUploadResource.Opacity = 1;
                CRMPublishResource.IsEnabled = true;
                CRMPublishResource.Opacity = 1;
            }
            else
            {
                CRMUploadResource.Opacity = 0.4;
                CRMUploadResource.IsEnabled = false;
                CRMPublishResource.Opacity = 0.4;
                CRMPublishResource.IsEnabled = false;
            }
        }

        private void ActionsOfSolutions(VisibilityType type)
        {
            switch (type)
            {
                case VisibilityType.Visible:
                    CRMSolutions.IsEnabled = true;
                    CRMSolutions.Opacity = 1;
                    CRMLoadSolutions.Opacity = 1;
                    CRMLoadSolutions.IsEnabled = true;
                    break;
                case VisibilityType.Hidden:
                    CRMSolutions.IsEnabled = false;
                    CRMSolutions.Opacity = 0.4;
                    CRMLoadSolutions.Opacity = 0.4;
                    CRMLoadSolutions.IsEnabled = false;
                    ActionsOfResources(type);
                    break;
                case VisibilityType.Disabled:
                    CRMSolutions.ItemsSource = listSolutions.OrderBy(k => k.friendlyname);
                    CRMSolutions.IsEnabled = false;
                    CRMSolutions.Opacity = 0.4;
                    CRMLoadSolutions.Opacity = 0.4;
                    CRMLoadSolutions.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        private void ActionsOfEnvironment(VisibilityType type)
        {
            switch (type)
            {
                case VisibilityType.Visible:
                    ConfigEnvironment.Visibility = Visibility.Visible;
                    ConfigEnvironment.IsEnabled = true;
                    ConfigEnvironment.Opacity = 1;
                    ConfigPaths.Visibility = Visibility.Visible;
                    ConfigPaths.IsEnabled = true;
                    ConfigPaths.Opacity = 1;
                    break;
                case VisibilityType.Hidden:
                    ConfigEnvironment.Visibility = Visibility.Hidden;
                    ConfigEnvironment.IsEnabled = false;
                    ConfigEnvironment.Opacity = 0.4;
                    ConfigPaths.Visibility = Visibility.Hidden;
                    ConfigPaths.IsEnabled = false;
                    ConfigPaths.Opacity = 0.4;
                    break;
                case VisibilityType.Disabled:
                    ConfigEnvironment.IsEnabled = false;
                    ConfigEnvironment.Opacity = 0.4;
                    ConfigPaths.IsEnabled = false;
                    ConfigPaths.Opacity = 0.4;
                    break;
                default:
                    break;
            }
        }

        private void ActionsOfLoading(Visibility type, string text = "")
        {
            LoadingText.Text = text;

            switch (type)
            {
                case Visibility.Visible:
                    Loading.Visibility = Visibility.Visible;
                    break;
                case Visibility.Collapsed:
                    Loading.Visibility = Visibility.Collapsed;
                    break;
                case Visibility.Hidden:
                    Loading.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

        private void SetEnvironment()
        {
            ActionsOfLoading(Visibility.Hidden);
            CRMNameSearchResource.Text = CRMNameSearchResourceDefaultText;
            CRMLoadSolutions.Opacity = 1;
            CRMLoadSolutions.IsEnabled = true;
            CRMUser.Text = Settings.Default.CRMUserName;
            CRMUrl.Text = Settings.Default.CRMUrl;
        }

        #region Pagination 
        private void btnFirst_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.First);
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Next);
        }

        private void btnPrev_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Previous);
        }

        private void btnLast_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Last);
        }

        private void cbNumberOfRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Navigate((int)PagingMode.PageCountChange);
        }

        private void Navigate(int mode)
        {
            try
            {
                int from = 0;
                int to = 0;

                if (CRMTypeResourceSelected > 0)
                {
                    listResources = listResources.Where(k => k.webresourcetype == CRMTypeResourceSelected).ToList();
                }

                if (!String.IsNullOrEmpty(CRMNameSearchResourceSelected))
                {
                    listResources = listResources.Where(k => k.name.Contains(CRMNameSearchResourceSelected)).ToList();
                }

                switch (mode)
                {
                    case (int)PagingMode.Next:
                        btnPrev.IsEnabled = true;
                        btnPrev.Opacity = 1;
                        btnFirst.IsEnabled = true;
                        btnFirst.Opacity = 1;

                        DataResources.ItemsSource = null;

                        from = (pageIndex * numberOfRecPerPage);

                        pageIndex += 1;

                        if (listResources.Count >= (from + numberOfRecPerPage))
                        {
                            DataResources.ItemsSource = listResources.GetRange(from, numberOfRecPerPage);
                            to = (pageIndex * numberOfRecPerPage);
                        }
                        else
                        {
                            DataResources.ItemsSource = listResources.GetRange(from, listResources.Count - from);
                            to = listResources.Count;
                        }

                        lblpageInformation.Content = from + " to " + to + " of " + listResources.Count;

                        if (to >= listResources.Count)
                        {
                            btnNext.IsEnabled = false;
                            btnNext.Opacity = 0.4;
                            btnLast.IsEnabled = false;
                            btnLast.Opacity = 0.4;
                        }

                        break;
                    case (int)PagingMode.Previous:
                        btnNext.IsEnabled = true;
                        btnNext.Opacity = 1;
                        btnLast.IsEnabled = true;
                        btnLast.Opacity = 1;
                        if (pageIndex > 1)
                        {
                            pageIndex -= 1;
                            DataResources.ItemsSource = null;
                            if (pageIndex == 1)
                            {
                                DataResources.ItemsSource = listResources.GetRange(0, numberOfRecPerPage);
                                from = 1;
                                to = listResources.GetRange(0, numberOfRecPerPage).Count;
                            }
                            else
                            {
                                from = ((pageIndex * numberOfRecPerPage)- numberOfRecPerPage);
                                DataResources.ItemsSource = listResources.GetRange(from, numberOfRecPerPage);                                
                                to = from + numberOfRecPerPage;
                            }
                        }

                        lblpageInformation.Content = from + " to " + to + " of " + listResources.Count;

                        if (pageIndex <= 1)
                        {
                            btnPrev.IsEnabled = false;
                            btnPrev.Opacity = 0.4;
                            btnFirst.IsEnabled = false;
                            btnFirst.Opacity = 0.4;
                        }
                        break;

                    case (int)PagingMode.First:
                        pageIndex = 2;
                        Navigate((int)PagingMode.Previous);
                        break;
                    case (int)PagingMode.Last:
                        if (numberOfRecPerPage > 0)
                        {
                            pageIndex = listResources.Count / numberOfRecPerPage;
                            Navigate((int)PagingMode.Next);
                        }
                        break;
                    case (int)PagingMode.PageCountChange:
                        pageIndex = 1;
                        numberOfRecPerPage = Convert.ToInt32(((ComboBoxItem)cbNumberOfRecords.SelectedItem).Content);
                        DataResources.ItemsSource = null;
                        from = 1;
                        if (listResources.Count > 0)
                        {
                            if (numberOfRecPerPage < listResources.Count)
                            {
                                DataResources.ItemsSource = listResources.GetRange(0, numberOfRecPerPage);                               
                            }
                            else
                            {
                                DataResources.ItemsSource = listResources.GetRange(0, listResources.Count);
                            }                          
                        }

                        to = DataResources.Items.Count;
                        lblpageInformation.Content = from + " to " + to + " of " + listResources.Count;

                        if (listResources.Count <= (pageIndex * numberOfRecPerPage))
                        {
                            btnNext.IsEnabled = false;
                            btnNext.Opacity = 0.4;
                            btnLast.IsEnabled = false;
                            btnLast.Opacity = 0.4;
                        }
                        else
                        {
                            btnNext.IsEnabled = true;
                            btnNext.Opacity = 1;
                            btnLast.IsEnabled = true;
                            btnLast.Opacity = 1;
                        }

                        btnPrev.IsEnabled = false;
                        btnPrev.Opacity = 0.4;
                        btnFirst.IsEnabled = false;
                        btnFirst.Opacity = 0.4;
                        break;
                    case (int)PagingMode.Refresh:
                        DataResources.ItemsSource = null;
                        pageIndex = 1;
                        from = 1;
                        if (numberOfRecPerPage < listResources.Count)
                        {
                            DataResources.ItemsSource = listResources.GetRange(0, numberOfRecPerPage);
                        }
                        else
                        {
                            DataResources.ItemsSource = listResources.GetRange(0, listResources.Count);
                        }
                        to = DataResources.Items.Count;
                        lblpageInformation.Content = from + " to " + to + " of " + listResources.Count;

                        if (listResources.Count <= (pageIndex * numberOfRecPerPage))
                        {
                            btnNext.IsEnabled = false;
                            btnNext.Opacity = 0.4;
                            btnLast.IsEnabled = false;
                            btnLast.Opacity = 0.4;
                        }
                        else
                        {
                            btnNext.IsEnabled = true;
                            btnNext.Opacity = 1;
                            btnLast.IsEnabled = true;
                            btnLast.Opacity = 1;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to navigate in the resource list : '{0}'", ex.Message));
            }                   
        }
        #endregion

        #region Events to Grid
        /// <summary>
        /// Detect resource selection in header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel c in listResources)
            {
                c.selectResource = true;
            }
            Navigate((int)PagingMode.Refresh);
            ActionsOfActionsOfResources();
        }
        /// <summary>
        ///  Detect resource deselection in header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel c in listResources)
            {
                c.selectResource = false;
            }
            Navigate((int)PagingMode.Refresh);
            ActionsOfActionsOfResources();
        }
        private void ResourceChecked(object sender, RoutedEventArgs e)
        {
            if (((DataGridCell)sender).DataContext is ResourceModel)
            {
                ((ResourceModel)((DataGridCell)sender).DataContext).selectResource = true;
                ActionsOfActionsOfResources();
            }
        }
        private void ResourceUnChecked(object sender, RoutedEventArgs e)
        {
            if (((DataGridCell)sender).DataContext is ResourceModel)
            {
                ((ResourceModel)((DataGridCell)sender).DataContext).selectResource = false;
                ActionsOfActionsOfResources();
            }
        }
        /// <summary>
        /// Allow resource selection when selecting the resource line in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row == null) return;
            if (row.IsEditing) return;
            if (!row.IsSelected) row.IsSelected = true;
        }
        /// <summary>
        /// Allow resource selection when selecting the resource line in grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell == null) return;
            if (cell.IsEditing) return;
            if (!cell.IsFocused) cell.Focus();
        }

        #endregion

        #region Search TextBox
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchResourceTextInTextBox(sender);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchResourceTextInTextBox(sender);
        }

        private void SearchResourceTextInTextBox(object sender)
        {
            TextBox tb = ((TextBox)sender);
            if (tb.IsFocused)
            {
                if (tb.Text == CRMNameSearchResourceDefaultText)
                {
                    tb.Text = "";
                }
            }
            else
            {
                if (tb.Text == "")
                {
                    tb.Text = CRMNameSearchResourceDefaultText;
                    CRMNameSearchResourceSelected = "";
                }
            }
        }
        #endregion

        private void CRMFilterResources_Click(object sender, RoutedEventArgs e)
        {
            listResources = listInitResources;

            if (CRMTypeResource.SelectedItem != null && CRMTypeResource.SelectedIndex > 0)
            {
                CRMTypeResourceSelected = Convert.ToInt32(((ComboBoxItem)CRMTypeResource.SelectedItem).Tag);
            }
            else
            {
                CRMTypeResourceSelected = 0;
            }

            if (CRMNameSearchResource.Text != CRMNameSearchResourceDefaultText)
            {
                CRMNameSearchResourceSelected = CRMNameSearchResource.Text;
            }
            else
            {
                CRMNameSearchResourceSelected = "";
            }

            Navigate((int)PagingMode.Refresh);
        }

        private void resetSelectedResourcesAfterAction()
        {
            foreach (ResourceModel c in listResources)
            {
                c.selectResource = false;
            }
            Navigate((int)PagingMode.Refresh);
            ActionsOfActionsOfResources();
        }

        private void CRMCompareResources_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ActionsOfLoading(Visibility.Visible, "Comparing selected resources");
                ActionsOfSolutions(VisibilityType.Disabled);
                ActionsOfResources(VisibilityType.Disabled);
                ActionsOfEnvironment(VisibilityType.Disabled);

                Window w = new Window();
                w.Title = "Differences of resources";
                w.Content = new DifferencesResourceWindowControl(reloadSettingsToModel(), resourcesBusiness, listResources.Where(k => k.selectResource == true).ToList());
                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                w.ShowDialog();

                ActionsOfLoading(Visibility.Hidden);
                ActionsOfSolutions(VisibilityType.Visible);
                ActionsOfResources(VisibilityType.Visible);
                ActionsOfEnvironment(VisibilityType.Visible);
                resetSelectedResourcesAfterAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot compare selected resources : '{0}'", ex.Message));
            }            
        }

        private void CRMDownloadResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CRMDownloadResourceAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to download selected resources : '{0}'", ex.Message));
            }            
        }

        private async Task<bool> CRMDownloadResourceAsync()
        {
            bool finish = false;
            ActionsOfLoading(Visibility.Visible, "Downloading solution resources");
            ActionsOfSolutions(VisibilityType.Disabled);
            ActionsOfResources(VisibilityType.Disabled);
            ActionsOfEnvironment(VisibilityType.Disabled);

            await Task.Run(() =>
            {
                foreach (var resource in listResources.Where(k => k.selectResource))
                {
                    PathAndNameResourceModel resourceModel = Utils.getFormatPathAndNameResource(reloadSettingsToModel(), resource.name, resource.webresourcetype);

                    if (!string.IsNullOrEmpty(resourceModel.path))
                    {

                        if (!Directory.Exists(resourceModel.path) && Utils.DirectoryHasPermission(string.Join("\\", resourceModel.path.Split('\\').Take(resourceModel.path.Split('\\').Length - 2)), FileSystemRights.Write))
                            Directory.CreateDirectory(resourceModel.path);

                        if (File.Exists(resourceModel.path + resourceModel.name) && Utils.DirectoryHasPermission(resourceModel.path, FileSystemRights.Delete))
                        {
                            //Backup of resource ¿?

                            File.Delete(resourceModel.path + resourceModel.name);
                        }

                        if (Utils.DirectoryHasPermission(resourceModel.path, FileSystemRights.Write))
                        {
                            File.WriteAllText(resourceModel.path + resourceModel.name, resource.contentCRM);
                            resourcesBusiness.GetResourceLocal(resource);
                        }
                    }
                }
            }).ContinueWith(resp => {

            });

            ActionsOfLoading(Visibility.Hidden);
            ActionsOfSolutions(VisibilityType.Visible);
            ActionsOfResources(VisibilityType.Visible);
            ActionsOfEnvironment(VisibilityType.Visible);
            resetSelectedResourcesAfterAction();

            return finish;
        }

        private void CRMUploadResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("The selected resources will be updated in CRM by the latest version of the local resource. You're sure?", "Update resources", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    CRMUploadResourceAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot upload selected resources : '{0}'", ex.Message));
            }           
        }

        private async Task<bool> CRMUploadResourceAsync()
        {
            bool finish = false;
            ActionsOfLoading(Visibility.Visible, "Uploading selected resources");
            ActionsOfSolutions(VisibilityType.Disabled);
            ActionsOfResources(VisibilityType.Disabled);
            ActionsOfEnvironment(VisibilityType.Disabled);
            List<ExecuteMultipleResponseModel> responses = new List<ExecuteMultipleResponseModel>();

            await Task.Run(() =>
            {
                responses = resourcesBusiness.UploadResources(listResources.Where(k => k.selectResource && !string.IsNullOrEmpty(k.pathlocal)).ToList());
            }).ContinueWith(resp => {

            });

            GetLastContentResourceAfterUpdate(responses);

            ActionsOfSolutions(VisibilityType.Visible);
            ActionsOfResources(VisibilityType.Visible);
            ActionsOfEnvironment(VisibilityType.Visible);
            resetSelectedResourcesAfterAction();
            ActionsOfLoading(Visibility.Hidden);

            return finish;
        }

        private void CRMPublishResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("The selected resources will be updated and published in CRM by the latest version of the local resource. You're sure?", "Update and publish resources", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    CRMPublishResourceAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to upload and publish selected resources : '{0}'", ex.Message));
            }
        }

        private async Task<bool> CRMPublishResourceAsync()
        {
            bool finish = false;
            ActionsOfLoading(Visibility.Visible, "Publishing selected resources");
            ActionsOfSolutions(VisibilityType.Disabled);
            ActionsOfResources(VisibilityType.Disabled);
            ActionsOfEnvironment(VisibilityType.Disabled);
            List<ExecuteMultipleResponseModel> responses = new List<ExecuteMultipleResponseModel>();
            await Task.Run(() =>
            {
                responses = resourcesBusiness.UploadAndPublishResources(listResources.Where(k => k.selectResource && !string.IsNullOrEmpty(k.pathlocal)).ToList());
               
            }).ContinueWith(resp => {

            });

            GetLastContentResourceAfterUpdate(responses);
            
            ActionsOfSolutions(VisibilityType.Visible);
            ActionsOfResources(VisibilityType.Visible);
            ActionsOfEnvironment(VisibilityType.Visible);
            resetSelectedResourcesAfterAction();
            ActionsOfLoading(Visibility.Hidden);

            return finish;
        }

        private void GetLastContentResourceAfterUpdate (List<ExecuteMultipleResponseModel> responses)
        {
            try
            {
                foreach (var item in responses)
                {
                    //Update resource CRM from new content upload
                    if (!item.error)
                    {
                        int indexResourceInList = listResources.IndexOf(listResources.Where(k => k.resourceid == item.id).FirstOrDefault());
                        ResourceModel lastContentResourceFromCRM = resourcesBusiness.DonwloadResourceFromCRM(item.id);
                        listResources[indexResourceInList] = lastContentResourceFromCRM;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CRMAddResources_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CRMSolutions.SelectedItem != null && CRMSolutions.SelectedIndex > 0)
                {
                    Guid solutionParse = Guid.Empty;
                    if (Guid.TryParse(((SolutionModel)CRMSolutions.SelectedItem).solutionid, out solutionParse))
                    {
                        ActionsOfLoading(Visibility.Visible, "Add resources to the solution");
                        ActionsOfSolutions(VisibilityType.Disabled);
                        ActionsOfResources(VisibilityType.Disabled);
                        ActionsOfEnvironment(VisibilityType.Disabled);

                        Window w = new Window();
                        w.Title = "Add resources to the solution";
                        w.Content = new AddResourceWindowControl(CRMClient, reloadSettingsToModel(), resourcesBusiness, listResources, (SolutionModel)CRMSolutions.SelectedItem);
                        w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        w.ShowDialog();

                        ActionsOfLoading(Visibility.Hidden);
                        ActionsOfSolutions(VisibilityType.Visible);
                        ActionsOfResources(VisibilityType.Visible);
                        ActionsOfEnvironment(VisibilityType.Visible);

                        try
                        {
                            CRMLoadResourcesAsync();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Unable to retrieve CRM solution resources : {0} - {0}", ((SolutionModel)CRMSolutions.SelectedItem).solutionid, ex.Message));
                        }

                        resetSelectedResourcesAfterAction();
                    }
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot add resources to solution : '{0}'", ex.Message));
            }
        }

        private void CompareResource_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Loading = new Controls.LoadingControl();
        }
    }
}