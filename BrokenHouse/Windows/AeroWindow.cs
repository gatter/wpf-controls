using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using BrokenHouse.Windows.Interop;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows
{
    /// <summary>
    /// Represents a special window that supports Aero Glass.
    /// </summary>
    /// <remarks>
    /// The role of the AeroWindow is facilitate the creation of aero style window. When using Aero there are cases where
    /// we want to extend the glass region of the window into the client area of the window. To extend the glass frame the
    /// developer only needs to use a <see cref="BrokenHouse.Windows.Controls.GlassPanel"/> to define sections of the 
    /// Visual tree that needs to be rendered in the Glass frame of the window.
    /// </remarks>
    public class AeroWindow : Window
    {
        #region --- Dependency objects ---

        /// <summary>
        /// Identifies the <c>GlassMargin</c> dependency property.
        /// </summary>
        public static readonly DependencyProperty GlassMarginProperty;

        /// <summary>
        /// Identifies the read only <c>IsCompositionEnabled</c> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCompositionEnabledProperty;

        /// <summary>
        /// Identifies the <c>IsCompositionEnabled</c> dependency property key.
        /// </summary>
        private static readonly DependencyPropertyKey IsCompositionEnabledKey;

        #endregion

        /// <summary>
        /// Static constructor
        /// </summary>
        static AeroWindow()
        {
            // Define the keys
            IsCompositionEnabledKey = DependencyProperty.RegisterReadOnly("IsCompositionEnabled", typeof(bool), typeof(AeroWindow), new FrameworkPropertyMetadata(false));

            // Define the properties
            GlassMarginProperty = DependencyProperty.Register("GlassMargin", typeof(Thickness), typeof(AeroWindow), new FrameworkPropertyMetadata(new Thickness(), OnPropertyChangedThunk));
            IsCompositionEnabledProperty = IsCompositionEnabledKey.DependencyProperty;

            // Define the default style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AeroWindow), new FrameworkPropertyMetadata(typeof(AeroWindow)));
        }

        /// <summary>
        /// The source has been initialised - we can do the glass
        /// </summary>
        /// <param name="e"></param>
        [SecuritySafeCritical]
        protected override void OnSourceInitialized(EventArgs e)
        {
            // Use the helper to hook our WPF window up to receive window messages.
            WindowInteropHelper helper = new WindowInteropHelper(this);
            HwndSource          source = HwndSource.FromHwnd(helper.Handle);

            // Add the hoook
            source.AddHook(new HwndSourceHook(MessageProc));

            // Save the glass enabled flag
            IsCompositionEnabled = NativeMethods.IsCompositionEnabled;

            // Call the default
            base.OnSourceInitialized(e);
        }

        /// <summary>
        /// A new arrange has been triggered - ensure that the glass extends into the correct area
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);

            // Check the glass panel
            NativeMethods.ExtendGlassFrame(this, GlassMargin);

            // Return the final size
            return size;
        }

        /// <summary>
        /// We need to check to see if we have a glass panel
        /// </summary>
        [SecuritySafeCritical]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Check the glass panel
            NativeMethods.ExtendGlassFrame(this, GlassMargin);
        }

        /// <summary>
        /// We just want to listen to the Hit test
        /// </summary>
        [SecuritySafeCritical]
        private IntPtr MessageProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            IntPtr result =  IntPtr.Zero;

            if (msg == 0x031E) // WM_DWMCOMPOSITIONCHANGED
            {
                IsCompositionEnabled = NativeMethods.IsCompositionEnabled;
            }
            else if (msg == 0x0084) // WM_NCHITTEST
            {
                Point            screenPoint = new Point(lParam.ToInt32() & 0xffff, lParam.ToInt32() >> 16);
                Point            clientPoint = PointFromScreen(screenPoint);
                HitTestResult    hitTest     = VisualTreeHelper.HitTest(this, clientPoint);

                if (hitTest != null)
                {
                    FrameworkElement hitElement         = hitTest.VisualHit as FrameworkElement;
                    bool             hitGlass           = false;

                    // Did we hit something important
                    if (hitElement.FindVisualAncestor<ButtonBase>() != null)
                    {
                        // Its a button
                    }
                    else
                    {
                        Thickness nonClientMargin   = this.GetNonClientMargin();
                        Rect      clientRect        = new Rect(nonClientMargin.Left, nonClientMargin.Bottom, ActualWidth - nonClientMargin.Right, ActualHeight - nonClientMargin.Bottom);
                        Thickness glassMargin       = GlassMargin;

                        // Adjust the client rect
                        clientRect.X += glassMargin.Left;
                        clientRect.Y += glassMargin.Top;
                        clientRect.Width -= (glassMargin.Left + glassMargin.Right);
                        clientRect.Height -= (glassMargin.Top + glassMargin.Bottom);

                        // Did we hit the glass
                        hitGlass = !clientRect.Contains(clientPoint);
                    }

                    // Did we hit the glass margin
                    if (hitGlass)
                    {
                        result = new IntPtr(2);
                        handled = true;
                    }
                }
                else if ((clientPoint.Y < 0) && (clientPoint.X < 32))
                {
                    result = new IntPtr(2);
                    handled = true;
                }
                else
                {
                    // Allow the event
                }
            }

            return result;
        }


        #region --- Properties ---

        /// <summary>
        /// Flag to determine if glass is enabled
        /// </summary>
        public bool IsCompositionEnabled
        {
            get { return (bool)GetValue(IsCompositionEnabledProperty); }
            private set { SetValue(IsCompositionEnabledKey, value); }
        }
        
        /// <summary>
        /// Gets or sets the margin that is used to extend glass into the content of the control.
        /// </summary>
        public Thickness GlassMargin
        {
            get { return (Thickness)GetValue(GlassMarginProperty); }
            set { SetValue(GlassMarginProperty, value); }
        }

        #endregion

        #region --- Dependency Property EventHandlers ---

        /// <summary>
        /// A property has changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        static private void OnPropertyChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            AeroWindow window = target as AeroWindow;

            NativeMethods.ExtendGlassFrame(window, window.GlassMargin);
        }

        #endregion

    }
}
