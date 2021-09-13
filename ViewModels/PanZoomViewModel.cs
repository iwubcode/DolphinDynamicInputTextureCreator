using DolphinDynamicInputTextureCreator.Data;
using System;
using System.Windows;

namespace DolphinDynamicInputTextureCreator.ViewModels
{
    public class PanZoomViewModel : Other.PropertyChangedBase
    {
        /// <summary>
        /// The panning offset in the X direction
        /// </summary>
        private double _offsetX;
        public double OffsetX
        {
            get { return _offsetX; }
            set
            {
                _offsetX = value;
                OnPropertyChanged(nameof(OffsetX));
            }
        }

        /// <summary>
        /// The panning offset in the Y direction
        /// </summary>
        private double _offsetY;
        public double OffsetY
        {
            get { return _offsetY; }
            set
            {
                _offsetY = value;
                OnPropertyChanged(nameof(OffsetY));
            }
        }

        /// <summary>
        /// The input pack that the editing texture and the regions are pulled from
        /// </summary>
        private DynamicInputPack _dynamic_input_pack;
        public DynamicInputPack InputPack
        {
            get
            {
                return _dynamic_input_pack;
            }
            set
            {
                _dynamic_input_pack = value;
                OnPropertyChanged(nameof(InputPack));
            }
        }

        /// <summary>
        /// Returns true if we are panning
        /// </summary>
        private bool _is_panning = false;
        public bool IsPanning
        {
            get { return _is_panning; }
            set
            {
                _is_panning = value;
                OnPropertyChanged(nameof(IsPanning));
            }
        }

        private Data.RectRegion _currently_creating_region;
        private System.Nullable<Point> _last_pan_position;

        /// <summary>
        /// Given a point, will create a new region if we did not click on a separate region
        /// </summary>
        /// <param name="p"></param>
        public void StartCreatingRegion(Point p)
        {
            if (InputPack.SelectedRegionBrush.SelectedEmulatedDevice == null || InputPack.SelectedRegionBrush.SelectedEmulatedKey == null)
            {
                return;
            }

            //prevents a region from being created in another region.
            foreach (Data.RectRegion r in InputPack.EditingTexture.Regions)
            {
                if (p.X >= r.X && p.X < (r.X + r.Width) &&
                    p.Y >= r.Y && p.Y < (r.Y + r.Height))
                {
                    _currently_creating_region = null;
                    return;
                }
            }

            if (InputPack.SelectedRegionBrush.FillRegion)
            {
                _currently_creating_region = new Data.RectRegion() { ScaleFactor = InputPack.EditingTexture.ScaleFactor, X = 0, Y = 0, Height = InputPack.EditingTexture.ImageHeight, Width = InputPack.EditingTexture.ImageWidth, Device = InputPack.SelectedRegionBrush.SelectedEmulatedDevice, Key = InputPack.SelectedRegionBrush.SelectedEmulatedKey, OwnedTexture = InputPack.EditingTexture };
            }
            else
            {
                _currently_creating_region = new Data.RectRegion() { ScaleFactor = InputPack.EditingTexture.ScaleFactor, X = p.X, Y = p.Y, Height = 1, Width = 1, Device = InputPack.SelectedRegionBrush.SelectedEmulatedDevice, Key = InputPack.SelectedRegionBrush.SelectedEmulatedKey, OwnedTexture = InputPack.EditingTexture };
            }
            InputPack.EditingTexture.Regions.Add(_currently_creating_region);

            if (InputPack.SelectedRegionBrush.FillRegion)
            {
                // No need to continue, operation is finished
                _currently_creating_region = null;
            }
        }

        /// <summary>
        /// Updates the currently created region bounds
        /// </summary>
        /// <param name="p"></param>
        public void UpdateCreatingRegion(Point p)
        {
            if (_currently_creating_region == null)
            {
                return;
            }

            double diff_x = _currently_creating_region.X - p.X;
            double diff_y = _currently_creating_region.Y - p.Y;

            double left = _currently_creating_region.X;
            double top = _currently_creating_region.Y;
            if (diff_x > 0)
            {
                left = p.X;
            }

            if (diff_y > 0)
            {
                top = p.Y;
            }

            double width = Math.Abs(diff_x);
            double height = Math.Abs(diff_y);

            _currently_creating_region.X = left;
            _currently_creating_region.Y = top;
            _currently_creating_region.Width = width;
            _currently_creating_region.Height = height;
        }

        /// <summary>
        /// Stops creating a region
        /// </summary>
        public void StopCreatingRegion()
        {
            if (_currently_creating_region != null)
            {
                RemoveSmallRegion();
            }
            _currently_creating_region = null;
        }

        /// <summary>
        /// Removes the currently creating region if it has a default width and height
        /// </summary>
        private void RemoveSmallRegion()
        {
            if (_currently_creating_region.Width == 1 && _currently_creating_region.Height == 1)
            {
                InputPack.EditingTexture.Regions.Remove(_currently_creating_region);
            }
        }

        /// <summary>
        /// Begins the panning process
        /// </summary>
        /// <param name="p"></param>
        public void StartPanning(Point p)
        {
            IsPanning = true;
            _last_pan_position = p;
        }

        /// <summary>
        /// Updates the pan offset
        /// </summary>
        /// <param name="p"></param>
        public void UpdatePanning(Point p)
        {
            if (!_last_pan_position.HasValue)
            {
                return;
            }

            double diff_x = _last_pan_position.Value.X - p.X;
            double diff_y = _last_pan_position.Value.Y - p.Y;
            _last_pan_position = p;

            OffsetX = (OffsetX + ((diff_x * -1) * InputPack.EditingTexture.ScaleFactor));
            OffsetY = (OffsetY + ((diff_y * -1) * InputPack.EditingTexture.ScaleFactor));
        }

        /// <summary>
        /// Stops panning
        /// </summary>
        public void StopPanning()
        {
            IsPanning = false;
            _last_pan_position = null;
        }
    }
}
