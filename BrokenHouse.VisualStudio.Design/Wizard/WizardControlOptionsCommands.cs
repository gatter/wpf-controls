using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BrokenHouse.Windows.VisualStudio.Design.Wizard
{
    /// <summary>
    /// A list of the comands supported by the Wizard Control
    /// </summary>
    public static class WizardControlOptionsCommands
    {
        private static RoutedUICommand    s_NewAeroPageCommand       = new RoutedUICommand("New Aero Page",       "NewAeropage",       typeof(WizardControlOptionsCommands)); 
        private static RoutedUICommand    s_NewContentPageCommand    = new RoutedUICommand("New Content Page",    "NewContentPage",    typeof(WizardControlOptionsCommands)); 
        private static RoutedUICommand    s_NewTitlePageCommand      = new RoutedUICommand("New Title Page",      "NewTitlePage",      typeof(WizardControlOptionsCommands)); 
        private static RoutedUICommand    s_DeletePageCommand        = new RoutedUICommand("Delete Page",         "DeletePage",        typeof(WizardControlOptionsCommands));
        private static RoutedUICommand    s_MovePageBackwardsCommand = new RoutedUICommand("Move Page Backwards", "MovePageBackwards", typeof(WizardControlOptionsCommands));
        private static RoutedUICommand    s_MovePageForwardsCommand  = new RoutedUICommand("Move Page Forwards",  "MovePageForwards",  typeof(WizardControlOptionsCommands)); 

        /// <summary>
        /// Gets the value that represents the New Areo Page command.
        /// </summary>
        public static RoutedUICommand NewAeroPage
        {
            get  { return s_NewAeroPageCommand; }
        }

        /// <summary>
        /// Gets the value that represents the New Content Page command.
        /// </summary>
        public static RoutedUICommand NewContentPage
        {
            get  { return s_NewContentPageCommand; }
        }
        /// <summary>
        /// Gets the value that represents the New Title Page command.
        /// </summary>
        public static RoutedUICommand NewTitlePage
        {
            get  { return s_NewTitlePageCommand; }
        }
        /// <summary>
        /// Gets the value that represents the Delete Page command.
        /// </summary>
        public static RoutedUICommand DeletePage
        {
            get  { return s_DeletePageCommand; }
        }
        /// <summary>
        /// Gets the value that represents the Move Page Backwards command.
        /// </summary>
        public static RoutedUICommand MovePageBackwards
        {
            get  { return s_MovePageBackwardsCommand; }
        }
        /// <summary>
        /// Gets the value that represents the Move Page Forwards command.
        /// </summary>
        public static RoutedUICommand MovePageForwards
        {
            get  { return s_MovePageForwardsCommand; }
        }
    }
}
