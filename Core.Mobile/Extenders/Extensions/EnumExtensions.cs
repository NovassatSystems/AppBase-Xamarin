using System;
using System.ComponentModel;

namespace Core
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum source) {
            var fi = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }
}
