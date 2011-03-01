using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BrokenHouse.Windows.Media.Imaging
{
    /// <summary>
    /// Defines a color transform. Derived classes define transforms that can be applied to a 
    /// <see cref="ColorTransformedBitmap"/>.
    /// </summary>
    public abstract class ColorTransform : Animatable
    {
        /// <summary>
        /// The function that does the actual color transformation.
        /// </summary>
        /// <param name="color">The colour to transform</param>
        /// <returns>The transformed colour.</returns>
        public abstract Color TransformColor( Color color );

        /// <summary>
        /// Gets whether the transform is an identity transform. 
        /// </summary>
        public abstract bool  IsIdentity { get; }
    }
}
