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
            get => _emulated_device ??= new EmulatedDevice();
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
                _region_rect = clonerect;
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
                    foreach (InputRegion Region in value)
                    {
                        Region.RegionRect.OwnedTexture = RegionRect.OwnedTexture;
                    }
                }
                _sub_entries = value;
                _sub_entries.CollectionChanged += OnCollectionOfSubEntriesChanged;
                OnPropertyChanged(nameof(SubEntries));
            }
        }
        private ObservableCollection<InputRegion> _sub_entries;

        /// <summary>
        /// connects all sub regions with this regions.
        /// </summary>
        private void OnCollectionOfSubEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (InputRegion Region in e.NewItems)
                {
                    Region.RegionRect.OwnedTexture = RegionRect.OwnedTexture;
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
