using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CRMResourceSynchronization.Controls
{
    public class CRMResourceSynchronizationDataGrid : DataGrid
    {
        protected override void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                base.OnAutoGeneratingColumn(e);
                var propertyDescriptor = e.PropertyDescriptor as PropertyDescriptor;
                e.Column.Header = propertyDescriptor.Description;
                
                e.Column.Visibility = propertyDescriptor.IsBrowsable ? Visibility.Visible : Visibility.Collapsed;
                if(e.PropertyType.FullName != "System.Boolean")
                {
                    e.Column.IsReadOnly = true;
                }
                else
                {
                    e.Column.IsReadOnly = false;
                }
            }
            catch
            {
                // ignore; retain field defaults
            }
        }
    }
}
