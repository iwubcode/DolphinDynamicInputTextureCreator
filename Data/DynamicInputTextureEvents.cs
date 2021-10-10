using System;
using System.Collections.Generic;
using System.Text;

namespace DolphinDynamicInputTextureCreator.Data
{
    public static class DynamicInputTextureEvents
    {
        /// <summary>
        /// Is triggered when an image is not found.
        /// </summary>
        /// <returns>true = image was reassigned</returns>
        public static ImageNotExistAction ImageNotExist { get; set; }

        public delegate bool ImageNotExistAction(Interfaces.IImage image, string details);

    }
}
