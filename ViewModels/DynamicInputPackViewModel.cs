using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTexture.Properties;
using DolphinDynamicInputTextureCreator.Models;
using DolphinDynamicInputTextureCreator.Other;
using DolphinDynamicInputTextureCreator.ViewModels.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        /// The Tags mapped in this pack
        /// </summary>
        public UICollection<Tag> Tags
        {
            get => _tags ??= Tags = new UICollection<Tag>(base._tags);
            set
            {
                base._tags = _tags = value;
                Tags.Select(Selection.First);
                SelectedRegionBrush.SelectedTag = Tags.Selected;
                OnPropertyChanged(nameof(Tags));
            }
        }
        private new UICollection<Tag> _tags;

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
                if (SelectedRegion != null)
                {
                    SelectedRegionBrush.SelectedEmulatedDevice = SelectedRegion.Device;
                    SelectedRegionBrush.UseKey = SelectedRegion.Key == null || SelectedRegion.Key.Name != "";
                    SelectedRegionBrush.UseTag = SelectedRegion.Tag == null || SelectedRegion.Tag.Name != "";
                    SelectedRegionBrush.SelectedEmulatedKey = SelectedRegionBrush.UseKey ? SelectedRegion.Key : EmulatedDevices.Selected.EmulatedKeys.Count > 0 ? EmulatedDevices.Selected.EmulatedKeys[0] : null;
                    SelectedRegionBrush.SelectedTag = SelectedRegionBrush.UseTag ? SelectedRegion.Tag : Tags.Count > 0 ? Tags[0] : null;
                }
                OnPropertyChanged(nameof(SelectedRegion));
                OnPropertyChanged(nameof(IfCopyTypeOverwrite));
                OnPropertyChanged(nameof(IsRegionSelected));
            }
        }
        private InputRegion _selected_region;

        #endregion
        #region Commands

        #region SelectedTexture

        [JsonIgnore]
        public ICommand DeleteRegionCommand => new RelayCommand<InputRegion>(Region => GetRegionList(Region).Remove(Region));

        [JsonIgnore]
        public ICommand AutoSubRegionCommand => new RelayCommand<InputRegion>(Region => AutoSubRegion(Region));

        [JsonIgnore]
        public ICommand ResetScaleFactorCommand => new RelayCommand((x) => SetInitialZoom(Textures.Selected));

        [JsonIgnore]
        public ICommand FillRegionCommand => new RelayCommand((x) => FillRegion(), (x) => CanFillRegion());

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
                ((UIRegionRect)SelectedRegion.RegionRect).UpdateScale();
            }

        }

        private bool CanFillRegion()
        {
            if (!Textures.ValidSelection || !SelectedRegionBrush.IsValid() || IsRegionSelected && SelectedRegion.OwnedRegion != null)
                return false;

            return Textures.Selected.Regions.Count == 0 || Textures.Selected.Regions.Count == 1 & IsRegionSelected && !SelectedRegion.RegionRect.Equals(new InputRegionRect(0, 0, SelectedRegion.RegionRect.OwnedTexture.ImageWidth, SelectedRegion.RegionRect.OwnedTexture.ImageHeight));
        }

        public void AutoSubRegion(InputRegion region)
        {
            if (region.SubEntries.Count != 0)
                return;

            foreach (var texture in Textures)
            {
                foreach (var dreg in texture.Regions)
                {
                    if (dreg.SubEntries.Count > 0 && dreg.Device.Equals(region.Device) & dreg.Key.Equals(region.Key) & dreg.Tag.Equals(region.Tag))
                    {
                        foreach (var dsubreg in dreg.SubEntries)
                        {
                            InputRegion clonregion = (InputRegion)dsubreg.Clone();
                            clonregion.RegionRect = new UIRegionRect(clonregion.RegionRect) { Pack = this };
                            region.SubEntries.Add(clonregion);
                        }
                        return;
                    }
                }
            }

            int d = InputRegionRect.DecimalPlaces;
            InputRegionRect.DecimalPlaces = 3;
            double height = region.RegionRect.Height / 3;
            double width = region.RegionRect.Width / 3;
            for (int i = 1; i < 8; i += 2)
            {
                int x = i / 3;
                int y = i - 3 * x;
                InputRegion subregion = new InputRegion() { Key = region.Key, Tag = region.Tag, CopyType = region.CopyType, RegionRect = new UIRegionRect(region.RegionRect.X + (width * x), region.RegionRect.Y + (height * y), width, height) { Pack = this } };
                region.SubEntries.Add(subregion);

                if (region.SubEntries.Count == 4)
                    subregion.SubIndex -= 1;
            }
            InputRegionRect.DecimalPlaces = d;

        }

        #endregion

        #region SelectedRegion

        [JsonIgnore]
        public ICommand DeleteSelectedRegionCommand => new RelayCommand(x => GetRegionList(SelectedRegion).Remove(SelectedRegion), x => IsRegionSelected);

        [JsonIgnore]
        public ICommand UpdateSelectedRegionCommand => new RelayCommand(x => SelectedRegionBrush.UpdateRegion(SelectedRegion),x => IsRegionSelected & SelectedRegionBrush.IsValid());

        [JsonIgnore]
        public ICommand MoveUpSelectedRegionCommand => new RelayCommand(x => SelectedRegion.SubIndex += 1, x => IsRegionSelected && SelectedRegion.OwnedRegion != null && SelectedRegion.SubIndex < SelectedRegion.OwnedRegion.SubEntries.Count - 1);

        [JsonIgnore]
        public ICommand MoveDownSelectedRegionCommand => new RelayCommand(x => SelectedRegion.SubIndex -= 1, x => IsRegionSelected && SelectedRegion.OwnedRegion != null && SelectedRegion.SubIndex > 0);

        public IList<InputRegion> GetRegionList(InputRegion region)
        {
            if (region.OwnedRegion != null)
            {
                if (region.OwnedRegion.OwnedRegion != null)
                    return GetRegionList(region.OwnedRegion);

                return region.OwnedRegion.SubEntries;
            }
            return Textures.Selected.Regions;
        }

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
                        if (Region.RegionRect is UIRegionRect)
                            break;

                        Region.RegionRect = new UIRegionRect(Region.RegionRect) { Pack = this };
                        foreach (InputRegion SubRegion in Region.SubEntries)
                        {
                            SubRegion.RegionRect = new UIRegionRect(SubRegion.RegionRect) { Pack = this };
                        }
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
