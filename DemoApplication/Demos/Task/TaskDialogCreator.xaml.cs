using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Parts.Task;
using DemoApplication.Demos;

namespace DemoApplication.Demos.Task
{
    /// <summary>
    /// Helper class to hold information about a command link
    /// </summary>
    public class CommandLinkInfo
    {
        public string        Content     { get; set; }
        public string        Instruction { get; set; }
        public StockIconInfo IconInfo    { get; set; }
    }

    /// <summary>
    /// Interaction logic for TaskDialogCreator.xaml
    /// </summary>
    public partial class TaskDialogCreator : Window, INotifyPropertyChanged
    {
        private CommandLinkInfo m_SelectedCommandLink;

        /// <summary>
        /// Initialise the WPF component
        /// </summary>
        public TaskDialogCreator()
        {
            // Iniitlaise the key properties
            MessageText         = "Message";
            Instruction         = "Instruction";
            MainIconInfo        = DataProvider.StockIconInfos.Where(i => i.Type == "TaskIcons.Error").FirstOrDefault();
            FooterIconInfo      = DataProvider.StockIconInfos.Where(i => i.Type == "TaskIcons.Warning").FirstOrDefault();
            ExpandedText        = "Hide details";
            CollapsedText       = "Show details";
            CheckBoxText        = "Show this message again";
            CheckBoxState       = true;
            ButtonSet           = TaskButtonSet.Ok;
            ShowMessageInFooter = false;
            AutoExpandMessage   = true;
            ContentText         = "Content";
            AutoExpandContent   = true;
            DialogTitle         = "Title";
            CommandLinks        = new ObservableCollection<CommandLinkInfo>();

            // Set the context for the window
            DataContext = this;

            // Do the XAML stuff
            InitializeComponent();
        }

        public string                                ContentText              { get; set; }
        public bool                                  AutoExpandContent        { get; set; } 
        public string                                MessageText              { get; set; }
        public bool                                  ShowMessageInFooter      { get; set; }
        public string                                Instruction              { get; set; }
        public StockIconInfo                         MainIconInfo             { get; set; }
        public StockIconInfo                         FooterIconInfo           { get; set; }
        public string                                FooterText               { get; set; }
        public string                                ExpandedText             { get; set; }
        public bool                                  IsExpanded               { get; set; }
        public string                                CollapsedText            { get; set; }
        public string                                CheckBoxText             { get; set; }
        public bool                                  CheckBoxState            { get; set; }
        public bool                                  AutoExpandMessage        { get; set; }
        public string                                DialogTitle              { get; set; }
        public TaskButtonSet                         ButtonSet                { get; set; }
        public ObservableCollection<CommandLinkInfo> CommandLinks             { get; set; }

        /// <summary>
        /// Provide details of the selected command link
        /// </summary>
        public CommandLinkInfo                       SelectedCommandLink 
        { 
            get { return m_SelectedCommandLink; }
            set
            {
                m_SelectedCommandLink = value;

                OnPropertyChanged("CanRemoveLink");
            }
        }

        /// <summary>
        /// A flag to indicate that we can remove the link
        /// </summary>
        public bool CanRemoveLink 
        { 
            get { return (m_SelectedCommandLink != null); }
        }    
    
        /// <summary>
        /// A flag to indicate whether we can add a command link
        /// </summary>
        public bool CanAddLink 
        { 
            get { return (CommandLinks.Count < 3); }
        }

        private string EscapeString( string input )
        {
            return (input == null)? "null" : "\"" + input.Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";
        }

