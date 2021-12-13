using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTexture.Interfaces;
using DolphinDynamicInputTextureCreator.ViewModels;

namespace DolphinDynamicInputTextureCreator.Models
{
    public class UIRegionRect : InputRegionRect
    {
        public DynamicInputPackViewModel Pack
        {
            get => _pack;
            set
            {
                _pack = value;
                OnPropertyChanged(nameof(Pack));
                UpdateScale();
            }
        }
        private DynamicInputPackViewModel _pack;

        public new double X
        {
            get => base.X;
            set
            {
                base.X = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(ScaledX));
            }
        }

        public new double Y
        {
            get => base.Y;
            set
            {
                base.Y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(ScaledY));
            }
        }

        public new double Height
        {
            get => base.Height;
            set
            {
                base.Height = value;
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(ScaledHeight));
            }
        }
        public new double Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(ScaledWidth));
            }
        }

        public UIRegionRect()
        { }
        public UIRegionRect(double x, double y, double width, double height) : base(x, y, width, height)
        { }
        public UIRegionRect(IRectRegion rect) : base(rect)
        { }

        public void UpdateScale()
        {
            OnPropertyChanged(nameof(ScaledX));
            OnPropertyChanged(nameof(ScaledY));
            OnPropertyChanged(nameof(ScaledHeight));
            OnPropertyChanged(nameof(ScaledWidth));
        }

        public double ScaledX
        {
            get => X * Pack.ScaleFactor;
            set => X = value / Pack.ScaleFactor;
        }

        public double ScaledY
        {
            get => Y * Pack.ScaleFactor;
            set => Y = value / Pack.ScaleFactor;
        }

        public double ScaledWidth
        {
            get => Width * Pack.ScaleFactor;
            set => Width = value / Pack.ScaleFactor;
        }

        public double ScaledHeight
        {
            get => Height * Pack.ScaleFactor;
            set => Height = value / Pack.ScaleFactor;
        }
    }
}
