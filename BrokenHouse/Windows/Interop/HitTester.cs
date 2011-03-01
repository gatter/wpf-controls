using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Permissions;

namespace BrokenHouse.Windows.Interop
{
    /// <summary>
    /// This static class to provide access extended functionality based on the cursor or screen coordinates.
    /// </summary>
    /// <remarks>
    /// There are times during certain window operations where you cannot determine the <c>Visual</c> that
    /// is under the cursor, for example, during drag and drop. The static functions contained in this
    /// class are designed to provide the same information by using calls to the Win32 SDK.
    /// </remarks>
    public static class HitTester
    {
        /// <summary>
        /// Performs a hit test based on the current position of the cursor
        /// </summary>
        /// <returns>The WPF hit test result</returns>
        public static HitTestResult HitTestFromCurrentPos()
        {
            return HitTester.HitTest(NativeMethods.CurrentPos);
        }

        /// <summary>
        /// Determines the root <see cref="System.Windows.Media.Visual"/> from the current position of the cursor
        /// </summary>
        /// <returns>The WPF window's root visual that is under the cursor</returns>
        public static Visual RootVisualFromCurrentPos()
        {
            return HitTester.RootVisualFromScreenPoint(NativeMethods.CurrentPos);
        }

        /// <summary>
        /// Determine the root <see cref="System.Windows.Media.Visual"/> from the supplied screen position
        /// </summary>
        /// <param name="screenPosition">The position on the screen to base the search</param>
        /// <returns>The root <see cref="System.Windows.Media.Visual"/> at the supplied screen position</returns>
        public static Visual RootVisualFromScreenPoint( Point screenPosition )
        {
            HwndSource hwndSource = HwndSource.FromHwnd(NativeMethods.WindowFromPoint(screenPosition));
            
            return (hwndSource != null)? hwndSource.RootVisual : null;
        }

        /// <summary>
        /// Perform a hit test from the supplied screen position
        /// </summary>
        /// <param name="screenPosition">The screen coordinates at which the hit test will be performed.</param>
        /// <returns>The result of the hit test based on the supplied screen position</returns>
        public static HitTestResult HitTest( Point screenPosition )
        {
            Visual        rootVisual = RootVisualFromScreenPoint(screenPosition);
            HitTestResult hitResult  = (rootVisual != null)? VisualTreeHelper.HitTest(rootVisual, rootVisual.PointFromScreen(screenPosition)) : null;

            return (hitResult != null)? hitResult : new PointHitTestResult(null, new Point());
        }


    }
}