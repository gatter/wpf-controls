using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Security;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Reflection;
using BrokenHouse.Extensions;
using BrokenHouse.Windows.Controls;
using BrokenHouse.Windows.Data;
using BrokenHouse.Windows.Parts.Transition;
using BrokenHouse.Windows.Parts.Transition.Primitives;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Parts.Wizard.Input;

namespace BrokenHouse.Windows.Parts.Wizard
{
    /// <summary>
    /// The basic wizard control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>WizardControl</c> when placed in a window forms the basis of a wizard. It provides
    /// container for all the wizard pages as well as the logic and buttons for navigation.
    /// </para>
    /// <para>
    /// The wizard framework has been deisned to work with controllers and view models. When you
    /// implement a wizard you can design the wizard to have a single controller and a single
    /// view model which will be used for all pages. Alternatively you can provide custom
    /// implementations of a WizardPage (or a derived class) for each distinct page in your
    /// wizard; each page would require its own controller and view model. There are no restrictions
    /// on whether the controller and view model are the same or different objects.
    /// </para>
    /// <para>
    /// The navigation within a wizard is controlled using the commands found in <see cref="WizardCommands"/>.
    /// If you place a button control linked to one of these commands then it will act like the appropriate
    /// navigation button. In additional to the normal backwards and forwards navigation there is a <see cref="WizardCommands.MoveTo"/>
    /// command; if you attach this command to a button and provide the name of a wizard page as the command parameter
    /// then when the button is pressed the wizard will jump to the named page. This extendard navigation allows
    /// multiple paths to be handled by the same wizard (similar to the Add Printer wizard in Vista and XP).
    /// </para>
    /// </remarks>
    [TemplatePart(Name = "PART_PageHost", Type = typeof(TransitionPresenter))]
    [ContentProperty("Pages")]
    public partial class WizardControl : Control, ICollectionViewModelParent
    {
        private static readonly DependencyPropertyKey  IsLastErrorVisibleKey;
        private static readonly DependencyPropertyKey  LastErrorKey;
        private static readonly DependencyPropertyKey  NextButtonStateKey;
        private static readonly DependencyPropertyKey  BackButtonStateKey;
        private static readonly DependencyPropertyKey  FinishButtonStateKey;
        private static readonly DependencyProperty     ActivePageProperty;

        /// <summary>
        /// Identifies the <see cref="PagesSource"/> dependency property.
        /// </summary>
        public  static readonly DependencyProperty     PagesSourceProperty;

        /// <summary>
        /// Identifies the <see cref="ActiveIndex"/> dependency property.
        /// </summary>
        public  static readonly DependencyProperty     ActiveIndexProperty;

        /// <summary>
        /// Identifies the <see cref="LastError"/> dependency property.
        /// </summary>
        public  static readonly DependencyProperty     LastErrorProperty;

        /// <summary>
        /// Identifies the <see cref="IsLastErrorVisible"/> dependency property.
        /// </summary>
        public  static readonly DependencyProperty     IsLastErrorVisibleProperty;

        /// <summary>
        /// Identifies the <see cref="NextButtonState"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     NextButtonStateProperty;

        /// <summary>
        /// Identifies the <see cref="BackButtonState"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     BackButtonStateProperty;

        /// <summary>
        /// Identifies the <see cref="FinishButtonState"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     FinishButtonStateProperty;
        
        /// <summary>
        /// Identifies the <see cref="IsFinishAlwaysVisible"/> dependency property.
        /// </summary>
        public  static readonly DependencyProperty     IsFinishAlwaysVisibleProperty;

        /// <summary>
        /// Identifies the <see cref="TransitionEffect"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     TransitionEffectProperty;

        /// <summary>
        /// Identifies the <see cref="ActivePageChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent ActivePageChangedEvent;

        /// <summary>
        /// Identifies the <see cref="ActiveIndexChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent ActiveIndexChangedEvent;

        /// <summary>
        /// This is a flag that is used to stop the active page/index from
        /// going into an infinate loop.
        /// </summary>
        private bool                            m_IsActiveChanging;

        /// <summary>
        /// If the control is not consistant then the control
        /// has not been initialised or shown.
        /// </summary>
        private bool                            m_IsConsistant;

        /// <summary>
        /// The history of pages that have been navigated.
        /// </summary>
        private Stack<int>                      m_History;

        /// <summary>
        /// The type of the last navigation
        /// </summary>
        private WizardPageChangeType            m_LastChangeType;

        /// <summary>
        /// The presenter that will perform the animations
        /// </summary>
        private TransitionPresenter             m_TransitionPresenter;

        #region --- Constructors ---

