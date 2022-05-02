using System.Runtime.InteropServices;

namespace Calendaro
{
    /// <summary>
    /// Defines P/Invoke wrappers for native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Flag indicating that window should be displayed in its most recent size and position without activating it.
        /// </summary>
        private const int SW_SHOWNOACTIVATE = 4;

        /// <summary>
        /// Flag indicating that window should be placed at the top of the Z order.
        /// </summary>
        private const int HWND_TOP = 0;

        /// <summary>
        /// Flag indicating that window should be shown without activation.
        /// </summary>
        private const uint SWP_NOACTIVATE = 0x0010;

        /// <summary>
        /// Size of the <see cref="FlashWindowInfo"/> structure in bytes.
        /// </summary>
        private static readonly uint FlashWindowInfoSize =
            Convert.ToUInt32(Marshal.SizeOf(new FlashWindowInfo()));

        /// <summary>
        /// Configuration for the <see cref="FlashWindowEx(ref FlashWindowInfo)"/> function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FlashWindowInfo
        {
            /// <summary>
            /// The size of the structure, in bytes.
            /// </summary>
            public uint cbSize;

            /// <summary>
            /// A handle to the window to be flashed.
            /// </summary>
            public IntPtr hwnd;

            /// <summary>
            /// The flash status.
            /// </summary>
            public FlashWindowStatus dwFlags;

            /// <summary>
            /// The number of times to flash the window.
            /// </summary>
            public uint uCount;

            /// <summary>
            /// The rate at which the window is to be flashed, in milliseconds.
            /// If set to zero, the function uses the default cursor blink rate.
            /// </summary>
            public uint dwTimeout;

            /// <summary>
            /// Initializes the structure with default values.
            /// </summary>
            public FlashWindowInfo()
            {
                cbSize = FlashWindowInfoSize;
                hwnd = IntPtr.Zero;
                dwFlags =
                    FlashWindowStatus.FlashWindowCaption
                    | FlashWindowStatus.FlashTaskbar
                    | FlashWindowStatus.TimerNoForegroung;
                uCount = uint.MaxValue;
                dwTimeout = 0;
            }
        }

        /// <summary>
        /// Supported window flashing statuses.
        /// </summary>
        public enum FlashWindowStatus : uint
        {
            /// <summary>
            /// Stop flashing. The system restores the window to its original state.
            /// </summary>    
            Stop = 0,

            /// <summary>
            /// Flash the window caption
            /// </summary>
            FlashWindowCaption = 1,

            /// <summary>
            /// Flash the taskbar button.
            /// </summary>
            FlashTaskbar = 2,

            /// <summary>
            /// Flash continuously, until the <see cref="Stop"/> flag is set.
            /// </summary>
            Timer = 4,

            /// <summary>
            /// Flash continuously until the window comes to the foreground.
            /// </summary>
            TimerNoForegroung = 12
        }

        /// <summary>
        /// Flashes the specified window.
        /// </summary>
        /// <param name="pwfi">Flashing configuration.</param>
        /// <returns>true if window caption was drawn as active before the call, otherwisefalse.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FlashWindowInfo pwfi);

        /// <summary>
        /// Flashes the specified window.
        /// </summary>
        /// <param name="form">The form that should flash.</param>
        public static void FlashWindowEx(Form form)
        {
            var configuration =
                new FlashWindowInfo
                {
                    hwnd = form.Handle,
                };

            _ = FlashWindowEx(ref configuration);
        }

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order.</param>
        /// <param name="x">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="y">The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">The new width of the window, in pixels.</param>
        /// <param name="cy">The new height of the window, in pixels.</param>
        /// <param name="uFlags">The window sizing and positioning flags.</param>
        /// <returns>true if function suceeds, otherwise false.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="nCmdShow">Controls how the window is to be shown.</param>
        /// <returns>true if the window was previously visible, otherwise false.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Brings the window to the front without activating it.
        /// </summary>
        /// <param name="form">The form to show.</param>
        public static void ShowInactiveTopmost(Form form)
        {
            ShowWindow(form.Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(
                form.Handle.ToInt32(),
                HWND_TOP,
                form.Left, form.Top,
                form.Width, form.Height,
                SWP_NOACTIVATE);
        }
    }
}
