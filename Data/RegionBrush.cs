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
        #endregion
    }
}
