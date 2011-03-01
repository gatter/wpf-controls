using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BrokenHouse.Windows.Controls
{
    /// <summary>
    /// A custom ScrollView that is controlled by buttons at the edge of the scrollable area.
    /// </summary>
    public class EdgeScrollViewer : ScrollViewer
    {
         static EdgeScrollViewer()
         {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EdgeScrollViewer), new FrameworkPropertyMetadata(typeof(EdgeScrollViewer)));
         }
    }
}
