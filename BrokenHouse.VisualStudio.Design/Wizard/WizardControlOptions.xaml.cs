using System;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Parts.Wizard;
using BrokenHouse.Windows.Parts.Task;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design;
    
namespace BrokenHouse.Windows.VisualStudio.Design.Wizard
{
    /// <summary>
    /// Interaction logic for WizardControlOptions.xaml
    /// </summary>
    internal partial class WizardControlOptions : UserControl, INotifyPropertyChanged
    {
        public bool                         IsAeroWizard       { get; private set; }
        public double                       ThumbnailWidth     { get; private set; }
        public double                       ThumbnailHeight    { get; private set; }
        public EditingContext               EditingContext     { get; private set; }
        public ModelItem                    EditingItem        { get; private set; }
        public bool                         IsActive           { get; private set; }

        #region --- Constructor ---

        /// <summary>
        /// Default Constructorr
        /// </summary>
        public WizardControlOptions()
        {
            // Force the data context
            DataContext = this;

            // Do the XAML stuff
            InitializeComponent();

            // We are initially hidden
            Visibility = Visibility.Collapsed;     
        }

        #endregion

        #region --- Private Helpers ---

        /// <summary>
        /// Update the bounds of all the pages (except the active one)
        /// </summary>
        private void UpdateBounds()
        {
            // Work out the page rect based on the coordinates of the control
            WizardControl    control    = (EditingItem.View as WizardControl);
            WizardPage       activePage = (ActivePageItem == null)? null : (ActivePageItem.View as WizardPage);
            FrameworkElement presenter  = control.FindVisualChild("PART_ActiveContentHost") as FrameworkElement;
            Size             pageSize   = presenter.RenderSize;

            // Define the page bounds
            Rect pageBounds = presenter.TransformToAncestor(control).TransformBounds(new Rect(0, 0, pageSize.Width, pageSize.Height));

            // Loop over all pages setting their dimensions
            foreach (var page in PageItemCollection.Select(i => i.View).OfType<WizardPage>().Where(p => p != activePage))
            {
                page.Measure(pageBounds.Size);
                page.Arrange(pageBounds);
            }

            // Update the thumbnails
            ThumbnailHeight = 100.0;
            ThumbnailWidth  = (ThumbnailHeight / pageSize.Height) * pageSize.Width;

            // We do not want the width > 200
            if (ThumbnailWidth > 200.0)
            {
                ThumbnailHeight *= 200.0 / ThumbnailWidth;
                ThumbnailWidth = 200.0;
            }

            // Trigger the property changes
            OnPropertyChanged("ThumbnailHeight");
            OnPropertyChanged("ThumbnailWidth");
        }

        #endregion

        #region --- Public Interface ---

        /// <summary>
        /// Activate the options
        /// </summary>
        /// <param name="context"></param>
        /// <param name="editingItem">the wizard control item that we are editing.</param>
        public void Activate( EditingContext context, ModelItem editingItem, Point offset )
        {
            WizardControl wizardControl = editingItem.View as WizardControl;

            // Initialise some properties
            EditingContext     = context;
            EditingItem        = editingItem;
            IsAeroWizard       = wizardControl is AeroWizardControl;

            // Initialise the position of this options control
            Margin = new Thickness(offset.X, offset.Y, 0, 0);
            MaxHeight = MinHeight = wizardControl.ActualHeight;

            // Work out the page rect based on the coordinates of the control
            UpdateBounds();

            // We want to know when the pages change
            PageItemCollection.CollectionChanged += OnPageItemCollectionChanged;

            // We want to know change is size
            wizardControl.SizeChanged += OnControlSizeChanged;

            // Trigger the property changes
            OnPropertyChanged("PageCount");
            OnPropertyChanged("ActivePageIndex");
            OnPropertyChanged("ActivePageItem");
            OnPropertyChanged("PageItemCollection");
            OnPropertyChanged("Summary");
            OnPropertyChanged("CanBringToFront");
            OnPropertyChanged("CanSendToBack");
            OnPropertyChanged("IsAeroWizard");

            // We are now visible and active
            Visibility = Visibility.Visible;
            IsActive   = true;

            // Enable the commands
            CommandManager.InvalidateRequerySuggested();
        }

        
        /// <summary>
        /// Deactivate the options
        /// </summary>
        /// <param name="context"></param>
        /// <param name="editingItem">the wizard control item that we are editing.</param>
        public void Deactivate()
        {
            // Detatch from the control
            if (EditingItem != null)
            {
                WizardControl wizardControl = EditingItem.View as WizardControl;

                // Disconnect from the page collection
                PageItemCollection.CollectionChanged -= OnPageItemCollectionChanged;
     
                // PageItemCollection from the size changing
                wizardControl.SizeChanged += OnControlSizeChanged;
            }

            // We are no longer visible - we do this so that we are not
            // shown on items that are not valid 
            // i.e. Wizardpages that are not children of wizard pages.
            Visibility = Visibility.Collapsed;       

            // Nolonger active
            IsActive = false;
        }
        
