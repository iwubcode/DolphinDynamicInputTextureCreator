using DolphinDynamicInputTextureCreator.Other;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;

namespace DolphinDynamicInputTextureCreator.Data
{
    public class DynamicInputPack : Other.PropertyChangedBase
    {
        /// <summary>
        /// The current emulated device / key "brush" chosen for writing regions to the texture
        /// </summary>
        private RegionBrush _selected_region_brush = new RegionBrush();
        public RegionBrush SelectedRegionBrush
        {
            get
            {
                return _selected_region_brush;
            }
            set
            {
                _selected_region_brush = value;
                OnPropertyChanged(nameof(SelectedRegionBrush));
            }
        }

        #region GENERAL PROPERTIES
        private string _generated_folder_name = "";
        public string GeneratedFolderName
        {
            get
            {
                return _generated_folder_name;
            }
            set
            {
                _generated_folder_name = value;
                OnPropertyChanged(nameof(GeneratedFolderName));
            }
        }

        private bool _preserve_aspect_ratio = true;
        public bool PreserveAspectRatio
        {
            get
            {
                return _preserve_aspect_ratio;
            }
            set
            {
                _preserve_aspect_ratio = value;
                OnPropertyChanged(nameof(PreserveAspectRatio));
            }
        }
        #endregion

        #region DYNAMIC TEXTURE PROPERTIES
        /// <summary>
        /// The current texture being edited
        /// </summary>
        private DynamicInputTexture _editing_texture;
        [JsonIgnore]
        public DynamicInputTexture EditingTexture
        {
            get
            {
                return _editing_texture;
            }
            set
            {
                _editing_texture = value;
                OnPropertyChanged(nameof(EditingTexture));
                OnPropertyChanged(nameof(IsEditingTexture));
                OnPropertyChanged(nameof(IsEditingTextureHelpAvailable));
            }
        }

        [JsonIgnore]
        public bool IsEditingTexture
        {
            get
            {
                return EditingTexture != null;
            }
        }

        [JsonIgnore]
        public bool IsEditingTextureHelpAvailable
        {
            get
            {
                return EditingTexture == null;
            }
        }

        /// <summary>
        /// The textures defined for dynamic input modification in this pack
        /// </summary>
        private ObservableCollection<DynamicInputTexture> _textures = new ObservableCollection<DynamicInputTexture>();
        public ObservableCollection<DynamicInputTexture> Textures
        {
            get
            {
                return _textures;
            }
            set
            {
                _textures = value;
                OnPropertyChanged(nameof(Textures));
            }
        }

        /// <summary>
        /// When adding a new texture, whether the hash should be pulled off of the filename
        /// </summary>
        private bool _should_get_hash_from_texture_filename;
        public bool ShouldGetHashFromTextureFilename
        {
            get
            {
                return _should_get_hash_from_texture_filename;
            }
            set
            {
                _should_get_hash_from_texture_filename = value;
                OnPropertyChanged(nameof(ShouldGetHashFromTextureFilename));
            }
        }
        #endregion

        #region DYNAMIC TEXTURE COMMANDS
        private ICommand _delete_texture_command;

        [JsonIgnore]
        public ICommand DeleteTextureCommand
        {
            get
            {
                if (_delete_texture_command == null)
                {
                    _delete_texture_command = new RelayCommand(param => DeleteTexture((DynamicInputTexture)param));
                }
                return _delete_texture_command;
            }
        }

        private void DeleteTexture(DynamicInputTexture texture)
        {
            if (texture == EditingTexture)
            {
                EditingTexture = null;
            }
            Textures.Remove(texture);
        }

        private ICommand _edit_texture_command;
        [JsonIgnore]
        public ICommand EditTextureCommand
        {
            get
            {
                if (_edit_texture_command == null)
                {
                    _edit_texture_command = new RelayCommand(param => EditTexture((DynamicInputTexture)param));
                }
                return _edit_texture_command;
            }
        }

