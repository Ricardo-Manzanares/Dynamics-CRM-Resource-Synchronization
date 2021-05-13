using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DynamicsCRMResourceSynchronization.Converters
{
    public class NameTypeResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            string myValue = value.ToString();
            string output;

            switch (myValue)
            {
                case "1":
                    output = ".htm, .html";
                    break;
                case "2":
                    output = ".css";
                    break;
                case "3":
                    output = ".js";
                    break;
                case "4":
                    output = ".xml";
                    break;
                case "5":
                    output = ".png";
                    break;
                case "6":
                    output = ".jpg";
                    break;
                case "7":
                    output = ".gif";
                    break;
                case "8":
                    output = ".xap";
                    break;
                case "9":
                    output = ".xsl, xslt";
                    break;
                case "10":
                    output = ".ico";
                    break;
                case "11":
                    output = ".svg";
                    break;
                case "12":
                    output = ".resx";
                    break;
                default:
                    output = "";
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
