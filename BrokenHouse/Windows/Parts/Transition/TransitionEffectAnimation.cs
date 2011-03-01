using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// Provides the controller for animating a transition.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This object can not only manage the animation involved with a transition but it can also reverse that animation
    /// once it has started. The state of the transition is controlled by the <see cref="TransitionPosition"/> enumeration.
    /// </para>
    /// <para>
    /// When the <see cref="TransitionFrame"/> is placed at the <see cref="TransitionPosition.Start"/> it is invisible but in a position where
    /// it can be transitioned into view in a forwards direction. Similarly, when the frame is placed at the 
    /// <see cref="TransitionPosition.End"/> it is also invisible but in a position where it can be transitioned into
    /// view in a backwards direction. Finally, the <see cref="TransitionPosition.Center"/> is used to indicate that 
    /// the <see cref="TransitionFrame"/> is in a position where it is completely visible.
    /// </para>
    /// <para>
    /// All animations are created to transition the <see cref="TransitionFrame"/> from one position to the next position. If a transition is 
    /// required that involves stepping over a position, for example, transitioning from <see cref="TransitionPosition.Start"/>
    /// to <see cref="TransitionPosition.End"/> then an animation is created to take the frame from <see cref="TransitionPosition.Start"/>
    /// to <see cref="TransitionPosition.Center"/>. Once this is complete another animation is created to take the frame from
    /// <see cref="TransitionPosition.Center"/> to <see cref="TransitionPosition.End"/>. When the desired position has been reached
    /// the <see cref="Completed"/> event is triggered.
    /// </para>
    /// </remarks>
    public class TransitionEffectAnimation : IDisposable
    {
        private TransitionFrame                             m_TransitionFrame                 = null;
        private TransitionPosition                          m_StartPosition           = TransitionPosition.Start;
        private TransitionPosition                          m_EndPosition          = TransitionPosition.Start;
        private TransitionPosition                          m_ActivePosition         = TransitionPosition.Start;
        private Storyboard                                  m_ActiveStoryboard       = null;
        private TransitionEffect                            m_ParentEffect           = null;
        private bool                                        m_RequiresInitialisation = true;
        
        
        #region --- Constructor ---

        /// <summary>
        /// Protected constructor to stop this class being directly created.
        /// </summary>
        protected TransitionEffectAnimation()
        {
        }

        #endregion

       
        #region --- IDisposable Members ---

        /// <summary>
        /// Stop all animations and ensure that the <see cref="TransitionFrame"/> is not attached to this object.
        /// </summary>
        /// <remarks>
        /// All animations are controlled by styles and storyboards; calling this function will
        /// remove all styles from the <see cref="TransitionFrame"/> and stop all running storyboards.
        /// </remarks>
        public void Dispose()
        {
            Stop();
            ReleaseTransitionFrame();
        }

        #endregion

        #region --- Properties & Events ---

        /// <summary>
        /// Gets ths flag signifying that the <see cref="TransitionFrame"/> needs to be initialised before an animation
        /// is created.
        /// </summary>
        internal bool RequiresInitialisation
        { 
            get { return m_RequiresInitialisation; } 
            set { m_RequiresInitialisation = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="TransitionEffect"/> that controls this animation.
        /// </summary>
        internal TransitionEffect ParentEffect
        { 
            get { return m_ParentEffect; } 
            set 
            { 
                m_ParentEffect = value;
                m_RequiresInitialisation = true;
            }
        }

        /// <summary>
        /// Gets the current speed ratio that should be used when creating the animations.
        /// </summary>
        public double SpeedRatio
        { 
            get { return (ParentEffect != null)? ParentEffect.SpeedRatio : 1.0; } 
        }

        /// <summary>
        /// Gets or sets the position that is the starting point of the animation
        /// </summary>
        public TransitionPosition StartPosition
        { 
            get { return m_StartPosition; } 
            set
            {
                m_StartPosition = value;
                m_RequiresInitialisation = true;
            }
        }

        /// <summary>
        /// Gets the final position of the animation
        /// </summary>
        public TransitionPosition EndPosition
        { 
            get { return m_EndPosition; } 
        }

        /// <summary>
        /// Gets the current position of the animation.
        /// </summary>
        /// <remarks>
        /// A transition cannot go from the <see cref="TransitionPosition.Start"/> position to the <see cref="TransitionPosition.End"/> 
        /// in one go. The animation must go through the <see cref="TransitionPosition.Center"/> position. This means that there
        /// may be cases where the current animation <see cref="TransitionFrame"/> is not the <see cref="EndPosition"/>
        /// </remarks>
        public TransitionPosition ActivePosition
        { 
            get { return m_ActivePosition; } 
        }

        /// <summary>
        /// Gets the <see cref="TransitionFrame"/> that is the target of this animation.
        /// </summary>
        public TransitionFrame TransitionFrame
        { 
            get { return m_TransitionFrame; } 
            set
            {
                if (m_TransitionFrame != null)
                {
                    m_TransitionFrame.Style = null;
                }

                m_TransitionFrame = value;
                m_RequiresInitialisation = true;
            }
        }

        /// <summary>
        /// Occurs when the animation has completed.
        /// </summary>
        public event EventHandler Completed;

        #endregion
                
        #region --- Public interface ---
 
        /// <summary>
        /// Stop the animation and move the <see cref="TransitionFrame"/> to the current active position.
        /// </summary>
        public void Stop()
        {
            if (m_ActiveStoryboard != null)
            {
                // Close the storyboard and clean up.
                m_ActiveStoryboard.Completed -= OnStoryboardCompleted;
                m_ActiveStoryboard.Remove(m_TransitionFrame);
                m_ActiveStoryboard = null;
            }
             
            // Use the base position as the active position
            m_StartPosition = m_ActivePosition;

            // Move the target to the new base position
            InitialiseTransitionFrame(m_StartPosition);
        }

        /// <summary>
        /// Transition the <see cref="TransitionFrame"/> to a new <see cref="TransitionPosition"/>.
        /// </summary>
        /// <para>
        /// If the new position is in the same direction as the current animation
        /// then the current animation will run to completion. Upon completion a
        /// new animation will be created to take the <see cref="TransitionFrame"/>  to the desired position.
        /// </para>
        /// <para>
        /// If the new position is in the oposite direction then the current animation
        /// is stop and the opposite animation is created. This new animation is positioned
        /// so to have the effect that the current animation is rewound. If the final
        /// required position requires further animation then a new animation will
        /// be created once this animation is completed.
        /// </para>
        /// <param name="newPosition">The position to which the <see cref="TransitionFrame"/> should be transitioned.</param>
        public void TransitionTo( TransitionPosition newPosition )
        {
            TimeSpan seekTime = TimeSpan.FromSeconds(0.0);

            // If we have recently tweaked things then we have to initialise the target.
            if (m_RequiresInitialisation)
            {
                m_RequiresInitialisation = false;
                InitialiseTransitionFrame(m_StartPosition);
            }
            
            // Save the new position as the final position of the animation
            m_EndPosition = newPosition;

            // Is a story board running?
            if (m_ActiveStoryboard != null)
            {
                // Work out if we have to rewind the animation
                if (((newPosition < m_ActivePosition) && (m_StartPosition < m_ActivePosition)) || ((newPosition > m_ActivePosition) && (m_StartPosition > m_ActivePosition)))
                {
                    // Work out where we are in the animation
                    TimeSpan? animationTime = m_ActiveStoryboard.GetCurrentTime(m_TransitionFrame);

                    // Stop the current storybpard and clean up
                    m_ActiveStoryboard.Completed -= OnStoryboardCompleted;
                    m_ActiveStoryboard.Stop(m_TransitionFrame);
                    m_ActiveStoryboard = null;

                    // Update the base position as this is a new starting point
                    m_StartPosition = m_ActivePosition;
                    InitialiseTransitionFrame(m_StartPosition);

                    // Did we get the time? 
                    if (animationTime.HasValue)
                    {
                        // yes - work out how far into the new animation we need to be to do the rewind
                       seekTime = TimeSpan.FromSeconds(1.0) - animationTime.Value;
                   }  
                }
            }

            // If we do not have a storyboard then we need to create one.
            if (m_ActiveStoryboard == null)
            {
                // Work out where we need to be in this animation (may not be the final position).
                if (newPosition < m_StartPosition)
                {
                    m_ActivePosition = m_StartPosition - 1;
                }
                else if (newPosition > m_StartPosition)
                {
                    m_ActivePosition = m_StartPosition + 1;
                }
                else
                {
                    m_ActivePosition = m_StartPosition;
                }

                // Is the position different from our current one.
                if (m_ActivePosition != m_StartPosition)
                {
                    // Yes - create a storyboard that will do the transition
                    m_ActiveStoryboard = CreateStoryboard(m_StartPosition, m_ActivePosition);

                    // Did we create one
                    if (m_ActiveStoryboard == null)
                    {
                        // No - jump to the end of the animation
                        m_StartPosition = m_ActivePosition;

                        // Set the target to the required position
                        InitialiseTransitionFrame(m_StartPosition);

                        // Trigger the completed event
                        OnCompleted();
                    }
                    else
                    {
                        // Adjust the storyboard based on our speed ratio
                        m_ActiveStoryboard.SpeedRatio = SpeedRatio;
                        m_ActiveStoryboard.Completed += OnStoryboardCompleted;
 
                        // Initialise the target for the transition
                        InitialiseTransitionFrame(m_StartPosition, m_ActivePosition);

                        // Start the transition
                        m_ActiveStoryboard.Begin(m_TransitionFrame, m_TransitionFrame.Template, HandoffBehavior.SnapshotAndReplace, true);
     
                        // Do we need to jump to a specific point (i.e. are we rewinding)
                        if (seekTime.TotalSeconds > 0.0)
                        {
                            // Yes.
                            m_ActiveStoryboard.SeekAlignedToLastTick(m_TransitionFrame, seekTime, TimeSeekOrigin.BeginTime);
                        }
                    }
                }
            }
        }


        #endregion
                
        #region --- Protected virtuals ---

        /// <summary>
        /// Create a storyboard that will perform the required animation
        /// </summary>
        /// <param name="startPosition">The initial position of the animation.</param>
        /// <param name="finalPosition">The final position of the animation.</param>
        /// <returns>A storyboard that will perform the animation.</returns>
        protected virtual Storyboard CreateStoryboard( TransitionPosition startPosition, TransitionPosition finalPosition )
        {
            return null;
        }

        /// <summary>
        /// Initialise the <see cref="TransitionFrame"/>  so that it will display correctly at the requested 
        /// <see cref="TransitionPosition"/>.
        /// </summary>
        /// <remarks>
        /// This method will be called when the <see cref="TransitionFrame"/> is in a static position.
        /// </remarks>
        /// <param name="position">The requured position for the target.</param>
        protected virtual void InitialiseTransitionFrame( TransitionPosition position )
        {
        }
 
        /// <summary>
        /// Initialise the <see cref="TransitionFrame"/>  so that is ready to transition from one position to another.
        /// </summary>
        /// <remarks>
        /// As animations can be very different it is often required that the <see cref="TransitionFrame"/> 
        /// be initialise specifically for that animation. An specialised animation should
        /// override this function to place the frame in a state ready to be transitioned
        /// using the storyboard created by <see cref="CreateStoryboard"/>.
        /// </remarks>
        /// <param name="startPosition">The initial position of the target</param>
        /// <param name="endPosition">The target position of the target.</param>
        protected virtual void InitialiseTransitionFrame( TransitionPosition startPosition, TransitionPosition endPosition )
        {
        }

        /// <summary>
        /// Ensures that the <see cref="TransitionFrame"/> has no references to any properties created by this <see cref="TransitionEffectAnimation"/>.
        /// </summary>
        protected virtual void ReleaseTransitionFrame()
        {
        }
        
        #endregion
                
        #region --- Private Event Handlers ---

        /// <summary>
        /// Trigger the <see cref="Completed"/> Event.
        /// </summary>
        private void OnCompleted()
        {
            if (Completed != null)
            {
                Completed(this, EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// Our storyboard is completed.
        /// </summary>
        /// <param name="sender">The storyboard that has just completed</param>
        /// <param name="e">Details about the event</param>
        private void OnStoryboardCompleted( object sender, EventArgs e )
        {
            // Detatch from the storyboard
            m_ActiveStoryboard.Completed -= OnStoryboardCompleted;
            m_ActiveStoryboard.Remove(m_TransitionFrame);
            m_ActiveStoryboard = null;

            // The base position is at the position that this animation ended
            m_StartPosition = m_ActivePosition;

            // Ensure the target reflects this
            InitialiseTransitionFrame(m_StartPosition);

            // Do we have to animate again
            if (m_ActivePosition != m_EndPosition)
            {
                // Transition to the final position
                TransitionTo(m_EndPosition);
            }
            else
            {
                // Trigger the completed event
                OnCompleted();
            }
        }
                
        #endregion

    }
}
