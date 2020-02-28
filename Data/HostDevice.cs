using DolphinDynamicInputTextureCreator.Other;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.Data
{
    public class HostDevice : Other.PropertyChangedBase
    {
        /// <summary>
        /// The name of this host device, ex: "DInput/0/Keyboard Mouse"
        /// </summary>
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));

                if (_name.EndsWith("Keyboard Mouse"))
                {
                    List<string> suggestions = new List<string>();
                    suggestions.Add("A");
                    suggestions.Add("B");
                    suggestions.Add("C");
                    suggestions.Add("D");
                    suggestions.Add("E");
                    suggestions.Add("F");
                    suggestions.Sort();

                    SearchSuggestions = new ObservableCollection<string>(suggestions);
                }
            }
        }

        /// <summary>
        /// The keys mapped for this device
        /// </summary>
        private ObservableCollection<HostKey> _host_keys = new ObservableCollection<HostKey>();

        public ObservableCollection<HostKey> HostKeys
        {
            get
            {
                return _host_keys;
            }
            set
            {
                _host_keys = value;
                OnPropertyChanged(nameof(HostKeys));
            }
        }

        private ObservableCollection<string> _search_suggestions = new ObservableCollection<string>();
        public ObservableCollection<string> SearchSuggestions
        {
            get
            {
                return _search_suggestions;
            }
            set
            {
                _search_suggestions = value;
                OnPropertyChanged(nameof(SearchSuggestions));
            }
        }

        #region Commands
        private ICommand _delete_key_command;
        public ICommand DeleteKeyCommand
        {
            get
            {
                if (_delete_key_command == null)
                {
                    _delete_key_command = new RelayCommand(param => DeleteKey((HostKey)param));
                }
                return _delete_key_command;
            }
        }

        private void DeleteKey(HostKey key)
        {
            HostKeys.Remove(key);
        }
        #endregion
    }
}
