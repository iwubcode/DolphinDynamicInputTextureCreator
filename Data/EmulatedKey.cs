namespace DolphinDynamicInputTextureCreator.Data
{
    public class EmulatedKey : Other.PropertyChangedBase, Interfaces.IDeviceKey
    {
        #region PROPERTIES

        /// <summary>
        /// The key name on the emulated machine, ex: "Buttons/A"
        /// </summary>
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        #endregion
    }
}
