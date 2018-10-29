using System;
using System.Linq;
using System.Windows;
using VirtualDesktop;

namespace VDManager.Utils
{
	/// <summary>
	/// Defines method for application context.
	/// </summary>
	public static class ApplicationExtensions
	{
		/// <summary>
		/// Defines if the app is pinned.
		/// </summary>
		public static bool IsPinned(this Application app)
		{
			return VirtualDesktop.VirtualDesktop.IsPinnedApplication(ApplicationHelper.GetAppId(app.GetWindowHandle()));
		}

		/// <summary>
		/// Pins the given app.
		/// </summary>
		public static void Pin(this Application app)
		{
			VirtualDesktop.VirtualDesktop.PinApplication(ApplicationHelper.GetAppId(app.GetWindowHandle()));
		}

		/// <summary>
		/// Unpins the given app.
		/// </summary>
		/// <param name="app"></param>
		public static void Unpin(this Application app)
		{
			VirtualDesktop.VirtualDesktop.UnpinApplication(ApplicationHelper.GetAppId(app.GetWindowHandle()));
		}

        /// <summary>
        /// Toggle the pin on/off for the given app.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appHandle"></param>
        public static void TogglePin(Application app, IntPtr appHandle = default(IntPtr))
		{
			var appId = appHandle == default(IntPtr) ? ApplicationHelper.GetAppId(app.GetWindowHandle()) : ApplicationHelper.GetAppId(appHandle);

			if (VirtualDesktop.VirtualDesktop.IsPinnedApplication(appId))
			{
				VirtualDesktop.VirtualDesktop.UnpinApplication(appId);
			}
			else
			{
				VirtualDesktop.VirtualDesktop.PinApplication(appId);
			}
		}

		/// <summary>
		/// Get the window handle of the given app.
		/// </summary>
		private static IntPtr GetWindowHandle(this Application app)
		{
			var window = app.Windows.OfType<Window>().FirstOrDefault();
			if (window == null) throw new InvalidOperationException();

			return window.GetHandle();
		}
	}
}
