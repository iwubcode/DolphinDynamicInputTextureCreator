using DolphinDynamicInputTextureCreator.Other;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.Data
{
    [JsonObject(IsReference = true)]
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

                if (_name.Contains("Keyboard Mouse"))
                {
                    List<string> suggestions = new List<string>();

                    // Mouse
                    suggestions.Add("Click 0");
                    suggestions.Add("Click 1");
                    suggestions.Add("Click 2");

                    // Keyboard
                    char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

                    foreach (char c in alpha)
                    {
                        suggestions.Add(c.ToString());
                    }
                    suggestions.Add("Shift");
                    suggestions.Add("Ctrl");
                    suggestions.Add("SPACE");
                    suggestions.Add("UP");
                    suggestions.Add("DOWN");
                    suggestions.Add("LEFT");
                    suggestions.Add("RIGHT");

                    suggestions.Sort();

                    SearchSuggestions = new ObservableCollection<string>(suggestions);
                }
                else if (_name.Contains("XInput"))
                {
                    List<string> suggestions = new List<string>();
                    suggestions.Add("`Pad N`");
                    suggestions.Add("`Pad S`");
                    suggestions.Add("`Pad E`");
                    suggestions.Add("`Pad W`");
                    suggestions.Add("`Button A`");
                    suggestions.Add("`Button B`");
                    suggestions.Add("`Button X`");
                    suggestions.Add("`Button Y`");
                    suggestions.Add("`Shoulder L`");
                    suggestions.Add("`Shoulder R`");
                    suggestions.Add("`Trigger L`");
                    suggestions.Add("`Trigger R`");
                    suggestions.Add("`Thumb L`");
                    suggestions.Add("`Thumb R`");
                    suggestions.Add("Start");
                    suggestions.Add("Back");
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
        [JsonIgnore]
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
        [JsonIgnore]
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