        private void EditTexture(DynamicInputTexture texture)
        {
            EditingTexture = texture;
        }
        #endregion

        #region HOST DEVICE PROPERTIES
        /// <summary>
        /// The host devices mapped in this pack
        /// </summary>
        private ObservableCollection<HostDevice> _host_devices = new ObservableCollection<HostDevice>();
        public ObservableCollection<HostDevice> HostDevices
        {
            get
            {
                return _host_devices;
            }
            set
            {
                _host_devices = value;
                OnPropertyChanged(nameof(HostDevices));
            }
        }

        /// <summary>
        /// The currently selected host device
        /// </summary>
        private HostDevice _selected_host_device;
        public HostDevice SelectedHostDevice
        {
            get { return _selected_host_device; }
            set
            {
                _selected_host_device = value;
                OnPropertyChanged(nameof(SelectedHostDevice));
                OnPropertyChanged(nameof(IsHostDeviceHelpVisible));
                OnPropertyChanged(nameof(AreHostDeviceDetailsVisible));
            }
        }

        /// <summary>
        /// Whether the help text is visible
        /// </summary>
        [JsonIgnore]
        public bool IsHostDeviceHelpVisible
        {
            get
            {
                return SelectedHostDevice == null;
            }
        }

        /// <summary>
        /// Whether the host device details are visible
        /// </summary>
        [JsonIgnore]
        public bool AreHostDeviceDetailsVisible
        {
            get
            {
                return SelectedHostDevice != null;
            }
        }

        /// <summary>
        /// Says whether a new host device entry is being edited
        /// </summary>
        private bool _is_editing_new_host_device_name;
        [JsonIgnore]
        public bool IsEditingNewHostDeviceName
        {
            get
            {
                return _is_editing_new_host_device_name;
            }
            set
            {
                _is_editing_new_host_device_name = value;
                OnPropertyChanged(nameof(IsEditingNewHostDeviceName));
            }
        }

        /// <summary>
        /// The new host device entry input for the name
        /// </summary>
        private string _new_host_device_input;
        [JsonIgnore]
        public string NewHostDeviceInput
        {
            get
            {
                return _new_host_device_input;
            }
            set
            {
                _new_host_device_input = value;
                OnPropertyChanged(nameof(NewHostDeviceInput));
                OnPropertyChanged(nameof(HasNewHostDeviceInput));
            }
        }

        /// <summary>
        /// Whether the new host device entry input is available
        /// </summary>
        /// 
        [JsonIgnore]
        public bool HasNewHostDeviceInput
        {
            get
            {
                if (NewHostDeviceInput == null)
                {
                    return false;
                }

                return NewHostDeviceInput.Length > 0;
            }
        }

        /// <summary>
        /// Suggestions for possible host device names
        /// </summary>
        private ObservableCollection<string> _host_device_suggestions = new ObservableCollection<string>();
        [JsonIgnore]
        public ObservableCollection<string> HostDeviceSuggestions
        {
            get
            {
                return _host_device_suggestions;
            }
            set
            {
                _host_device_suggestions = value;
                OnPropertyChanged(nameof(HostDeviceSuggestions));
            }
        }
        #endregion

        #region HOST DEVICE COMMANDS
        private ICommand _add_host_device_command;
        [JsonIgnore]
        public ICommand AddHostDeviceCommand
        {
            get
            {
                if (_add_host_device_command == null)
                {
                    _add_host_device_command = new RelayCommand(AddHostDevice);
                }
                return _add_host_device_command;
            }
        }

        private void AddHostDevice(object obj)
        {
            IsEditingNewHostDeviceName = true;
        }

        private bool CanDeleteHostDevice
        {
            get { return SelectedHostDevice != null; }
        }

