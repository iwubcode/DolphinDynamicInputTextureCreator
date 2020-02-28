using DolphinDynamicInputTextureCreator.Other;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace DolphinDynamicInputTextureCreator.Data
{
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
                    suggestions.Add("Buttons/A");
                    suggestions.Add("Buttons/B");
                    suggestions.Add("D-Pad/Up");
                    suggestions.Add("D-Pad/Down");
                    suggestions.Add("D-Pad/Left");
                    suggestions.Add("D-Pad/Right");
                    suggestions.Sort();

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
        public ICommand AddKeyCommand
        {
            get
            {
                if (_add_key_command == null)
                {
                    _add_key_command = new RelayCommand(param => AddKey((Color)param));
                }
                return _add_key_command;
            }
        }

        private void AddKey(Color color)
        {
            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = "",
                    RegionColor = color
                });
        }
        #endregion
    }
}
