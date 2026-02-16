using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Aadev.JTF.Tools;

public sealed class JsonBuilder
{
    private readonly StringBuilder stringBuilder;
    private bool addComma;
    [return: NotNullIfNotNull(nameof(value))]
    private static string? SanitizeString(string? value)
    {
        return value?.Replace("\"", "\\\"", StringComparison.Ordinal)?.Replace("\\", "\\\\", StringComparison.Ordinal);
    }
    public JsonBuilder()
    {
        stringBuilder = new StringBuilder();
    }
    public void StartBlock()
    {
        stringBuilder.Append('{');
        addComma = false;
    }
    public void EndBlock()
    {
        stringBuilder.Append('}');
        addComma = true;
    }
    public void StartArray()
    {
        stringBuilder.Append('[');
        addComma = false;
    }
    public void EndArray()
    {
        stringBuilder.Append(']');
        addComma = true;
    }

#if NETSTANDARD2_1

    public  void AddProperty(string name, string? value)
    {
        AddComma();
        stringBuilder.Append($"\"{name}\": \"{SanitizeString(value)}\"");
    }
    public  void AddProperty(string name, bool value)
    {
        AddComma();
        if (value)
        {
            stringBuilder.Append($"\"{name}\": true");
        }
        else
        {
            stringBuilder.Append($"\"{name}\": false");
        }
    }
    public  void AddProperty(string name, int value)
    {
        AddComma();
        stringBuilder.Append($"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public  void AddProperty(string name, long value)
    {
        AddComma();
        stringBuilder.Append($"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public  void AddProperty(string name, short value)
    {
        AddComma();
        stringBuilder.Append($"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public  void AddProperty(string name, byte value)
    {
        AddComma();
        stringBuilder.Append($"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public  void AddProperty(string name, float value)
    {
        AddComma();
        stringBuilder.Append($"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public  void AddProperty(string name, double value)
    {
        AddComma();
        stringBuilder.Append($"\"{name}\": {value}");
    }
    public  void AddProperty(string name, IJsonBuildable builder)
    {
        AddComma();
        stringBuilder.Append($"\"{name}\": ");
        addComma = false;
        builder.BuildJson(this);
        addComma = true;
    }
    public  void AddProperty(string name)
    {
        AddComma();
        stringBuilder.Append($"\"{name}\": ");
        addComma = false;
    }
    public  void AddValue(string? value)
    {
        AddComma();
        stringBuilder.Append($"\"{SanitizeString(value)}\"");
    }
#else
    public void AddProperty(string name, string? value)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": \"{SanitizeString(value)}\"");
    }
    public void AddProperty(string name, bool value)
    {
        AddComma();
        if (value)
        {
            stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": true");
        }
        else
        {
            stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": false");
        }
    }
    public void AddProperty(string name, int value)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public void AddProperty(string name, long value)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public void AddProperty(string name, short value)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public void AddProperty(string name, byte value)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public void AddProperty(string name, float value)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": {value.ToString(CultureInfo.InvariantCulture)}");
    }
    public void AddProperty(string name, double value)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": {value}");
    }
    public void AddProperty(string name, IJsonBuildable builder)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": ");
        addComma = false;
        builder.BuildJson(this);
        addComma = true;
    }
    public void AddProperty(string name)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{name}\": ");
        addComma = false;
    }
    public void AddValue(string? value)
    {
        AddComma();
        stringBuilder.Append(CultureInfo.InvariantCulture, $"\"{SanitizeString(value)}\"");
    }
#endif


    public void AddValue(bool value)
    {
        AddComma();
        if (value)
        {
            stringBuilder.Append("true");
        }
        else
        {
            stringBuilder.Append("false");
        }
    }
    public void AddValue(int value)
    {
        AddComma();
        stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
    }
    public void AddValue(long value)
    {
        AddComma();
        stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
    }
    public void AddValue(short value)
    {
        AddComma();
        stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
    }
    public void AddValue(byte value)
    {
        AddComma();
        stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
    }
    public void AddValue(float value)
    {
        AddComma();
        stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
    }
    public void AddValue(double value)
    {
        AddComma();
        stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
    }
    public void AddValue(IJsonBuildable builder)
    {
        AddComma();
        addComma = false;
        builder.BuildJson(this);
        addComma = true;
    }
    public void AddTProperty<TValue>(string name, TValue value)
    {
        switch (value)
        {
            case string s:
                AddProperty(name, s);
                break;
            case byte b:
                AddProperty(name, b);
                break;
            case short s:
                AddProperty(name, s);
                break;
            case int i:
                AddProperty(name, i);
                break;
            case long l:
                AddProperty(name, l);
                break;
            case float f:
                AddProperty(name, f);
                break;
            case double d:
                AddProperty(name, d);
                break;
            case bool b:
                AddProperty(name, b);
                break;
            case IJsonBuildable j:
                AddProperty(name, j);
                break;
            default:
                throw new InvalidCastException();
        }
    }


    private void AddComma()
    {
        if (addComma)
            stringBuilder.Append(',');
        else
            addComma = true;
    }
    public override string ToString() => stringBuilder.ToString();
}
