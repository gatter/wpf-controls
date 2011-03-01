using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Windows;

namespace BrokenHouse.Windows
{
    /// <summary>
    /// Describes the thickness of a frame around a rectangle. 
    /// Four int values describe the <see cref="Left"/>, <see cref="Top"/>, <see cref="Right"/>, and <see cref="Bottom"/> sides of the rectangle, respectively. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(ThicknessConverter))]
    public struct Int32Thickness : IEquatable<Int32Thickness>
    {
        private int m_Left;
        private int m_Top;
        private int m_Right;
        private int m_Bottom;

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32Thickness"/> structure that has the specified uniform length on each side. 
        /// </summary>
        /// <param name="uniformLength"></param>
        public Int32Thickness( int uniformLength )
        {
            m_Left = m_Top = m_Right = m_Bottom = uniformLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32Thickness"/> structure that has specific lengths 
        /// (supplied as a int) applied to each side of the rectangle. 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public Int32Thickness( int left, int top, int right, int bottom )
        {
            m_Left = left;
            m_Top = top;
            m_Right = right;
            m_Bottom = bottom;
        }

        /// <summary>
        /// Compares this <see cref="Int32Thickness"/> structure to another Object for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><b>true</b> if the two objects are equal; otherwise, false.</returns>
        [SecuritySafeCritical]
        public override bool Equals(object obj)
        {
            if (obj is Int32Thickness)
            {
                Int32Thickness thickness = (Int32Thickness) obj;
                return (this == thickness);
            }
            return false;
        }


        /// <summary>
        /// Compares this <see cref="Int32Thickness"/> structure to another <see cref="Int32Thickness"/> for equality. 
        /// </summary>
        /// <param name="thickness">The <c>Int32Thickness</c> to compare.</param>
        /// <returns><b>true</b> if the two <c>Int32Thickness</c> are equal; otherwise, <b>false</b>.</returns>
        [SecuritySafeCritical]
        public bool Equals(Int32Thickness thickness)
        {
            return (this == thickness);
        }

        /// <summary>
        /// Returns the hash code of the structure.
        /// </summary>
        /// <returns>A hash code for this instance of <see cref="Int32Thickness"/>.</returns>
        [SecuritySafeCritical]
        public override int GetHashCode()
        {
            return (((this.m_Left.GetHashCode() ^ this.m_Top.GetHashCode()) ^ this.m_Right.GetHashCode()) ^ this.m_Bottom.GetHashCode());
        }

        /// <summary>
        /// Returns the string representation of the <see cref="Int32Thickness"/> structure.
        /// </summary>
        /// <returns>The string representation of this instance of <see cref="Int32Thickness"/></returns>
        [SecuritySafeCritical]
        public override string ToString()
        {
            return Int32ThicknessConverter.ToString(this, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compares the value of two <see cref="Int32Thickness"/> structures for equality.
        /// </summary>
        /// <param name="left">The first thickness to compare.</param>
        /// <param name="right">The other thickness to compare.</param>
        /// <returns><b>true</b> if the two thicknesses are equal; otherwise, <b>false</b>.</returns>
        public static bool operator ==( Int32Thickness left, Int32Thickness right )
        {
            return ((left.m_Left == right.m_Left) && (left.m_Top == right.m_Top) && (left.m_Right == right.m_Right) && (left.m_Bottom == right.m_Bottom));
        }

        /// <summary>
        /// Compares the value of two <see cref="Int32Thickness"/> structures for inequality.
        /// </summary>
        /// <param name="left">The first thickness to compare.</param>
        /// <param name="right">The other thickness to compare.</param>
        /// <returns><b>true</b> if the two thicknesses are not equal; otherwise, <b>false</b>.</returns>
        public static bool operator !=(Int32Thickness left, Int32Thickness right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Gets or sets the width, in pixels, of the upper side of the bounding rectangle.
        /// </summary>
        public int Top
        {
            get { return m_Top; }
            set { m_Top = value; }
        }

        /// <summary>
        /// Gets or sets the width, in pixels, of the left side of the bounding rectangle. 
        /// </summary>
        public int Left
        {
            get { return m_Left; }
            set { m_Left = value; }
        }
        /// <summary>
        /// Gets or sets the width, in pixels, of the lower side of the bounding rectangle.
        /// </summary>
        public int Bottom
        {
            get { return m_Bottom; }
            set { m_Bottom = value; }
        }

        /// <summary>
        /// Gets or sets the width, in pixels, of the right side of the bounding rectangle. 
        /// </summary>
        public int Right
        {
            get { return m_Right; }
            set { m_Right = value; }
        }

        /// <summary>
        /// Gets the combined size, in pixels, of both the vertical and horizonal sides of the bounding rectangle
        /// </summary>
        public Int32Size Size
        {
            get { return new Int32Size(m_Left + m_Right, m_Top + m_Bottom); }
        }

        /// <summary>
        /// Gets whether the thickess is uniform.
        /// </summary>
        public bool IsUniform
        {
            get { return ((m_Left == m_Top) && (m_Left == m_Right) && (m_Left == m_Bottom)); }
        }
    }
}
