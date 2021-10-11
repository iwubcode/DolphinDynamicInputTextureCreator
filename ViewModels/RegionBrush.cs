using Newtonsoft.Json;
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
        public EmulatedDevice SelectedEmulatedDevice
        {
            get => _selected_emulated_device;
            set
            {
                _selected_emulated_device = value;
                OnPropertyChanged(nameof(SelectedEmulatedDevice));

                if (value?.EmulatedKeys.Count > 0)
                    SelectedEmulatedKey ??= value.EmulatedKeys[0];
            }
        }
        private EmulatedDevice _selected_emulated_device;

        /// <summary>
        /// The currently selected emulated key for this "brush"
        /// </summary>
        [JsonIgnore]
        public EmulatedKey SelectedEmulatedKey
        {
            get => _selected_emulated_key;
            set
            {
                _selected_emulated_key = value;
                OnPropertyChanged(nameof(SelectedEmulatedKey));
            }
        }
        private EmulatedKey _selected_emulated_key;

        /// <summary>
        /// whether subpixels are used.
        /// </summary>
        public bool Subpixel
        {
            get => _subpixel;
            set
            {
                _subpixel = value;
                RectRegion.DecimalPlaces = value ? 2 : 0;
                OnPropertyChanged(nameof(Subpixel));
            }
        }
        private bool _subpixel;

        /// <summary>
        /// false = Bright canvas.
        /// true = Dark canvas.
        /// </summary>
        public bool UseDarkBackground
        {
            get => _use_dark_background;
            set
            {
                _use_dark_background = value;
                OnPropertyChanged(nameof(UseDarkBackground));
            }
        }
        private bool _use_dark_background = false;

        #endregion
    }
}
