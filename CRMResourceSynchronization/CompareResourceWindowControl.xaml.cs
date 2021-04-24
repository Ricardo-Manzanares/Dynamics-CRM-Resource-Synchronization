using CRMResourceSynchronization.Core.Business;
using CRMResourceSynchronization.Core.Business.Models;
using CRMResourceSynchronization.Core.Dynamics;
using CRMResourceSynchronization.Extensions;
using CRMResourceSynchronization.Properties;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using static CRMResourceSynchronization.Core.Dynamics.CRMClient;

namespace CRMResourceSynchronization
{
    /// <summary>
    /// Interaction logic for MainControl.
    /// </summary>
    public partial class CompareResourceWindowControl : UserControl
    {
        private CRMClient CRMClient = null;
        public SolutionsBusiness solutions { get; set; }
        private List<SolutionModel> listSolutions = new List<SolutionModel>();
        public ResourcesBusiness resources { get; set; }
        private List<ResourceModel> listResources = new List<ResourceModel>();

        private int pageIndex = 1;
        private int numberOfRecPerPage;
        //To check the paging direction according to use selection.
        private enum PagingMode  { First = 1, Next = 2, Previous = 3, Last = 4, PageCountChange = 5, Refresh = 6 };

        private int CRMTypeResourceSelected = 0;
        private string CRMNameSearchResourceSelected = "";

        public string CRMNameSearchResourceDefaultText = "Search by resource name";

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareResourceWindowControl"/> class.
        /// </summary>
        public CompareResourceWindowControl()
        {
            this.InitializeComponent();
            ActionsOfSolutions(Visibility.Hidden);
            CRMNameSearchResource.Text = CRMNameSearchResourceDefaultText;
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
            w.Width = this.ActualWidth;
            w.Height = this.ActualHeight;
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.ShowDialog();
        }

