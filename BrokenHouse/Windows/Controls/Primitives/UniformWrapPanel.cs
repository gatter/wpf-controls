using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Controls.Primitives
{
    /// <summary>
    /// Provides a way to arrange content in a the same way as a <see cref="System.Windows.Controls.WrapPanel"/> 
    /// but ensuring that all the child elements have the same size. 
    /// </summary>
    public class UniformWrapPanel : WrapPanel
    {
        /// <summary>
        /// Defines the layout of the <see cref="UniformWrapPanel"/> by distributing the child elements and
        /// ensuring that all the elements are of the same size.
        /// </summary>
        /// <param name="finalSize">The <see cref="System.Windows.Size"/> of the area for the panel to use. </param>
        /// <returns>The actual <see cref="System.Windows.Size"/> of the panel that is rendered to display 
        /// the child elements that are visible.</returns>
        protected override Size ArrangeOverride( Size finalSize )
        {
            var    visibleChildren   = this.EnumerateVisualChildren().OfType<UIElement>().Where(e => e.Visibility != Visibility.Collapsed);
            Size   maxDesiredSize    = new Size();
            int    visibleChildCount = 0;

            // Find the maximum size
            foreach (var child in visibleChildren)
            {
                // Update the maximum size
                maxDesiredSize = new Size(Math.Max(maxDesiredSize.Width, child.DesiredSize.Width), Math.Max(maxDesiredSize.Height, child.DesiredSize.Height));

                // Update the visible count
                visibleChildCount++;
            }
            
            // Clamp the desired size
            Rect   childBounds  = new Rect(0, 0, Math.Min(maxDesiredSize.Width, finalSize.Width), Math.Min(maxDesiredSize.Height, finalSize.Height));
            bool   isHorizontal = (Orientation == Orientation.Horizontal);

            // Position the children
            foreach (var child in visibleChildren)
            {
                child.Arrange(childBounds);

                if (isHorizontal)
                {
                    childBounds.X += maxDesiredSize.Width;

                    if (childBounds.Right > finalSize.Width)
                    {
                        childBounds.X = 0;
                        childBounds.Y += maxDesiredSize.Height;
                    }
                }
                else
                {
                    childBounds.Y += maxDesiredSize.Height;

                    if (childBounds.Bottom > finalSize.Height)
                    {
                        childBounds.Y = 0;
                        childBounds.X += maxDesiredSize.Height;
                    }
                }
            }
            
            return finalSize;
        }

        /// <summary>
        /// Computes the desired size of the <see cref="UniformWrapPanel"/> by measuring all of the child elements. 
        /// </summary>
        /// <remarks>
        /// The size of each element is defined by the maximum width and height of the child elements. Once the size
        /// has been determined the child elements are wrapped to fit the <paramref name="constraintSize"/>.
        /// </remarks>
        /// <param name="constraintSize">The <see cref="System.Windows.Size"/> of the available area for the panel. </param>
        /// <returns>The desired <see cref="System.Windows.Size"/> based on the child content of the
        /// panel and the <paramref name="constraintSize"/>.</returns>
        protected override Size MeasureOverride( Size constraintSize )
        {
            var  visibleChildren   = this.EnumerateVisualChildren().OfType<UIElement>().Where(e => e.Visibility != Visibility.Collapsed);
            Size maxDesiredSize    = new Size();
            int  visibleChildCount = 0;

            // Find the maximum size
            foreach (var child in visibleChildren)
            {
                // Measure the child
                child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                // Update the maximum size
                maxDesiredSize = new Size(Math.Max(maxDesiredSize.Width, child.DesiredSize.Width), Math.Max(maxDesiredSize.Height, child.DesiredSize.Height));

                // Update the visible count
                visibleChildCount++;
            }

            // Try to fit in the space
            double columns     = 1.0;
            double rows        = 1.0;

            // How many children can fit on a row
            if (Orientation == Orientation.Horizontal)
            {
                columns = Math.Min(Math.Max(1.0, Math.Floor(constraintSize.Width / maxDesiredSize.Width)), visibleChildCount); 
                rows    = Math.Ceiling(visibleChildCount / columns);
            }
            else
            {
                rows    = Math.Min(Math.Max(1.0, Math.Floor(constraintSize.Height / maxDesiredSize.Height)), visibleChildCount); 
                columns = Math.Ceiling(visibleChildCount / rows);
            }

            return new Size(maxDesiredSize.Width * columns, maxDesiredSize.Height * rows);
        }
    }
}