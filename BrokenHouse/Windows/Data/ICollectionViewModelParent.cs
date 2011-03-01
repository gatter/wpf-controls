using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Windows.Data
{
    /// <summary>
    /// Provides a way for an external object to add or remove a logical child.
    /// </summary>
    public interface ICollectionViewModelParent
    {
        /// <summary>
        /// Removes the suppied item as a logical child.
        /// </summary>
        /// <param name="item"></param>
        void RemoveModelItem( object item );

        /// <summary>
        /// Adds the supplied item as a logical child.
        /// </summary>
        /// <param name="item"></param>
        void AddModelItem( object item );
    }
}
