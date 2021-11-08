using DolphinDynamicInputTexture.Data;
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
            if (Textures.Selected.Regions.Count > 0)
            {
                foreach (InputRegion r in Textures.Selected?.Regions)
                {
                    if (r.RegionRect.Equals(new InputRegionRect(0, 0, Textures.Selected.ImageWidth, Textures.Selected.ImageHeight)))
                    {
                        SelectedRegionBrush.UpdateRegion(r);
                        return;
                    }
                }
            }

            InputRegion f = SelectedRegionBrush.GetNewRegion(0, 0, Textures.Selected.ImageWidth, Textures.Selected.ImageHeight, this);
            Textures.Selected.Regions.Add(f);
        }

        private bool CanFillRegion()
        {
            if (!Textures.ValidSelection || SelectedRegionBrush.SelectedEmulatedDevice == null || SelectedRegionBrush.SelectedEmulatedKey == null)
                return false;

            if (Textures.Selected.Regions.Count > 0)
            {
                foreach (InputRegion r in Textures.Selected.Regions)
                {
                    if (r.RegionRect.Equals(new InputRegionRect(0,0, Textures.Selected.ImageWidth, Textures.Selected.ImageHeight)))
                    {
                        return true;
                    }
                }
                return false;
            }

            return true;
        }

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
