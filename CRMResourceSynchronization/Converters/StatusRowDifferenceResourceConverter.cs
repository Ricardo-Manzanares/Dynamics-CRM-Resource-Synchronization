using CRMResourceSynchronization.Core.Business.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CRMResourceSynchronization.Converters
{
    public class StatusRowDifferenceResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "Equal":
                    return "#F0F0F0";
                    break;
                case "LocalResourceMissing":
                    return "#FF9999";
                    break;
                case "CRMResourceMissing":
                    return "#FF9999";
                    break;
                case "DifferencesExist":
                    return "#FFFF00";
                    break;
                case "DifferencesResolved":
                    return "#00E400";
                    break;
                default:
                    return "#FF9999";
                    break;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
