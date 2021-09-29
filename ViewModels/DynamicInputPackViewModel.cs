using DolphinDynamicInputTextureCreator.Data;
using DolphinDynamicInputTextureCreator.Other;
using DolphinDynamicInputTextureCreator.ViewModels.Commands;
using Newtonsoft.Json;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.ViewModels
{
    public class DynamicInputPackViewModel : DynamicInputPack
    {
        #region View Model Properties

        /// <summary>
        /// The textures defined for dynamic input modification in this pack
        /// </summary>
        public new UICollection<DynamicInputTexture> Textures
        {
            get => _textures ??= Textures = new UICollection<DynamicInputTexture>();
            set
            {
                base.Textures = _textures = value;
                OnPropertyChanged(nameof(Textures));
            }
        }
        private UICollection<DynamicInputTexture> _textures;

        /// <summary>
        /// The host devices mapped in this pack
        /// </summary>
        public new UICollection<HostDevice> HostDevices
        {
            get => _host_devices ??= HostDevices = new UICollection<HostDevice>();
            set
            {
                base.HostDevices = _host_devices = value;
                HostDeviceSuggestions.SetTargetList(HostDevices);
                OnPropertyChanged(nameof(HostDevices));
            }
        }
        private UICollection<HostDevice> _host_devices;

        /// <summary>
        /// The emulated devices mapped in this pack
        /// </summary>
        public UICollection<EmulatedDevice> EmulatedDevices
        {
            get => emulatedDevices ??= EmulatedDevices = new UICollection<EmulatedDevice>();
            set
            {
                emulatedDevices = value;
                EmulatedDeviceSuggestions.SetTargetList(EmulatedDevices);
                OnPropertyChanged(nameof(ShouldGetHashFromTextureFilename));
            }
        }
        private UICollection<EmulatedDevice> emulatedDevices;

        /// <summary>
        /// The current emulated device / key "brush" chosen for writing regions to the texture
        /// </summary>
        private RegionBrush _selected_region_brush = new RegionBrush();
        public RegionBrush SelectedRegionBrush
        {
            get => _selected_region_brush;
            set
            {
                _selected_region_brush = value;
                OnPropertyChanged(nameof(SelectedRegionBrush));
            }
        }

        /// <summary>
        /// When adding a new texture, whether the hash should be pulled off of the filename
        /// </summary>
        private bool _should_get_hash_from_texture_filename = true;
        public bool ShouldGetHashFromTextureFilename
        {
            get => _should_get_hash_from_texture_filename;
            set
            {
                _should_get_hash_from_texture_filename = value;
                OnPropertyChanged(nameof(ShouldGetHashFromTextureFilename));
            }
        }

        #endregion
		
        #region Suggestions

        /// <summary>
        /// Suggestions for possible host device names
        /// </summary>
        [JsonIgnore]
        public SuggestionList<HostDevice> HostDeviceSuggestions
        {
            get
            {
                return _host_device_suggestions ??= new SuggestionList<HostDevice>((device) => device.Name, StaticData.Default.StartupSettings.GetAllHostDevicesNameSuggestions(4));
            }
        }
        private SuggestionList<HostDevice> _host_device_suggestions;


        /// <summary>
        /// Suggestions for possible emulated device names
        /// </summary>
        [JsonIgnore]
        public SuggestionList<EmulatedDevice> EmulatedDeviceSuggestions
        {
            get
            {
                return _emulated_device_suggestions ??= new SuggestionList<EmulatedDevice>((device) => device.Name, StaticData.Default.StartupSettings.GetAllEmulatedDeviceNameSuggestions(4));
            }
        }
        private SuggestionList<EmulatedDevice> _emulated_device_suggestions;

        #endregion

        #region Commands

        #region SelectedTexture

        [JsonIgnore]
        public ICommand DeleteSelectedRegionCommand => new RelayCommand<RectRegion>(Region => Textures.Selected.Regions.Remove(Region));

        [JsonIgnore]
        public ICommand ResetScaleFactorCommand => new RelayCommand((x) => Textures.Selected.SetInitialZoom());

        #endregion

        #region NewHostDevice

        [JsonIgnore]
        public VisibilityCommand InputNewHostDevice
        {
            get => _input_new_host_device ??= new VisibilityCommand(
                (input) => ((System.Windows.Controls.ComboBox)input).Text = HostDeviceSuggestions.GetUnusedSuggestion());
        }
        private VisibilityCommand _input_new_host_device;

        [JsonIgnore]
        public ICommand InputNewHostDeviceOKCommand => new RelayCommand<string>(OkNewHostDevice, StaticData.Default.StartupSettings.HostDevicesRegex.IsMatch);

        private void OkNewHostDevice(string name)
        {
            HostDevices.Add(new HostDevice { Name = name });
            InputNewHostDevice.SetToCollapsCommand.Execute(name);
        }

        #endregion

        #region NewEmulatedDevice

        [JsonIgnore]
        public VisibilityCommand InputNewEmulatedDevice
        {
            get => _input_new_emulated_device ??= new VisibilityCommand(
                (input) => ((System.Windows.Controls.ComboBox)input).Text = EmulatedDeviceSuggestions.GetUnusedSuggestion());
        }
        private VisibilityCommand _input_new_emulated_device;

        [JsonIgnore]
        public ICommand InputNewEmulatedDeviceOKCommand => new RelayCommand<string>(OkNewEmulatedDevice, StaticData.Default.StartupSettings.EmulatedDeviceRegex.IsMatch);

        private void OkNewEmulatedDevice(string name)
        {
            EmulatedDevices.Add(new EmulatedDevice
            {
                Name = name
            });
            InputNewEmulatedDevice.SetToCollapsCommand.Execute(name);
            SelectedRegionBrush.SelectedEmulatedDevice ??= EmulatedDevices.Selected;
        }
        #endregion

        #endregion

    }
}
