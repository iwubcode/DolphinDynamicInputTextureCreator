using DolphinDynamicInputTexture.Interfaces;
using DolphinDynamicInputTexture.Properties;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace DolphinDynamicInputTexture.Data
{
    public class InputRegion : Other.PropertyChangedBase, ITagable, ICloneable, IEquatable<InputRegion>
    {
        /// <summary>
        /// From which device is the key on this region.
        /// </summary>
        public EmulatedDevice Device
        {
            get => OwnedRegion == null ? _emulated_device ??= new EmulatedDevice() : OwnedRegion.Device;
            set
            {
                _emulated_device = value;
                OnPropertyChanged(nameof(Device));
            }
        }
        private EmulatedDevice _emulated_device;

        /// <summary>
        /// For which key is this region.
        /// </summary>
        public EmulatedKey Key
        {
            get => _emulated_key ??= new EmulatedKey();
            set
            {
                _emulated_key = value;
                OnPropertyChanged(nameof(Key));
            }
        }
        private EmulatedKey _emulated_key;

        /// <summary>
        /// The tag of this region.
        /// </summary>
        public Tag Tag
        {
            get => _tag ??= new Tag();
            set
            {
                _tag = value;
                OnPropertyChanged(nameof(Tag));
            }
        }
        private Tag _tag;

        /// <summary>
        /// Describes how the exchange region should be transferred to the image.
        /// </summary>
        public CopyTypeProperties CopyType
        {
            get => _copy_type;
            set
            {
                _copy_type = value;
                OnPropertyChanged(nameof(CopyType));
            }
        }
        private CopyTypeProperties _copy_type = default;

        /// <summary>
        /// Describes under which conditions the exchange region and sub regions are transferred to the image.
        /// </summary>
        public BindTypeProperties BindType
        {
            get => _bind_type;
            set
            {
                _bind_type = value;
                OnPropertyChanged(nameof(BindType));
            }
        }
        private BindTypeProperties _bind_type = default;

        /// <summary>
        /// the position of this region.
        /// </summary>
        public IRectRegion RegionRect
        {
            get => _region_rect;
            set
            {
                IRectRegion clonerect = (IRectRegion)value.Clone();
                clonerect.OwnedTexture = _region_rect.OwnedTexture;
                if (OwnedRegion != null && clonerect is ISubRectRegion)
                {
                    ((ISubRectRegion)clonerect).MainRegion = OwnedRegion.RegionRect;
                }
                _region_rect = clonerect;
                foreach (InputRegion Region in SubEntries)
                {
                    Region.OwnedRegion = this;
                }
                OnPropertyChanged(nameof(RegionRect));
            }
        }
        private IRectRegion _region_rect = new InputRegionRect();

        /// <summary>
        /// Subregions used under certain conditions.
        /// </summary>
        public ObservableCollection<InputRegion> SubEntries
        {
            get => _sub_entries ??= SubEntries = new ObservableCollection<InputRegion>();
            set
            {
                if (_sub_entries != null)
                {
                    _sub_entries.CollectionChanged -= OnCollectionOfSubEntriesChanged;
                }
                if (value.Count > 0)
                {
                    if (BindType == BindTypeProperties.single)
                        BindType = BindTypeProperties.multi;

                    foreach (InputRegion Region in value)
                    {
                        Region.OwnedRegion = this;
                    }
                }
                _sub_entries = value;
                _sub_entries.CollectionChanged += OnCollectionOfSubEntriesChanged;
                OnPropertyChanged(nameof(SubEntries));
            }
        }
        private ObservableCollection<InputRegion> _sub_entries;


        public InputRegion OwnedRegion
        {
            get => _main_region;
            internal set
            {
                _main_region = value;
                if (value != null)
                {
                    RegionRect.OwnedTexture = _main_region.RegionRect.OwnedTexture;
                    if (RegionRect is ISubRectRegion)
                    {
                        ((ISubRectRegion)RegionRect).MainRegion = OwnedRegion.RegionRect;
                    }
                }
                OnPropertyChanged(nameof(OwnedRegion));
            }
        }
        private InputRegion _main_region;

        /// <summary>
        /// connects all sub regions with this regions.
        /// </summary>
        private void OnCollectionOfSubEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.OldItems != null)
            {
                if (SubEntries.Count == 0)
                    BindType = BindTypeProperties.single;
            }

            if (e.NewItems != null)
            {
                if (BindType == BindTypeProperties.single)
                    BindType = BindTypeProperties.multi;

                foreach (InputRegion Region in e.NewItems)
                {
                    Region.OwnedRegion = this;
                }
            }
        }

        public object Clone()
        {
            InputRegion clone = (InputRegion)this.MemberwiseClone();
            clone.RegionRect = (IRectRegion)this.RegionRect.Clone();
            clone.SubEntries = new ObservableCollection<InputRegion>();
            foreach (InputRegion item in this.SubEntries)
            {
                clone.SubEntries.Add((InputRegion)item.Clone());
            }
            return clone;
        }

        public bool Equals([AllowNull] InputRegion other)
        {
            if (other != null && other.CopyType == CopyType & other.BindType == BindType & other.Device.Equals(Device) & other.Key.Equals(Key) & other.RegionRect.Equals(RegionRect))
            {
                if (other.SubEntries.Equals(SubEntries))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
