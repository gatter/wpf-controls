using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrokenHouse.Windows.Parts.Transition;
using BrokenHouse.Windows.Parts.Transition.Effects;

namespace BrokenHouse.Windows.Parts.Transition.Primitives
{
    /// <summary>
    /// Provides the basic transition functionality to any control. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class should only be used to develop controls that supports transitions. It is used
    /// by <see cref="TransitionPresenter"/>, <see cref="TransitionItemsControl"/> and 
    /// <see cref="BrokenHouse.Windows.Parts.Wizard.WizardControl"/> to provide their transition functionality. Essentially,
    /// any control that requires a more direct manipulation of its content should use this control. For example,
    /// the order of <see cref="BrokenHouse.Windows.Parts.Wizard.WizardPage">WizardPages</see> in the
    /// <see cref="BrokenHouse.Windows.Parts.Wizard.WizardControl"/> is dynamic, the only key factor is that
    /// new pages are transitioned <see cref="TransitionDirection.Forwards"/> and old pages (navigation back) are
    /// transitioned <see cref="TransitionDirection.Backwards"/>. Due to this the lower level of control supplied
    /// by this presenter is ideally suited.
    /// </para>
    /// <para>
    /// Even though this element supports the same functionality as the system <see cref="System.Windows.Controls.ContentPresenter"/>
    /// it does not extend from it because we do not present one item at a time. Depending on the configuration
    /// and context this element could be presenting more than 10 items at any one time.
    /// </para>
    /// </remarks>
    public class TransitionPresenter : FrameworkElement
    {
        #region --- Dependancy Properties & Routed Events ---

        /// <summary>
        /// Identifies the <see cref="TransitionEffect"/> dependency property. 
        /// </summary>
        public static DependencyProperty     TransitionEffectProperty;

        /// <summary>
        /// Identifies the <see cref="ContentStringFormat"/> dependency property. 
        /// </summary>
        public static DependencyProperty     ContentStringFormatProperty;

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property. 
        /// </summary>
        public static DependencyProperty     ContentTemplateProperty;

        /// <summary>
        /// Identifies the <see cref="ContentTemplateSelector"/> dependency property. 
        /// </summary>
        public static DependencyProperty     ContentTemplateSelectorProperty;

        /// <summary>
        /// Identifies the <see cref="IsInTransition"/> dependency property. 
        /// </summary>
        public static DependencyProperty     IsInTransitionProperty;

        /// <summary>
        /// Identifies the <see cref="TransitionFrames"/> dependency property. 
        /// </summary>
        public static DependencyProperty     TransitionFramesProperty;

        /// <summary>
        /// Identifies the <see cref="HorizontalContentAlignment"/> dependency property. 
        /// </summary>
        public static DependencyProperty     HorizontalContentAlignmentProperty;

        /// <summary>
        /// Identifies the <see cref="VerticalContentAlignment"/> dependency property. 
        /// </summary>
        public static DependencyProperty     VerticalContentAlignmentProperty;

        /// <summary>
        /// Identifies the <see cref="IsInTransition"/> dependency property key. 
        /// </summary>
        private static DependencyPropertyKey  IsInTransitionPropertyKey;

        /// <summary>
        /// Identifies the <see cref="TransitionFrames"/> dependency property key. 
        /// </summary>
        private static DependencyPropertyKey  TransitionFramesPropertyKey;

        /// <summary>
        /// Identifies the <see cref="BeginTransition"/> routed event.  
        /// </summary>
        public static RoutedEvent            BeginTransitionEvent;

        /// <summary>
        /// Identifies the <see cref="EndTransition"/> routed event. 
        /// </summary>
        public static RoutedEvent            EndTransitionEvent;

        /// <summary>
        /// Identifies the <see cref="BeginFrameAnimation"/> routed event.  
        /// </summary>
        public static RoutedEvent            BeginFrameAnimationEvent;

        /// <summary>
        /// Identifies the <see cref="EndFrameAnimation"/> routed event. 
        /// </summary>
        public static RoutedEvent            EndFrameAnimationEvent;

        #endregion

        /// <summary>
        /// If content is supplied to early we cannot transition to it. So we save
        /// it and transition to it when we can.
        /// </summary>
        private PendingTransition            m_PendingTransition;

