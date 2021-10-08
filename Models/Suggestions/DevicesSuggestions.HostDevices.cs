using System.Collections.Generic;

namespace DolphinDynamicInputTextureCreator.Models.Suggestions
{
    public partial class DevicesSuggestions
    {

        public List<string> HostDeviceNames { get; set; } = GetFallbackHostDeviceNameData();

        public Dictionary<string, List<string>> HostDevicesKeys { get; set; } = GetFallbackHostDevicesKeyData();

        private static List<string> GetFallbackHostDeviceNameData()
        {
            List<string> list = new List<string>();

            foreach (string devicename in GetFallbackHostDeviceNameBase())
            {
                for (int i = 0; i < 4; i++)
                {
                    list.Add(string.Format(devicename, i));
                    if (!devicename.Contains("{0}"))
                        break;
                }
            }
            return list;
        }

        private static string[] GetFallbackHostDeviceNameBase() => new string[] {
            "XInput/{0}/Gamepad",
            "DInput/0/Keyboard Mouse",
            "XInput2/0/Virtual core pointer",
            "Bluetooth/{0}/Wii Remote"
        };

        private static Dictionary<string, List<string>> GetFallbackHostDevicesKeyData() => new Dictionary<string, List<string>>() {
            {
                "XInput",
                new List<string>(){
                    "`Buttons A`", "`Buttons B`", "`Buttons X`", "`Buttons Y`", "`Buttons Start`",
                    "`Shoulder L`", "`Shoulder R`",
                    "`Trigger L`", "`Trigger R`",
                    "`Thumb L`", "`Thumb R`",
                    "Start", "Back", "Guide",
                    "`Pad N`", "`Pad S`", "`Pad E`", "`Pad W`",
                    "`Left X-`", "`Left X+`", "`Left Y-`", "`Left Y+`",
                    "`Right X-`", "`Right X+`", "`Right Y-`", "`Right Y+`"
                }
            },
                {
                "Keyboard Mouse",
                new List<string>(){
                    "`Click0`", "`Click1`", "`Click2`", "`Click3`", "`Click4`",
                    "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                    "`1`", "`2`", "`3`", "`4`", "`5`", "`6`", "`7`", "`8`", "`9`", "`0`",
                    "`F1`", "`F2`", "`F3`", "`F4`", "`F5`", "`F6`", "`F7`", "`F8`", "`F9`", "`F10`", "`F11`", "`F12`",
                    "Up", "Down", "Left", "Right", "Shift", "Ctrl", "SPACE", "Alt", "TAB", "RETURN", "APOSTROPHE", "BACK", "SLASH", "BACKSLASH", "PERIOD",
                    "COMMA", "EQUALS", "LBRACKET", "RBRACKET", "SEMICOLON", "ESC", "DELETE", "END", "NEXT", "INSERT", "TAB", "HOME", "PRIOR", "PAUSE",
                    "`NUMPAD1`", "`NUMPAD2`", "`NUMPAD3`", "`NUMPAD4`", "`NUMPAD5`", "`NUMPAD6`", "`NUMPAD7`", "`NUMPAD8`", "`NUMPAD9`", "`NUMPAD0`",
                    "DIVIDE", "MULTIPLY", "SUBTRACT", "ADD", "NUMPADENTER", "DECIMAL"
                }
            },
                {
                "Wii Remote",
                new List<string>(){
                    "A", "B", "`1`", "`2`", "`-`", "`+`", "HOME", "Up", "Down", "Left", "Right",
                    "`Nunchuk C`", "`Nunchuk Z`", "`Nunchuk X-`", "`Nunchuk X+`", "`Nunchuk Y-`", "`Nunchuk Y+`",
                    "`Classic A`", "`Classic B`", "`Classic X`", "`Classic Y`", "`Classic -`", "`Classic +`", "`Classic HOME`", "`Classic L`",
                    "`Classic R`", "`Classic ZL`", "`Classic ZR`", "`Classic Up`", "`Classic Down`", "`Classic Left`", "`Classic Right`",
                    "`Classic Left X-`", "`Classic Left X+`", "`Classic Left Y-`", "`Classic Left Y+`",
                    "`Classic Right X-`", "`Classic Right X+`", "`Classic Right Y-`", "`Classic Right Y+`"
                }
            },
                {
                "DInput",
                new List<string>(){
                    "`Buttons 1`", "`Buttons 2`", "`Buttons 3`", "`Buttons 4`", "`Buttons 5`", "`Buttons 6`", "`Buttons 7`", "`Buttons 8`",
                    "`Buttons 9`", "`Buttons 10`", "`Buttons 11`", "`Buttons 12`", "`Buttons 13`", "`Buttons 14`", "`Buttons 15`",
                    "`Hat 0 N`", "`Hat 0 S`", "`Hat 0 E`", "`Hat 0 W`",
                    "`Axis X-`", "`Axis X+`", "`Axis Y-`", "`Axis Y+`",
                    "`Axis Xr-`", "`Axis Xr+`", "`Axis Yr-`", "`Axis Yr+`"
                }
            },
                {
                "DSUClient",
                new List<string>(){
                    "Circle", "Cross", "Square", "Triangle", "Options", "Share", "PS", "`Touch Button`",
                    "`L1`", "`L2`", "`L3`","`R1`", "`R2`", "`R3`",
                    "`Pad N`", "`Pad S`", "`Pad E`", "`Pad W`",
                    "`Left X-`", "`Left X+`", "`Left Y-`", "`Left Y+`",
                    "`Right X-`", "`Right X+`", "`Right Y-`", "`Right Y+`"
                }
            },
                {
                "evdev",
                new List<string>(){
                    "SOUTH", "EAST", "NORTH", "WEST", "TL", "TR", "`TL2`", "`TR2`", "START", "SELECT", "MODE", "LEFT", "UP", "RIGHT", "DOWN", "THUMBL", "THUMBR",
                    "`Axis 0-`", "`Axis 0+`", "`Axis 1-`", "`Axis 1+`", "`Axis 3-`", "`Axis 3+`", "`Axis 4-`", "`Axis 4+`", "`Axis 6-`", "`Axis 6+`", "`Axis 7-`", "`Axis 7+`"
                }
            },
                {
                "XInput2",
                new List<string>(){
                    "`Click 1`", "`Click 2`", "`Click 3`", "`Click 8`", "`Click 9`",
                    "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                    "`1`", "`2`", "`3`", "`4`", "`5`", "`6`", "`7`", "`8`", "`9`", "`0`",
                    "`F1`", "`F2`", "`F3`", "`F4`", "`F5`", "`F6`", "`F7`", "`F8`", "`F9`", "`F10`", "`F11`", "`F12`",
                    "Up", "Down", "Left", "Right", "`Alt_L`", "Alt", "`ISO_Level3_Shift`", "Menu", "apostrophe", "BackSpace", "backslash", "comma", "`Control_L`",
                    "`Control_R`", "Ctrl", "Escape", "Tab", "equal", "grave", "bracketleft", "bracketright", "`Shift_L`", "`Shift_R`", "Shift", "minus", "period",
                    "Return", "semicolon", "slash", "space", "`Super_L`", "`Super_R`", "less", "Delete", "End", "Next", "SUBTRACT", "Insert", "Prior", "Pause",
                    "`KP_Insert`", "`KP_End`", "`KP_Down`", "`KP_Next`", "`KP_Left`", "`KP_Begin`", "`KP_Right`", "`KP_Home`", "`KP_Up`", "`KP_Prior`", "`KP_Add`",
                    "`KP_Delete`", "`KP_Divide`", "`KP_Multiply`", "`KP_Enter`", "`KP_Subtract`"
                }
            }
        };
    }
}
