using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            SetInputPack(new Data.DynamicInputPack());
        }

        private void SetInputPack(Data.DynamicInputPack pack)
        {
            DataContext = pack;
            ((ViewModels.PanZoomViewModel)PanZoom.DataContext).InputPack = pack;
        }

        private string _saved_document = null;

        private Window _edit_emulated_devices_window;
        private Window _edit_host_devices_window;

        private Data.DynamicInputPack InputPack
        {
            get
            {
                return (Data.DynamicInputPack)DataContext;
            }
        }

        private void ExportToLocation_Click(object sender, RoutedEventArgs e)
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
            if (_edit_host_devices_window != null)
            {
                _edit_host_devices_window.Close();
            }

            _edit_host_devices_window = new Window
            {
                Title = "Editing Host Devices",
                ResizeMode = ResizeMode.CanResize,
                SizeToContent = SizeToContent.Height,
                Owner = Application.Current.MainWindow,
                Width = 500, MinWidth = 500, MinHeight = 400
            };

            UpdateEditWindows();
            _edit_host_devices_window.Show();
        }

        private void EditEmulatedDevices_Click(object sender, RoutedEventArgs e)
        {
            if (_edit_emulated_devices_window != null)
            {
                _edit_emulated_devices_window.Close();
            }

            _edit_emulated_devices_window = new Window
            {
                Title = "Editing Emulated Devices",
                ResizeMode = ResizeMode.CanResize,
                SizeToContent = SizeToContent.Height,
                Owner = Application.Current.MainWindow,
                Width= 500, MinWidth = 500, MinHeight = 400
            };

            UpdateEditWindows();
            _edit_emulated_devices_window.Show();
        }

        public static RoutedUICommand SaveAsCmd = new RoutedUICommand("Save as...", "SaveAsCmd", typeof(MainWindow));

        #region SAVE
        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            if (_saved_document == null)
            {
                SaveAsData_Click(sender, e);
            }
            else
            {
                string output = JsonConvert.SerializeObject(InputPack, Formatting.Indented);
                File.WriteAllText(_saved_document, output);
            }
        }

        private void SaveData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region OPEN
        private void OpenData_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "JSON Files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string input = File.ReadAllText(dialog.FileName);
                var settings = new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace };
                SetInputPack(JsonConvert.DeserializeObject<Data.DynamicInputPack>(input, settings));
                _saved_document = dialog.FileName;
                UpdateEditWindows();
            }
        }

        private void OpenData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region NEW
        private void NewData_Click(object sender, RoutedEventArgs e)
        {
            SetInputPack(new Data.DynamicInputPack());
            UpdateEditWindows();
            _saved_document = null;
        }

        private void NewData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region SAVE AS
        private void SaveAsData_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "JSON Files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string output = JsonConvert.SerializeObject(InputPack, Formatting.Indented);
                File.WriteAllText(dialog.FileName, output);
                _saved_document = dialog.FileName;
            }
        }

        private void SaveDataAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        private void UpdateEditWindows()
        {
            if (_edit_emulated_devices_window != null)
            {
                var user_control = new Controls.EditEmulatedDevices { DataContext = InputPack };
                _edit_emulated_devices_window.Content = user_control;
            }

            if (_edit_host_devices_window != null)
            {
                var user_control = new Controls.EditHostDevices { DataContext = InputPack };
                _edit_host_devices_window.Content = user_control;
            }
        }
    }
}
