using DynamicsCRMResourceSynchronization.Core.Business;
using DynamicsCRMResourceSynchronization.Core.Business.Models;
using DynamicsCRMResourceSynchronization.Core.DiffPlex.DiffBuilder;
using DynamicsCRMResourceSynchronization.Core.DiffPlex.DiffBuilder.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DynamicsCRMResourceSynchronization
{
    /// <summary>
    /// Interaction logic for DifferencesResourceWindowControl.
    /// </summary>
    public partial class DifferencesResourceWindowControl : UserControl
    {
        private ResourceModel resource { get; set; }
        private List<ResourceModel> resources { get; set; }
        private ResourcesBusiness resourcesBusiness { get; set; }

        private int posResource = 0;
        private int conflictsInResource = 0;

        private ScrollViewer resourceCRMScroll, resourceLocalScroll, resourceCombinedScroll = new ScrollViewer();
        private string InfoResourceDefaultText = "Differences in resource ";
        private DiffPaneModel DiffPaneResourceCombined = new DiffPaneModel();
        private SettingsModel settings = new SettingsModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="DifferencesResourceWindowControl"/> class.
        /// </summary>
        public DifferencesResourceWindowControl(SettingsModel settings, ResourcesBusiness resourceBusiness, List<ResourceModel> resourcesToView)
        {
            this.InitializeComponent();

            if (resourcesToView is null || resourceBusiness is null || settings is null)
                return;

            this.resourcesBusiness = resourceBusiness;
            this.settings = settings;

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
            Modified.Text = "Modified ";
            Unchanged.Text = "Unchanged ";
            Inserted.Text = "Inserted ";
            Deleted.Text = "Deleted ";
            

            //Set info of resource CRM and Local
            InfoResource.Text = InfoResourceDefaultText;
            InfoResourcesSize.Text = (posResource + 1) + " of " + resources.Count;
            resource = resources[posResource];

            //Get last content of resource local
            resourcesBusiness.GetResourceLocal(resource);

            if (!string.IsNullOrEmpty(resource.contentCRM) && !string.IsNullOrEmpty(resource.contentLocal))
            {
                resource.resourceCompareStatus = SideBySideDiffBuilder.Diff(resource.contentCRM, resource.contentLocal);
                resource.resourceDifference = (resource.resourceCompareStatus.NewText.HasDifferences || resource.resourceCompareStatus.OldText.HasDifferences) ? ResourceContentStatus.DifferencesExist : ResourceContentStatus.DifferencesResolved;

                ResourceCRM.ItemsSource = resource.resourceCompareStatus.OldText.Lines;
                ResourceLocal.ItemsSource = resource.resourceCompareStatus.NewText.Lines;
                DiffPaneResourceCombined = InlineDiffBuilder.Diff(resource.contentCRM, resource.contentLocal);

                calculateConflict(true);

                ResourceCombined.ItemsSource = DiffPaneResourceCombined.Lines;

                int rowsMax = resource.resourceCompareStatus.OldText.Lines.Count > resource.resourceCompareStatus.NewText.Lines.Count ? resource.resourceCompareStatus.OldText.Lines.Count : resource.resourceCompareStatus.NewText.Lines.Count;

                //Size of differences in rows from resource CRM vs Local
                Modified.Text += "(" + resource.resourceCompareStatus.OldText.Lines.Where(k => k.Type == ChangeType.Modified).Count() + "/" + rowsMax + ")";
                Unchanged.Text += "(" + resource.resourceCompareStatus.OldText.Lines.Where(k => k.Type == ChangeType.Unchanged).Count() + "/" + rowsMax + ")";
                Inserted.Text += "(" + resource.resourceCompareStatus.NewText.Lines.Where(k => k.Type == ChangeType.Inserted).Count() + "/" + rowsMax + ")";
                Deleted.Text += "(" + resource.resourceCompareStatus.OldText.Lines.Where(k => k.Type == ChangeType.Deleted).Count() + "/" + rowsMax + ")";

                //Set info of resource CRM and Local
                InfoResource.Text += resource.name;
                InfoCRMDate.Text = resource.modifiedon;
                InfoLocalDate.Text = resource.localmodifiedon;
            }
        }

        #region Events Scroll
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
            if (!ResourcePendingLinesMerge())
            {
                if (posResource < (resources.Count - 1))
                    posResource += 1;

                loadResource();
            }
            else
            {
                SaveResource();
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (!ResourcePendingLinesMerge())
            {
                if (posResource > 0)
                    posResource -= 1;

                loadResource();
            }
            else
            {
                SaveResource();
            }
        }

        private bool ResourcePendingLinesMerge()
        {
            bool skipResource = false;
            if(DiffPaneResourceCombined.Lines.Where(k => k.Type == ChangeType.Merged).Count() > 0)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("The current resource has differences between different origins and you have marked lines to merge, if you change the resource you will lose the marked lines. You want to update the local resource with the lines marked to merge before changing the resource?", "Update resource local", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    skipResource = true;
                }
            }

            return skipResource;
        }

        private void MergeLine_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).DataContext is DiffPiece)
            {
                DiffPiece line = ((DiffPiece)((CheckBox)sender).DataContext);
                DiffPiece lineConflict = DiffPaneResourceCombined.Lines.Where(k => k.Position == line.Position.Value && k.Text != line.Text).FirstOrDefault();

                if (((CheckBox)sender).IsChecked.Value)
                {
                    line.TypePrevious = line.Type;
                    line.Type = ChangeType.Merged;                   
                    if (lineConflict != null)
                    {
                        lineConflict.TypePrevious = lineConflict.Type;
                        lineConflict.Type = ChangeType.UnMerged;
                    }
                }
                else
                {
                    line.Type = line.TypePrevious;
                    if (lineConflict != null)
                    {
                        lineConflict.Type = lineConflict.TypePrevious;
                    }
                }

                ResourceCombined.ItemsSource = null;
                ResourceCombined.ItemsSource = DiffPaneResourceCombined.Lines;

                calculateConflict();
            }
        }

        private void calculateConflict(bool initCalculated = false)
        {
            PendingConflict.Text = "Pending conflict ";
            SolvedConflict.Text = "Solved conflict ";

            //Detect conflict automatic merge
            List<DiffPiece> linesConflict = DiffPaneResourceCombined.Lines.Where(k => k.Type != ChangeType.Unchanged).ToList();
            if (initCalculated)
            {
                foreach (DiffPiece line in linesConflict)
                {
                    if (DiffPaneResourceCombined.Lines.Where(k => k.Position == line.Position).Count() == 1)
                    {
                        line.TypePrevious = line.Type;
                        line.Type = ChangeType.Merged;

                    }
                    else
                    {
                        line.TypePrevious = line.Type;
                        line.Type = ChangeType.Conflict;
                    }
                }

                conflictsInResource = linesConflict.Count() - (DiffPaneResourceCombined.Lines.Where(k => k.Type == ChangeType.Conflict).Count() / 2);
            }

            SolvedConflict.Text += "(" + DiffPaneResourceCombined.Lines.Where(k => k.Type == ChangeType.Merged).Count() + "/" + conflictsInResource + ")";
            PendingConflict.Text += "(" + (DiffPaneResourceCombined.Lines.Where(k => k.Type == ChangeType.Conflict).Count() / 2) + "/" + conflictsInResource + ")";
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveResource();
        }

        private void SaveResource()
        {
            try
            {
                PathAndNameResourceModel resourceModel = Utils.getFormatPathAndNameResource(this.settings, resource.name, resource.webresourcetype);

                if (!Directory.Exists(resourceModel.path) && Utils.DirectoryHasPermission(string.Join("\\", resourceModel.path.Split('\\').Take(resourceModel.path.Split('\\').Length - 2)), FileSystemRights.Write))
                    Directory.CreateDirectory(resourceModel.path);

                if (File.Exists(resourceModel.path + resourceModel.name) && Utils.DirectoryHasPermission(resourceModel.path, FileSystemRights.Delete))
                {
                    //Backup of resource ¿?

                    File.Delete(resourceModel.path + resourceModel.name);
                }

                if (Utils.DirectoryHasPermission(resourceModel.path, FileSystemRights.Write))
                {
                    string contentMerged = String.Join("\r\n", DiffPaneResourceCombined.Lines.Where(k => k.Type == ChangeType.Merged || k.Type == ChangeType.Unchanged).Select(s => s.Text));

                    File.WriteAllText(resourceModel.path + resourceModel.name, contentMerged);

                    MessageBox.Show(string.Format("The resource has been updated successfully"));

                    //Update content resource in CRM and in local
                    ResourceModel resourceUpdate = resources.Where(k => k.resourceid == resource.resourceid).FirstOrDefault();
                    resourceUpdate.contentCRM = contentMerged;
                    resourceUpdate.contentLocal = contentMerged;

                    //Reload resources and differentes
                    loadResource();
                }
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
    }
}