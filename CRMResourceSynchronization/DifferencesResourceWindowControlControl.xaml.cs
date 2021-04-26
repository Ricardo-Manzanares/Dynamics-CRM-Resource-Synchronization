using CRMResourceSynchronization.Core.Business.Models;
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
        private ScrollViewer resourceCRMScroll, resourceLocalScroll = new ScrollViewer();
        private string colorFocusRowSelected = "#E5F3FB";
        private string colorFocusBorderRowSelected = "#70C0E7";

        /// <summary>
        /// Initializes a new instance of the <see cref="DifferencesResourceWindowControlControl"/> class.
        /// </summary>
        public DifferencesResourceWindowControlControl(ResourceModel resourceToDifferences)
        {
            this.InitializeComponent();

            if(resourceToDifferences != null)
                resource = resourceToDifferences;

            //List<ResourceContentModel> rowsResourceCRM = new List<ResourceContentModel>();
            //for (int i = 0; i < 50; i++)
            //{
            //    ResourceContentModel resourceContent = new ResourceContentModel();
            //    resourceContent.numRow = i + 1;
            //    resourceContent.textRow = "Row - " + (i + 1);
            //    resourceContent.statusRow = (i % 2 == 0) ? ResourceContentRowStatus.Difference : ResourceContentRowStatus.Delete;
            //    rowsResourceCRM.Add(resourceContent);
            //}

            //resource.contentRowsCRM = rowsResourceCRM;

            //List<ResourceContentModel> rowsResourceLocal = new List<ResourceContentModel>();
            //for (int i = 0; i < 50; i++)
            //{
            //    ResourceContentModel resourceContent = new ResourceContentModel();
            //    resourceContent.numRow = i + 1;
            //    resourceContent.textRow = "Row - " + (i + 1);
            //    resourceContent.statusRow = (i % 2 == 0) ? ResourceContentRowStatus.Difference : ResourceContentRowStatus.Delete;
            //    rowsResourceLocal.Add(resourceContent);
            //}

            //resource.contentRowsLocal = rowsResourceLocal;

            ResourceCRM.Items.Clear();
            ResourceCRM.ItemsSource = resource.contentRowsCRM;
            ResourceLocal.Items.Clear();
            ResourceLocal.ItemsSource = resource.contentRowsLocal;
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            resourceCRMScroll = (ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild((ListView)ResourceCRM, 0), 0);
            resourceLocalScroll = (ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild((ListView)ResourceLocal, 0), 0);

            resourceCRMScroll.ScrollChanged += new ScrollChangedEventHandler(ResourceCRM_ScrollChanged);
            resourceLocalScroll.ScrollChanged += new ScrollChangedEventHandler(ResourceLocal_ScrollChanged);
        }

        private void ResourceCRM_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            resourceLocalScroll.ScrollToVerticalOffset(resourceCRMScroll.VerticalOffset);
        }


        private void ResourceLocal_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            resourceCRMScroll.ScrollToVerticalOffset(resourceLocalScroll.VerticalOffset);
        }
        private void ListViewItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid item = (Grid)sender;
            if (item != null)
            {
                ResourceContentModel r = (ResourceContentModel)((Grid)sender).DataContext;
                
                if (ResourceCRM.Items.Count > 0 && (r.numRow - 1) <= (ResourceCRM.Items.Count - 1))
                {
                    if (ResourceCRM.Items[r.numRow - 1] != null)
                    {
                        ListViewItem LVIRsourceCRM = (ListViewItem)ResourceCRM.ItemContainerGenerator.ContainerFromItem(ResourceCRM.Items[r.numRow - 1]);
                        LVIRsourceCRM.Background = (Brush)(new BrushConverter().ConvertFrom(colorFocusRowSelected));
                        LVIRsourceCRM.BorderBrush = (Brush)(new BrushConverter().ConvertFrom(colorFocusBorderRowSelected));
                    }
                }
                if (ResourceLocal.Items.Count > 0 && (r.numRow - 1) <= (ResourceLocal.Items.Count - 1))
                {
                    if (ResourceLocal.Items[r.numRow - 1] != null)
                    {
                        ListViewItem LVIRsourceLocal = (ListViewItem)ResourceLocal.ItemContainerGenerator.ContainerFromItem(ResourceLocal.Items[r.numRow - 1]);
                        LVIRsourceLocal.Background = (Brush)(new BrushConverter().ConvertFrom(colorFocusRowSelected));
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
                ResourceContentModel r = (ResourceContentModel)((Grid)sender).DataContext;

                if (ResourceCRM.Items.Count > 0 && ((r.numRow - 1)) <= (ResourceCRM.Items.Count - 1))
                {
                    if (ResourceCRM.Items[r.numRow - 1] != null)
                    {
                        ListViewItem LVIRsourceCRM = (ListViewItem)ResourceCRM.ItemContainerGenerator.ContainerFromItem(ResourceCRM.Items[r.numRow - 1]);
                        LVIRsourceCRM.Background = Brushes.White;
                        LVIRsourceCRM.BorderBrush = Brushes.White;
                    }
                }
                if (ResourceLocal.Items.Count > 0 && (r.numRow - 1) <= (ResourceLocal.Items.Count - 1))
                {
                    if (ResourceLocal.Items[r.numRow - 1] != null)
                    {
                        ListViewItem LVIRsourceLocal = (ListViewItem)ResourceLocal.ItemContainerGenerator.ContainerFromItem(ResourceLocal.Items[r.numRow - 1]);
                        LVIRsourceLocal.Background = Brushes.White;
                        LVIRsourceLocal.BorderBrush = Brushes.White;
                    }
                }
            }
        }
    }
}