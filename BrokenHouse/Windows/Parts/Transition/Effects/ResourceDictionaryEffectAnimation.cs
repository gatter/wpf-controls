using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BrokenHouse.Internal;

namespace BrokenHouse.Windows.Parts.Transition.Effects
{
    /// <summary>
    /// Provides a <see cref="TransitionEffectAnimation"/> that can be created from a <see cref="System.Windows.ResourceDictionary"/>.
    /// </summary>
    public class ResourceDictionaryEffectAnimation : TransitionEffectAnimation
    {
        /// <summary>
        /// The store on all resources that have be loaded from a <see cref="System.Windows.ResourceDictionary"/>.
        /// </summary>
        public ResourceDictionaryEffectStore     EffectStore { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ResourceDictionaryEffectAnimation"/> based on a <see cref="ResourceDictionaryEffectStore"/>.
        /// </summary>
        /// <remarks>
        /// For performance it is adviseable to create a single static <see cref="ResourceDictionaryEffectStore"/> and 
        /// use that to create all the required <see cref="ResourceDictionaryEffectAnimation">ResourceDictionaryEffectAnimation</see>.
        /// </remarks>
        /// <param name="effectStore">An effect store that holds all the objects from a resource dictionary.</param>
        public ResourceDictionaryEffectAnimation( ResourceDictionaryEffectStore effectStore )
        { 
            EffectStore = effectStore;
        }
        
        /// <summary>
        /// Creates a storyboard that will perform the required animation
        /// </summary>
        /// <param name="startPosition">The starting position of the animation.</param>
        /// <param name="endPosition">The target position of the animation.</param>
        /// <returns>A storyboard that will perform the animation.</returns>
        protected override Storyboard CreateStoryboard( TransitionPosition startPosition, TransitionPosition endPosition )
        {
            return EffectStore.GetStoryboard(startPosition, endPosition);
        }

        /// <summary>
        /// Sets the style of the target so that it will display correctly at the requested position
        /// </summary>
        /// <param name="position">The requured position for the target.</param>
        protected override void InitialiseTransitionFrame( TransitionPosition position )
        {
            TransitionFrame.Style = EffectStore.GetStyle(position);
            TransitionFrame.Measure(ParentEffect.TransitionPresenter.RenderSize);
            TransitionFrame.InvalidateVisual();
            TransitionFrame.UpdateLayout();

            // Call the base class
            base.InitialiseTransitionFrame(position);
        }

        /// <summary>
        /// Sets the style of the target so that is ready to transition from one position to another.
        /// </summary>
        /// <param name="startPosition">The initial position of the target</param>
        /// <param name="endPosition">The target position of the target.</param>
        protected override void InitialiseTransitionFrame( TransitionPosition startPosition, TransitionPosition endPosition )
        {
            TransitionFrame.Style = EffectStore.GetStyle(startPosition, endPosition);
            TransitionFrame.Measure(ParentEffect.TransitionPresenter.RenderSize);
            TransitionFrame.InvalidateVisual();
            TransitionFrame.UpdateLayout();
        }

        /// <summary>
        /// Remove the style from the target.
        /// </summary>
        protected override void ReleaseTransitionFrame()
        {
            TransitionFrame.Style = null;
            TransitionFrame.Measure(ParentEffect.TransitionPresenter.RenderSize);
            TransitionFrame.InvalidateVisual();
            TransitionFrame.UpdateLayout();
        }
    }
}
