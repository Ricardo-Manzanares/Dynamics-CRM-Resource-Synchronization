using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsCRMResourceSynchronization.Extensions
{
    public static class ResourcePath
    {
        /// <summary>
        /// Get local path of the resource located in the Resources folder
        /// </summary>
        /// <param name="NameResource"></param>
        /// <returns></returns>
        public static string GetLocalPath(string NameResource)
        {
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var iconPath = Path.Combine(outPutDirectory, "Resources\\"+ NameResource);
            return new Uri(iconPath).LocalPath;
        }
    }
}
