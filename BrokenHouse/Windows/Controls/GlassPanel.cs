using System.Linq;
using System;
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
using BrokenHouse.Utils;

namespace BrokenHouse.Windows.Controls
{
    /// <summary>
    /// A special class that defines which part of a window should be placed in the Glass frame of an <see cref="AeroWindow"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When Aero is enabled any window can be altered to define how far the glass frame should be extended into the window's
    /// client area. <see cref="AeroWindow"/> has a property <see cref="AeroWindow.GlassMargin">GlassMargin</see> which 
    /// defines how much the glass margin should be extended. However, this may need to be periodically updated to ensure 
    /// that the correct area of the windows is rendered in the glass frame. The <see cref="GlassPanel"/> is a special version
    /// of the standard <see cref="System.Windows.Controls.DockPanel"/> that attaches to a parent <see cref="AeroWindow"/> and
    /// ensures that the <see cref="AeroWindow.GlassMargin">GlassMargin</see> is set to ensure that all the docked elements
    /// are kept in the glass margin.
    /// </para>
    /// </remarks>
    /// <seealso cref="AeroWindow"/>
    /// <seealso cref="System.Windows.Controls.DockPanel"/>
    public class GlassPanel : DockPanel
    {
        private AeroWindow m_AttachedWindow = null;
        
        /// <summary>
        /// We have been initialized.
        /// </summary>
        /// <remarks>
        /// When we are initialized we have to set up an element handler that will update the attached window when this objects
        /// layout has changed. We have to check all cases where there is a posibility that our ancestry has changed.
        /// The attached window will only be updated if the ancestor that we have attached to has changed.
        /// </remarks>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data</param>
        [SecuritySafeCritical]
        protected override void OnInitialized( EventArgs e )
        {
            // Call the default
            base.OnInitialized(e);

            // Hook up to the event
            LayoutUpdated += delegate { UpdateAttachedWindow(); };
        }

        /// <summary>
        /// Update the attached window.
        /// </summary>
        private void UpdateAttachedWindow()
        {
            // Find the new wondow
            AeroWindow newAttachedWindow = this.FindVisualAncestor<AeroWindow>();

            // Has the window actually changed
            if (newAttachedWindow != m_AttachedWindow)
            {
                m_AttachedWindow = newAttachedWindow;

                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Arranges the children and then works out how much the Glass frame should be extended.
        /// </summary>
        /// <param name="arrangeSize">The <see cref="System.Windows.Size"/> in which the children should be arranged.</param>
        /// <returns>The <see cref="System.Windows.Size"/> that represents the rendered size of this element</returns>
        [SecuritySafeCritical]
        protected override Size ArrangeOverride( Size arrangeSize )
        {
            Size  finalSize   = base.ArrangeOverride(arrangeSize);

            // Are we attached to an aero window
            if (m_AttachedWindow != null)
            {
                Rect  nonDockedBounds = new Rect(0, 0, arrangeSize.Width, arrangeSize.Height);
                int   childCount      = VisualChildrenCount;
                int   dockedCount     = childCount - (LastChildFill? 1 : 0);

                // Find the margins by looping over the children
                for (int i = 0; i < childCount; i++)
                {
                    UIElement child = GetVisualChild(i) as UIElement;
                    Size      desiredSize = child.DesiredSize;

                    if (i < dockedCount)
                    {
                        switch (GetDock(child))
                        {
                            case Dock.Left:
                                nonDockedBounds.X += desiredSize.Width;
                                nonDockedBounds.Width -= desiredSize.Width;
                                break;

                            case Dock.Top:
                                nonDockedBounds.Y += desiredSize.Height;
                                nonDockedBounds.Height -= desiredSize.Height;
                                break;

                            case Dock.Right:
                                nonDockedBounds.Width -= desiredSize.Width;
                                break;

                            case Dock.Bottom:
                                nonDockedBounds.Height -= desiredSize.Height;
                                break;
                        }
                    }
                }

                // Need to convert this non-glass rect to a margin on the aero window
                Rect      nonGlassBounds = TransformToAncestor(m_AttachedWindow).TransformBounds(nonDockedBounds);

                // Calcuate the thickness
                Size      nonClientSize = m_AttachedWindow.GetNonClientSize();
                Thickness glassMargin   = new Thickness(nonGlassBounds.Left, nonGlassBounds.Top, 
                                                          m_AttachedWindow.Width - (nonClientSize.Width + nonGlassBounds.Right), 
                                                          m_AttachedWindow.Height - (nonClientSize.Height + nonGlassBounds.Bottom));

                // Set the glass margin
                m_AttachedWindow.GlassMargin = glassMargin;
            }
            return finalSize;
        }

        /// <summary>
        /// Called when the visual parent has changed.
        /// </summary>
        /// <param name="oldParent"></param>
        [SecuritySafeCritical]
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            // Update the attached window because our parent has definately changed
            UpdateAttachedWindow();

            // Call the super classes implementation
            base.OnVisualParentChanged(oldParent);
        }
    }
}
