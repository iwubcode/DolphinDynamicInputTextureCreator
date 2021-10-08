using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace DolphinDynamicInputTextureCreator.ViewModels.Commands
{
    // From microsoft patterns: https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern

    /// <summary>
    /// basis Class for all Commands
    /// </summary>
    /// <typeparam name="A">Action</typeparam>
    /// <typeparam name="P">Predicate</typeparam>
    internal class RelayCommand<A,P> : ICommand
    {
        #region Fields 
        readonly Action<A> _execute;
        readonly Predicate<P> _canExecute;
        #endregion // Fields 
        #region Constructors 
        public RelayCommand(Action<A> execute) : this(execute, null) { }
        public RelayCommand(Action<A> execute, Predicate<P> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }
        #endregion // Constructors 
        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((P)parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter) { _execute((A)parameter); }
        #endregion // ICommand Members 
    }


    internal class RelayCommand<T> : RelayCommand<T,T>
    {
        public RelayCommand(Action<T> execute) : base(execute){}

        public RelayCommand(Action<T> execute, Predicate<T> canExecute) : base(execute, canExecute){}
    }

    internal class RelayCommand : RelayCommand<object, object>
    {
        public RelayCommand(Action<object> execute) : base(execute) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute) { }
    }
}
