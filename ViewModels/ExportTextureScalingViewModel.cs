using DolphinDynamicInputTexture.Data;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DolphinDynamicInputTextureCreator.ViewModels
{
    public class ExportTextureScalingViewModel : Other.PropertyChangedBase
    {
        /// <summary>
        /// Possible texture scaling modes.
        /// </summary>
        internal enum Modes
        {
            None, NearestNeighbor, Bicubic, Bilinear
        }

        /// <summary>
        /// The currently selected export texture scaling mode.
        /// </summary>
        internal Modes SelectedScalingMode
        {
            get => _selected_scaling_mode;
            set
            {
                _selected_scaling_mode = value;
                SetExportTextureScaling();
                OnPropertyChanged(nameof(SelectedScalingMode));
            }
        }
        private Modes _selected_scaling_mode = Modes.None;

        /// <summary>
        /// Scaling factor that the exported image should have.
        /// </summary>
        public int SelectedScalingFactor { get; set; } = 4;

        #region UI Helper

        public string[] ScalingModesHelper
        {
            get => Enum.GetNames(typeof(Modes));
        }

        public string SelectedScalingModeHelper
        {
            get => SelectedScalingMode.ToString();
            set
            {
                SelectedScalingMode = Enum.Parse<Modes>(value);
                OnPropertyChanged(nameof(SelectedScalingModeHelper));
            }
        }

        public int[] ScalingFactorHelper => new int[] { 1, 2, 3, 4, 5, 6, 8, 10 };

        #endregion UI Helper

        /// <summary>
        /// set the export scale for each texture.
        /// </summary>
        private void SetExportTextureScaling()
        {
            switch (SelectedScalingMode)
            {
                case Modes.None:
                    DynamicInputTextureEvents.DynamicInputTextureExportProcessor = null;
                    break;

                case Modes.NearestNeighbor:
                case Modes.Bicubic:
                case Modes.Bilinear:
                    DynamicInputTextureEvents.DynamicInputTextureExportProcessor = DefaultScalingProcessor;
                    break;

                default:
                    break;
            }
        }

        public bool DefaultScalingProcessor(string savepath, DynamicInputTexture dynamicinputtexture)
        {
            int Scaling = SelectedScalingFactor;

            //Should we use scaling?
            if (Scaling == dynamicinputtexture.ImageWidthScaling) return false;

            //The actual scaling.
            using (Bitmap newImage = new Bitmap(dynamicinputtexture.HashProperties.ImageWidth * Scaling, dynamicinputtexture.HashProperties.ImageHeight * Scaling))
            {
                using (Bitmap Image = new Bitmap(dynamicinputtexture.TexturePath))
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    switch (SelectedScalingMode)
                    {
                        case Modes.NearestNeighbor:
                            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            break;

                        case Modes.Bicubic:
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            break;

                        case Modes.Bilinear:
                            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                            break;

                        default:
                            break;
                    }

                    graphics.DrawImage(Image, 0, 0, newImage.Width, newImage.Height);
                }
                newImage.Save(savepath, System.Drawing.Imaging.ImageFormat.Png);
            }

            return true;
        }
    }
}