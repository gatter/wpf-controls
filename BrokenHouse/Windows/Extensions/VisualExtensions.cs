using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;

namespace BrokenHouse.Windows.Extensions
{
    /// <summary>
    /// Provides a set of static methods that extend the <see cref="System.Windows.Media.Visual"/> class.
    /// </summary>
    /// <remarks>
    /// The main role of these extensions is to provide mechanisms to navigate the visual
    /// tree by using the standard <see cref="System.Collections.Generic.IEnumerable{T}"/> extensions.
    /// </remarks>
    public static class VisualExtensions
    {
        /// <summary>
        /// Determines if a visual has an ancestor of a specific type.
        /// </summary>
        /// <typeparam name="T">The object type that defines the ancestor type required.</typeparam>
        /// <param name="start">The <see cref="System.Windows.Media.Visual"/> from which to start the search</param>
        /// <returns><b>true</b> if the visual has an ancestor of the required type.</returns>
        public static bool HasVisualAncestor<T>( this Visual start ) 
        {
            return (FindVisualAncestor<T>(start) != null);
        }
        
        /// <summary>
        /// Obtains a sequence of visuals that defines a visual's ancestry.
        /// </summary>
        /// <param name="start">The <see cref="System.Windows.Media.Visual"/> from which to start the enumeration.</param>
        /// <returns>A sequence containing the visual's ancestry.</returns>
        public static IEnumerable<Visual> EnumerateAncestors( this Visual start )
        {
            for (Visual loop = start; loop != null; loop = VisualTreeHelper.GetParent(loop) as Visual)
            {
                yield return loop;
            }
        }

        /// <summary>
        /// Finds a visual's ancestor with a specific name.
        /// </summary>
        /// <param name="start">The <see cref="System.Windows.Media.Visual"/> from which to start the search.</param>
        /// <param name="name">The  name of the ancestor to find.</param>
        /// <returns>The ancestor that has the specific name.</returns>
        public static Visual FindVisualAncestor( this Visual start, string name ) 
        {
            return EnumerateAncestors(start).OfType<FrameworkElement>().Where(e => e.Name == name).FirstOrDefault();
        }