        private ICommand _delete_host_device_command;
        [JsonIgnore]
        public ICommand DeleteHostDeviceCommand
        {
            get
            {
                if (_delete_host_device_command == null)
                {
                    _delete_host_device_command = new RelayCommand(param => DeleteHostDevice((HostDevice)param), param => CanDeleteHostDevice);
                }
                return _delete_host_device_command;
            }
        }

        private void DeleteHostDevice(HostDevice device)
        {
            int i = HostDevices.IndexOf(device);
            HostDevices.Remove(device);

            if (HostDevices.Count == 0)
            {
                SelectedHostDevice = null;
            }
            else
            {
                if (i == 0)
                {
                    SelectedHostDevice = HostDevices[0];
                }
                else
                {
                    SelectedHostDevice = HostDevices[i - 1];
                }
            }
        }

        private ICommand _ok_new_host_device_command;
        [JsonIgnore]
        public ICommand OkNewHostDeviceCommand
        {
            get
            {
                if (_ok_new_host_device_command == null)
                {
                    _ok_new_host_device_command = new RelayCommand(OkNewHostDevice);
                }
                return _ok_new_host_device_command;
            }
        }

        private void OkNewHostDevice(object obj)
        {
            HostDevices.Add(new HostDevice
            {
                Name = NewHostDeviceInput
            });
            NewHostDeviceInput = "";
            SelectedHostDevice = HostDevices[HostDevices.Count - 1];
            IsEditingNewHostDeviceName = false;
        }

        private ICommand _cancel_new_host_device_command;
        [JsonIgnore]
        public ICommand CancelNewHostDeviceCommand
        {
            get
            {
                if (_cancel_new_host_device_command == null)
                {
                    _cancel_new_host_device_command = new RelayCommand(CancelNewHostDevice);
                }
                return _cancel_new_host_device_command;
            }
        }

        private void CancelNewHostDevice(object obj)
        {
            NewHostDeviceInput = "";
            IsEditingNewHostDeviceName = false;
        }
        #endregion

        #region EMULATED DEVICE PROPERTIES

        /// <summary>
        /// The emulated devices mapped in this pack
        /// </summary>
        private ObservableCollection<EmulatedDevice> _emulated_devices = new ObservableCollection<EmulatedDevice>();
        public ObservableCollection<EmulatedDevice> EmulatedDevices
        {
            get
            {
                return _emulated_devices;
            }
            set
            {
                _emulated_devices = value;
                OnPropertyChanged(nameof(EmulatedDevices));
            }
        }

        /// <summary>
        /// The currently selected emulated device
        /// </summary>
        private EmulatedDevice _selected_emulated_device;
        public EmulatedDevice SelectedEmulatedDevice
        {
            get { return _selected_emulated_device; }
            set
            {
                _selected_emulated_device = value;
                OnPropertyChanged(nameof(SelectedEmulatedDevice));
                OnPropertyChanged(nameof(IsEmulatedDeviceHelpVisible));
                OnPropertyChanged(nameof(AreEmulatedDeviceDetailsVisible));
            }
        }

        /// <summary>
        /// Whether the emulated device help is visible
        /// </summary>
        /// 
        [JsonIgnore]
        public bool IsEmulatedDeviceHelpVisible
        {
            get
            {
                return SelectedEmulatedDevice == null;
            }
        }

        /// <summary>
        /// Whether the emulated device details are visible
        /// </summary>
        /// 
        [JsonIgnore]
        public bool AreEmulatedDeviceDetailsVisible
        {
            get
            {
                return SelectedEmulatedDevice != null;
            }
        }

        /// <summary>
        /// Returns true if we are editing a new emulated device entry name
        /// </summary>
        private bool _is_editing_new_emulated_device_name;
        [JsonIgnore]
        public bool IsEditingNewEmulatedDeviceName
        {
            get
            {
                return _is_editing_new_emulated_device_name;
            }
            set
            {
                _is_editing_new_emulated_device_name = value;
                OnPropertyChanged(nameof(IsEditingNewEmulatedDeviceName));
            }
        }

