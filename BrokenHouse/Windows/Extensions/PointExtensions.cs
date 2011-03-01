using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BrokenHouse.Extensions;

namespace BrokenHouse.Windows.Extensions
{
    /// <summary>
    /// Provides a set of <b>static</b> methods that extend the <see cref="System.Windows.Point"/> value type.
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// Determines if the point can be considered to be zero.
        /// </summary>
        /// <param name="point">The <see cref="System.Windows.Point"/> that is to be tested</param>
        /// <returns><c>true</c> if the <paramref name="point"/> <see cref="System.Windows.Point"/> is close to zero.</returns>        
        public static bool IsZero( this Point point )
        {
            return point.X.IsZero() && point.Y.IsZero();
        }

       /// <summary>
        /// Determines if one point is close to another.
        /// </summary>
        /// <param name="value">The <see cref="System.Windows.Point"/> that is to be tested.</param>
        /// <param name="test">The <see cref="System.Windows.Point"/> that <paramref name="value"/> is to be compared agains.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> <see cref="System.Windows.Point"/> is 
        /// close to the <paramref name="test"/> <see cref="System.Windows.Point"/>.</returns>
        public static bool IsCloseTo( this Point value, Point test )
        {
            return value.X.IsCloseTo(test.X) && test.Y.IsCloseTo(test.Y);
        }
    }
}
