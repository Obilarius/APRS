using System;
using System.Windows.Input;

namespace ARPS
{
    /// <summary>
    /// Ein Basis Command die eine Action ausführt
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private Member

        /// <summary>
        /// Die Action die ausgeführt werden soll
        /// </summary>
        private Action mAction;

        #endregion

        #region Public Events

        /// <summary>
        /// Das Event das ausgeführt wird wenn <see cref="CanExecute(object)"/> geändert wurde
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        public RelayCommand(Action action)
        {
            mAction = action;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Ein RelayCommand kann immer ausgeführt werden
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Führt die Action des Commans aus
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            mAction();
        }

        #endregion
    }
}
