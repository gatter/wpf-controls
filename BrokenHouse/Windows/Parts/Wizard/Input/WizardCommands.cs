using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BrokenHouse.Windows.Parts.Wizard.Input
{
    /// <summary>
    /// Provides a standard set of commands for use with the Wizard framework.
    /// </summary>
    /// <remarks>
    /// The commands in the WizardCommands class are intended to represent a set of common commands that 
    /// are required as part of a wizard. The commands only represent the instance of the <c>RoutedUICommand</c> 
    /// and not the implementation logic for the command. The implementation logic is bound to the command with a <c>CommandBinding</c>. 
    /// </remarks>
    public class WizardCommands
    {
        private static RoutedUICommand    s_NextCommand           = new RoutedUICommand("Next",           "Next",           typeof(WizardCommands)); 
        private static RoutedUICommand    s_BackCommand           = new RoutedUICommand("Back",           "Back",           typeof(WizardCommands)); 
        private static RoutedUICommand    s_FinishCommand         = new RoutedUICommand("Finish",         "Finish",         typeof(WizardCommands)); 
        private static RoutedUICommand    s_CancelCommand         = new RoutedUICommand("Cancel",         "Cancel",         typeof(WizardCommands));
        private static RoutedUICommand    s_MoveToCommand         = new RoutedUICommand("MoveTo",         "MoveTo",         typeof(WizardCommands));
        private static RoutedUICommand    s_AddCommand            = new RoutedUICommand("Add",            "Add",            typeof(WizardCommands));
        private static RoutedUICommand    s_CloseLastErrorCommand = new RoutedUICommand("CloseLastError", "CloseLastError", typeof(WizardCommands)); 

        /// <summary>
        /// Gets the value that represents the Next wizard command.
        /// </summary>
        public static RoutedUICommand Next
        {
            get  { return s_NextCommand; }
        }

        /// <summary>
        /// Gets the value that represents the Back wizard command.
        /// </summary>
        public static RoutedUICommand Back
        {
            get  { return s_BackCommand; }
        }

        /// <summary>
        /// Gets the value that represents the Finish wizard command.
        /// </summary>
        public static RoutedUICommand Finish
        {
            get  { return s_FinishCommand; }
        }

        /// <summary>
        /// Gets the value that represents the Cancel wizard command.
        /// </summary>
        public static RoutedUICommand Cancel
        {
            get  { return s_CancelCommand; }
        }

        /// <summary>
        /// Gets the value that represents the MoveTo wizard command.
        /// </summary>
        public static RoutedUICommand MoveTo
        {
            get { return s_MoveToCommand; }
        }

        /// <summary>
        /// Gets the value that represents the Add wizard command.
        /// </summary>
        public static RoutedUICommand Add
        {
            get { return s_AddCommand; }
        }

        /// <summary>
        /// Gets the value that represents the CloseLastError wizard command.
        /// </summary>
        public static RoutedUICommand CloseLastError
        {
            get { return s_CloseLastErrorCommand; }
        }
    }
}
