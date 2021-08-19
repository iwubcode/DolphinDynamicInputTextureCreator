using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for PanZoom.xaml
    /// </summary>
    public partial class PanZoom : UserControl
    {
        public ViewModels.PanZoomViewModel ViewModel { get; set; }

        public PanZoom()
        {
            InitializeComponent();

            DataContext = ViewModel = new ViewModels.PanZoomViewModel();
        }
        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!ViewModel.IsPanning)
            {
                ViewModel.OffsetX = e.HorizontalOffset;
                ViewModel.OffsetY = e.VerticalOffset;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ViewModel.StartPanning(e.GetPosition(Img));
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.StartCreatingRegion(e.GetPosition(Img));
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                ViewModel.StopCreatingRegion();
            }

            if (e.MiddleButton == MouseButtonState.Released)
            {
                ViewModel.StopPanning();
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.UpdateCreatingRegion(e.GetPosition(Img));
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ViewModel.UpdatePanning(e.GetPosition(Img));

                scr.ScrollToVerticalOffset(ViewModel.OffsetY);
                scr.ScrollToHorizontalOffset(ViewModel.OffsetX);
            }
        }

        /// <summary>
        /// ScrollViewer mouse wheel functions
        /// </summary>
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Mouse wheel + Ctrl = zoom
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                e.Handled = true;
                if (e.Delta > 0)
                {
                    ViewModel.InputPack.EditingTexture.ScaleFactor *= 1.1;
                }
                else
                {
                    ViewModel.InputPack.EditingTexture.ScaleFactor /= 1.1;
                }
            }
            // Mouse wheel + Shift = horizontal scrolling
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                e.Handled = true;
                double offset = scr.HorizontalOffset;
                scr.ScrollToHorizontalOffset(offset+e.Delta/2);
            }
        }
    }
}
