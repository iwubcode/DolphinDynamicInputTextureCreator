using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTextureCreator.Models;
using Newtonsoft.Json;

namespace DolphinDynamicInputTextureCreator.ViewModels
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
        ///  The currently selected Tag for this "brush"
        /// </summary>
        [JsonIgnore]
        public Tag SelectedTag
        {
            get => _selected_tag;
            set
            {
                _selected_tag = value;
                OnPropertyChanged(nameof(SelectedTag));
            }
        }
        private Tag _selected_tag;

        /// <summary>
        /// false = dont use Keys.
        /// true = Use Keys.
        /// </summary>
        [JsonIgnore]
        public bool UseKey
        {
            get => _use_key;
            set
            {
                _use_key = value;
                if (!_use_key)
                    UseTag = true;
                OnPropertyChanged(nameof(UseKey));
            }
        }
        private bool _use_key = true;

        /// <summary>
        /// false = dont use tag.
        /// true = Use Tag.
        /// </summary>
        [JsonIgnore]
        public bool UseTag
        {
            get => _use_tag;
            set
            {
                _use_tag = value;
                if (!_use_tag)
                    UseKey = true;
                OnPropertyChanged(nameof(UseTag));
            }
        }
        private bool _use_tag = false;

        /// <summary>
        /// whether subpixels are used.
        /// </summary>
        public bool Subpixel
        {
            get => _subpixel;
            set
            {
                _subpixel = value;
                InputRegionRect.DecimalPlaces = value ? 2 : 0;
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

        /// <summary>
        /// Update the region with the current brush. 
        /// </summary>
        /// <param name="region">InputRegion to update</param>
        public void UpdateRegion(InputRegion region)
        {
            region.Device = SelectedEmulatedDevice;
            region.Key = UseKey ? SelectedEmulatedKey : new EmulatedKey();
            region.Tag = UseTag ? SelectedTag : new Tag();
        }

        /// <summary>
        /// creates a new region with this brush
        /// </summary>
        /// <returns>New Region</returns>
        public InputRegion GetNewRegion(double x, double y, double width, double height,DynamicInputPackViewModel input_pack)
        {
            InputRegion newregion = GetNewRegion();
            newregion.RegionRect = new UIRegionRect(x, y, width, height) { Pack = input_pack };
            return newregion;
        }

        public InputRegion GetNewRegion()
        {
            InputRegion newregion = new InputRegion();
            UpdateRegion(newregion);
            return newregion;
        }

        public bool IsValid()
        {
            return SelectedEmulatedDevice != null && UseKey ? SelectedEmulatedKey != null : SelectedTag != null;
        }
        #endregion
    }
}