        /// <summary>
        /// Static constructor to register all the WPF stuff
        /// </summary>
        static WizardControl()
        {
            // Define the read only keys
            LastErrorKey                      = DependencyProperty.RegisterReadOnly("LastError", typeof(string), typeof(WizardControl), new FrameworkPropertyMetadata("", null), null);
            NextButtonStateKey                = DependencyProperty.RegisterReadOnly("NextButtonState", typeof(WizardButtonState), typeof(WizardControl), new FrameworkPropertyMetadata(null, null), null);
            BackButtonStateKey                = DependencyProperty.RegisterReadOnly("BackButtonState", typeof(WizardButtonState), typeof(WizardControl), new FrameworkPropertyMetadata(null, null), null);
            FinishButtonStateKey              = DependencyProperty.RegisterReadOnly("FinishButtonState", typeof(WizardButtonState), typeof(WizardControl), new FrameworkPropertyMetadata(null, null), null);
            IsLastErrorVisibleKey             = DependencyProperty.RegisterReadOnly("IsLastErrorVisible", typeof(bool), typeof(WizardControl), new FrameworkPropertyMetadata(false));
            
            // Define the properties
            IsFinishAlwaysVisibleProperty     = DependencyProperty.Register("IsFinishAlwaysVisible", typeof(bool), typeof(WizardControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsFinishAlwaysVisibleChangedThunk)), null);
            TransitionEffectProperty          = TransitionPresenter.TransitionEffectProperty.AddOwner(typeof(WizardControl));
            ActiveIndexProperty               = DependencyProperty.Register("ActiveIndex", typeof(int), typeof(WizardControl), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnActiveIndexChangedThunk, CoerceActiveIndex), ValidateActiveIndex);
            ActivePageProperty                = DependencyProperty.Register("ActivePage", typeof(WizardPage), typeof(WizardControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnActivePageChangedThunk, CoerceActivePage), null);
            PagesSourceProperty               = DependencyProperty.Register("PagesSource", typeof(IEnumerable), typeof(WizardControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnPagesSourceChangedThunk), null);
            LastErrorProperty                 = LastErrorKey.DependencyProperty;
            NextButtonStateProperty           = NextButtonStateKey.DependencyProperty;
            BackButtonStateProperty           = BackButtonStateKey.DependencyProperty;
            FinishButtonStateProperty         = FinishButtonStateKey.DependencyProperty;
            IsLastErrorVisibleProperty        = IsLastErrorVisibleKey.DependencyProperty;

