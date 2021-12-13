using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DolphinDynamicInputTextureCreator.ValueConverters
{

    [ValueConversion(typeof(Path), typeof(DrawingImage))]
    public class ShapePathToImageConverter : IValueConverter
    {
        public static DrawingImage ConvertToDrawingImage(in Path path, double outline_thickness = 1)
        {
            Pen pen = new Pen(path.Fill.Clone(), outline_thickness);
            pen.Brush.Opacity = 0.4;

            GeometryDrawing drawing = new GeometryDrawing
            {
                Geometry = path.Data,
                Brush = path.Fill,
                Pen = pen
            };

            return new DrawingImage(drawing);
        }

        public static Path ConvertToPath(in GeometryDrawing drawing)
        {
            return new Path() { Data = drawing.Geometry, Fill = drawing.Brush };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToDrawingImage((Path)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DrawingImage image = (DrawingImage)value;
            return ConvertToPath((GeometryDrawing)image.Drawing);
        }
    }
}
