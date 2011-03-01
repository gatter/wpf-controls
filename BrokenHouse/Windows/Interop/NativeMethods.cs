using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Security;
using System.Security.Permissions;

namespace BrokenHouse.Windows.Interop
{
    /// <summary>
    /// The role of this class is to provide one central place for all native methods and structures.
    /// </summary>
    internal static class NativeMethods
    {
        #region --- Structures ---

        /// <summary>
        /// A C# implementation of the Win32 <c>POINT</c> structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// The X component of the <c>POINT</c>
            /// </summary>
            public int X;

            /// <summary>
            /// The Y component of the <c>POINT</c>
            /// </summary>
            public int Y;

            /// <summary>
            /// Create a <c>POINT</c> from a WPF <c>Point</c> structure.
            /// </summary>
            /// <param name="p">The WPF point.</param>
            public POINT( Point p )
            {
                X = (int)p.X;
                Y = (int)p.Y;
            }

            /// <summary>
            /// Create a <c>POINT</c> from the supplied x and y coordinates.
            /// </summary>
            /// <param name="x">The x coordinate</param>
            /// <param name="y">The y coordinate</param>
            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        
        /// <summary>
        /// A C# implementation of the Win32 <c>RECT</c> structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// The position of the left edge of the <c>RECT</c>
            /// </summary>
            public int Left;

            /// <summary>
            /// The position of the top edge of the <c>RECT</c>
            /// </summary>
            public int Top;

            /// <summary>
            /// The position of the right edge of the <c>RECT</c>
            /// </summary>
            public int Right;
 
            /// <summary>
            /// The position of the bottom edge of the <c>RECT</c>
            /// </summary>
           public int Bottom;

            /// <summary>
            /// Construct a <c>RECT</c> from the supplied information.
            /// </summary>
            /// <param name="left">The x coordinate of the TopLeft corner.</param>
            /// <param name="top">The y coordinate of the TopLeft corner.</param>
            /// <param name="right">The x coordinate of the BottomRight corner.</param>
            /// <param name="bottom">The y coordinate of the BottomRight corner.</param>
            public RECT( int left, int top, int right, int bottom )
            {
                Left   = left;
                Top    = top;
                Right  = right;
                Bottom = bottom;
            }

            /// <summary>
            /// Construct a <c>RECT</c> from a WPF <c>Rect</c>
            /// </summary>
            /// <param name="r"></param>
            public RECT( Rect r )
            {
                Left   = (int)r.Left;
                Top    = (int)r.Top;
                Right  = (int)r.Right;
                Bottom = (int)r.Bottom;
            }

            /// <summary>
            /// A read only property that provides access to the width of the rectangle
            /// </summary>
            public int Width 
            {
                get { return Right - Left; }
            }

            /// <summary>
            /// A read only property that provides access to the height of the rectangle
            /// </summary>
            public int Height 
            {
                get { return Bottom - Top; }
            }
        }
        
        /// <summary>
        /// A C# implementation of the Win32 <c>MARGINS</c> structure.
        /// </summary>
        public struct MARGINS 
        {
            /// <summary>
            /// Creates a <c>MARGINS</c> structure based on a WPF <c>Thickness</c>.
            /// </summary>
            /// <param name="thickness">A thickness object to define the margins</param>
            public MARGINS( Thickness thickness )
            {
                Left = (int)thickness.Left;
                Right = (int)thickness.Right;
                Top = (int)thickness.Top;
                Bottom = (int)thickness.Bottom;
            }

            /// <summary>
            /// The left margin
            /// </summary>
            public int Left;

            /// <summary>
            /// The right margin
            /// </summary>
            public int Right;

            /// <summary>
            /// The top margin
            /// </summary>
            public int Top;

            /// <summary>
            /// The bottom margin
            /// </summary>
            public int Bottom;
        }

        /// <summary>
        /// A C# implementation of the Win32 <c>MONITORINFOEX</c> structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto, Pack=4)]
        public class MONITORINFOEX
        {
            /// <summary>
            /// The size of this structure
            /// </summary>
            public int                cbSize      = Marshal.SizeOf(typeof(NativeMethods.MONITORINFOEX));

            /// <summary>
            /// The display monitor rectangle, expressed in virtual-screen coordinates
            /// </summary>
            public NativeMethods.RECT rcMonitor   = new NativeMethods.RECT();

