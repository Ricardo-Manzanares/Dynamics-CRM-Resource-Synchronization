using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DynamicsCRMResourceSynchronization.Converters
{
    public class SelectedMergeRowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            string myValue = value.ToString();
            Visibility output;

            switch (myValue)
            {
                case "Modified":
                    output = Visibility.Visible;
                    break;
                case "Unchanged":
                    output = Visibility.Hidden;
                    break;
                case "Inserted":
                    output = Visibility.Visible;
                    break;
                case "Deleted":
                    output = Visibility.Visible;
                    break;
                case "Merged":
                    output = Visibility.Visible;
                    break;
                case "UnMerged":
                    output = Visibility.Visible;
                    break;
                case "Conflict":
                    output = Visibility.Visible;
                    break;
                default:
                    output = Visibility.Hidden;
                    break;
            }

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
