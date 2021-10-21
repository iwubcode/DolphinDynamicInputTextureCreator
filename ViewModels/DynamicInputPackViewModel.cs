using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTextureCreator.Other;
using DolphinDynamicInputTextureCreator.ViewModels.Commands;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
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
            get => emulatedDevices ??= EmulatedDevices = new UICollection<EmulatedDevice>();
            set
            {
                emulatedDevices = value;
                EmulatedDevices.Select(Selection.First);
                OnPropertyChanged(nameof(EmulatedDevices));
            }
        }
        private UICollection<EmulatedDevice> emulatedDevices;

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
        public ICommand DeleteSelectedRegionCommand => new RelayCommand<RectRegion>(Region => Textures.Selected.Regions.Remove(Region));

        [JsonIgnore]
        public ICommand ResetScaleFactorCommand => new RelayCommand((x) => Textures.Selected.SetInitialZoom());

        [JsonIgnore]
        public ICommand FillRegionCommand => new ViewModels.Commands.RelayCommand((x) => FillRegion(), (x) => CanFillRegion());

        #region FillRegion

        /// <summary>
        /// adds a filled region.
        /// </summary>
        public void FillRegion()
        {
            if (Textures.Selected.Regions.Count > 0)
            {
                foreach (RectRegion r in Textures.Selected?.Regions)
                {
                    if (r.X == 0 && r.Y == 0 &&
                        r.Width == r.OwnedTexture.ImageWidth &&
                        r.Height == r.OwnedTexture.ImageHeight)
                    {
                        r.Device = SelectedRegionBrush.SelectedEmulatedDevice;
                        r.Key = SelectedRegionBrush.SelectedEmulatedKey;
                        return;
                    }
                }
            }

            RectRegion f = new RectRegion() { X = 0, Y = 0, Height = Textures.Selected.ImageHeight, Width = Textures.Selected.ImageWidth, Device = SelectedRegionBrush.SelectedEmulatedDevice, Key = SelectedRegionBrush.SelectedEmulatedKey, OwnedTexture = Textures.Selected };
            Textures.Selected.Regions.Add(f);
        }

        private bool CanFillRegion()
        {
            if (!Textures.ValidSelection || SelectedRegionBrush.SelectedEmulatedDevice == null || SelectedRegionBrush.SelectedEmulatedKey == null)
                return false;

            if (Textures.Selected.Regions.Count > 0)
            {
                foreach (RectRegion r in Textures.Selected.Regions)
                {
                    if (r.X == 0 && r.Y == 0 &&
                        r.Width == r.OwnedTexture.ImageWidth &&
                        r.Height == r.OwnedTexture.ImageHeight)
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

        #endregion

        #region Checks

        private void CheckTextureSelection(DynamicInputTexture Texture)
        {
            if (Texture != null && !File.Exists(Texture.TexturePath))
            {
                if (!Dialogs.ImageNotExistMessage(Texture, Path.GetFileName(Texture.TexturePath)))
                    Textures.Select(null);
            }
        }

        #endregion

    }
}
