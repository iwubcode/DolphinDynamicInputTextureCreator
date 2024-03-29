﻿using DolphinDynamicInputTexture.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DolphinDynamicInputTexture.Data
{
    public class EmulatedKey : Other.PropertyChangedBase, IName, IEquatable<EmulatedKey>
    {
        #region PROPERTIES

        /// <summary>
        /// The key name on the emulated machine, ex: "Buttons/A"
        /// </summary>
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

        public bool Equals([AllowNull] EmulatedKey other)
        {
            return other != null && other.Name == Name;
        }

        #endregion
    }
}
