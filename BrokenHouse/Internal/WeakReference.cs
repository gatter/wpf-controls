using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Utils
{
    /// <summary>
    /// A simple generic wrapper to the standard <see cref="System.WeakReference"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class WeakReference<T> where T : class
    {
        private WeakReference           m_Inner;
        private IEqualityComparer<T>    m_Comparer;

        /// <summary>
        /// Construct a simple wrapper
        /// </summary>
        /// <param name="target">The object to track or null</param>
        public WeakReference( T target ) : this(target, null)
        {
        }

        /// <summary>
        /// Construct a simple wrapper with a comparer
        /// </summary>
        /// <param name="target">The object to track or null</param>
        /// <param name="comparer">The equaility comparer that will be used to compare an object to <paramref name="target"/></param>
        public WeakReference( T target, IEqualityComparer<T> comparer )
        {
            m_Inner = new WeakReference(target);
            m_Comparer = (comparer == null)? EqualityComparer<T>.Default : comparer;
        }

        /// <summary>
        /// Gets an indication whether the object referenced has been garbage collected.
        /// </summary>
        public bool IsAlive 
        { 
            get { return m_Inner.IsAlive; } 
        }

        /// <summary>
        /// Gets or sets the referenced object
        /// </summary>
        public T Target 
        { 
            get { return (T)m_Inner.Target; }
            set { m_Inner.Target = value; }
        }

        /// <summary>
        /// Provide a hash code for the <see cref="Target"/>.
        /// </summary>
        /// <returns>The hash code of the current <see cref="Target"/>.</returns>
        public override int GetHashCode()
        {
            return IsAlive? m_Comparer.GetHashCode(Target) : base.GetHashCode();
        }

        /// <summary>
        /// Determine if the supplied object is equal to the current <see cref="Target"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current <see cref="Target"/>.</param>
        /// <returns><c>true</c> if the specified value is queal to the current <see cref="Target"/>.</returns>
        public override bool Equals( object other )
        {
            WeakReference<T> otherReference  = other as WeakReference<T>;
            bool             otherIsAlive    = true;
            T                otherTarget     = null;
            bool             result          = false;

            // Extract the other reference
            if (otherReference != null)
            {
                otherIsAlive = otherReference.IsAlive;
                otherTarget  = otherReference.Target;
            }
            else
            {
                otherTarget = (T)other;
            }
 
            // Now the comparison
            if (otherIsAlive && IsAlive)
            {
                // Underlying object must match
                result = m_Comparer.Equals(otherTarget, Target);
            }
            else if (!otherIsAlive && !IsAlive)
            {
                // Reference wrapper must match
                result = object.ReferenceEquals(this, other);
            }
            else
            {
                // No match
            }

            return result;        
        }
    }
}
