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
        #endregion
    }
}
