using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Aadev.JTF.Design
{
    public class JtIdentifierConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(JtIdentifier))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
        {
            if (destinationType == typeof(string) || destinationType == typeof(JtIdentifier))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string strValue)
            {
                return new JtIdentifier(strValue);
            }
            if (value is JtIdentifier idValue)
            {
                return idValue.Value;
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value is string strValue && destinationType == typeof(JtIdentifier))
            {
                return new JtIdentifier(strValue);
            }
            if (value is JtIdentifier idValue && destinationType == typeof(string))
            {
                return idValue.Value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
