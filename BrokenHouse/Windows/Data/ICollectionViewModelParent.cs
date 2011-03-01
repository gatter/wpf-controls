using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace BrokenHouse.Windows.Data
{
    /// <summary>
    /// Provides a way for an external object to add or remove a logical child.
    /// </summary>
    [SecurityCritical]
    internal interface ICollectionViewModelParent
    {
        /// <summary>
        /// Removes the suppied item as a logical child.
        /// </summary>
        /// <param name="item"></param>
        [SecurityCritical]
        void RemoveModelItem( object item );

        /// <summary>
        /// Adds the supplied item as a logical child.
        /// </summary>
        /// <param name="item"></param>
        [SecurityCritical]
        void AddModelItem( object item );
    }
}
