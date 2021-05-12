using CRMResourceSynchronization.Core.Business;
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
    public class NameDifferenceResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            return Utils.GetEnumDescriptionFromValue<ResourceContentStatus>(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