        /// <summary>
        /// THe current item that we are transitioning to.
        /// </summary>
        private object                       m_TargetContent;

        #region --- Constructor ---

        /// <summary>
        /// Static constructor to set up WPF
        /// </summary>
        static TransitionPresenter()
        {
            IsInTransitionPropertyKey          = DependencyProperty.RegisterReadOnly("IsInTransition", typeof(bool), typeof(TransitionPresenter), new FrameworkPropertyMetadata(false));
            TransitionFramesPropertyKey        = DependencyProperty.RegisterReadOnly("TransitionFrames", typeof(UIElementCollection), typeof(TransitionPresenter), new FrameworkPropertyMetadata(null));

            HorizontalContentAlignmentProperty = Control.HorizontalContentAlignmentProperty.AddOwner(typeof(TransitionPresenter), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
            VerticalContentAlignmentProperty   = Control.VerticalContentAlignmentProperty.AddOwner(typeof(TransitionPresenter), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
            ContentTemplateProperty            = ContentPresenter.ContentTemplateProperty.AddOwner(typeof(TransitionPresenter), new FrameworkPropertyMetadata(null, OnContentTemplateChangedThunk));
            ContentTemplateSelectorProperty    = ContentPresenter.ContentTemplateSelectorProperty.AddOwner(typeof(TransitionPresenter), new FrameworkPropertyMetadata(null, OnContentTemplateSelectorChangedThunk));
            ContentStringFormatProperty        = ContentPresenter.ContentStringFormatProperty.AddOwner(typeof(TransitionPresenter), new FrameworkPropertyMetadata(null, OnContentTemplateSelectorChangedThunk));
            TransitionEffectProperty           = DependencyProperty.Register("TransitionEffect", typeof(TransitionEffect), typeof(TransitionPresenter), new FrameworkPropertyMetadata(null, OnTransitionEffectChangedThunk, OnTransitionEffectCoerceThunk));
            IsInTransitionProperty             = IsInTransitionPropertyKey.DependencyProperty;
            TransitionFramesProperty           = TransitionFramesPropertyKey.DependencyProperty;

            BeginTransitionEvent               = EventManager.RegisterRoutedEvent("BeginTransition", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TransitionPresenter));
            EndTransitionEvent                 = EventManager.RegisterRoutedEvent("EndTransition", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TransitionPresenter));

            BeginFrameAnimationEvent           = EventManager.RegisterRoutedEvent("BeginFrameAnimation", RoutingStrategy.Bubble, typeof(EventHandler<TransitionAnimationEventArgs>), typeof(TransitionPresenter));
            EndFrameAnimationEvent             = EventManager.RegisterRoutedEvent("EndFrameAnimation", RoutingStrategy.Bubble, typeof(EventHandler<TransitionAnimationEventArgs>), typeof(TransitionPresenter));
        }

        #endregion

        #region --- Properties ---
        
