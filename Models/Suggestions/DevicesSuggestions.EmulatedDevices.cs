using System.Collections.Generic;

namespace DolphinDynamicInputTextureCreator.Models.Suggestions
{
    public partial class DevicesSuggestions
    {

        public List<string> EmulatedDeviceNames { get; set; } = GetFallbackEmulatedDeviceNameData();

        public Dictionary<string, List<string>> EmulatedDeviceKeys { get; set; } = GetFallbackEmulatedDeviceKeyData();

        private static List<string> GetFallbackEmulatedDeviceNameData()
        {
            List<string> list = new List<string>();

            foreach (string devicename in GetFallbackEmulatedDeviceNameBase())
            {
                for (int i = 1; i <= 4; i++)
                {
                    list.Add(string.Format(devicename, i));
                    if (!devicename.Contains("{0}"))
                        break;
                }
            }
            return list;
        }

        private static string[] GetFallbackEmulatedDeviceNameBase() => new string[] {
            "Wiimote{0}",
            "GCPad{0}",
            "GBA{0}",
            "GCKeyboard{0}"
        };


        private static Dictionary<string, List<string>> GetFallbackEmulatedDeviceKeyData() => new Dictionary<string, List<string>>() {
            {
                "Wiimote",
                new List<string>(){
                    "Buttons/A", "Buttons/B", "Buttons/1", "Buttons/2", "Buttons/-", "Buttons/+", "Buttons/Home",
                    "D-Pad/Up", "D-Pad/Down", "D-Pad/Left", "D-Pad/Right",
                    "Swing/Up", "Swing/Down", "Swing/Left", "Swing/Right", "Swing/Forward",
                    "Shake/X", "Shake/Y", "Shake/Z",
                    "Nunchuk/Buttons/Z", "Nunchuk/Buttons/C",
                    "Nunchuk/Stick/Up", "Nunchuk/Stick/Down", "Nunchuk/Stick/Left", "Nunchuk/Stick/Right",
                    "Nunchuk/Shake/X", "Nunchuk/Shake/Y", "Nunchuk/Shake/Z",
                    "Nunchuk/Buttons/A", "Nunchuk/Buttons/B", "Nunchuk/Buttons/X", "Nunchuk/Buttons/Y", "Nunchuk/Buttons/ZL",
                    "Nunchuk/Buttons/ZR", "Nunchuk/Buttons/-", "Nunchuk/Buttons/+", "Nunchuk/Buttons/Home",
                    "Classic/Triggers/L", "Classic/Triggers/R",
                    "Classic/D-Pad/Up", "Classic/D-Pad/Down", "Classic/D-Pad/Left", "Classic/D-Pad/Right",
                    "Classic/Left Stick/Up", "Classic/Left Stick/Down", "Classic/Left Stick/Left", "Classic/Left Stick/Right",
                    "Classic/Right Stick/Up", "Classic/Right Stick/Down", "Classic/Right Stick/Left", "Classic/Right Stick/Right"
                }
            },            
            {
                "GCPad",
                new List<string>(){
                    "Buttons/A", "Buttons/B", "Buttons/X", "Buttons/Y", "Buttons/Z", "Buttons/Start",
                    "D-Pad/Up", "D-Pad/Down", "D-Pad/Left", "D-Pad/Right",
                    "Main Stick/Up", "Main Stick/Down", "Main Stick/Left", "Main Stick/Right",
                    "C-Stick/Up", "C-Stick/Down", "C-Stick/Left", "C-Stick/Right",
                    "Triggers/L", "Triggers/R"
                }
            },
            {
                "GBA",
                new List<string>(){
                    "Buttons/A", "Buttons/B", "Buttons/L", "Buttons/R", "Buttons/SELECT", "Buttons/START",
                    "D-Pad/Up", "D-Pad/Down", "D-Pad/Left", "D-Pad/Right"
                }
            },
            {
                "GCKeyboard",
                GenGCKeyboardList()
            }
        };

        private static List<string> GenGCKeyboardList()
        {
            string[] keys = new string[] {
                "HOME", "END", "PGUP", "PGDN", "SCR LK",
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
                "Up", "Down", "Left", "Right", "ESC", "INSERT", "DELETE", ";", "BACKSPACE", "TAB", "CAPS LOCK", "L SHIFT", "R SHIFT", "L CTRL", "R ALT", "L WIN",
                "SPACE", "R WIN", "MENU", "ENTER","-", "`", "PRT SC", "'", "[", "]", "EQUALS", "*", ",", ".", "/", @"\",
                "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12"
            };
            List<string> keylist = new List<string>();
            foreach (string key in keys)
                keylist.Add("Keys/" + key);

            return keylist;
        }

    }
}