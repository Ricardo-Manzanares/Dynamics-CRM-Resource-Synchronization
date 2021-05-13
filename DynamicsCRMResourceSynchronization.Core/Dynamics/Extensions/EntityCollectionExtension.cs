using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DynamicsCRMResourceSynchronization.Core.Dynamics.Extensions
{
    public static class EntityCollectionExtension
    {
        public static string ToDateTimeFormatUS(this DateTime date)
        {
            // DBF : Las peticones via POST para los retrive 
            //       devuelven un 404 por la hora.
            //result = string.Format("{0:MM/dd/yyyy HH:mm:ss}", date);
            return string.Format("{0:MM/dd/yyyy}", date);
        }

        public static string ToDateTimeFormatUS_Time(this DateTime date)
        {
            // DBF : Las peticones via POST para los retrive 
            //       devuelven un 404 por la hora.
            //result = string.Format("{0:MM/dd/yyyy HH:mm:ss}", date);
            return string.Format("{0:MM/dd/yyyy  HH:mm:ss}", date);
        }

        public static string ClearPagingCookie(this string pagingCookie)
        {
            return pagingCookie
                            .Replace("<", "&lt;")
                            .Replace("\"", "&quot;")
                            .Replace(">", "&gt;");
        }

        public static bool ExistProperty(Entity ObjectProperties, string propertyKey)
        {
            return ((AttributeCollection)ObjectProperties.Attributes).ContainsKey(propertyKey);
        }
    }
}
