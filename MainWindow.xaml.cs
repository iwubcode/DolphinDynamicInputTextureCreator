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
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string input = File.ReadAllText(dialog.FileName);
                var settings = new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace };
                SetInputPack(JsonConvert.DeserializeObject<Data.DynamicInputPack>(input, settings));
                _saved_document = dialog.FileName;
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
    }
}
