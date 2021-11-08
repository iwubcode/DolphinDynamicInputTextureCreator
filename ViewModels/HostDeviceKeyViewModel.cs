using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTextureCreator.Models.Suggestions;
using DolphinDynamicInputTextureCreator.Other;
using DolphinDynamicInputTextureCreator.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.ViewModels
{
    public class HostDeviceKeyViewModel : Other.PropertyChangedBase
    {
        #region Properties

        /// <summary>
        /// The host devices mapped in this pack
        /// </summary>
        public UICollection<HostDevice> HostDevices
        {
            get => _host_devices;
            set
            {
                _host_devices = value;
                HostDeviceSuggestions.SetTargetList(HostDevices);
                HostDevices.SelectedChanged = (Device) => UpdateAvailableSuggestions();
                HostDevices.Select(Selection.First);
                OnPropertyChanged(nameof(HostDevices));
            }
        }
        private UICollection<HostDevice> _host_devices;

        private static Regex _host_devices_regex = new Regex(@"\A[^\/\\?:<>|]+\/\d\/[^\/\\?:<>|]+\Z");

        /// <summary>
        /// The Tags mapped in this pack
        /// </summary>
        public ObservableCollection<Tag> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }
        private ObservableCollection<Tag> _tags;

        #endregion

        #region Suggestions

        /// <summary>
        /// Suggestions for possible host device names
        /// </summary>
        public SuggestionList<HostDevice> HostDeviceSuggestions { get; } = new SuggestionList<HostDevice>((device) => device.Name, Models.DefaultData.Suggestions.HostDeviceNames);

        /// <summary>
        /// possible key Suggestions for the Selected host device.
        /// </summary>
        public ObservableCollection<string> HostKeySuggestions { get; set; } = new ObservableCollection<string>();

        private void UpdateAvailableSuggestions()
        {
            HostKeySuggestions.Clear();
            if (HostDevices.Selected == null)
                return;

            string[] splitname = HostDevices.Selected.Name.Split('/');
            for (int i = splitname.Length-1; i >= 0; i-=2)
            {
                if (Models.DefaultData.Suggestions.HostDevicesKeys.ContainsKey(splitname[i]))
                {
                    HostKeySuggestions = new ObservableCollection<string>(Models.DefaultData.Suggestions.HostDevicesKeys[splitname[i]]);
                    break;
                }
            }
        }

        #endregion

        #region Commands

        #region KeyCommands

        public ICommand DeleteKeyCommand => new RelayCommand<HostKey>(key => HostDevices.Selected.HostKeys.Remove(key));

        public ICommand AddNewKeyDialogCommand => new RelayCommand(I => AddNewKeyDialog());

        public void AddNewKeyDialog()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Files (*.png)|*.png";
            dlg.Multiselect = true;

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                foreach (var file in dlg.FileNames)
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    name = HostKeySuggestions.Contains(name) ? name : "";

                    HostDevices.Selected.HostKeys.Add(new HostKey { Name = name, TexturePath = file });
                }
            }
        }

        #endregion

        #region NewHostDevice

        public VisibilityCommand InputNewHostDevice
        {
            get => _input_new_host_device ??= new VisibilityCommand(
                (input) => ((System.Windows.Controls.ComboBox)input).Text = HostDeviceSuggestions.GetUnusedSuggestion());
        }
        private VisibilityCommand _input_new_host_device;

        public ICommand InputNewHostDeviceOKCommand => new RelayCommand<string>(OkNewHostDevice, _host_devices_regex.IsMatch);

        private void OkNewHostDevice(string name)
        {
            HostDevices.Add(new HostDevice { Name = name });
            InputNewHostDevice.SetToCollapsCommand.Execute(name);
        }

    #endregion

    #endregion

}
}
