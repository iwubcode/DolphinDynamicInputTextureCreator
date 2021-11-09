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
            region.Key = SelectedEmulatedKey;
        }
        /// <summary>
        /// creates a new region with this brush
        /// </summary>
        /// <returns>New Region</returns>
        public InputRegion GetNewRegion(double x, double y, double width, double height,DynamicInputPackViewModel input_pack)
        {
            return new InputRegion() { RegionRect = new UIRegionRect(x, y, width, height) { Pack = input_pack }, Device = SelectedEmulatedDevice, Key = SelectedEmulatedKey };
        }

        public bool IsValid()
        {
            return SelectedEmulatedDevice != null && SelectedEmulatedKey != null;
        }
        #endregion
    }
}
