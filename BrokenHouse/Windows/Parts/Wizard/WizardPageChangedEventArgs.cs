using System;
using System.Collections.Generic;
using System.Windows;

namespace BrokenHouse.Windows.Parts.Wizard
{
    /// <summary>
    /// The types of change that can trigger a page change event
    /// </summary>
    public enum WizardPageChangeType 
    { 
        /// <summary>
        /// Signifies that that the change is a result of a navigation direct to the new page
        /// </summary>
        NavigateTo, 

        /// <summary>
        /// Signifies that the change is a result of a Back wizard command
        /// </summary>
        NavigateBack, 

        /// <summary>
        /// Signifies that the change is a result of the Next wizard command
        /// </summary>
        NavigateNext, 

        /// <summary>
        /// Signifies that the change is a result of the Finish wizard command
        /// </summary>
        NavigateFinish 
    };

    /// <summary>
    /// Provides data for the <see cref="WizardPage.PageActivated"/> and <see cref="WizardPage.PageActivated"/> events.
    /// </summary>
    /// <remarks>
    /// When a page has been changed and event is triggered to say that the old page has been
    /// deactivated and the new page has been activated. This class contains the information
    /// about that change.
    /// </remarks>
    public class WizardPageChangedEventArgs : RoutedEventArgs 
    {
        /// <summary>
        /// Specifies the new active wizard page. If the <see cref="ChangeType"/> property
        /// is set to <see cref="WizardPageChangeType.NavigateFinish"/> then this value
        /// will be <c>null</c> signify that there will be no new page because the wizard
        /// is finishing.
        /// </summary>
        public WizardPage NewPage { get; private set; }

        /// <summary>
        /// Specifies page that was active at the time the change was requested.
        /// </summary>
        public WizardPage OldPage { get; private set; }

        /// <summary>
        /// The type of change that triggered the change event
        /// </summary>
        public WizardPageChangeType ChangeType { get; private set; } 
      
        /// <summary>
        /// Creates new instance of the class with default values.
        /// </summary>
        /// <param name="newPage">The new page that we are moving to</param>
        /// <param name="oldPage">The old page that we are moving from</param>
        /// <param name="changeType">The type of change that has triggered this event</param>
        public WizardPageChangedEventArgs( WizardPage newPage, WizardPage oldPage, WizardPageChangeType changeType )
        {
             NewPage    = newPage;
             OldPage    = oldPage;
             ChangeType = changeType;
        }
    }
}
