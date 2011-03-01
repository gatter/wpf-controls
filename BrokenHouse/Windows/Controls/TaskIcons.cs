using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using BrokenHouse.Internal;

namespace BrokenHouse.Windows.Controls
{
    /// <summary>
    /// A set of icons used in a varaty of conditions such as <see cref="CommandLink"/> and the
    /// TaskDialog system.
    /// </summary>
    public static class TaskIcons
    {
        /// <summary>
        /// When running the constructor load the icons - their creation is delayed so there should only
        /// be a small overhead
        /// </summary>
        static TaskIcons()
        {
            Warning     = LoadIcon("Warning");
            Error       = LoadIcon("Error");
            NoEntry     = LoadIcon("NoEntry");
            Arrow       = LoadIcon("Arrow");
            Question    = LoadIcon("Question");
            Information = LoadIcon("Information");
            Tick        = LoadIcon("Tick");
            AltTick     = LoadIcon("AltTick");
        }

        /// <summary>
        /// Helper function to acutally load the icons
        /// </summary>
        /// <param name="iconName"></param>
        /// <returns></returns>
        private static BitmapSource LoadIcon( string iconName )
        {
            string            path    = "/Windows/Controls/Resources/" + iconName + ".ico";
            IconBitmapDecoder decoder = new IconBitmapDecoder(ResourceHelper.MakePackUri(path), BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
        
            return decoder.Frames[0];
        }

        /// <summary>
        /// A yellow triangle with a black exclamation mark used for indicating a warning.
        /// </summary>
        public static BitmapSource Warning     { get; private set; }
 
        /// <summary>
        /// A red circle containing an white cross used to indicate an erro has ocurred.
        /// </summary>
        public static BitmapSource Error       { get; private set; }
 
        /// <summary>
        /// A red circle with a white line used to indicate that there is no access.
        /// </summary>       
        public static BitmapSource NoEntry     { get; private set; }

        /// <summary>
        /// A greeen arrow with a white outline used to indicate input is required.
        /// </summary>
        public static BitmapSource Arrow       { get; private set; }

        /// <summary>
        /// A blue circle with a white question mark used for indication of help.
        /// </summary>
        public static BitmapSource Question    { get; private set; }

        /// <summary>
        /// A blue circle with a white i used to indicate information.
        /// </summary>
        public static BitmapSource Information { get; private set; }

        /// <summary>
        /// A white tick in a green circle used to indicate success.
        /// </summary>
        public static BitmapSource Tick        { get; private set; }

        /// <summary>
        /// A white tick in a yellow circle used as an alternative for a tick.
        /// </summary>
        public static BitmapSource AltTick     { get; private set; }
    }
}
