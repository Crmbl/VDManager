using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
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
        /// <see cref="Utils.KeyUtil"/>.
        /// </summary>
        public KeyUtil KeyUtil { get; set; }

		/// <summary>
		/// Determines if running or not.
		/// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Determines only macro keys.
        /// </summary>
        public bool OnlyMacro { get; set; }

        /// <summary>
        /// Is using dark theme ?
        /// </summary>
        public bool IsDarkTheme { get; set; }

        /// <summary>
        /// Window used to steal the focus when GridSetter is launched.
        /// </summary>
        public OverlayWindow Overlay { get; set; }

        #endregion // Properties

        #region DLL imports

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        #endregion // DLL imports

        #region Constructor

        /// <summary>
        /// <see cref="MainWindow"/> constructor.
        /// </summary>
        public MainWindow()
	    {
		    InitializeComponent();

			IsRunning = true;
            OnlyMacro = false;
            KeyUtil = new KeyUtil(this);

            IsDarkTheme = true;
            SystrayWindow = new SystrayWindow.SystrayWindow(new Uri("pack://application:,,,/Resources/Images/3d.ico"), "VD Manager", IsDarkTheme, 150);
            SystrayWindow.Show();
            SystrayWindow.OverrideHide();
            SystrayWindow.NotifyIcon.Visible = true;
            SystrayWindow.NotifyIcon.DoubleClick += delegate { ApplicationStatusTargetUpdated(new object(), new RoutedEventArgs()); };

            SystrayWindow.AddButton("toggle", "Toggle OFF", new Uri($"pack://application:,,,/Resources/Images/toggleOff-{(IsDarkTheme ? "dark" : "light")}.png"));
            SystrayWindow.AddButton("switch", "Only macros", new Uri($"pack://application:,,,/Resources/Images/keyboard-{(IsDarkTheme ? "dark" : "light")}.png"));
            SystrayWindow.AddButton("refreshTaskbar", "Refresh taskbar", new Uri($"pack://application:,,,/Resources/Images/reset-{(IsDarkTheme ? "dark" : "light")}.png"));
            SystrayWindow.AddButton("terminate", "Terminate", new Uri($"pack://application:,,,/Resources/Images/terminate.png"));
            SystrayWindow.AddButton("exit", "Exit", new Uri($"pack://application:,,,/Resources/Images/exit.png"));

            var toggleBtn = SystrayWindow.GetButton("toggle")!;
            toggleBtn.Click += ApplicationStatusTargetUpdated;

            var switchBtn = SystrayWindow.GetButton("switch")!;
            switchBtn.Click += ToggleKeys;

            var terminateBtn = SystrayWindow.GetButton("terminate")!;
            terminateBtn.Click += Terminate;

            var refreshTaskbar = SystrayWindow.GetButton("refreshTaskbar");
            refreshTaskbar.Click += RefreshTaskbar;

            var exitButton = SystrayWindow.GetButton("exit");
            exitButton.Click += ExitClick;

            Overlay = new OverlayWindow();
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

        public void ShowOverlay()
        {
            Overlay.Show();
            Overlay.Activate();
        }

        public void HideOverlay()
        {
            Overlay.Hide();
        }

        public void MoveOverlay()
        {
            var handle = new WindowInteropHelper(Overlay).Handle;
            Desktop.Current.MoveWindow(handle);
            Overlay.Activate();
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

            KeyUtil.RegisterHotKeyMacro();
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
            KeyUtil.UnregisterHotKeyMacro();
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
                KeyUtil.UnregisterHotKeyMacro();
            }
            else
		    {
                SystrayWindow.UpdateButton("toggle", "Toggle OFF", new Uri($"pack://application:,,,/Resources/Images/toggleOff-{(IsDarkTheme ? "dark" : "light")}.png"));
                KeyUtil.RegisterHotKeyMacro();
                KeyUtil.RegisterHotKeyArrow();
			}
            IsRunning = !IsRunning;
	    }

		private void ToggleKeys(object sender, RoutedEventArgs e)
        {
            SystrayWindow.Hide();
            if (OnlyMacro)
            {
                SystrayWindow.UpdateButton("switch", "Only macros", new Uri($"pack://application:,,,/Resources/Images/keyboard-{(IsDarkTheme ? "dark" : "light")}.png"));
                KeyUtil.RegisterHotKeyArrow();
            }
            else
            {
                SystrayWindow.UpdateButton("switch", "All keys", new Uri($"pack://application:,,,/Resources/Images/keyboard-{(IsDarkTheme ? "dark" : "light")}.png"));
                KeyUtil.UnregisterHotKeyArrow();
            }
            OnlyMacro = !OnlyMacro;
        }

        /// <summary>
        /// Kill all instances of GridSetter and remove all virtual desktops
        /// </summary>
        private void Terminate(object sender, RoutedEventArgs e)
        {
            foreach (Process process in Process.GetProcesses().Where(x => x.ProcessName.ToLower().StartsWith("gridsetter")).ToList())
                process.Kill();

            Desktop.RemoveAll();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Exit app on click and revert back to default settings.
        /// </summary>
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Refresh taskbar on click
        /// https://stackoverflow.com/questions/19736921/convert-c-code-to-c-sendmessagetimeout
        /// https://www.reddit.com/r/Windows11/comments/12ygqei/how_can_i_refresh_the_taskbar_after_making_some/
        /// </summary>
        public void RefreshTaskbar(object sender, RoutedEventArgs e)
        {
            SystrayWindow.Hide();
            
            try
            {
                IntPtr result = IntPtr.Zero;
                IntPtr setting = Marshal.StringToHGlobalUni("TraySettings");

                var value = SendMessageTimeout(0xffff, 0x001A, 0, setting, 0x0002, 5000, out result);
                Marshal.FreeHGlobal(setting);
            }
            catch
            {
                throw;
            }
        }

        #endregion // Events
    }
}
