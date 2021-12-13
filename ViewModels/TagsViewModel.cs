using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTextureCreator.Models.Suggestions;
using DolphinDynamicInputTextureCreator.Other;
using DolphinDynamicInputTextureCreator.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.ViewModels
{
    class TagsViewModel : Other.PropertyChangedBase
    {
        /// <summary>
        /// The Tags mapped in this pack
        /// </summary>
        public UICollection<Tag> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                TagsSuggestions.SetTargetList(Tags);
                OnPropertyChanged(nameof(Tags));
            }
        }
        private UICollection<Tag> _tags;

        /// <summary>
        /// possible tag Suggestions.
        /// </summary>
        public SuggestionList<Tag> TagsSuggestions { get; } = new SuggestionList<Tag>((tag) => tag.Name, Models.DefaultData.Suggestions.TagNames);

        public ICommand DeleteTagCommand => new RelayCommand<Tag>(Tag => Tags.Remove(Tag));

        public ICommand AddTagCommand => new RelayCommand<string>(AddTag);

        public ICommand AddAllTagsCommand => new RelayCommand(AddAllKey, (I) => TagsSuggestions.Available.Count != 0);

        private void AddAllKey(object obj)
        {
            string name;
            while ((name = TagsSuggestions.GetUnusedSuggestion()) != "")
            {
                AddTag(name);
            }
        }

        private void AddTag(string name)
        {
            name ??= TagsSuggestions.GetUnusedSuggestion();

            Tag new_tag = new Tag { Name = name };
            Tags.Add(new_tag);
        }

    }
}
