using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace DolphinDynamicInputTexture.Data
{
    [JsonObject(IsReference = true)]
    public class HostDevice : Other.PropertyChangedBase
    {

        #region Properties

        /// <summary>
        /// The name of this host device, ex: "DInput/0/Keyboard Mouse"
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
        /// The keys mapped for this device
        /// </summary>
        public ObservableCollection<HostKey> HostKeys
        {
            get => _host_keys;
            set
            {
                _host_keys = value;
                OnPropertyChanged(nameof(HostKeys));
            }
        }
        private ObservableCollection<HostKey> _host_keys = new ObservableCollection<HostKey>();

        #endregion

    }
}
