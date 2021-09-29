using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DolphinDynamicInputTextureCreator.Models.Suggestions
{
    /// <summary>
    /// List of suggestions for possible Item names.
    /// automatically removes used suggestions when bound to an ObservableCollection.
    /// </summary>
    /// <typeparam name="T">Item</typeparam>
    public class SuggestionList<T> : ObservableCollection<string> where T : INotifyPropertyChanged
    {

        #region Properties

        /// <summary>
        /// converts the element into a string
        /// </summary>
        private readonly Converter<T, string> _item_to_suggestions;

        /// <summary>
        /// List that is observed.
        /// </summary>
        private ObservableCollection<T> _target_list;

        /// <summary>
        /// get a list of unused items.
        /// </summary>
        public ReadOnlyCollection<string> Available => _available.AsReadOnly();
        private List<string> _available = new List<string>();

        #endregion

        #region constructor

        /// <summary>
        /// List of suggestions for possible Item names.
        /// </summary>
        /// <param name="ConvertItemsToSuggestions">converts the element into a string</param>
        /// <param name="target_list">basies list to be observed</param>
        public SuggestionList(Converter<T, string> ConvertItemsToSuggestions, ObservableCollection<T> target_list = null)
        {
            _item_to_suggestions = ConvertItemsToSuggestions;
            this.CollectionChanged += UpdateAvailableSuggestions;

            if (target_list != null)
                SetTargetList(target_list);

        }

        /// <summary>
        /// List of suggestions for possible Item names.
        /// </summary>
        /// <param name="ConvertItemsToSuggestions">converts the element into a string</param>
        /// <param name="collection">The collection from which the elements are copied</param>
        public SuggestionList(Converter<T, string> ConvertItemsToSuggestions, IEnumerable<string> collection) : base(collection)
        {
            _item_to_suggestions = ConvertItemsToSuggestions;
            this.CollectionChanged += UpdateAvailableSuggestions;
        }

        #endregion

        #region SetTargetList

        /// <summary>
        /// sets a list to be observed.
        /// to automatically adjust the available suggestions.
        /// </summary>
        /// <param name="target_list">basies list to be observed</param>
        public void SetTargetList(ObservableCollection<T> target_list)
        {
            if (_target_list != null)
            {
                _target_list.CollectionChanged -= UpdateUsedSuggestions;
                foreach (T item in _target_list)
                {
                    item.PropertyChanged -= UpdateUsedItemSuggestions;
                }
            }

            _target_list = target_list;
            target_list.CollectionChanged += UpdateUsedSuggestions;
            foreach (T item in _target_list)
            {
                item.PropertyChanged += UpdateUsedItemSuggestions;
            }
            UpdateAvailableSuggestions();
        }

        #endregion

        #region UpdateAvailableSuggestions

        /// <summary>
        /// Forces update Available suggestions
        /// </summary>
        public void UpdateAvailableSuggestions()
        {
            _available = new List<string>(this);

            if (_target_list == null)
                return;

            foreach (T item in _target_list)
            {
                _available.Remove(_item_to_suggestions(item));
            }
            OnPropertyChanged(nameof(Available));
        }

        private void UpdateAvailableSuggestions(object sender, NotifyCollectionChangedEventArgs e) => UpdateAvailableSuggestions();

        private void UpdateUsedItemSuggestions(object sender, System.ComponentModel.PropertyChangedEventArgs e) => this.UpdateAvailableSuggestions();

        private void UpdateUsedSuggestions(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T item in e.NewItems)
                    {
                        _available.Remove(_item_to_suggestions(item));
                        item.PropertyChanged += UpdateUsedItemSuggestions;
                    }
                    OnPropertyChanged(nameof(Available));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (T item in e.OldItems)
                    {
                        item.PropertyChanged -= UpdateUsedItemSuggestions;
                    }
                    goto default;
                case NotifyCollectionChangedAction.Move:
                    break;
                default:
                    UpdateAvailableSuggestions();
                    break;
            }
        }

        #endregion

        #region GetUnusedSuggestion

        /// <summary>
        /// get the first unused suggestion.
        /// </summary>
        /// <returns>Unused Suggestion or a empty string</returns>
        public string GetUnusedSuggestion() => Available.Count != 0 ? Available[0] : "";

        /// <summary>
        /// get the first unused suggestion.
        /// </summary>
        /// <param name="collection">collection to use</param>
        /// <returns>Unused Suggestion or a empty string</returns>
        public string GetUnusedSuggestion(IList<T> collection)
        {
            foreach (string suggestion in this)
            {
                bool skip_suggestion = false;
                foreach (T item in collection)
                {
                    // Skip suggestion when a match is found
                    if (_item_to_suggestions(item) == suggestion)
                    {
                        skip_suggestion = true;
                        break;
                    }
                }

                if (!skip_suggestion)
                    return suggestion;
            }
            return "";
        }

        #endregion

        protected void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
}
