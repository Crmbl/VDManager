using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;
using VDManager.Views;

namespace VDManager.Utils
{
	/// <summary>
	/// Util for the keys management and binding.
	/// Info : http://www.kbdedit.com/manual/low_level_vk_list.html
	/// </summary>
	public class KeyUtil
	{
		#region Constants

		private const int HOTKEY_ID_ARROW_LEFT = 9008;
		private const int HOTKEY_ID_ARROW_RIGHT = 9009;
		private const int KEYEVENTF_EXTENDEDKEY = 1;
		private const int KEYEVENTF_KEYUP = 2;

		#endregion // Constants

		#region Struct

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		internal struct Windowplacement
		{
			public int length;
			public int flags;
			public ShowWindowCommandEnum showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public System.Drawing.Rectangle rcNormalPosition;
		}

        #endregion // Struct

        #region Enums

        public enum KeysEnum
        {
            Left,
            Right
        }

        public enum ShowWindowCommandEnum
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window 
            /// for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3,
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>       
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value 
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except 
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position. 
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level 
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is 
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position. 
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
            /// that owns the window is not responding. This flag should only be 
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }

        #endregion // Enums

        #region DLL Imports

        [DllImport("User32.dll")]
		private static extern bool RegisterHotKey([In] IntPtr hWnd, [In] int id, [In] uint fsModifiers, [In] uint vk);

		[DllImport("User32.dll")]
		private static extern bool UnregisterHotKey([In] IntPtr hWnd, [In] int id);

		[DllImport("User32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);

        [DllImport("USER32.DLL")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion // DLL Imports

        #region Properties

        /// <summary>
        /// Source of binding.
        /// </summary>
        public HwndSource Source { get; set; }

		/// <summary>
		/// The mainwindow of the app.
		/// </summary>
		public MainWindow MainWindow { get; set; }

		#endregion // Properties

		#region Constructor

		/// <summary>
		/// <see cref="KeyUtil"/> constructor.
		/// </summary>
		/// <param name="mainWindow">The main window of the app.</param>
		public KeyUtil(MainWindow mainWindow)
		{
			MainWindow = mainWindow;
		}

		#endregion // Constructor

		#region KeyManagement

		/// <summary>
		/// Register the arrow hotkeys.
		/// </summary>
		/// <param name="window"></param>
		public void RegisterHotKeyArrow()
		{
			var helper = new WindowInteropHelper(MainWindow);
			const uint VK_LEFT = 0x25;
			const uint VK_RIGHT = 0x27;
			const uint MOD_CTRL = 0;

			if (!RegisterHotKey(helper.Handle, HOTKEY_ID_ARROW_LEFT, MOD_CTRL, VK_LEFT))
			{
				throw new Exception($"Error with binding to Arrow left [{VK_LEFT}]");
			}
			if (!RegisterHotKey(helper.Handle, HOTKEY_ID_ARROW_RIGHT, MOD_CTRL, VK_RIGHT))
			{
				throw new Exception($"Error with binding to Arrow right [{VK_RIGHT}]");
			}
		}

		/// <summary>
		/// Unregister the arrow hotkeys.
		/// </summary>
		/// <param name="window"></param>
		public void UnregisterHotKeyArrow()
		{
			var helper = new WindowInteropHelper(MainWindow);
			UnregisterHotKey(helper.Handle, HOTKEY_ID_ARROW_LEFT);
			UnregisterHotKey(helper.Handle, HOTKEY_ID_ARROW_RIGHT);
		}

		/// <summary>
		/// Hooks the key with a keypress event.
		/// </summary>
		public IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			const int WM_HOTKEY = 0x0312;
			switch (msg)
			{
				case WM_HOTKEY:
					switch (wParam.ToInt32())
					{
						case HOTKEY_ID_ARROW_LEFT:
							OnHotKeyPressed(KeysEnum.Left);
							handled = true;
							break;

						case HOTKEY_ID_ARROW_RIGHT:
							OnHotKeyPressed(KeysEnum.Right);
							handled = true;
							break;
					}
					break;
			}
			return IntPtr.Zero;
		}

		/// <summary>
		/// On KeyDown event to create a macro.
		/// </summary>
		/// <param name="vKey">The key to KeyDown.</param>
		public static void KeyDown(Keys vKey)
		{
			keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
		}

		/// <summary>
		/// On KeyUp event to release the macro.
		/// </summary>
		/// <param name="vKey">The key to KeyUp.</param>
		public static void KeyUp(Keys vKey)
		{
			keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
		}

		#endregion // KeyManagement

		#region Methods

		/// <summary>
		/// Main method to manage the macro system.
		/// </summary>
		/// <param name="key">The key pressed (num0 to num9).</param>
		private void OnHotKeyPressed(KeysEnum key)
		{
			switch (key)
			{
				case KeysEnum.Left:
                    MainWindow.SwitchLeft();
				    ManageGridSetters();
                    break;

				case KeysEnum.Right:
					MainWindow.SwitchRight();
				    ManageGridSetters();
                    break;
			}
		}

		/// <summary>
        /// Allow the GridSetters to be hidden or restore when changing virtual desktop.
        /// </summary>
	    private static void ManageGridSetters()
	    {
			var processes = Process.GetProcesses();
            var gridSetters = processes.Where(x => x.ProcessName.ToLower().StartsWith("gridsetter")).ToList();
            foreach (Process process in gridSetters)
            {
                if (process.ProcessName.ToLower().StartsWith("gridsetter") && process.MainWindowTitle.ToLower() == "grid")
                {
					nint windowHandle = process.MainWindowHandle;
                    if (windowHandle == IntPtr.Zero)
                        return;

                    if (VirtualDesktop.IsWindowOnCurrentVirtualDesktop(windowHandle))
                        ShowWindow(windowHandle, (int)ShowWindowCommandEnum.Maximize);
                    else
                        ShowWindow(windowHandle, (int)ShowWindowCommandEnum.Minimize);
                }
            }
        }

        #endregion // Methods
    }
}
