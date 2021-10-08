using DolphinDynamicInputTextureCreator.Other;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.ViewModels.Commands
{
    public class UICollection<T> : SelectableCollection<T>
    {
        #region Construktor

        public UICollection(){}

        public UICollection(List<T> list) : base(list){}

        public UICollection(IEnumerable<T> collection) : base(collection){}

        #endregion

        [JsonIgnore]
        public ICommand AddCommand => new RelayCommand<T>(Add);

        [JsonIgnore]
        public ICommand RemoveCommand => new RelayCommand<T>(item => Remove(item));

        [JsonIgnore]
        public ICommand RemoveSelectedCommand => new RelayCommand(x => RemoveSelected(), x => ValidSelection);

        [JsonIgnore]
        public ICommand SelectCommand => new RelayCommand<T>(item => Select(item));

    }
}
