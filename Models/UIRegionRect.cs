using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTexture.Interfaces;
using DolphinDynamicInputTextureCreator.ViewModels;

namespace DolphinDynamicInputTextureCreator.Models
{
    public class UIRegionRect : InputSubRegionRect
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
                OnPropertyChanged(nameof(ScaledX));
            }
        }

        public new double Y
        {
            get => base.Y;
            set
            {
                base.Y = value;
                OnPropertyChanged(nameof(ScaledY));
            }
        }


        public new double Height
        {
            get => base.Height;
            set
            {
                base.Height = value;
                OnPropertyChanged(nameof(ScaledY));
                OnPropertyChanged(nameof(ScaledHeight));
            }
        }

        public new double Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                OnPropertyChanged(nameof(ScaledX));
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
            get => (X - (MainRegion != null ? MainRegion.X: 0)) * Pack.ScaleFactor;
            set => X = value / Pack.ScaleFactor + (MainRegion != null ? MainRegion.X : 0);
        }

        public double ScaledY
        {
            get => (Y - (MainRegion != null ? MainRegion.Y : 0)) * Pack.ScaleFactor;
            set => Y = value / Pack.ScaleFactor + (MainRegion != null ? MainRegion.Y : 0);
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