        #endregion
                             
        #region --- Private Event Handler ---
 
        /// <summary>
        /// The control has changed size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnControlSizeChanged( object sender, SizeChangedEventArgs e )
        {
            MaxHeight = MinHeight = e.NewSize.Height;

            UpdateBounds();
        }

        /// <summary>
        /// The page collection has changed - ensure that things are up to date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPageItemCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            // Check the bounds
            UpdateBounds();
            
            // Trigger a notification because the active page may have changed
            OnPropertyChanged("ActivePageItem");
            OnPropertyChanged("ActivePageIndex");
            OnPropertyChanged("PageCount");
            OnPropertyChanged("Summary");
        }
        
        #endregion
                              
        #region --- Public Properties ---
               
        /// <summary>
        /// Get the pages collection
        /// </summary>
        public int PageCount      
        { 
            get 
            {
                return (EditingItem == null)? 0 : PageItemCollection.Count;
            } 
        }
       
        /// <summary>
        /// Get the pages collection
        /// </summary>
        public ModelItemCollection PageItemCollection        
        { 
            get 
            {
                return (EditingItem == null)? null : EditingItem.Properties["Items"].Value as ModelItemCollection;
            } 
        }

        /// <summary>
        /// Gets the summary for the control
        /// </summary>
        public string Summary 
        { 
            get 
            { 
                return (EditingItem == null)? "" : string.Format("{0}/{1}", ActivePageIndex + 1, PageItemCollection.Count); 
            } 
        }

        /// <summary>
        /// Get the active index
        /// </summary>
        public int ActivePageIndex        
        { 
            get 
            {
                return (EditingItem == null)? -1 : (int)EditingItem.Properties["ActiveIndex"].ComputedValue;
            } 
        }

