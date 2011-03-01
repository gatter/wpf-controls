using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Parts.Transition.Primitives;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// Defines a transition effect. This object can be overridden to create diferent animations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The transition effect is the core of the Transition system; if controls all the animations, ensuring
    /// that the correct content is being transitioned into view. The is especially true when multiple transitions
    /// are taking place at the same time. When a transition takes place two animations are created, the first
    /// brings the new content into view, the second moves the old content out of view. If during an animation
    /// the old content is re-requested then the active animation is stopped and re-wound to ensure that the old content
    /// is brought back into view as smoothly as possible. By allowing this concept of rewinding each requested 
    /// transition has to specify a <see cref="TransitionDirection"/>.
    /// </para>
    /// <para>
    /// When a transition is running it has a direction associated with it. If a new transition is request with
    /// a different direction then the current transition is stop and rewound even if this means that it is 
    /// transitioned into view and then immediately transitioned out of view. The direction is automatically
    /// ignored if the requested content is already being transition; in this case the appropriate direction
    /// is selected to ensure that the content is brought into view.
    /// </para>
    /// <para>
    /// When the <see cref="BrokenHouse.Windows.Parts.Transition.TransitionType"/> is set to 
    /// <see cref="BrokenHouse.Windows.Parts.Transition.TransitionType.Sequential"/> the transition effect
    /// ensures that only one transition takes place at any one time. Any new transition that is requested before
    /// the current transition has ended is put into a pending list. When the active transition completes the
    /// next transition is removed from the pending list on a first-in first-out basis. If a content requested
    /// is already pending then the pending list is modified to reflect this new request. 
    /// </para>
    /// <para>
    /// When the <see cref="BrokenHouse.Windows.Parts.Transition.TransitionType"/> is set to 
    /// <see cref="BrokenHouse.Windows.Parts.Transition.TransitionType.Overlapped"/> the transition effect
    /// will allow multiple transitions to be active. When a new transition is requested the animations are created and
    /// started immediately and the previous animation is extended so that the now old content is transitioned out of view.
    /// </para>
    /// </remarks>
    public abstract class TransitionEffect : DependencyObject
    {
        private TransitionPresenter                 m_TransitionPresenter = null;
        private List<TransitionEffectAnimation>     m_Animations          = new List<TransitionEffectAnimation>();
        private List<TransitionInfo>                m_PendingTransitions  = new List<TransitionInfo>();

        /// <summary>
        /// Identifies the <see cref="SpeedRatio"/> dependency property. 
        /// </summary>
        public static DependencyProperty SpeedRatioProperty;

        /// <summary>
        /// Identifies the <see cref="BrokenHouse.Windows.Parts.Transition.TransitionType"/> dependency property. 
        /// </summary>
        public static DependencyProperty TransitionTypeProperty;


        #region --- Constructors ---

        /// <summary>
        /// Register the WPF properties
        /// </summary>
        static TransitionEffect()
        {
            SpeedRatioProperty     = DependencyProperty.Register("SpeedRatio", typeof(double), typeof(TransitionEffect), new FrameworkPropertyMetadata(1.0, null, OnSpeedRatioCoerceThunk));
            TransitionTypeProperty = DependencyProperty.Register("TransitionType", typeof(TransitionType), typeof(TransitionEffect), new FrameworkPropertyMetadata(TransitionType.Overlapped));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TransitionEffect()
        {
            StartOnTop = true;
        }

        #endregion

        #region --- Properties ---

        /// <summary>
        /// Gets or sets whether frames in the <see cref="TransitionPosition.Start"/> position are created and added
        /// to the transition control as the topmost visual.
        /// </summary>
        protected bool StartOnTop { get; set; }

        /// <summary>
        /// Gets or sets whether the frames should be clipped to the bounds of the transition control.
        /// </summary>
        protected bool ClipToBounds { get; set; }

        /// <summary>
        /// Gets whether this effect has any active transitions.
        /// </summary>
        public bool HasActiveAnimations
        {
            get {  return (m_Animations.Count == 1)? (m_Animations[0].StartPosition != TransitionPosition.Center) : (m_Animations.Count > 1); }
        }

        /// <summary>
        /// Gets whether this effect has any pending transitions
        /// </summary>
        /// <remarks>
        /// There will only be pending transitions if the <see cref="BrokenHouse.Windows.Parts.Transition.TransitionType"/> is 
        /// <see cref="BrokenHouse.Windows.Parts.Transition.TransitionType.Sequential"/>. For
        /// a sequential transition to work all new transitions are stacked and are only run when the previous transition is complete.
        /// </remarks>
        public bool HasPendingTransitions
        {
            get { return (m_PendingTransitions.Count > 0);  }
        }

        /// <summary>
        /// Gets or sets the type of transition that this effect will use. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// Transitions can run either <see cref="BrokenHouse.Windows.Parts.Transition.TransitionType.Sequential"/> or 
        /// <see cref="BrokenHouse.Windows.Parts.Transition.TransitionType.Overlapped"/>. A 
        /// sequential transition means that a new transition does not start until the old one is completed. This is in
        /// contrast to an overlapped transition where any the new transition will start immediately (i.e. multiple
        /// transitions can be running at the same time).
        /// </remarks>
        public TransitionType TransitionType
        {
            get { return (TransitionType)GetValue(TransitionTypeProperty); }
            set { SetValue(TransitionTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets rate at which time progresses for this <see cref="TransitionEffect"/>. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// A finite value greater than 0 that describes the rate at which time progresses for this <see cref="TransitionEffect"/>
        /// relative to the default timeline speed. Any timeline that are created as part of this effect will have 
        /// its <see cref="System.Windows.Media.Animation.Timeline.SpeedRatio"/> set to this value.
        /// The default value is 1.
        /// </remarks>
        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }

        /// <summary>
        /// The transition control to which we are attached.
        /// </summary>
        internal TransitionPresenter TransitionPresenter 
        { 
            get { return m_TransitionPresenter; } 
            set
            {
                // If the transition control was previously set detach from it.
                if (m_TransitionPresenter != null)
                {
                    StopAllAnimations();
                    m_TransitionPresenter.SetValue(UIElement.ClipToBoundsProperty, DependencyProperty.UnsetValue);
                }

                // Save the value
                m_TransitionPresenter = value;

                // Attach to the new transition control
                if (m_TransitionPresenter != null)
                {
                    m_TransitionPresenter.SetValue(UIElement.ClipToBoundsProperty, ClipToBounds);
                }
            }
        }

        #endregion
        
        #region --- Dependancy Property Callbacks ---

        /// <summary>
        /// Coerce the value about to be assigned to the <see cref="SpeedRatio"/> and ensure its within
        /// the correct limits.
        /// </summary>
        /// <param name="d">The dependancy object whose property we are about to set.</param>
        /// <param name="baseValue">The new value that is about to be assigned to the <see cref="SpeedRatio"/>.</param>
        /// <returns>The value of the <see cref="SpeedRatio"/> within the required limits.</returns>
        private static object OnSpeedRatioCoerceThunk( DependencyObject d, object baseValue )
        {
            return Math.Max(0.0, (double)baseValue);
        }

        #endregion

        #region --- Transition Management ---

        /// <summary>
        /// Trigger a transition to the new content in a specific direction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is the key to all transitions. Calling this function will cause
        /// the supplied content to be transitioned into the view. If the content is
        /// already being transitioned then any running or pending transitions will
        /// be altered to reflect the change required (even if a change requires
        /// the direction of other transitions to be modified).
        /// </para>
        /// <para>
        /// If the content is currently being animated then the direction is ignored. The
        /// direction chosen will be determined by the direction of the current animations. 
        /// </para>
        /// </remarks>
        /// <param name="newContent">The content to be animated</param>
        /// <param name="direction">The direction of the animation</param>
        internal void DoTransition( object newContent, TransitionDirection direction )
        {
            // If we are doing an immediate transition then all other animations must stop.
            if (direction == TransitionDirection.Immediate)
            {
                StopAllAnimations();
            }
             
            // Is the content already being animated
            TransitionEffectAnimation animation = m_Animations.Where(i => i.TransitionFrame.Content == newContent).FirstOrDefault();

            // Are we starting any transitions
            if (!HasActiveAnimations && !HasPendingTransitions)
            {
                TransitionPresenter.RaiseBeginTransitionEvent();
            }
 
            // Act on the transition type
            if (TransitionType == TransitionType.Overlapped)
            {
                UpdateAnimations(animation ?? CreateEffectAnimation(newContent, direction));
            }
            else if (HasActiveAnimations && (animation == null))
            {
                // We are animating items but not the one requrested.
                TransitionInfo exisitingInfo = m_PendingTransitions.Where(i => i.Content == newContent).FirstOrDefault();

                // Has the transition already been requested and is pending.
                if (exisitingInfo != null)
                {
                    // Yes. we have to junk all animations after this item as they are no longer needed
                    m_PendingTransitions = m_PendingTransitions.Take(m_PendingTransitions.IndexOf(exisitingInfo) + 1).ToList();
                }
                else
                {
                    // No. add it to the pending list
                    m_PendingTransitions.Add(new TransitionInfo { Content = newContent, Direction = direction });
                }
            }
            else if (animation == null)
            {
                // We are not currently animating - do the transition now.
                UpdateAnimations(CreateEffectAnimation(newContent, direction));
            }
            else
            {
                // We are transitioning the item we are currently animating
                // All the other pending transitions are no longer required.
                m_PendingTransitions.Clear();

                // Update the animations 
                UpdateAnimations(animation);
            }
        }

        #endregion

        #region --- Internal helpers ---

        /// <summary>
        /// Help function to update the current animations to ensure
        /// the the required content is brought into view by changing
        /// the direction of all running animations.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method should only be supplied with an animation that 
        /// is already in effect. 
        /// </para>
        /// </remarks>
        /// <param name="animation">The animation that is to be the main animation; the one that brings the desired
        /// content into view.</param>
        private void UpdateAnimations( TransitionEffectAnimation animation )
        {
            // Attach the transition item
            TransitionPosition endPosition = TransitionPosition.Start;

            // Loop over the animations (N.B. take a new list because the animation may end before we go over the elements)
            foreach ( TransitionEffectAnimation item in m_Animations.ToList())
            {
                // Is this animation that 
                if (item == animation)
                {
                    item.TransitionTo(TransitionPosition.Center);
                    endPosition = TransitionPosition.End;
                }
                else 
                {
                    item.TransitionTo(endPosition);
                }
            }
        }

        /// <summary>
        /// Helper function to create an animation effect.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Upon successfully creating the animation the target will be added as a child of the 
        /// <see cref="BrokenHouse.Windows.Parts.Transition.Primitives.TransitionPresenter"/>.
        /// </para>
        /// </remarks>
        /// <param name="newContent">The content to be animated</param>
        /// <param name="direction">The direction of the animation</param>
        /// <returns>The animation that will perform the required transition.</returns>
        private TransitionEffectAnimation CreateEffectAnimation( object newContent, TransitionDirection direction )
        {
            TransitionPosition position = TransitionPosition.Center;
            
            // Determine the initial position of the animation
            if (direction == TransitionDirection.Forwards)
            {
                position = TransitionPosition.Start;
            }
            else if (direction == TransitionDirection.Backwards)
            {
                position = TransitionPosition.End;
            }
            else
            {
                position = TransitionPosition.Center;
            }

            // Create the frame and animation
            TransitionFrame           frame     = TransitionPresenter.CreateFrame(newContent);
            TransitionEffectAnimation animation = CreateEffectAnimationOverride(frame, position);

            // Ensure that the new items know the effect that created them
            animation.ParentEffect = this;
            frame.TransitionEffect = this;

            // We want to know when the animation is completed
            animation.Completed += OnAnimationCompleted;            

            // Determine where we should add the item in the animation panel
            if ((animation.StartPosition == TransitionPosition.Start) ^ !StartOnTop)
            {
                TransitionPresenter.TransitionFrames.Add(animation.TransitionFrame);
            }
            else
            {
                TransitionPresenter.TransitionFrames.Insert(0, animation.TransitionFrame);
            }
 
            // Determine where we place the animation
            if (animation.StartPosition == TransitionPosition.Start)
            {
                m_Animations.Insert(0, animation);
            }
            else
            {
                m_Animations.Add(animation);
            }

            // Ensure that the sizes of the new elements are correct.
            animation.TransitionFrame.InvalidateMeasure();
            animation.TransitionFrame.InvalidateArrange();
            animation.TransitionFrame.InvalidateVisual();
            animation.TransitionFrame.UpdateLayout();
            TransitionPresenter.InvalidateMeasure();
            TransitionPresenter.InvalidateArrange();
            TransitionPresenter.InvalidateVisual();
            TransitionPresenter.UpdateLayout();

            // Trigger an event signifying that this frame has started
            TransitionPresenter.RaiseBeginFrameAnimationEvent(frame);

            // Return the new animation
            return animation;
        } 

        /// <summary>
        /// The animation is complete - make sure everythin is cleaned up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAnimationCompleted( object sender, EventArgs e )
        {
            TransitionEffectAnimation completedAnimation = sender as TransitionEffectAnimation;

            // If the animation was not to the center
            if (completedAnimation.StartPosition != TransitionPosition.Center)
            {
                // Then we can process it and remove the content from the display
                CleanUpCompletedAnimation(completedAnimation);
 
                // Trigger the event that signals that the frame has ended its animation
                TransitionPresenter.RaiseEndFrameAnimationEvent(completedAnimation.TransitionFrame);
            }

            // Was this the last active animation (N.B. Animations always run in pairs)
            if (!HasActiveAnimations)
            {
                // Are there any pending animations
                if (HasPendingTransitions)
                {
                    // Get the firest pending transition.
                    TransitionInfo            info      = m_PendingTransitions[0];
                    TransitionEffectAnimation animation = CreateEffectAnimation(info.Content, info.Direction);

                    // Remove it from the list 
                    m_PendingTransitions.RemoveAt(0);

                    // Cause our new animation to be the key one.
                    UpdateAnimations(animation);
                }
                else
                {
                    // End the transition
                    TransitionPresenter.RaiseEndTransitionEvent();
                }
            }
        }

        /// <summary>
        /// A helper function to release an animation and clean up any references
        /// </summary>
        /// <param name="animation"></param>
        private void CleanUpCompletedAnimation( TransitionEffectAnimation animation )
        {
            // Release the animation target
            animation.Dispose();

            // Remove the target from the animation frames of the transition control
            TransitionPresenter.TransitionFrames.Remove(animation.TransitionFrame);

            // Remove our reference to the animation
            m_Animations.Remove(animation);

            // Remove our attachment to the completed event.
            animation.Completed -= OnAnimationCompleted;
        }

        #endregion

        #region --- Overridable methods ---

        /// <summary>
        /// The function that allows the dervied animations to create their own animations
        /// </summary>
        /// <param name="animationFrame">The frame to be animated.</param>
        /// <param name="startPosition">The initial position of the transition.</param>
        /// <returns>A new transition animation.</returns>
        protected abstract TransitionEffectAnimation CreateEffectAnimationOverride( TransitionFrame animationFrame, TransitionPosition startPosition );

        #endregion

        #region --- Public Methods ---

        /// <summary>
        /// Stop all running animations.
        /// </summary>
        public virtual void StopAllAnimations()
        {
            bool hadTransitions = HasActiveAnimations || HasPendingTransitions;

            // Loop over each of the running animations and clear them
            foreach (var animation in m_Animations.ToList())
            {
                // Stop the animation
                animation.Stop();

                // Clean up any of its renferences
                CleanUpCompletedAnimation(animation);
            }

            // Clear the pending transitions
            m_PendingTransitions.Clear();

            // Did we have any pending or active transitions 
            if (hadTransitions)
            {
                // If so trigger that the transition is complete
                TransitionPresenter.RaiseEndTransitionEvent();
            }
        }

        #endregion

    }

    /// <summary>
    /// Helper class to hold details of pending transitions.
    /// </summary>
    internal class TransitionInfo
    {
        public object               Content   { get; set; }
        public TransitionDirection  Direction { get; set; }
    }


}
