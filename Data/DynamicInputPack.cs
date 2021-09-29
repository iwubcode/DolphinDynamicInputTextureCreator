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

        private string _generated_json_name = "output";
        public string GeneratedJsonName
        {
            get
            {
                return _generated_json_name;
            }
            set
            {
                _generated_json_name = value;
                OnPropertyChanged(nameof(GeneratedJsonName));
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

        private string _game_id = "";
        public string GameID
        {
            get
            {
                return _game_id;
            }
            set
            {
                if (value.Length <= 6)
                {
                    _game_id = value.ToUpper();
                }

                OnPropertyChanged(nameof(GameID));
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
        #region Export

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
        public void OutputToLocation(string location)
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
            WriteJson(Path.Combine(location, GeneratedJsonName + ".json"));
            WriteImages(location);
            WriteGameID(location);
        }

        /// <summary>
        /// Returns true if the emulated device input has text
        /// creates a GameID.txt so that the pack is recognized.
        /// </summary>
        /// 
        [JsonIgnore]
        public bool HasNewEmulatedDeviceInput
        private void WriteGameID(string location)
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
            if (GameID.Length < 3) return;

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
            location = Path.Combine(location, "GameID");
            Directory.CreateDirectory(location);
            File.Create(Path.Combine(location, GameID + ".txt")).Dispose();
        }
        #endregion

        #region EMULATED DEVICE COMMANDS
        private ICommand _add_emulated_device_command;
        [JsonIgnore]
        public ICommand AddEmulatedDeviceCommand
        private void WriteImages(string location)
        {
            get
            foreach (HostDevice device in HostDevices)
            {
                if (_add_emulated_device_command == null)
                foreach (HostKey key in device.HostKeys)
                {
                    _add_emulated_device_command = new RelayCommand(AddEmulatedDevice);
                    //exports the images for the default_host_controls
                    WriteImage(location, key);
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
            foreach (DynamicInputTexture texture in Textures)
            {
                if (_delete_emulated_device_command == null)
                //exports the images for the output_textures
                WriteImage(location, texture);
                foreach (HostDevice device in texture.HostDevices)
                {
                    _delete_emulated_device_command = new RelayCommand(param => DeleteEmulatedDevice((EmulatedDevice)param), param => CanDeleteEmulatedtDevice);
                    foreach (HostKey key in device.HostKeys)
                    {
                        //exports the images for the host_controls
                        WriteImage(location, key);
                    }
                }
                return _delete_emulated_device_command;
            }
        }

        private void DeleteEmulatedDevice(EmulatedDevice device)
        private void WriteImage(string location, Interfaces.IExportableImage image)
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
            // Unlikely that we get here but skip textures that don't exist
            if (!File.Exists(image.TexturePath))
                return;

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
            //check the output Path
            image.RelativeTexturePath = CheckRelativeTexturePath(image);
            string output_location = Path.Combine(location, image.RelativeTexturePath);

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
            // Prevents the file from trying to overwrite itself.
            if (output_location == image.TexturePath)
                return;

            if (was_empty)
            {
                SelectedRegionBrush.SelectedEmulatedDevice = SelectedEmulatedDevice;
            }
            //write the image
            const bool overwrite = true;
            Directory.CreateDirectory(Path.GetDirectoryName(output_location));
            File.Copy(image.TexturePath, output_location, overwrite);
        }

        private ICommand _cancel_new_emulated_device_command;
        [JsonIgnore]
        public ICommand CancelNewEmulatedDeviceCommand
        #region JSON WRITER HELPERS
        private string CheckRelativeTexturePath(Interfaces.IExportableImage image, HostDevice device, DynamicInputTexture texture = null)
        {
            get
            string relativepath = device.Name.Replace("/", "_");
            if (texture != null)
            {
                if (_cancel_new_emulated_device_command == null)
                {
                    _cancel_new_emulated_device_command = new RelayCommand(CancelEmulatedHostDevice);
                }
                return _cancel_new_emulated_device_command;
                string texturename = Path.GetFileNameWithoutExtension(texture.TextureHash);
                relativepath = Path.Combine(texturename, relativepath);
            }
            return CheckRelativeTexturePath(image, relativepath);
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
            host_device_suggestions.Add("Bluetooth/0/Wii Remote");
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

            emulated_device_suggestions.Add("GBA1");
            emulated_device_suggestions.Add("GBA2");
            emulated_device_suggestions.Add("GBA3");
            emulated_device_suggestions.Add("GBA4");

            //emulated_device_suggestions.Sort(); is more enjoyable without :)
            EmulatedDeviceSuggestions = new ObservableCollection<string>(emulated_device_suggestions);

            EditingTexture = null;
        }
        #endregion
        private string CheckRelativeTexturePath(Interfaces.IExportableImage image, string relativepath = "")
        {
            if (image.RelativeTexturePath != null)
                return image.RelativeTexturePath;

            return Path.Combine(relativepath, Path.GetFileName(image.TexturePath));
        }

        private void WriteHostControls(JsonWriter writer, IList<HostDevice> hostDevices, DynamicInputTexture texture = null)
        {
            writer.WriteStartObject();
            foreach (var device in hostDevices)
            {
                // Skip devices with no mapped keys
                if (device.HostKeys.Count == 0)
                    continue;

                writer.WritePropertyName(device.Name);
                writer.WriteStartObject();
                foreach (var key in device.HostKeys)
                {
                    writer.WritePropertyName(key.Name);
                    key.RelativeTexturePath = CheckRelativeTexturePath(key, device, texture);
                    writer.WriteValue(key.RelativeTexturePath);
                }
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }
        #endregion

        private void WriteJson(string file)
        {
            using (FileStream fs = File.Open(file, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();

                #region DefaultHOST    
                // Only create if devices are assigned.   
                if (HostDevices.Count != 0)
                {
                    writer.WritePropertyName("default_host_controls");
                    WriteHostControls(writer, HostDevices);
                }
                #endregion

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
                foreach (DynamicInputTexture texture in Textures)
                {
                    // Unlikely that we get here but skip textures that don't exist
                    if (!File.Exists(texture.TexturePath))
                    {
                        continue;
                    }

                    writer.WritePropertyName(texture.TextureHash);
                    writer.WriteStartObject();

                    writer.WritePropertyName("image");
                    writer.WriteValue(CheckRelativeTexturePath(texture));

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
                    if (texture.HostDevices.Count != 0)
                    {
                        writer.WritePropertyName("host_controls");
                        WriteHostControls(writer, texture.HostDevices, texture);
                    }
                    #endregion

                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
                #endregion

                writer.WriteEndObject();
            }
        }
        #endregion

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