            /// <summary>
            /// The work area rectangle of the display monitor that can be used by applications, expressed in virtual-screen coordinates.
            /// </summary>
            public NativeMethods.RECT rcWork      = new NativeMethods.RECT();

            /// <summary>
            /// The attributes of this monitor
            /// </summary>
            public int                dwFlags;

            /// <summary>
            /// A string that specifies the device name of the video card that the monitor is using. 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x20)]
            internal char[]             szDevice    = new char[0x20];
        }
        
        /// <summary>
        /// A C# implementation of the Win32 <c>WTA_OPTIONS</c> structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WTA_OPTIONS
        {
            /// <summary>
            /// The value of the changed flags
            /// </summary>
            public uint Flags;

            /// <summary>
            /// The mask to select which flags to change
            /// </summary>
            public uint Mask;
        }

        #endregion

        #region --- Statics ---

        /// <summary>See <c>SetWindowThemeAttribute</c> in the Win32 SDK.</summary>
        public const  int    WTA_NONCLIENT            = 0x0001;
       
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_NOSIZE               = 0x0001;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_NOMOVE               = 0x0002;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_NOZORDER             = 0x0004;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_NOREDRAW             = 0x0008;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_NOACTIVATE           = 0x0010;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_FRAMECHANGED         = 0x0020;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_SHOWWINDOW           = 0x0040;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_HIDEWINDOW           = 0x0080;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_NOCOPYBITS           = 0x0100;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_NOOWNERZORDER        = 0x0200;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_NOSENDCHANGING       = 0x0400;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_DRAWFRAME            = SWP_FRAMECHANGED;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_NOREPOSITION         = SWP_NOOWNERZORDER;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_DEFERERASE           = 0x2000;
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public const  int    SWP_ASYNCWINDOWPOS       = 0x4000;

        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public static IntPtr HWND_TOP                 = new IntPtr(0);
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public static IntPtr HWND_BOTTOM              = new IntPtr(1);
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public static IntPtr HWND_TOPMOST             = new IntPtr(-1);
        /// <summary>See <c>SetWindowPos</c> in the Win32 SDK.</summary>
        public static IntPtr HWND_NOTOPMOST           = new IntPtr(-2);

        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_HIDE                  = 0;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_SHOWNORMAL            = 1;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_NORMAL                = 1;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_SHOWMINIMIZED         = 2;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_SHOWMAXIMIZED         = 3;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_MAXIMIZE              = 3;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_SHOWNOACTIVATE        = 4;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_SHOW                  = 5;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_MINIMIZE              = 6;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_SHOWMINNOACTIVE       = 7;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_SHOWNA                = 8;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_RESTORE               = 9;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_SHOWDEFAULT           = 10;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_FORCEMINIMIZE         = 11;
        /// <summary>See <c>ShowWindow</c> in the Win32 SDK.</summary>
        public const  int    SW_MAX                   = 11;
                          
