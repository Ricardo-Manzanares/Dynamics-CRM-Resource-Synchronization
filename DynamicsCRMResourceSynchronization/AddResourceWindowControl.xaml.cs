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

            ResourcesInSolution.ItemsSource = resourcesToView;

            AvaiableResourcesNameFilter.Text = CRMNameFilterResourceDefaultText;
            ResourcesInSolutionNameFilter.Text = CRMNameFilterResourceDefaultText;
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

            List<ResourceModel> resourcesInDefaultSolution = new List<ResourceModel>();
            await Task.Run(() =>
            {
                SolutionsBusiness solution = new SolutionsBusiness(CRMClient, settings);
                solutionDefault = solution.GetSolutionDefault();
                if (solutionDefault != null && !string.IsNullOrEmpty(solutionDefault.solutionid))
                {                    
                    Guid solutionParse = Guid.Empty;
                    if (Guid.TryParse(solutionDefault.solutionid, out solutionParse))
                    {
                        resourcesInDefaultSolution = resourcesBusiness.GetResourcesFromSolution(solutionParse, true);

                        //Order resources by name and remove resources in solution
                        resourcesInDefaultSolution = resourcesInDefaultSolution.OrderBy(k => k.name).ToList();
                        if (resourcesInDefaultSolution.Count > 0)
                        {
                            resourcesInDefaultSolution.RemoveAll(k => ResourcesInSolution.ItemsSource.Cast<ResourceModel>().ToList().Select(s => s.name).Contains(k.name));
                            finish = true;
                        }
                    }
                }
            }).ContinueWith(resp => {

            });

            ActionsOfLoading(Visibility.Hidden);
            ActionsOfButtons(VisibilityType.Visible);

            if (finish && resourcesInDefaultSolution.Count() > 0)
            {
                AvailableResourcesInDefaultSolution.ItemsSource = resourcesInDefaultSolution;
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
                    ResourcesInSolutionNameFilter.IsEnabled = true;
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
                    ResourcesInSolutionNameFilter.IsEnabled = false;
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
            if(tb.Name == AvaiableResourcesNameFilter.Name)
            {
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
                        AvaiableResourcesNameFilter.Text = "";
                    }
                }
            }
            else
            {
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
                        ResourcesInSolutionNameFilter.Text = "";
                    }
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

        }

        private void ResourcesInSolutionFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddResourceToSolution_Click(object sender, RoutedEventArgs e)
        {
            List<ResourceModel> resourceToSolutionPendingAdd = ResourcesInSolution.ItemsSource.Cast<ResourceModel>().ToList();
            foreach (ResourceModel resource in AvailableResourcesInDefaultSolution.Items)
            {
                if (resource.selectResource)
                {
                    resource.resourceStatusInSolution = ResourceStatusInSolution.PendingToAdd;
                    resource.selectResource = false;
                    resourceToSolutionPendingAdd.Add(resource);
                }
            }

            ResourcesInSolution.ItemsSource = null;
            ResourcesInSolution.ItemsSource = resourceToSolutionPendingAdd.OrderBy(k => k.name);
            ResourcesInSolutionNameFilter.Text = "";
            ResourcesInSolutionTypeFilter.SelectedIndex = 0;
           
            var resourcesMoved = AvailableResourcesInDefaultSolution.Items.Cast<ResourceModel>().ToList();
            resourcesMoved.RemoveAll(k => k.resourceStatusInSolution == ResourceStatusInSolution.PendingToAdd);
            AvailableResourcesInDefaultSolution.ItemsSource = null;
            AvailableResourcesInDefaultSolution.ItemsSource = resourcesMoved.OrderBy(k => k.name);
        }

        private void RemoveResourceFromSolution_Click(object sender, RoutedEventArgs e)
        {
            List<ResourceModel> resourceToSolutionPendingAdd = AvailableResourcesInDefaultSolution.ItemsSource.Cast<ResourceModel>().ToList();
            foreach (ResourceModel resource in ResourcesInSolution.Items)
            {
                if (resource.selectResource)
                {
                    resource.resourceStatusInSolution = ResourceStatusInSolution.PendingToRemove;
                    resource.selectResource = false;
                    resourceToSolutionPendingAdd.Add(resource);
                }
            }

            var resourcesDeleted = ResourcesInSolution.Items.Cast<ResourceModel>().ToList();
            resourcesDeleted.RemoveAll(k => k.resourceStatusInSolution == ResourceStatusInSolution.PendingToRemove);

            ResourcesInSolution.ItemsSource = null;
            ResourcesInSolution.ItemsSource = resourcesDeleted.OrderBy(k => k.name);

            AvailableResourcesInDefaultSolution.ItemsSource = null;
            AvailableResourcesInDefaultSolution.ItemsSource = resourceToSolutionPendingAdd.OrderBy(k => k.name);
            AvaiableResourcesNameFilter.Text = "";
            AvaiableResourcesTypeFilter.SelectedIndex = 0;
        }

        private void ResourcesInSolutionSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in ResourcesInSolution.Items)
            {
                resource.selectResource = true;
            }
        }

        private void ResourcesInSolutionUnselect_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in ResourcesInSolution.Items)
            {
                resource.selectResource = false;
            }
        }

        private void ResourcesInSolutionInvertSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in ResourcesInSolution.Items)
            {
                resource.selectResource = !resource.selectResource;
            }
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
        }

        private void AvaiableResourcesUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in AvailableResourcesInDefaultSolution.Items)
            {
                resource.selectResource = false;
            }
        }

        private void AvaiableResourcesInvertSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourceModel resource in AvailableResourcesInDefaultSolution.Items)
            {
                resource.selectResource = !resource.selectResource;
            }
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
    }
}