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
        private ScrollViewer resourceCRMScroll, resourceLocalScroll, resourceCombinedScroll = new ScrollViewer();
        private string colorFocusBorderRowSelected = "#70C0E7";

       

    /// <summary>
    /// Initializes a new instance of the <see cref="DifferencesResourceWindowControlControl"/> class.
    /// </summary>
    public DifferencesResourceWindowControlControl(ResourceModel resourceToView)
        {
            this.InitializeComponent();

            if(resourceToView != null)
                resource = resourceToView;

            resource.resourceCompareStatus = SideBySideDiffBuilder.Diff(resource.contentCRM, resource.contentLocal);

            ResourceCRM.Items.Clear();
            ResourceCRM.ItemsSource = resource.resourceCompareStatus.OldText.Lines;
            ResourceLocal.Items.Clear();
            ResourceLocal.ItemsSource = resource.resourceCompareStatus.NewText.Lines;

            ResourceCombined.Items.Clear();
            ResourceCombined.ItemsSource = InlineDiffBuilder.Diff(resource.contentCRM, resource.contentLocal).Lines;

            int rowsMax = resource.resourceCompareStatus.OldText.Lines.Count > resource.resourceCompareStatus.NewText.Lines.Count ? resource.resourceCompareStatus.OldText.Lines.Count : resource.resourceCompareStatus.NewText.Lines.Count;

            //Size of differences in rows from resource CRM vs Local
            Modified.Text += " (" + resource.resourceCompareStatus.OldText.Lines.Where(k => k.Type == ChangeType.Modified).Count() + "/" + rowsMax + ")";
            Unchanged.Text += " (" + resource.resourceCompareStatus.OldText.Lines.Where(k => k.Type == ChangeType.Unchanged).Count() + "/" + rowsMax + ")";
            Inserted.Text += " (" + resource.resourceCompareStatus.NewText.Lines.Where(k => k.Type == ChangeType.Inserted).Count() + "/" + rowsMax + ")";
            Deleted.Text += " (" + resource.resourceCompareStatus.OldText.Lines.Where(k => k.Type == ChangeType.Deleted).Count() + "/" + rowsMax + ")";

            //Set info of resource CRM and Local
            InfoResource.Text += resourceToView.name;
            InfoCRMDate.Text = resourceToView.modifiedon;
            InfoLocalDate.Text = resourceToView.localmodifiedon;
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
            resourceCombinedScroll.ScrollToHorizontalOffset(resourceLocalScroll.HorizontalOffset);
        }

        private void ResourceLocal_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            resourceCRMScroll.ScrollToVerticalOffset(resourceLocalScroll.VerticalOffset);
            resourceCRMScroll.ScrollToHorizontalOffset(resourceLocalScroll.HorizontalOffset);
            resourceCombinedScroll.ScrollToHorizontalOffset(resourceLocalScroll.HorizontalOffset);
        }
        private void ResourceCombined_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            resourceCRMScroll.ScrollToVerticalOffset(resourceLocalScroll.VerticalOffset);
            resourceCRMScroll.ScrollToHorizontalOffset(resourceLocalScroll.HorizontalOffset);
            resourceCombinedScroll.ScrollToHorizontalOffset(resourceLocalScroll.HorizontalOffset);
        }
        private void ListViewItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid item = (Grid)sender;
            if (item != null)
            {
                DiffPiece r = (DiffPiece)((Grid)sender).DataContext;
                if (r.Position == null)
                    return;

                if (ResourceCRM.Items.Count > 0 && (r.Position.Value - 1) <= (ResourceCRM.Items.Count - 1))
                {
                    if (ResourceCRM.Items[r.Position.Value - 1] != null)
                    {
                        ListViewItem LVIRsourceCRM = (ListViewItem)ResourceCRM.ItemContainerGenerator.ContainerFromItem(ResourceCRM.Items[r.Position.Value - 1]);
                        LVIRsourceCRM.BorderBrush = (Brush)(new BrushConverter().ConvertFrom(colorFocusBorderRowSelected));
                    }
                }
                if (ResourceLocal.Items.Count > 0 && (r.Position.Value - 1) <= (ResourceLocal.Items.Count - 1))
                {
                    if (ResourceLocal.Items[r.Position.Value - 1] != null)
                    {
                        ListViewItem LVIRsourceLocal = (ListViewItem)ResourceLocal.ItemContainerGenerator.ContainerFromItem(ResourceLocal.Items[r.Position.Value - 1]);
                        LVIRsourceLocal.BorderBrush = (Brush)(new BrushConverter().ConvertFrom(colorFocusBorderRowSelected));
                    }
                }
            }
        }

        private void ListViewItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid item = (Grid)sender;
            if (item != null)
            {
                DiffPiece r = (DiffPiece)((Grid)sender).DataContext;
                if (r.Position == null)
                    return;

                if (ResourceCRM.Items.Count > 0 && ((r.Position.Value - 1)) <= (ResourceCRM.Items.Count - 1))
                {
                    if (ResourceCRM.Items[r.Position.Value - 1] != null)
                    {
                        ListViewItem LVIRsourceCRM = (ListViewItem)ResourceCRM.ItemContainerGenerator.ContainerFromItem(ResourceCRM.Items[r.Position.Value - 1]);
                        LVIRsourceCRM.BorderBrush = Brushes.White;
                    }
                }
                if (ResourceLocal.Items.Count > 0 && (r.Position.Value - 1) <= (ResourceLocal.Items.Count - 1))
                {
                    if (ResourceLocal.Items[r.Position.Value - 1] != null)
                    {
                        ListViewItem LVIRsourceLocal = (ListViewItem)ResourceLocal.ItemContainerGenerator.ContainerFromItem(ResourceLocal.Items[r.Position.Value - 1]);
                        LVIRsourceLocal.BorderBrush = Brushes.White;
                    }
                }
            }
        }

        #endregion
    }
}