            // Define the events
            ActivePageChangedEvent            = EventManager.RegisterRoutedEvent("ActivePageChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<WizardPage>), typeof(WizardPage));
            ActiveIndexChangedEvent           = EventManager.RegisterRoutedEvent("ActiveIndexChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(WizardPage));

            // Register the event handles
            EventManager.RegisterClassHandler(typeof(WizardControl), WizardPage.PageActivatingEvent, new EventHandler<WizardPageChangingEventArgs>(OnPageActivatingThunk));
            EventManager.RegisterClassHandler(typeof(WizardControl), WizardPage.PageDeactivatingEvent, new EventHandler<WizardPageChangingEventArgs>(OnPageDeactivatingThunk));
            EventManager.RegisterClassHandler(typeof(WizardControl), WizardPage.PageActivatedEvent, new EventHandler<WizardPageChangedEventArgs>(OnPageActivatedThunk));
            EventManager.RegisterClassHandler(typeof(WizardControl), WizardPage.PageDeactivatedEvent, new EventHandler<WizardPageChangedEventArgs>(OnPageDeactivatedThunk));

            // Override the keyboard navigation
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(WizardControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(WizardControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(WizardControl), new FrameworkPropertyMetadata(false));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected WizardControl()
        {
            // Initialise the instance properties
            m_History          = new Stack<int>();
            m_IsActiveChanging = false;
            m_IsConsistant     = false;
            Pages              = new CompoundCollectionView(this);

            // We want to know when the pages change
            Pages.CollectionChanged += OnPagesChanged;

            // Define the bindings
            CommandBindings.Add(new CommandBinding(WizardCommands.MoveTo,            OnCommandExecute, OnCommandConfig));
            CommandBindings.Add(new CommandBinding(WizardCommands.Next,              OnCommandExecute, OnCommandConfig));
            CommandBindings.Add(new CommandBinding(WizardCommands.Back,              OnCommandExecute, OnCommandConfig));
            CommandBindings.Add(new CommandBinding(WizardCommands.Cancel,            OnCommandExecute, OnCommandConfig));
            CommandBindings.Add(new CommandBinding(WizardCommands.Finish,            OnCommandExecute, OnCommandConfig));
            CommandBindings.Add(new CommandBinding(WizardCommands.CloseLastError,    OnCommandExecute, OnCommandConfig));
            CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack,    OnCommandExecute, OnCommandConfig));
            CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseForward, OnCommandExecute, OnCommandConfig));
        }

        #endregion
        

        #region --- Protected Overrides ---

       /// <summary>
        /// The <see cref="IsFinishAlwaysVisible"/> has changed we need to ensure that all the buttons have the correct state.
        /// </summary>
        /// <param name="oldValue">The old value of the flag</param>
        /// <param name="newValue">The new value of the flag</param>
        protected virtual void OnIsFinishAlwaysVisibleChanged( bool oldValue, bool newValue )
        {
            UpdateButtonState();
        }

        /// <summary>
        /// Called when the <see cref="ActivePage"/> has changed.
        /// </summary>
        /// <remarks>
        /// When the <see cref="ActivePage"/> changes the <see cref="ActiveIndex"/> must be kept synchronised. 
        /// </remarks>
        /// <param name="oldValue">The old wizard page</param>
        /// <param name="newValue">The new wizard page</param>
        protected virtual void OnActivePageChanged( WizardPage oldValue, WizardPage newValue )
        {
            // Are we currently changing the active page
            if (!m_IsActiveChanging)
            {
                try
                {
                    // Set the changing flag so we do not get infinate recursion
                    m_IsActiveChanging = true;

                    // Set the active index based on the supplied item
                    ActiveIndex = Pages.IndexOf(newValue);


                    // Update the buttons
                    this.UpdateButtonState();
                }
                finally
                {
                    // Finished updating
                    m_IsActiveChanging = false;
                }
            }

            // Change the states
            if (oldValue != null)
            {
                oldValue.IsActive = false;
            }
            if (newValue != null)
            {
                newValue.IsActive = true;
            }

            // Do the transition
            if (m_TransitionPresenter != null)
            {
                TransitionDirection direction = (m_LastChangeType == WizardPageChangeType.NavigateBack)? TransitionDirection.Backwards : TransitionDirection.Forwards;

                // if there was no previous page then transition immediately
                if (oldValue == null)
                {
                    direction = TransitionDirection.Immediate;
                }

                // Do the transition
                m_TransitionPresenter.DoTransition(newValue, direction);
            }

            // Trigger the event
            RaiseEvent(new RoutedPropertyChangedEventArgs<WizardPage>(oldValue, newValue, ActivePageChangedEvent));
        }

        /// <summary>
        /// Called when the <see cref="ActiveIndex"/> has changed.
        /// </summary>
        /// <remarks>
        /// When the <see cref="ActiveIndex"/> changes the <see cref="ActivePage"/> must be kept synchronised. 
        /// </remarks>
        /// <param name="oldIndex">The index of the old page</param>
        /// <param name="newIndex">The index of the new page</param>
        protected virtual void OnActiveIndexChanged( int oldIndex, int newIndex )
        {
            // Are we currently changing the active page
            if (!m_IsActiveChanging)
            {
                try
                {
                    // Set the changing flag so we do not get infinate recursion
                    m_IsActiveChanging = true;

                    // What is the page
                    ActivePage = ((ActiveIndex < 0) || (ActiveIndex >= Pages.Count))? null : Pages[ActiveIndex] as WizardPage;

                    // Update the buttons
                    this.UpdateButtonState();
                }
                finally
                {
                    // Finished updating
                    m_IsActiveChanging = false;
                }
            }
 
            // Trigger the event
            RaiseEvent(new RoutedPropertyChangedEventArgs<int>(oldIndex, newIndex, ActiveIndexChangedEvent));
       }
        
        /// <summary>
        /// Called when the <see cref="PagesSource"/> property changes.
        /// </summary>
        /// <param name="oldSource">The source of the old <see cref="Pages"/> collection</param>
        /// <param name="newSource">The source of the new <see cref="Pages"/> collection</param>
        protected virtual void OnPagesSourceChanged( IEnumerable oldSource, IEnumerable newSource )
        {
            Pages.SetSource(newSource);
        }

        /// <summary>
        /// Called when the pages collection has changed.
        /// </summary>
        /// <remarks>
        /// When the pages collection changes we need to make sure that the <see cref="ActivePage"/>
        /// and <see cref="ActiveIndex"/> are valid.
        /// </remarks>
        /// <param name="sender">The pages collection that has changed.</param>
        /// <param name="args">The information about how the collection changed.</param>
        [SecurityCritical]
        protected virtual void OnPagesChanged( object sender, NotifyCollectionChangedEventArgs args )
        {
             // Are we in a consistant state
            if (!m_IsConsistant)
            {
                object value = base.ReadLocalValue(ActiveIndexProperty);
                int    index = (value == DependencyProperty.UnsetValue) ? 0 : (int)value;

                // Check the value
                if ((index >= 0) && (index < Pages.Count))
                {
                    // Set the active index
                    ActiveIndex = index;

                    // We are now consistant
                    m_IsConsistant = true;
                }
            }

            // If we are consistant ensure that the button state is correct
            if (m_IsConsistant)
            {
                UpdateButtonState();
            }

            // If a remove tirggered this event then make sure the active page is valid
            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                if (ActiveIndex >= args.OldStartingIndex)
                {
                    ActiveIndex = Math.Max(Math.Min(args.OldStartingIndex, Pages.Count - 1), -1);
                    ActivePage  = (ActiveIndex == -1)? null : Pages[ActiveIndex] as WizardPage;
                }
                else if (Pages.Count == 0)
                {
                    ActiveIndex = -1;
                    ActivePage  = null;
                }
                else
                {
                    // NoChange
                }
            }
            else
            {
                ActiveIndex = Pages.IndexOf(ActivePage);
            }

            // Ensure that the pages know their parent
            if (args.OldItems != null)
            {
                args.OldItems.OfType<WizardPage>().ForEach(p => p.ParentWizard = null);
            }
            if (args.NewItems != null)
            {
                args.NewItems.OfType<WizardPage>().ForEach(p => p.ParentWizard = this);
            }

        }
        
        /// <summary>
        /// The transition has finished - remove any border that we added
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnBeginFrameAnimation( object sender, TransitionAnimationEventArgs args )
        {
        }

        /// <summary>
        /// The transition is starting - add a border so that we can see the edge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEndFrameAnimation( object sender, TransitionAnimationEventArgs args )
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

        #region --- Standard overrides ---

        /// <summary>
        /// We have been initialised - hook into the control
        /// </summary>
        /// <param name="e"></param>
        [SecuritySafeCritical]
        protected override void OnInitialized( EventArgs e )
        {
            // Ensure taht the events are handled
            base.OnInitialized(e);

            // Active index is always 0 on initialisation
            ActiveIndex = 0;

            // If we have an active page then move focus
            if (ActivePage != null)
            {
                this.FocusPage(ActivePage);
            }
        }


        /// <summary>
        /// This is our chance to find the key elements of the control
        /// </summary>
        [SecuritySafeCritical]
        public override void OnApplyTemplate()
        {
            // Call the default
            base.OnApplyTemplate();

            // Get the transition control
            m_TransitionPresenter = GetTemplateChild("PART_PageHost") as TransitionPresenter;

            // Did we find it
            if (m_TransitionPresenter != null)
            {
                m_TransitionPresenter.BeginFrameAnimation += OnBeginFrameAnimation;
                m_TransitionPresenter.EndFrameAnimation   += OnEndFrameAnimation;
                m_TransitionPresenter.DoTransition(ActivePage, TransitionDirection.Immediate);
            }
        }

        /// <summary>
        /// Custom logic stops key events whilst the wizard is in transitions. 
        /// </summary>
        /// <param name="e">The information about which key has been pressed.</param>
        [SecuritySafeCritical]
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if ((m_TransitionPresenter == null) || !m_TransitionPresenter.IsInTransition)
            {
                base.OnPreviewKeyDown(e);
            }
        }

        /// <summary>
        /// Custom logic stops key events whilst the wizard is in transitions. 
        /// </summary>
        /// <param name="e">The information about which key has been released.</param>
        [SecuritySafeCritical]
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if ((m_TransitionPresenter == null) || !m_TransitionPresenter.IsInTransition)
            {
                base.OnPreviewKeyUp(e);
            }
        }


        #endregion

        #region --- Public Properties ---
 
        /// <summary>
        /// Gets or sets the effect to be used for the transition betweem pages. This is a dependancy property.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public TransitionEffect TransitionEffect
        {
            get { return (TransitionEffect) GetValue(TransitionEffectProperty); }
            set { SetValue(TransitionEffectProperty, value); }
        }  

        /// <summary>
        /// Gets or sets whether the finish button is always visible. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// The is a key property in determining how the wizard behaves. It will affect
        /// the visibility of the Next and Finish buttons for any given wizard page. 
        /// </para>
        /// <para>
        /// If <c>IsFinishAlwaysVisible</c> is set to <c>True</c> then the finish button will always be visible; 
        /// it will be enabled if the active page has the <see cref="WizardPage.IsFinishEnabled"/> attached property set to <c>True</c>.
        /// </para>
        /// <para>
        /// If <c>IsFinishAlwaysVisible</c> is set to <c>False</c> and the current page is a final page or
        /// it has the <see cref="WizardPage.IsFinalPage"/> attached property set to <c>True</c> then the Next button
        /// will be replaced by the Finish button; for all other pages the Finish button will be hidden.
        /// then the finish button will be hidden for
        /// all apart from a final page. On final pages it will take the place of the Next button if the active page is the last page or 
        /// set to <c>True</c>. It will then be enabled if the active page has the <see cref="WizardPage.IsFinishEnabled"/> 
        /// attached property set to <c>True</c>. If <see cref="WizardPage.IsFinishEnabled"/> is unset then the value of 
        /// <see cref="WizardPage.IsNextEnabled"/> is used to define the state of the finish button. This is because in this context
        /// the finish button is behaving in the same way as the Next button.
        /// </para>
        /// </remarks>
        [Bindable(true), Category("Behavior")]
        public bool IsFinishAlwaysVisible
        {
            get { return (bool)GetValue(IsFinishAlwaysVisibleProperty); }
            set { SetValue(IsFinishAlwaysVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a collection used as the source of pages for the <see cref="WizardControl"/>. This is a dependency property. 
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public IEnumerable PagesSource
        {
            get { return (IEnumerable)GetValue(PagesSourceProperty); }
            set { SetValue(PagesSourceProperty, value); }
        }        
        
        /// <summary>
        /// Gets the index of the currently active page of the wizard. This is a dependency property. 
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ActiveIndex
        {
            get { return (int)GetValue(ActiveIndexProperty); }
            set { SetValue(ActiveIndexProperty, value);  }
        }

        /// <summary>
        /// Gets the curently active page of the wizard. This is a dependency property. 
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WizardPage ActivePage
        {
            get { return (WizardPage)GetValue(ActivePageProperty); }
            set { SetValue(ActivePageProperty, value);  }
        }

        /// <summary>
        /// Gets the text of the last error message that occurred. This is a dependency property. 
        /// </summary>
        /// <remaks>
        /// This property is set as a result of response to page navigation events.
        /// </remaks>
        /// <seealso cref="WizardPage.PageActivating"/>
        /// <seealso cref="WizardPage.PageDeactivating"/>
        [Browsable(false)]
        public string LastError
        {
            get { return (string)GetValue(LastErrorProperty); }
            internal set 
            { 
                SetValue(LastErrorKey, value); 
            }
        }

        /// <summary>
        /// Gets whether the last error message is currently visible. This is a dependency property. 
        /// </summary>
        /// <remaks>
        /// This property is set as a result of response to page navigation events.
        /// </remaks>
        /// <seealso cref="WizardPage.PageActivating"/>
        /// <seealso cref="WizardPage.PageDeactivating"/>
        [Browsable(false)]
        public bool IsLastErrorVisible
        {
            get { return (bool)GetValue(IsLastErrorVisibleProperty); }
            internal set 
            { 
                SetValue(IsLastErrorVisibleKey, value); 
            }
        }

        /// <summary>
        /// Gets the state of the Next button. This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// The property is used by the styles to determine the state of the Next button
        /// </remarks>
        [Browsable(false)]
        [ReadOnly(true)]
        public WizardButtonState NextButtonState
        {
            get { return (WizardButtonState)GetValue(NextButtonStateProperty); }
            internal set 
            { 
                SetValue(NextButtonStateKey, value); 
            }
        }

        /// <summary>
        /// Gets the state of the Back button. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// The property is used by the styles to determine the state of the Back button
        /// </remarks>
        [Browsable(false)]
        [ReadOnly(true)]
        public WizardButtonState BackButtonState
        {
            get { return (WizardButtonState)GetValue(BackButtonStateProperty); }
            internal set 
            { 
                SetValue(BackButtonStateKey, value); 
            }
        }

        /// <summary>
        /// Gets the state of the Finish button. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// The property is used by the styles to determine the state of the Finish button
        /// </remarks>
        [Browsable(false)]
        [ReadOnly(true)]
        public WizardButtonState FinishButtonState
        {
            get { return (WizardButtonState)GetValue(FinishButtonStateProperty); }
            internal set 
            { 
                SetValue(FinishButtonStateKey, value); 
            }
        }
        
        /// <summary>
        /// Gets the collection used as the source of pages for the <see cref="WizardControl"/>.
        /// </summary>
        public  CompoundCollectionView          Pages               { get; private set; }
        
        /// <summary>
        /// Gets the list of pages that represents the pages that have already been seen. 
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        public ReadOnlyCollection<WizardPage>   History
        {
            get { return new ReadOnlyCollection<WizardPage>(m_History.Select(i => Pages[i]).Cast<WizardPage>().ToList()); }
        }

        /// <summary>
        /// Occurs when the <see cref="ActivePage"/> changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<WizardPage> ActivePageChanged
        {
            add { AddHandler(ActivePageChangedEvent, value); }
            remove { RemoveHandler(ActivePageChangedEvent, value); }
        }

        /// <summary>
        /// Occurs when the <see cref="ActiveIndex"/> changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> ActiveIndexChanged
        {
            add { AddHandler(ActiveIndexChangedEvent, value); }
            remove { RemoveHandler(ActiveIndexChangedEvent, value); }
        }

        #endregion

        #region --- Private Event Handlers ---
        

        /// <summary>
        /// Coerce the active page into a valid value
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceActivePage( DependencyObject target, object value )
        {
            WizardControl wizardControl = target as WizardControl;

            // Do we have a valid value
            if (value != null)
            {
                if (!wizardControl.Pages.Contains(value as WizardPage))
                {
                    value = DependencyProperty.UnsetValue;
                }
            }

            return value;
        }

        /// <summary>
        /// Validate active index into a valid value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ValidateActiveIndex( object value )
        {
            return (((int) value) >= -1);
        }

        /// <summary>
        /// Coerce the active index into a valid value
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceActiveIndex( DependencyObject target, object value )
        {
            WizardControl wizardControl = target as WizardControl;

            // Check the value
            if ((value is int) && (((int) value) >= wizardControl.Pages.Count))
            {
                return DependencyProperty.UnsetValue;
            }

            return value;
        }

      
        /// <summary>
        /// Static method to trigger the instance <see cref="OnActivePageChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnActivePageChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as WizardControl).OnActivePageChanged((WizardPage)args.OldValue, (WizardPage)args.NewValue);
        }
    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnActiveIndexChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnActiveIndexChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as WizardControl).OnActiveIndexChanged((int)args.OldValue, (int)args.NewValue);
        }
    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnIsFinishAlwaysVisibleChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnIsFinishAlwaysVisibleChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as WizardControl).OnIsFinishAlwaysVisibleChanged((bool)args.OldValue, (bool)args.NewValue);
           
        }
    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnPagesSourceChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnPagesSourceChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as WizardControl).OnPagesSourceChanged(args.OldValue as IEnumerable, args.NewValue as IEnumerable);
        }
        
        /// <summary>
        /// Static method to trigger the instance <see cref="OnPageActivating"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnPageActivatingThunk( object target, WizardPageChangingEventArgs args )
        {
            (target as WizardControl).OnPageActivating(args);
        }
        
        /// <summary>
        /// Static method to trigger the instance <see cref="OnPageDeactivating"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnPageDeactivatingThunk( object target, WizardPageChangingEventArgs args )
        {
            (target as WizardControl).OnPageDeactivating(args);
        }

        /// <summary>
        /// Static method to trigger the instance <see cref="OnPageActivated"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnPageActivatedThunk( object target, WizardPageChangedEventArgs args )
        {
            (target as WizardControl).OnPageActivated(args);
        }

        /// <summary>
        /// Static method to trigger the instance <see cref="OnPageDeactivated"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnPageDeactivatedThunk( object target, WizardPageChangedEventArgs args )
        {
            (target as WizardControl).OnPageDeactivated(args);
        }
        
        #endregion
        
        #region --- Private Event Raisers ---

        /// <summary>
        /// Raise the page activating event
        /// </summary>
        private static void RaisePageActivating( WizardPage page, RoutedEventArgs args )
        {
            // Define the routed event
            args.RoutedEvent = WizardPage.PageActivatingEvent;

            // Raise the event
            page.RaiseEvent(args);
        }

        /// <summary>
        /// Raise the page activated event
        /// </summary>
        private static void RaisePageActivated( WizardPage page, RoutedEventArgs args )
        {
            // Define the routed event
            args.RoutedEvent = WizardPage.PageActivatedEvent;
            
            // Raise the event
            page.RaiseEvent(args);
        }

        /// <summary>
        /// Raise the page deactivating event
        /// </summary>
        private static void RaisePageDeactivating( WizardPage page, RoutedEventArgs args )
        {
            // Define the routed event
            args.RoutedEvent = WizardPage.PageDeactivatingEvent;

            // Raise the event
            page.RaiseEvent(args);
        }

        /// <summary>
        /// Raise the page deactivating event
        /// </summary>
        private static void RaisePageDeactivated( WizardPage page, RoutedEventArgs args )
        {
            // Define the routed event
            args.RoutedEvent = WizardPage.PageDeactivatedEvent;

            // Raise the event
            page.RaiseEvent(args);
        }

        #endregion

        #region --- Command Handlers ---

        /// <summary>
        /// Execute the wizard command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Window parentWindow = this.FindVisualAncestor<Window>();

            // Act on the command
            if (e.Command == WizardCommands.CloseLastError)
            {
                // Last error is now hidden
                IsLastErrorVisible = false;

                // All handled
                e.Handled = true;
            }
            else if ((e.Command == WizardCommands.Next) || (e.Command == NavigationCommands.BrowseForward))
            {
                // Navigate forwards
                int oldIndex = ActiveIndex;
                int newIndex = Math.Min(oldIndex + 1, Pages.Count - 1);

                // Are we actually moving
                if (newIndex != oldIndex)
                {
                    // Add to the history
                    m_History.Push(oldIndex);

                    // Move to the new page
                    if (!MoveToPage(Pages[newIndex] as WizardPage, WizardPageChangeType.NavigateNext))
                    {
                        // Failed remove page from history
                        m_History.Pop();
                    }
                }

                // Event was handled
                e.Handled = true;
            }
            else if ((e.Command == WizardCommands.Back) || (e.Command == NavigationCommands.BrowseBack))
            {
                // Navigate backwards
                if (m_History.Count > 0)
                {
                    MoveToPage(Pages[m_History.Pop()] as WizardPage, WizardPageChangeType.NavigateBack);
                }
                
                // Event was handled
                e.Handled = true;
            }
            else if (e.Command == WizardCommands.Finish)
            {
                // if we have a valid parent and can move to null (i.e. finish) then close the dialog
                if ((parentWindow != null) && MoveToPage(null, WizardPageChangeType.NavigateFinish))
                {
                    try
                    {
                        parentWindow.DialogResult = true;
                    }
                    catch ( InvalidOperationException )
                    {
                        // most likely the dialog was not show with ShowDialog.
                        parentWindow.Close();
                    }                
                }              
                
                // Event was handled
                e.Handled = true;
            }
            else if (e.Command == WizardCommands.Cancel)
            {
                // If we have a valid parent window then trigger a close
                if (parentWindow != null)
                {
                    try
                    {
                        parentWindow.DialogResult = false;
                    }
                    catch ( InvalidOperationException )
                    {
                        // most likely the dialog was not show with ShowDialog.
                        parentWindow.Close();
                    }
                }
                
                // Event was handled
                e.Handled = true;
            }
            else if (e.Command == WizardCommands.MoveTo)
            {
                WizardPage page = Pages.OfType<WizardPage>().Where(p => (p.Name == e.Parameter.ToString())).FirstOrDefault();

                // Navigate to a specific page
                if (page != null)
                {
                    int oldIndex = ActiveIndex;

                    // try to move to the page
                    if (MoveToPage(page, WizardPageChangeType.NavigateTo))
                    {
                        // Add to the history
                        m_History.Push(oldIndex);
                    }
                }
            }
            else
            {
                e.Handled    = false;
            }

            // This should be final update
            UpdateButtonState();
        }

        /// <summary>
        /// Configure the commands used by the wizard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCommandConfig( object sender, CanExecuteRoutedEventArgs e )
        {
            if (e.Command == WizardCommands.CloseLastError)
            {
                e.CanExecute = IsLastErrorVisible;
                e.Handled    = true;
            }
            else if ((e.Command == WizardCommands.Next) || (e.Command == NavigationCommands.BrowseForward))
            {
                e.CanExecute = (NextButtonState != null) && NextButtonState.IsEnabled;
                e.Handled    = true;
            }
            else if ((e.Command == WizardCommands.Back) || (e.Command == NavigationCommands.BrowseBack))
            {
                e.CanExecute = (BackButtonState != null) && BackButtonState.IsEnabled;
                e.Handled    = true;
            }
            else if (e.Command == WizardCommands.Finish)
            {
                e.CanExecute = (FinishButtonState != null) && FinishButtonState.IsEnabled;
                e.Handled    = true;
            }
            else if (e.Command == WizardCommands.Cancel)
            {
                e.CanExecute = true;
                e.Handled    = true;
            }
            else if (e.Command == WizardCommands.MoveTo)
            {
                WizardPage page = null;
                
                if (e.Parameter is string)
                {
                    page = Pages.OfType<WizardPage>().Where(p => (p.Name == e.Parameter.ToString())).FirstOrDefault();
                }
                else
                {
                    page = e.Parameter as WizardPage;
                }

                e.CanExecute = (page != null);
                e.Handled    = true;
            }
            else
            {
                e.Handled    = false;
            }
        }

        #endregion

        #region --- Internal Helpers ---

        /// <summary>
        /// Ensure that the the pages are set to the correct style.
        /// </summary>
        /// <remarks>
        /// If <b>null</b> is supplied for the style then the style is obtained by searching for the 
        /// resource by the <paramtyperef name="pageT"/> type.
        /// </remarks>
        /// <typeparam name="pageT">The type of page to update.</typeparam>
        /// <param name="style">The style to be applied to the pages</param>
        internal protected void UpdatePageStyles<pageT>( Style style ) where pageT : WizardPage
        {
            Style pageStyle = style ?? TryFindResource(typeof(pageT)) as Style;

            if (pageStyle != null)
            {
                Pages.OfType<pageT>().Where(p => p.Style != pageStyle).ForEach(p => p.Style = pageStyle);
            }
        }

        /// <summary>
        /// Update the buttons at the bottom of the control based on the state of the control
        /// </summary>
        internal void UpdateButtonState()
        {
            bool       isNextEnabled   = false;
            bool       isBackEnabled   = false;
            bool       isFinishEnabled = false;
            bool       isFinalPage     = false;
            bool       hasHistory      = (m_History.Count > 0);
            string     nextLabel       = BrokenHouse.Resources.Strings.WizardControl_Next;
            string     backLabel       = BrokenHouse.Resources.Strings.WizardControl_Back;
            string     finishLabel     = BrokenHouse.Resources.Strings.WizardControl_Finish;

            // If we have an active page obtain the states from the attached properties on the page
            if ((ActiveIndex >= 0) && (ActiveIndex < Pages.Count))
            {
                WizardPage currentPage = Pages[ActiveIndex] as WizardPage;

                // Do we have a current page
                if (currentPage != null)
                {
                    isNextEnabled   = currentPage.IsNextEnabled ?? true;
                    isBackEnabled   = currentPage.IsBackEnabled ?? true;
                    isFinalPage     = currentPage.IsFinalPage || (ActiveIndex + 1 == Pages.Count);
                    isFinishEnabled = currentPage.IsFinishEnabled ?? (isFinalPage? isNextEnabled : false);

                    // Pull our any non standard label
                    if (currentPage.NextLabel != null)
                    {
                        nextLabel = currentPage.NextLabel;
                    }
                    if (currentPage.BackLabel != null)
                    {
                        backLabel = currentPage.BackLabel;
                    }
                    if (currentPage.FinishLabel != null)
                    {
                        finishLabel = currentPage.FinishLabel;
                    }
                }
            }

            // Take a first stab at the states
            WizardButtonState finishButtonState = new WizardButtonState(finishLabel);
            WizardButtonState nextButtonState   = new WizardButtonState(nextLabel);
            WizardButtonState backButtonState   = new WizardButtonState(backLabel, isBackEnabled && hasHistory);

            // Handle the visibility of the next button & finish buttons
            if (IsFinishAlwaysVisible)
            {
                finishButtonState.Visibility = Visibility.Visible;
                finishButtonState.IsDefault  = false;

                nextButtonState.Visibility   = Visibility.Visible;
                nextButtonState.IsDefault    = true;
            }
            else 
            {
                finishButtonState.Visibility = isFinalPage? Visibility.Visible : Visibility.Collapsed;
                finishButtonState.IsDefault  = isFinalPage;
                nextButtonState.Visibility   = isFinalPage? Visibility.Collapsed : Visibility.Visible;
                nextButtonState.IsDefault    = !isFinalPage;
            }

            // If next has a value then use it - otherwise it is the opposite of the last page flag.
            nextButtonState.IsEnabled   = isNextEnabled && !isFinalPage;            
            finishButtonState.IsEnabled = isFinishEnabled; 
   
            // Update the states
            NextButtonState   = nextButtonState;
            FinishButtonState = finishButtonState;
            BackButtonState   = backButtonState;

            // We want the commands to be updated
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// This function will try to move to the supplied new page. However, there are a number 
        /// of things that may affect this. The deactivating page can change which page we should
        /// navigate to. The destination page can change the page we navigate to. When all the 
        /// deactivating and activating events have triggered the old page will be triggered
        /// with a deactivated event and the new page will be triggered with a activated event.
        /// </summary>
        /// <param name="newPage">The new page, (null if finish is requrested)</param>
        /// <param name="changeType">The type of change</param>
        /// <returns></returns>
        internal bool MoveToPage( WizardPage newPage, WizardPageChangeType changeType )
        {
            WizardPage                  oldPage      = Pages[ActiveIndex] as WizardPage;
            WizardPageChangingEventArgs changingArgs = new WizardPageChangingEventArgs(newPage, oldPage, changeType);

            // Clear the error
            IsLastErrorVisible = false;

            // If any exception occurs we cancel the change
            try
            {
                // If we have an old page then we need to check to see if we can deactivate
                if (oldPage != null)
                {
                    RaisePageDeactivating(oldPage, changingArgs);
                }

                // Are we allowed to deactivate
                if (changingArgs.IsChangeAllowed)
                {
                    // If we are we have to keep checking to see if we 
                    // can activate the new page. It is important to note
                    // that both activating and deactivating can change
                    // the target page.
                    while (changingArgs.NewPage != null)
                    {
                        WizardPage loopPage = changingArgs.NewPage;

                        // Can we activate this new page - we need to ask it?
                        RaisePageActivating(loopPage, changingArgs);

                        // Act on the response
                        if (!changingArgs.IsChangeAllowed)
                        {
                            break;
                        }
                        else if (loopPage == changingArgs.NewPage)
                        {
                            // page has not been changed by the handler - thats it.
                            break;
                        }
                        else
                        {
                            // NewPage has changed - keep looping
                        }
                    }
                }
            }
            catch (TargetInvocationException e)
            {
                changingArgs.CancelChange(e.InnerException.Message);
            }
            catch (Exception e)
            {
                changingArgs.CancelChange(e.Message);
            }

            // Are we allowed to change page
            if (!changingArgs.IsChangeAllowed)
            {
                // No - go back to the old page
                newPage = oldPage;

                // Set the reason
                LastError = changingArgs.CancelReason;
                IsLastErrorVisible = true;
            }
            else
            {
                // Obtain the new page from the event args
                newPage = changingArgs.NewPage;

                // Create the change args
                WizardPageChangedEventArgs changedArgs = new WizardPageChangedEventArgs(newPage, oldPage, changeType);

                // Do we have an old page
                if (oldPage != null)
                {
                    // Yes - Tell it that it has been deactivated event
                    RaisePageDeactivated(oldPage, changedArgs);
                }

                // Do we have a new page. If new page is null then we are finishing
                if (newPage != null)
                {
                    // Define which way the transition should go.
                    m_LastChangeType = changeType;
                    ActiveIndex = Pages.IndexOf(newPage);

                    // Raise the activated event
                    RaisePageActivated(newPage, changedArgs);
                }

                // Update the display
                UpdateButtonState();
            }

            // Lets try and select the page
            if (newPage != null)
            {
                FocusPage(newPage);
            }

            // Return true if the page has changed and it was a successful move
            return (newPage != oldPage);
        }

        /// <summary>
        /// Move focus to the supplied page. This function is called repeatedly with the dispatcher
        /// to ensure focus has been moved. If this has just been made the active page then it will
        /// not be visible. We have to wait untill it is visible before we can set the focus.
        /// </summary>
        /// <param name="page"></param>
        public void FocusPage( WizardPage page )
        {
            if (!page.IsVisible)
            {
                Action action = delegate { FocusPage(page); };

                Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
            }
            else
            {
                // Move focus to the first item on the page
                page.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        #endregion

        #region --- ICollectionViewModelParent Members ---
        
        /// <summary>
        /// Allow a item to be removed as a logical child.
        /// </summary>
        /// <param name="item"></param>
        [SecurityCritical]
        void ICollectionViewModelParent.RemoveModelItem(object item)
        {
            RemoveLogicalChild(item);
        }

        /// <summary>
        /// Allow an item to be added as a logical child.
        /// </summary>
        /// <param name="item"></param>
        [SecurityCritical]
        void ICollectionViewModelParent.AddModelItem(object item)
        {
            AddLogicalChild(item);
        }

        #endregion

    }

    #region --- Helper class ---

    /// <summary>
    /// This helper class helps manage the state of a wizard button.
    /// </summary>
    public sealed class WizardButtonState
    {
        /// <summary>
        /// Construct a default state object that is hidden
        /// </summary>
        internal WizardButtonState( string label ) : this(label, Visibility.Hidden, false, false)
        {
        }

        /// <summary>
        /// Construct a default state state that is enabled based on the supplied flag
        /// </summary>
        internal WizardButtonState( string label, bool isEnabled ) : this(label, Visibility.Visible, isEnabled, false)
        {
        }
        
        /// <summary>
        /// Construct the state
        /// </summary>
        /// <param name="label">The label to use in the button.</param>
        /// <param name="visibility">The visibility of the button.</param>
        /// <param name="isEnabled">The flag indicating that the button is enabled.</param>
        /// <param name="isDefault">The flag indicating that the button is the default button.</param>
        internal WizardButtonState( string label, Visibility visibility, bool isEnabled, bool isDefault )
        {
            Label      = label;
            Visibility = visibility;
            IsEnabled  = isEnabled;
            IsDefault  = isDefault;
        }

        /// <summary>
        /// The label of the button
        /// </summary>
        public string Label { get; internal set; }

        /// <summary>
        /// The visibility of the button
        /// </summary>
        public Visibility Visibility { get; internal set; }

        /// <summary>
        /// The enabled flag of the button
        /// </summary>
        public bool IsEnabled { get; internal set; }

        /// <summary>
        /// The default flag of the button
        /// </summary>
        public bool IsDefault { get; internal set; }
    }

    #endregion
}
