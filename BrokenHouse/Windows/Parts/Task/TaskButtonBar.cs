using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Parts.Task
{
    
    /// <summary>
    /// This enum is used to identify a button that can be used in the <see cref="TaskButtonBar"/>.
    /// </summary>
    [Flags]
    public enum TaskButton
    {
        /// <summary>
        /// No button
        /// </summary>
        None         = 0,

        /// <summary>
        /// Ok Button
        /// </summary>
        Ok           = 1,

        /// <summary>
        /// Cancel Button
        /// </summary>
        Cancel       = 2,

        /// <summary>
        /// Retry Button
        /// </summary>
        Retry        = 4,

        /// <summary>
        /// Yes Button
        /// </summary>
        Yes          = 8,

        /// <summary>
        /// No Button
        /// </summary>
        No           = 16,

        /// <summary>
        /// Close Button
        /// </summary>
        Close        = 32,

        /// <summary>
        /// Custom Button
        /// </summary>
        Custom       = 64
    }

    /// <summary>
    /// This enum is used to identify a set of buttons on the <see cref="TaskButtonBar"/>.
    /// </summary>
    public enum TaskButtonSet
    {
        /// <summary>
        /// No button is visible
        /// </summary>
        None         = 0,

        /// <summary>
        /// Only an Ok button should be visible.
        /// </summary>
        Ok           = TaskButton.Ok,

        /// <summary>
        /// Only a close button should be visible
        /// </summary>
        Close        = TaskButton.Close,

        /// <summary>
        /// Only a yes button should be visible
        /// </summary>
        Yes          = TaskButton.Yes,

        /// <summary>
        /// Only a cancel button should be visible
        /// </summary>
        Cancel       = TaskButton.Cancel,

        /// <summary>
        /// Only Ok and Cancel buttons should be visible
        /// </summary>
        OkCancel = TaskButton.Cancel | TaskButton.Ok,

        /// <summary>
        /// Only Retry and Cancel buttons should be visible
        /// </summary>
        RetryCancel = TaskButton.Cancel | TaskButton.Retry,

        /// <summary>
        /// Only Yes and No buttons should be visible
        /// </summary>
        YesNo = TaskButton.Yes | TaskButton.No,

        /// <summary>
        /// Only the Yes, No and Cancel buttons should be visible
        /// </summary>
        YesNoCancel = TaskButton.Yes | TaskButton.No | TaskButton.Cancel
    }


    /// <summary>
    /// The task button bar that provies a set of standard buttons sets. 
    /// </summary>
    /// <remarks>
    /// The aim of this control is to display to the the user a set of 
    /// standard buttons by means of the <see cref="ButtonSet"/> propety. Each
    /// button set will choose a button to be the default and cancel button.
    /// The default and cancel buttons can be overidden by supplying a <see cref="TaskButton"/> to
    /// <see cref="DefaultButton"/> and <see cref="CancelButton"/> respectively.
    /// </remarks>
    public class TaskButtonBar : Control
    {
        /// <summary>
        /// The mapping between the name of a button and its enum
        /// </summary>
        private static Dictionary<String, TaskButton>   s_ButtonMap;

        #region --- Dependency objects ---

        /// <summary>
        /// Identifies the <see cref="TaskButtonClick"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TaskButtonClickEvent;

        /// <summary>
        /// Identifies the <see cref="ButtonSet"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonSetProperty;

        /// <summary>
        /// Identifies the <see cref="DefaultButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultButtonProperty;

        /// <summary>
        /// Identifies the <see cref="CancelButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelButtonProperty;

        #endregion

        /// <summary>
        /// Static class to do the WPF Stuff
        /// </summary>
        static TaskButtonBar()
        {
            // Define the events
            TaskButtonClickEvent = EventManager.RegisterRoutedEvent("TaskButtonClick", RoutingStrategy.Bubble, typeof(EventHandler<TaskButtonClickEventArgs>), typeof(TaskButtonBar));
             
            // Define the properties
            ButtonSetProperty     = DependencyProperty.Register("ButtonSet", typeof(TaskButtonSet), typeof(TaskButtonBar), new FrameworkPropertyMetadata(TaskButtonSet.None));
            DefaultButtonProperty = DependencyProperty.Register("DefaultButton", typeof(TaskButton), typeof(TaskButtonBar), new FrameworkPropertyMetadata(TaskButton.None, OnDefaultButtonChangedThunk));
            CancelButtonProperty  = DependencyProperty.Register("CancelButton", typeof(TaskButton), typeof(TaskButtonBar), new FrameworkPropertyMetadata(TaskButton.None, OnDefaultButtonChangedThunk));

            // Register the event handles
            EventManager.RegisterClassHandler(typeof(TaskButtonBar), Button.ClickEvent, new RoutedEventHandler(OnButtonClickedThunk));
            
            // Define the default style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskButtonBar), new FrameworkPropertyMetadata(TaskElements.TaskButtonBarControlStyleKey));
        
            // Override the keyboard navigation
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TaskButtonBar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TaskButtonBar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(TaskButtonBar), new FrameworkPropertyMetadata(false));

            // Define the mapping between name and enum
            s_ButtonMap = new Dictionary<string, TaskButton>  {{"PART_Ok", TaskButton.Ok},       {"PART_Cancel", TaskButton.Cancel},
                                                               {"PART_Retry", TaskButton.Retry}, {"PART_Yes", TaskButton.Yes},
                                                               {"PART_No", TaskButton.No},       {"PART_Close", TaskButton.Close}};
        }
          
        #region --- Public Events ---
        
        /// <summary>
        /// Provide access to the TaskButtonClick Event
        /// </summary>
        /// <remarks>
        /// The arguments of this event identifies which control
        /// has been clicked.
        /// </remarks>
        public event EventHandler<TaskButtonClickEventArgs> TaskButtonClick
        {
            add { AddHandler(TaskButtonClickEvent, value); }
            remove { RemoveHandler(TaskButtonClickEvent, value); }
        }

        #endregion
               
        #region --- Private event handlers ---

        /// <summary>
        /// Ensure the right button in the template has the is default flag
        /// based on the new value of the <see cref="DefaultButton"/>.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnDefaultButtonChanged( TaskButton oldValue, TaskButton newValue )
        {
            updateActiveButtonProperty(Button.IsDefaultProperty, newValue);
        }

        /// <summary>
        /// Ensure the right button in the template has the is IsCancel flag
        /// based on the new value of the <see cref="CancelButton"/>.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnCancelButtonChanged( TaskButton oldValue, TaskButton newValue )
        {
            updateActiveButtonProperty(Button.IsCancelProperty, newValue);
        }

        /// <summary>
        /// Static thunk to handle changes the the DefaultButton property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private static void OnDefaultButtonChangedThunk( DependencyObject source, DependencyPropertyChangedEventArgs args )
        {
            (source as TaskButtonBar).OnDefaultButtonChanged((TaskButton)args.OldValue, (TaskButton)args.NewValue);
        }

        /// <summary>
        /// Static thunk to handle changes the the CancelButton property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private static void OnCancelButtonChangedThunk( DependencyObject source, DependencyPropertyChangedEventArgs args )
        {
            (source as TaskButtonBar).OnCancelButtonChanged((TaskButton)args.OldValue, (TaskButton)args.NewValue);
        }

        /// <summary>
        /// The handler that is called when a button has been pressed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnButtonClickedThunk( object target, RoutedEventArgs args)
        {
            Button        buttonClicked = args.OriginalSource as Button;
            TaskButtonBar buttonBar     = target as TaskButtonBar;
            TaskButton    button        = TaskButton.None;

            if (s_ButtonMap.TryGetValue(buttonClicked.Name, out button))
            {
                buttonBar.RaiseEvent(new TaskButtonClickEventArgs(button) { RoutedEvent = TaskButtonClickEvent });
            }
            else
            {
                throw new InvalidProgramException("Unexpected button was clicked in the TaskButtonBar.");
            }
        }
        
        #endregion
               
        #region --- Private Helpers ---

        /// <summary>
        /// This function set the supplied property to <c>true</c> if it matches a specific button, <c>false</c>
        /// if it does not match. If <see cref="TaskButton.None"/> is supplied then the value for 
        /// the property on all the buttons is set to its default.
        /// </summary>
        /// <remarks>
        /// This function is designed to work with the IsDefaultProperty and the IsCancelProprty.
        /// </remarks>
        /// <param name="property"></param>
        /// <param name="taskButton"></param>
        private void updateActiveButtonProperty( DependencyProperty property, TaskButton taskButton )
        {
            foreach (var entry in s_ButtonMap)
            {
                Button button = GetTemplateChild(entry.Key) as Button;

                // Did we find the button
                if (button != null)
                {
                    if (taskButton != TaskButton.None)
                    {
                        // Set the value
                        button.SetValue(property, (entry.Value == taskButton));
                    }
                    else
                    {
                        // Reset value so that the default is used.
                        button.ClearValue(property);
                    }
                }
            }
        }

        #endregion
               
        #region --- Public Properties ---
        
        /// <summary>
        /// Gets or sets the button set that should be used in the button bar.
        /// </summary>
        public TaskButtonSet ButtonSet
        {
            get { return (TaskButtonSet)GetValue(ButtonSetProperty); }
            set { SetValue(ButtonSetProperty, value); }
        }        

        /// <summary>
        /// Gets or sets the button that should be used for the default action
        /// </summary>
        /// <remarks>
        /// Setting a default button value will override the default button defined in the current
        /// <see cref="ButtonSet"/>.
        /// </remarks>
        public TaskButton DefaultButton
        {
            get { return (TaskButton)GetValue(DefaultButtonProperty); }
            set { SetValue(DefaultButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets the button that should be used for the cancel action
        /// </summary>
        /// <remarks>
        /// Setting a default button value will override the default button defined in the current
        /// <see cref="ButtonSet"/>.
        /// </remarks>
        public TaskButton CancelButton
        {
            get { return (TaskButton)GetValue(CancelButtonProperty); }
            set { SetValue(CancelButtonProperty, value); }
        }

        #endregion
    }
}