        /// <summary>
        /// Finds a visual's ancestor that is of a specific type.
        /// </summary>
        /// <typeparam name="T">The object type that defines the ancestor type required.</typeparam>
        /// <param name="start">The <see cref="System.Windows.Media.Visual"/> from which to start the search.</param>
        /// <returns>The ancestor that is of the required type.</returns>
        public static T FindVisualAncestor<T>( this Visual start ) 
        {
            return EnumerateAncestors(start).OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Finds the first descendant of a visual that is of a specific type.
        /// </summary>
        /// <typeparam name="T">The object type that defines the decendant type required.</typeparam>
        /// <param name="start">The visual from which to start looking</param>
        /// <returns>The descendant that is of the required type.</returns>
        public static T FindVisualDescendant<T>( this Visual start  ) 
        {
            return EnumerateVisualDescendants(start).OfType<T>().FirstOrDefault();
        }     

        /// <summary>
        /// Finds a visual's descendant with a specific name.
        /// </summary>
        /// <param name="start">The <see cref="System.Windows.Media.Visual"/> from which to start the search.</param>
        /// <param name="name">The name of the decendant to find.</param>
        /// <returns>The decendant that has the specific name.</returns>
        public static Visual FindVisualDescendant( this Visual start, string name ) 
        {
            return EnumerateVisualDescendants(start).OfType<FrameworkElement>().Where(e => e.Name == name).FirstOrDefault() as Visual;
        }    
                   
        /// <summary>
        /// Obtains a sequence of visuals that contains all of a visual's descendants.
        /// </summary>
        /// <param name="start">The <see cref="System.Windows.Media.Visual"/> from which to start the search.</param>
        /// <returns>A sequence containing all the descendants.</returns>
        public static IEnumerable<Visual> EnumerateVisualDescendants( this Visual start )
        {
            return EnumerateVisualDescendants(start, int.MaxValue);
        }

        /// <summary>
        /// Obtains a sequence of visuals that contains a visual's immediate children.
        /// </summary>
        /// <param name="parent">The <see cref="System.Windows.Media.Visual"/> from which to start the search.</param>
        /// <returns>A sequence containing the visual's immediate children.</returns>
        public static IEnumerable<Visual> EnumerateVisualChildren( this Visual parent )
        {
            return EnumerateVisualDescendants(parent, 0);
        }

        /// <summary>
        /// Obtains a sequence of visuals that contains all of a visual's descendants to a specific depth.
        /// </summary>
        /// <param name="start">The <see cref="System.Windows.Media.Visual"/> from which to start the search.</param>
        /// <param name="depth">The depth at which the sequence should stop</param>
        /// <returns>A sequence containing the descendants to the specified depth.</returns>
        public static IEnumerable<Visual> EnumerateVisualDescendants( this Visual start, int depth )
        {
            int childCount = VisualTreeHelper.GetChildrenCount(start);

            // Loop over the children
            for (int i = 0; i < childCount; i++)
            {
                Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                
                // Loop over the items
                yield return childVisual;
            }

            // Loop over the children again return their decendants
            for (int i = 0; i < childCount; i++)
            {
                Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                
                // Loop over the items
                if (depth > 0)
                {
                    foreach (var decendantVisual in EnumerateVisualDescendants(childVisual, depth--))
                    {
                        yield return decendantVisual;
                    }
                }
            }
        }
 
        /// <summary>
        /// Returns a transform used to transform coordinates from the specified visual to display pixels.
        /// </summary>
        /// <param name="visual">The <see cref="System.Windows.Media.Visual"/> from which to base the transform.</param>
        /// <returns>The <see cref="System.Windows.Media.GeneralTransform"/> containing the transform</returns>
        static public GeneralTransform TransformToPixels( this Visual visual )
        {
            GeneralTransformGroup  transformGroup      = new GeneralTransformGroup();
            PresentationSource     presentationSource  = PresentationSource.FromVisual(visual);
            Matrix                 sourceToPixelMatrix = Matrix.Identity;
            Visual                 rootVisual          = null;

            // Did we find the presentation source
            if (presentationSource != null)
            {
                rootVisual          = presentationSource.RootVisual;
                sourceToPixelMatrix = presentationSource.CompositionTarget.TransformToDevice;
            }
            else
            {
                rootVisual = visual.EnumerateAncestors().LastOrDefault();
            }

            // Do the transformation
            Matrix           rootToSourceMatrix    = GetVisualTransform(rootVisual);
            GeneralTransform visualToRootTransform = visual.TransformToAncestor(rootVisual);
 
            // Transform from visual to pixels
            transformGroup.Children.Add(visualToRootTransform);
            transformGroup.Children.Add(new MatrixTransform(rootToSourceMatrix));
            transformGroup.Children.Add(new MatrixTransform(sourceToPixelMatrix));
 
            // Return the result
            return transformGroup;
       }


        /// <summary>
        /// Returns a transform used to transform coordinates from the display pixels to the specified visual.
        /// </summary>
        /// <param name="visual">The <see cref="System.Windows.Media.Visual"/> on which to base the transform.</param>
        /// <returns>The <see cref="System.Windows.Media.GeneralTransform"/> containing the transform.</returns>
        static public GeneralTransform TransformFromPixels( this Visual visual )
        {
            GeneralTransformGroup  transformGroup      = new GeneralTransformGroup();
            PresentationSource     presentationSource  = PresentationSource.FromVisual(visual);
            Matrix                 pixelToSourceMatrix = Matrix.Identity;
            Visual                 rootVisual          = null;

            // Did we find the presentation source
            if (presentationSource != null)
            {
                rootVisual          = presentationSource.RootVisual;
                pixelToSourceMatrix = presentationSource.CompositionTarget.TransformFromDevice;
            }
            else
            {
                rootVisual = visual.EnumerateAncestors().LastOrDefault();
            }

            // Do the transformation
            Matrix           sourceToRootMatrix    = GetVisualTransform(rootVisual);
            GeneralTransform rootToVisualTransform = rootVisual.TransformToDescendant(visual);

            // Invert the source to root transform
            sourceToRootMatrix.Invert();

            // Transform from visual to pixels
            transformGroup.Children.Add(new MatrixTransform(pixelToSourceMatrix));
            transformGroup.Children.Add(new MatrixTransform(sourceToRootMatrix));

            // Chck for nulls
            if (rootToVisualTransform != null)
            {
                transformGroup.Children.Add(rootToVisualTransform);
            }
 
            // Return the result
            return transformGroup;
        }

        /// <summary>
        /// Helper function the will round a visual's point to the nearest device pixel.
        /// </summary>
        /// <param name="visual">The <see cref="System.Windows.Media.Visual"/> that defines the local coordinate space.</param>
        /// <param name="point">The <see cref="System.Windows.Point"/> in local coordinates</param>
        /// <returns>The nearest local coordinate that maps to a device pixel.</returns>
        static internal Point RoundToPixel( this Visual visual, Point point )
        {
            GeneralTransform transformToPixels   = visual.TransformToPixels();
            GeneralTransform transformFromPixels = visual.TransformFromPixels();

            // Transform from visual to pixels
            Point pixelPoint = transformToPixels.Transform(point);

            // Round the origin to the nearest whole pixel.
            pixelPoint.X = Math.Round(pixelPoint.X);
            pixelPoint.Y = Math.Round(pixelPoint.Y);

            // Transform from pixel to visuals
            Point pixelOffset = transformFromPixels.Transform(pixelPoint); 
          

            // Return the pixel offset
            return pixelOffset;
        }

        /// <summary>
        /// Obtain the transform associated with a given visual
        /// </summary>
        /// <param name="visual">The <see cref="System.Windows.Media.Visual"/> which contains the transform.</param>
        /// <returns>The visuals Transform</returns>
        private static Matrix GetVisualTransform( Visual visual )
        {
            Matrix matrix = Matrix.Identity;

            if (visual != null)
            {
                // transform the matrix
                Transform transform = VisualTreeHelper.GetTransform(visual);
                if (transform != null)
                {
                    matrix = Matrix.Multiply(matrix, transform.Value);
                }

                // Translate the matrix
                Vector offset = VisualTreeHelper.GetOffset(visual);
                matrix.Translate(offset.X, offset.Y);
            }

            return matrix;
        }

    }
}
