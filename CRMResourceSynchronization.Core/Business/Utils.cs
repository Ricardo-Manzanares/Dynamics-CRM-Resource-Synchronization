using CRMResourceSynchronization.Core.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CRMResourceSynchronization.Core.Business
{
    public static class Utils
    {
        public static string GetDescriptionStatusFromEnumValue<T>(String value)
        {
            return GetEnumDescriptionFromValue<T>(value);
        }

        public static string GetEnumValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(DescriptionAttribute), false), (
                                    f, a) => new { Field = f, Att = a })
                            .Where(a => ((DescriptionAttribute)a.Att)
                                .Description.Replace(" ","") == description.Replace(" ", "")).SingleOrDefault();
            return field == null ? "--" : ((EnumMemberAttribute)field.Field.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault()).Value;
        }

        public static T GetObjectEnumFromDescription<T>(string description)
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }

        public static string GetEnumDescriptionFromValue<T>(string value)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(EnumMemberAttribute), false), (
                                    f, a) => new { Field = f, Att = a })
                            .Where(a => ((EnumMemberAttribute)a.Att)
                                .Value == value).SingleOrDefault();
            return field == null ? "--" : ((DescriptionAttribute)field.Field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault()).Description;
        }

        /// <summary>
        /// Directory for create file access permissions
        /// </summary>
        /// <param name="DirectoryPath">Full path to directory </param>
        /// <param name="AccessRight">File System right tested</param>
        /// <returns>State [bool]</returns>
        public static bool DirectoryHasPermission(string DirectoryPath, FileSystemRights AccessRight)
        {
            if (string.IsNullOrEmpty(DirectoryPath)) return false;

            try
            {
                AuthorizationRuleCollection rules = Directory.GetAccessControl(DirectoryPath).GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (identity.Groups.Contains(rule.IdentityReference))
                    {
                        if ((AccessRight & rule.FileSystemRights) == AccessRight)
                        {
                            if (rule.AccessControlType == AccessControlType.Allow)
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            return false;
        }

        public static PathAndNameResourceModel getFormatPathAndNameResource(SettingsModel Settings, string resourceName, int typeFile)
        {
            PathAndNameResourceModel resource = new PathAndNameResourceModel();
            resource.name = resourceName;

            try
            {
                switch (typeFile)
                {
                    case 1:
                        //*.htm, *.html
                        resource.path = Settings.PathHTML;
                        if (!resourceName.ToLower().Contains(".htm") || !resourceName.ToLower().Contains(".html"))
                            resource.name += ".html";
                        break;
                    case 2:
                        //*.css
                        resource.path = Settings.PathCSS;
                        if (!resourceName.ToLower().Contains(".css"))
                            resource.name += ".css";
                        break;
                    case 3:
                        //.js
                        resource.path = Settings.PathJS;
                        if (!resourceName.ToLower().Contains(".js"))
                            resource.name += ".js";
                        break;
                    case 4:
                        //.xml
                        resource.path = Settings.PathXML;
                        if (!resourceName.ToLower().Contains(".xml"))
                            resource.name += ".xml";
                        break;
                    case 5:
                        //.png
                        resource.path = Settings.PathPNG;
                        if (!resourceName.ToLower().Contains(".png"))
                            resource.name += ".png";
                        break;
                    case 6:
                        //.jpg
                        resource.path = Settings.PathJPG;
                        if (!resourceName.ToLower().Contains(".jpg"))
                            resource.name += ".jpg";
                        break;
                    case 7:
                        //.gif
                        resource.path = Settings.PathGIF;
                        if (!resourceName.ToLower().Contains(".gif"))
                            resource.name += ".gif";
                        break;
                    case 8:
                        //.xap
                        resource.path = Settings.PathXAP;
                        if (!resourceName.ToLower().Contains(".xap"))
                            resource.name += ".xap";
                        break;
                    case 9:
                        //.xsl, xslt
                        resource.path = Settings.PathXSL;
                        if (!resourceName.ToLower().Contains(".xsl") || !resourceName.ToLower().Contains(".xslt"))
                            resource.name += ".xslt";
                        break;
                    case 10:
                        //.ico
                        resource.path = Settings.PathICO;
                        if (!resourceName.ToLower().Contains(".ico"))
                            resource.name += ".ico";
                        break;
                    case 11:
                        //.svg
                        resource.path = Settings.PathSVG;
                        if (!resourceName.ToLower().Contains(".svg"))
                            resource.name += ".svg";
                        break;
                    case 12:
                        //.resx
                        resource.path = Settings.PathRESX;
                        if (!resourceName.ToLower().Contains(".resx"))
                            resource.name += ".resx";
                        break;
                }
            }
            catch (Exception ex) { throw ex; }

            if (resource.path.Substring(resource.path.Length - 1, 1) != "\\")
                resource.path += "\\";

            return resource;
        }
    }
}
