using DolphinDynamicInputTexture.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DolphinDynamicInputTexture.Data
{
    public class Tag : Other.PropertyChangedBase, IName, IEquatable<Tag>
    {
        #region PROPERTIES

        public string Name
        {
            get => _name ??= "";
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _name;

        public bool Equals([AllowNull] Tag other)
        {
            return other != null && other.Name == Name;
        }

        #endregion
    }
}
