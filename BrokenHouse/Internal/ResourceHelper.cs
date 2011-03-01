using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BrokenHouse.Windows.Media.Imaging;

namespace BrokenHouse.Internal
{
    /// <summary>
    /// Provides a set of internal functions to access resources contained in this assembly.
    /// </summary>
    internal static class ResourceHelper
    {
        private static string UriPackPrefix { get; set; }

        /// <summary>
        /// Static constructor - our chance to initialise things
        /// </summary>
        static ResourceHelper()
        {
            string shortName = typeof(ResourceHelper).Assembly.FullName.Split(',').FirstOrDefault();
           
            // Save the pack prefix
            UriPackPrefix = "pack://application:,,,/" + shortName + ";component";
        }

        /// <summary>
        /// Helper function to make a pack <see cref="System.Uri"/> for this assembly
        /// </summary>
        /// <param name="path">The path to the resource in this assembly.</param>
        /// <returns>The pack <see cref="System.Uri"/> that can be used to access the resource.</returns>
        public static Uri MakePackUri( string path )
        {
            return new Uri(UriPackPrefix + (path.StartsWith("/", StringComparison.CurrentCulture)? "" : "/") + path);
        }

        /// <summary>
        /// Helper function to find and load a resource dictionary based on the location of the
        /// dictionary in the assembly.
        /// </summary>
        /// <param name="location">The path to the resource dictionary in this assembly.</param>
        /// <returns>The <see cref="System.Windows.ResourceDictionary"/> found at the supplied <typeparamref name="location"/>.</returns>
        public static ResourceDictionary FindDictionary( string location )
        {
            return new ResourceDictionary { Source = MakePackUri(location) };
        }
    }
}
