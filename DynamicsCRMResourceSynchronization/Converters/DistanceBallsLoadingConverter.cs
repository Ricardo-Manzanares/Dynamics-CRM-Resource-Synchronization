using DynamicsCRMResourceSynchronization.Core.Business;
using DynamicsCRMResourceSynchronization.Core.Business.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DynamicsCRMResourceSynchronization.Converters
{
    public class DistanceBallsLoadingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            int porcent = 0;
            switch (int.Parse(parameter.ToString().Split('|')[0]))
            {
                case 1:
                    porcent = int.Parse(parameter.ToString().Split('|')[1])  + 47;
                    break;
                case 2:
                    porcent = int.Parse(parameter.ToString().Split('|')[1]) + 48;
                    break;
                case 3:
                    porcent = int.Parse(parameter.ToString().Split('|')[1]) + 49;
                    break;
                case 4:
                    porcent = int.Parse(parameter.ToString().Split('|')[1]) + 50;
                    break;
                case 5:
                    porcent = int.Parse(parameter.ToString().Split('|')[1]) + 51;
                    break;
                default:
                    break;
            }

            return (porcent * int.Parse(value.ToString())) / 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
