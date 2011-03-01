using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BrokenHouse.Windows.Extensions
{
    /// <summary>
    /// Provides a set of static methods that extend the <see cref="System.Windows.Thickness"/> class.
    /// </summary>
    public static class ThicknessExtensions
    {
        /// <summary>
        /// Inflates all sides defined in a <see cref="System.Windows.Thickness"/> value by a uniform amount.
        /// </summary>
        /// <param name="thickness">The <see cref="System.Windows.Thickness"/> value that is to be inflated.</param>
        /// <param name="amount">The uniform amount by which the thickness will be inflated.</param>
        /// <returns>A new thinkness inflated by the supplied <paramref name="amount"/>.</returns>
        public static Thickness Inflate( this Thickness thickness, double amount )
        {
            return new Thickness(thickness.Left + amount, thickness.Top + amount, thickness.Right + amount, thickness.Bottom + amount);
        }

    }
}
