using System.Windows.Media;

namespace DolphinDynamicInputTextureCreator.Data
{
    public class EmulatedKey : Other.PropertyChangedBase
    {
        #region PROPERTIES
        /// <summary>
        /// The key name on the emulated machine, ex: "Buttons/A"
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
            }
        }

        /// <summary>
        /// The color in the texture associated with the emulated key
        /// </summary>
        private Color _region_color;
        public Color RegionColor
        {
            get
            {
                return _region_color;
            }
            set
            {
                _region_color = value;
                OnPropertyChanged(nameof(RegionColor));
            }
        }
        #endregion
    }
}
