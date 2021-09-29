﻿using DolphinDynamicInputTextureCreator.ViewModels;
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
            this.Title += " " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            InputPack = Models.DefaultData.NewInputPack();
        }

        private DynamicInputPackViewModel InputPack
        {
            get => (DynamicInputPackViewModel)DataContext;
            set
            {
                DataContext = value;
                CheckInputPackTexturePathes(value);
                ((PanZoomViewModel)PanZoom.DataContext).InputPack = value;
                UnsavedChanges = false;
            }
        }

        private string _saved_document = null;

        private Window _edit_emulated_devices_window;
        private Window _edit_host_devices_window;
        private Window _edit_metadata_window;

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
            this.Close();
        }

        private void EditHostDevices_Click(object sender, RoutedEventArgs e)
        {
            _edit_host_devices_window?.Close();

            _edit_host_devices_window = new Window
            {
                Title = "Editing Host Devices",
                ResizeMode = ResizeMode.CanResize,
                SizeToContent = SizeToContent.Manual,
                Owner = Application.Current.MainWindow,
                Top = this.Top + 50, Left = this.Left + 70,
                Width = 620, Height = 550, MinWidth = 500, MinHeight = 400
            };

            UpdateEditWindows();
            _edit_host_devices_window.Show();
        }

        private void EditEmulatedDevices_Click(object sender, RoutedEventArgs e)
        {
            _edit_emulated_devices_window?.Close();

            _edit_emulated_devices_window = new Window
            {
                Title = "Editing Emulated Devices",
                ResizeMode = ResizeMode.CanResize,
                SizeToContent = SizeToContent.Manual,
                Owner = Application.Current.MainWindow,
                Top = this.Top + 50, Left = this.Left + 70,
                Width = 620, Height = 550, MinWidth = 500, MinHeight = 400
            };

            UpdateEditWindows();
            _edit_emulated_devices_window.Show();
        }

        private void EditMetadata_Click(object sender, RoutedEventArgs e)
        {
            _edit_metadata_window?.Close();

            _edit_metadata_window = new Window
            {
                Title = "Editing Metadata",
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                Owner = Application.Current.MainWindow,
                Top = this.Top + 50, Left = this.Left + 70
            };

            UpdateEditWindows();
            _edit_metadata_window.Show();
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
                UnsavedChanges = false;
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
            dialog.Filter = "DIT Files (*.dit)|*.dit|JSON Files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string input = File.ReadAllText(dialog.FileName);
                var settings = new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace };
                InputPack = JsonConvert.DeserializeObject<DynamicInputPackViewModel>(input, settings);
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
            InputPack = Models.DefaultData.NewInputPack();
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
            dialog.Filter = "DIT Files (*.dit)|*.dit|JSON Files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string output = JsonConvert.SerializeObject(InputPack, Formatting.Indented);
                File.WriteAllText(dialog.FileName, output);
                _saved_document = dialog.FileName;
                UnsavedChanges = false;
            }
        }

        private void SaveStartupSuggestions_Click(object sender, RoutedEventArgs e)
        {
            Models.DefaultData.SaveSettings();
        }

        private void SaveAsDefaultPack_Click(object sender, RoutedEventArgs e)
        {
            Models.DefaultData.SaveInputPack(InputPack);
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
                var user_control = new Controls.EditEmulatedDevices { DataContext = new EmulatedDeviceKeysViewModel { InputPack = InputPack } };
                _edit_emulated_devices_window.Content = user_control;
            }

            if (_edit_host_devices_window != null)
            {
                var user_control = new Controls.EditHostDevices { DataContext = new HostDeviceKeyViewModel { HostDevices = InputPack.HostDevices } };
                _edit_host_devices_window.Content = user_control;
            }

            if (_edit_metadata_window != null)
            {
                var user_control = new Controls.Metadata { DataContext = InputPack };
                _edit_metadata_window.Content = user_control;
            }
        }

        #region Test
        public void CheckInputPackTexturePathes(DynamicInputPackViewModel inputPack)
        {
            foreach (Data.HostDevice device in inputPack.HostDevices)
            {
                foreach (Data.HostKey key in device.HostKeys)
                {
                    if (!File.Exists(key.TexturePath))
                    {
                        key.TexturePath = CheckForNewPath(key.Name, key.TexturePath);
                    }
                }
            }
            foreach (Data.DynamicInputTexture texture in inputPack.Textures)
            {
                if (!File.Exists(texture.TexturePath))
                {
                    texture.TexturePath = CheckForNewPath(texture.TextureHash, texture.TexturePath);
                }
            }
        }

        private string CheckForNewPath(string name, string path)
        {
            MessageBoxResult MessageResult;
            MessageResult = MessageBox.Show(String.Format("'{1}'\nThe image of '{0}' could not be found!\nsearch for the picture?", name, path), name, MessageBoxButton.YesNo);
            if (MessageResult == MessageBoxResult.Yes)
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.FileName = System.IO.Path.GetFileName(path);
                dialog.DefaultExt = ".png";
                dialog.Filter = "Original Name |" + dialog.FileName;
                dialog.Filter += "|PNG Files (*.png)|*.png";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }
            return path;
        }
        #endregion

        private void ButtonFill_Click(object sender, RoutedEventArgs e)
        {
            PanZoom.ViewModel.FillRegion();
        }

        #region Closing

        public bool UnsavedChanges
        {
            get => unsavedChanges;
            set
            {
                unsavedChanges = value;

                if (unsavedChanges)
                {
                    InputPack.Textures.CollectionChanged -= ChangesObserved;
                    InputPack.HostDevices.CollectionChanged -= ChangesObserved;
                    InputPack.EmulatedDevices.CollectionChanged -= ChangesObserved;
                }
                else
                {
                    InputPack.Textures.CollectionChanged += ChangesObserved;
                    InputPack.HostDevices.CollectionChanged += ChangesObserved;
                    InputPack.EmulatedDevices.CollectionChanged += ChangesObserved;
                }
            }
        }
        private bool unsavedChanges = false;

        private void ChangesObserved(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => UnsavedChanges = true;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (unsavedChanges)
            {
                MessageBoxResult MessageResult;
                MessageResult = MessageBox.Show("unsaved changes are lost, do you want to save?", "unsaved changes!", MessageBoxButton.YesNoCancel);
                switch (MessageResult)
                {
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.Yes:
                        SaveData_Click(sender, new RoutedEventArgs());
                        e.Cancel = unsavedChanges;
                        break;
                }
            }
        }

        #endregion

    }
}