        /// <summary>See <c>MonitorFromPoint</c> in the Win32 SDK.</summary>
        public const  int    MONITOR_DEFAULTTONEAREST = 2;
        /// <summary>See <c>MonitorFromPoint</c> in the Win32 SDK.</summary>
        public const  int    MONITOR_DEFAULTTONULL    = 0;
        /// <summary>See <c>MonitorFromPoint</c> in the Win32 SDK.</summary>
        public const  int    MONITOR_DEFAULTTOPRIMARY = 1;
        /// <summary>See <c>MONITORINFO</c> in the Win32 SDK.</summary>
        public const  int    MONITORINFOF_PRIMARY     = 1;
                          
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXSCREEN              = 0;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYSCREEN              = 1;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXVSCROLL             = 2;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYHSCROLL             = 3;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYCAPTION             = 4;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXBORDER              = 5;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYBORDER              = 6;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXDLGFRAME            = 7;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYDLGFRAME            = 8;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYVTHUMB              = 9;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXHTHUMB              = 10;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXICON                = 11;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYICON                = 12;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXCURSOR              = 13;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYCURSOR              = 14;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYMENU                = 15;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXFULLSCREEN          = 16;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYFULLSCREEN          = 17;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYKANJIWINDOW         = 18;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_MOUSEPRESENT          = 19;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYVSCROLL             = 20;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXHSCROLL             = 21;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_DEBUG                 = 22;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_SWAPBUTTON            = 23;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_RESERVED1             = 24;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_RESERVED2             = 25;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_RESERVED3             = 26;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_RESERVED4             = 27;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXMIN                 = 28;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYMIN                 = 29;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXSIZE                = 30;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYSIZE                = 31;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXFRAME               = 32;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYFRAME               = 33;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXMINTRACK            = 34;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYMINTRACK            = 35;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXDOUBLECLK           = 36;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYDOUBLECLK           = 37;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXICONSPACING         = 38;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYICONSPACING         = 39;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_MENUDROPALIGNMENT     = 40;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_PENWINDOWS            = 41;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_DBCSENABLED           = 42;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CMOUSEBUTTONS         = 43;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXFIXEDFRAME          = SM_CXDLGFRAME;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYFIXEDFRAME          = SM_CYDLGFRAME;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXSIZEFRAME           = SM_CXFRAME;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYSIZEFRAME           = SM_CYFRAME;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_SECURE                = 44;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXEDGE                = 45;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYEDGE                = 46;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXMINSPACING          = 47;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYMINSPACING          = 48;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXSMICON              = 49;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYSMICON              = 50;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYSMCAPTION           = 51;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXSMSIZE              = 52;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYSMSIZE              = 53;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXMENUSIZE            = 54;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYMENUSIZE            = 55;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_ARRANGE               = 56;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXMINIMIZED           = 57;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYMINIMIZED           = 58;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXMAXTRACK            = 59;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYMAXTRACK            = 60;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXMAXIMIZED           = 61;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYMAXIMIZED           = 62;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_NETWORK               = 63;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CLEANBOOT             = 67;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXDRAG                = 68;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYDRAG                = 69;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_SHOWSOUNDS            = 70;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXMENUCHECK           = 71;   
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYMENUCHECK           = 72;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_SLOWMACHINE           = 73;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_MIDEASTENABLED        = 74;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_MOUSEWHEELPRESENT     = 75;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_XVIRTUALSCREEN        = 76;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_YVIRTUALSCREEN        = 77;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXVIRTUALSCREEN       = 78;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYVIRTUALSCREEN       = 79;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CMONITORS             = 80;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_SAMEDISPLAYFORMAT     = 81;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_IMMENABLED            = 82;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CXFOCUSBORDER         = 83;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CYFOCUSBORDER         = 84;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_TABLETPC              = 86;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_MEDIACENTER           = 87;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_STARTER               = 88;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_SERVERR2              = 89;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_REMOTESESSION         = 0x1000;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_SHUTTINGDOWN          = 0x2000;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_REMOTECONTROL         = 0x2001;
        /// <summary>See <c>GetSystemMetric</c> in the Win32 SDK.</summary>
        public const  int    SM_CARETBLINKINGENABLED  = 0x2002;
                                                     
        /// <summary>See <c>GetWindowLong</c> in the Win32 SDK.</summary>
        public const  int    GWL_STYLE                = -16;
        /// <summary>See <c>GetWindowLong</c> in the Win32 SDK.</summary>
        public const  int    GWL_EXSTYLE              = -20;

        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_OVERLAPPED            = 0x00000000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_POPUP                 = 0x80000000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_CHILD                 = 0x40000000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_MINIMIZE              = 0x20000000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_VISIBLE               = 0x10000000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_DISABLED              = 0x08000000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_CLIPSIBLINGS          = 0x04000000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_CLIPCHILDREN          = 0x02000000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_MAXIMIZE              = 0x01000000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_CAPTION               = 0x00C00000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_BORDER                = 0x00800000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_DLGFRAME              = 0x00400000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_VSCROLL               = 0x00200000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_HSCROLL               = 0x00100000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_SYSMENU               = 0x00080000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_THICKFRAME            = 0x00040000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_GROUP                 = 0x00020000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_TABSTOP               = 0x00010000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_MINIMIZEBOX           = 0x00020000;
        /// <summary>See <c>Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_MAXIMIZEBOX           = 0x00010000;
                                                     
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_DLGMODALFRAME      = 0x00000001;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_NOPARENTNOTIFY     = 0x00000004;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_TOPMOST            = 0x00000008;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_ACCEPTFILES        = 0x00000010;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_TRANSPARENT        = 0x00000020;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_MDICHILD           = 0x00000040;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_TOOLWINDOW         = 0x00000080;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_WINDOWEDGE         = 0x00000100;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_CLIENTEDGE         = 0x00000200;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_CONTEXTHELP        = 0x00000400;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_RIGHT              = 0x00001000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_LEFT               = 0x00000000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_RTLREADING         = 0x00002000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_LTRREADING         = 0x00000000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_LEFTSCROLLBAR      = 0x00004000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_RIGHTSCROLLBAR     = 0x00000000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_CONTROLPARENT      = 0x00010000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_STATICEDGE         = 0x00020000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_APPWINDOW          = 0x00040000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_OVERLAPPEDWINDOW   = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_PALETTEWINDOW      = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_LAYERED            = 0x00080000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_NOINHERITLAYOUT    = 0x00100000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_LAYOUTRTL          = 0x00400000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_COMPOSITED         = 0x02000000;
        /// <summary>See <c>Extended Window Styles</c> in the Win32 SDK.</summary>
        public const uint    WS_EX_NOACTIVATE         = 0x08000000;
                         
