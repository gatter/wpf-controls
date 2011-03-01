using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Extensions
{
    /// <summary>
    /// Provides a set of <b>static</b> methods that extend objects that implement <see cref="System.Collections.Generic.IEnumerable{T}"/>.
    /// </summary>
    internal static class EnumerationExtensions
    {
        /// <summary>
        /// Applies an action to all the elements in the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements contained in <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence of elements on which the <paramref name="action"/> will be applied.</param>
        /// <param name="action">The <see cref="Action"/> to apply to all the elements.</param>
        public static void ForEach<T>( this IEnumerable<T> source, Action<T> action )
        {
            foreach (T item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Converts the elements in a sequence to a string with a separator.
        /// </summary>
        /// <remarks>
        /// If there are no elements in the sequence that an empty string is returned.
        /// </remarks>
        /// <typeparam name="T">The type of the elements contained in <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence of elements that we are converting to a string.</param>
        /// <param name="separator">The separator to use between the elements in the sequence.</param>
        /// <returns>The string representation of the sequence of elements.</returns>
        public static string ToString<T>( this IEnumerable<T> source, string separator )
        {
            string result = "";

            foreach (T item in source)
            {
                if (result.Length > 0)
                {
                    result += separator;
                }
                result += item.ToString();
            }

            return result;
        }
    }
}
