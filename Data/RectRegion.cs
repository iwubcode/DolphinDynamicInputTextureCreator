using Newtonsoft.Json;
﻿using System;

namespace DolphinDynamicInputTextureCreator.Data
{
    public class RectRegion : Other.PropertyChangedBase
    {

        /// <summary>
        /// this determines the sub pixel value, 0 uses only full pixels.
        /// </summary>
        public static int DecimalPlaces { get; set; } = 0;

        private EmulatedDevice _emulated_device;
        public EmulatedDevice Device
        {
            get { return _emulated_device; }
            set
            {
                _emulated_device = value;
                OnPropertyChanged(nameof(Device));
            }
        }

        private EmulatedKey _emulated_key;
        public EmulatedKey Key
        {
            get { return _emulated_key; }
            set
            {
                _emulated_key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        private DynamicInputTexture _owned_texture;
        public DynamicInputTexture OwnedTexture
        {
            get { return _owned_texture; }
            set
            {
                _owned_texture = value;
                OnPropertyChanged(nameof(OwnedTexture));
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(Width));
            }
        }

        private double _scale_factor;
        public double ScaleFactor
        {
            get { return _scale_factor; }
            set
            {
                _scale_factor = value;
                OnPropertyChanged(nameof(ScaleFactor));
                OnPropertyChanged(nameof(ScaledX));
                OnPropertyChanged(nameof(ScaledY));
                OnPropertyChanged(nameof(ScaledHeight));
                OnPropertyChanged(nameof(ScaledWidth));
            }
        }

        private double _x;
        public double X
        {
            get { return _x; }
            set
            {
                _x = Math.Round(value, DecimalPlaces);
                if (_x < 0)
                    _x = 0;

                if (OwnedTexture != null)
                {
                    if (_x + Width > OwnedTexture.ImageWidth)
                        _x = OwnedTexture.ImageWidth - Width;
                }

                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(ScaledX));
            }
        }

        private double _y;
        public double Y
        {
            get { return _y; }
            set
            {
                _y = Math.Round(value, DecimalPlaces);
                if (_y < 0)
                    _y = 0;

                if (OwnedTexture != null)
                {
                    if (_y + Height > OwnedTexture.ImageHeight)
                        _y = OwnedTexture.ImageHeight - Height;
                }

                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(ScaledY));
            }
        }

        private double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                _height = Math.Round(value, DecimalPlaces);
                if (OwnedTexture != null)
                {
                    if ((_height + Y) > OwnedTexture.ImageHeight)
                        _height = OwnedTexture.ImageHeight - Y;
                }
                if (_height < 0)
                    _height = 1;
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(ScaledHeight));
            }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
            set
            {
                _width = Math.Round(value, DecimalPlaces);
                if (OwnedTexture != null)
                {
                    if ((_width + X) > OwnedTexture.ImageWidth)
                        _width = OwnedTexture.ImageWidth - X;
                }
                if (_width < 0)
                    _width = 1;
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(ScaledWidth));
            }
        }

        [JsonIgnore]
        public double ScaledX
        {
            get
            {
                return X * ScaleFactor;
            }
            set
            {
                X = value / ScaleFactor;
            }
        }

        [JsonIgnore]
        public double ScaledY
        {
            get
            {
                return Y * ScaleFactor;
            }
            set
            {
                Y = value / ScaleFactor;
            }
        }

        [JsonIgnore]
        public double ScaledWidth
        {
            get 
            {
                return Width * ScaleFactor;
            }
            set
            {
                Width = value / ScaleFactor;
            }
        }

        [JsonIgnore]
        public double ScaledHeight
        {
            get
            {
                return Height * ScaleFactor;
            }
            set
            {
                Height = value / ScaleFactor;
            }
        }
    }
}
