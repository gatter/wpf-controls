using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Controls
{
    /// <summary>
    /// This class forces the top-left corner of the child element so that 
    /// it is snapped to device pixels.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One of the issues with WPF layout is that any element in the display
    /// can find its positioned so that its local coordinates are not aligned
    /// to pixels; resulting in blurred rendering. For a small number of XAML
    /// element this effect can be elimited by setting the  
    /// <see cref="System.Windows.UIElement.SnapsToDevicePixels"/> property to <c>True</c>.
    /// Unfortunately, <see cref="System.Windows.Controls.Image"/> control does not support
    /// the <see cref="System.Windows.UIElement.SnapsToDevicePixels"/> property and as such
    /// it has a tendancy to render blurred images.
    /// </para>
    /// <para>
    /// The role of the <c>SnapDecorator</c> is to work out the offset that needs
    /// to be applied to its child so that the childs's local coordinates maps
    /// exactly to a device pixels. The components of this offset can either
    /// be positive or negative depending on the direction of the closest pixel.
    /// By applying an offset in this manner the content of the 
    /// <c>SnapDecorator</c> could extend beyond its own boundry by at least 0.5 pixels
    /// in any direction.
    /// </para>
    /// <para>
    /// If your are rendering images then the <see cref="Icon"/> element should be used as 
    /// this is significantly more efficient for rendering images. For all other cases this 
    /// <c>SnapDecorator</c> can be used.
    /// </para>
    /// <seealso cref="Icon"/>
    /// </remarks>
    public class SnapDecorator : Decorator
    {
        private Point        m_Offset = new Point(0.0, 0.0);

        /// <summary>
        /// Default constructor
        /// </summary>
        public SnapDecorator()
        {
            LayoutUpdated += OnLayoutUpdated;
        }

        /// <summary>
        /// The layout has been updated - check to see if we need to adjust the offset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (Child != null)
            {
                // Transform to the top level
                Point childOffset = this.RoundToPixel(new Point(0.5, 0.5));
      
                // Would the offset have changed
                if ((childOffset.X != m_Offset.X) || (childOffset.Y != m_Offset.Y))
                {
                    // Save the offset
                    m_Offset = childOffset;

                    // Trigger an arrange so that we can take account of the new offset
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// Arranges the child so that it is coordinates map directly to display pixels.
        /// </summary>
        /// <param name="arrangeSize">The <see cref="System.Windows.Size"/> in which the child should be arranged.</param>
        /// <returns>The actual <see cref="System.Windows.Size"/> used by this element.</returns>
        protected override Size ArrangeOverride( Size arrangeSize )
        {
            // Do we have a childw
            if (Child != null)
            {
                double childWidth  = Math.Max(arrangeSize.Width - m_Offset.X, 0.0);
                double childHeight = Math.Max(arrangeSize.Height - m_Offset.Y, 0.0);
                Rect   childBounds = new Rect(m_Offset, new Size(childWidth, childHeight));

                // Arrange the child
                Child.Arrange(childBounds);
           }

            // Return the size
            return arrangeSize;
        }
    }

}
