namespace DolphinDynamicInputTexture.Properties
{
    /// <summary>
    /// Describes how the exchange region should be transferred to the image.
    /// </summary>
    public enum CopyTypeProperties : int
    {
        /// <summary>
        /// The exchange region overwrite the previous image.
        /// </summary>
        overwrite = default,
        /// <summary>
        /// The image is overlaid with the exchange region.
        /// </summary>
        overlay
    }

    /// <summary>
    /// Describes under which conditions the exchange region and sub regions are transferred to the image.
    /// </summary>
    public enum BindTypeProperties : int
    {
        /// <summary>
        /// The exchange region is used if the key matches.
        /// </summary>
        single = default,
        /// <summary>
        /// All keys must match otherwise the sub regions will be used.
        /// </summary>
        multi
    }
}
