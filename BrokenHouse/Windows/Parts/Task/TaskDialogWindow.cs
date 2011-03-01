using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Interop;
using BrokenHouse.Windows.Interop;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Parts.Task
{
    /// <summary>
    /// A special window to be used inconjunction with the <see cref="TaskDialogControl"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Even though this window is designed to work with the <see cref="TaskDialogControl"/>
    /// it can be used for other windows that require automatic adjustment of the 
    /// <see cref="System.Windows.FrameworkElement.MinHeight"/> property. When this window is resized,
    /// especially its width, the minimum height of the window is recalculated to ensure
    /// that the content is not sized smaller then required.
    /// </para>
    /// </remarks>
    public class TaskDialogWindow : Window
    {
        /// <summary>
        /// Property storing the hight which is currently being updated. Stop recurrsion.
        /// </summary>
        bool    IsSettingHeight   {get; set;}

        /// <summary>
        /// Flag to indicate that we are currently resizeing the dialog
        /// </summary>
        bool    IsSizing        {get; set;}

        /// <summary>
        /// The target height of the window
        /// </summary>
        double TargetMinHeight  {get; set;}
        
        /// <summary>
        /// The last measured height
        /// </summary>
        double? LastMeasuredHeight  {get; set;}
        
        /// <summary>
        /// Public constructor for the task dialog.
        /// </summary>
        public TaskDialogWindow()
        {
            NativeWindowStyles.SetCanMaximize(this, false);
            NativeWindowStyles.SetCanMinimize(this, true);
            NativeWindowStyles.SetIsSystemMenuVisible(this, false);
            Topmost = true;
        }


        /// <summary>
        /// This handler is called when the source window has been initialised.
        /// </summary>
        /// <remarks>
        /// When the source window is initialised we need to hook into windows
        /// messages so that we can respond to sizing events.
        /// </remarks>
        /// <param name="e"></param>
        [SecuritySafeCritical]
        protected override void OnSourceInitialized(EventArgs e)
        {
            // Use the helper to hook our WPF window up to receive window messages.
            WindowInteropHelper helper = new WindowInteropHelper(this);
            HwndSource          source = HwndSource.FromHwnd(helper.Handle);

            // Add the hoook
            source.AddHook(new HwndSourceHook(MessageProc));

            // Call the default
            base.OnSourceInitialized(e);

            // We want to ensure that we can size to the height
            SizeToContent = SizeToContent.Height;
        }
        
        /// <summary>
        /// Our message handler that will change the minimum size after certain windows messages
        /// </summary>
        /// <remarks>
        /// Upon receiving a WM_EXITSIZEMOVE or WM_SIZING event the minimum height is updated based
        /// on the optimum size obtained from the last measure. For example, if you expand the 
        /// dialog horizontally there will be a point where the minimum size changes. However, as we
        /// do not change the minimum size during the sizing operation we have to do it at the end.
        /// </remarks>
        private IntPtr MessageProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            IntPtr result =  IntPtr.Zero;

            if (msg == 0x0231)      // WM_ENTERSIZEMOVE
            {
                if (TargetMinHeight != 0.0)
                {
                    MinHeight = TargetMinHeight;
                }
                IsSizing = true;
            }
            else if (msg == 0x0232)      // WM_EXITSIZEMOVE
            {
                if (TargetMinHeight != 0.0)
                {
                    MinHeight = TargetMinHeight;
                }
                IsSizing = false;
            }
            else if (msg == 0x0214)  // WM_SIZING
            {
                if (TargetMinHeight > Height)
                {
                    MinHeight = TargetMinHeight;
                }
            }

            return result;
        }

        /// <summary>
        /// Our chance to work out the optimum size.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This measuring function will measure its contents twice; the first time to determine the minimum height of the
        /// content, and the second time to determine the size of the controls based on the available size. Doing the measure
        /// twice allows us to determine the minimum size without fixing the content to the mininimum size. 
        /// Once the minimum height is obtained it is used to update the <see cref="System.Windows.FrameworkElement.MinHeight"/> at the end of the sizing action. 
        /// The <see cref="System.Windows.FrameworkElement.MinHeight"/> is set in this way to avoid any ugly jumps in the height of the window which can occur
        /// if the size is changed during the measuring of the window.
        /// </para>
        /// </remarks>
        /// <param name="constraintSize"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        protected override Size  MeasureOverride( Size constraintSize )
        {
            Size measuredSize = constraintSize;

            // Do we have a child
            if (VisualChildrenCount > 0)
            {
                UIElement visualChild = GetVisualChild(0) as UIElement;

                // Is the child valid
                if (visualChild != null)
                {
                    Size nonClientAreaSize = this.GetNonClientSize();
                    
                    // Determine the available size
                    Size availableSize     = new Size(Math.Max(0.0, constraintSize.Width - nonClientAreaSize.Width), double.PositiveInfinity);

                    // Do the first measure to work out the minimum height
                    visualChild.Measure(availableSize);

                    // Save the desired height to be the target minimum height
                    TargetMinHeight = Math.Ceiling(visualChild.DesiredSize.Height) + nonClientAreaSize.Height;

                    // Adjust the available size
                    availableSize.Height = Math.Max(Math.Ceiling(visualChild.DesiredSize.Height), constraintSize.Height - nonClientAreaSize.Height);
 
                    // Do the measure
                    visualChild.Measure(availableSize);

                    // The actual measured size includes the non client area
                    measuredSize = new Size(Math.Ceiling(visualChild.DesiredSize.Width + nonClientAreaSize.Width), Math.Ceiling(visualChild.DesiredSize.Height + nonClientAreaSize.Height));
  
                    // Only adjust our size if the we have the last height and we are not actively sizeing
                    if (!IsSizing && !IsSettingHeight)
                    {
                        // Update things within the barrier
                        IsSettingHeight = true;

                        // Can we calculate any change
                        if (LastMeasuredHeight.HasValue)
                        {
                            // How much has the height changed
                            double changeInHeight = measuredSize.Height - LastMeasuredHeight.Value;

                            // Work out the new  height
                            double newHeight = Math.Max(Height + changeInHeight, measuredSize.Height);

                            // Change the height if it has changed
                            if (Height != newHeight)
                            {
                                MinHeight = 0;
                                Height = newHeight;
                            }
                        }

                        // Set the minimum height
                        MinHeight = TargetMinHeight;

                        // Clear the barrier
                        IsSettingHeight = false;
                    }

                    // Save the last measured height - use this to determine how much the size should change
                    LastMeasuredHeight = measuredSize.Height;
                }
            }

            return measuredSize;
        }
        

    }

}
