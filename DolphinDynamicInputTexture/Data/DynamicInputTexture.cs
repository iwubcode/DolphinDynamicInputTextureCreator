using DolphinDynamicInputTexture.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;

namespace DolphinDynamicInputTexture.Data
{
    [JsonObject(IsReference = true)]
    public class DynamicInputTexture : Other.PropertyChangedBase, Interfaces.IImage
    {

        #region PROPERTIES

        /// <summary>
        /// The path to the texture before any input modification
        /// </summary>
        public string TexturePath
        {
            get => _texture_path;
            set
            {
                _texture_path = value;
                TextureHash ??= Path.GetFileName(value);
                OnPropertyChanged(nameof(TexturePath));
                UpdateImageWidthHeight();
            }
        }
        private string _texture_path;

        /// <summary>
        /// the reladive image paht, starting from the packages directory.
        /// </summary>
        public string RelativeTexturePath
        {
            get => _relative_texture_path;
            set
            {
                _relative_texture_path = value;
                OnPropertyChanged(nameof(RelativeTexturePath));
            }
        }
        private string _relative_texture_path;

        /// <summary>
        /// The host devices mapped in this image
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
        /// Regions in the texture that are applicable for replacement
        /// </summary>
        public string TextureHash
        {
            get => _texture_hash;
            set
            {
                _texture_hash = value;
                HashProperties = new DolphinTextureHash(value);
                OnPropertyChanged(nameof(TextureHash));
                OnPropertyChanged(nameof(HashProperties));
            }
        }
        private string _texture_hash;

        /// <summary>
        /// Gives specific information about the Dolphin textures hash.
        /// </summary>
        [JsonIgnore]
        public DolphinTextureHash HashProperties { get; private set; }

        /// <summary>
        /// Regions in the texture that are applicable for replacement
        /// </summary>
        public ObservableCollection<InputRegion> Regions
        {
            get => _regions ??= Regions = new ObservableCollection<InputRegion>();
            set
            {
                if (_regions != null)
                {
                    _regions.CollectionChanged -= OnCollectionOfRegionsChanged;
                }
                if (value.Count > 0)
                {
                    foreach (InputRegion Region in value)
                    {
                        Region.OwnedRegion = null;
                        Region.RegionRect.OwnedTexture = this;
                    }
                }
                _regions = value;
                _regions.CollectionChanged += OnCollectionOfRegionsChanged;
                OnPropertyChanged(nameof(Regions));
            }
        }
        private ObservableCollection<InputRegion> _regions;

        /// <summary>
        /// connects all regions with this texture.
        /// </summary>
        private void OnCollectionOfRegionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (InputRegion Region in e.NewItems)
                {
                    Region.OwnedRegion = null;
                    Region.RegionRect.OwnedTexture = this;
                    foreach (InputRegion subentry in Region.SubEntries)
                    {
                        subentry.RegionRect.OwnedTexture = this;
                    }
                }
            }
        }

        /// <summary>
        /// The texture width
        /// </summary>
        [JsonIgnore]
        public int ImageWidth
        {
            get => _image_width;
            private set
            {
                _image_width = value;
                OnPropertyChanged(nameof(ImageWidth));
                OnPropertyChanged(nameof(ImageWidthScaling));
            }
        }
        private int _image_width;

        /// <summary>
        /// The texture height
        /// </summary>
        [JsonIgnore]
        public int ImageHeight
        {
            get => _image_height;
            private set
            {
                _image_height = value;
                OnPropertyChanged(nameof(ImageHeight));
                OnPropertyChanged(nameof(ImageHeightScaling));
            }
        }
        private int _image_height;

        /// <summary>
        /// Scaling of the width in relation to the original width of the texture dump.
        /// </summary>
        public double ImageWidthScaling
        {
            get
            {
                if (HashProperties != null && HashProperties.IsValid)
                {
                    return ImageWidth / HashProperties.ImageWidth;
                }
                return 0;
            }
        }

        /// <summary>
        /// Scaling of the height in relation to the original height of the texture dump.
        /// </summary>
        public double ImageHeightScaling
        {
            get
            {
                if (HashProperties != null && HashProperties.IsValid)
                {
                    return ImageHeight / HashProperties.ImageHeight;
                }
                return 0;
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// reads the image size and adjusts the regions if necessary.
        /// </summary>
        private void UpdateImageWidthHeight()
        {
            if (File.Exists(_texture_path))
            {
                using (var bmp = new Bitmap(_texture_path))
                {
                    if (ImageHeight > 0 && ImageWidth > 0)
                    {
                        double width_scale = (double)bmp.Width / ImageWidth;
                        double height_scale = (double)bmp.Height / ImageHeight;

                        //When scaling up, the scale must be set directly.
                        if (width_scale >= 1) ImageWidth = bmp.Width;
                        if (height_scale >= 1) ImageHeight = bmp.Height;

                        foreach (InputRegion region in Regions)
                        {
                            region.RegionRect.X *= width_scale;
                            region.RegionRect.Width *= width_scale;
                            region.RegionRect.Y *= height_scale;
                            region.RegionRect.Height *= height_scale;
                        }

                        ImageWidth = bmp.Width;
                        ImageHeight = bmp.Height;
                    }
                    else
                    {
                        ImageHeight = bmp.Height;
                        ImageWidth = bmp.Width;
                    }
                }
            }
        }

        #endregion
    }
}