        /// <summary>See <c>SetWindowThemeNonClientAttributes</c> in the Win32 SDK.</summary>
        public const uint    WTNCA_NODRAWCAPTION      = 0x00000001;
        /// <summary>See <c>SetWindowThemeNonClientAttributes</c> in the Win32 SDK.</summary>
        public const uint    WTNCA_NODRAWICON         = 0x00000002;
        /// <summary>See <c>SetWindowThemeNonClientAttributes</c> in the Win32 SDK.</summary>
        public const uint    WTNCA_NOSYSMENU          = 0x00000004;
        /// <summary>See <c>SetWindowThemeNonClientAttributes</c> in the Win32 SDK.</summary>
        public const uint    WTNCA_NOMIRRORHELP       = 0x00000008;

        #endregion

        #region --- Import Classes ---
      
        /// <summary>
        /// By putting all the imports in their own class we can suppress the native call type checks
        /// </summary>
        [SecurityCritical(SecurityCriticalScope.Everything)]
        [SuppressUnmanagedCodeSecurity]
        private static class NativeMethodImports
        {
            ///<summary>
            /// A C# reference to the <c>SetWindowThemeAttribute</c> found in <c>UxTheme.dll</c>
            ///</summary>
            [DllImport("UxTheme.dll")]
            public static extern int SetWindowThemeAttribute(IntPtr hWnd, int wtype, ref WTA_OPTIONS attributes, uint size);
          
            ///<summary>
            /// A C# reference to the <c>DwmExtendFrameIntoClientArea</c> found in <c>dwmapi.dll</c>
            ///</summary>
            [DllImport("dwmapi.dll", PreserveSig = false)]
            public static extern void DwmExtendFrameIntoClientArea( IntPtr hwnd, ref NativeMethods.MARGINS margins );
      
            ///<summary>
            /// A C# reference to the <c>DwmIsCompositionEnabled</c> found in <c>dwmapi.dll</c>
            ///</summary>
            [DllImport("dwmapi.dll", PreserveSig = false)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DwmIsCompositionEnabled();

            ///<summary>
            /// A C# reference to the <c>SetWindowPos</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", SetLastError=true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);
            
            ///<summary>
            /// A C# reference to the <c>SetLastError</c> found in <c>kernel32.dll</c>
            ///</summary>
            [DllImport("kernel32.dll")]
            public static extern void SetLastError(uint error);

            ///<summary>
            /// A C# reference to the <c>GetCursorPos</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetCursorPos(out POINT pt);

            ///<summary>
            /// A C# reference to the <c>SetWindowLong</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", SetLastError=true)]
            public static extern uint SetWindowLong( IntPtr hwnd, int nIndex, uint dwNewLong );

            ///<summary>
            /// A C# reference to the <c>GetWindowLong</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", SetLastError=true)]
            public static extern uint GetWindowLong( IntPtr hwnd, int nIndex );

            ///<summary>
            /// A C# reference to the <c>GetSystemMetrics</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern int GetSystemMetrics(int nIndex);

            ///<summary>
            /// A C# reference to the <c>GetMonitorInfo</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", CharSet=CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] NativeMethods.MONITORINFOEX info);

            ///<summary>
            /// A C# reference to the <c>MonitorFromPoint</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", ExactSpelling=true)]
            public static extern IntPtr MonitorFromPoint(NativeMethods.POINT pt, int flags);

            ///<summary>
            /// A C# reference to the <c>MonitorFromRect</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", ExactSpelling=true)]
            public static extern IntPtr MonitorFromRect(ref NativeMethods.RECT rect, int flags);

            ///<summary>
            /// A C# reference to the <c>EnumDisplayMonitors</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", ExactSpelling=true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool EnumDisplayMonitors(HandleRef hdc, ref NativeMethods.RECT rcClip, NativeMethodImports.MonitorEnumProc lpfnEnum, IntPtr dwData);

            ///<summary>
            /// A C# delegate required for the <c>MonitorEnumProc</c> callback found in <c>user32.dll</c>
            ///</summary>
            public delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);

