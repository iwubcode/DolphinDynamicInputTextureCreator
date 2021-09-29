using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DolphinDynamicInputTextureCreator.Data
{
    [JsonObject(IsReference = true)]
    public class DynamicInputTexture : Other.PropertyChangedBase, Interfaces.IExportableImage
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
        public ObservableCollection<RectRegion> Regions
        {
            get => _regions;
            set
            {
                _regions = value;
                OnPropertyChanged(nameof(Regions));
            }
        }
        private ObservableCollection<RectRegion> _regions = new ObservableCollection<RectRegion>();

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
                OnPropertyChanged(nameof(ImageWidth));
            }
        }
        private int _image_height;
        #endregion

        #region Update

        private void UpdateImageWidthHeight()
        {
            if (File.Exists(_texture_path))
            {
                using (var bmp = new Bitmap(_texture_path))
                {
                    ImageHeight = bmp.Height;
                    ImageWidth = bmp.Width;
                }
                //Automatically select suitable zoom. (ViewModels)
                if (ScaleFactor == 1)
                {
                    SetInitialZoom();
                }
            }
        }

        #endregion

        // part of the ViewModels

        /// <summary>
        /// The scale factor for how much to zoom the current texture and regions
        /// </summary>
        [JsonIgnore]
        public double ScaleFactor
        {
            get => _scale_factor;
            set
            {
                _scale_factor = Smooth(value, 2);
                OnPropertyChanged(nameof(ScaleFactor));
                Regions.ToList().ForEach(x => x.ScaleFactor = _scale_factor);
            }
        }
        private double _scale_factor = 1;

        private double Smooth(double value, int accuracy)
        {
            double factor = ((int)value * 10).ToString().Length;
            factor = Math.Pow(10, factor);
            return Math.Round(value / factor, accuracy + 1) * factor;
        }

        public void SetInitialZoom(double absolutescale = 600)
        {
            if (ImageHeight <= 0 || ImageWidth <= 0)
                return;

            absolutescale /= (ImageHeight + ImageWidth) / 2;
            ScaleFactor = absolutescale;
        }

        #region COMMANDS
        private ICommand _delete_region_command;
        [JsonIgnore]
        public ICommand DeleteRegionCommand
        {
            get
            {
                if (_delete_region_command == null)
                {
                    _delete_region_command = new RelayCommand(param => DeleteRegion((RectRegion)param));
                }
                return _delete_region_command;
            }
        }

        private void DeleteRegion(RectRegion region)
        {
            Regions.Remove(region);
        }

        private ICommand _reset_scale_factor;
        [JsonIgnore]
        public ICommand ResetScaleFactorCommand
        {
            get
            {
                if (_reset_scale_factor == null)
                {
                    _reset_scale_factor = new RelayCommand(ResetScaleFactor);
                }
                return _reset_scale_factor;
            }
        }

        private void ResetScaleFactor(object obj)
        {
            ScaleFactor = 1;
        }
        #endregion
    }
}
