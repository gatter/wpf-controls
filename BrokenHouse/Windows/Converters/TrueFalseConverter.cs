using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace BrokenHouse.Windows.Converters
{
    /// <summary>
    /// This converter will convert between <b>true</b> and <b>false</b> and vice versa.
    /// </summary>
    public class TrueFalseConverter : IValueConverter
    {
        /// <summary>
        /// Return <b>true</b> if the supplied parameter is <b>false</b> and vice versa. 
        /// </summary>
        /// <remarks>
        /// If the value to be converted is <b>null</b> then this is assumed to be <b>false</b>
        /// and the converted value will be <b>true</b>. In all other cases the <see cref="System.Convert"/>
        /// class will be used to perform any conversion.
        /// </remarks>
        /// <param name="value">The value to flip.</param>
        /// <param name="targetType">The target type; ignored as the result will always be a boolean. </param>
        /// <param name="param">Ignored.</param>
        /// <param name="culture">The culture to use in any conversions.</param>
        /// <returns>The boolean inverse of the input.</returns>
        public object Convert( object value, Type targetType, object param, CultureInfo culture )
        {
            bool result = false;

            if (value == null)
            {
                // Null corresponds to false (flip to true).
                result = true;
            }
            else 
            {
                try
                {
                    result = !((bool)System.Convert.ChangeType(value, typeof(bool), culture));
                }
                catch ( InvalidCastException )
                {
                    // Non-null converts to true - flip to false;
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// As this converter essentially flips a boolean from <b>true</b> to <b>false</b> the
        /// <see cref="Convert"/> and <see cref="ConvertBack"/> perform the same function.
        /// </summary>
        /// <param name="value">The value to flip.</param>
        /// <param name="targetType">The target type; ignored as the result will always be a boolean. </param>
        /// <param name="param">Ignored.</param>
        /// <param name="culture">The culture to use in any conversions.</param>
        /// <returns>The boolean inverse of the input.</returns>
        public object ConvertBack( object value, Type targetType, object param, CultureInfo culture )
        {
            return Convert(value, targetType, param, culture);
        }
    }
}
