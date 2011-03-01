using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Extensions
{
    /// <summary>
    /// Provides a set of <b>static</b> methods that extend the <see cref="double"/> value type.
    /// </summary>
    internal static class DoubleExtensions
    {
        /// <summary>
        /// Determines if a number os close to zero.
        /// </summary>
        /// <param name="value">The number that we are testing.</param>
        /// <returns><b>true</b> if <paramref name="value"/> is close to zero.</returns>
        public static bool IsZero(this double value)
        {
            return (Math.Abs(value) < 2.2204460492503131E-15);
        }

        /// <summary>
        /// Determines if a value is nearly an integer.
        /// </summary>
        /// <param name="value">The number that we are testing.</param>
        /// <returns><b>true</b> if the <paramref name="value"/> is nearly an integer.</returns>
        public static bool IsInteger(this double value)
        {        
            return Math.Round(value).IsCloseTo(value);
        }

        /// <summary>
        /// Determines if a number is less than or close to another integer.
        /// </summary>
        /// <param name="value">The number that is to be tested.</param>
        /// <param name="test">The number that <paramref name="value"/> is to be compared against.</param>
        /// <returns><b>true</b> if <paramref name="value"/> is less than or close to <paramref name="test"/>.</returns>
        public static bool IsLessThanOrCloseTo( this double value, double test )
        {
            return (value < test) || value.IsCloseTo(test);
        }

        /// <summary>
        /// Determines if one number is close to another number.
        /// </summary>
        /// <param name="value">The number that is to be tested.</param>
        /// <param name="test">The number that <paramref name="value"/> is to be compared against.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is close to <paramref name="test"/>.</returns>
        public static bool IsCloseTo( this double value, double test )
        {
            bool isClose = false;

            // Handle the quick and simple case
            if (value == test)
            {
                isClose = true;
            }
            else
            {
                double num = ((Math.Abs(value) + Math.Abs(test)) + 10.0) * 2.2204460492503131E-16;
                double diff = value - test;

                isClose = ((-num < diff) && (num > diff));
            }

            return isClose;
        }
    }
}
