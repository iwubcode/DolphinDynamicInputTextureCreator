using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTexture.Properties;
using DolphinDynamicInputTextureCreator.Models;
using DolphinDynamicInputTextureCreator.Other;
using DolphinDynamicInputTextureCreator.ViewModels.Commands;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.ViewModels
{
    public class DynamicInputPackViewModel : DynamicInputPack
    {

        #region View Model Properties

        /// <summary>
        /// The textures defined for dynamic input modification in this pack
        /// </summary>
        public new UICollection<DynamicInputTexture> Textures
        {
            get => _textures ??= Textures = new UICollection<DynamicInputTexture>(base.Textures);
            set
            {
                base.Textures = _textures = value;
                Textures.SelectedChanged = CheckTextureSelection;
                Textures.Select(Selection.First);
                OnPropertyChanged(nameof(Textures));
            }
        }
        private UICollection<DynamicInputTexture> _textures;

        /// <summary>
        /// The host devices mapped in this pack
        /// </summary>
        public new UICollection<HostDevice> HostDevices
        {
            get => _host_devices ??= HostDevices = new UICollection<HostDevice>(base.HostDevices);
            set
            {
                base.HostDevices = _host_devices = value;
                HostDevices.Select(Selection.First);
                OnPropertyChanged(nameof(HostDevices));
            }
        }
        private UICollection<HostDevice> _host_devices;

        /// <summary>
        /// The emulated devices mapped in this pack
        /// </summary>
        public UICollection<EmulatedDevice> EmulatedDevices
        {
            get => _emulated_devices ??= EmulatedDevices = new UICollection<EmulatedDevice>(base._emulated_devices);
            set
            {
                base._emulated_devices = _emulated_devices = value;
                EmulatedDevices.Select(Selection.First);
                SelectedRegionBrush.SelectedEmulatedDevice = EmulatedDevices.Selected;
                OnPropertyChanged(nameof(EmulatedDevices));
            }
        }
        private new UICollection<EmulatedDevice> _emulated_devices;

        /// <summary>
        /// The current emulated device / key "brush" chosen for writing regions to the texture
        /// </summary>
        private RegionBrush _selected_region_brush = new RegionBrush();
        public RegionBrush SelectedRegionBrush
        {
            get => _selected_region_brush;
            set
            {
                _selected_region_brush = value;
                OnPropertyChanged(nameof(SelectedRegionBrush));
            }
        }

        /// <summary>
        /// The currently selected Region.
        /// </summary>
        [JsonIgnore]
        public InputRegion SelectedRegion
        {
            get => _selected_region;
            set
            {
                _selected_region = value;
                OnPropertyChanged(nameof(SelectedRegion));
                OnPropertyChanged(nameof(IfCopyTypeOverwrite));
                OnPropertyChanged(nameof(IsRegionSelected));
            }
        }
        private InputRegion _selected_region;

        /// <summary>
        /// When adding a new texture, whether the hash should be pulled off of the filename
        /// </summary>
        private bool _should_get_hash_from_texture_filename = true;
        public bool ShouldGetHashFromTextureFilename
        {
            get => _should_get_hash_from_texture_filename;
            set
            {
                _should_get_hash_from_texture_filename = value;
                OnPropertyChanged(nameof(ShouldGetHashFromTextureFilename));
            }
        }

        #endregion
        #region Commands

        #region SelectedTexture

        [JsonIgnore]
        public ICommand DeleteRegionCommand => new RelayCommand<InputRegion>(Region => Textures.Selected.Regions.Remove(Region));

        [JsonIgnore]
        public ICommand ResetScaleFactorCommand => new RelayCommand((x) => SetInitialZoom(Textures.Selected));

        [JsonIgnore]
        public ICommand FillRegionCommand => new ViewModels.Commands.RelayCommand((x) => FillRegion(), (x) => CanFillRegion());

        /// <summary>
        /// adds a filled region.
        /// </summary>
        public void FillRegion()
        {
            InputRegion f = SelectedRegionBrush.GetNewRegion(0, 0, Textures.Selected.ImageWidth, Textures.Selected.ImageHeight, this);
            if (Textures.Selected.Regions.Count == 0)
            {
                Textures.Selected.Regions.Add(f);
            }
            else
            {
                SelectedRegion.RegionRect = f.RegionRect;
            }

        }

        private bool CanFillRegion()
        {
            if (!Textures.ValidSelection || !SelectedRegionBrush.IsValid())
                return false;

            return Textures.Selected.Regions.Count == 0 || Textures.Selected.Regions.Count == 1 & IsRegionSelected && !SelectedRegion.RegionRect.Equals(new InputRegionRect(0, 0, SelectedRegion.RegionRect.OwnedTexture.ImageWidth, SelectedRegion.RegionRect.OwnedTexture.ImageHeight));
        }

        #endregion

        #region SelectedRegion

        [JsonIgnore]
        public ICommand DeleteSelectedRegionCommand => new RelayCommand(x => Textures.Selected.Regions.Remove(SelectedRegion), x => IsRegionSelected);

        [JsonIgnore]
        public ICommand UpdateSelectedRegionCommand => new RelayCommand(x => SelectedRegionBrush.UpdateRegion(SelectedRegion),x => IsRegionSelected & SelectedRegionBrush.IsValid());

        [JsonIgnore]
        public bool IfCopyTypeOverwrite
        {
            get => SelectedRegion == null || SelectedRegion.CopyType == default;
            set
            {
                SelectedRegion.CopyType = value ? CopyTypeProperties.overwrite : CopyTypeProperties.overlay;
                OnPropertyChanged(nameof(IfCopyTypeOverwrite));
            }
        }

        [JsonIgnore]
        public bool IsRegionSelected => SelectedRegion != null;

        #endregion

        #endregion

        #region Checks

        private void CheckTextureSelection(DynamicInputTexture Texture)
        {
            if (Texture != null && !File.Exists(Texture.TexturePath))
            {
                if (!Dialogs.ImageNotExistMessage(Texture, Path.GetFileName(Texture.TexturePath)))
                    Textures.Select(null);
            }
            if (Texture != null && File.Exists(Texture.TexturePath))
            {
                foreach (InputRegion Region in Texture.Regions)
                {
                    if (Region.RegionRect is InputRegionRect)
                    {
                        Region.RegionRect = new UIRegionRect(Region.RegionRect) { Pack = this };
                    }
                }
                SelectedRegion = null;
                SetInitialZoom(Texture);
            }
        }

        #endregion

        #region Zoom
        /// <summary>
        /// The scale factor for how much to zoom the current texture and regions
        /// </summary>
        [JsonIgnore]
        public double ScaleFactor
        {
            get => _scale_factor;
            set
            {
                _scale_factor = Smooth(value, 2);
                foreach (InputRegion region in Textures.Selected.Regions)
                {
                    ((UIRegionRect)region.RegionRect).UpdateScale();
                }
                OnPropertyChanged(nameof(ScaleFactor));
            }
        }
        private double _scale_factor = 1;

        private static double Smooth(double value, int accuracy)
        {
            double factor = ((int)value * 10).ToString().Length;
            factor = Math.Pow(10, factor);
            return Math.Round(value / factor, accuracy + 1) * factor;
        }

        public void SetInitialZoom(DynamicInputTexture Texture, double absolutescale = 600)
        {
            if (Texture.ImageHeight <= 0 || Texture.ImageWidth <= 0)
                return;

            absolutescale /= (Texture.ImageHeight + Texture.ImageWidth) / 2;
            ScaleFactor = absolutescale;
        }

        #endregion

    }
}
