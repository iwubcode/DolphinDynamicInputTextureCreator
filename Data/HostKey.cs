using DolphinDynamicInputTextureCreator.Other;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.Data
{
    public class HostKey : Other.PropertyChangedBase
    {
        /// <summary>
        /// The name of the key on the host machine, ex: "`Shoulder L`"
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
        /// A path to a texture that will be scaled up or down wherever there is a mapped host key
        /// in the original provided texture
        /// </summary>
        private string _texture_path;
        public string TexturePath
        {
            get
            {
                return _texture_path;
            }
            set
            {
                _texture_path = value;
                OnPropertyChanged(nameof(TexturePath));
            }
        }
    }
}