        private void OnGenerateCode( object sender, RoutedEventArgs e ) 
        {
            StringWriter writer     = new StringWriter();
            bool         hasContent = !string.IsNullOrEmpty(ContentText);

            writer.WriteLine("TaskDialog taskDialog   = new TaskDialog()");
            if (hasContent)
            {
                writer.WriteLine("TextBlock  taskContent = new TextBlock {{ Text = {0} }};", EscapeString(ContentText));
                writer.WriteLine();
                writer.WriteLine("TaskDialogControl.SetExpand(taskContent, {0});", AutoExpandContent);
                writer.WriteLine("taskDialog.AddElement(taskContent, new GridLength(0.0, GridUnitType.Auto));");
            }
            writer.WriteLine();

            if (AutoExpandMessage != (bool)TaskDialogControl.AutoExpandMessageProperty.DefaultMetadata.DefaultValue)
            {
                writer.WriteLine("taskDialog.AutoExpandMessage   = {0};", AutoExpandMessage);
            }
            if (CheckBoxState != (bool)TaskDialogControl.CheckBoxStateProperty.DefaultMetadata.DefaultValue)
            {
                writer.WriteLine("taskDialog.CheckBoxState       = {0};", CheckBoxState);
            }
            if (IsExpanded != (bool)TaskDialogControl.IsExpandedProperty.DefaultMetadata.DefaultValue)
            {
                writer.WriteLine("taskDialog.IsExpanded          = {0};", IsExpanded);
            }
            if (ShowMessageInFooter != (bool)TaskDialogControl.ShowMessageInFooterProperty.DefaultMetadata.DefaultValue)
            {
                writer.WriteLine("taskDialog.ShowMessageInFooter = {0};", ShowMessageInFooter);
            }
            if (CheckBoxText != null)
            {
                writer.WriteLine("taskDialog.CheckBoxText        = {0};", EscapeString(CheckBoxText));
            }
            if (CollapsedText != null)
            {
                writer.WriteLine("taskDialog.CollapsedText       = {0};", EscapeString(CollapsedText));
            }
            if (ExpandedText != null)
            {
                writer.WriteLine("taskDialog.ExpandedText        = {0};", EscapeString(ExpandedText));
            }
            if (FooterText != null)
            {
                writer.WriteLine("taskDialog.FooterText          = {0};", EscapeString(FooterText));
                writer.WriteLine("taskDialog.FooterIconSource    = BrokenHouse.Windows.Controls.{0};", FooterIconInfo.Type);
            }
            writer.WriteLine("taskDialog.Message             = {0};", EscapeString(MessageText));
            writer.WriteLine("taskDialog.Instruction         = {0};", EscapeString(Instruction));
            writer.WriteLine("taskDialog.Title               = {0};", EscapeString(DialogTitle));
            writer.WriteLine("taskDialog.MainIconSource      = BrokenHouse.Windows.Controls.{0};", MainIconInfo.Type);
            writer.WriteLine();
          
            // Add the command links
            if (CommandLinks.Count > 0)
            {
                foreach ( CommandLinkInfo info in CommandLinks )
                {
                    writer.WriteLine("taskDialog.AddCommandLink({0}, {1}, BrokenHouse.Windows.Controls.{2}, {0}, false, false);",
                                        EscapeString(info.Instruction), EscapeString(info.Content), info.IconInfo.Type, 
                                        EscapeString(info.Instruction));
                }
                writer.WriteLine();
            }
    
            // Show the dialog
            writer.WriteLine("taskDialog.ShowDialog();");

            // Finished
            writer.Close();

            Clipboard.SetText(writer.ToString());
        }

        /// <summary>
        /// Show dialog button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowDialogClicked( object sender, RoutedEventArgs e )
        {
            TaskDialog dialog = new TaskDialog(this);

            // Use our properties to define the dialog
            dialog.AutoExpandMessage = AutoExpandMessage;
            dialog.CheckBoxState = CheckBoxState;
            dialog.IsExpanded = IsExpanded;
            dialog.ShowMessageInFooter = ShowMessageInFooter;
            dialog.CheckBoxText = CheckBoxText;
            dialog.TaskButtonSet = ButtonSet;
            dialog.CollapsedText = CollapsedText;
            dialog.ExpandedText = ExpandedText;
            dialog.FooterText = FooterText;
            dialog.Message = MessageText;
            dialog.Instruction = Instruction;
            dialog.Title = DialogTitle;
            dialog.FooterIconSource = FooterIconInfo.BitmapSource;
            dialog.MainIconSource = MainIconInfo.BitmapSource;
            
            // Add the content text if defined
            if (!string.IsNullOrEmpty(ContentText))
            {
                TextBlock content = new TextBlock { Text = ContentText };

                TaskDialogControl.SetExpand(content, AutoExpandContent);

                dialog.AddElement(content, new GridLength(0.0, GridUnitType.Auto));
            }

            // Add the command links
            foreach ( CommandLinkInfo info in CommandLinks )
            {
                dialog.AddCommandLink(info.Instruction, info.Content, info.IconInfo.BitmapSource, info.Instruction, false, false);
            }
    
            // Show the dialog
            dialog.ShowDialog();

            // Extract the properties that may have changed
            IsExpanded = dialog.IsExpanded;
            CheckBoxState = dialog.CheckBoxState;
                                                 
        }

        /// <summary>
        /// The add button was clicked - add a info holder to the displayed links
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddClicked(object sender, RoutedEventArgs e)
        {
            CommandLinks.Add(new CommandLinkInfo { Content = "Content", 
                                                   Instruction = "Action", 
                                                   IconInfo = DataProvider.StockIconInfos.Where(i => i.Type == "TaskIcons.Arrow").FirstOrDefault() });

            OnPropertyChanged("CanAddLink");
            OnPropertyChanged("CanRemoveLink");
        }

        /// <summary>
        /// Remove a command link from the list of command links
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveClicked(object sender, RoutedEventArgs e)
        {
            CommandLinks.Remove(m_SelectedCommandLink);

            OnPropertyChanged("CanAddLink");
            OnPropertyChanged("CanRemoveLink");
        }


        #region --- INotifyPropertyChanged Implementation ---

        /// <summary>
        /// Event that is triggered when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Trigger the property changed event handler
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged( string propertyName )
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
