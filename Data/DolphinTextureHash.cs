using System;
using System.Text.RegularExpressions;

namespace DolphinDynamicInputTextureCreator.Data
{
    /// <summary>
    /// Gives specific information about the Dolphin textures hash.
    /// </summary>
    public class DolphinTextureHash
    {
        public enum ImageFormat:int { Unknown = -1, I4 = 0, I8 = 1, IA4 = 2, IA8 = 3, RGB565 = 4, RGB5A3 = 5, RGBA32 = 6, C4 = 8, C8 = 9, C14X2 = 10, CMPR = 14 }

        #region Properties

        private string _full_hash;
        /// <summary>
        /// The FullHash of the texture, as defined by Dolphin.
        /// </summary>
        public string FullHash
        {
            get => _full_hash;
            set
            {
                _full_hash = value;
                GetDataFromHash();
            }
        }

        /// <summary>
        /// Indicates whether the hash corresponds to the Dolphin texture hash format.
        /// </summary>
        public bool IsValid { get; private set; } = false;

        /// <summary>
        /// specifies the width of the original texture, not the actual width of the image file.
        /// </summary>
        public int ImageWidth { get; private set; }

        /// <summary>
        /// specifies the height of the original texture, not the actual height of the image file.
        /// </summary>
        public int ImageHeight { get; private set; }

        /// <summary>
        /// Indicates whether the texture is a Mipmap.
        /// </summary>
        public bool IsMipmap { get; private set; }

        /// <summary>
        /// Indicates whether the texture is a Arbitrary Mipmap.
        /// </summary>
        public bool IsArbitraryMipmap { get; private set; }

        /// <summary>
        /// indicates the level of the mipmap.
        /// 0 = Main file
        /// </summary>
        public int MipmapLevel { get; private set; }

        /// <summary>
        /// specifies 1 to 2 16 character hashes, separated by _.
        /// hash 2 can consist of the placeholder $.
        /// </summary>
        public string HashValue { get; private set; }

        /// <summary>
        /// specifies the used image format.
        /// </summary>
        public ImageFormat Format { get; private set; }

        #endregion

        #region Constructor

        public DolphinTextureHash() { }

        public DolphinTextureHash(string hash) { FullHash = hash; }

        #endregion

        #region FromHashCheck

        private static readonly Regex HashFormat = new Regex(@"tex1_(?'X'\d+)x(?'Y'\d+)_(?'M'm_)?(?'H'(?:_?[0-9a-fA-F]{16})+(?:_[$])?)_(?'F'\d+)(?:(?'A'_arb)?(?:_mip(?'Ms'\d+))?)");

        /// <summary>
        /// recognizes the data from the hash
        /// </summary>
        private void GetDataFromHash()
        {
            Match match = HashFormat.Match(FullHash);
            IsValid = match.Success;
            if (IsValid)
            {
                ImageWidth = Int32.Parse(match.Groups["X"].Value);
                ImageHeight = Int32.Parse(match.Groups["Y"].Value);
                IsMipmap = match.Groups["M"].Success;
                IsArbitraryMipmap = match.Groups["A"].Success;
                if (IsMipmap && match.Groups["Ms"].Success)
                {
                    MipmapLevel = Int32.Parse(match.Groups["Ms"].Value);
                }
                else
                {
                    MipmapLevel = 0;
                } 
                HashValue = match.Groups["H"].Value;
                Format = (ImageFormat)Int32.Parse(match.Groups["F"].Value);
            }
            else
            {
                ImageWidth = ImageHeight = MipmapLevel = 0;
                IsMipmap = IsArbitraryMipmap = false;
                Format = ImageFormat.Unknown;
            }
        }

        #endregion

    }
}
