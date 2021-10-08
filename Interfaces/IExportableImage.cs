using System;
using System.Collections.Generic;
using System.Text;

namespace DolphinDynamicInputTextureCreator.Interfaces
{
    interface IExportableImage
    {
        /// <summary>
        /// the absulute paht to image file.
        /// </summary>
        string TexturePath { get; set; }

        /// <summary>
        /// the reladive image paht, starting from the packages directory.
        /// </summary>
        string RelativeTexturePath { get; set; }
    }
}
