using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DolphinDynamicInputTextureCreator.Controls
{
    /// <summary>
    /// Interaction logic for Resizer.xaml.  Original author: Federico from StackOverflow https://stackoverflow.com/questions/16930074/wpf-image-pan-zoom-and-scroll-with-layers-on-a-canvas
    /// </summary>
    public partial class Resizer : UserControl
    {
        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(Resizer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(Resizer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(Resizer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(Resizer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public Resizer()
        {
            InitializeComponent();
        }

        private void UpperLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double old_x = X;
            double old_y = Y;
            X = old_x + e.HorizontalChange;
            Y = old_y + e.VerticalChange;

            double horizontal_change = X - old_x;
            double vertical_change = Y - old_y;

            ItemHeight = ItemHeight + vertical_change * -1;
            ItemWidth = ItemWidth + horizontal_change * -1;

            if (ItemHeight < RegionMinHeight)
                ItemHeight = RegionMinHeight;

            if (ItemWidth < RegionMinWidth)
                ItemWidth = RegionMinWidth;
        }

        private void UpperRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double old_y = Y;
            Y = old_y + e.VerticalChange;

            double vertical_change = Y - old_y;

            ItemHeight = ItemHeight + vertical_change * -1;
            ItemWidth = ItemWidth + e.HorizontalChange;

            if (ItemHeight < RegionMinHeight)
                ItemHeight = RegionMinHeight;

            if (ItemWidth < RegionMinWidth)
                ItemWidth = RegionMinWidth;
        }

        private void LowerLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double old_x = X;
            X = old_x + e.HorizontalChange;

            double horizontal_change = X - old_x;

            ItemHeight = ItemHeight + e.VerticalChange;
            ItemWidth = ItemWidth + horizontal_change * -1;

            if (ItemHeight < RegionMinHeight)
                ItemHeight = RegionMinHeight;

            if (ItemWidth < RegionMinWidth)
                ItemWidth = RegionMinWidth;
        }

        private void LowerRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ItemHeight = ItemHeight + e.VerticalChange;
            ItemWidth = ItemWidth + e.HorizontalChange;

            if (ItemHeight < RegionMinHeight)
                ItemHeight = RegionMinHeight;

            if (ItemWidth < RegionMinWidth)
                ItemWidth = RegionMinWidth;
        }

        private void Center_DragDelta(object sender, DragDeltaEventArgs e)
        {
            X = X + e.HorizontalChange;
            Y = Y + e.VerticalChange;
        }

        private static double RegionMinWidth = 5.0;
        private static double RegionMinHeight = 5.0;
    }
}
