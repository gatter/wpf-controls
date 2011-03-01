using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BrokenHouse.Extensions
{
    /// <summary>
    /// Provides a set of static methods that extend the <see cref="System.Windows.Size"/> value type.
    /// </summary>
    public static class SizeExtensions
    {
        /// <summary>
        /// Obtains the size of the diagonal based on the width and height of the <i>size</i> value.
        /// </summary>
        /// <param name="size">The <see cref="System.Windows.Size"/> value from which the diagonal is obtained.</param>
        /// <returns>The length of the diagonal</returns>
        public static double GetDiagonal( this Size size )
        {
            return GetDiagonal(size.Width, size.Height);
        }

        /// <summary>
        /// Obtains the diagonal (hypotenuse) based on the shorter sides of a right-angled triangle.
        /// </summary>
        /// <param name="side1">The length of side 1.</param>
        /// <param name="side2">The length of side 2.</param>
        /// <returns>The hypotenuse of the right-angled triangle.</returns>
        public static double GetDiagonal( double side1, double side2 )
        {
            return Math.Sqrt((side1 * side1) + (side2 * side2));
        }

        /// <summary>
        /// Determines if the <see cref="System.Windows.Size"/> value is close to zero.
        /// </summary>
        /// <param name="value">The <see cref="System.Windows.Size"/> value to be tested</param>
        /// <returns><b>true</b> if the <paramref name="value"/> is close to zero</returns>
        public static bool IsZero( this Size value )
        {
            return value.Width.IsZero() && value.Height.IsZero();
        }

        /// <summary>
        /// Determines if any length of the <see cref="System.Windows.Size"/> value is NaN.
        /// </summary>
        /// <param name="value">The <see cref="System.Windows.Size"/> value to be tested</param>
        /// <returns><b>true</b> if the <paramref name="value"/> has a NaN dimension</returns>
        public static bool IsNaN( this Size value )
        {
            return double.IsNaN(value.Width) || double.IsNaN(value.Height);
        }

        /// <summary>
        /// Determine if the <see cref="System.Windows.Size"/> value is close to another.
        /// </summary>
        /// <param name="value">The <see cref="System.Windows.Size"/> value to be tested</param>
        /// <param name="other">The <see cref="System.Windows.Size"/> that <paramref name="value"/> is are being compared to</param>
        /// <returns><b>true</b> if both values are close to each other.</returns>
        public static bool IsCloseTo( this Size value, Size other )
        {
            return value.Width.IsCloseTo(other.Width) && value.Height.IsCloseTo(other.Height);
        }
    }
}
