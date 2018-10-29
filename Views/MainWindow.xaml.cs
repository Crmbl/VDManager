using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using VDManager.Utils;
using VDManager.ViewModels;
using static System.Windows.Application;
using Application = System.Windows.Application;
using DragEventArgs = System.Windows.DragEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace VDManager.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
		#region Properties

		/// <summary>
	    /// Icon in the system tray.
	    /// </summary>
	    public NotifyIcon NotifyIcon { get; set; }

	    /// <summary>
	    /// <see cref="Utils.KeyUtil"/>.
	    /// </summary>
	    public KeyUtil KeyUtil { get; set; }

		/// <summary>
		/// The main viewmodel.
		/// </summary>
		private VDManagerViewModel ViewModel { get; }

		#endregion // Properties

		#region Constants

	    /// <summary>
	    /// Ref to the icon.
	    /// </summary>
	    private readonly Uri _iconOn = new Uri("pack://application:,,,/Resources/Images/watermelon.ico");

	    /// <summary>
	    /// Ref to the icon.
	    /// </summary>
	    private readonly Uri _iconOff = new Uri("pack://application:,,,/Resources/Images/peach.ico");

		#endregion // Constants

		#region Constructor

		/// <summary>
		/// <see cref="MainWindow"/> constructor.
		/// </summary>
		public MainWindow()
	    {
		    InitializeComponent();

		    ViewModel = new VDManagerViewModel();
		    ViewModel.CheckboxEvent += CheckboxChanged;
		    KeyUtil = new KeyUtil(ViewModel, this);

		    DataContext = ViewModel;

			#region System Tray Icon

			var resourceStream = GetResourceStream(_iconOn);
		    if (resourceStream == null) return;

		    Stream iconStream = resourceStream.Stream;
		    NotifyIcon = new NotifyIcon
		    {
			    Icon = new Icon(iconStream),
			    Visible = false
		    };
		    iconStream.Dispose();

		    NotifyIcon.DoubleClick += delegate
		    {
			    Show();
			    WindowState = WindowState.Normal;
			    NotifyIcon.Visible = false;
		    };

		    SetNotifyIconMenuItems();

			#endregion // System Tray Icon
		}

		#endregion // Constructor

		#region Methods

		/// <summary>
		/// Switch to the left virtual desktop.
		/// </summary>
		public void SwitchLeft()
		{
			if (VirtualDesktop.VirtualDesktop.Current.GetLeft() == null)
			{
				VirtualDesktop.VirtualDesktop.GetDesktops().Last().Switch();
				return;
			}

			VirtualDesktop.VirtualDesktop.Current.GetLeft().Switch();
		}

		/// <summary>
		/// Switch to the right virtual desktop.
		/// </summary>
		public void SwitchRight()
		{
			if (VirtualDesktop.VirtualDesktop.Current.GetRight() == null)
			{
				VirtualDesktop.VirtualDesktop.GetDesktops().First().Switch();
				return;
			}

			VirtualDesktop.VirtualDesktop.Current.GetRight().Switch();
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
			    NotifyIcon.Visible = true;
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
		    KeyUtil.RegisterToggleServiceKey();
	    }

		/// <summary>
		/// Event raised on app closed.
		/// </summary>
		protected override void OnClosed(EventArgs e)
	    {
		    KeyUtil.Source.RemoveHook(KeyUtil.HwndHook);
		    KeyUtil.Source = null;
			KeyUtil.UnregisterHotKeyNumPad();
			KeyUtil.UnregisterHotKeyF();
			KeyUtil.UnregisterHotKeyArrow();
			KeyUtil.UnregisterToggleServiceKey();
	        KeyUtil.UnregisterTogglePinKey();

            ViewModel.CheckboxEvent -= CheckboxChanged;
		    base.OnClosed(e);
	    }

	    /// <summary>
	    /// Used to register/unregister the hotkeys.
	    /// </summary>
	    public void CheckboxChanged(object obj, EventArgs args)
	    {
		    KeyUtil.UnregisterHotKeyNumPad();
		    KeyUtil.UnregisterHotKeyF();
		    KeyUtil.UnregisterHotKeyArrow();

		    if (ViewModel.AppStatus.Equals("STOPPED"))
			    return;

		    if (ViewModel.UseFKeys)
			    KeyUtil.RegisterHotKeyF();
		    if (ViewModel.UseNumPad)
			    KeyUtil.RegisterHotKeyNumPad();
		    if (ViewModel.UseArrows)
			    KeyUtil.RegisterHotKeyArrow();
	    }

		/// <summary>
		/// Used when the ApplicationStatus value is updated.
		/// </summary>
		private void ApplicationStatusTargetUpdated(object sender, DataTransferEventArgs e)
		{
			var isRunning = ViewModel.AppStatus == "RUNNING";

			var resourceStream = GetResourceStream(isRunning ? _iconOn : _iconOff);
		    if (resourceStream == null) return;
		    Stream iconStream = resourceStream.Stream;
		    NotifyIcon.Icon = new Icon(iconStream);
		    iconStream.Dispose();

			if (isRunning)
			{
				SetNotifyIconMenuItems();
			    KeyUtil.RegisterTogglePinKey();

                if (ViewModel.UseFKeys)
				    KeyUtil.RegisterHotKeyF();
			    if (ViewModel.UseNumPad)
				    KeyUtil.RegisterHotKeyNumPad();
			    if (ViewModel.UseArrows)
				    KeyUtil.RegisterHotKeyArrow();
            }
            else
		    {
			    SetNotifyIconMenuItems();

				KeyUtil.UnregisterHotKeyNumPad();
			    KeyUtil.UnregisterHotKeyF();
			    KeyUtil.UnregisterHotKeyArrow();
                KeyUtil.UnregisterTogglePinKey();
			}
	    }

		/// <summary>
		/// Event raised when the user click somewhere on the window and won't release click.
		/// </summary>
	    private void WindowMouseDown(object sender, MouseButtonEventArgs e)
		{
			Mouse.OverrideCursor = System.Windows.Input.Cursors.None;
			if (e.ChangedButton == MouseButton.Left)
			    DragMove();
	    }

		/// <summary>
		/// Event raised when the user release the mouse click.
		/// </summary>
	    private void WindowMouseUp(object sender, MouseButtonEventArgs e)
	    {
		    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
		}

		/// <summary>
		/// Event raised when the mouse enter the window.
		/// </summary>
	    private void WindowMouseEnter(object sender, MouseEventArgs e)
	    {
			Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
		}

		/// <summary>
		/// Event raised when the use double click on the window.
		/// </summary>
		private void WindowMouseDouble(object sender, MouseButtonEventArgs e)
		{
			if (WindowState == WindowState.Normal)
				WindowState = WindowState.Minimized;
		}

		#endregion // Events

		#region Methods

		/// <summary>
		/// Defines the NotifyIcon.ContextMenu.MenuItems.
		/// </summary>
	    public void SetNotifyIconMenuItems()
	    {
			if (NotifyIcon.ContextMenu == null)
				NotifyIcon.ContextMenu = new ContextMenu();

			NotifyIcon.ContextMenu.MenuItems.Clear();
		    NotifyIcon.ContextMenu.MenuItems.Add("Maximize", (s, e) => { Show(); WindowState = WindowState.Normal; NotifyIcon.Visible = false; });

			bool isRunning = ViewModel.AppStatus == "RUNNING";
			if (isRunning)
				NotifyIcon.ContextMenu.MenuItems.Add("Toggle Off", (s, e) => { ViewModel.AppStatus = "STOPPED"; });
			else
				NotifyIcon.ContextMenu.MenuItems.Add("Toggle On", (s, e) => { ViewModel.AppStatus = "RUNNING"; });

		    NotifyIcon.ContextMenu.MenuItems.Add("Exit", (s, e) => Current.Shutdown());
		}

		#endregion // Methods
    }
}
