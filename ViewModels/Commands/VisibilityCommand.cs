using DolphinDynamicInputTextureCreator.Other;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.ViewModels.Commands
{
    public class VisibilityCommand : Other.PropertyChangedBase
    {
        private readonly Action<object> _on_visible;
        private readonly Action<object> _on_Hidden;
        private readonly Action<object> _on_collaps;

        private Visibility visibility = Visibility.Collapsed;
        public Visibility Visibility
        {
            get => visibility;
            private set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        /// <summary>
        /// basic command to change the visibility of an object .
        /// </summary>
        /// <param name="on_visible">is executed when the SetToVisibleCommand is executed</param>
        /// <param name="on_collaps">is executed when the SetToCollapsCommand is executed</param>
        /// <param name="on_Hidden">is executed when the SetToHiddenCommand is executed</param>
        public VisibilityCommand(Action<object> on_visible = null, Action<object> on_collaps = null, Action<object> on_Hidden = null)
        {
            _on_visible = on_visible; _on_collaps = on_collaps; _on_Hidden = on_Hidden;
        }

        public ICommand SetToVisibleCommand => new RelayCommand(param => SetVisibility(param, Visibility.Visible));

        public ICommand SetToHiddenCommand => new RelayCommand(param => SetVisibility(param, Visibility.Hidden));

        public ICommand SetToCollapsCommand => new RelayCommand(param => SetVisibility(param, Visibility.Collapsed));

        private protected void SetVisibility(object parameter, Visibility vis)
        {
            Visibility = vis;

            switch (Visibility)
            {
                case Visibility.Visible:
                    _on_visible?.Invoke(parameter);
                    break;
                case Visibility.Hidden:
                    _on_Hidden?.Invoke(parameter);
                    break;
                case Visibility.Collapsed:
                    _on_collaps?.Invoke(parameter);
                    break;
            }
        }
    }
}
