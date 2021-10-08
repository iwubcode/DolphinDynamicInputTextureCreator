using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace DolphinDynamicInputTextureCreator.Other
{
    /// <summary>
    /// specify the position in the list to be selected
    /// </summary>
    public enum Selection { Keep = default , First, Last, Null}   

    public class SelectableCollection<T> : ObservableCollection<T>, Interfaces.ISelectableCollection<T>
    {
        #region Properties

        /// <summary>
        /// determines what happens when the selected element is deleted.
        /// default = Keep. tries to hold the position.
        /// </summary>
        [JsonIgnore]
        public Selection OnRemoving;

        /// <summary>
        /// determines what happens when a element is added.
        /// Last = set to the new element.
        /// </summary>
        [JsonIgnore]
        public Selection OnAdd = Selection.Last;

        private T _selected;

        /// <summary>
        /// current selected Item.
        /// accepts only items that are part of the list or Null.
        /// </summary>
        [JsonIgnore]
        public T Selected
        {
            get => _selected;
            set
            {
                if (!IsSelectable(value))
                    return;

                _selected = value;
                SelectedChanged?.Invoke(value);
                OnPropertyChanged(nameof(Selected));
                OnPropertyChanged(nameof(ValidSelection));
            }
        }

        /// <summary>
        /// is triggered when the selected element is changed.
        /// </summary>
        [JsonIgnore]
        public Action<T> SelectedChanged { get; set; }

        /// <summary>
        /// whether a valid item is selected
        /// </summary>
        [JsonIgnore]
        public bool ValidSelection { get => IndexOf(Selected) != -1; }

        #endregion

        #region Construktor

        public SelectableCollection() { }

        public SelectableCollection(IEnumerable<T> collection) : base(collection) { }

        public SelectableCollection(List<T> list) : base(list) { }

        #endregion

        #region Override List

        public new void Add(T Item)
        {
            base.Add(Item);
            Select(OnAdd);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the Collections.
        /// </summary>
        /// <param name="Item">Item which should be deleted</param>
        /// <returns>true if successfully</returns>
        public new bool Remove(T Item) => Remove(Item, OnRemoving);

        /// <summary>
        /// Removes the first occurrence of a specific object from the Collections.
        /// </summary>
        /// <param name="Item">Item which should be deleted</param>
        /// <param name="selectProperties">is used instead of OnRemoving</param>
        /// <returns>true if successfully</returns>
        public bool Remove(T Item, Selection selectProperties)
        {
            if (!IsSelected(Item))
                return base.Remove(Item);

            int i = IndexOf(Selected);
            base.Remove(Item);
            Select(i, selectProperties);

            return true;
        }

        public new bool RemoveAt(int index) => RemoveAt(index, OnRemoving);
        public bool RemoveAt(int index, Selection selectProperties) => Remove(Items[index], selectProperties);

        public bool RemoveSelected() => RemoveSelected(OnRemoving);
        public bool RemoveSelected(Selection selectProperties)
        {
            if (!ValidSelection)
                return false;

            return Remove(Selected, selectProperties);
        }

        public new void Clear()
        {
            base.Clear();
            Selected = default;
        }

        #endregion

        #region Select

        public bool Select(T Item)
        {
            Selected = Item;
            return IsSelected(Item);
        }

        /// <summary>
        /// selects the item that is at a relative position
        /// </summary>
        /// <param name="selectPropertie">specify the position</param>
        /// <returns>true if successfully</returns>
        public bool Select(Selection selectPropertie) => Select(IndexOf(Selected), selectPropertie);
        private bool Select(int i, Selection selectPropertie)
        {
            if (Count == 0)
            {
                Selected = default;
                return false;
            }

            switch (selectPropertie)
            {
                case Selection.First:
                    Selected = Items[0];
                    break;
                case Selection.Last:
                    Selected = Items[Count - 1];
                    break;
                case Selection.Keep:
                    if (i == -1) goto case Selection.First;
                    if (i > Count - 1) goto case Selection.Last;
                    Selected = Items[i];
                    break;
                case Selection.Null:
                    Selected = default;
                    break;
            }
            return ValidSelection;
        }

        #endregion

        public bool IsSelectable(T Item) => IndexOf(Item) != -1 || IsDefault(Item);

        public bool IsSelected(T Item) => !IsDefault(Selected) && Selected.Equals(Item);

        private bool IsDefault(T Item) => Item == null || Item.Equals(default);

        protected void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

    }
}
