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

        private void Upper_DragDelta(object sender, DragDeltaEventArgs e) => Expand_Up(e.VerticalChange);
        private void Lower_DragDelta(object sender, DragDeltaEventArgs e) => Expand_Down(e.VerticalChange);
        private void Left_DragDelta(object sender, DragDeltaEventArgs e) => Expand_Left(e.HorizontalChange);
        private void Right_DragDelta(object sender, DragDeltaEventArgs e) => Expand_Right(e.HorizontalChange);

        private void UpperLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Expand_Up(e.VerticalChange);
            Expand_Left(e.HorizontalChange);
        }

        private void UpperRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Expand_Up(e.VerticalChange);
            Expand_Right(e.HorizontalChange);
        }

        private void LowerLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Expand_Down(e.VerticalChange);
            Expand_Left(e.HorizontalChange);
        }

        private void LowerRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Expand_Down(e.VerticalChange);
            Expand_Right(e.HorizontalChange);
        }

        private void Expand_Up(double VerticalChange)
        {
            // Change only when it makes sense
            if (ItemHeight - VerticalChange < RegionMinHeight)
                return;

            double old_y = Y;

            //it was the easiest way...
            double old_Height = ItemHeight;
            ItemHeight -= VerticalChange;

            Y += VerticalChange;

            ItemHeight = old_Height - (Y - old_y);
        }

        private void Expand_Down(double VerticalChange)
        {
            // Change only when it makes sense
            if (ItemHeight + VerticalChange < RegionMinHeight)
            {
                ItemHeight = RegionMinHeight;
                return;
            }

            ItemHeight = ItemHeight + VerticalChange;
        }

        private void Expand_Left(double HorizontalChange)
        {
            // Change only when it makes sense
            if (ItemWidth - HorizontalChange < RegionMinWidth)
                return;

            double old_x = X;

            //it was the easiest way...
            double old_width = ItemWidth;
            ItemWidth -= HorizontalChange;

            X += HorizontalChange;

            ItemWidth = old_width - (X - old_x);
        }

        private void Expand_Right(double HorizontalChange)
        {
            // Change only when it makes sense
            if (ItemWidth + HorizontalChange < RegionMinWidth)
            {
                ItemWidth = RegionMinWidth;
                return;
            }

            ItemWidth = ItemWidth + HorizontalChange;
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
