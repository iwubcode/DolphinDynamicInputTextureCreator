using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace DolphinDynamicInputTextureCreator.Interfaces
{
    interface ISelectableCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        T Selected { get; set; }

        bool ValidSelection { get; }

        bool RemoveSelected();

        new bool Remove(T Item);

        new void Clear();
    }
}