        /// <summary>
        /// Returns the current input for the new emulated device name
        /// </summary>
        private string _new_emulated_device_input;
        [JsonIgnore]
        public string NewEmulatedDeviceInput
        {
            get
            {
                return _new_emulated_device_input;
            }
            set
            {
                _new_emulated_device_input = value;
                OnPropertyChanged(nameof(NewEmulatedDeviceInput));
                OnPropertyChanged(nameof(HasNewEmulatedDeviceInput));
            }
        }

        /// <summary>
        /// Returns true if the emulated device input has text
        /// </summary>
        /// 
        [JsonIgnore]
        public bool HasNewEmulatedDeviceInput
        {
            get
            {
                if (NewEmulatedDeviceInput == null)
                {
                    return false;
                }

                return NewEmulatedDeviceInput.Length > 0;
            }
        }

        /// <summary>
        /// A list of emulated device name suggestions
        /// </summary>
        private ObservableCollection<string> _emulated_device_suggestions = new ObservableCollection<string>();
        [JsonIgnore]
        public ObservableCollection<string> EmulatedDeviceSuggestions
        {
            get
            {
                return _emulated_device_suggestions;
            }
            set
            {
                _emulated_device_suggestions = value;
                OnPropertyChanged(nameof(EmulatedDeviceSuggestions));
            }
        }
        #endregion

        #region EMULATED DEVICE COMMANDS
        private ICommand _add_emulated_device_command;
        [JsonIgnore]
        public ICommand AddEmulatedDeviceCommand
        {
            get
            {
                if (_add_emulated_device_command == null)
                {
                    _add_emulated_device_command = new RelayCommand(AddEmulatedDevice);
                }
                return _add_emulated_device_command;
            }
        }

        private void AddEmulatedDevice(object obj)
        {
            IsEditingNewEmulatedDeviceName = true;
        }

        private bool CanDeleteEmulatedtDevice
        {
            get { return SelectedEmulatedDevice != null; }
        }

        private ICommand _delete_emulated_device_command;
        [JsonIgnore]
        public ICommand DeleteEmulatedDeviceCommand
        {
            get
            {
                if (_delete_emulated_device_command == null)
                {
                    _delete_emulated_device_command = new RelayCommand(param => DeleteEmulatedDevice((EmulatedDevice)param), param => CanDeleteEmulatedtDevice);
                }
                return _delete_emulated_device_command;
            }
        }

        private void DeleteEmulatedDevice(EmulatedDevice device)
        {
            int i = EmulatedDevices.IndexOf(device);
            EmulatedDevices.Remove(device);

            if (EmulatedDevices.Count == 0)
            {
                SelectedEmulatedDevice = null;
            }
            else
            {
                if (i == 0)
                {
                    SelectedEmulatedDevice = EmulatedDevices[0];
                }
                else
                {
                    SelectedEmulatedDevice = EmulatedDevices[i - 1];
                }
            }

            SelectedRegionBrush.SelectedEmulatedDevice = SelectedEmulatedDevice;
            if (SelectedRegionBrush.SelectedEmulatedDevice == null)
            {
                SelectedRegionBrush.SelectedEmulatedKey = null;
            }
        }

        private ICommand _ok_new_emulated_device_command;
        [JsonIgnore]
        public ICommand OkNewEmulatedDeviceCommand
        {
            get
            {
                if (_ok_new_emulated_device_command == null)
                {
                    _ok_new_emulated_device_command = new RelayCommand(OkNewEmulatedDevice);
                }
                return _ok_new_emulated_device_command;
            }
        }

