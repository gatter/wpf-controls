using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Internal
{
    /// <summary>
    /// An internal class that used to access a value based on a pair of keys.
    /// </summary>
    /// <remarks>
    /// This is a very simple class in that it only has one method to set or get a value
    /// that is accociated with a pair of keys.
    /// </remarks>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    internal class CompoundDictionary<TKey, TValue>
    {
        private Dictionary<TKey, Dictionary<TKey, TValue>> m_Map = new Dictionary<TKey, Dictionary<TKey, TValue>>();

        /// <summary>
        /// Gets or sets the value associated with the specified pair of keys. 
        /// </summary>
        /// <remarks>
        /// If a value is requested and the dictionary does not contain an entry for a pair of keys
        /// then the <see cref="System.Collections.Generic.KeyNotFoundException"/>
        /// is thrown.
        /// </remarks>
        /// <param name="key1">The first key of the association.</param>
        /// <param name="key2">The second key of the association.</param>
        /// <returns>The value associated with the two keys.</returns>
        public TValue this[TKey key1, TKey key2]
        {
            get
            {
                return m_Map[key1][key2];
            }
            set
            {
                Dictionary<TKey, TValue> subMap = null;
 
                if (!m_Map.TryGetValue(key1, out subMap))
                {
                    m_Map[key1] = subMap = new Dictionary<TKey, TValue>();
                }

                subMap[key2] = value;
            }
        }
    }
}
