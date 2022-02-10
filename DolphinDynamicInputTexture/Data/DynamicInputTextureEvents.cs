
namespace DolphinDynamicInputTexture.Data
{
    public static class DynamicInputTextureEvents
    {
        /// <summary>
        /// Is triggered when an image is not found.
        /// </summary>
        /// <returns>true = image was reassigned</returns>
        public static ImageNotExistAction ImageNotExist { get; set; }

        public delegate bool ImageNotExistAction(Interfaces.IImage image, string details);

        /// <summary>
        /// Allows editing the terxtures during exporting.
        /// </summary>
        /// <param name="savepath">Path where the new texture should be saved</param>
        /// <param name="dynamicinputtexture">texture that should be scaled</param>
        /// <returns></returns>
        public delegate bool ExternalScalingProcesses(string savepath, DynamicInputTexture dynamicinputtexture);

        /// <summary>
        /// Allows editing the DynamicInputTextures during exporting.
        /// </summary>
        public static ExternalScalingProcesses DynamicInputTextureExportProcessor { get; set; }

    }
}