        private void OkNewEmulatedDevice(object obj)
        {
            bool was_empty = EmulatedDevices.Count == 0;
            EmulatedDevices.Add(new EmulatedDevice
            {
                Name = NewEmulatedDeviceInput
            });
            NewEmulatedDeviceInput = "";
            SelectedEmulatedDevice = EmulatedDevices[EmulatedDevices.Count - 1];
            IsEditingNewEmulatedDeviceName = false;

            if (was_empty)
            {
                SelectedRegionBrush.SelectedEmulatedDevice = SelectedEmulatedDevice;
            }
        }

        private ICommand _cancel_new_emulated_device_command;
        [JsonIgnore]
        public ICommand CancelNewEmulatedDeviceCommand
        {
            get
            {
                if (_cancel_new_emulated_device_command == null)
                {
                    _cancel_new_emulated_device_command = new RelayCommand(CancelEmulatedHostDevice);
                }
                return _cancel_new_emulated_device_command;
            }
        }

        private void CancelEmulatedHostDevice(object obj)
        {
            NewEmulatedDeviceInput = "";
            IsEditingNewEmulatedDeviceName = false;
        }
        #endregion

        #region CONSTRUCTOR
        public DynamicInputPack()
        {
            List<string> host_device_suggestions = new List<string>();
            host_device_suggestions.Add("XInput/0/Gamepad");
            host_device_suggestions.Add("XInput/1/Gamepad");
            host_device_suggestions.Add("XInput/2/Gamepad");
            host_device_suggestions.Add("XInput/3/Gamepad");

            host_device_suggestions.Add("DInput/0/Keyboard Mouse");
            host_device_suggestions.Sort();
            HostDeviceSuggestions = new ObservableCollection<string>(host_device_suggestions);

            List<string> emulated_device_suggestions = new List<string>();
            emulated_device_suggestions.Add("Wiimote1");
            emulated_device_suggestions.Add("Wiimote2");
            emulated_device_suggestions.Add("Wiimote3");
            emulated_device_suggestions.Add("Wiimote4");

            emulated_device_suggestions.Add("GCPad1");
            emulated_device_suggestions.Add("GCPad2");
            emulated_device_suggestions.Add("GCPad3");
            emulated_device_suggestions.Add("GCPad4");

            emulated_device_suggestions.Sort();
            EmulatedDeviceSuggestions = new ObservableCollection<string>(emulated_device_suggestions);

            EditingTexture = null;
        }
        #endregion

        public void OutputToLocation(string location)
        {
            WriteJson(Path.Combine(location, "output.json"));
            WriteImages(location);
        }

        #region JSON WRITER HELPERS
        private string GetImageName(string texture_name)
        {
            string name = Path.GetFileNameWithoutExtension(texture_name);
            string extension = Path.GetExtension(texture_name);
            return name + extension;
        }

        private string GetHostDeviceKeyTextureLocation(string root_location, HostDevice device, HostKey key)
        {
            string device_location = Path.Combine(root_location, device.Name.Replace("/", "_"));
            return Path.Combine(device_location, Path.GetFileName(key.TexturePath));
        }
        #endregion

        private void WriteImages(string location)
        {
            foreach (DynamicInputTexture texture in _textures)
            {
                string destImagePath = Path.Combine(location, GetImageName(texture.TexturePath));

                // Unlikely that we get here but skip textures that don't exist
                if (!File.Exists(texture.TexturePath))
                {
                    continue;
                }

                // Prevents the file from trying to overwrite itself.
                if (texture.TexturePath == destImagePath)
                {
                    continue;
                }

                const bool overwrite = true;
                File.Copy(texture.TexturePath, destImagePath, overwrite);
            }

            foreach (var device in _host_devices)
            {
                foreach (var key in device.HostKeys)
                {
                    const bool overwrite = true;
                    var texture_location = GetHostDeviceKeyTextureLocation(location, device, key);
                    Directory.CreateDirectory(Path.GetDirectoryName(texture_location));
                    File.Copy(key.TexturePath, texture_location, overwrite);
                }
            }
        }

