using DolphinDynamicInputTexture.Interfaces;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace DolphinDynamicInputTexture.Data
{
    public class InputSubRegionRect : InputRegionRect, ISubRectRegion
    {
        IRectRegion ISubRectRegion.MainRegion
        {
            get => _main_region;
            set
            {
                if (MainRegion != null)
                {
                    MainRegion.PropertyChanged -= OnMainRegionChanged;
                    _main_region = value;
                    UpdateXScale();
                    UpdateYScale();
                }
                else
                {
                    base.X = X - value.X;
                    base.Y = Y - value.Y;
                    LastMainRegionWidth = value.Width;
                    LastMainRegionHeight = value.Height;
                    _main_region = value;
                }
                MainRegion.PropertyChanged += OnMainRegionChanged;
                OnPropertyChanged(nameof(MainRegion));
            }
        }

        public IRectRegion MainRegion => _main_region;
        private IRectRegion _main_region;

        public new double X
        {
            get => MainRegion != null ? MainRegion.X + base.X : base.X;
            set
            {
                if (MainRegion != null)
                {
                    base.X = value - MainRegion.X;

                    if (RightX > MainRegion.RightX)
                        base.X -= RightX - MainRegion.RightX;
                }
                else
                {
                    base.X = value;
                }
            }
        }

        public new double Y
        {
            get => MainRegion != null ? MainRegion.Y + base.Y : base.Y;
            set
            {
                if (MainRegion != null)
                {
                    base.Y = value - MainRegion.Y;

                    if (BottomY > MainRegion.BottomY)
                        base.Y -= BottomY - MainRegion.BottomY;
                }
                else
                {
                    base.Y = value;
                }
            }
        }

        public new double Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                if (MainRegion != null)
                {
                    if (RightX > MainRegion.RightX)
                        base.Width -= RightX - MainRegion.RightX;
                }
            }
        }

        public new double Height
        {
            get => base.Height;
            set
            {
                base.Height = value;
                if (MainRegion != null)
                {
                    if (BottomY > MainRegion.BottomY)
                        base.Height -= BottomY - MainRegion.BottomY;
                }
            }
        }

        public new double BottomY => Y + Height;

        public new double RightX => X + Width;

        public new bool Equals([AllowNull] IRectRegion other)
        {
            return other != null && Equals(other.X, other.Y, other.Width, other.Height);
        }

        public new bool Equals(double x, double y, double width, double height)
        {
            return x == X & y == Y & width == Width & height == Height;
        }

        private double LastMainRegionWidth;
        private double LastMainRegionHeight;

        private void UpdateXScale()
        {
            if (LastMainRegionWidth != MainRegion.Width)
            {
                int i = InputRegionRect.DecimalPlaces;
                InputRegionRect.DecimalPlaces = 3;
                base.X *= MainRegion.Width / LastMainRegionWidth;
                Width *= MainRegion.Width / LastMainRegionWidth;
                LastMainRegionWidth = MainRegion.Width;
                InputRegionRect.DecimalPlaces = i;
            }
        }

        private void UpdateYScale()
        {
            if (LastMainRegionHeight != MainRegion.Height)
            {
                int i = InputRegionRect.DecimalPlaces;
                InputRegionRect.DecimalPlaces = 3;
                base.Y *= MainRegion.Height / LastMainRegionHeight;
                Height *= MainRegion.Height / LastMainRegionHeight;
                LastMainRegionHeight = MainRegion.Height;
                InputRegionRect.DecimalPlaces = i;
            }
        }

        public InputSubRegionRect() { }

        public InputSubRegionRect(double x, double y, double width, double height) :base(x,y, width, height) { }

        public InputSubRegionRect(IRectRegion Rect) : this(Rect.X, Rect.Y, Rect.Width, Rect.Height){}

        private void OnMainRegionChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Width):
                    UpdateXScale();
                    break;
                case nameof(Height):
                    UpdateYScale();
                    break;
                default:
                    OnPropertyChanged(e.PropertyName);
                    break;
            }
        }
    }
}
