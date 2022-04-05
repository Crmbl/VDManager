using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;
using VDManager.Utils.Enums;
using VDManager.ViewModels;
using VDManager.Views;
using WindowsDesktop;

namespace VDManager.Utils
{
	/// <summary>
	/// Util for the keys management and binding.
	/// Info : http://www.kbdedit.com/manual/low_level_vk_list.html
	/// </summary>
	public class KeyUtil
	{
		#region Constants

		private const int HOTKEY_ID_PAD1 = 9001;
		private const int HOTKEY_ID_PAD2 = 9002;
		private const int HOTKEY_ID_PAD3 = 9003;
		private const int HOTKEY_ID_F1 = 9004;
		private const int HOTKEY_ID_F2 = 9005;
		private const int HOTKEY_ID_F3 = 9006;
		private const int HOTKEY_ID_INSERT = 9007;
		private const int HOTKEY_ID_ARROW_LEFT = 9008;
		private const int HOTKEY_ID_ARROW_RIGHT = 9009;
        private const int HOTKEY_ID_TILDE = 9010;

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
		/// The viewModel we want to bind on.
		/// </summary>
		public VDManagerViewModel ViewModel { get; set; }

		/// <summary>
		/// The mainwindow of the app.
		/// </summary>
		public MainWindow MainWindow { get; set; }

		#endregion // Properties

		#region Constructor

		/// <summary>
		/// <see cref="KeyUtil"/> constructor.
		/// </summary>
		/// <param name="viewModel">The viewModel to bind to.</param>
		/// <param name="mainWindow">The main window of the app.</param>
		public KeyUtil(VDManagerViewModel viewModel, MainWindow mainWindow)
		{
			ViewModel = viewModel;
			MainWindow = mainWindow;
		}

		#endregion // Constructor

		#region KeyManagement

		/// <summary>
		/// Register the toggle service key.
		/// </summary>
		public void RegisterToggleServiceKey()
		{
			var helper = new WindowInteropHelper(MainWindow);
			const uint VK_INSERT = 0x2D;
			const uint MOD_CTRL = 0;
			if (!RegisterHotKey(helper.Handle, HOTKEY_ID_INSERT, MOD_CTRL, VK_INSERT))
			{
				throw new Exception($"Error with binding to Insert [{VK_INSERT}]");
			}
		}

