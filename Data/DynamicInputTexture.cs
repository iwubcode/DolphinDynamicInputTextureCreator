﻿using DolphinDynamicInputTextureCreator.Other;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.Data
{
    [JsonObject(IsReference = true)]
    public class DynamicInputTexture : Other.PropertyChangedBase
    {
        #region PROPERTIES
        /// <summary>
        /// The hash of the texture, as defined by Dolphin
        /// </summary>
        private string _texture_hash;
        public string TextureHash
        {
            get
            {
                return _texture_hash;
            }
            set
            {
                _texture_hash = value;
                OnPropertyChanged(nameof(TextureHash));
            }
        }

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
        /// The path to the texture before any input modification
        /// </summary>
        private string _texture_path;
        public string TexturePath
        {
            get
            {
                return _texture_path;
            }
            set
            {
                _texture_path = value;
                OnPropertyChanged(nameof(TexturePath));
                OnPropertyChanged(nameof(CanEditTexture));

                // WPF likes to apply image scaling
                // we need to remember the image size
                // so we can enforce it
                if (File.Exists(_texture_path))
                {
                    using (var bmp = new Bitmap(_texture_path))
                    {
                        _image_height = bmp.Height;
                        _image_width = bmp.Width;
                    }
                    //Automatically select suitable zoom.
                    if (ScaleFactor == 1)
                    {
                        SetInitialZoom();
                    }
                }
            }
        }

        /// <summary>
        /// Regions in the texture that are applicable for replacement
        /// </summary>
        private ObservableCollection<RectRegion> _regions = new ObservableCollection<RectRegion>();
        public ObservableCollection<RectRegion> Regions
        {
            get
            {
                return _regions;
            }
            set
            {
                _regions = value;
                OnPropertyChanged(nameof(Regions));
            }
        }

        /// <summary>
        /// Returns true if the texture is editable
        /// </summary>
        [JsonIgnore]
        public bool CanEditTexture
        {
            get
            {
                return TexturePath != null && File.Exists(TexturePath);
            }
        }

        /// <summary>
        /// The texture width
        /// </summary>
        private int _image_width;
        public int ImageWidth
        {
            get
            {
                return _image_width;
            }
            set
            {
                _image_width = value;
                OnPropertyChanged(nameof(ImageWidth));
            }
        }

        /// <summary>
        /// The texture height
        /// </summary>
        private int _image_height;
        public int ImageHeight
        {
            get
            {
                return _image_height;
            }
            set
            {
                _image_height = value;
                OnPropertyChanged(nameof(ImageHeight));
            }
        }

        /// <summary>
        /// The scale factor for how much to zoom the current texture and regions
        /// </summary>
        private double _scale_factor = 1;
        public double ScaleFactor
        {
            get
            {
                return _scale_factor;
            }
            set
            {
                _scale_factor = Smooth(value,2);
                OnPropertyChanged(nameof(ScaleFactor));
                Regions.ToList().ForEach(x => x.ScaleFactor = _scale_factor);
            }
        }
        #endregion

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
