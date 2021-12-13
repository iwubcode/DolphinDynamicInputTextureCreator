using System.Collections.Generic;

namespace DolphinDynamicInputTextureCreator.Models.Suggestions
{
    public partial class DevicesSuggestions
    {

        public List<string> TagNames { get; set; } = GetFallbackTagNameData();

        private static List<string> GetFallbackTagNameData() => new List<string>() { "Pressed", "Neutral", "Left+Right", "Up+Down", "Up+Right", "Up+Left", "Down+Right", "Down+Left"};
    }
}
