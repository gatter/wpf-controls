using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BrokenHouse.Extensions
{
    /// <summary>
    /// Provides a set of <b>static</b> methods that extend the <see cref="System.Collections.ObjectModel.Collection{T}"/> objects.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Inserts a range of item into a collection.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the <paramref name="target"/>.</typeparam>
        /// <param name="target">The collection into which the new items will be inserted.</param>
        /// <param name="index">The index where the insertion should take place.</param>
        /// <param name="toAdd">The items that should be inserted into the collection.</param>
        static public void InsertRange<T>( this IList<T> target, int index, IEnumerable<T> toAdd )
        {
            foreach (T item in toAdd)
            {
                target.Insert(index++, item);
            }
        }

        /// <summary>
        /// Appends a range of items on to the end of a collection
        /// </summary>
        /// <typeparam name="T">The type of items contained in the <paramref name="target"/> collection.</typeparam>
        /// <param name="target">The collection on to which the new items will be appended.</param>
        /// <param name="source">The items that should be inserted into the <paramref name="target"/> collection.</param>
        static public void AddRange<T>( this IList<T> target, IEnumerable<T> source )
        {
            foreach (T item in source)
            {
                target.Add(item);
            }
        }

        /// <summary>
        /// Removes all the items from a collection that are in the supplied another collection.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the <paramref name="target"/> and <paramref name="source"/> collections.</typeparam>
        /// <param name="target">The collection from which the items will be removed.</param>
        /// <param name="source">The collection containing the items that will be removed from the <paramref name="target"/> collection.</param>
        static public void RemoveItems<T>( this IList<T> target, IEnumerable<T> source )
        {
            foreach (T item in source)
            {
                target.Remove(item);
            }
        }

        /// <summary>
        /// Replaces the items in a collection with those from another collection. The 
        /// original collection is used to ensure that the order of items is maintained,
        /// </summary>
        /// <typeparam name="T">The type of items contained in the <paramref name="target"/> and <paramref name="source"/> collections.</typeparam>
        /// <param name="target">The collection that will be updated.</param>
        /// <param name="source">The items that should be in the final collection.</param>
        static public void ReplaceItems<T>( this IList<T> target, IEnumerable<T> source )
        {
            var toRemove   = target.Where(i => !source.Contains(i)).ToList();
            var sourceList = source.ToList();

            target.RemoveItems(toRemove);

            for (int i = 0; i < sourceList.Count; i++)
            {
                if ((target.Count <= i) || !target[i].Equals(sourceList[i]))
                {
                    target.Insert(i, sourceList[i]);
                }
            }
        }

        /// <summary>
        /// Removes all the items in the collection. This is similar to clear function; however, if the collection
        /// is an ObservableCollection{T} then events will be triggered for each item that is removed.
        /// </summary>
        /// <typeparam name="T">The type of items contained in the <paramref name="target"/>.</typeparam>
        /// <param name="target">The collection that will be removed of all items</param>
        static public void RemoveAll<T>( this IList<T> target )
        {
            while (target.Count > 0)
            {
                target.RemoveAt(0);
            }
        }

    }
}
