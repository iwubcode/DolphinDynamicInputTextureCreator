using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace DolphinDynamicInputTextureCreator.ValueConverters
{
    /// <summary>
    /// Combines several converters into one new converter.
    /// </summary>
    [ContentProperty("Converters")]
    [ContentWrapper(typeof(List<IValueConverter>))]
    class ConverterCombinationConverter : IValueConverter
    {
        public List<IValueConverter> Converters { get; } = new List<IValueConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (IValueConverter item in Converters)
            {
                value = item.Convert(value, targetType, parameter, culture);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Converters.Reverse();
            foreach (IValueConverter item in Converters)
            {
                value = item.ConvertBack(value, targetType, parameter, culture);
            }
            Converters.Reverse();
            return value;
        }
    }
}
