using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
//using System.Windows.Media;

namespace DolphinDynamicInputTexture.Data
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
        /// The game id used by dolphin
        /// </summary>
        public string GameID
        {
            get => _game_id;
            set
            {
                if (value.Length <= 6)
                {
                    _game_id = value.ToUpper();
                }

                OnPropertyChanged(nameof(GameID));
            }
        }
        private string _game_id = "";

        #endregion

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

        private void WriteImage(string location, Interfaces.IImage image)
        {
            // Unlikely that we get here but skip textures that don't exist
            if (!File.Exists(image.TexturePath))
                return;

            //check the output Path
            image.RelativeTexturePath = CheckRelativeTexturePath(image);
            string output_location = Path.Combine(location, image.RelativeTexturePath);

            // Prevents the file from trying to overwrite itself.
            if (Path.GetFullPath(output_location) == Path.GetFullPath(image.TexturePath))
                return;

            //write the image
            const bool overwrite = true;
            Directory.CreateDirectory(Path.GetDirectoryName(output_location));
            File.Copy(image.TexturePath, output_location, overwrite);
        }

        #region JSON WRITER HELPERS
        private string CheckRelativeTexturePath(Interfaces.IImage image, HostDevice device, DynamicInputTexture texture = null)
        {
            string relativepath = device.Name.Replace("/", "_");
            if (texture != null)
            {
                string texturename = Path.GetFileNameWithoutExtension(texture.TextureHash);
                relativepath = Path.Combine(texturename, relativepath);
            }
            return CheckRelativeTexturePath(image, relativepath);
        }

        private string CheckRelativeTexturePath(Interfaces.IImage image, string relativepath = "")
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

        #region Checks

        /// <summary>
        /// Check if all images exist.
        /// Calls DynamicInputTextureEvents.ImageNotExist
        /// </summary>
        /// <returns>true = Error-free</returns>
        public bool CheckImagePaths()
        {
            foreach (HostDevice device in HostDevices)
            {
                if (!CheckTexturePathes(device.HostKeys, device.Name))
                    return false;
            }

            if (!CheckTexturePathes(Textures))
                return false;

            foreach (DynamicInputTexture texture in Textures)
            {
                foreach (HostDevice device in texture.HostDevices)
                {
                    if (!CheckTexturePathes(device.HostKeys, $"{texture.TextureHash} {device.Name}"))
                        return false;
                }
            }
            return true;
        }

        private bool CheckTexturePathes<T>(IList<T> imagelist, string details = "") where T : Interfaces.IImage
        {
            foreach (var image in imagelist)
            {
                if (!File.Exists(image.TexturePath))
                {
                    if (DynamicInputTextureEvents.ImageNotExist == null || !DynamicInputTextureEvents.ImageNotExist(image, details != "" ? details : Path.GetFileName(image.TexturePath) ))
                        return false;
                }
            }
            return true;
        }

        #endregion
		
        #region Import

        #region JSON Read HELPERS
        private void ReadHostControls(JsonReader reader, string path, IList<HostDevice> hostDevices)
        {
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    HostDevice Device = new HostDevice { Name = reader.Value.ToString() };

                    reader.Read();
                    switch (reader.TokenType)
                    {
                        case JsonToken.StartObject: // V1
                            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                            {
                                if (reader.TokenType == JsonToken.PropertyName)
                                {
                                    HostKey key = new HostKey();
                                    key.Name = reader.Value.ToString();
                                    key.RelativeTexturePath = reader.ReadAsString();
                                    key.TexturePath = Path.Combine(path, key.RelativeTexturePath);
                                    Device.HostKeys.Add(key);
                                }
                            }
                            break;
                        case JsonToken.StartArray: // V2
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    ReadV2Hostkey(reader, path, Device);
                                }
                            }
                            break;
                        default:
                            return;
                    }
                    hostDevices.Add(Device);
                }
            }
        }

        private void ReadV2Hostkey(JsonReader reader, string path, HostDevice device)
        {
            HostKey key = new HostKey();
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                switch (reader.Value)
                {
                    case "keys":
                        if (!reader.Read() || reader.TokenType != JsonToken.StartArray)
                            break;

                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                        {
                            //multi key not implemented
                            if (key.Name == null)
                                key.Name = reader.Value.ToString();
                        }
                        break;
                    case "image":
                        key.RelativeTexturePath = reader.ReadAsString();
                        key.TexturePath = Path.Combine(path, key.RelativeTexturePath);
                        break;
                    case "tag":
                        //not implemented
                        return;
                }
            }
            device.HostKeys.Add(key);
        }

        private void ReadEmulatedControls(JsonReader reader, DynamicInputTexture texture)
        {
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    EmulatedDevice emulatedDevice = new EmulatedDevice { Name = reader.Value.ToString() };

                    reader.Read();
                    switch (reader.TokenType)
                    {
                        case JsonToken.StartObject: // V1
                            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                            {
                                if (reader.TokenType == JsonToken.PropertyName)
                                {
                                    EmulatedKey emulatedkey = new EmulatedKey { Name = reader.Value.ToString() };
                                    if (!emulatedDevice.EmulatedKeys.Contains(emulatedkey))
                                        emulatedDevice.EmulatedKeys.Add(emulatedkey);

                                    if (reader.Read() && reader.TokenType == JsonToken.StartArray)
                                    {
                                        while (reader.Read() && reader.TokenType == JsonToken.StartArray)
                                        {
                                            RectRegion Region = new RectRegion { Device = emulatedDevice, Key = emulatedkey, OwnedTexture = texture, ScaleFactor = 1 };
                                            if (ReadRectRegion(reader, Region))
                                            {
                                                texture.Regions.Add(Region);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case JsonToken.StartArray: // V2
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                ReadV2EmulatedKey(reader, emulatedDevice, texture);
                            }
                            break;
                    }
                }
            }
        }

        private bool ReadRectRegion(JsonReader reader, RectRegion Region)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                try
                {
                    Region.X = reader.ReadAsDouble().Value;
                    Region.Y = reader.ReadAsDouble().Value;
                    Region.Width = reader.ReadAsDouble().Value - Region.X;
                    Region.Height = reader.ReadAsDouble().Value - Region.Y;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                finally
                {
                    while (reader.TokenType != JsonToken.EndArray && reader.Read()) { }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ReadV2EmulatedKey(JsonReader reader, EmulatedDevice emulatedDevice, DynamicInputTexture texture)
        {
            EmulatedKey emulatedkey = new EmulatedKey();
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                switch (reader.Value)
                {
                    case "key":
                        emulatedkey.Name = reader.ReadAsString();
                        break;
                    case "region":
                        RectRegion Region = new RectRegion { Device = emulatedDevice, Key = emulatedkey, OwnedTexture = texture, ScaleFactor = 1 };
                        if (reader.Read() && ReadRectRegion(reader, Region))
                        {
                            texture.Regions.Add(Region);
                        }
                        break;
                    case "tag":
                        //not implemented
                        break;
                    case "bind_type":
                        //not implemented
                        break;
                    case "copy_type":
                        //not implemented
                        break;
                    case "sub_entries":
                        //not implemented
                        break;
                }
            }
        }

        #endregion

        public void ImportFromLocation(string file) => ReadJson(file);
        private void ReadJson(string file)
        {
            string path = Path.GetDirectoryName(file);
            GeneratedJsonName = Path.GetFileNameWithoutExtension(file);
            using (FileStream fs = File.Open(file, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader Reader = new JsonTextReader(sr))
            {
                while (Reader.Read())
                {
                    if (Reader.TokenType == JsonToken.PropertyName)
                    {
                        switch (Reader.Value)
                        {
                            case "specification":
                                break;
                            case "default_host_controls":
                                ReadHostControls(Reader, path, HostDevices);
                                break;
                            case "generated_folder_name":
                                GeneratedFolderName = Reader.ReadAsString();
                                break;
                            case "preserve_aspect_ratio":
                                PreserveAspectRatio = Reader.ReadAsBoolean().Value;
                                break;
                            case "output_textures":
                                while (Reader.Read() && Reader.TokenType != JsonToken.EndObject)
                                {
                                    if (Reader.TokenType == JsonToken.PropertyName)
                                    {
                                        DynamicInputTexture texture = new DynamicInputTexture { TextureHash = Reader.Value.ToString() };
                                        while (Reader.Read() && Reader.TokenType != JsonToken.EndObject)
                                        {
                                            if (Reader.TokenType == JsonToken.PropertyName)
                                            {
                                                switch (Reader.Value)
                                                {
                                                    case "image":
                                                        texture.RelativeTexturePath = Reader.ReadAsString();
                                                        texture.TexturePath = Path.Combine(path, texture.RelativeTexturePath);
                                                        break;
                                                    case "host_controls":
                                                        ReadHostControls(Reader, path, texture.HostDevices);
                                                        break;
                                                    case "emulated_controls":
                                                        ReadEmulatedControls(Reader, texture);
                                                        break;
                                                    case "preserve_aspect_ratio":
                                                        //not implemented
                                                        break;
                                                }
                                            }
                                        }
                                        Textures.Add(texture);
                                    }
                                }
                                break;
                        }
                    }
                }
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
