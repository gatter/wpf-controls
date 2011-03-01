using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BrokenHouse.Utils;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Parts.Transition;
using BrokenHouse.Windows.Parts.Transition.Primitives;

namespace BrokenHouse.Windows.Parts.Transition.Effects
{
    /// <summary>
    /// Provides a transition effect that does not animate the target. Used as the default <see cref="TransitionEffect"/>.
    /// </summary>
    public class EmptyTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Create the <see cref="TransitionEffectAnimation"/> that does not contain any animations.
        /// </summary>
        /// <param name="target">The target of the animation.</param>
        /// <param name="startPosition">The initial position of the <paramref name="target"/>.</param>
        /// <returns>The <see cref="TransitionEffectAnimation"/> that will manage the animation.</returns>
        protected override TransitionEffectAnimation CreateEffectAnimationOverride( TransitionFrame target, TransitionPosition startPosition )
        {
            return new EmptyTransitionEffectAnimation() { TransitionFrame = target, StartPosition = startPosition };
        }
    }

    /// <summary>
    /// Provides the actual animation objects for the effect
    /// </summary>
    internal class EmptyTransitionEffectAnimation : TransitionEffectAnimation
    {

        /// <summary>
        /// Creates a storyboard that will perform the required animation
        /// </summary>
        /// <param name="startPosition">The starting position of the animation.</param>
        /// <param name="endPosition">The target position of the animation.</param>
        /// <returns>A storyboard that will perform the animation.</returns>
        protected override Storyboard CreateStoryboard( TransitionPosition startPosition, TransitionPosition endPosition )
        {
            return null;
        }

        /// <summary>
        /// Sets the style of the target so that it will display correctly at the requested position
        /// </summary>
        /// <param name="position">The requured position for the target.</param>
        protected override void InitialiseTransitionFrame( TransitionPosition position )
        {
            ComponentResourceKey key = (position == TransitionPosition.Center)? TransitionElements.TransitionFrameStyleKey : TransitionElements.TransitionFrameEmptyStyleKey;
            
            TransitionFrame.Style = TransitionFrame.FindResource(key) as Style;
            TransitionFrame.InvalidateVisual();
            TransitionFrame.UpdateLayout();
        }

        /// <summary>
        /// Sets the style of the target so that is ready to transition from one position to another.
        /// </summary>
        /// <param name="startPosition">The initial position of the target</param>
        /// <param name="endPosition">The target position of the target.</param>
        protected override void InitialiseTransitionFrame( TransitionPosition startPosition, TransitionPosition endPosition )
        {
            ComponentResourceKey key = (endPosition == TransitionPosition.Center)? TransitionElements.TransitionFrameStyleKey : TransitionElements.TransitionFrameEmptyStyleKey;
            
            TransitionFrame.Style = TransitionFrame.FindResource(key) as Style;
        }

        /// <summary>
        /// Remove the style from the target.
        /// </summary>
        protected override void ReleaseTransitionFrame()
        {
            TransitionFrame.Style = TransitionFrame.FindResource(TransitionElements.TransitionFrameEmptyStyleKey) as Style;
            TransitionFrame.InvalidateMeasure();
            TransitionFrame.InvalidateVisual();
            TransitionFrame.UpdateLayout();
        }
    }
}
