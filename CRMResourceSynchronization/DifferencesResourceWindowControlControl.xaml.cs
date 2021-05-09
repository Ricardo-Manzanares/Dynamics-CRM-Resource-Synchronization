using CRMResourceSynchronization.Core.Business;
using CRMResourceSynchronization.Core.Business.Models;
using CRMResourceSynchronization.Core.DiffPlex.DiffBuilder;
using CRMResourceSynchronization.Core.DiffPlex.DiffBuilder.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CRMResourceSynchronization
{
    /// <summary>
    /// Interaction logic for DifferencesResourceWindowControlControl.
    /// </summary>
    public partial class DifferencesResourceWindowControlControl : UserControl
    {
        private ResourceModel resource { get; set; }
        private List<ResourceModel> resources { get; set; }
        private ResourcesBusiness resourcesBusiness { get; set; }

        private int posResource = 0;
        private ScrollViewer resourceCRMScroll, resourceLocalScroll, resourceCombinedScroll = new ScrollViewer();
        public string InfoResourceDefaultText = "Differences in resource ";

        /// <summary>
        /// Initializes a new instance of the <see cref="DifferencesResourceWindowControlControl"/> class.
        /// </summary>
        public DifferencesResourceWindowControlControl(ResourcesBusiness resourceBusiness, List<ResourceModel> resourcesToView)
        {
            this.InitializeComponent();

            if (resourcesToView is null || resourceBusiness is null)
                return;

            this.resourcesBusiness = resourceBusiness;

            resources = resourcesToView;
            loadResource();
        }

        private void loadResource()
        {
            if (posResource < 0 && posResource > (resources.Count - 1))
                return;

            if (posResource == 0)
            {
                btnPrev.Opacity = 0.4;
                btnPrev.IsEnabled = false;
            }
            else if(posResource > 0)
            {
                btnPrev.Opacity = 1;
                btnPrev.IsEnabled = true;
            }

            if (posResource == (resources.Count - 1))
            {
                btnNext.Opacity = 0.4;
                btnNext.IsEnabled = false;
            }
            else if (posResource < (resources.Count - 1))
            {
                btnNext.Opacity = 1;
                btnNext.IsEnabled = true;
            }

            resource = null;
            ResourceCRM.ItemsSource = null;
            ResourceLocal.ItemsSource = null;
            ResourceCombined.ItemsSource = null;
            Modified.Text = "";
            Unchanged.Text = "";
            Inserted.Text = "";
            Deleted.Text = "";

            //Set info of resource CRM and Local
            InfoResource.Text = InfoResourceDefaultText;
            InfoResourcesSize.Text = (posResource + 1) + " of " + resources.Count;
            resource = resources[posResource];

            //Get last content of resource local
            resourcesBusiness.GetResourceLocal(resource);

            if (!string.IsNullOrEmpty(resource.contentCRM) && !string.IsNullOrEmpty(resource.contentLocal))
            {
                resource.resourceCompareStatus = SideBySideDiffBuilder.Diff(resource.contentCRM, resource.contentLocal);                

                ResourceCRM.ItemsSource = resource.resourceCompareStatus.OldText.Lines;
                ResourceLocal.ItemsSource = resource.resourceCompareStatus.NewText.Lines;
                ResourceCombined.ItemsSource = InlineDiffBuilder.Diff(resource.contentCRM, resource.contentLocal).Lines;

                int rowsMax = resource.resourceCompareStatus.OldText.Lines.Count > resource.resourceCompareStatus.NewText.Lines.Count ? resource.resourceCompareStatus.OldText.Lines.Count : resource.resourceCompareStatus.NewText.Lines.Count;

                //Size of differences in rows from resource CRM vs Local
                Modified.Text += " (" + resource.resourceCompareStatus.OldText.Lines.Where(k => k.Type == ChangeType.Modified).Count() + "/" + rowsMax + ")";
                Unchanged.Text += " (" + resource.resourceCompareStatus.OldText.Lines.Where(k => k.Type == ChangeType.Unchanged).Count() + "/" + rowsMax + ")";
                Inserted.Text += " (" + resource.resourceCompareStatus.NewText.Lines.Where(k => k.Type == ChangeType.Inserted).Count() + "/" + rowsMax + ")";
                Deleted.Text += " (" + resource.resourceCompareStatus.OldText.Lines.Where(k => k.Type == ChangeType.Deleted).Count() + "/" + rowsMax + ")";

                //Set info of resource CRM and Local
                InfoResource.Text += resource.name;
                InfoCRMDate.Text = resource.modifiedon;
                InfoLocalDate.Text = resource.localmodifiedon;
            }
        }

        #region Events Scroll and Event mouse
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            resourceCRMScroll = (ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild((ListView)ResourceCRM, 0), 0);
            resourceLocalScroll = (ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild((ListView)ResourceLocal, 0), 0);
            resourceCombinedScroll = (ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild((ListView)ResourceCombined, 0), 0);

            resourceCRMScroll.ScrollChanged += new ScrollChangedEventHandler(ResourceCRM_ScrollChanged);
            resourceLocalScroll.ScrollChanged += new ScrollChangedEventHandler(ResourceLocal_ScrollChanged);
            resourceCombinedScroll.ScrollChanged += new ScrollChangedEventHandler(ResourceCombined_ScrollChanged);
        }

        private void ResourceCRM_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            resourceLocalScroll.ScrollToVerticalOffset(resourceCRMScroll.VerticalOffset);
            resourceLocalScroll.ScrollToHorizontalOffset(resourceCRMScroll.HorizontalOffset);
            resourceCombinedScroll.ScrollToHorizontalOffset(resourceCRMScroll.HorizontalOffset);
            resourceCombinedScroll.ScrollToVerticalOffset(resourceCRMScroll.VerticalOffset);
        }

        private void ResourceLocal_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            resourceCRMScroll.ScrollToVerticalOffset(resourceLocalScroll.VerticalOffset);
            resourceCRMScroll.ScrollToHorizontalOffset(resourceLocalScroll.HorizontalOffset);
            resourceCombinedScroll.ScrollToHorizontalOffset(resourceLocalScroll.HorizontalOffset);
            resourceCombinedScroll.ScrollToVerticalOffset(resourceLocalScroll.VerticalOffset);
        }
        private void ResourceCombined_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            resourceCRMScroll.ScrollToVerticalOffset(resourceCombinedScroll.VerticalOffset);
            resourceCRMScroll.ScrollToHorizontalOffset(resourceCombinedScroll.HorizontalOffset);
            resourceLocalScroll.ScrollToVerticalOffset(resourceCombinedScroll.VerticalOffset);
            resourceLocalScroll.ScrollToHorizontalOffset(resourceCombinedScroll.HorizontalOffset);
        }
        #endregion

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if(posResource < (resources.Count - 1))
                posResource += 1;

            loadResource();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if(posResource > 0)
                posResource -= 1;

            loadResource();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Close();
        }
    }
}