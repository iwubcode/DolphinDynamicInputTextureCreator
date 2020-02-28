using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace DolphinDynamicInputTextureCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Data.DynamicInputPack();
            ((ViewModels.PanZoomViewModel)PanZoom.DataContext).InputPack = InputPack;
        }

        private Data.DynamicInputPack InputPack
        {
            get
            {
                return (Data.DynamicInputPack)DataContext;
            }
        }

        private void SaveToLocation_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                InputPack.OutputToLocation(dialog.SelectedPath);
            }
        }

        private void QuitProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void EditHostDevices_Click(object sender, RoutedEventArgs e)
        {
            var user_control = new Controls.EditHostDevices { DataContext = InputPack };
            Window window = new Window
            {
                Title = "Editing Host Devices",
                Content = user_control,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();
        }

        private void EditEmulatedDevices_Click(object sender, RoutedEventArgs e)
        {
            var user_control = new Controls.EditEmulatedDevices { DataContext = InputPack };
            Window window = new Window
            {
                Title = "Editing Emulated Devices",
                Content = user_control,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();
        }
    }
}
