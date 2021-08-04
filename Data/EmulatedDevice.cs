using DolphinDynamicInputTextureCreator.Other;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace DolphinDynamicInputTextureCreator.Data
{
    [JsonObject(IsReference = true)]
    public class EmulatedDevice : Other.PropertyChangedBase
    {
        /// <summary>
        /// The name of the emulated device, ex: "Wiimote1"
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

                if (_name.StartsWith("Wiimote"))
                {
                    List<string> suggestions = new List<string>();

                    // Wiimote button presses
                    suggestions.Add("Buttons/A");
                    suggestions.Add("Buttons/B");
                    suggestions.Add("Buttons/1");
                    suggestions.Add("Buttons/2");
                    suggestions.Add("Buttons/-");
                    suggestions.Add("Buttons/+");
                    suggestions.Add("D-Pad/Up");
                    suggestions.Add("D-Pad/Down");
                    suggestions.Add("D-Pad/Left");
                    suggestions.Add("D-Pad/Right");

                    // Wiimote simulated motions
                    suggestions.Add("Swing/Up");
                    suggestions.Add("Swing/Down");
                    suggestions.Add("Swing/Left");
                    suggestions.Add("Swing/Right");
                    suggestions.Add("Swing/Forward");
                    suggestions.Add("Shake/X");
                    suggestions.Add("Shake/Y");
                    suggestions.Add("Shake/Z");

                    // Nunchuk
                    suggestions.Add("Nunchuk/Buttons/Z");
                    suggestions.Add("Nunchuk/Buttons/C");
                    suggestions.Add("Nunchuk/Shake/X");
                    suggestions.Add("Nunchuk/Shake/Y");
                    suggestions.Add("Nunchuk/Shake/Z");

                    // Classic
                    suggestions.Add("Classic/Buttons/A");
                    suggestions.Add("Classic/Buttons/B");
                    suggestions.Add("Classic/Buttons/X");
                    suggestions.Add("Classic/Buttons/Y");
                    suggestions.Add("Classic/Buttons/ZL");
                    suggestions.Add("Classic/Buttons/ZR");
                    suggestions.Add("Classic/Buttons/-");
                    suggestions.Add("Classic/Buttons/+");
                    suggestions.Add("Classic/Triggers/L");
                    suggestions.Add("Classic/Triggers/R");
                    suggestions.Add("Classic/D-Pad/Up");
                    suggestions.Add("Classic/D-Pad/Down");
                    suggestions.Add("Classic/D-Pad/Left");
                    suggestions.Add("Classic/D-Pad/Right");

                    suggestions.Sort();

                    SearchSuggestions = new ObservableCollection<string>(suggestions);
                }
                else if (_name.StartsWith("GCPad"))
                {
                    List<string> suggestions = new List<string>();

                    suggestions.Add("Buttons/A");
                    suggestions.Add("Buttons/B");
                    suggestions.Add("Buttons/X");
                    suggestions.Add("Buttons/Y");
                    suggestions.Add("Buttons/Z");
                    suggestions.Add("Buttons/Start");
                    suggestions.Add("D-Pad/Up");
                    suggestions.Add("D-Pad/Down");
                    suggestions.Add("D-Pad/Left");
                    suggestions.Add("D-Pad/Right");

                    suggestions.Add("Triggers/L");
                    suggestions.Add("Triggers/R");

                    suggestions.Sort();

                    SearchSuggestions = new ObservableCollection<string>(suggestions);
                }
                else if (_name.StartsWith("GBA"))
                {
                    List<string> suggestions = new List<string>();

                    suggestions.Add("Buttons/B");
                    suggestions.Add("Buttons/A");
                    suggestions.Add("Buttons/L");
                    suggestions.Add("Buttons/R");
                    suggestions.Add("Buttons/SELECT");
                    suggestions.Add("Buttons/START");
                    suggestions.Add("D-Pad/Up");
                    suggestions.Add("D-Pad/Down");
                    suggestions.Add("D-Pad/Left");
                    suggestions.Add("D-Pad/Right");

                    //suggestions.Sort(); only makes it messy :/

                    SearchSuggestions = new ObservableCollection<string>(suggestions);
                }
            }
        }

        /// <summary>
        /// Emulated keys accounted for by this device
        /// </summary>
        private ObservableCollection<EmulatedKey> _keys = new ObservableCollection<EmulatedKey>();
        public ObservableCollection<EmulatedKey> EmulatedKeys
        {
            get
            {
                return _keys;
            }
            set
            {
                _keys = value;
                OnPropertyChanged(nameof(EmulatedKeys));
            }
        }

        private EmulatedKey _selected_key;
        public EmulatedKey SelectedKey
        {
            get { return _selected_key; }
            set
            {
                _selected_key = value;
                OnPropertyChanged(nameof(SelectedKey));
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
                    _delete_key_command = new RelayCommand(param => DeleteKey((EmulatedKey)param));
                }
                return _delete_key_command;
            }
        }

        private void DeleteKey(EmulatedKey key)
        {
            EmulatedKeys.Remove(key);
        }

        private ICommand _add_key_command;
        [JsonIgnore]
        public ICommand AddKeyCommand
        {
            get
            {
                if (_add_key_command == null)
                {
                    _add_key_command = new RelayCommand(AddKey);
                }
                return _add_key_command;
            }
        }

        private void AddKey(object obj)
        {
            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = ""
                });
        }
        #endregion
    }
}
