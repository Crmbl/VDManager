using System;
using System.Linq;
using System.Windows;
using WindowsDesktop;

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
			VirtualDesktop.TryGetAppUserModelId(app.GetWindowHandle(), out string appId);
            return VirtualDesktop.IsPinnedApplication(appId);
		}

		/// <summary>
		/// Pins the given app.
		/// </summary>
		public static void Pin(this Application app)
		{
            VirtualDesktop.TryGetAppUserModelId(app.GetWindowHandle(), out string appId);
            VirtualDesktop.PinApplication(appId);
		}

		/// <summary>
		/// Unpins the given app.
		/// </summary>
		/// <param name="app"></param>
		public static void Unpin(this Application app)
		{
            VirtualDesktop.TryGetAppUserModelId(app.GetWindowHandle(), out string appId);
            VirtualDesktop.UnpinApplication(appId);
		}

        /// <summary>
        /// Toggle the pin on/off for the given app.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appHandle"></param>
        public static void TogglePin(Application app, IntPtr appHandle = default(IntPtr))
		{
			string appId;
            if (appHandle == default(IntPtr))
				VirtualDesktop.TryGetAppUserModelId(app.GetWindowHandle(), out appId);
			else
				VirtualDesktop.TryGetAppUserModelId(appHandle, out appId);

			if (VirtualDesktop.IsPinnedApplication(appId))
				VirtualDesktop.UnpinApplication(appId);
			else
				VirtualDesktop.PinApplication(appId);
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
