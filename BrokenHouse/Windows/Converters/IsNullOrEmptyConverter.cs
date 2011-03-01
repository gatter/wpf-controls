using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Windows.Data;
using System.Globalization;

namespace BrokenHouse.Windows.Converters
{
    /// <summary>
    /// This converter will convert a string to boolean <c>true</c> if it is <c>null</c> or empty (zero length).
    /// </summary>
    public class IsNullOrEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Return true if the supplied string is null or empty
        /// </summary>
        /// <param name="value">The value to convert, must be a string.</param>
        /// <param name="targetType">Not applicable.</param>
        /// <param name="param">Not applicable.</param>
        /// <param name="culture">Not applicable.</param>
        /// <returns><b>true</b> if the value is a null or empty string.</returns>
        [SecuritySafeCritical]
        public object Convert( object value, Type targetType, object param, CultureInfo culture )
        {
            return (value == null)? false : string.IsNullOrEmpty(value.ToString());
        }

        /// <summary>
        /// Convert back - not implemented.
        /// </summary>
        /// <param name="value">Not applicable.</param>
        /// <param name="targetType">Not applicable.</param>
        /// <param name="param">Not applicable.</param>
        /// <param name="culture">Not applicable.</param>
        /// <returns>Not applicable.</returns>
        [SecuritySafeCritical]
        public object ConvertBack( object value, Type targetType, object param, CultureInfo culture )
        {
            return null;
        }
    }
}