        /// <summary>
        /// Gets the collection <see cref="TransitionFrame">TransitionFrames</see> currently being animated.
        /// </summary>
        public UIElementCollection TransitionFrames
        {
            get { return (UIElementCollection)GetValue(TransitionFramesProperty); }
            private set { SetValue(TransitionFramesPropertyKey, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the <see cref="TransitionFrame">TransitionFrame's</see> content. 
        /// This is a dependency property. 
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the <see cref="TransitionFrame">TransitionFrame's</see> content. 
        /// This is a dependency property. 
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="TransitionEffect"/> that will define and control the transitions.
        /// This is a dependency property. 
        /// </summary>
        public TransitionEffect TransitionEffect
        {
            get { return (TransitionEffect)GetValue(TransitionEffectProperty); }
            set { SetValue(TransitionEffectProperty, value); }
        }

        /// <summary>
        /// Gets or sets a template selector that enables an application writer to provide custom template-selection logic. 
        /// This is a dependency property.
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }
  

        /// <summary>
        /// Gets or sets the data template used to display the content of the 
        /// <see cref="TransitionPresenter"/>. This is a dependency property.
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }
  
        /// <summary>
        /// Gets or sets the format used to display the string content of the <see cref="TransitionPresenter"/>. This is a dependency property.
        /// </summary>
        public string ContentStringFormat
        {
            get { return (string)GetValue(ContentStringFormatProperty); }
            set { SetValue(ContentStringFormatProperty, value); }
        }

        /// <summary>
        /// Gets the flag that indicates that a transition is in progress.
        /// </summary>
        public bool IsInTransition
        {
            get { return (bool)GetValue(IsInTransitionProperty); }
            private set { SetValue(IsInTransitionPropertyKey, value); }
        }

        /// <summary>
        /// Occurs when a transition starts.
        /// </summary>
        public event RoutedEventHandler BeginTransition
        {
            add { AddHandler(BeginTransitionEvent, value); }
            remove { RemoveHandler(BeginTransitionEvent, value); }
        }

        /// <summary>
        /// Occurs when a transition ends.
        /// </summary>
        public event RoutedEventHandler EndTransition
        {
            add { AddHandler(EndTransitionEvent, value); }
            remove { RemoveHandler(EndTransitionEvent, value); }
        }

        /// <summary>
        /// Occurs when a new frame has been added to the <see cref="TransitionPresenter"/>
        /// and is about to be animated.
        /// </summary>
        public event EventHandler<TransitionAnimationEventArgs> BeginFrameAnimation
        {
            add { AddHandler(BeginFrameAnimationEvent, value); }
            remove { RemoveHandler(BeginFrameAnimationEvent, value); }
        }

        /// <summary>
        /// Occurs when a frame's animation is completed and has been removed from the <see cref="TransitionPresenter"/>.
        /// </summary>
        public event EventHandler<TransitionAnimationEventArgs> EndFrameAnimation
        {
            add { AddHandler(EndFrameAnimationEvent, value); }
            remove { RemoveHandler(EndFrameAnimationEvent, value); }
        }

        #endregion

        #region --- Public Interface ---   
        
        /// <summary>
        /// Triggers the presenter to transition its current content to the supplied content.
        /// </summary>
        /// <remarks>
        /// <para>
        /// As it is the role of the <see cref="TransitionEffect"/> to manage all transitions 
        /// this method will call the <see cref="BrokenHouse.Windows.Parts.Transition.TransitionEffect.DoTransition"/> to initiate
        /// the transition to the new content.
        /// </para>
        /// <para>
        /// If the control is in the process of being initialised then the content is saved
        /// and the transition is carried out at the earliest opportunity.
        /// </para>
        /// </remarks>
        /// <param name="newContent">The new content that should be presented</param>
        /// <param name="direction">The direction in which the content is transitioned.</param>
        public void DoTransition( object newContent, TransitionDirection direction )
        {
            if ((RenderSize.Width > 0) || (RenderSize.Height > 0))
            {
                TransitionEffect.DoTransition(newContent, direction);

                m_TargetContent = newContent;
            }
            else
            {
                m_PendingTransition = new PendingTransition(newContent, direction);
            }
        }
        
        /// <summary>
        /// Stop all active animations
        /// </summary>
        protected void StopAllTransitions()
        {
            if (TransitionEffect != null)
            {
                TransitionEffect.StopAllAnimations();
            }
        }
        
        #endregion

        #region --- Positioning ---

        /// <summary>
        /// Ensures that all the frames measure themselves.
        /// </summary>
        /// <remarks>
        /// This control will take up all the available size; due to this, this method will return the same
        /// <paramref name="availableSize"/> passed to it. It is mainly used to ensure that all the <see cref="TransitionFrames"/>
        /// are measured.
        /// </remarks>
        /// <param name="availableSize">The <see cref="System.Windows.Size"/> of the available area for the transition presenter. </param>
        /// <returns>The same size supplied to the method.</returns>
        [SecuritySafeCritical]
        protected override Size MeasureOverride( Size availableSize )
        {
            Size maxSize = Size.Empty;

            foreach (UIElement element in TransitionFrames)
            {
                element.Measure(availableSize);

                if (maxSize.IsEmpty)
                {
                    maxSize = element.DesiredSize;
                }
                else
                {
                    maxSize.Width  = Math.Max(maxSize.Width, element.DesiredSize.Width);
                    maxSize.Height = Math.Max(maxSize.Height, element.DesiredSize.Height);
                }
            }

            return maxSize;
        }

        /// <summary>
        /// Arranges the <see cref="TransitionFrames"/> so that they fill the available space.
        /// </summary>
        /// <param name="finalSize">The <see cref="System.Windows.Size"/> in which the frames should be arranged.</param>
        /// <returns>The actual <see cref="System.Windows.Size"/> of the transition control.</returns>
        [SecuritySafeCritical]
        protected override Size ArrangeOverride( Size finalSize )
        {
            Rect bounds = new Rect(0, 0, finalSize.Width, finalSize.Height);

            foreach (UIElement element in TransitionFrames)
            {
                element.Arrange(bounds);
                element.InvalidateVisual();
            }

            return finalSize;
        }
        
        #endregion

        #region --- Event Handlers ---

        /// <summary>
        /// The control has been initialised and it is our chance to finish off our initialisation
        /// </summary>
        /// <param name="e"></param>
        [SecuritySafeCritical]
        protected override void OnInitialized( EventArgs e )
        {
            TransitionFrames = new UIElementCollection(this, this);

            // Ensure the event is handled properly
            base.OnInitialized(e);
        }

        /// <summary>
        /// The control is being rendered. Ensure that any pending content is rendered.
        /// </summary>
        /// <param name="drawingContext">An instance of <see cref="System.Windows.Media.DrawingContext"/> used to render the control.</param>
        [SecuritySafeCritical]
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // If we have pending content that transition to it.
            if (m_PendingTransition != null)
            {
                // Do thetransition
                TransitionEffect.DoTransition(m_PendingTransition.Content, m_PendingTransition.Direction);

                // We now have the target
                m_TargetContent = m_PendingTransition.Content;

                // And no pending content
                m_PendingTransition = null;
            }
        }
        
