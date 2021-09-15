using System;
using System.Collections.Generic;
using System.Text;

namespace DolphinDynamicInputTextureCreator.Data
{
    public class RegionBrush : Other.PropertyChangedBase
    {
        #region PROPERTIES
        /// <summary>
        /// The currently selected emulated device for this "brush"
        /// </summary>
        private EmulatedDevice _selected_emulated_device;
        public EmulatedDevice SelectedEmulatedDevice
        {
            get { return _selected_emulated_device; }
            set
            {
                _selected_emulated_device = value;
                OnPropertyChanged(nameof(SelectedEmulatedDevice));
            }
        }

        /// <summary>
        /// The currently selected emulated key for this "brush"
        /// </summary>
        private EmulatedKey _selected_emulated_key;
        public EmulatedKey SelectedEmulatedKey
        {
            get { return _selected_emulated_key; }
            set
            {
                _selected_emulated_key = value;
                OnPropertyChanged(nameof(SelectedEmulatedKey));
            }
        }

        /// <summary>
        /// Whether to fill the entire image with the region
        /// </summary>
        private bool _fill_region;
        public bool FillRegion
        {
            get { return _fill_region; }
            set
            {
                _fill_region = value;
                OnPropertyChanged(nameof(FillRegion));
            }
        }

        /// <summary>
        /// whether subpixels are used.
        /// </summary>
        private bool _subpixel;
        public bool Subpixel
        {
            get { return _subpixel; }
            set
            {
                _subpixel = value;
                RectRegion.DecimalPlaces = value ? 1 : 0;
                OnPropertyChanged(nameof(Subpixel));
            }
        }
        #endregion
    }
}