        /// <summary>
        /// Get & Set the active page item
        /// </summary>
        public ModelItem ActivePageItem        
        { 
            get 
            {
                return (EditingItem == null)? null : EditingItem.Properties["ActivePage"].Value;
            } 
            set
            {
                if (value != null)
                {
                    // Set the selection
                    EditingContext.Items.SetValue(new Selection(new ModelItem[] { value }));

                    // Ensure that the commands are updated
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        #endregion

        #region --- INotifyPropertyChanged Members ---

        /// <summary>
        /// The property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Trigger the property changed event
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged( string propertyName )
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region --- Command Handlers ---
        
        /// <summary>
        /// Can the new content page be executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!IsActive)
            {
               e.CanExecute = false;
            }
            else if (e.Command == WizardControlOptionsCommands.NewAeroPage)
            {
                e.CanExecute = IsAeroWizard;
            }
            else if ((e.Command == WizardControlOptionsCommands.NewContentPage) || (e.Command == WizardControlOptionsCommands.NewTitlePage))
            {
                e.CanExecute = !IsAeroWizard;
            }
            else if (e.Command == WizardControlOptionsCommands.DeletePage)
            {
                e.CanExecute = (PageItemCollection.Count > 0);
            }
            else if (e.Command == WizardControlOptionsCommands.MovePageBackwards)
            {
                e.CanExecute = (ActivePageIndex != 0);
            }
            else if (e.Command == WizardControlOptionsCommands.MovePageForwards)
            {
                e.CanExecute = (ActivePageIndex != (PageCount - 1));
            }
            else
            {
            }
        }
           
        /// <summary>
        /// Add a new aero page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewAreoPage( object sender, ExecutedRoutedEventArgs e )
        {
            ModelItem wizardPageItem = ModelFactory.CreateItem(EditingContext, typeof(AeroWizardPage), CreateOptions.InitializeDefaults, new object[0]);

            // Insert the page
            PageItemCollection.Insert(ActivePageIndex, wizardPageItem);

            // Update the active index
            ActivePageItem = wizardPageItem;

            // Event Complete
            e.Handled = true;        
        }
 
        /// <summary>
        /// Add a new content page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewContentPage( object sender, ExecutedRoutedEventArgs e )
        {   
            ModelItem wizardPageItem = ModelFactory.CreateItem(EditingContext, typeof(ClassicWizardContentPage), CreateOptions.InitializeDefaults, new object[0]);

            // Insert the page
            PageItemCollection.Insert(ActivePageIndex, wizardPageItem);

            // Update the active index
            ActivePageItem = wizardPageItem;

            // Event Complete
            e.Handled = true;        
        }

        /// <summary>
        /// Add a new title page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewTitlePage( object sender, ExecutedRoutedEventArgs e )
        {   
            ModelItem wizardPageItem = ModelFactory.CreateItem(EditingContext, typeof(ClassicWizardTitlePage), CreateOptions.InitializeDefaults, new object[0]);

            // Set the content of the page
            if (PageItemCollection.Count > 0)
            {
                wizardPageItem.Properties[WizardPage.IsFinalPageProperty].SetValue(true);
            }

            // Insert the page
            PageItemCollection.Insert(ActivePageIndex, wizardPageItem);

            // Update the active index
            ActivePageItem = wizardPageItem;

            // Event Complete
            e.Handled = true;     
        }
        


        /// <summary>
        /// Move the selected page forwards
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMovePageForwards( object sender, ExecutedRoutedEventArgs e )
        {
            int        oldIndex        = ActivePageIndex;
            int        newIndex        = oldIndex + 1;

            // Start the edit
            using (ModelEditingScope scope = PageItemCollection.BeginEdit())
            {
                ModelItem  tempItem  = PageItemCollection[newIndex];

                // Move the item
                PageItemCollection.RemoveAt(newIndex);
                PageItemCollection.Insert(oldIndex, tempItem);

                // Commit
                scope.Complete();
            }
        }

 
        /// <summary>
        /// Move the selected page backwards
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMovePageBackwards( object sender, ExecutedRoutedEventArgs e )
        {
            int        oldIndex        = ActivePageIndex;
            int        newIndex        = oldIndex - 1;

            // Start the edit
            using (ModelEditingScope scope = PageItemCollection.BeginEdit())
            {
                 ModelItem  tempItem  = PageItemCollection[newIndex];

                // Update the page item collection
                PageItemCollection.RemoveAt(newIndex);
                PageItemCollection.Insert(oldIndex, tempItem);

                // Commit
                scope.Complete();
            }
        }


        /// <summary>
        /// delete a page from the wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeletePage( object sender, ExecutedRoutedEventArgs e )
        {
            TaskDialog   taskDialog = new TaskDialog(this);

            taskDialog.Title = "Remove Page";
            taskDialog.Instruction = "Do you want to remove the current page";
            taskDialog.Message = "This operation can be undone using Visual Studio's Undo mechanism.";
            taskDialog.AddCommandLink("Remove Page", "Remove page from the wizard", TaskIcons.Arrow, "Remove", false, false);
            taskDialog.AddCommandLink("Cancel", "Cancel the operation", TaskIcons.Arrow, "Cancel", true, false);

            if (taskDialog.ShowDialog() == "Remove")
            {
                PageItemCollection.Remove(ActivePageItem);
            }
        }
        #endregion

    }
}
