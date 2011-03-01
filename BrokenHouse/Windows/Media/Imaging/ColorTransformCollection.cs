using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Media.Imaging
{
    /// <summary>
    /// Represents a collection of <see cref="ColorTransform"/> objects. 
    /// This collection is used as part of a <see cref="ColorTransformGroup"/> to apply multiple transforms 
    /// to a <see cref="System.Windows.Media.Imaging.BitmapSource"/>.
    /// </summary>
    public class ColorTransformCollection : FreezableCollection<ColorTransform>
    {
        /// <summary>
        /// Simple function to crate an instance of the collectiuon.
        /// </summary>
        /// <returns></returns>
        [SecuritySafeCritical]
        protected override Freezable CreateInstanceCore()
        {
            return new ColorTransformCollection();
        }
    }

}
