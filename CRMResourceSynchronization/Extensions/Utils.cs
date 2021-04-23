using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CRMResourceSynchronization.Extensions
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
    }
}
