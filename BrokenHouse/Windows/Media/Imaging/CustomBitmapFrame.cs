using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrokenHouse.Windows.Media.Imaging
{
    /// <summary>
    /// Provides a way to have a bitmap source with associated other frames
    /// </summary>
    public class CustomBitmapFrame : CustomBitmap
    {
        private ReadOnlyCollection<BitmapSource> m_Frames = null;

        /// <summary>
        /// Creates a CustomBitmapFrame with an image source and list of other frames
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="allFrames"></param>
        public CustomBitmapFrame( BitmapSource frame, ReadOnlyCollection<BitmapSource> allFrames )
        {
            Source = frame;

            m_Frames = allFrames;
        }
        
        /// <summary>
        /// Provide access to the other frames associated with image source
        /// </summary>
        public ReadOnlyCollection<BitmapSource> Frames
        {
            get { return m_Frames; }
        }
    }
}
