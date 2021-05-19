using DynamicsCRMResourceSynchronization.Core.Business;
using DynamicsCRMResourceSynchronization.Core.Business.Models;
using DynamicsCRMResourceSynchronization.Core.DiffPlex.DiffBuilder;
using DynamicsCRMResourceSynchronization.Core.DiffPlex.DiffBuilder.Model;
using DynamicsCRMResourceSynchronization.Core.Dynamics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DynamicsCRMResourceSynchronization
{
    /// <summary>
    /// Interaction logic for AddResourceWindowControl.
    /// </summary>
    public partial class AddResourceWindowControl : UserControl
    {
        public CRMClient CRMClient { get; set; }
        private ResourcesBusiness resourcesBusiness { get; set; }

        private SolutionModel solutionDefault;
        private SolutionModel solutionSelected;
        private int AvaiableResourcesTypeSelected = 0;
        private string AvaiableResourcesTextSelected = "";
        private int ResourcesInSolutionTypeSelected = 0;
        private List<ResourceModel> lstAvaiableResources = new List<ResourceModel>();
        private List<ResourceModel> lstResourcesInSolution = new List<ResourceModel>();

        private SettingsModel settings = new SettingsModel();

        private string CRMNameFilterResourceDefaultText = "Filter by resource name";

        private enum VisibilityType { Visible, Hidden, Disabled }

        /// <summary>
        /// Initializes a new instance of the <see cref="DifferencesResourceWindowControl"/> class.
        /// </summary>
        public AddResourceWindowControl(CRMClient CRMClient, SettingsModel settings, ResourcesBusiness resourceBusiness, List<ResourceModel> resourcesToView, SolutionModel solutionSelected)
        {
            this.InitializeComponent();

            if (CRMClient is null || resourcesToView is null || resourceBusiness is null || settings is null || solutionSelected is null)
                return;

            this.CRMClient = CRMClient;
            this.resourcesBusiness = resourceBusiness;
            this.settings = settings;
            this.solutionSelected = solutionSelected;
            resourcesToView.ForEach(k => k.selectResource = false);

            ResourcesInSolution.ItemsSource = resourcesToView;
            lstResourcesInSolution = resourcesToView;

            AvaiableResourcesNameFilter.Text = CRMNameFilterResourceDefaultText;
            ResourcesInSolutionTextFilter.Text = CRMNameFilterResourceDefaultText;
            AvaiableResourcesTypeFilter.SelectedIndex = 0;
            ResourcesInSolutionTypeFilter.SelectedIndex = 0;

            CRMLoadResourcesFromSolutionDefaultAsync();
        }

        /// <summary>
        /// Donwload resources from CRM solution default
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CRMLoadResourcesFromSolutionDefaultAsync()
        {
            bool finish = false;

            ActionsOfLoading(Visibility.Visible, "Downloading default solution resources");
            ActionsOfButtons(VisibilityType.Disabled);

            await Task.Run(() =>
            {
                SolutionsBusiness solution = new SolutionsBusiness(CRMClient, settings);
                solutionDefault = solution.GetSolutionDefault();
                if (solutionDefault != null && !string.IsNullOrEmpty(solutionDefault.solutionid))
                {                    
                    Guid solutionParse = Guid.Empty;
                    if (Guid.TryParse(solutionDefault.solutionid, out solutionParse))
                    {
                        lstAvaiableResources = resourcesBusiness.GetResourcesFromSolution(solutionParse, true);

                        //Order resources by name and remove resources in solution
                        lstAvaiableResources = lstAvaiableResources.OrderBy(k => k.name).ToList();
                        if (lstAvaiableResources.Count > 0)
                        {
                            lstAvaiableResources.RemoveAll(k => lstResourcesInSolution.Select(s => s.name).Contains(k.name));
                            finish = true;
                        }
                    }
                }
            }).ContinueWith(resp => {

            });

            ActionsOfLoading(Visibility.Hidden);
            ActionsOfButtons(VisibilityType.Visible);

            if (finish && lstAvaiableResources.Count() > 0)
            {
                AvailableResourcesInDefaultSolution.ItemsSource = lstAvaiableResources;
            }          

            return finish;
        }

        private void ActionsOfLoading(Visibility type, string text = "")
        {
            LoadingText.Text = text;
            Loading.Visibility = type;
            PanelLoading.Visibility = type;
        }

        private void ActionsOfButtons(VisibilityType type)
        {
            switch (type)
            {
                case VisibilityType.Visible:
                    ResourcesInSolution.IsEnabled = true;
                    AvailableResourcesInDefaultSolution.IsEnabled = true;
                    AvaiableResourcesNameFilter.IsEnabled = true;
                    AvaiableResourcesTypeFilter.IsEnabled = true;
                    ResourcesInSolutionTextFilter.IsEnabled = true;
                    ResourcesInSolutionFilter.IsEnabled = true;
                    ResourcesInSolutionTypeFilter.IsEnabled = true;
                    AvaiableResourcesFilter.IsEnabled = true;
                    AvaiableResourcesFilter.Opacity = 1;
                    AvaiableResourcesSelectAll.IsEnabled = true;
                    AvaiableResourcesSelectAll.Opacity = 1;
                    AvaiableResourcesUnselectAll.IsEnabled = true;
                    AvaiableResourcesUnselectAll.Opacity = 1;
                    AvaiableResourcesInvertSelect.IsEnabled = true;
                    AvaiableResourcesInvertSelect.Opacity = 1;
                    RemoveResourceFromSolution.IsEnabled = true;
                    RemoveResourceFromSolution.Opacity = 1;
                    AddResourceToSolution.IsEnabled = true;
                    AddResourceToSolution.Opacity = 1;
                    ResourcesInSolutionSelectAll.IsEnabled = true;
                    ResourcesInSolutionSelectAll.Opacity = 1;
                    ResourcesInSolutionUnselect.IsEnabled = true;
                    ResourcesInSolutionUnselect.Opacity = 1;
                    ResourcesInSolutionInvertSelect.IsEnabled = true;
                    ResourcesInSolutionInvertSelect.Opacity = 1;
                    Save.IsEnabled = true;
                    Save.Opacity = 1;                    
                    break;
                case VisibilityType.Disabled:
                    ResourcesInSolution.IsEnabled = false;
                    AvailableResourcesInDefaultSolution.IsEnabled = false;
                    AvaiableResourcesNameFilter.IsEnabled = false;
                    AvaiableResourcesTypeFilter.IsEnabled = false;
                    ResourcesInSolutionTextFilter.IsEnabled = false;
                    ResourcesInSolutionFilter.IsEnabled = false;
                    ResourcesInSolutionFilter.Opacity = 0.4;
                    ResourcesInSolutionTypeFilter.IsEnabled = false;
                    AvaiableResourcesFilter.IsEnabled = false;
                    AvaiableResourcesFilter.Opacity = 0.4;
                    AvaiableResourcesSelectAll.IsEnabled = false;
                    AvaiableResourcesSelectAll.Opacity = 0.4;
                    AvaiableResourcesUnselectAll.IsEnabled = false;
                    AvaiableResourcesUnselectAll.Opacity = 0.4;
                    AvaiableResourcesInvertSelect.IsEnabled = false;
                    AvaiableResourcesInvertSelect.Opacity = 0.4;
                    RemoveResourceFromSolution.IsEnabled = false;
                    RemoveResourceFromSolution.Opacity = 0.4;
                    AddResourceToSolution.IsEnabled = false;
                    AddResourceToSolution.Opacity = 0.4;
                    ResourcesInSolutionSelectAll.IsEnabled = false;
                    ResourcesInSolutionSelectAll.Opacity = 0.4;
                    ResourcesInSolutionUnselect.IsEnabled = false;
                    ResourcesInSolutionUnselect.Opacity = 0.4;
                    ResourcesInSolutionInvertSelect.IsEnabled = false;
                    ResourcesInSolutionInvertSelect.Opacity = 0.4;
                    Save.IsEnabled = false;
                    Save.Opacity = 0.4;
                    break;
                default:
                    break;
            }
        }

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
                if (tb.Text == CRMNameFilterResourceDefaultText)
                {
                    tb.Text = "";
                }
            }
            else
            {
                if (tb.Text == "")
                {
                    tb.Text = CRMNameFilterResourceDefaultText;
                }
            }
        }
        #endregion

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveResource();
        }

        private async Task<bool> SaveResource()
        {
            bool finish = false;
            try
            {
                ActionsOfLoading(Visibility.Visible, "Link and unlink resources to the solution");
                ActionsOfButtons(VisibilityType.Disabled);

                List<ResourceModel> resourcesInDefaultSolution = new List<ResourceModel>();
                await Task.Run(() =>
                {
                    SolutionsBusiness solution = new SolutionsBusiness(CRMClient, settings);

                    foreach (ResourceModel resource in ResourcesInSolution.Items)
                    {
                        if (resource.resourceStatusInSolution == ResourceStatusInSolution.PendingToAdd && resource.resourceid != Guid.Empty)
                        {
                            try
                            {
                                solution.AddResourceToSolution(solutionSelected, resource);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }                           
                        }
                    }

                    foreach (ResourceModel resource in AvailableResourcesInDefaultSolution.Items)
                    {
                        if (resource.resourceStatusInSolution == ResourceStatusInSolution.PendingToRemove && resource.resourceid != Guid.Empty)
                        {
                            try
                            {
                                solution.RemoveResourceToSolution(solutionSelected, resource);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }

                }).ContinueWith(resp => {

                });

                ActionsOfLoading(Visibility.Hidden);
                ActionsOfButtons(VisibilityType.Visible);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot link or unlink resource to solution : '{0}'", ex.Message));
            }
            return finish;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Close();
        }

        private void AvaiableResourcesFilter_Click(object sender, RoutedEventArgs e)
        {
            if (AvaiableResourcesTypeFilter.SelectedItem != null && AvaiableResourcesTypeFilter.SelectedIndex > 0)
            {
                AvaiableResourcesTypeSelected = Convert.ToInt32(((ComboBoxItem)AvaiableResourcesTypeFilter.SelectedItem).Tag);
            }
            else
            {
                AvaiableResourcesTypeSelected = 0;
            }

            if (AvaiableResourcesNameFilter.Text != CRMNameFilterResourceDefaultText)
            {
                AvaiableResourcesTextSelected = AvaiableResourcesNameFilter.Text;
            }
            else
            {
                AvaiableResourcesTextSelected = "";
            }

            List<ResourceModel> lstTemp = lstAvaiableResources;

            if (AvaiableResourcesTypeSelected > 0)
            {
                lstTemp = lstTemp.Where(k => k.webresourcetype == AvaiableResourcesTypeSelected).ToList();
            }

            if (!string.IsNullOrEmpty(AvaiableResourcesTextSelected))
            {
                lstTemp = lstTemp.Where(k => k.name.Contains(AvaiableResourcesTextSelected)).ToList();
            }

            AvailableResourcesInDefaultSolution.ItemsSource = null;
            AvailableResourcesInDefaultSolution.ItemsSource = lstTemp.OrderBy(k => k.name);
        }

        private void ResourcesInSolutionFilter_Click(object sender, RoutedEventArgs e)
        {
            string ResourcesInSolutionTextSelected = "";

            if (ResourcesInSolutionTypeFilter.SelectedItem != null && ResourcesInSolutionTypeFilter.SelectedIndex > 0)
            {
                ResourcesInSolutionTypeSelected = Convert.ToInt32(((ComboBoxItem)ResourcesInSolutionTypeFilter.SelectedItem).Tag);
            }
            else
            {
                ResourcesInSolutionTypeSelected = 0;
            }

            if (ResourcesInSolutionTextFilter.Text != CRMNameFilterResourceDefaultText)
            {
                ResourcesInSolutionTextSelected = ResourcesInSolutionTextFilter.Text;
            }
            else
            {
                ResourcesInSolutionTextSelected = "";
            }

            List<ResourceModel> lstTemp = lstResourcesInSolution;
            if (ResourcesInSolutionTypeSelected > 0)
            {
                lstTemp = lstTemp.Where(k => k.webresourcetype == ResourcesInSolutionTypeSelected).ToList();
            }

            if (!String.IsNullOrEmpty(ResourcesInSolutionTextSelected))
            {
                lstTemp = lstTemp.Where(k => k.name.Contains(ResourcesInSolutionTextSelected)).ToList();
            }

            ResourcesInSolution.ItemsSource = null;
            ResourcesInSolution.ItemsSource = lstTemp;
        }

        #region Select All, Unselect, Invert and moving resource from solutions
        private void AddResourceToSolution_Click(object sender, RoutedEventArgs e)
        {
            List<Guid> resourcesRemove = new List<Guid>();
            foreach (ResourceModel resource in AvailableResourcesInDefaultSolution.Items)
            {
                if (resource.selectResource)
                {
                    resource.resourceStatusInSolution = ResourceStatusInSolution.PendingToAdd;
                    resource.selectResource = false;
                    resourcesRemove.Add(resource.resourceid);                 

                    lstResourcesInSolution.Add(resource);
                }
            }

            lstAvaiableResources.RemoveAll(k => resourcesRemove.Contains(k.resourceid));

            ResourcesInSolution.ItemsSource = null;
            ResourcesInSolution.ItemsSource = lstResourcesInSolution.OrderBy(k => k.name);
            ResourcesInSolutionTextFilter.Text = "";
            ResourcesInSolutionTypeFilter.SelectedIndex = 0;
           
            AvailableResourcesInDefaultSolution.ItemsSource = null;
            AvailableResourcesInDefaultSolution.ItemsSource = lstAvaiableResources.OrderBy(k => k.name);
        }

        private void RemoveResourceFromSolution_Click(object sender, RoutedEventArgs e)
        {
            List<Guid> resourcesRemove = new List<Guid>();
            foreach (ResourceModel resource in ResourcesInSolution.Items)
            {
                if (resource.selectResource)
                {
                    resource.resourceStatusInSolution = ResourceStatusInSolution.PendingToRemove;
                    resource.selectResource = false;
                    resourcesRemove.Add(resource.resourceid);

                    lstAvaiableResources.Add(resource);
                }
            }

            lstResourcesInSolution.RemoveAll(k => resourcesRemove.Contains(k.resourceid));

            ResourcesInSolution.ItemsSource = null;
            ResourcesInSolution.ItemsSource = lstResourcesInSolution.OrderBy(k => k.name);

            AvailableResourcesInDefaultSolution.ItemsSource = null;
            AvailableResourcesInDefaultSolution.ItemsSource = lstAvaiableResources.OrderBy(k => k.name);
            AvaiableResourcesNameFilter.Text = "";
            AvaiableResourcesTypeFilter.SelectedIndex = 0;
        }

        private void ResourcesInSolutionSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in ResourcesInSolution.Items)
            {
                resource.selectResource = true;
            }
            ResourcesInSolution.ItemsSource = null;
            ResourcesInSolution.ItemsSource = lstResourcesInSolution;
        }

        private void ResourcesInSolutionUnselect_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in ResourcesInSolution.Items)
            {
                resource.selectResource = false;
            }
            ResourcesInSolution.ItemsSource = null;
            ResourcesInSolution.ItemsSource = lstResourcesInSolution;
        }

        private void ResourcesInSolutionInvertSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in ResourcesInSolution.Items)
            {
                resource.selectResource = !resource.selectResource;
            }
            ResourcesInSolution.ItemsSource = null;
            ResourcesInSolution.ItemsSource = lstResourcesInSolution;
        }

        private void ResourcesInSolution_SelectionChanged(object sender, MouseButtonEventArgs e)
        {         
            if (((ListViewItem)sender).DataContext is ResourceModel)
            {
                if (!((ListViewItem)sender).IsSelected)
                {
                    ((ResourceModel)((ListViewItem)sender).DataContext).selectResource = true;
                }
                else
                {
                    ((ResourceModel)((ListViewItem)sender).DataContext).selectResource = false;
                }
            }
        }

        private void AvaiableResourcesSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in AvailableResourcesInDefaultSolution.Items)
            {
                resource.selectResource = true;
            }
            AvailableResourcesInDefaultSolution.ItemsSource = null;
            AvailableResourcesInDefaultSolution.ItemsSource = lstAvaiableResources;
        }

        private void AvaiableResourcesUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in AvailableResourcesInDefaultSolution.Items)
            {
                resource.selectResource = false;
            }
            AvailableResourcesInDefaultSolution.ItemsSource = null;
            AvailableResourcesInDefaultSolution.ItemsSource = lstAvaiableResources;
        }

        private void AvaiableResourcesInvertSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in AvailableResourcesInDefaultSolution.Items)
            {
                resource.selectResource = !resource.selectResource;
            }
            AvailableResourcesInDefaultSolution.ItemsSource = null;
            AvailableResourcesInDefaultSolution.ItemsSource = lstAvaiableResources;
        }

        private void AvailableResourcesInDefaultSolution_SelectionChanged(object sender, MouseButtonEventArgs e)
        {           
            if (((ListViewItem)sender).DataContext is ResourceModel)
            {
                if (!((ListViewItem)sender).IsSelected)
                {
                    ((ResourceModel)((ListViewItem)sender).DataContext).selectResource = true;
                }
                else
                {
                    ((ResourceModel)((ListViewItem)sender).DataContext).selectResource = false;
                }
            }
        }
        #endregion
    }
}