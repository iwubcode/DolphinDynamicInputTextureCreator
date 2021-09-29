using DolphinDynamicInputTextureCreator.Other;
using DolphinDynamicInputTextureCreator.ViewModels.Commands;
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

        #region GENERAL PROPERTIES

        /// <summary>
        /// The host devices mapped in this pack
        /// </summary>
        public ObservableCollection<HostDevice> HostDevices
        {
            get => _host_devices;
            set
            {
                _host_devices = value;
                OnPropertyChanged(nameof(HostDevices));
            }
        }
        private ObservableCollection<HostDevice> _host_devices = new ObservableCollection<HostDevice>();

        /// <summary>
        /// The textures defined for dynamic input modification in this pack
        /// </summary>
        public ObservableCollection<DynamicInputTexture> Textures
        {
            get => _textures;
            set
            {
                _textures = value;
                OnPropertyChanged(nameof(Textures));
            }
        }
        private ObservableCollection<DynamicInputTexture> _textures = new ObservableCollection<DynamicInputTexture>();

        /// <summary>
        /// specifies the name of the json file, which is generated during export.
        /// </summary>
        public string GeneratedJsonName
        {
            get => _generated_json_name;
            set
            {
                _generated_json_name = value;
                OnPropertyChanged(nameof(GeneratedJsonName));
            }
        }
        private string _generated_json_name = "output";

        /// <summary>
        /// the name of the folder that will be created by dolphin when using the package
        /// </summary>
        public string GeneratedFolderName
        {
            get => _generated_folder_name;
            set
            {
                _generated_folder_name = value;
                OnPropertyChanged(nameof(GeneratedFolderName));
            }
        }
        private string _generated_folder_name = "";

        /// <summary>
        /// indicates whether the exchanged textures keep their aspect ratio.
        /// </summary>
        public bool PreserveAspectRatio
        {
            get => _preserve_aspect_ratio;
            set
            {
                _preserve_aspect_ratio = value;
                OnPropertyChanged(nameof(PreserveAspectRatio));
            }
        }
        private bool _preserve_aspect_ratio = true;

        /// <summary>
        /// Suggestions for possible host device names
        /// The game id used by dolphin
        /// </summary>
        private ObservableCollection<string> _host_device_suggestions = new ObservableCollection<string>();
        [JsonIgnore]
        public ObservableCollection<string> HostDeviceSuggestions
        public string GameID
        {
            get
            {
                return _host_device_suggestions;
            }
            get => _game_id;
            set
            {
                _host_device_suggestions = value;
                OnPropertyChanged(nameof(HostDeviceSuggestions));
            }
        }
        #endregion
                if (value.Length <= 6)
                {
                    _game_id = value.ToUpper();
                }

                OnPropertyChanged(nameof(GameID));
            }
        }
        private string _game_id = "";

        #endregion

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
        #region Export

        public void OutputToLocation(string location)
        {
            WriteJson(Path.Combine(location, GeneratedJsonName + ".json"));
            WriteImages(location);
            WriteGameID(location);
        }

        /// <summary>
        /// creates a GameID.txt so that the pack is recognized.
        /// </summary>
        private void WriteGameID(string location)
        {
            if (GameID.Length < 3) return;

            location = Path.Combine(location, "GameID");
            Directory.CreateDirectory(location);
            File.Create(Path.Combine(location, GameID + ".txt")).Dispose();
        }

        private void WriteImages(string location)
        {
            foreach (HostDevice device in HostDevices)
            {
                foreach (HostKey key in device.HostKeys)
                {
                    //exports the images for the default_host_controls
                    WriteImage(location, key);
                }
            }

            foreach (DynamicInputTexture texture in Textures)
            {
                //exports the images for the output_textures
                WriteImage(location, texture);
                foreach (HostDevice device in texture.HostDevices)
                {
                    foreach (HostKey key in device.HostKeys)
                    {
                        //exports the images for the host_controls
                        WriteImage(location, key);
                    }
                }
            }
        }

        private void WriteImage(string location, Interfaces.IExportableImage image)
        {
            // Unlikely that we get here but skip textures that don't exist
            if (!File.Exists(image.TexturePath))
                return;

            //check the output Path
            image.RelativeTexturePath = CheckRelativeTexturePath(image);
            string output_location = Path.Combine(location, image.RelativeTexturePath);

            // Prevents the file from trying to overwrite itself.
            if (output_location == image.TexturePath)
                return;

            //write the image
            const bool overwrite = true;
            Directory.CreateDirectory(Path.GetDirectoryName(output_location));
            File.Copy(image.TexturePath, output_location, overwrite);
        }

        #region JSON WRITER HELPERS
        private string CheckRelativeTexturePath(Interfaces.IExportableImage image, HostDevice device, DynamicInputTexture texture = null)
        {
            string relativepath = device.Name.Replace("/", "_");
            if (texture != null)
            {
                string texturename = Path.GetFileNameWithoutExtension(texture.TextureHash);
                relativepath = Path.Combine(texturename, relativepath);
            }
            return CheckRelativeTexturePath(image, relativepath);
        }


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
