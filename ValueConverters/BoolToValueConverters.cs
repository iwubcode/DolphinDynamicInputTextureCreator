using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace DolphinDynamicInputTextureCreator.ValueConverters
{

    [ValueConversion(typeof(bool), typeof(System.Windows.Media.SolidColorBrush))]
    public class BoolToSolidColorBrushConverter : BoolToValueConverter<System.Windows.Media.SolidColorBrush> {}


    [ValueConversion(typeof(bool), typeof(System.Windows.Media.Color))]
    public class BoolToColorConverter : BoolToValueConverter<System.Windows.Media.Color> { }

    public abstract class BoolToValueConverter<T> : IValueConverter
    {
        public T TrueValue { get; set; }
        public T FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? FalseValue : ((bool)value ? TrueValue : FalseValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(TrueValue);
        }
    }
}
