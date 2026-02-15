using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString()).FirstOrDefault();
            var displayAttribute = memberInfo?.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                .FirstOrDefault();

            return displayAttribute?.Name ?? value.ToString();
        }

        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString()).FirstOrDefault();
            var descriptionAttribute = memberInfo?.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                .Cast<System.ComponentModel.DescriptionAttribute>()
                .FirstOrDefault();

            return descriptionAttribute?.Description ?? value.ToString();
        }
    }
}
