using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using VDManager.Utils;

namespace VDManager.Views
{
    public partial class MainWindow
    {
        #region Properties

        /// <summary>
        /// Icon window in the system tray.
        /// </summary>
        public SystrayWindow.SystrayWindow SystrayWindow { get; set; }

        /// <summary>
        /// Toggle button.
        /// </summary>
        public Button ToggleButton { get; set; }

        /// <summary>
        /// <see cref="Utils.KeyUtil"/>.
        /// </summary>
        public KeyUtil KeyUtil { get; set; }

		/// <summary>
		/// Determines if running or not.
		/// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Is using dark theme ?
        /// </summary>
        public bool IsDarkTheme { get; set; }

        #endregion // Properties

        #region Constructor

        /// <summary>
        /// <see cref="MainWindow"/> constructor.
        /// </summary>
        public MainWindow()
	    {
		    InitializeComponent();

			IsRunning = true;
		    KeyUtil = new KeyUtil(this);

            IsDarkTheme = true;
            SystrayWindow = new SystrayWindow.SystrayWindow(new Uri("pack://application:,,,/Resources/Images/3d.ico"), "VD Manager", IsDarkTheme, 150);
            SystrayWindow.Show();
            SystrayWindow.OverrideHide();
            SystrayWindow.NotifyIcon.Visible = true;
            SystrayWindow.NotifyIcon.DoubleClick += delegate
            {
                IsRunning = !IsRunning;
            };

            SystrayWindow.AddButton("toggle", "Toggle OFF", new Uri($"pack://application:,,,/Resources/Images/toggleOff-{(IsDarkTheme ? "dark" : "light")}.png"));
            SystrayWindow.AddButton("exit", "Exit", new Uri($"pack://application:,,,/Resources/Images/exit.png"));

            ToggleButton = SystrayWindow.GetButton("toggle")!;
            ToggleButton.Click += ApplicationStatusTargetUpdated;

            var exitButton = SystrayWindow.GetButton("exit");
            if (exitButton != null)
                exitButton.Click += ExitClick;
        }

        #endregion // Constructor

        #region Methods

        /// <summary>
        /// Switch to the left virtual desktop.
        /// </summary>
        public void SwitchLeft()
		{
            if (Desktop.Count != 1)
                VirtualDesktop.GoLeft();
		}

		/// <summary>
		/// Switch to the right virtual desktop.
		/// </summary>
		public void SwitchRight()
		{
			if (Desktop.Count != 1)
				VirtualDesktop.GoRight();
		}

		#endregion // Methods

		#region Events

		/// <summary>
		/// Event raised on state changed.
		/// </summary>
		protected override void OnStateChanged(EventArgs e)
	    {
			if (WindowState == WindowState.Minimized)
		    {
                SystrayWindow.NotifyIcon.Visible = true;
			    Hide();
		    }

			base.OnStateChanged(e);
	    }

		/// <summary>
		/// Event raised on source initialized.
		/// </summary>
		protected override void OnSourceInitialized(EventArgs e)
	    {
		    base.OnSourceInitialized(e);

		    var helper = new WindowInteropHelper(this);
		    KeyUtil.Source = HwndSource.FromHwnd(helper.Handle);
		    KeyUtil.Source?.AddHook(KeyUtil.HwndHook);
            KeyUtil.RegisterHotKeyArrow();

            WindowState = WindowState.Minimized;
            SystrayWindow.NotifyIcon.Visible = true;
            Hide();
        }

		/// <summary>
		/// Event raised on app closed.
		/// </summary>
		protected override void OnClosed(EventArgs e)
	    {
		    KeyUtil.Source.RemoveHook(KeyUtil.HwndHook);
		    KeyUtil.Source = null;
			KeyUtil.UnregisterHotKeyArrow();
		    base.OnClosed(e);
	    }

		/// <summary>
		/// Used when the ApplicationStatus value is updated.
		/// </summary>
		private void ApplicationStatusTargetUpdated(object sender, RoutedEventArgs e)
		{
			SystrayWindow.Hide();
			if (IsRunning)
			{
                SystrayWindow.UpdateButton("toggle", "Toggle ON", new Uri($"pack://application:,,,/Resources/Images/toggleOn-{(IsDarkTheme ? "dark" : "light")}.png"));
                KeyUtil.UnregisterHotKeyArrow();
            }
            else
		    {
                SystrayWindow.UpdateButton("toggle", "Toggle OFF", new Uri($"pack://application:,,,/Resources/Images/toggleOff-{(IsDarkTheme ? "dark" : "light")}.png"));
                KeyUtil.RegisterHotKeyArrow();
			}
            IsRunning = !IsRunning;
	    }

        /// <summary>
        /// Exit app on click and revert back to default settings.
        /// </summary>
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion // Events
    }
}
