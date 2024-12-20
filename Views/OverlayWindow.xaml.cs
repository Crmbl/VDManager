using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;

namespace VDManager.Views
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = -20;

        #region DLL imports 

        [DllImport("User32.dll")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("User32.dll")]
        public static extern nint FindWindow(string className, string windowTitle);

        [DllImport("User32.dll")]
        public static extern int GetWindowLong(nint hwnd, int index);

        [DllImport("User32.dll")]
        public static extern int SetWindowLong(nint hwnd, int index, int newStyle);

        #endregion // DLL imports

        public OverlayWindow()
        {
            InitializeComponent();

            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true;
            IsHitTestVisible = false;
            Topmost = true;
            Left = 0;
            Top = 0;

            Screen currentScreen = Screen.FromPoint(new System.Drawing.Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
            var zoomFactor = 100 * Screen.PrimaryScreen!.Bounds.Width / SystemParameters.PrimaryScreenWidth;
            Width = currentScreen.Bounds.Width / zoomFactor * 100;
            Height = currentScreen.Bounds.Height / zoomFactor * 100;

            Loaded += Window_Loaded;
            StateChanged += Window_StateChanged;
        }

        #region Events

        /// <summary>
        /// On source initialized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            nint hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            base.OnSourceInitialized(e);
        }

        /// <summary>
        /// Maximize the window onLoad
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Grid senderWindow)
                WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// Set Maximized after quick hack minimized.
        /// </summary>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        #endregion // Events
    }
}
