using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CRMResourceSynchronization.Converters
{
    public class EnabledMergeRowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            string myValue = value.ToString();
            bool output;

            switch (myValue)
            {
                case "Merged":
                    output = true;
                    break;
                case "UnMerged":
                    output = false;
                    break;
                default:
                    output = true;
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
