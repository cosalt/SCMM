﻿namespace SCMM.Shared.Data.Models.Extensions
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.First().ToString().ToUpper() + value.Substring(1);
        }

        public static string FirstCharToLower(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.First().ToString().ToLower() + value.Substring(1);
        }
        
        public static string Pluralise(this string value, int count = 0)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            if (value.EndsWith('s'))
            {
                return count == 1
                    ? value.TrimEnd('s')
                    : value;
            }
            if (value.EndsWith('x'))
            {
                return count == 1
                    ? value
                    : $"{value}es";
            }
            else
            {
                return count == 1
                    ? value
                    : $"{value}s";
            }
        }

        public static string Trim(this string value, params string[] trimStrings)
        {
            foreach(var trimString in trimStrings)
            {
                if (value.StartsWith(trimString))
                {
                    value = value.Substring(trimString.Length);
                }
                if (value.EndsWith(trimString))
                {
                    value = value.Substring(0, value.Length - trimString.Length);
                }
            }
            return value;
        }

        public static T As<T>(this string value)
        {
            var underlyingType = Nullable.GetUnderlyingType(typeof(T));
            if (underlyingType != null && value == null)
            {
                return default;
            }
            var baseType = (underlyingType == null ? typeof(T) : underlyingType);
            if (baseType.IsEnum)
            {
                return ((T)Enum.Parse(baseType, value));
            }
            else
            {
                return ((T)Convert.ChangeType(value, baseType)) ?? default;
            }
        }
    }
}
