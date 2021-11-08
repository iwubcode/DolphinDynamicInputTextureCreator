using DolphinDynamicInputTexture.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DolphinDynamicInputTexture.Data
{
    public class InputRegionRect : Other.PropertyChangedBase, IRectRegion
    {
        /// <summary>
        /// this determines the sub pixel value, 0 uses only full pixels.
        /// </summary>
        public static int DecimalPlaces { get; set; } = 0;

        DynamicInputTexture IRectRegion.OwnedTexture
        {
            get => _owned_texture;
            set
            {
                _owned_texture = value;
                OnPropertyChanged(nameof(IRectRegion.OwnedTexture));
                OnPropertyChanged(nameof(OwnedTexture));
            }
        }
        public DynamicInputTexture OwnedTexture => _owned_texture;
        private DynamicInputTexture _owned_texture;

        public double X
        {
            get => _x;
            set
            {
                _x = Math.Round(value, DecimalPlaces);
                if (_x < 0)
                    _x = 0;

                if (OwnedTexture != null && OwnedTexture.ImageWidth != 0)
                {
                    if (_x + Width > OwnedTexture.ImageWidth)
                        _x = OwnedTexture.ImageWidth - Width;
                }

                OnPropertyChanged(nameof(X));
            }
        }
        private double _x;

        public double Y
        {
            get => _y;
            set
            {
                _y = Math.Round(value, DecimalPlaces);
                if (_y < 0)
                    _y = 0;

                if (OwnedTexture != null && OwnedTexture.ImageHeight != 0)
                {
                    if (_y + Height > OwnedTexture.ImageHeight)
                        _y = OwnedTexture.ImageHeight - Height;
                }

                OnPropertyChanged(nameof(Y));
            }
        }
        private double _y;

        public double Height
        {
            get => _height;
            set
            {
                _height = Math.Round(value, DecimalPlaces);
                if (OwnedTexture != null && OwnedTexture.ImageHeight != 0)
                {
                    if ((_height + Y) > OwnedTexture.ImageHeight)
                        _height = OwnedTexture.ImageHeight - Y;
                }
                if (_height <= 0)
                    _height = GetMinHeight();
                OnPropertyChanged(nameof(Height));
            }
        }
        private double _height;

        public double Width
        {
            get => _width;
            set
            {
                _width = Math.Round(value, DecimalPlaces);
                if (OwnedTexture != null && OwnedTexture.ImageWidth != 0)
                {
                    if ((_width + X) > OwnedTexture.ImageWidth)
                        _width = OwnedTexture.ImageWidth - X;
                }
                if (_width <= 0)
                    _width = GetMinWidth();
                OnPropertyChanged(nameof(Width));
            }
        }
        private double _width;

        public double BottomY => Y + Height;

        public double RightX => X + Width;

        protected double GetMinHeight() => GetMinWidth();

        protected double GetMinWidth() => Math.Pow(10.0, -DecimalPlaces);

        public InputRegionRect() { }

        public InputRegionRect(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public InputRegionRect(IRectRegion Rect) : this(Rect.X, Rect.Y, Rect.Width, Rect.Height) { }

        public object Clone() => this.MemberwiseClone();

        public bool Equals([AllowNull] IRectRegion other)
        {
            return other != null && Equals(other.X, other.Y, other.Width, other.Height);
        }

        public bool Equals(double x, double y, double width, double height)
        {
            return x == X & y == Y & width == Width & height == Height;
        }

        public bool OnTexture()
        {
            return OwnedTexture != null && X >= 0 && Y >= 0 && RightX <= OwnedTexture.ImageWidth && BottomY <= OwnedTexture.ImageHeight;
        }

        public bool IntersectsWith(IRectRegion other)
        {
            return !(X > other.RightX || Y > other.BottomY || RightX < other.X || BottomY < other.Y);
        }

        public bool Contains(IRectRegion other)
        {
            return other.X >= X && other.Y >= Y && other.RightX <= RightX && other.BottomY <= BottomY;
        }

        public bool Contains(double x, double y)
        {
            return x >= X && x <= RightX && y >= Y && y <= BottomY;
        }
    }
}