        #endregion

        #region --- Visual Children ---

        /// <summary>
        /// Provides the number of <see cref="TransitionFrames"/> being animated.
        /// </summary>
        protected override int VisualChildrenCount
        {
            [SecuritySafeCritical]
            get
            {
                UIElementCollection frames = TransitionFrames;

                return (frames == null)? 0 : TransitionFrames.Count;
            }
        }

        /// <summary>
        /// Provides access to the <see cref="TransitionFrames"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        protected override Visual GetVisualChild(int index)
        {
            UIElementCollection frames = TransitionFrames;

            return (frames == null)? null : frames[index];
        }
                
        #endregion

        #region --- Event Handlers ---

        /// <summary>
        /// Called when the <see cref="TransitionEffect"/> property changes
        /// </summary>
        /// <remarks>
        /// Upon changing the <see cref="TransitionEffect"/> all running animations are stopped and
        /// the current target is immediately displayed.
        /// </remarks>
        /// <param name="oldValue">The new value of the <see cref="TransitionEffect"/>.</param>
        /// <param name="newValue">The olf value of the <see cref="TransitionEffect"/>.</param>
        protected void OnTransitionEffectChanged( TransitionEffect oldValue, TransitionEffect newValue )
        {
            if (oldValue != null)
            {
                oldValue.TransitionPresenter   = null;
            }
            if (newValue != null)
            {
                newValue.TransitionPresenter   = this;

                if (m_TargetContent != null)
                {
                    newValue.DoTransition(m_TargetContent, TransitionDirection.Immediate);
                }
            }
        }

        /// <summary>
        /// Called when the <see cref="ContentTemplateSelector"/> property changes
        /// </summary>
        /// <param name="oldValue">The new value of the <see cref="System.Windows.Controls.DataTemplateSelector"/>.</param>
        /// <param name="newValue">The olf value of the <see cref="System.Windows.Controls.DataTemplateSelector"/>.</param>
        protected virtual void OnContentTemplateSelectorChanged( DataTemplateSelector oldValue, DataTemplateSelector newValue )
        {
        }
 
        /// <summary>
        /// Called when the <see cref="ContentTemplate"/> property changes
        /// </summary>
        /// <param name="oldValue">The new value of the <see cref="System.Windows.DataTemplate"/>.</param>
        /// <param name="newValue">The olf value of the <see cref="System.Windows.DataTemplate"/>.</param>
        protected virtual void OnContentTemplateChanged( DataTemplate oldValue, DataTemplate newValue )
        {
        }
     
        /// <summary>
        /// Static method to trigger the instance <see cref="OnContentTemplateChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnContentTemplateChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as TransitionPresenter).OnContentTemplateChanged((DataTemplate)args.OldValue, (DataTemplate)args.NewValue);
        }
     
