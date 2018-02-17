using System;
using System.ComponentModel;
using System.Threading;
using VDManager.Utils.Interfaces;

namespace VDManager.Utils
{
    public class Dispatcher : IDispatcher
    {
        #region Instance variables

        /// <summary>
        /// The WPF UI dispatcher used to execute the specified actions on the UI thread. 
        /// </summary>
        readonly private System.Windows.Threading.Dispatcher _Dispatcher;

        #endregion // Instance variables

        #region Constructors

        /// <summary>
        /// WpfDispatcher constructor.
        /// </summary>
        /// <param name="uiThread">The UI thread.</param>
        public Dispatcher(Thread uiThread)
        {
            // Initialization.
            _Dispatcher = System.Windows.Threading.Dispatcher.FromThread(uiThread);
        }

        #endregion // Constructors

        #region Methods

        /// <summary>
        /// Executes the specified delegate synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="handler">The handler used to launch the event.</param>
        /// <param name="propertyName">The name of the property which has been modified.</param>
        public void Invoke(object source, PropertyChangedEventHandler handler, string propertyName)
        {
            _Dispatcher.Invoke(() =>
            {
                handler(source, new PropertyChangedEventArgs(propertyName));
            });
        }

        /// <summary>
        /// Executes the specified delegate synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public void Invoke(Action action)
        {
            _Dispatcher.Invoke(action);
        }

        #endregion //Methods
    }
}
