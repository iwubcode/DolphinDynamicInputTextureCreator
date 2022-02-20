using DolphinDynamicInputTexture.Interfaces;
using DolphinDynamicInputTexture.Properties;
using Newtonsoft.Json;
using System;
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
        /// The emulated Devices mapped in this pack.
        /// </summary>
        protected Collection<EmulatedDevice> _emulated_devices = new Collection<EmulatedDevice>();

        /// <summary>
        /// The tags mapped in this pack.
        /// </summary>
        protected Collection<Tag> _tags = new Collection<Tag>();

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
        /// <summary>
        /// Exports the package to the specified directory
        /// </summary>
        /// <param name="location">export directory</param>
        public void ExportToLocation(string location)
        {
            WriteImages(location);
            WriteJson(Path.Combine(location, GeneratedJsonName + ".json"));
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

        private void WriteImage(string location, IImage image)
        {
            // Unlikely that we get here but skip textures that don't exist
            if (!File.Exists(image.TexturePath))
                return;

            //check the output Path
            image.RelativeTexturePath = CheckRelativeTexturePath(image);
            string output_location = Path.Combine(location, image.RelativeTexturePath);

            //create the directory when it does not exist.
            Directory.CreateDirectory(Path.GetDirectoryName(output_location));

            //Use External Scaling if necessary.
            if (image is DynamicInputTexture texture)
            {
                if (DynamicInputTextureEvents.DynamicInputTextureExportProcessor != null)
                {
                    DynamicInputTextureEvents.DynamicInputTextureExportProcessor.Invoke(output_location, texture);
                    if (File.Exists(output_location))
                    {
                        texture.TexturePath = output_location;
                        return;
                    }
                }
            }

            // Prevents the file from trying to overwrite itself.
            if (Path.GetFullPath(output_location) == Path.GetFullPath(image.TexturePath))
                return;

            //write the image
            const bool overwrite = true;
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
                writer.WriteStartArray();
                foreach (var key in device.HostKeys)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("keys");
                    writer.WriteStartArray();
                    foreach (var name in key.Name.Split(","))
                    {
                        writer.WriteValue(name);
                    }
                    writer.WriteEndArray();
                    if (key.Tag != null && key.Tag.Name != "")
                    {
                        writer.WritePropertyName("tag");
                        writer.WriteValue(key.Tag.Name);
                    }
                    writer.WritePropertyName("image");
                    key.RelativeTexturePath = CheckRelativeTexturePath(key, device, texture);
                    writer.WriteValue(key.RelativeTexturePath);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        private void WriteRegionData(JsonWriter writer, InputRegion region)
        {
            writer.WriteStartObject();

            if (region.Key.Name != "")
            {
                writer.WritePropertyName("key");
                writer.WriteValue(region.Key.Name);
            }
            if (region.Tag != null && region.Tag.Name != "")
            {
                writer.WritePropertyName("tag");
                writer.WriteValue(region.Tag.Name);
            }
            if (region.CopyType != default)
            {
                writer.WritePropertyName("copy_type");
                writer.WriteValue(region.CopyType.ToString());
            }
            if (region.BindType != default)
            {
                writer.WritePropertyName("bind_type");
                writer.WriteValue(region.BindType.ToString());
            }
            writer.WritePropertyName("region");
            writer.WriteStartArray();
            writer.WriteValue(region.RegionRect.X);
            writer.WriteValue(region.RegionRect.Y);
            writer.WriteValue(region.RegionRect.RightX);
            writer.WriteValue(region.RegionRect.BottomY);
            writer.WriteEndArray();
            if (region.SubEntries.Count > 0)
            {
                writer.WritePropertyName("sub_entries");
                writer.WriteStartArray();
                foreach (var subregin in region.SubEntries)
                {
                    WriteRegionData(writer, subregin);
                }
                writer.WriteEndArray();
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
                writer.WritePropertyName("specification");
                writer.WriteValue(2.0);
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

                    foreach (var region_data in CollectRegionsForDevices(texture))
                    {
                        // Skip devices with no mapped keys
                        if (region_data.Value.Count == 0)
                        {
                            continue;
                        }
                        writer.WritePropertyName(region_data.Key);
                        writer.WriteStartArray();

                        foreach (var region in region_data.Value)
                        {
                            WriteRegionData(writer, region);
                        }
                        writer.WriteEndArray();
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
                    if (DynamicInputTextureEvents.ImageNotExist == null || !DynamicInputTextureEvents.ImageNotExist(image, details != "" ? details : Path.GetFileName(image.TexturePath)))
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
                            if (key.Name == null)
                            {
                                key.Name = reader.Value.ToString();
                            }
                            else
                            {
                                key.Name += "," + reader.Value.ToString();
                            }
                        }
                        break;
                    case "image":
                        key.RelativeTexturePath = reader.ReadAsString();
                        key.TexturePath = Path.Combine(path, key.RelativeTexturePath);
                        break;
                    case "tag":
                        if (!IsEqualInCollection(_tags, new Tag { Name = reader.ReadAsString() }, out Tag tag))
                        {
                            _tags.Add(tag);
                        }

                        key.Tag = tag;
                        break;
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
                    if (!IsEqualInCollection(_emulated_devices, new EmulatedDevice { Name = reader.Value.ToString() }, out EmulatedDevice emulatedDevice))
                    {
                        _emulated_devices.Add(emulatedDevice);
                    }

                    reader.Read();
                    switch (reader.TokenType)
                    {
                        case JsonToken.StartObject: // V1
                            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                            {
                                if (reader.TokenType == JsonToken.PropertyName)
                                {
                                    if (!IsEqualInCollection(emulatedDevice.EmulatedKeys, new EmulatedKey { Name = reader.Value.ToString() }, out EmulatedKey emulatedkey))
                                    {
                                        emulatedDevice.EmulatedKeys.Add(emulatedkey);
                                    }

                                    if (reader.Read() && reader.TokenType == JsonToken.StartArray)
                                    {
                                        while (reader.Read() && reader.TokenType == JsonToken.StartArray)
                                        {
                                            InputRegion Region = new InputRegion { Device = emulatedDevice, Key = emulatedkey };
                                            if (ReadInputRegionRect(reader, Region.RegionRect))
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
                                texture.Regions.Add(ReadV2InputRegion(reader, emulatedDevice, texture));
                            }
                            break;
                    }
                }
            }
        }

        private bool ReadInputRegionRect(JsonReader reader, IRectRegion Region)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                Region.X = reader.ReadAsDouble().Value;
                Region.Y = reader.ReadAsDouble().Value;
                Region.Width = reader.ReadAsDouble().Value - Region.X;
                Region.Height = reader.ReadAsDouble().Value - Region.Y;
                while (reader.TokenType != JsonToken.EndArray && reader.Read()) { }
                return true;
            }
            else
            {
                return false;
            }
        }

        private InputRegion ReadV2InputRegion(JsonReader reader, EmulatedDevice emulatedDevice, DynamicInputTexture texture)
        {
            InputRegion Region = new InputRegion { Device = emulatedDevice};
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                switch (reader.Value)
                {
                    case "key":
                        if (!IsEqualInCollection(emulatedDevice.EmulatedKeys, new EmulatedKey { Name = reader.ReadAsString() }, out EmulatedKey emulatedkey))
                        {
                            emulatedDevice.EmulatedKeys.Add(emulatedkey);
                        }

                        Region.Key = emulatedkey;
                        break;
                    case "region":
                        reader.Read();
                        ReadInputRegionRect(reader, Region.RegionRect);
                        break;
                    case "tag":
                        if (!IsEqualInCollection(_tags, new Tag { Name = reader.ReadAsString() }, out Tag tag))
                        {
                            _tags.Add(tag);
                        }

                        Region.Tag = tag;
                        break;
                    case "bind_type":
                        Region.BindType = Enum.Parse<BindTypeProperties>(reader.ReadAsString());
                        break;
                    case "copy_type":
                        Region.CopyType = Enum.Parse<CopyTypeProperties>(reader.ReadAsString());
                        break;
                    case "sub_entries":
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                        {
                            InputRegion subentry = ReadV2InputRegion(reader, emulatedDevice, texture);
                            subentry.RegionRect = new InputSubRegionRect(subentry.RegionRect);
                            Region.SubEntries.Add(subentry);
                        }
                        break;
                }
            }
            return Region;
        }

        #endregion
        /// <summary>
        /// Imports a package in dolphin dynamic input texture format
        /// </summary>
        /// <param name="file">path to the file</param>
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
        private Dictionary<string, List<InputRegion>> CollectRegionsForDevices(DynamicInputTexture texture)
        {
            Dictionary<string, List<InputRegion>> result = new Dictionary<string, List<InputRegion>>();
            foreach (var region in texture.Regions)
            {
                if (result.ContainsKey(region.Device.Name))
                {
                    result[region.Device.Name].Add(region);
                }
                else
                {
                    result.Add(region.Device.Name, new List<InputRegion> { region });
                }
            }

            return result;
        }

        internal static bool IsEqualInCollection<T>(IList<T> list, T check, out T match) where T : IEquatable<T>
        {
            foreach (T item in list)
            {
                if (item.Equals(check))
                {
                    match = item;
                    return true;
                }
            }
            match = check;
            return false;
        }

    }
}