        private void WriteJson(string file)
        {
            using (FileStream fs = File.Open(file, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();

                #region GENERAL PROPERTIES
                if (GeneratedFolderName.Length > 0)
                {
                    writer.WritePropertyName("generated_folder_name");
                    writer.WriteValue(GeneratedFolderName);
                }

                writer.WritePropertyName("preserve_aspect_ratio");
                writer.WriteValue(PreserveAspectRatio);
                #endregion

                #region OUTPUT TEXTURES
                writer.WritePropertyName("output_textures");
                writer.WriteStartObject();
                foreach (DynamicInputTexture texture in _textures)
                {
                    // Unlikely that we get here but skip textures that don't exist
                    if (!File.Exists(texture.TexturePath))
                    {
                        continue;
                    }

                    writer.WritePropertyName(texture.TextureHash);
                    writer.WriteStartObject();

                    writer.WritePropertyName("image");
                    writer.WriteValue(GetImageName(texture.TexturePath));

                    #region EMULATED KEYS
                    writer.WritePropertyName("emulated_controls");
                    writer.WriteStartObject();
                    var device_to_keys_to_regions = CollectRegionsForDevices(texture);
                    foreach (var pair in device_to_keys_to_regions)
                    {
                        // Skip devices with no mapped keys
                        if (pair.Value.Count == 0)
                        {
                            continue;
                        }

                        writer.WritePropertyName(pair.Key);
                        writer.WriteStartObject();

                        foreach (var keys_to_regions in pair.Value)
                        {
                            writer.WritePropertyName(keys_to_regions.Key);

                            if (keys_to_regions.Value.Count > 0)
                            {
                                writer.WriteStartArray();
                                foreach (var region in keys_to_regions.Value)
                                {
                                    writer.WriteStartArray();
                                    writer.WriteValue(region.X);
                                    writer.WriteValue(region.Y);
                                    writer.WriteValue(region.X + region.Width);
                                    writer.WriteValue(region.Y + region.Height);
                                    writer.WriteEndArray();
                                }
                                writer.WriteEndArray();
                            }
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                    #endregion

                    #region HOST    
                    // Only create if devices are assigned.   
                    if (_host_devices.Count != 0)
                    {
                        writer.WritePropertyName("host_controls");
                        writer.WriteStartObject();
                        foreach (var device in _host_devices)
                        {
                            // Skip devices with no mapped keys
                            if (device.HostKeys.Count == 0)
                            {
                                continue;
                            }

                            writer.WritePropertyName(device.Name);
                            writer.WriteStartObject();
                            foreach (var key in device.HostKeys)
                            {
                                writer.WritePropertyName(key.Name);
                                writer.WriteValue(GetHostDeviceKeyTextureLocation("", device, key));
                            }
                            writer.WriteEndObject();
                        }
                        writer.WriteEndObject();
                    }
                    #endregion

                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
                #endregion

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Given a texture will return a mapping between each emulated device name and map of key name to list of regions
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        private Dictionary<string, Dictionary<string, List<RectRegion>>> CollectRegionsForDevices(DynamicInputTexture texture)
        {
            Dictionary<string, Dictionary<string, List<RectRegion>>> result = new Dictionary<string, Dictionary<string, List<RectRegion>>>();
            foreach (var region in texture.Regions)
            {
                if (result.ContainsKey(region.Device.Name))
                {
                    var key_to_regions = result[region.Device.Name];
                    if (key_to_regions.ContainsKey(region.Key.Name))
                    {
                        key_to_regions[region.Key.Name].Add(region);
                    }
                    else
                    {
                        key_to_regions.Add(region.Key.Name, new List<RectRegion> { region });
                    }
                }
                else
                {
                    var key_to_regions = new Dictionary<string, List<RectRegion>>();
                    key_to_regions.Add(region.Key.Name, new List<RectRegion> { region });
                    result.Add(region.Device.Name, key_to_regions);
                }
            }

            return result;
        }
    }
}
