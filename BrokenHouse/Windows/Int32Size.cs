using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Windows;

namespace BrokenHouse.Windows
{
    /// <summary>
    /// Describes the width, height of an integer size.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Int32Size : IFormattable
    {
        private static Int32Size s_Empty = new Int32Size(0, 0);

        /// <summary>
        /// Gets or sets the horizontal component of this Size.
        /// </summary>
        public int              Width;

        /// <summary>
        /// Gets or sets the horizontal component of this Size.
        /// </summary>
        public int              Height;

        /// <summary>
        /// Gets an empty size
        /// </summary>
        public static Int32Size Empty  
        { 
            get { return s_Empty; }
        }

        /// <summary>
        /// Initializes a new instance of an <see cref="Int32Size"/> with the specified Width and Height. 
        /// </summary>
        /// <param name="width">The width component of the new <see cref="Int32Size"/>.</param>
        /// <param name="height">The height component of the new <see cref="Int32Size"/>.</param>
        public Int32Size( int width, int height )
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of an <see cref="Int32Size"/> with the specified Size. 
        /// </summary>
        /// <param name="size">The <see cref="System.Windows.Size"/> with which to initialise this <see cref="Int32Size"/>.</param>
        public Int32Size( Size size )
        {
            Width = (int)size.Width;
            Height = (int)size.Height;
        }

        /// <summary>
        /// Tests whether this <see cref="Int32Size"/> has width and height of 0.
        /// </summary>
        public bool IsEmpty
        {
            get { return ((Width == 0) && (Height == 0)); }
        }

        /// <summary>
        /// Obtain the size of the diagonal line from 0,0 to Width,Height
        /// </summary>
        public double Diagonal
        {
            get { return Math.Sqrt((Width * Width) + (Height * Height)); }
        }

        /// <summary>
        /// Compares two instances of <see cref="Int32Size"/> for equality. 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns><b>true</b> if the two sizes are equal; otherwise, <b>false</b>.</returns>
        public static bool operator ==(Int32Size left, Int32Size right)
        {
            return ((left.Width == right.Width) && (left.Height == right.Height));
        }

        /// <summary>
        /// Compares two instances of <see cref="Int32Size"/> for inequality. 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Int32Size left, Int32Size right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares two instances of Size for equality. 
        /// </summary>
        /// <param name="left">The first size to compare.</param>
        /// <param name="right">The other size to compare.</param>
        /// <returns><b>true</b> if the two sizes are not equal; otherwise, <b>false</b>.</returns>
        public static bool Equals(Int32Size left, Int32Size right)
        {
            return ((left.Width == right.Width) && (left.Height == right.Height));
        }

        /// <summary>
        /// Compares an object to an instance of <see cref="Int32Size"/> for equality. 
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><b>true</b> if the two objects are equal; otherwise, <b>false</b>.</returns>
        public override bool Equals( object obj )
        {
            return (obj is Int32Size)? Int32Size.Equals(this, (Int32Size)obj) : false;
        }

        /// <summary>
        /// Obtain the hash code for this structure.
        /// </summary>
        /// <returns>A hash code for this instance of <see cref="Int32Size"/>.</returns>
        public override int GetHashCode()
        {
            return (Width.GetHashCode() ^ Height.GetHashCode());
        }

        /// <summary>
        /// Get a string representation of this object
        /// </summary>
        /// <returns>The string representation of this instance of <see cref="Int32Size"/></returns>
        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        /// <summary>
        /// Gets a string representation of this object based on the supplied format provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>The string representation of this instance of <see cref="Int32Size"/></returns>
        public string ToString( IFormatProvider provider )
        {
            return ConvertToString(null, provider);
        }

        /// <summary>
        /// Implementation of the formattable to string
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        string IFormattable.ToString( string format, IFormatProvider provider )
        {
            return ConvertToString(format, provider);
        }

        /// <summary>
        /// This is our internal convert to string fuction
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        internal string ConvertToString(string format, IFormatProvider provider)
        {
            string result = null;

            if (IsEmpty)
            {
                result = "Empty";
            }
            else
            {
                string decimalSeparator = NumberFormatInfo.GetInstance(provider).NumberDecimalSeparator;
                char   listSeparator    = (string.IsNullOrEmpty(decimalSeparator) || (decimalSeparator[0] == ','))? ';' : ',';

                result = string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", listSeparator, Width, Height);
            }
            
            return result;
        }
    }
}
