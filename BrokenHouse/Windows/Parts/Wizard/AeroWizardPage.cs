using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace BrokenHouse.Windows.Parts.Wizard
{
    /// <summary>
    /// This class provides a content page for the Aero wizard.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlinke the Wizard97 specification, the Aero wizard specification defines that a wizard should have only one
    /// type of wizard page that has a header that is in large bold text.
    /// </para>
    /// </remarks>
    [DefaultEvent("PageActivatingEvent")]
    public class AeroWizardPage : WizardPage
    {
        /// <summary>
        /// Identifies the <see cref="Header"/> depedency property.
        /// </summary>
        public  static readonly DependencyProperty    HeaderProperty;
        
        #region -- Static Constructor ---

        /// <summary>
        /// Static constructor
        /// </summary>
		static AeroWizardPage()
		{
            // The header for the dialog
            HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(AeroWizardPage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnHeaderChangedThunk));
            
            // Override the metadata
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AeroWizardPage), new FrameworkPropertyMetadata(WizardElements.AeroWizardPageStyleKey));
        }
  
        #endregion
         
        #region -- Public Properties ---

        /// <summary>
        /// Gets or sets the content to be used for the header of the <see cref="AeroWizardPage"/>. This is a depedency property.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
  
        #endregion
         
        #region -- Static Property Changed Event Handlers ---

        /// <summary>
        /// The header property has changed - trigger the instance specific handler
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnHeaderChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            AeroWizardPage page = target as AeroWizardPage;

            page.OnHeaderChanged(args.OldValue as string, args.NewValue as String);
        }
  
        #endregion
         
        #region -- Protected Event Handlers ---
        
        /// <summary>
        /// Called when the <see cref="Header"/> property of a <see cref="AeroWizardPage"/> changes. 
        /// </summary>
        /// <param name="oldHeader">The old value of the <see cref="Header"/> property</param>
        /// <param name="newHeader">The new value of the <see cref="Header"/> property</param>
        protected virtual void OnHeaderChanged( string oldHeader, string newHeader )
        {
            InvalidateVisual();
        }
  
        #endregion
         
    }
}
