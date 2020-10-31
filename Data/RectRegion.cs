namespace DolphinDynamicInputTextureCreator.Data
{
    public class RectRegion : Other.PropertyChangedBase
    {
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
                _x = value;
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
                _y = value;
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
                _height = value;
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
                _width = value;
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(ScaledWidth));
            }
        }

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

        public static double MinWidth = 5.0;
        public static double MinHeight = 5.0;
    }
}
