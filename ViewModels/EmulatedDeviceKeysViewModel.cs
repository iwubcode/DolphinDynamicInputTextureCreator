using DolphinDynamicInputTextureCreator.Data;
using DolphinDynamicInputTextureCreator.Models.Suggestions;
using DolphinDynamicInputTextureCreator.Other;
using DolphinDynamicInputTextureCreator.ViewModels.Commands;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.ViewModels
{
    public class EmulatedDeviceKeysViewModel : Other.PropertyChangedBase
    {

        #region Properties

        public DynamicInputPackViewModel InputPack
        {
            get => _input_pack;
            set
            {
                _input_pack = value;
                EmulatedDevices = value.EmulatedDevices;
            }
        }
        private DynamicInputPackViewModel _input_pack;

        /// <summary>
        /// The emulated devices mapped in this pack
        /// </summary>
        public UICollection<EmulatedDevice> EmulatedDevices
        {
            get => emulatedDevices;
            private set
            {
                emulatedDevices = value;
                EmulatedDeviceSuggestions.SetTargetList(EmulatedDevices);
                EmulatedDevices.SelectedChanged = (Device) => UpdateAvailableSuggestions();
                OnPropertyChanged(nameof(EmulatedDevices));
            }
        }
        private UICollection<EmulatedDevice> emulatedDevices;

        private static Regex _emulated_devices_regex = new Regex(@"\A\S+\d\Z");

        #endregion

        #region Suggestion

        /// <summary>
        /// Suggestions for possible emulated device names.
        /// </summary>
        public SuggestionList<EmulatedDevice> EmulatedDeviceSuggestions { get; } = new SuggestionList<EmulatedDevice>((device) => device.Name, Models.DefaultData.Suggestions.EmulatedDeviceNames);

        /// <summary>
        /// possible key Suggestions for the Selected emulated device.
        /// </summary>
        public SuggestionList<EmulatedKey> EmulatedKeySuggestions { get; set; } = new SuggestionList<EmulatedKey>((key) => key.Name);

        private void UpdateAvailableSuggestions()
        {
            EmulatedKeySuggestions.Clear();
            if (EmulatedDevices.Selected == null)
                return;

            string name = EmulatedDevices.Selected.Name[0..^1];
            if (Models.DefaultData.Suggestions.EmulatedDeviceKeys.ContainsKey(name))
            {
                EmulatedKeySuggestions = new SuggestionList<EmulatedKey>((key) => key.Name, Models.DefaultData.Suggestions.EmulatedDeviceKeys[name]);
                EmulatedKeySuggestions.SetTargetList(EmulatedDevices.Selected.EmulatedKeys);
            }
        }


        #endregion

        #region Commands

        #region KeyCommands

        public ICommand DeleteKeyCommand => new RelayCommand<EmulatedKey>(key => EmulatedDevices.Selected.EmulatedKeys.Remove(key));

        public ICommand AddKeyCommand => new RelayCommand<string>(AddKey);

        public ICommand AddAllKeyCommand => new RelayCommand(AddAllKey, (I) => EmulatedKeySuggestions.Available.Count != 0);

        private void AddAllKey(object obj)
        {
            string name;
            while ((name = EmulatedKeySuggestions.GetUnusedSuggestion()) != "")
            {
                AddKey(name);
            }
        }

        private void AddKey(string name)
        {
            name ??= EmulatedKeySuggestions.GetUnusedSuggestion();

            EmulatedKey new_key = new EmulatedKey { Name = name };
            EmulatedDevices.Selected.EmulatedKeys.Add(new_key);
            InputPack.SelectedRegionBrush.SelectedEmulatedKey ??= new_key;
        }

        #endregion

        #region NewEmulatedDevice

        public VisibilityCommand InputNewEmulatedDevice
        {
            get => _input_new_emulated_device ??= new VisibilityCommand(
                (input) => ((System.Windows.Controls.ComboBox)input).Text = EmulatedDeviceSuggestions.GetUnusedSuggestion());
        }
        private VisibilityCommand _input_new_emulated_device;

        public ICommand InputNewEmulatedDeviceOKCommand => new RelayCommand<string>(OkNewEmulatedDevice, _emulated_devices_regex.IsMatch);

        private void OkNewEmulatedDevice(string name)
        {
            EmulatedDevices.Add(new EmulatedDevice
            {
                Name = name
            });
            InputNewEmulatedDevice.SetToCollapsCommand.Execute(name);
            InputPack.SelectedRegionBrush.SelectedEmulatedDevice ??= EmulatedDevices.Selected;
        }
        #endregion

        #endregion

    }
}
