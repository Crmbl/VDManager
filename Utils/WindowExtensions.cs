using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using WindowsDesktop;

namespace VDManager.Utils
{
	/// <summary>
	/// Defines methods for the window context.
	/// </summary>
	public static class WindowExtensions
	{
		/// <summary>
		/// Determines whether the window is located over the virtual desktop that current displayed.
		/// </summary>
		public static bool IsCurrentVirtualDesktop(this Window window)
		{
			return VirtualDesktopHelper.IsCurrentVirtualDesktop(window.GetHandle());
		}

		/// <summary>
		/// Returns a virtual desktop that window is located.
		/// </summary>
		public static VirtualDesktop GetCurrentDesktop(this Window window)
		{
			return VirtualDesktop.FromHwnd(window.GetHandle());
		}

		/// <summary>
		/// Move this window to specified virtual desktop.
		/// </summary>
		public static void MoveToDesktop(this Window window, VirtualDesktop virtualDesktop)
		{
			VirtualDesktopHelper.MoveToDesktop(window.GetHandle(), virtualDesktop);
		}

		/// <summary>
		/// Switch to virtual desktop and move the window.
		/// </summary>
		public static void SwitchAndMove(this VirtualDesktop virtualDesktop, Window window)
		{
			window.MoveToDesktop(virtualDesktop);
			virtualDesktop.Switch();
		}

		/// <summary>
		/// Defines if the current window is pinned.
		/// </summary>
		public static bool IsPinned(this Window window)
		{
			return VirtualDesktop.IsPinnedWindow(window.GetHandle());
		}

		/// <summary>
		/// Pin the given window.
		/// </summary>
		public static void Pin(this Window window)
		{
			VirtualDesktop.PinWindow(window.GetHandle());
		}

		/// <summary>
		/// Unpin the given window.
		/// </summary>
		public static void Unpin(this Window window)
		{
			VirtualDesktop.UnpinWindow(window.GetHandle());
		}

		/// <summary>
		/// Toggle on/off the pin of a given window.
		/// </summary>
		public static void TogglePin(Window window, IntPtr windowHandle = default(IntPtr))
		{
		    var handle = windowHandle == default(IntPtr) ? window.GetHandle() : windowHandle;

			if (VirtualDesktop.IsPinnedWindow(handle))
			{
				VirtualDesktop.UnpinWindow(handle);
			}
			else
			{
				VirtualDesktop.PinWindow(handle);
			}
		}

		/// <summary>
		/// Get the handle of the given window.
		/// </summary>
		internal static IntPtr GetHandle(this Visual window)
		{
			var hwndSource = (HwndSource)PresentationSource.FromVisual(window);
			if (hwndSource == null) throw new InvalidOperationException();

			return hwndSource.Handle;
		}
	}
}
