using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CRMResourceSynchronization.Converters
{
    public class StatusRowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            string myValue = value.ToString();
            string output;

            switch (myValue)
            {
                case "Modified":
                    output = "#FF9999";
                    break;
                case "Unchanged":
                    output = "#FFFFFF";
                    break;
                case "Inserted":
                    output = "#D7E3BC";
                    break;
                case "Deleted":
                    output = "#FF0000";
                    break;
                default:
                    output = "#C0C0C0";
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
