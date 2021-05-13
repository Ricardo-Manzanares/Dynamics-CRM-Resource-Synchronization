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
using System.Windows.Media;

namespace DynamicsCRMResourceSynchronization
{
    /// <summary>
    /// Interaction logic for AddResourceWindowControl.
    /// </summary>
    public partial class AddResourceWindowControl : UserControl
    {
        public CRMClient CRMClient { get; set; }
        private List<ResourceModel> resourcesInDefaultSolution { get; set; }
        private List<ResourceModel> resourcesInSolution { get; set; }
        private ResourcesBusiness resourcesBusiness { get; set; }

        private SettingsModel settings = new SettingsModel();

        private string CRMNameFilterResourceDefaultText = "Filter by resource name";
        private SolutionModel solutionDefault;

        /// <summary>
        /// Initializes a new instance of the <see cref="DifferencesResourceWindowControl"/> class.
        /// </summary>
        public AddResourceWindowControl(CRMClient CRMClient, SettingsModel settings, ResourcesBusiness resourceBusiness, List<ResourceModel> resourcesToView)
        {
            this.InitializeComponent();

            if (CRMClient is null || resourcesToView is null || resourceBusiness is null || settings is null)
                return;

            this.CRMClient = CRMClient;
            this.resourcesBusiness = resourceBusiness;
            this.settings = settings;

            resourcesInSolution = resourcesToView;

            AvaiableResourcesNameFilter.Text = CRMNameFilterResourceDefaultText;
            ResourcesInSolutionNameFilter.Text = CRMNameFilterResourceDefaultText;
            AvaiableResourcesTypeFilter.SelectedIndex = 0;
            ResourcesInSolutionTypeFilter.SelectedIndex = 0;

            ResourcesInSolution.ItemsSource = resourcesInSolution;

            CRMLoadResourcesFromSolutionDefaultAsync();
        }

        private async Task<bool> CRMLoadResourcesFromSolutionDefaultAsync()
        {
            bool finish = false;

            await Task.Run(() =>
            {
                SolutionsBusiness solution = new SolutionsBusiness(CRMClient, settings);
                solutionDefault = solution.GetSolutionDefault();
                if (solutionDefault != null && !string.IsNullOrEmpty(solutionDefault.solutionid))
                {                    
                    Guid solutionParse = Guid.Empty;
                    if (Guid.TryParse(solutionDefault.solutionid, out solutionParse))
                    {
                        resourcesInSolution = resourcesBusiness.GetResourcesFromSolution(solutionParse, true);
                        resourcesInSolution = resourcesInSolution.OrderBy(k => k.name).ToList();
                        if (resourcesInSolution.Count > 0)
                        {
                            finish = true;
                        }
                    }
                }
            }).ContinueWith(resp => {

            });

            if (finish && resourcesInSolution.Count() > 0)
            {
                AvailableResourcesInDefaultSolution.ItemsSource = resourcesInSolution;
            }          

            return finish;
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

        private void SaveResource()
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot save resource : '{0}'", ex.Message));
            }
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

        }

        private void RemoveResourceFromSolution_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AvaiableResourcesSelectAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResourcesInSolutionSelectAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AvaiableResourcesUnselectAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AvaiableResourcesInvertSelect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResourcesInSolutionUnselect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResourcesInSolutionInvertSelect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}