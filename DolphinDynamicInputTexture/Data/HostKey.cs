using DolphinDynamicInputTexture.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DolphinDynamicInputTexture.Data
{
    public class HostKey : Other.PropertyChangedBase, IName, IImage, ITagable, IEquatable<HostKey>
    {

        #region Properties

        /// <summary>
        /// The name of the key on the host device, ex: "`Shoulder L`"
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _name;

        /// <summary>
        /// The tag of this host device.
        /// </summary>
        public Tag Tag
        {
            get => _tag ??= new Tag();
            set
            {
                _tag = value;
                OnPropertyChanged(nameof(Tag));
            }
        }
        private Tag _tag;

        /// <summary>
        /// A path to a texture that will be scaled up or down wherever there is a mapped host key
        /// in the original provided texture
        /// </summary>
        public string TexturePath
        {
            get => _texture_path;
            set
            {
                _texture_path = value;
                OnPropertyChanged(nameof(TexturePath));
            }
        }
        private string _texture_path;

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

        public bool Equals([AllowNull] HostKey other)
        {
            return other != null && other.Name == Name && other.Tag.Equals(Tag);
        }

        #endregion

    }
}
