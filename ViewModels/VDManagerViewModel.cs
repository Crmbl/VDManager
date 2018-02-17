using System;
using System.Diagnostics;
using VDManager.Utils;
using VDManager.Utils.Enums;

namespace VDManager.ViewModels
{
    /// <summary>
    /// MainViewModel of the app.
    /// </summary>
    public class VDManagerViewModel : BaseViewModel
    {
        #region Instance Variables

        /// <summary>
        /// Private variable for <see cref="AppTitle"/>.
        /// </summary>
        private string _appTitle;

	    /// <summary>
	    /// Private variable for <see cref="WelcomeMessage"/>.
	    /// </summary>
	    private string _welcomeMessage;

		/// <summary>
		/// Private variable for <see cref="Version"/>.
		/// </summary>
		private string _version;

		/// <summary>
		/// Private variable for <see cref="AppStatus"/>.
		/// </summary>
		private string _appStatus;

		/// <summary>
		/// Private variable for <see cref="InfoStop"/>.
		/// </summary>
		private string _infoStop;

		/// <summary>
		/// Private variable for <see cref="UseArrows"/>.
		/// </summary>
		private bool _useArrows;

		/// <summary>
		/// Private variable for <see cref="UseFKeys"/>.
		/// </summary>
		private bool _useFKeys;

		/// <summary>
		/// Private variable for <see cref="UseNumPad"/>.
		/// </summary>
		private bool _useNumPad;

        #endregion // Instance Variables

        #region Properties

        /// <summary>
        /// Defines the app title.
        /// </summary>
        public string AppTitle
        {
            get => _appTitle;
	        set
            {
                if (_appTitle == value) return;
                _appTitle = value;
                NotifyPropertyChanged("AppTitle");
            }
        }

	    /// <summary>
	    /// The welcome message of the app.
	    /// </summary>
	    public string WelcomeMessage
	    {
		    get => _welcomeMessage;
		    set
		    {
			    if (_welcomeMessage == value) return;
			    _welcomeMessage = value;
			    NotifyPropertyChanged("WelcomeMessage");
		    }
	    }

		/// <summary>
		/// The version of the app.
		/// </summary>
	    public string Version
	    {
		    get => _version;
		    set
		    {
			    if (_version == value) return;
			    _version = value;
			    NotifyPropertyChanged("Version");
		    }
	    }

		/// <summary>
		/// Defines the app status.
		/// </summary>
		public string AppStatus
        {
            get => _appStatus;
			set
            {
                if (_appStatus == value) return;
                _appStatus = value;
                NotifyPropertyChanged("AppStatus");
            }
        }

		/// <summary>
		/// Defines the info message.
		/// </summary>
	    public string InfoStop
	    {
		    get => _infoStop;
		    set
		    {
			    if (_infoStop == value) return;
			    _infoStop = value;
			    NotifyPropertyChanged("InfoStop");
		    }
	    }

		/// <summary>
		/// Defines if the user want to use arrow keys.
		/// </summary>
	    public bool UseArrows
	    {
		    get => _useArrows;
		    set
		    {
			    if (_useArrows == value) return;
			    _useArrows = value;

			    OnCheckboxChanged(EventArgs.Empty);
				NotifyPropertyChanged("UseArrows");
		    }
	    }

		/// <summary>
		/// Defines if the user want to use the F keys.
		/// </summary>
	    public bool UseFKeys
	    {
		    get => _useFKeys;
		    set
		    {
			    if (_useFKeys == value) return;
			    _useFKeys = value;

			    OnCheckboxChanged(EventArgs.Empty);
				NotifyPropertyChanged("UseFKeys");
		    }
	    }

		/// <summary>
		/// Defines if the user want to use the numpad keys.
		/// </summary>
	    public bool UseNumPad
	    {
		    get => _useNumPad;
		    set
		    {
			    if (_useNumPad == value) return;
			    _useNumPad = value;

			    OnCheckboxChanged(EventArgs.Empty);
				NotifyPropertyChanged("UseNumPad");
		    }
	    }

		/// <summary>
		/// Exit the application on command.
		/// </summary>
		public ActionCommand ExitCommand { get; set; }

	    /// <summary>
	    /// Event for the checkboxes property.
	    /// </summary>
	    public event EventHandler CheckboxEvent;

	    /// <summary>
	    /// The currently hidden <see cref="Process"/>.
	    /// </summary>
	    public Process CurrentlyHiddenProcess { get; set; }

	    /// <summary>
	    /// Defines the previous state of the window before hiding it.
	    /// </summary>
	    public ShowWindowCommandEnum PreviousStateBeforeHiding { get; set; }

		#endregion // Properties

		#region Constructors

		/// <summary>
		/// Default parameterless constructor.
		/// </summary>
		public VDManagerViewModel()
        {
            AppTitle = "VDManager";
			#if DEBUG
			Version = "2.0d";
			#else
			Version = "2.0r";
			#endif
			WelcomeMessage = "Virtual Desktop Manager";
	        InfoStop = "[inser] key to toggle";
            AppStatus = "RUNNING";

	        UseNumPad = false;
			UseFKeys = true;
	        UseArrows = true;

	        ExitCommand = new ActionCommand(() => System.Windows.Application.Current.Shutdown());
		}

		#endregion // Constructors

		#region Events

		/// <summary>
		/// Raised when checkbox value changed.
		/// </summary>
		protected virtual void OnCheckboxChanged(EventArgs e)
	    {
		    EventHandler handler = CheckboxEvent;
		    handler?.Invoke(this, e);
	    }

		#endregion // Events
    }
}
