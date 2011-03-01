using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Parts.Wizard
{
    /// <summary>
    /// The basic wizard page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="WizardPage"/> is a specialised content control that can be used as a page in a wizard. There
    /// is no styling associated with the wizard page so any content will be rendered directly into the 
    /// page area of the wizard.
    /// </para>
    /// <para>
    /// The fundemental logic of the wizard system is defined by its collection of pages. Each page
    /// controls its own navigation via a series of events and dependency properties. If this page is the
    /// active page then its <see cref="PageDeactivating"/> event is triggered, the handler of this event
    /// has the option to allow the move to the next page, force the wizard to move to a different page 
    /// or cancel the navigation with an optional error message (see <see cref="WizardPageChangingEventArgs"/>).
    /// Once the next page has been decieded it will be sent the <see cref="PageActivating"/> event, similar
    /// to the <see cref="PageDeactivating"/> event the activating page has the option to become the active page, 
    /// force the wizard to move to a different page or cancel the navigation with an optional error message. Once a page
    /// has been defined as the active page then the old active page is sent the  <see cref="PageDeactivated"/> event
    /// and new new active page is sent the <see cref="PageActivated"/> event.
    /// </para>
    /// <para>
    /// When a page is the active page the navigation buttons are controled using <see cref="IsNextEnabled"/>, 
    /// <see cref="IsBackEnabled"/> and <see cref="IsFinishEnabled"/> poperties of that page.
    /// These properties will be overridden if the page is either the first page or the final
    /// page in the wizard, in these cases the Back and Next buttons will be disabled respectively. To enable
    /// multiple paths through the wizard a page can be defined as the final page (even if it is not the last
    /// page) by setting the <see cref="IsFinalPage"/> property to <b>True</b>.
    /// </para>
    /// <para>
    /// The Finish button is a special button and is controlled by the <see cref="WizardControl.IsFinishAlwaysVisible"/>. 
    /// When this property is set to false the Finish button will take the place of the Next button when the active page
    /// is a final page. It will be enabled if the <see cref="IsFinishEnabled"/> property is set to <b>True</b> (if this
    /// property is unset then the <see cref="IsNextEnabled"/> will be used as a fallback). If 
    /// <see cref="WizardControl.IsFinishAlwaysVisible"/> is set to true then the Finish button will always be visible
    /// and it will only be enabled if the <see cref="IsFinishEnabled"/> property is set to <b>True</b> (again if the property
    /// is unset for a final page then the <see cref="IsNextEnabled"/> will be used as a fallback).
    /// </para>
    /// </remarks>
    [DefaultEvent("PageActivatingEvent")]
    public class WizardPage : ContentControl
    {
        #region --- Dependency objects ---

        /// <summary>
        /// Identifies the PageActiviting event
        /// </summary>
        public static readonly RoutedEvent PageActivatingEvent;

        /// <summary>
        /// Identifies the PageDeactivating event
        /// </summary>
        public static readonly RoutedEvent PageDeactivatingEvent;

        /// <summary>
        /// Identifies the PageActivated event
        /// </summary>
        public static readonly RoutedEvent PageActivatedEvent;

        /// <summary>
        /// Identifies the PageDeactivated event
        /// </summary>
        public static readonly RoutedEvent PageDeactivatedEvent;
        
        /// <summary>
        /// Identifies the <see cref="NextLabel"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     NextLabelProperty;

        /// <summary>
        /// Identifies the <see cref="FinishLabel"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     FinishLabelProperty;
 
        /// <summary>
        /// Identifies the <see cref="BackLabel"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     BackLabelProperty;
        
        /// <summary>
        /// Identifies the <see cref="IsNextEnabled"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     IsNextEnabledProperty;

        /// <summary>
        /// Identifies the <see cref="IsNextEnabled"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     IsFinishEnabledProperty;
 
        /// <summary>
        /// Identifies the <see cref="IsBackEnabled"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     IsBackEnabledProperty;
        
        /// <summary>
        /// Identifies the <see cref="IsFinalPage"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     IsFinalPageProperty;

        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property key
        /// </summary>
        private static readonly DependencyPropertyKey  IsActiveKey;

        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     IsActiveProperty;

        #endregion

        /// <summary>
        /// The parent wizard of this page
        /// </summary>
        private WizardControl m_ParentWizard    = null;

        /// <summary>
        /// The binding that links our flow directions to the parents
        /// </summary>
        private Binding       m_FlowDirectionBinding = null;

        #region --- Constructors ---

        /// <summary>
        /// Static constructor
        /// </summary>
		static WizardPage()
		{
            // Define the events
            PageActivatingEvent        = EventManager.RegisterRoutedEvent("PageActivatingEvent",        RoutingStrategy.Bubble, typeof(EventHandler<WizardPageChangingEventArgs>), typeof(WizardPage));
            PageDeactivatingEvent      = EventManager.RegisterRoutedEvent("PageDeactivatingEvent",      RoutingStrategy.Bubble, typeof(EventHandler<WizardPageChangingEventArgs>), typeof(WizardPage));
            PageActivatedEvent         = EventManager.RegisterRoutedEvent("PageActivatedEvent",         RoutingStrategy.Bubble, typeof(EventHandler<WizardPageChangedEventArgs>), typeof(WizardPage));
            PageDeactivatedEvent       = EventManager.RegisterRoutedEvent("PageDeactivatedEvent",       RoutingStrategy.Bubble, typeof(EventHandler<WizardPageChangedEventArgs>), typeof(WizardPage));
 
            // First the readonly properties
            IsActiveKey             = DependencyProperty.RegisterReadOnly("IsActive", typeof(bool), typeof(WizardPage), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsActiveChangedThunk)));
            
            // Define the properties
            IsNextEnabledProperty   = DependencyProperty.Register("IsNextEnabled", typeof(bool?), typeof(WizardPage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIsNextEnabledChangedThunk)));
            IsBackEnabledProperty   = DependencyProperty.Register("IsBackEnabled", typeof(bool?), typeof(WizardPage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIsBackEnabledChangedThunk)));
            IsFinishEnabledProperty = DependencyProperty.Register("IsFinishEnabled", typeof(bool?), typeof(WizardPage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIsFinishEnabledChangedThunk)));
            NextLabelProperty       = DependencyProperty.Register("NextLabel", typeof(string), typeof(WizardPage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNextLabelChangedThunk)));
            BackLabelProperty       = DependencyProperty.Register("BackLabel", typeof(string), typeof(WizardPage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBackLabelChangedThunk)));
            FinishLabelProperty     = DependencyProperty.Register("FinishLabel", typeof(string), typeof(WizardPage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFinishLabelChangedThunk)));
            IsFinalPageProperty     = DependencyProperty.Register("IsFinalPage", typeof(bool), typeof(WizardPage), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsFinalPageChangedThunk)));
            IsActiveProperty        = IsActiveKey.DependencyProperty;

            // Register the event handles
            EventManager.RegisterClassHandler(typeof(WizardPage), PageActivatingEvent, new EventHandler<WizardPageChangingEventArgs>(OnPageActivatingThunk));
            EventManager.RegisterClassHandler(typeof(WizardPage), PageDeactivatingEvent, new EventHandler<WizardPageChangingEventArgs>(OnPageDeactivatingThunk));
            EventManager.RegisterClassHandler(typeof(WizardPage), PageActivatedEvent, new EventHandler<WizardPageChangedEventArgs>(OnPageActivatedThunk));
            EventManager.RegisterClassHandler(typeof(WizardPage), PageDeactivatedEvent, new EventHandler<WizardPageChangedEventArgs>(OnPageDeactivatedThunk));
          
            // Override the keyboard navigation
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(WizardPage), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(WizardPage), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(WizardPage), new FrameworkPropertyMetadata(false));
        }

        /// <summary>
        /// Protected constructor to stop this page from being instantiated directly.
        /// </summary>
        internal protected WizardPage()
        {
        }
         
        #endregion
               
        #region --- Private Helpers ---
 
        /// <summary>
        /// Cause the wizard to update the state of the buttons
        /// </summary>
        private void UpdateWizardState()
        {
            if (m_ParentWizard != null)
            {
                m_ParentWizard.UpdateButtonState();
            }
        }


        #endregion
               
        #region --- Public Events ---
        
        /// <summary>
        /// Occurs when this page is being deactivated.
        /// </summary>
        /// <remarks>
        /// Upon receiving this event the handler can either: 
        /// <list type="number">
        /// <item>
        /// <term>Cancel the navigation.</term>
        /// <description>If the content of the wizard can be checked and validated at this point then an
        /// error message can be supplied before the next page is activated.</description>
        /// </item>
        /// <item>
        /// <term>Change the next page.</term>
        /// <description>It may be appropriate to skip a page depending on the state of the wizard or
        /// jump to another page.</description>
        /// </item>
        /// <item>
        /// <term>Simply allow the navigation to proceed.</term>
        /// <description>This is the default action</description>
        /// </item>
        /// </list>
        /// If an excpetion is triggered by an event handler then the message of the exception will be used to set
        /// the <see cref="WizardPageChangingEventArgs.CancelReason"/> property.
        /// </remarks>
        public event EventHandler<WizardPageChangingEventArgs> PageDeactivating
        {
            add { AddHandler(PageDeactivatingEvent, value); }
            remove { RemoveHandler(PageDeactivatingEvent, value); }
        }

        /// <summary>
        /// Occurs when this page is being activated.
        /// </summary>
        /// <remarks>
        /// The role of this event is to allow the page to perform checks to see if it can be shown to the user.
        /// Upon receiving this event the handler can either: 
        /// <list type="number">
        /// <item>
        /// <term>Cancel the navigation.</term>
        /// <description>If the preparation fails. In this case the <see cref="WizardPageChangingEventArgs.OldPage"/>
        /// will remain visible with the <see cref="WizardPageChangingEventArgs.CancelReason"/> being displayed to the user (if specified).</description>
        /// </item>
        /// <item>
        /// <term>Change the next page.</term>
        /// <description>It may be appropriate to change the next page based on what the page is appropriate. In
        /// some cases it may be advantageous to skip a page if action by the user is not required.
        /// For example, if the page contained a list box and the user had to select an item before proceeding; if
        /// that list box only contained one item then it would be appropriate to select the item automatically
        /// and skip the page.</description>
        /// </item>
        /// <item>
        /// <term>Simply allow the navigation to proceed.</term>
        /// <description>This is the default action</description>
        /// </item>
        /// </list>
        /// If an excpetion is triggered by an event handler then the message of the exception will be used to set
        /// the <see cref="WizardPageChangingEventArgs.CancelReason"/> property.
        /// </remarks>
        public event EventHandler<WizardPageChangingEventArgs> PageActivating
        {
            add { AddHandler(PageActivatingEvent, value); }
            remove { RemoveHandler(PageActivatingEvent, value); }
        }

        /// <summary>
        /// Occurs when the page has been deactivated
        /// </summary>
        /// <remarks>
        /// <para>
        /// Once all the <see cref="PageActivating"/> events have run thier course and a final page has
        /// been selected the original page will receive this event. The event is used to allow the page
        /// to perform any clean up operations that might be required before the page is removed from
        /// display.
        /// </para>
        /// <para>
        /// Unlike the <see cref="PageActivating"/> and <see cref="PageDeactivating"/> events if an exception
        /// occurs then it will be handled by the system.
        /// </para>
        /// </remarks>
        public event EventHandler<WizardPageChangedEventArgs> PageDeactivated
        {
            add { AddHandler(PageDeactivatedEvent, value); }
            remove { RemoveHandler(PageDeactivatedEvent, value); }
        }

        /// <summary>
        /// Occurs when the page has been activated
        /// </summary>
        /// <remarks>
        /// <para>
        /// Once all the <see cref="PageActivating"/> events have run thier course and a final page has
        /// been selected the new page will receive this event. This event is used to allow the page 
        /// to perform any preparation before the page is displayed to the user.
        /// </para>
        /// <para>
        /// Unlike the <see cref="PageActivating"/> and <see cref="PageDeactivating"/> events if an exception
        /// occurs then it will be handled by the system because at this point the navigation has competed and
        /// no errors are expected.
        /// </para>
        /// </remarks>
        public event EventHandler<WizardPageChangedEventArgs> PageActivated
        {
            add { AddHandler(PageActivatedEvent, value); }
            remove { RemoveHandler(PageActivatedEvent, value); }
        }

        #endregion
        
        #region --- Public Properties ---
 
        /// <summary>
        /// Gets or sets whether the Next button should be enabled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The effect that this property has is dependent on whether the page is a final page or not. 
        /// <list type="bullet">
        /// <item>
        /// <term>The page is not a final page</term>
        /// <description>
        /// This property will be used to define the enabled state of the Next button. If a value has not been 
        /// assigned then the Next button will be enabled by default.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Page is a final page</term>
        /// <description>
        /// If <see cref="WizardControl.IsFinishAlwaysVisible"/> property is set to
        /// <c>True</c> then the Next button will be visible but disabled (irrespective of the value of this property).
        /// If the <see cref="WizardControl.IsFinishAlwaysVisible"/> property is set to <c>False</c> 
        /// then the Next button will be replaced with a Finish button. In both cases the enabled state of the Finish
        /// button is determined by the <see cref="IsFinishEnabled"/> property. 
        /// </description>
        /// </item>
        /// <item>
        /// <term>Simply allow the navigation to proceed.</term>
        /// <description>This is the default action</description>
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// If <see cref="IsFinishEnabled"/> is unset then the enabled state of the Finish button will be determined by this property.
        /// </para>
        /// </remarks>
        /// <seealso cref="IsFinishEnabled"/>
        /// <seealso cref="IsFinalPage"/>
        /// <seealso cref="WizardControl.IsFinishAlwaysVisible"/>
        [Bindable(true)]
        [Category("Behavior")]
        public bool? IsNextEnabled
        {
            get { return (bool?)GetValue(IsNextEnabledProperty); }
            set { SetValue(IsNextEnabledProperty, value); }
        }
        
        /// <summary>
        /// Gets ör sets whether the Finish button is enabled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The effect that this property has is dependent on whether the page is a final page or not. 
        /// <list type="bullet">
        /// <item>
        /// <term>The page is not a final page</term>
        /// <description>
        /// This property will be used to define the enabled state of the Finish button. If a value has not been 
        /// assigned then the Finish button will be disabled by default.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Page is a final page</term>
        /// <description>
        /// If <see cref="WizardControl.IsFinishAlwaysVisible"/> property is set to
        /// <b>True</b> then the Finish button will always be visible; it will only be enabled if the
        /// <see cref="IsFinishEnabled"/> property is set to <c>True</c>.
        /// If the <see cref="WizardControl.IsFinishAlwaysVisible"/> property is set to <b>False</b> 
        /// then the Finish button will replace with a Next button. In both cases the enabled state of the Finish
        /// button is determined by the <see cref="IsFinishEnabled"/> property. If this is unset then the state
        /// will be determined by the <see cref="IsNextEnabled"/>. 
        /// </description>
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// The reason why we fallback to the <see cref="IsNextEnabled"/> property for a final page is to keep the logic
        /// for a basic wizard simple. If the code does not modify any of the properties then the user will automatically
        /// navigate trough all the pages and finish the wizard on the final page. This is irrespective of the the 
        /// <see cref="WizardControl.IsFinishAlwaysVisible"/> property. If a more complicated wizard is required all the
        /// developer has to do is define the properties on the pages that need to deviate from this simple navigation.
        /// </para>
        /// </remarks>
        /// <seealso cref="IsNextEnabled"/>
        /// <seealso cref="IsFinalPage"/>
        /// <seealso cref="WizardControl.IsFinishAlwaysVisible"/>
        [Bindable(true)]
        [Category("Behavior")]
        public bool? IsFinishEnabled
        {
            get { return (bool?)GetValue(IsFinishEnabledProperty); }
            set { SetValue(IsFinishEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the Back button is enabled.
        /// </summary>
        /// <remarks>
        /// The default behaviour of the Back button is to be enabled
        /// for all but the first page in the wizard. If you set this property
        /// to <b>False</b> then the Back button will be 
        /// disabled when this page becomes active.
        /// </remarks>
        [Bindable(true)]
        [Category("Behavior")]
        public bool? IsBackEnabled
        {
            get { return (bool?)GetValue(IsBackEnabledProperty); }
            set { SetValue(IsBackEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether this page is a final page.
        /// </summary>
        /// <remarks>
        /// Defining a wizard page as a final page means that the wizard
        /// will not proceed any further even than this page, even if the page is not the final
        /// page in the wizard. This is useful when you have a wizard that can have two paths that lead to different 
        /// final pages.
        /// </remarks>
        [Bindable(true)]
        [Category("Behavior")]
        public bool IsFinalPage
        {
            get { return (bool)GetValue(IsFinalPageProperty); }
            set { SetValue(IsFinalPageProperty, value); }
        }

        /// <summary>
        /// Gets whether this page is the current active page in the wizard
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            internal set { SetValue(IsActiveKey, value); }
        }

        /// <summary>
        /// Gets the parent <see cref="WizardControl"/> that this <see cref="WizardPage"/> is associated.
        /// </summary>
        [Bindable(false)]
        [ReadOnly(true)]
        public WizardControl ParentWizard
        {
            get { return m_ParentWizard; }
            internal set 
            { 
                if (m_ParentWizard != null)
                {
                    if (m_FlowDirectionBinding != null)
                    {
                        BindingOperations.ClearBinding(this, FrameworkElement.FlowDirectionProperty);
                        m_FlowDirectionBinding = null;
                    }
                }
                m_ParentWizard = value; 
                if (m_ParentWizard != null)
                {
                    m_FlowDirectionBinding = new Binding("FlowDirection") { Source = m_ParentWizard };
                    BindingOperations.SetBinding(this, FrameworkElement.FlowDirectionProperty, m_FlowDirectionBinding);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the custom label for the back button which will be used when this page is active.
        /// </summary>
        [Bindable(true)]
        public string BackLabel
        {
            get { return (string)GetValue(BackLabelProperty); }
            set { SetValue(BackLabelProperty, value); }
        }
       
        /// <summary>
        /// Gets or sets the custom label for the next button which will be used when this page is active.
        /// </summary>
        [Bindable(true)]
        public string NextLabel
        {
            get { return (string)GetValue(NextLabelProperty); }
            set { SetValue(NextLabelProperty, value); }
        }
       
        /// <summary>
        /// Gets or sets the custom label for the finish button which will be used when this page is active.
        /// This is a 
        /// </summary>
        [Bindable(true)]
        public string FinishLabel
        {
            get { return (string)GetValue(FinishLabelProperty); }
            set { SetValue(FinishLabelProperty, value); }
        }

        #endregion

        #region --- Static Event Thunks ---
     
        /// <summary>
        /// Thunk for the PageActivating Event
        /// </summary>
        private static void OnPageActivatingThunk( object sender, WizardPageChangingEventArgs args )
        {
            (sender as WizardPage).OnPageActivating(args);
        }
        
        /// <summary>
        /// Thunk for the PageDeactivating Event
        /// </summary>
        private static void OnPageDeactivatingThunk( object sender, WizardPageChangingEventArgs args )
        {
            (sender as WizardPage).OnPageDeactivating(args);
        }

        /// <summary>
        /// Thunk for the PageActivated Event
        /// </summary>
        private static void OnPageActivatedThunk( object sender, WizardPageChangedEventArgs args )
        {
            (sender as WizardPage).OnPageActivated(args);
        }

        /// <summary>
        /// Thunk for the PageDeactivated Event
        /// </summary>
        private static void OnPageDeactivatedThunk( object sender, WizardPageChangedEventArgs args )
        {
            (sender as WizardPage).OnPageDeactivated(args);
        }
        
        /// <summary>
        /// The IsNextEnabled property has changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnIsNextEnabledChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
           (target as WizardPage).OnIsNextEnabledChanged((bool?)args.NewValue, (bool?)args.OldValue);
        }
       
        /// <summary>
        /// The IsBackEnabled property has changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnIsBackEnabledChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
           (target as WizardPage).OnIsBackEnabledChanged((bool?)args.NewValue, (bool?)args.OldValue);
        }

        /// <summary>
        /// The IsFinishEnabled property has changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnIsFinishEnabledChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
           (target as WizardPage).OnIsFinishEnabledChanged((bool?)args.NewValue, (bool?)args.OldValue);
        }
        
        /// <summary>
        /// The NextLabel property has changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnNextLabelChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
           (target as WizardPage).OnNextLabelChanged((string)args.NewValue, (string)args.OldValue);
        }
       
        /// <summary>
        /// The BackLabel property has changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnBackLabelChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
           (target as WizardPage).OnBackLabelChanged((string)args.NewValue, (string)args.OldValue);
        }

        /// <summary>
        /// The FinishLabel property has changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnFinishLabelChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
           (target as WizardPage).OnFinishLabelChanged((string)args.NewValue, (string)args.OldValue);
        }

        /// <summary>
        /// The IsFinalPage property has changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnIsFinalPageChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
           (target as WizardPage).OnIsFinalPageChanged((bool)args.NewValue, (bool)args.OldValue);
        }

        /// <summary>
        /// The IsActive property has changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        private static void OnIsActiveChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
           (target as WizardPage).OnIsActiveChanged((bool)args.NewValue, (bool)args.OldValue);
        }

        #endregion
 
        #region --- Overridable Event Handlers ---
 
        /// <summary>
        /// Called when the <see cref="IsFinalPage"/> property changes.
        /// </summary>
        protected virtual void OnIsFinalPageChanged( bool newValue, bool oldValue )
        {
            UpdateWizardState();
        }

        /// <summary>
        /// Called when the <see cref="IsFinishEnabled"/> property changes.
        /// </summary>
        protected virtual void OnIsFinishEnabledChanged( bool? newValue, bool? oldValue )
        {
            UpdateWizardState();
        }
                 
        /// <summary>
        /// Called when the <see cref="IsNextEnabled"/> property changes.
        /// </summary>
        protected virtual void OnIsNextEnabledChanged( bool? newValue, bool? oldValue )
        {
            UpdateWizardState();
        }     
            
        /// <summary>
        /// Called when the <see cref="IsBackEnabled"/> property changes.
        /// </summary>
        protected virtual void OnIsBackEnabledChanged( bool? newValue, bool? oldValue )
        {
            UpdateWizardState();
        }
         
        /// <summary>
        /// Called when the <see cref="FinishLabel"/> property changes.
        /// </summary>
        protected virtual void OnFinishLabelChanged( string newValue, string oldValue )
        {
            UpdateWizardState();
        }
                 
        /// <summary>
        /// Called when the <see cref="NextLabel"/> property changes.
        /// </summary>
        protected virtual void OnNextLabelChanged( string newValue, string oldValue )
        {
            UpdateWizardState();
        }     
            
        /// <summary>
        /// Called when the <see cref="BackLabel"/> property changes.
        /// </summary>
        protected virtual void OnBackLabelChanged( string newValue, string oldValue )
        {
            UpdateWizardState();
        }
   
        /// <summary>
        /// Called when the <see cref="IsActive"/> property changed.
        /// </summary>
        protected virtual void OnIsActiveChanged( bool? newValue, bool? oldValue )
        {
        }
               
        /// <summary>
        /// This is a chance for any derived class to handle the PageDeactivating Event
        /// </summary>
        protected virtual void OnPageDeactivating( WizardPageChangingEventArgs args )
        {
        }

        /// <summary>
        /// This is a chance for any derived class to handle the PageActivating Event
        /// </summary>
        protected virtual void OnPageActivating( WizardPageChangingEventArgs args )
        {
        }

        /// <summary>
        /// This is a chance for any derived class to handle the PageActivated Event
        /// </summary>
        protected virtual void OnPageActivated( WizardPageChangedEventArgs args )
        {
        }

        /// <summary>
        /// This is a chance for any derived class to handle the PageDeactivated Event
        /// </summary>
        protected virtual void OnPageDeactivated( WizardPageChangedEventArgs args )
        {
        }

        #endregion
    }
}