            ///<summary>
            /// A C# reference to the <c>GetWindowRect</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

            ///<summary>
            /// A C# reference to the <c>WindowFromPoint</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll")]
            public static extern IntPtr WindowFromPoint( POINT pt );

            ///<summary>
            /// A C# reference to the <c>AdjustWindowRectEx</c> found in <c>user32.dll</c>
            ///</summary>
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, [MarshalAs(UnmanagedType.Bool)] bool bMenu, uint dwExStyle);
        }

        #endregion
              
        /// <summary>
        /// Adjsut the non client theme flags by calling <c>SetWindowThemeAttribute</c>.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="flag">The flags to change</param>
        /// <param name="state">The state to which we are changing the flags.</param>
        [SecurityTreatAsSafe]
        [SecurityCritical]
        public static void SetNonClientThemeFlags( this Window window, uint flag, bool state )
        {
            IntPtr                    hwnd    = new WindowInteropHelper(window).Handle;
            NativeMethods.WTA_OPTIONS options = new NativeMethods.WTA_OPTIONS();
            uint                      size    = (uint)Marshal.SizeOf(typeof(NativeMethods.WTA_OPTIONS));

            options.Flags = state? flag : 0;
            options.Mask  = flag;

            // Call the native method
            int result = NativeMethodImports.SetWindowThemeAttribute(hwnd, NativeMethods.WTA_NONCLIENT, ref options, size);

            // Check the result
            if (result != 0)
            {
                throw new Win32Exception(result);
            }
        }

        /// <summary>
        /// Obtain the current position of the cursor in screen coordinates
        /// </summary>
        public static Point CurrentPos
        {
            [SecurityTreatAsSafe]
            [SecurityCritical]
            get
            {
                NativeMethods.POINT point;
                Point               result = new Point();

                if (NativeMethodImports.GetCursorPos(out point))
                {
                    result = new Point(point.X, point.Y);
                }
                
                return result;
            }
        }

        /// <summary>
        /// A simple property to determine if Areo Glass is enabled.
        /// </summary>
        public static bool IsCompositionEnabled
        {
            [SecurityTreatAsSafe]
            [SecurityCritical]
            get { return (Environment.OSVersion.Version.Major >= 6) && NativeMethodImports.DwmIsCompositionEnabled(); }
        }

        /// <summary>
        /// Modify the glass frame
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="margin">The amount that the class frame extends into the window's client area</param>
        /// <returns></returns>
        [SecurityTreatAsSafe]
        [SecurityCritical]
        internal static void ExtendGlassFrame( Window window, Thickness margin )
        {
            if (IsCompositionEnabled)
            {
                IntPtr                hwnd    = new WindowInteropHelper(window).Handle;
                NativeMethods.MARGINS margins = new NativeMethods.MARGINS(margin);

                // Has the handle been created
                if (hwnd == IntPtr.Zero)
                {
                    throw new InvalidOperationException("The Window must be shown before extending glass.");
                }

                // Set the background to transparent from both the WPF and Win32 perspectives
                window.Background = Brushes.Transparent;

                // Set the background colour
                HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

                // Demand the permission to access the window
                new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();

                // Extend the frame
                NativeMethodImports.DwmExtendFrameIntoClientArea(hwnd, ref margins);
            }
        }

        /// <summary>
        /// Bring the window to the front of all the current windows by using the <c>SetWindowPos</c>.
        /// </summary>
        /// <param name="window">The target window</param>
        [SecurityTreatAsSafe]
        [SecurityCritical]
        public static void BringToFront( this Window window )
        {
            IntPtr hwnd  = new WindowInteropHelper(window).Handle;

            // Move the window to the front
            NativeMethodImports.SetWindowPos(hwnd, NativeMethods.HWND_TOP, 0, 0, 0, 0, 
                                             NativeMethods.SWP_SHOWWINDOW | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_FRAMECHANGED);
        }


        /// <summary>
        /// Set the window flags by using the <c>GetWindowLong</c> and <c>SetWindowLong</c> SDK calls.
        /// </summary>
        /// <param name="window">The target window.</param>
        /// <param name="index">Must be either GWL_STYLE or GWL_EXSTYLE</param>
        /// <param name="flag"></param>
        /// <param name="state"></param>
        [SecurityCritical]
        [SecurityTreatAsSafe]
        private static void SetWindowFlags( this Window window, int index, uint flag, bool state )
        {
            IntPtr hwnd       = new WindowInteropHelper(window).Handle;
            int    error      = 0;
            uint   newValue   = 0;
            uint   oldValue   = 0;

            // Sanity check
            if ((index != NativeMethods.GWL_EXSTYLE) && (index != NativeMethods.GWL_STYLE))
            {
                throw new InvalidOperationException("Can only call SetWindowFlags with an index of GWL_STYLE or GWL_EXSTYLE");
            }

            // Clear any error
            NativeMethodImports.SetLastError(0);

            // Safely get the value of the window long
            oldValue = NativeMethodImports.GetWindowLong(hwnd, index);
            error = Marshal.GetLastWin32Error();
            
            // Check the error
            if ((oldValue == 0) && (error != 0))
            {
                throw new Win32Exception(error);
            }

            // Build up the new value
            newValue = oldValue;

            // Adjust the flag
            if (state)
            {
                newValue |= flag;
            }
            else
            {
                newValue &= ~flag;
            }

            // Clear any error
            NativeMethodImports.SetLastError(0);

            // Set the value
            oldValue = NativeMethodImports.SetWindowLong(hwnd, index, newValue);

            // Check the error
            error = Marshal.GetLastWin32Error();
              
            // Check the error
            if ((oldValue == 0) && (error != 0))
            {
                throw new Win32Exception(error);
            }
        }

        /// <summary>
        /// Controls the context help control box of a window
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>true</c> if you want to show the context help control box.</param>
        [SecurityTreatAsSafe]
        public static void ShowContextHelp( this Window window, bool state )
        {
            SetWindowFlags(window, NativeMethods.GWL_EXSTYLE, NativeMethods.WS_EX_CONTEXTHELP, state);
        }

        /// <summary>
        /// Controls the minimise control box
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>true</c> if you want to show the minimisecontrol box.</param>
        [SecurityTreatAsSafe]
        public static void ShowMinimiseBox( this Window window, bool state )
        {
            SetWindowFlags(window, NativeMethods.GWL_STYLE, NativeMethods.WS_MINIMIZEBOX, state);
        }

        /// <summary>
        /// Controls the maximise control box
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>true</c> if you want to show the maximise control box.</param>
        [SecurityTreatAsSafe]
        public static void ShowMaximiseBox( this Window window, bool state )
        {
            SetWindowFlags(window, NativeMethods.GWL_STYLE, NativeMethods.WS_MAXIMIZEBOX, state);
        }
        
        /// <summary>
        /// Controls whether the window's caption is hidden by using <c>SetNonClientThemeFlags</c>.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>false</c> if you want to hide the window caption</param>
        [SecurityTreatAsSafe]
        public static void ShowWindowCaption( this Window window, bool state )
        {
            SetNonClientThemeFlags(window, NativeMethods.WTNCA_NODRAWCAPTION, !state);
        }

        /// <summary>
        /// Controls whether the window's icon is hidden by using <c>SetNonClientThemeFlags</c>.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>false</c> if you want to hide the window icon</param>
        [SecurityTreatAsSafe]
        public static void ShowWindowIcon( this Window window, bool state )
        {
            SetNonClientThemeFlags(window, NativeMethods.WTNCA_NODRAWICON, !state);
        }

        /// <summary>
        /// Controls whether the window is a layered window by adjusting the windows extended style flags
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>true</c> if you want to make the window a lyered window</param>
        [SecurityTreatAsSafe]
        public static void SetLayered( this Window window, bool state )
        {
            SetWindowFlags(window, NativeMethods.GWL_EXSTYLE, NativeMethods.WS_EX_LAYERED, state);
        }

 
        /// <summary>
        /// Controls whether the window is transparent to input by adjusting the windows extended style flags
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>true</c> if you want to make the window transparent</param>
        [SecurityTreatAsSafe]
        public static void SetTransparent( this Window window, bool state )
        {
            SetWindowFlags(window, NativeMethods.GWL_EXSTYLE, NativeMethods.WS_EX_TRANSPARENT, state);
        }

        /// <summary>
        /// Controls the window's <c>No Activate</c>
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>true</c> if you want to stop the window becoming active when it is shown.</param>
        [SecurityTreatAsSafe]
        public static void SetNoActivate( this Window window, bool state )
        {
            SetWindowFlags(window, NativeMethods.GWL_EXSTYLE, NativeMethods.WS_EX_NOACTIVATE, state);
        }
        
        /// <summary>
        /// Sets whether the window has an icon and close button. 
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>true</c> if you want the window to have a sys menu</param>
        [SecurityTreatAsSafe]
        public static void ShowSystemMenu( this Window window, bool state )
        {
            SetWindowFlags(window, NativeMethods.GWL_STYLE, NativeMethods.WS_SYSMENU, state);
        }

        /// <summary>
        /// Sets the windows popup style
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="state"><c>true</c> if you want the window to become a popup window</param>
        [SecurityTreatAsSafe]
        public static void SetPopup( this Window window, bool state )
        {
            SetWindowFlags(window, NativeMethods.GWL_STYLE, NativeMethods.WS_POPUP, state);
        }

        /// <summary>
        /// Obtain a Hwnd from a screen position
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        [SecurityTreatAsSafe]
        [SecurityCritical]
        public static IntPtr WindowFromPoint( Point screenPosition )
        {
            return NativeMethodImports.WindowFromPoint(new NativeMethods.POINT(screenPosition));
        }

        /// <summary>
        /// Obtains the non-client margin of the window 
        /// </summary>
        /// <param name="window">The target window</param>
        /// <returns>The bounds of the client area of the window.</returns>
        [SecurityTreatAsSafe]
        [SecurityCritical]
        public static Thickness GetNonClientMargin( this Window window )
        {
            IntPtr hwnd    = new WindowInteropHelper(window).Handle;
            uint   styleEx = NativeMethodImports.GetWindowLong(hwnd, GWL_EXSTYLE);
            uint   style   = NativeMethodImports.GetWindowLong(hwnd, GWL_STYLE);

            // Define the rects
            NativeMethods.RECT baseRect     = new NativeMethods.RECT(100, 100, 500, 500);
            NativeMethods.RECT adjustedRect = new NativeMethods.RECT(baseRect.Left, baseRect.Top, baseRect.Right, baseRect.Bottom);

            // Adjust the rect
            NativeMethodImports.AdjustWindowRectEx(ref adjustedRect, style, false, styleEx);

            // Work out the size
            Point      topLeft      = new Point((double) (baseRect.Left - adjustedRect.Left), (double) (baseRect.Top - adjustedRect.Top));
            Point      bottomRight = new Point((double) (adjustedRect.Right - baseRect.Right), (double) (adjustedRect.Bottom - baseRect.Bottom));
            HwndSource sourceWindow = HwndSource.FromHwnd(hwnd);

            // Adjust the point
            topLeft = sourceWindow.CompositionTarget.TransformFromDevice.Transform(topLeft);
            bottomRight = sourceWindow.CompositionTarget.TransformFromDevice.Transform(bottomRight);

            // Return the size
            return new Thickness(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }

        /// <summary>
        /// Obtains the total size of the non-client margin of the window 
        /// </summary>
        /// <param name="window">The target window</param>
        /// <returns>The size of the non-client margins</returns>
        public static Size GetNonClientSize( this Window window )
        {
            Thickness nonClientAreaMargin = GetNonClientMargin(window);
            
            return new Size(nonClientAreaMargin.Left + nonClientAreaMargin.Right, nonClientAreaMargin.Top + nonClientAreaMargin.Bottom);
        }
    }
}
