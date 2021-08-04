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
                    suggestions.Add("Click 3");
                    suggestions.Add("Click 4");

                    // Keyboard
                    char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

                    foreach (char c in alpha)
                    {
                        suggestions.Add(c.ToString());
                    }

                    char[] Numbers = "1234567890".ToCharArray();

                    foreach (char c in Numbers)
                    {
                        suggestions.Add(c.ToString());
                    }

                    suggestions.Add("UP");
                    suggestions.Add("DOWN");
                    suggestions.Add("LEFT");
                    suggestions.Add("RIGHT");

                    suggestions.Add("Shift");
                    suggestions.Add("Ctrl");
                    suggestions.Add("SPACE");
                    suggestions.Add("Alt");
                    suggestions.Add("TAP");
                    suggestions.Add("RETURN");

                    suggestions.Add("APOSTROPHE");
                    suggestions.Add("BACK");
                    suggestions.Add("SLASH");
                    suggestions.Add("BACKSLASH");
                    suggestions.Add("PERIOD");
                    suggestions.Add("COMMA");
                    suggestions.Add("EQUALS");
                    suggestions.Add("LBRACKET");
                    suggestions.Add("RBRACKET");
                    suggestions.Add("SEMICOLON");

                    //numpad
                    foreach (char c in Numbers)
                    {
                        suggestions.Add("NUMPAD" + c.ToString());
                    }
                    suggestions.Add("DIVIDE");
                    suggestions.Add("MULTIPLY");
                    suggestions.Add("SUBTRACT");
                    suggestions.Add("ADD");
                    suggestions.Add("NUMPADENTER");
                    suggestions.Add("DECIMAL");

                    //suggestions.Sort(); only makes it messy :|

                    SearchSuggestions = new ObservableCollection<string>(suggestions);
                }
                else if (_name.Contains("XInput"))
                {
                    List<string> suggestions = new List<string>();
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
                    suggestions.Add("Guide");
                    suggestions.Add("`Pad N`");
                    suggestions.Add("`Pad S`");
                    suggestions.Add("`Pad E`");
                    suggestions.Add("`Pad W`");
                    suggestions.Add("`Left X-`");
                    suggestions.Add("`Left X+`");
                    suggestions.Add("`Left Y-`");
                    suggestions.Add("`Left Y+`");
                    suggestions.Add("`Right X-`");
                    suggestions.Add("`Right X+`");
                    suggestions.Add("`Right Y-`");
                    suggestions.Add("`Right Y+`");
                    //suggestions.Sort(); only makes it messy :/

                    SearchSuggestions = new ObservableCollection<string>(suggestions);
                }
                else if (_name.Contains("Wii Remote"))
                {
                    List<string> suggestions = new List<string>();

                    // Wiimote button presses
                    suggestions.Add("A");
                    suggestions.Add("B");
                    suggestions.Add("`1`");
                    suggestions.Add("`2`");
                    suggestions.Add("`-`");
                    suggestions.Add("`+`");
                    suggestions.Add("HOME");
                    suggestions.Add("Up");
                    suggestions.Add("Down");
                    suggestions.Add("Left");
                    suggestions.Add("Right");

                    // Nunchuk
                    suggestions.Add("`Nunchuk C`");
                    suggestions.Add("`Nunchuk Z`");
                    suggestions.Add("`Nunchuk X-`");
                    suggestions.Add("`Nunchuk X+`");
                    suggestions.Add("`Nunchuk Y-`");
                    suggestions.Add("`Nunchuk Y+`");

                    // Classic
                    suggestions.Add("`Classic A`");
                    suggestions.Add("`Classic B`");
                    suggestions.Add("`Classic X`");
                    suggestions.Add("`Classic Y`");
                    suggestions.Add("`Classic -`");
                    suggestions.Add("`Classic +`");
                    suggestions.Add("`Classic HOME`");
                    suggestions.Add("`Classic L`");
                    suggestions.Add("`Classic R`");
                    suggestions.Add("`Classic ZL`");
                    suggestions.Add("`Classic ZR`");
                    suggestions.Add("`Classic Up`");
                    suggestions.Add("`Classic Down`");
                    suggestions.Add("`Classic Left`");
                    suggestions.Add("`Classic Right`");
                    suggestions.Add("`Classic Left X-`");
                    suggestions.Add("`Classic Left X+`");
                    suggestions.Add("`Classic Left Y-`");
                    suggestions.Add("`Classic Left Y+`");
                    suggestions.Add("`Classic RightS X-`");
                    suggestions.Add("`Classic Right X+`");
                    suggestions.Add("`Classic Right Y-`");
                    suggestions.Add("`Classic Right Y+`");

                    //suggestions.Sort(); only makes it messy :/

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
