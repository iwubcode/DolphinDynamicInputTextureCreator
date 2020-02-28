using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace DolphinDynamicInputTextureCreator.ValueConverters
{
    class ColorToPropertyInfoValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            var type = typeof(Colors);
            var res = type.GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.GetValue(null, null).ToString() == color.ToString())
                .Select(p => p).FirstOrDefault();
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var prop = (System.Reflection.PropertyInfo)value;
            return ColorConverter.ConvertFromString(prop.Name);
        }
    }
}