        /// <summary>
        /// Static method to trigger the instance <see cref="OnContentTemplateSelectorChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnContentTemplateSelectorChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as TransitionPresenter).OnContentTemplateSelectorChanged((DataTemplateSelector)args.NewValue, (DataTemplateSelector)args.NewValue);
        }
     
        /// <summary>
        /// Static method to ensure that we have a <see cref="TransitionEffect"/>.
        /// </summary>
        /// <remarks>
        /// If we are in design mode then ensure the <see cref="TransitionEffect"/> is <see cref="EmptyTransitionEffect"/>.
        /// </remarks>
        /// <param name="target">The target of the property.</param>
        /// <param name="baseValue">Ensures that the transition effect is valid.</param>
        private static object OnTransitionEffectCoerceThunk( DependencyObject target, object baseValue )
        {
            if (SystemParameters.IsRemoteSession || DesignerProperties.GetIsInDesignMode(target) || (baseValue == null))
            {
                baseValue = new EmptyTransitionEffect();
            }
            return baseValue;
        }
     
        /// <summary>
        /// Static method to trigger the instance <see cref="OnTransitionEffectChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnTransitionEffectChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as TransitionPresenter).OnTransitionEffectChanged((TransitionEffect)args.OldValue, (TransitionEffect)args.NewValue);
        }

                
        #endregion

        #region --- Internal Helpers ---

        /// <summary>
        /// Create a <see cref="TransitionFrame"/> for use by this <see cref="TransitionPresenter"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal TransitionFrame CreateFrame( object content )
        {
            TransitionFrame frame = new TransitionFrame { Content = content,
                                                          HorizontalAlignment = HorizontalAlignment.Stretch,
                                                          VerticalAlignment = VerticalAlignment.Stretch,
                                                          FlowDirection = FlowDirection.LeftToRight
            };

            BindingOperations.SetBinding(frame, TransitionFrame.ContentTemplateProperty, new Binding("ContentTemplate") { Source = this });
            BindingOperations.SetBinding(frame, TransitionFrame.ContentTemplateSelectorProperty, new Binding("ContentTemplateSelector") { Source = this });
            BindingOperations.SetBinding(frame, TransitionFrame.HorizontalContentAlignmentProperty, new Binding("HorizontalContentAlignment") { Source = this });
            BindingOperations.SetBinding(frame, TransitionFrame.VerticalContentAlignmentProperty, new Binding("VerticalContentAlignment") { Source = this });

            return frame;
        }

        /// <summary>
        /// Raise the <see cref="BeginTransitionEvent"/> event.
        /// </summary>
        internal void RaiseBeginTransitionEvent()
        {
            IsInTransition = true;

            RaiseEvent(new RoutedEventArgs { RoutedEvent = BeginTransitionEvent });
        }
      
        /// <summary>
        /// Raise the <see cref="EndTransitionEvent"/> event.
        /// </summary>
        internal void RaiseEndTransitionEvent()
        {
            RaiseEvent(new RoutedEventArgs { RoutedEvent = EndTransitionEvent });

            IsInTransition = false;
        }

        /// <summary>
        /// Raise the <see cref="BeginFrameAnimationEvent"/> event.
        /// </summary>
        /// <param name="frame">The <see cref="TransitionFrame"/> that is about to be animated.</param>
        internal void RaiseBeginFrameAnimationEvent( TransitionFrame frame )
        {
            RaiseEvent(new TransitionAnimationEventArgs { TransitionFrame = frame, RoutedEvent = BeginFrameAnimationEvent });
        }
      
        /// <summary>
        /// Raise the <see cref="EndFrameAnimationEvent"/> event.
        /// </summary>
        /// <param name="frame">The <see cref="TransitionFrame"/> which has just been animated.</param>
        internal void RaiseEndFrameAnimationEvent( TransitionFrame frame )
        {
            RaiseEvent(new TransitionAnimationEventArgs { TransitionFrame = frame, RoutedEvent = EndFrameAnimationEvent });
        }


        #endregion
    }

    /// <summary>
    /// Helper class to hold information about a pending transition
    /// </summary>
    internal class PendingTransition
    {
        public object              Content   { get; private set; }
        public TransitionDirection Direction { get; private set; }

        public PendingTransition( object content, TransitionDirection direction )
        {
            Content   = content;
            Direction = direction;
        }
    }
}
