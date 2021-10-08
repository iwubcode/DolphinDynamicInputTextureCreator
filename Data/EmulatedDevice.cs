using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DolphinDynamicInputTextureCreator.Data
{
    [JsonObject(IsReference = true)]
    public class EmulatedDevice : Other.PropertyChangedBase
    {

        #region PROPERTIES

        /// <summary>
        /// The name of the emulated device, ex: "Wiimote1"
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _name;

        /// <summary>
        /// Emulated keys accounted for by this device
        /// </summary>
        public ObservableCollection<EmulatedKey> EmulatedKeys
        {
            get => _emulated_keys;
            set
            {
                _emulated_keys = value;
                OnPropertyChanged(nameof(EmulatedKeys));
            }
        }
        private ObservableCollection<EmulatedKey> _emulated_keys = new ObservableCollection<EmulatedKey>();

        #endregion

    }
}