        private void ConfigPaths_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void CRMLoadSolutions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connectCRM())
                {
                    CRMSolutions.ItemsSource = null;
                    solutions = new SolutionsBusiness(CRMClient);
                    listSolutions = solutions.GetSolutionsManaged();
                    if (listSolutions.Count > 0)
                    {
                        listSolutions.Insert(0, new SolutionModel() { solutionid = "", friendlyname = "-- Seleccionar una solución --" });
                        ActionsOfSolutions(Visibility.Visible);
                    }
                    else
                    {
                        ActionsOfSolutions(Visibility.Hidden);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("No se pueden recuperar las soluciones de CRM : '{0}'", ex.Message));
            }
        }
       
        private void CRMSolutions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                listResources.Clear();
               
                if (CRMSolutions.SelectedItem != null && CRMSolutions.SelectedIndex > 0)
                {
                    Guid solutionParse = Guid.Empty;
                    if (Guid.TryParse(((SolutionModel)CRMSolutions.SelectedItem).solutionid, out solutionParse))
                    {
                        if (connectCRM())
                        {
                            resources = new ResourcesBusiness(CRMClient);
                            listResources = resources.GetResourcesFromSolution(solutionParse);
                            listResources = listResources.OrderBy(k => k.name).ToList();
                            if (listResources.Count > 0)
                            {
                                Navigate((int)PagingMode.PageCountChange);
                                ActionsOfResources(Visibility.Visible);
                                CRMTypeResource.SelectedIndex = 0;
                            }
                            else
                            {
                                ActionsOfResources(Visibility.Hidden);
                                CRMSolutions.SelectedIndex = 0;
                            }
                        }
                    }
                }
                else
                {
                    ActionsOfResources(Visibility.Hidden);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("No se pueden recuperar los recursos de la solucion de CRM : {0} - {0}", ((SolutionModel)CRMSolutions.SelectedItem).solutionid, ex.Message));
            }
        }

        private void ActionsOfResources(Visibility type)
        {
            if (type == Visibility.Visible)
            {
                CRMNameSearchResource.IsEnabled = true;
                CRMTypeResource.IsEnabled = true;
                CRMNameSearchResource.IsEnabled = true;
                CRMNameSearchResource.Opacity = 1;
                CRMSearchResource.IsEnabled = true;
                GridResourceActions.Visibility = Visibility.Visible;
                GridPagination.Visibility = Visibility.Visible;
                DataResources.Visibility = Visibility.Visible;
            }
            else
            {
                DataResources.ItemsSource = null;
                CRMNameSearchResource.IsEnabled = false;
                CRMTypeResource.IsEnabled = false;
                CRMNameSearchResource.IsEnabled = false;
                CRMNameSearchResource.Opacity = 0.4;
                CRMSearchResource.IsEnabled = false;
                GridResourceActions.Visibility = Visibility.Hidden;
                GridPagination.Visibility = Visibility.Hidden;
                DataResources.Visibility = Visibility.Hidden;
            }
            ActionsOfActionsOfResources();
        }

        private void ActionsOfActionsOfResources()
        {
            bool enableBtnUploadAndPublishResurce = true;

            foreach (var resource in listResources.Where(k => k.selectResource))
            {
                if (enableBtnUploadAndPublishResurce && String.IsNullOrEmpty(resource.pathlocal))
                    enableBtnUploadAndPublishResurce = false;
            }

            //Enabled/Disabled button donwload resources
            if (listResources.Where(k => k.selectResource).Count() > 0)
            {
                CRMDownloadResource.Opacity = 1;
                CRMDownloadResource.IsEnabled = true;
                CRMDownloadResource.Visibility = Visibility.Visible;
                CRMCompareResources.Opacity = 1;
                CRMCompareResources.IsEnabled = true;
            }
            else
            {
                CRMDownloadResource.Opacity = 0.4;
                CRMDownloadResource.IsEnabled = false;
                CRMCompareResources.Opacity = 0.4;
                CRMCompareResources.IsEnabled = false;                
            }

            //Enabled/Disabled button upload and publish resources
            if (listResources.Where(k => k.selectResource).Count() > 0 && enableBtnUploadAndPublishResurce)
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

        private void ActionsOfSolutions(Visibility type)
        {
            if (type == Visibility.Visible)
            {
                CRMSolutions.ItemsSource = listSolutions.OrderBy(k => k.friendlyname);
                CRMSolutions.SelectedValue = "solutionid";
                CRMSolutions.SelectedItem = "friendlyname";
                CRMSolutions.DisplayMemberPath = "friendlyname";
                CRMSolutions.SelectedValuePath = "solutionid";
                CRMSolutions.IsEnabled = true;
                CRMSolutions.SelectedIndex = 0;
            }
            else
            {
                CRMSolutions.IsEnabled = false;
                CRMSolutions.SelectedIndex = 0;
                ActionsOfResources(Visibility.Hidden);
            }
        }

        private void SetEnvironment()
        {
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
            int count;
            switch (mode)
            {
                case (int)PagingMode.Next:
                    btnPrev.IsEnabled = true;
                    btnPrev.Opacity = 1;
                    btnFirst.IsEnabled = true;
                    btnFirst.Opacity = 1;
                    if (listResources.Count >= (pageIndex * numberOfRecPerPage))
                    {
                        DataResources.ItemsSource = null;
                        if (listResources.Count > (pageIndex * numberOfRecPerPage) + numberOfRecPerPage)
                        {                           
                            DataResources.ItemsSource = listResources.GetRange((pageIndex * numberOfRecPerPage) - numberOfRecPerPage, numberOfRecPerPage);
                            count = (pageIndex * numberOfRecPerPage) + (listResources.GetRange(pageIndex * numberOfRecPerPage, numberOfRecPerPage).Count);
                        }
                        else
                        {
                            DataResources.ItemsSource = listResources.GetRange(pageIndex * numberOfRecPerPage, listResources.Count - (pageIndex * numberOfRecPerPage));
                            count = (pageIndex * numberOfRecPerPage) + (listResources.GetRange(pageIndex * numberOfRecPerPage, listResources.Count - (pageIndex * numberOfRecPerPage)).Count);
                            pageIndex++;
                        }

                        lblpageInformation.Content = count + " of " + listResources.Count;
                    }

                    if (listResources.Count <= (pageIndex * numberOfRecPerPage))
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
                            count = listResources.GetRange(0, numberOfRecPerPage).Count;
                            lblpageInformation.Content = count + " of " + listResources.Count;
                        }
                        else
                        {
                            DataResources.ItemsSource = listResources.GetRange(pageIndex * numberOfRecPerPage, numberOfRecPerPage);
                            count = Math.Min(pageIndex * numberOfRecPerPage, listResources.Count);
                            lblpageInformation.Content = count + " of " + listResources.Count;
                        }
                    }

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
                        pageIndex = (listResources.Count / numberOfRecPerPage);
                        Navigate((int)PagingMode.Next);
                    }
                    break;
                case (int)PagingMode.PageCountChange:
                    pageIndex = 1;
                    numberOfRecPerPage = Convert.ToInt32(((ComboBoxItem)cbNumberOfRecords.SelectedItem).Content);
                    DataResources.ItemsSource = null;
                    if (listResources.Count > 0)
                    {
                        if (numberOfRecPerPage < listResources.Count)
                        {
                            DataResources.ItemsSource = listResources.GetRange(0, numberOfRecPerPage);
                            count = listResources.GetRange(0, numberOfRecPerPage).Count;
                        }
                        else
                        {
                            DataResources.ItemsSource = listResources.GetRange(0, listResources.Count);
                            count = listResources.GetRange(0, listResources.Count).Count;
                        }
                        lblpageInformation.Content = count + " de " + listResources.Count;
                    }

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
                    List<ResourceModel> newData = new List<ResourceModel>();
                    if (pageIndex > 1)
                    {
                        newData = listResources.GetRange(((pageIndex -1) * numberOfRecPerPage), listResources.Count - ((pageIndex - 1) * numberOfRecPerPage));
                    }
                    else
                    {
                        newData = listResources.GetRange(0, numberOfRecPerPage);
                    }

                    if(CRMTypeResourceSelected > 0)
                    {
                        newData = newData.Where(k => k.webresourcetype == CRMTypeResourceSelected).ToList();
                    }

                    if (!String.IsNullOrEmpty(CRMNameSearchResourceSelected))
                    {
                        newData = newData.Where(k => k.name.Contains(CRMNameSearchResourceSelected)).ToList();
                    }

                    DataResources.ItemsSource = newData;

                    lblpageInformation.Content = newData.Count + " de " + listResources.Count;

                    break;
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
            ((ResourceModel)((DataGridCell)sender).DataContext).selectResource = true;
            ActionsOfActionsOfResources();
        }
        private void ResourceUnChecked(object sender, RoutedEventArgs e)
        {
            ((ResourceModel)((DataGridCell)sender).DataContext).selectResource = false;
            ActionsOfActionsOfResources();
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

        private void CRMSearchResource_Click(object sender, RoutedEventArgs e)
        {
            if (CRMTypeResource.SelectedItem != null && CRMTypeResource.SelectedIndex > 0) {
                CRMTypeResourceSelected = Convert.ToInt32(((ComboBoxItem)CRMTypeResource.SelectedItem).Tag);
            }
            if(CRMNameSearchResource.Text != CRMNameSearchResourceDefaultText)
            {
                CRMNameSearchResourceSelected = CRMNameSearchResource.Text;
            }

            Navigate((int)PagingMode.Refresh);
        }

        private void PathToResources()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.InitializeComponent();

            IntPtr hierarchyPointer, selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer, out projectItemId, out multiItemSelect, out selectionContainerPointer);

            if (hierarchyPointer != null && hierarchyPointer.ToInt32() > 0)
            {
                IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPointer, typeof(IVsHierarchy)) as IVsHierarchy;

                if (selectedHierarchy == null)
                {
                    MessageBox.Show("Cannot find an open solution in Visual Studio");
                }
                else
                {
                    selectedHierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out selectedObject);
                    if (selectedObject == null)
                    {
                        MessageBox.Show("You do not have any active projects in the solution");
                    }
                    else
                    {
                        EnvDTE.Project selectedProject = selectedObject as EnvDTE.Project;

                        string projectName = selectedProject.FullName;
                        string projectPathName = selectedProject.FullName.Split('\\').Last();
                    }
                }
            }
            else
            {
                MessageBox.Show("Cannot find an open solution in Visual Studio");
            }
        }

        private void CRMCompareResources_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            w.Title = "Setting up the paths resources";
            w.Content = new DifferencesResourceWindowControlControl(listResources.Where(k => k.selectResource == true).FirstOrDefault());
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.ShowDialog();
        }
    }
}