using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrokenHouse.Windows.Parts.Transition.Primitives;
using BrokenHouse.Windows.Data;
using BrokenHouse.Extensions;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// Provides a control to transition between 
    /// </summary>
    [DefaultProperty("Items")]
    [ContentProperty("Items")]
    public class TransitionItemsControl : Control, ICollectionViewModelParent
    {
        /// <summary>
        /// Identifies the <see cref="ActiveItem"/> dependency property key. 
        /// </summary>
        public static DependencyProperty     ActiveItemProperty;
        /// <summary>
        /// Identifies the <see cref="ActiveIndex"/> dependency property key. 
        /// </summary>
        public static DependencyProperty     ActiveIndexProperty;
        /// <summary>
        /// Identifies the <see cref="ActiveName"/> dependency property key. 
        /// </summary>
        public static DependencyProperty     ActiveNameProperty;
        /// <summary>
        /// Identifies the <see cref="TransitionEffect"/> dependency property key. 
        /// </summary>
        public static DependencyProperty     TransitionEffectProperty;
        /// <summary>
        /// Identifies the <see cref="TransitionInSequence"/> dependency property key. 
        /// </summary>
        public static DependencyProperty     TransitionInSequenceProperty;
        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property key. 
        /// </summary>
        public static DependencyProperty     ItemsSourceProperty;
        /// <summary>
        /// Identifies the <see cref="ItemStringFormat"/> dependency property. 
        /// </summary>
        public static DependencyProperty     ItemStringFormatProperty;
        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property. 
        /// </summary>
        public static DependencyProperty     ItemTemplateProperty;
        /// <summary>
        /// Identifies the <see cref="ItemTemplateSelector"/> dependency property. 
        /// </summary>
        public static DependencyProperty     ItemTemplateSelectorProperty;

        /// <summary>
        /// The actual presenter of the animation
        /// </summary>
        private TransitionPresenter          m_TransitionPresenter = null;

        /// <summary>
        /// The timer used to run pending animations..
        /// </summary>
        private DispatcherTimer              m_TransitionTimer     = null;

        /// <summary>
        /// The list of pending transitions.
        /// </summary>
        private List<object>                 m_PendingTransitions  = new List<object>();

        /// <summary>
        /// The current target of the animation.
        /// </summary>
        private object                       m_TransitionTarget    = null;

        /// <summary>
        /// The collection view holding the items to transition.
        /// </summary>
        private CompoundCollectionView       m_Items;

        /// <summary>
        /// A flag indicating that the active item is being updated
        /// </summary>
        private bool                         m_UpdatingActiveItem;

        /// <summary>
        /// Static construct to perform the WPF registration
        /// </summary>
        static TransitionItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionItemsControl), new FrameworkPropertyMetadata(typeof(TransitionItemsControl)));

            ItemsSourceProperty          = ItemsControl.ItemsSourceProperty.AddOwner(typeof(TransitionItemsControl), new FrameworkPropertyMetadata(null, OnItemsSourceChangedThunk));
            ItemTemplateProperty         = ItemsControl.ItemTemplateProperty.AddOwner(typeof(TransitionItemsControl), new FrameworkPropertyMetadata(null));
            ItemTemplateSelectorProperty = ItemsControl.ItemTemplateSelectorProperty.AddOwner(typeof(TransitionItemsControl), new FrameworkPropertyMetadata(null));
            ItemStringFormatProperty     = ItemsControl.ItemStringFormatProperty.AddOwner(typeof(TransitionItemsControl), new FrameworkPropertyMetadata(null));

            ActiveIndexProperty          = DependencyProperty.Register("ActiveIndex", typeof(int), typeof(TransitionItemsControl), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnActiveIndexChangedThunk));
            ActiveItemProperty           = DependencyProperty.Register("ActiveItem", typeof(object), typeof(TransitionItemsControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnActiveItemChangedThunk));
            ActiveNameProperty           = DependencyProperty.Register("ActiveName", typeof(string), typeof(TransitionItemsControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal, OnActiveNameChangedThunk));
            TransitionEffectProperty     = DependencyProperty.Register("TransitionEffect", typeof(TransitionEffect), typeof(TransitionItemsControl), new FrameworkPropertyMetadata(null));
            TransitionInSequenceProperty = DependencyProperty.Register("TransitionInSequence", typeof(bool), typeof(TransitionItemsControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        }

        /// <summary>
        /// Constructs a <see cref="TransitionItemsControl"/>.
        /// </summary>
        public TransitionItemsControl()
        {
            m_Items = new CompoundCollectionView(this);

            m_Items.CollectionChanged += OnItemsChanged;

            DataContextChanged += OnDataContextChanged;
        }

        #region --- Properties ---

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.DataTemplate"/> used to present the items being transitioned. This is a dependency property. 
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom logic that will choose the appropriate template used to present each item. This is a dependency property. 
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the string format that will be used to present the items in the list.  This is a dependency property. 
        /// </summary>
        public string ItemStringFormat
        {
            get { return (string)GetValue(ItemStringFormatProperty); }
            set { SetValue(ItemStringFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source of the items in the <see cref="TransitionItemsControl"/>. This is a dependency property. 
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="TransitionEffect"/> used to transition the items. This is a dependency property. 
        /// </summary>
        public TransitionEffect TransitionEffect
        {
            get { return (TransitionEffect)GetValue(TransitionEffectProperty); }
            set { SetValue(TransitionEffectProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the items should be transitioned in sequence. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// When transitioning items in sequence the order of items is maintained. For example,
        /// if item 1 is currently being viewed and item 4 is to be the active item then items
        /// 2 and 3 are transitioned before item 4.
        /// </remarks>
        public bool TransitionInSequence
        {
            get { return (bool)GetValue(TransitionInSequenceProperty); }
            set { SetValue(TransitionInSequenceProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the item that we are displaying or transitioning to.  This is a dependency property. 
        /// </summary>
        public object ActiveItem
        {
            get { return GetValue(ActiveItemProperty); }
            set { SetValue(ActiveItemProperty, value); }
        }
       
        /// <summary>
        /// Gets or sets the name of the item that we are displaying or transitioning to.  This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// If the active item does not have a name then this property will be null. If the transition items control
        /// does not contain the name supplied to it then the active item will be set to null.
        /// </remarks>
        public string ActiveName
        {
            get { return (string)GetValue(ActiveNameProperty); }
            set { SetValue(ActiveNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the item we are displaying or transitioning to.  This is a dependency property. 
        /// </summary>
        public int ActiveIndex
        {
            get { return (int) GetValue(ActiveIndexProperty); }
            set { SetValue(ActiveIndexProperty, value); }
        }

        /// <summary>
        /// Gets the items that are the source of the transitions.
        /// </summary>
        public CompoundCollectionView Items
        {
            get { return m_Items; }
        }

        #endregion

        #region --- Event Handlers ---

        /// <summary>
        /// Called when the control is initialised.
        /// </summary>
        /// <param name="e"></param>
        [SecuritySafeCritical]
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            m_TransitionTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.01), DispatcherPriority.Render, OnTransitionTimer, Dispatcher);
            m_TransitionTimer.Stop();
        }

        /// <summary>
        /// The transition timer has ticked. Update any pending transitions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnTransitionTimer( object sender, EventArgs args )
        {
            ProcessPendingTransitions();
        }

        /// <summary>
        /// The template has been applied. It is our chance to obtain the presenter that will do the actual transitions.
        /// </summary>
        [SecuritySafeCritical]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_TransitionPresenter = GetTemplateChild("PART_TransitionPresenter") as TransitionPresenter;

            ProcessPendingTransitions();
        }
        
        /// <summary>
        /// Called when the data context changes - force an update of the data context on all internal items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SecuritySafeCritical]
        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
 	        m_Items.OfType<FrameworkElement>().ForEach(i => i.DataContext = e.NewValue);
        }

        /// <summary>
        /// Called when the items have changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SecuritySafeCritical]
        protected virtual void OnItemsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (Items.Count > 0)
                {
                    ActiveItem = Items[0];
                }
            }
            else
            {
                // Has the active item been defined
                if ((e.NewItems != null) && (e.NewItems.Count > 0) && (ActiveItem == null))
                {
                    object newActiveItem = null;

                    if (ActiveName != null)
                    {
                        newActiveItem = e.NewItems.OfType<FrameworkElement>().FirstOrDefault(fe => fe.FindName(ActiveName) != null);
                    }
                    else if (ActiveIndex == -1)
                    {
                        newActiveItem = (ActiveIndex < Items.Count)? Items[ActiveIndex] : null;
                    }
                    else
                    {
                        newActiveItem = e.NewItems[0];
                    }

                    // Do we update the active item?
                    if (newActiveItem != null)
                    {
                        ActiveItem = newActiveItem;
                    }

                    e.NewItems.OfType<FrameworkElement>().ForEach(fe => AddLogicalChild(fe));
                }
                if (e.OldItems != null)
                {
                    if (e.OldItems.Contains(ActiveItem))
                    {
                        ActiveItem = (Items.Count > 0)? Items[ActiveIndex - 1] : null;
                    }

                    e.OldItems.OfType<FrameworkElement>().ForEach(fe => RemoveLogicalChild(fe));
                }
            }
        }

        /// <summary>
        /// Add a number of items to the list of pending transitions. Then we process the next transitions in the pending list.
        /// </summary>
        /// <param name="newItems"></param>
        private void AddTransitions(List<object> newItems)
        {
            m_PendingTransitions.AddRange(newItems);

            if ((m_TransitionPresenter != null) && !m_TransitionTimer.IsEnabled)
            {
                ProcessPendingTransitions();
            }
        }

        /// <summary>
        /// Process the next item in the pending transition list.
        /// </summary>
        private void ProcessPendingTransitions()
        {
            if (m_PendingTransitions.Count > 0)
            {
                object oldContent = m_TransitionTarget;
                object newContent = m_PendingTransitions[0];
                int oldIndex = Items.IndexOf(oldContent);
                int newIndex = Items.IndexOf(newContent);

                if ((newIndex != -1) && (m_TransitionPresenter != null))
                {
                    m_TransitionPresenter.DoTransition(newContent, (oldIndex < newIndex) ? TransitionDirection.Backwards : TransitionDirection.Forwards);
                }
                else
                {
                    // Old and new items are not part of the list
                }

                // The next transition target
                m_PendingTransitions.RemoveAt(0);
                m_TransitionTarget = newContent;

                // Do we have any more pending transitions
                if (m_PendingTransitions.Count == 0)
                {
                    m_TransitionTimer.Stop();

                    // Ensure that the current item is correct
                    ActiveItem = m_TransitionTarget;
                }
                else if (!m_TransitionTimer.IsEnabled)
                {
                    m_TransitionTimer.Start();
                }
                else
                {
                    // No Change
                }
            }
        }

        /// <summary>
        /// The active index has changed. Ensure that the active item is set.
        /// </summary>
        /// <param name="oldActiveIndex">Tbe old value of the active index.</param>
        /// <param name="newActiveIndex">The new value of the active index.</param>
        private void OnActiveIndexChanged( int oldActiveIndex, int newActiveIndex )
        {
            int testOldIndex = Items.IndexOf(ActiveItem);

            // Check the update
            if ((testOldIndex == oldActiveIndex) && (Items.Count > 0))
            {
                // Index changed before content
                ActiveItem = Items[Math.Max(0, Math.Min(Items.Count - 1, newActiveIndex))];
            }
        }

        /// <summary>
        /// The active name has changed. Ensure that the active item is set.
        /// </summary>
        /// <param name="oldActiveName">Tbe old value of the active name.</param>
        /// <param name="newActiveName">The new value of the active name.</param>
        private void OnActiveNameChanged( string oldActiveName, string newActiveName )
        {
            if (!m_UpdatingActiveItem)
            {
                object newActiveItem = Items.OfType<FrameworkElement>().FirstOrDefault(e => string.Equals(e.Name, newActiveName));

                // Check the update
                if ((newActiveItem != null) && (newActiveItem != ActiveItem))
                {
                    // Index changed before content
                    ActiveItem = newActiveItem;
                }
            }
        }

        /// <summary>
        /// The active item has changed. Make sure we start transitioning to it.
        /// </summary>
        /// <param name="oldActiveItem">The old active item</param>
        /// <param name="newActiveItem">The new active item</param>
        private void OnActiveItemChanged( object oldActiveItem, object newActiveItem )
        {
            if (!m_UpdatingActiveItem)
            {
                // We are now updating
                m_UpdatingActiveItem = true;

                // Get the index of the new active item
                int newIndex = Items.IndexOf(newActiveItem);

                // Did we find it
                if (newIndex != -1)
                {
                    List<object> items    = new List<object>();
                    int          oldIndex = Items.IndexOf(oldActiveItem);
          
                    if ((oldIndex == -1) || !TransitionInSequence)
                    {
                        items.Add(newActiveItem);
                    }
                    else if (oldIndex < newIndex)
                    {
                        for (int i = oldIndex + 1; i <= newIndex; i++)
                        {
                            items.Add(Items[i]);
                        }
                    }
                    else
                    {
                        for (int i = oldIndex - 1; i >= newIndex; i--)
                        {
                            items.Add(Items[i]);
                        }
                    }

                    AddTransitions(items);
                }
                else
                {
                    // Old and new items are not part of the list
                }

                // Save the new index
                ActiveIndex = newIndex;

                // Finished updating
                m_UpdatingActiveItem = false;
            }
        }
 
        /// <summary>
        /// The items source has changed. Update our collection view.
        /// </summary>
        /// <param name="oldEnumerable"></param>
        /// <param name="newEnumerable"></param>
        private void OnItemsSourceChanged( IEnumerable oldEnumerable, IEnumerable newEnumerable )
        {
            Items.SetSource(newEnumerable);
        }

    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnActiveItemChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnActiveItemChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as TransitionItemsControl).OnActiveItemChanged(args.OldValue, args.NewValue);
        }
    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnActiveIndexChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnActiveIndexChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as TransitionItemsControl).OnActiveIndexChanged((int)args.OldValue, (int)args.NewValue);
        }
    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnActiveNameChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnActiveNameChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as TransitionItemsControl).OnActiveNameChanged((string)args.OldValue, (string)args.NewValue);
        }
    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnItemsSourceChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnItemsSourceChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as TransitionItemsControl).OnItemsSourceChanged((IEnumerable)args.OldValue, (IEnumerable)args.NewValue);
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
}