        /// <summary>
        /// Register the toggle pin key.
        /// </summary>
	    public void RegisterTogglePinKey()
	    {
	        var helper = new WindowInteropHelper(MainWindow);
	        const uint VK_TILDE = 0xDE; 
	        const uint MOD_CTRL = 0x0002;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID_TILDE, MOD_CTRL, VK_TILDE))
	        {
                throw new Exception($"Error with binding to Tilde [{VK_TILDE}]");
	        }
	    }

		/// <summary>
		/// Register the number hotkeys.
		/// </summary>
		public void RegisterHotKeyF()
		{
			var helper = new WindowInteropHelper(MainWindow);
			const uint VK_F1 = 0x70;
			const uint VK_F2 = 0x71;
			const uint VK_F3 = 0x72;
			const uint MOD_CTRL = 0;

			if (!RegisterHotKey(helper.Handle, HOTKEY_ID_F1, MOD_CTRL, VK_F1))
			{
				throw new Exception($"Error with binding to Number1 [{VK_F1}]");
			}
			if (!RegisterHotKey(helper.Handle, HOTKEY_ID_F2, MOD_CTRL, VK_F2))
			{
				throw new Exception($"Error with binding to Number2 [{VK_F2}]");
			}
			if (!RegisterHotKey(helper.Handle, HOTKEY_ID_F3, MOD_CTRL, VK_F3))
			{
				throw new Exception($"Error with binding to Number3 [{VK_F3}]");
			}
		}

		/// <summary>
		/// Register the numpad hotkeys.
		/// </summary>
		public void RegisterHotKeyNumPad()
		{
			var helper = new WindowInteropHelper(MainWindow);
			const uint VK_NUMPAD1 = 0x61;
			const uint VK_NUMPAD2 = 0x62;
			const uint VK_NUMPAD3 = 0x63;
			const uint MOD_CTRL = 0;
			
			if (!RegisterHotKey(helper.Handle, HOTKEY_ID_PAD1, MOD_CTRL, VK_NUMPAD1))
			{
				throw new Exception($"Error with binding to Numpad1 [{VK_NUMPAD1}]");
			}
			if (!RegisterHotKey(helper.Handle, HOTKEY_ID_PAD2, MOD_CTRL, VK_NUMPAD2))
			{
				throw new Exception($"Error with binding to Numpad2 [{VK_NUMPAD2}]");
			}
			if (!RegisterHotKey(helper.Handle, HOTKEY_ID_PAD3, MOD_CTRL, VK_NUMPAD3))
			{
				throw new Exception($"Error with binding to Numpad3 [{VK_NUMPAD3}]");
			}
		}

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
		/// Unregister the numpad hotkeys.
		/// </summary>
		public void UnregisterToggleServiceKey()
		{
			var helper = new WindowInteropHelper(MainWindow);
			UnregisterHotKey(helper.Handle, HOTKEY_ID_INSERT);
		}

        /// <summary>
        /// Unregister the toggle pin hotkeys.
        /// </summary>
	    public void UnregisterTogglePinKey()
	    {
            var helper = new WindowInteropHelper(MainWindow);
	        UnregisterHotKey(helper.Handle, HOTKEY_ID_TILDE);
	    }

		/// <summary>
		/// Unregister the numpad hotkeys.
		/// </summary>
		public void UnregisterHotKeyNumPad()
		{
			var helper = new WindowInteropHelper(MainWindow);
			UnregisterHotKey(helper.Handle, HOTKEY_ID_PAD1);
			UnregisterHotKey(helper.Handle, HOTKEY_ID_PAD2);
			UnregisterHotKey(helper.Handle, HOTKEY_ID_PAD3);
		}

		/// <summary>
		/// Unregister the number hotkeys.
		/// </summary>
		public void UnregisterHotKeyF()
		{
			var helper = new WindowInteropHelper(MainWindow);
			UnregisterHotKey(helper.Handle, HOTKEY_ID_F1);
			UnregisterHotKey(helper.Handle, HOTKEY_ID_F2);
			UnregisterHotKey(helper.Handle, HOTKEY_ID_F3);
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
						case HOTKEY_ID_PAD1:
							OnHotKeyPressed(KeysEnum.Numpad1);
							handled = true;
							break;

						case HOTKEY_ID_PAD2:
							OnHotKeyPressed(KeysEnum.Numpad2);
							handled = true;
							break;

						case HOTKEY_ID_PAD3:
							OnHotKeyPressed(KeysEnum.Numpad3);
							handled = true;
							break;

						case HOTKEY_ID_F1:
							OnHotKeyPressed(KeysEnum.F1);
							handled = true;
							break;

						case HOTKEY_ID_F2:
							OnHotKeyPressed(KeysEnum.F2);
							handled = true;
							break;

						case HOTKEY_ID_F3:
							OnHotKeyPressed(KeysEnum.F3);
							handled = true;
							break;

						case HOTKEY_ID_ARROW_LEFT:
							OnHotKeyPressed(KeysEnum.Left);
							handled = true;
							break;

						case HOTKEY_ID_ARROW_RIGHT:
							OnHotKeyPressed(KeysEnum.Right);
							handled = true;
							break;

						case HOTKEY_ID_INSERT:
							OnHotKeyPressed(KeysEnum.Insert);
							handled = true;
							break;

                        case HOTKEY_ID_TILDE:
                            OnHotKeyPressed(KeysEnum.Tilde);
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
				case KeysEnum.Numpad3:
				case KeysEnum.F3:
					IntPtr hwnd = GetForegroundWindow();
					uint pid;
					GetWindowThreadProcessId(hwnd, out pid);
					Process process = Process.GetProcessById((int)pid);

					if ((GetPlacement(process.MainWindowHandle).showCmd == ShowWindowCommandEnum.Normal
					     || GetPlacement(process.MainWindowHandle).showCmd == ShowWindowCommandEnum.Maximize)
					    && ViewModel.CurrentlyHiddenProcess == null)
					{
						if (process.ProcessName.ToLower().StartsWith("explorer")) return;
						if (process.ProcessName.ToLower().StartsWith("VDManager")) return;

						ViewModel.CurrentlyHiddenProcess = process;
						ViewModel.PreviousStateBeforeHiding = GetPlacement(process.MainWindowHandle).showCmd;
						ShowWindow(ViewModel.CurrentlyHiddenProcess.MainWindowHandle, (int)ShowWindowCommandEnum.Minimize);
						return;
					}
					if (ViewModel.CurrentlyHiddenProcess != null)
					{
						ShowWindow(ViewModel.CurrentlyHiddenProcess.MainWindowHandle, (int)ViewModel.PreviousStateBeforeHiding);
						ViewModel.CurrentlyHiddenProcess = null;
					}
					break;

				case KeysEnum.Numpad1:
				case KeysEnum.F1:
				case KeysEnum.Left:
                    MainWindow.SwitchLeft();
				    ManageGridSetters();
                    break;

				case KeysEnum.Numpad2:
				case KeysEnum.F2:
				case KeysEnum.Right:
					MainWindow.SwitchRight();
				    ManageGridSetters();
                    break;

				case KeysEnum.Insert:
					ViewModel.AppStatus = ViewModel.AppStatus == "RUNNING" ? "STOPPED" : "RUNNING";
					break;

                case KeysEnum.Tilde:
                    IntPtr pinHwnd = GetForegroundWindow();
                    GetWindowThreadProcessId(pinHwnd, out var pinPid);
                    Process pinProcess = Process.GetProcessById((int)pinPid);
                    WindowExtensions.TogglePin(null, pinProcess.MainWindowHandle);
                    break;
			}
		}

		/// <summary>
		/// Get the window placement.
		/// </summary>
		private static Windowplacement GetPlacement(IntPtr hwnd)
		{
			Windowplacement placement = new Windowplacement();
			placement.length = Marshal.SizeOf(placement);
			GetWindowPlacement(hwnd, ref placement);
			return placement;
		}

		/// <summary>
        /// Allow the GridSetters to be hidden or restore when changing virtual desktop.
        /// </summary>
	    private static void ManageGridSetters()
	    {
            foreach (Process process in Process.GetProcesses())
	        {
	            if (process.ProcessName.ToLower().StartsWith("gridsetter"))
	            {
                    var windowHandle = process.MainWindowHandle;
                    if (VirtualDesktopHelper.IsCurrentVirtualDesktop(windowHandle))
	                    ShowWindow(windowHandle, (int)ShowWindowCommandEnum.Maximize);
                    else
                        ShowWindow(windowHandle, (int)ShowWindowCommandEnum.Minimize);
	            }
	        }
        }

		#endregion // Methods
	}
}
