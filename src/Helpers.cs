﻿namespace Aadev.JTF;

internal static class Helpers
{
    internal static byte Clamp(this byte value, byte min, byte max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static short Clamp(this short value, short min, short max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static int Clamp(this int value, int min, int max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static long Clamp(this long value, long min, long max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static float Clamp(this float value, float min, float max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static double Clamp(this double value, double min, double max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static byte? Clamp(this byte? value, byte min, byte max)
    {
        if (value is null)
            return value;
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static short? Clamp(this short? value, short min, short max)
    {
        if (value is null)
            return value;
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static int? Clamp(this int? value, int min, int max)
    {
        if (value is null)
            return value;
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static long? Clamp(this long? value, long min, long max)
    {
        if (value is null)
            return value;
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static float? Clamp(this float? value, float min, float max)
    {
        if (value is null)
            return value;
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static double? Clamp(this double? value, double min, double max)
    {
        if (value is null)
            return value;
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
    internal static int Min(this int a, int b) => a < b ? a : b;

    internal static byte? Min(this byte? a, byte? b) => a is null || b is null ? null : (a < b ? a : b);
    internal static short? Min(this short? a, short? b) => a is null || b is null ? null : (a < b ? a : b);
    internal static int? Min(this int? a, int? b) => a is null || b is null ? null : (a < b ? a : b);
    internal static long? Min(this long? a, long? b) => a is null || b is null ? null : (a < b ? a : b);
    internal static float? Min(this float? a, float? b) => a is null || b is null ? null : (a < b ? a : b);
    internal static double? Min(this double? a, double? b) => a is null || b is null ? null : (a < b ? a : b);


    internal static int Max(this int a, int b) => a > b ? a : b;

    internal static byte? Max(this byte? a, byte? b) => a is null || b is null ? null : (a > b ? a : b);
    internal static short? Max(this short? a, short? b) => a is null || b is null ? null : (a > b ? a : b);
    internal static int? Max(this int? a, int? b) => a is null || b is null ? null : (a > b ? a : b);
    internal static long? Max(this long? a, long? b) => a is null || b is null ? null : (a > b ? a : b);
    internal static float? Max(this float? a, float? b) => a is null || b is null ? null : (a > b ? a : b);
    internal static double? Max(this double? a, double? b) => a is null || b is null ? null : (a > b ? a : b);
}