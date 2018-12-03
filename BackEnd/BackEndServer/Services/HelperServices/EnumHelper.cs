using System;
using System.ComponentModel;
using System.Reflection;
using BackEndServer.Models.Enums;

namespace BackEndServer.Services.HelperServices
{
    public static class EnumHelper
    {
        public static string GetDescription(this ContactMethod value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}