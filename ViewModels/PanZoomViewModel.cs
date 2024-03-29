﻿using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTextureCreator.Models;
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
        private DynamicInputPackViewModel _dynamic_input_pack;
        public DynamicInputPackViewModel InputPack
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

        private InputRegion _currently_creating_region;
        private System.Nullable<Point> _last_pan_position;

        /// <summary>
        /// Given a point, will create a new region if we did not click on a separate region
        /// </summary>
        /// <param name="p"></param>
        public void StartCreatingRegion(Point p)
        {
            if (!InputPack.SelectedRegionBrush.IsValid())
            {
                return;
            }

            //prevents a region from being created in another region.
            foreach (InputRegion r in InputPack.Textures.Selected.Regions)
            {
                if (r.RegionRect.Contains(p.X,p.Y))
                {
                    _currently_creating_region = null;
                    return;
                }
            }

            _currently_creating_region = InputPack.SelectedRegionBrush.GetNewRegion(p.X, p.Y, 2, 2, InputPack);
            InputPack.Textures.Selected.Regions.Add(_currently_creating_region);
        }


        public void StartCreatingSubRegion(Point p)
        {
            if (!InputPack.SelectedRegionBrush.IsValid())
            {
                return;
            }

            foreach (InputRegion r in InputPack.Textures.Selected.Regions)
            {
                if (r.RegionRect.Contains(p.X, p.Y) && InputPack.SelectedRegionBrush.SelectedEmulatedDevice.Equals(r.Device))
                {
                    //prevents a region from being created in another sub region.
                    foreach (InputRegion subr in r.SubEntries)
                    {
                        if (subr.RegionRect.Contains(p.X, p.Y))
                        {
                            _currently_creating_region = null;
                            return;
                        }
                    }

                    _currently_creating_region = InputPack.SelectedRegionBrush.GetNewRegion(p.X, p.Y, 2, 2, InputPack);
                    r.SubEntries.Add(_currently_creating_region);
                    return;
                }
            }
            _currently_creating_region = null;
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

            double diff_x = _currently_creating_region.RegionRect.X - p.X;
            double diff_y = _currently_creating_region.RegionRect.Y - p.Y;

            double left = _currently_creating_region.RegionRect.X;
            double top = _currently_creating_region.RegionRect.Y;
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

            _currently_creating_region.RegionRect.X = left;
            _currently_creating_region.RegionRect.Y = top;
            _currently_creating_region.RegionRect.Width = width;
            _currently_creating_region.RegionRect.Height = height;
            ((UIRegionRect)_currently_creating_region.RegionRect).UpdateScale();

        }

        /// <summary>
        /// Stops creating a region
        /// </summary>
        public void StopCreatingRegion()
        {
            if (_currently_creating_region != null)
            {
                RemoveSmallRegion();
                _currently_creating_region = null;
            }
        }

        /// <summary>
        /// Removes the currently creating region if it has a default width and height
        /// </summary>
        private void RemoveSmallRegion()
        {
            if (_currently_creating_region.RegionRect.Width + _currently_creating_region.RegionRect.Height <= 50 / InputPack.ScaleFactor)
            {
                InputPack.GetRegionList(_currently_creating_region).Remove(_currently_creating_region);
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

            OffsetX += (diff_x * -1) * InputPack.ScaleFactor;
            OffsetY += (diff_y * -1) * InputPack.ScaleFactor;
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
