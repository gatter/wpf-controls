using System;
using System.Collections.Generic;
using System.Windows;

namespace BrokenHouse.Windows.Parts.Wizard
{
    /// <summary>
    /// Provides data for the <see cref="WizardPage.PageActivating"/> and <see cref="WizardPage.PageDeactivating"/> events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When a page has been changed an event is triggered on the old page to inform the page that it is being deactivated
    /// and a subsequent event is triggered on the new page to inform it that it is being activated. This class not only contains
    /// the information about the change but also allows the event handler to influence that change.
    /// </para>
    /// <para>
    /// The handler of the either the <see cref="WizardPage.PageActivating"/> or the <see cref="WizardPage.PageDeactivating"/> event 
    /// has the option to change the new page to another page other than the one requested by setting the <see cref="NewPage"/> property. 
    /// If a new page is suppled then that page will have its <see cref="WizardPage.PageActivating"/> event triggered. That page can
    /// also change the new page if required.
    /// </para>
    /// <para>
    /// If at any point the change is dissallowed then you can use the <see cref="CancelChange"/> method to provide
    /// the reason the navigation was cancelled. This text will be displayed, in the default styles, 
    /// at the bottom of the wizard page.
    /// </para>
    /// </remarks>
    public class WizardPageChangingEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or Sets the new active wizard page.
        /// </summary>
        /// <remarks>
        /// When the next page in the wizard is requested
        /// </remarks>
        public WizardPage NewPage { get; set; }

        /// <summary>
        /// Specifies page that was or currently is active.
        /// </summary>
        public WizardPage OldPage { get; private set; }

        /// <summary>
        /// The type of change that triggered the change event
        /// </summary>
        public WizardPageChangeType ChangeType { get; private set; } 
  
        /// <summary>
        /// Gets whenther the changing of the page has been allowed
        /// </summary>
        public bool IsChangeAllowed 
        { 
            get { return string.IsNullOrEmpty(CancelReason); } 
        }

        /// <summary>
        /// Allows a reason for the cancel to be provided
        /// </summary>
        public string CancelReason { get; private set; }

        /// <summary>
        /// Creates new instance of the class with default values.
        /// </summary>
        /// <param name="newPage">The new page that we will be moving to.</param>
        /// <param name="oldPage">The old page that we are moving from.</param>
        /// <param name="changeType">The type of change that is occuring.</param>
        public WizardPageChangingEventArgs( WizardPage newPage, WizardPage oldPage, WizardPageChangeType changeType )
        {
            NewPage    = newPage;
            OldPage    = oldPage;
            ChangeType = changeType;
        }

        /// <summary>
        /// Cancel the change with an appropriate message
        /// </summary>
        public void CancelChange( string message )
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message", "Cannot cancel a change without a valid message");
            }
            CancelReason = message;
        }
    }
}
