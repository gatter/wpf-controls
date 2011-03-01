using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using BrokenHouse.Internal;

namespace BrokenHouse.Windows.Controls
{
    /// <summary>
    /// A set of icons usually used alongside of security messages.
    /// </summary>
    public static class ShieldIcons
    {
        /// <summary>
        /// When running the constructor load the icons - their creation is delayed so there should only
        /// be a small overhead
        /// </summary>
        static ShieldIcons()
        {
            Windows     = LoadIcon("WindowsShield");
            Error       = LoadIcon("ErrorShield");
            Warning     = LoadIcon("WarningShield");
            Question    = LoadIcon("QuestionShield");
            Tick        = LoadIcon("TickShield");
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
        ///  A black exclemation mark on a yellow shield that usually used to indicate a security warning.
        /// </summary>
        public static BitmapSource Warning     { get; private set; }
 
        /// <summary>
        /// A red shield with a white cross used to indicate a security error.
        /// </summary>
        public static BitmapSource Error       { get; private set; }

        /// <summary>
        ///  A white question mark on a blue shield usually used to indicate help on security.
        /// </summary>
        public static BitmapSource Question    { get; private set; }

         /// <summary>
        /// A shield with yellow, blue, red and green squares usually used to indicate UAC elevation is required on Windows Vista.
        /// </summary>
        public static BitmapSource Windows     { get; private set; }

        /// <summary>
        /// A green shield with a white tick used to indicate that there are no security errors.
        /// </summary>
        public static BitmapSource Tick        { get; private set; }
    }
}
