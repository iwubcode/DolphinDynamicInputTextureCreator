using DolphinDynamicInputTextureCreator.Other;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.Data
{
    public class HostKey : Other.PropertyChangedBase, Interfaces.IDeviceKey, Interfaces.IImage
    {

        #region Properties

        /// <summary>
        /// The name of the key on the host machine, ex: "`Shoulder L`"
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

        /// <summary>
        /// A path to a texture that will be scaled up or down wherever there is a mapped host key
        /// in the original provided texture
        /// </summary>
        private string _texture_path;
        public string TexturePath
        {
            get => _texture_path;
            set
            {
                _texture_path = value;
                OnPropertyChanged(nameof(TexturePath));
            }
        }

        /// <summary>
        /// the reladive image paht, starting from the packages directory.
        /// </summary>
        private string _relative_texture_path;
        public string RelativeTexturePath
        {
            get => _relative_texture_path;
            set
            {
                _relative_texture_path = value;
                OnPropertyChanged(nameof(RelativeTexturePath));
            }
        }

        #endregion

    }
}
