using System;
using System.ComponentModel;

namespace Services
{
    public static class EnumExtensions
    {
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T) attributes[0];
        }

        public static string GetDescription(this Enum value)
        {
            return value.GetAttribute<DescriptionAttribute>().Description;
        }
    }
}
