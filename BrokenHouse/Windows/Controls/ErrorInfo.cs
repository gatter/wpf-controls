using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Interop;

namespace BrokenHouse.Windows.Controls
{
    /// <summary>
    /// Represents a control that is used to display a dismissable error message.
    /// </summary>
    /// <remarks>
    /// This control, when supplied with a <see cref="CloseCommand"/> will show a button,
    /// or in the case of the Classic style the control itself will be a button. When this button
    /// is clicked the supplied <see cref="CloseCommand"/> will be triggerd. By doing this the 
    /// parent control can control how the <see cref="ErrorInfo"/> is dismissed.
    /// </remarks>
    public class ErrorInfo : ContentControl
    {
        #region --- Dependency Properties ---

        /// <summary>
        /// Identifies the <see cref="ErrorInfo.CloseCommand">CloseCommand</see> property. 
        /// </summary>
        public static readonly DependencyProperty CloseCommandProperty;

        #endregion    
   
        #region --- Constructor ---

        /// <summary>
        /// The static constructor to register the WPF stuff
        /// </summary>
        static ErrorInfo()
        {
            // Define the events
            CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(RoutedCommand), typeof(ErrorInfo), new FrameworkPropertyMetadata(null));
            
            // Override the metadata
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ErrorInfo), new FrameworkPropertyMetadata(typeof(ErrorInfo)));
        }
   
        #endregion    
    
        #region --- Public Events and Properties ---

        /// <summary>
        /// Gets and sets the command that is triggered in repsense to the user clicking the close button.
        /// </summary>
        public RoutedCommand CloseCommand
        {
            get { return (RoutedCommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        #endregion
    }

}
