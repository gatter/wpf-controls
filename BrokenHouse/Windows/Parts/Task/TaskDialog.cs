using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using BrokenHouse.Windows.Interop;
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Parts.Task
{
    /// <summary>
    /// This is a helper class to create task dialogs by using the <see cref="TaskDialogControl"/> framework element.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="TaskDialogControl"/> framework element defines how the content of the task dialog should
    /// displayed. This class uses <see cref="TaskDialogControl"/> within in a <see cref="System.Windows.Window"/> to provide 
    /// a mechanism to create the majority of task dialogs that may be required by an application. 
    /// </para>
    /// <para>
    /// Essentially, any task dialog that consists of a simple message or a couple of 
    /// <see cref="BrokenHouse.Windows.Controls.CommandLink">CommandLinks</see> can be created using this class.
    /// </para>
    /// </remarks>
    /// <example>
    /// This example shows how to create a simple task dialog that could be used to confirm the closing of a document window
    /// <code>
    /// TaskDialog dialog = new TaskDialog(this);
    ///
    /// dialog.Title           = "Closing Window";
    /// dialog.Instruction     = string.Format("Changes have been made to '{0}'", Window.Title);
    /// dialog.MainIconSource  = TaskIcons.Question;
    /// dialog.AddCommandLink("Close without Saving", "All changes made will be lost", "Close", false, false);
    /// dialog.AddCommandLink("Save and then close", "Changes will be sent to the database before closing", "Save", true, false);
    /// dialog.AddCommandLink("Cancel", "Cancel the close and keep the file open", "Cancel", false, true);
    ///
    /// string command = dialog.ShowDialog();
    /// </code>
    /// This next example shows how to create a simple task dialog to represent a more complex sample. In this example the ContentDetail
    /// element is intended to represent a control that will be expanded and collapsed.
    /// <code>
    /// TaskDialog    dialog  = new TaskDialog(this);
    /// ContentDetail content = new ContentDetail();
    ///
    /// dialog.Title           = "Task Dialog";
    /// dialog.Instruction     = "Complex example";
    /// dialog.Message         = "This example shows how to use a custom control in a task dialog";
    /// dialog.MainIconSource  = TaskIcons.Information;
    /// dialog.ButtonSet       = ButtonSet.Close;
    /// dialog.Expanded        = false;
    /// dialog.CheckBoxText    = "Show this message again";
    /// dialog.CheckBoxState   = true;
    /// 
    /// // Add an element and save the index into the content list
    /// dialog.AddElement(content);
    /// 
    /// // Make the content detail expandable
    /// dialog.SetExpandable(content, true);
    ///
    /// // Show the dialog
    /// string command = dialog.ShowDialog();
    /// bool   state   = dialog.CheckBoxState;
    /// </code>
    /// </example>
    public class TaskDialog
    {
        private Window              m_Window;
        private TaskDialogControl   m_Control;
        private Grid                m_Content;
        private TaskButtonBar       m_ButtonBar;
        private Icon                m_MainIcon;
        private TextBlock           m_FooterText;
        private Icon                m_FooterIcon;
        private TextBlock           m_CheckBoxText;
        private TaskDialogResult    m_DialogResult;
        private Button              m_CustomDefaultButton;
        private Button              m_CustomCancelButton;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <remarks>
        /// The owner of the dialog is defined as the main window of the application.
        /// </remarks>
        public TaskDialog() : this(Application.Current.MainWindow)
        {
        }

        /// <summary>
        /// Construct the <c>TaskDialog</c> and associate the dialog with the window containing the <c>owner</c>
        /// framework element.
        /// </summary>
        /// <remarks>
        /// The window with which the dialog will be associated is derived by finding the root visual of the supplied element
        /// </remarks>
        /// <param name="owner"></param>
        public TaskDialog( FrameworkElement owner ) : this(owner.FindVisualAncestor<Window>())
        {
        }

        /// <summary>
        /// Constructor the <c>TaskDialog</c> and associate it with the supplied window
        /// </summary>
        /// <param name="owner"></param>
        public TaskDialog( Window owner )
        {
            // The window that hols the control
            m_Window  = new Window { Title                 = "Task Dialog Window", 
                                     Height                = 303,
                                     Width                 = 500,
                                     Icon                  = (owner != null)? owner.Icon : null,
                                     SizeToContent         = SizeToContent.Height,
                                     WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                     ShowInTaskbar         = false,
                                     ResizeMode            = ResizeMode.NoResize,
                                     Topmost               = true };
            
            // The dialog control
            m_Control = new TaskDialogControl() { AllowExpand = false, IsExpanded = false };

            // Create the extra controls
            m_MainIcon      = new Icon  { Source = TaskIcons.Information, Width = 32, Height = 32, VerticalAlignment = VerticalAlignment.Center };
            m_FooterIcon    = new Icon  { Source = TaskIcons.Information, Width = 16, Height = 16 };
            m_FooterText    = new TextBlock { Text     = "", TextWrapping = TextWrapping.Wrap };
            m_CheckBoxText  = new TextBlock { Text     = "", TextWrapping = TextWrapping.Wrap };
            m_ButtonBar     = new TaskButtonBar();
            m_Content       = new Grid { Margin = new Thickness(0, 2, 0, 2) };
            
            // Define the static parts of the control
            m_Control.MainIcon = m_MainIcon;
            m_Control.Content  = m_Content;

            // Set the content of the window
            m_Window.Content = m_Control;

            // Define the content
            m_Content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) } );

            // We want to know when a button was pressed
            m_ButtonBar.TaskButtonClick += new EventHandler<TaskButtonClickEventArgs>(OnTaskButtonClick);

            // We want to adjust the window when it is shown
            NativeWindowStyles.SetCanMaximize(m_Window, false);
            NativeWindowStyles.SetCanMinimize(m_Window, false);
            NativeWindowStyles.SetIsSystemMenuVisible(m_Window, false);
        }


        #region --- Private Event Handlers ---
        
        /// <summary>
        /// A button in the button bar has been clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnTaskButtonClick( object sender, TaskButtonClickEventArgs args )
        {
            // Set the button clicked
            m_DialogResult = new TaskDialogResult(args.Button);

            // Close the dialog
            m_Window.Close();
        }

        /// <summary>
        /// A button has been clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            // Determine the button clicked value
            if (sender is FrameworkElement)
            {
                m_DialogResult = new TaskDialogResult((sender as FrameworkElement).Name);
            }
            else
            {
                m_DialogResult = new TaskDialogResult("");
            }

            // Close the dialog
            m_Window.Close();
        }

        #endregion

        #region --- Public Properties ---

        /// <summary>
        /// Gets or sets the button set that is used in the task dialog
        /// </summary>
        /// <remarks>
        /// The buttons in the <c>TaskDialog</c> can be one of a number of button sets. If 
        /// the property is set to <see cref="BrokenHouse.Windows.Parts.Task.TaskButtonSet.None"/> then the button bar is removed
        /// completly.
        /// </remarks>
        public TaskButtonSet TaskButtonSet
        {
            get { return m_ButtonBar.ButtonSet; }
            set 
            { 
                // Set the button set to useg
                m_ButtonBar.ButtonSet = value;

                // Add or remove the button bar
                m_Control.Buttons = (value == TaskButtonSet.None)? null : m_ButtonBar;
            }
        }

        /// <summary>
        /// Gets or sets the task button that should be used as the default button. 
        /// </summary>
        /// <remarks>
        /// Setting this value a value other than <see cref="TaskButton.None"/> will
        /// override the standard default button based on the current <see cref="TaskButtonSet"/>.
        /// </remarks>
        public TaskButton DefaultTaskButton
        {
            get { return m_ButtonBar.DefaultButton; }
            set { m_ButtonBar.DefaultButton = value; }
        }

        /// <summary>
        /// Gets or sets the task button that should be used as the cancel button. 
        /// </summary>
        /// <remarks>
        /// Setting this value a value other than <see cref="TaskButton.None"/> will
        /// override the standard cancel button based on the current <see cref="TaskButtonSet"/>.
        /// </remarks>
        public TaskButton CancelTaskButton
        {
            get { return m_ButtonBar.DefaultButton; }
            set { m_ButtonBar.DefaultButton = value; }
        }
        
        /// <summary>
        /// Gets or sets the instruction associated with this task control. The instruction
        /// appears in bold at the top of the control
        /// </summary>
        public string Instruction
        {
            get { return (string)m_Control.Instruction; }
            set { m_Control.Instruction = value; }
        } 
        
        /// <summary>
        /// Gets or sets the message associated with this task control. The message appears
        /// below the instruction text.
        /// </summary>
        public string Message
        {
            get { return (string)m_Control.Message; }
            set { m_Control.Message = value; }
        } 
        
        /// <summary>
        /// Gets or sets whether the message is shown in the footer or not. 
        /// </summary>
        public bool ShowMessageInFooter
        {
            get { return m_Control.ShowMessageInFooter; }
            set { m_Control.ShowMessageInFooter = value; }
        }

        /// <summary>
        /// Gets or sets whether the message is should auto expand with the expand button of the dialog.
        /// </summary>
        /// <remarks>
        /// When the message is set to auto expand the height of the dialog
        /// will expand and contract by the size of the message.
        /// </remarks>
        public bool AutoExpandMessage
        {
            get { return m_Control.AutoExpandMessage; }
            set { m_Control.AutoExpandMessage = value; }
        }   
   
        /// <summary>
        /// Gets or sets the text used in the title of the dialog
        /// </summary>
        public string Title
        {
            get { return m_Window.Title; }
            set { m_Window.Title = value; }
        } 
       
        /// <summary>
        /// Gets or sets the check box text. The check box appears towards in the bottom
        /// section of the control.
        /// </summary>
        public string CheckBoxText
        {
            get { return m_CheckBoxText.Text; }
            set 
            {
                bool isEmpty = string.IsNullOrEmpty(value);

                // Save the text
                m_CheckBoxText.Text = value; 

                // update the control
                m_Control.CheckBoxContent = isEmpty? null : m_CheckBoxText;
            }
        }

        /// <summary>
        /// Gets of sets the state of the check box.
        /// </summary>
        public bool CheckBoxState
        {
            get { return m_Control.CheckBoxState; }
            set { m_Control.CheckBoxState = value; }
        }
        
        /// <summary>
        /// Gets of sets main bitmap used in the dialog. The icon
        /// appears to the left of the instruction and message text.
        /// </summary>
        public ImageSource MainIconSource
        {
            get { return m_MainIcon.Source; }
            set { m_MainIcon.Source = value; }
        }

        /// <summary>
        /// Gets or sets the text that appears in the expander when the control is expanded
        /// </summary>
        public string ExpandedText
        {
            get { return m_Control.ExpandedText; }
            set 
            { 
                m_Control.ExpandedText = value;
                m_Control.AllowExpand = !(string.IsNullOrEmpty(ExpandedText) || string.IsNullOrEmpty(CollapsedText));
            }
        }

        /// <summary>
        /// Gets or sets the text that appears in the expander when the control is collapsed
        /// </summary>
        public string CollapsedText
        {
            get { return m_Control.CollapsedText; }
            set 
            { 
                m_Control.CollapsedText = value; 
                m_Control.AllowExpand = !(string.IsNullOrEmpty(ExpandedText) || string.IsNullOrEmpty(CollapsedText));
            }
        }

        /// <summary>
        /// Gets the name of the button that was clicked to dismiss the dialog
        /// </summary>
        public TaskDialogResult DialogResult
        {
            get { return m_DialogResult; }
        }
        
        /// <summary>
        /// Gets or sets the expanded state of the dialog.
        /// </summary>
        public bool IsExpanded
        {
            get { return m_Control.IsExpanded; }
            set { m_Control.IsExpanded = value; }
        }

        /// <summary>
        /// Gets or sets the type of image used in the footer of the dialog.
        /// </summary>
        public ImageSource FooterIconSource
        {
            get { return m_FooterIcon.Source; }
            set { m_FooterIcon.Source = value; }    
        }

        /// <summary>
        /// Gets or sets the text used in the footer of the dialog.
        /// </summary>
        public string FooterText
        {
            get { return m_FooterText.Text; }
            set 
            {
                bool isEmpty = string.IsNullOrEmpty(value);

                // Save the text
                m_FooterText.Text = value; 

                // update the control
                m_Control.FooterContent = isEmpty? null : m_FooterText;
                m_Control.FooterIcon    = isEmpty? null : m_FooterIcon;

            }
        }

        #endregion

        #region --- Public Interface ---

        /// <summary>
        /// Add a framework element to the control.
        /// </summary>
        /// <remarks>
        /// The control is added to the content section of the dialog. This content section
        /// is managed by a  grid <see cref="System.Windows.Controls.Grid"/>.
        /// </remarks>
        /// <param name="element">The element to add to the dialog</param>
        /// <param name="height">The height to assign to the row containing the element. </param>
        /// <returns>The index of the control in the content area</returns>
        public int AddElement( FrameworkElement element, GridLength height )
        {
            int index = m_Content.Children.Count;

            // Add the command link to the content
            m_Content.Children.Add(element);

            // Set the row
            Grid.SetRow(element, index);

            // Add the row definitioin
            m_Content.RowDefinitions.Add(new RowDefinition { Height = height } );
        
            // Return the index of the element
            return index;
        }

        /// <summary>
        /// Add a framework element to the control.
        /// </summary>
        /// <remarks>
        /// The control is added to the content section of the dialog. This content section
        /// is managed by a grid <see cref="System.Windows.Controls.Grid"/> and the element
        /// will be added to it with a height of 1*;
        /// </remarks>
        /// <param name="element">The element to add to the dialog</param>
        /// <returns>The index of the control in the content area</returns>
        public int AddElement( FrameworkElement element )
        {
            return AddElement(element, new GridLength(1.0, GridUnitType.Star));
        }

        /// <summary>
        /// Add a button to the content section of the dialog
        /// </summary>
        /// <remarks>
        /// The button is added to the content section of the dialog. The content section
        /// is managed by a uniform grid <see cref="System.Windows.Controls.Primitives.UniformGrid"/> which
        /// means that all the controls will be of a uniform height.
        /// </remarks>
        /// <param name="button">The button to add to the dialog</param>
        /// <returns>The index of the control in the content area</returns>
        public int AddButton( Button button )
        {
            // Add the click so we know what has happened
            button.Click += new RoutedEventHandler(OnButtonClicked);

            // is the button a default button?
            if (button.IsDefault)
            {
                if (m_CustomDefaultButton != null)
                {
                    throw new InvalidOperationException("A custom button has already been defined as the default button.");
                }
                m_CustomDefaultButton = button;
                m_ButtonBar.DefaultButton = TaskButton.Custom;
            }

            // is the button as cancel button
            if (button.IsCancel)
            {
                if (m_CustomCancelButton != null)
                {
                    throw new InvalidOperationException("A custom button has already been defined as the cancel button.");
                }
                m_CustomCancelButton = button;
                m_ButtonBar.CancelButton = TaskButton.Custom;
            }

            // Add the element
            return AddElement(button);
        }

        /// <summary>
        /// Helper function to add a command link to the content of the dialog.
        /// </summary>
        /// <remarks>
        /// The command link is added to the content section of the dialog. The content section
        /// is managed by a uniform grid <see cref="System.Windows.Controls.Primitives.UniformGrid"/> which
        /// means that all the controls will be of a uniform height.
        /// </remarks>
        /// <param name="instruction">The instruction text the is in a large font</param>
        /// <param name="message">The message that appears under the instruction</param>
        /// <param name="icon">The icon to be used in the command link.</param>
        /// <param name="name">The name of this control</param>
        /// <param name="isDefault">True if this is the default button</param>
        /// <param name="isCancel">True if the  button cancels the dialog</param>
        /// <returns>The index of the control in the content area</returns>
        public int AddCommandLink( string instruction, string message, BitmapSource icon, string name, bool isDefault, bool isCancel )
        {
            CommandLink commandLink = new CommandLink { Name = name,
                                                        Instruction = instruction,
                                                        Content = message,
                                                        Icon = (icon == null)? TaskIcons.Arrow : icon,
                                                        Margin = new Thickness(6.0),
                                                        IsDefault = isDefault,
                                                        IsCancel = isCancel,
                                                        SnapsToDevicePixels = true };

            // Add the button
            return AddButton(commandLink);
        }

        /// <summary>
        /// Gets or sets which controls are to be collapsed/expanded with the expand button. 
        /// </summary>
        /// <param name="element">The element that is to be defined as expandable</param>
        /// <param name="flag">True if the control is to be expandable</param>
        public static void SetExpandable( FrameworkElement element, bool flag )
        {
            if (element != null)
            {
                TaskDialogControl.SetExpand(element, flag);
            }
        }

        /// <summary>
        /// Show the dialog
        /// </summary>
        /// <returns>The name of the button that was clicked</returns>
        public TaskDialogResult ShowDialog()
        {
            // Set the result
            m_DialogResult = new TaskDialogResult();

            // Show the dialog
            m_Window.ShowDialog();

            // Return the result
            return m_DialogResult;
        }

        #endregion
        
    }
}
