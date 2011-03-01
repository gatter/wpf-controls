using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace BrokenHouse.Windows
{
    /// <summary>
    /// A converter for the <see cref="Int32Thickness"/> object.
    /// </summary>
    public class Int32ThicknessConverter : TypeConverter
    {
        /// <summary>
        /// Determine if we can convert from the supplied type
        /// </summary>
        /// <param name="typeDescriptorContext">The format context</param>
        /// <param name="sourceType">The type to convert from</param>
        /// <returns><b>true</b> if we can do the convertion</returns>
        public override bool CanConvertFrom( ITypeDescriptorContext typeDescriptorContext, Type sourceType )
        {
            switch (Type.GetTypeCode(sourceType))
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.String:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determine if we can convert to the supplied type
        /// </summary>
        /// <param name="typeDescriptorContext">The format context</param>
        /// <param name="destinationType">The destination type to convert to</param>
        /// <returns>true if we can do the convertion</returns>
        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            return ((destinationType == typeof(InstanceDescriptor)) || (destinationType == typeof(string)));
        }

        /// <summary>
        /// Perform the conversion
        /// </summary>
        /// <param name="typeDescriptorContext">The format context</param>
        /// <param name="cultureInfo">The info about the culture</param>
        /// <param name="source">The source object</param>
        /// <returns>If conversions is successfull a Int32Thickness object </returns>
        public override object ConvertFrom( ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source )
        {
            Int32Thickness result;
            string         sourceAsString = source as string;

            // Act on the source type
            if (source == null)
            {
                throw base.GetConvertFromException(source);
            }
            else if (sourceAsString != null)
            {
                result = FromString(sourceAsString, cultureInfo);
            }
            else if (source is int)
            {
                result = new Int32Thickness((int)source);
            }
            else
            {
                result = new Int32Thickness(Convert.ToInt32(source, cultureInfo));
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// Perform the convert to an another type
        /// </summary>
        /// <param name="typeDescriptorContext">The format context</param>
        /// <param name="cultureInfo">The info about the culture</param>
        /// <param name="value">The Int32Thickness object</param>
        /// <param name="destinationType">The type we are converting to</param>
        /// <returns>If successful an object of the desired type is returned</returns>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            object result = null;

            // Sanity checks
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (!(value is Int32Thickness))
            {
                throw new ArgumentException("Unexpected argument type supplied for conversion");
            }

            // Obtain the value
            Int32Thickness thickness = (Int32Thickness) value;

            // Act on the destination type
            if (destinationType == typeof(string))
            {
                result = ToString(thickness, cultureInfo);
            }
            else if (destinationType == typeof(InstanceDescriptor))
            {
                Type[]   types  = new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) };
                object[] values = new object[] { thickness.Left, thickness.Top, thickness.Right, thickness.Bottom };

                result = new InstanceDescriptor(typeof(Int32Thickness).GetConstructor(types), values); 
            }
            else
            {
                throw new ArgumentException("Unexpected argument type supplied for conversion");
            }

            return result;
        }

        /// <summary>
        /// Helper functions to do the conversion
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="cultureInfo">The culture with which to do the conversion</param>
        /// <returns></returns>
        internal static Int32Thickness FromString( string source, CultureInfo cultureInfo )
        {
            int[]          parts = source.Split(new char[] {GetListSeparator(cultureInfo)}, StringSplitOptions.None).Select(s => Convert.ToInt32(s, cultureInfo)).ToArray();
            Int32Thickness result;

            if (parts.Length == 1)
            {
                result = new Int32Thickness(parts[0]);
            }
            else if (parts.Length == 2)
            {
                result = new Int32Thickness(parts[0], parts[1], parts[0], parts[1]);
            }
            else if (parts.Length >= 4)
            {
                result = new Int32Thickness(parts[0], parts[1], parts[2], parts[3]);
            }
            else
            {
                throw new FormatException(string.Format(cultureInfo, "'{0}' cannot be converted to a Int32Thickness", source));
            }

            return result;
        }

        /// <summary>
        /// Convert the thickness to a string
        /// </summary>
        /// <param name="thickness"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        internal static string ToString(Int32Thickness thickness, CultureInfo cultureInfo)
        {
            char          separator = GetListSeparator(cultureInfo);
            StringBuilder builder   = new StringBuilder(0x40);

            builder.Append(Convert.ToString(thickness.Left, cultureInfo));
            builder.Append(separator);
            builder.Append(Convert.ToString(thickness.Top, cultureInfo));
            builder.Append(separator);
            builder.Append(Convert.ToString(thickness.Right, cultureInfo));
            builder.Append(separator);
            builder.Append(Convert.ToString(thickness.Bottom, cultureInfo));

            return builder.ToString();
        }
 
        /// <summary>
        /// Provide a common way of obtaining the list separator
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private static char GetListSeparator( CultureInfo cultureInfo )
        {
            string decimalSeparator = NumberFormatInfo.GetInstance(cultureInfo).NumberDecimalSeparator;
            
            return (string.IsNullOrEmpty(decimalSeparator) || (decimalSeparator[0] == ','))? ';' : ',';
        }
   }

     

}
