﻿using System;

namespace SCMM.Shared.Data.Models.Extensions
{
    public static class NumberExtensions
    {
        public static string ToQuantityString(this int quantity)
        {
            return ToQuantityString((long)quantity);
        }

        public static string ToQuantityString(this long quantity)
        {
            return quantity > 0
                ? quantity.ToString("#,##")
                : "∞";
        }

        public static string ToMovementString(this int value, int max)
        {
            return ToMovementString((long)value, (long)max);
        }

        public static string ToMovementString(this long value, long max)
        {
            if (value == 0 || max == 0)
            {
                return null;
            }
            var movement = Math.Abs(100 - (int)Math.Round((decimal)value / max * 100, 0));
            return $"{movement.ToString("#,##0")}%".Trim();
        }

        public static string ToPercentageString(this int value, int max)
        {
            return ToPercentageString((long)value, (long)max);
        }

        public static string ToPercentageString(this long value, long max)
        {
            if (value == 0 || max == 0)
            {
                return null;
            }
            var percentage = (int)Math.Round((decimal)value / max * 100, 0);
            return $"{percentage.ToString("#,##0")}%".Trim();
        }

        public static string ToRoIString(this int percentage)
        {
            return (percentage != 0 ? $"{percentage}%" : "-").Trim();
        }

        public static string ToGCDRatioString(this int a, int b)
        {
            if (a == 0 || b == 0)
            {
                return "∞";
            }
            var gcd = GCD(a, b);
            return $"{a / gcd}:{b / gcd}";
        }

        public static long GCD(long a, long b)
        {
            return b == 0 ? Math.Abs(a) : GCD(b, a % b);
        }
    }
}
