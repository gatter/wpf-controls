using System;
using System.Collections.Generic;
using System.Windows;

namespace BrokenHouse.Windows.Parts.Task
{
    /// <summary>
    /// The arguments contining informations as to what button was clicked
    /// </summary>
    public class TaskButtonClickEventArgs : RoutedEventArgs 
    {
        /// <summary>
        /// The button that was clicked
        /// </summary>
        public TaskButton Button { get; private set; } 
      
        /// <summary>
        /// Creates new instance of the class with default values.
        /// </summary>
        /// <param name="button">The name of the control that triggered the event</param>
        internal TaskButtonClickEventArgs( TaskButton button )
        {
             Button = button;
        }
    }
}
