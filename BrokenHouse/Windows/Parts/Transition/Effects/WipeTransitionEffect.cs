using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using BrokenHouse.Utils;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Parts.Transition;

namespace BrokenHouse.Windows.Parts.Transition.Effects
{
    /// <summary>
    /// Provides a wipe transition effect.
    /// </summary>
    public class WipeTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// The effects store that holds the basic resources for the animation
        /// </summary>
        private static ResourceDictionaryEffectStore EffectStore = new ResourceDictionaryEffectStore("/Windows/Parts/Transition/Effects/Resources/Wipe.xaml");

        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property. 
        /// </summary>
        public  static DependencyProperty            AngleProperty;

        /// <summary>
        /// Initialise the WPF properties
        /// </summary>
        static WipeTransitionEffect()
        {
            AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(WipeTransitionEffect), new FrameworkPropertyMetadata(0.0));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WipeTransitionEffect"/> class.
        /// </summary>
        public WipeTransitionEffect()
        {
            StartOnTop = true;
        }

        #region --- Properties ---

        /// <summary>
        /// Gets or sets the angle of the wipe. This is a dependency property.
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        #endregion
 
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
