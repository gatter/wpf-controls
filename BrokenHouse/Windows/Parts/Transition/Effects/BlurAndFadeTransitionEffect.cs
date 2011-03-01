using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using BrokenHouse.Utils;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Parts.Transition;

namespace BrokenHouse.Windows.Parts.Transition.Effects
{

    /// <summary>
    /// Provides the Blur and Fade transition effect.
    /// </summary>
    public class BlurAndFadeTransitionEffect : TransitionEffect
    {
        private static ResourceDictionaryEffectStore EffectStore = new ResourceDictionaryEffectStore("/Windows/Parts/Transition/Effects/Resources/BlurAndFade.xaml");

        /// <summary>
        /// Initializes a new instance of the <see cref="BlurAndFadeTransitionEffect"/> class.
        /// </summary>
        public BlurAndFadeTransitionEffect()
        {
            StartOnTop = false;
            ClipToBounds = true;
        }

        /// <summary>
        /// Create the <see cref="TransitionEffectAnimation"/> that will perform the actual animation.
        /// </summary>
        /// <param name="target">The target of the animation.</param>
        /// <param name="position">The initial position of the <paramref name="target"/>.</param>
        /// <returns>The <see cref="TransitionEffectAnimation"/> that will manage the animation.</returns>
        protected override TransitionEffectAnimation CreateEffectAnimationOverride( TransitionFrame target, TransitionPosition position )
        {
            return new ResourceDictionaryEffectAnimation(EffectStore) { TransitionFrame = target, StartPosition = position };
        }    
    }
}

