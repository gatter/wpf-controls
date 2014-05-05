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
    /// Provides a sliding transition effect.
    /// </summary>
    public class SlideTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// The effects store that holds the basic resources for the animation
        /// </summary>
        private static ResourceDictionaryEffectStore EffectStore = new ResourceDictionaryEffectStore("/Windows/Parts/Transition/Effects/Resources/Slide.xaml");

        /// <summary>
        /// Identifies the <see cref="Direction"/> dependency property. 
        /// </summary>
        public  static DependencyProperty      DirectionProperty;

        /// <summary>
        /// Register the additional WPF properties required by this effect.
        /// </summary>
        static SlideTransitionEffect()
        {
            DirectionProperty = DependencyProperty.Register("Direction", typeof(TransitionMovement), typeof(SlideTransitionEffect), new FrameworkPropertyMetadata(TransitionMovement.RightToLeft));
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SlideTransitionEffect"/> class.
        /// </summary>
        public SlideTransitionEffect()
        {
            StartOnTop = true;
            ClipToBounds = true;
        }

        #region --- Properties ---

        /// <summary>
        /// Gets or sets the direction of the slide using in this effect. This is a dependency property.
        /// </summary>
        public TransitionMovement Direction
        {
            get { return (TransitionMovement)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        #endregion

        /// <summary>
        /// Create the <see cref="TransitionEffectAnimation"/> that will perform the actual animation.
        /// </summary>
        /// <param name="target">The target of the animation.</param>
        /// <param name="startPosition">The initial position of the <paramref name="target"/>.</param>
        /// <returns>The <see cref="TransitionEffectAnimation"/> that will manage the animation.</returns>
        protected override TransitionEffectAnimation CreateEffectAnimationOverride( TransitionFrame target, TransitionPosition startPosition )
        {
            return new SlideTransitionEffectAnimation(EffectStore) { TransitionFrame = target, StartPosition = startPosition, Direction = Direction };
        }    

    }

    /// <summary>
    /// Provides the actual animation objects for the effect
    /// </summary>
    internal class SlideTransitionEffectAnimation : ResourceDictionaryEffectAnimation
    {
        private TransitionMovement                  m_Direction   = TransitionMovement.LeftToRight;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="SlideTransitionEffectAnimation"/> class.
        /// </summary>
        public SlideTransitionEffectAnimation( ResourceDictionaryEffectStore store ) : base(store)
        {
        }

        /// <summary>
        /// Gets or sets the direction of the slide using in this effect.
        /// </summary>
        public TransitionMovement Direction
        {
            get { return m_Direction; }
            set 
            { 
                m_Direction = value;
                RequiresInitialisation = true;
            }
        }


        /// <summary>
        /// Gets the vectors required to slide the target based on the current direction.
        /// </summary>
        internal Point DirectionVector
        {
            get 
            { 
                Point result;

                if (Direction == TransitionMovement.LeftToRight)
                {
                    result = new Point(-1.0, 0.0);
                }
                else if (Direction == TransitionMovement.RightToLeft)
                {
                    result = new Point(1.0, 0.0);
                }
                else if (Direction == TransitionMovement.TopToBottom)
                {
                    result = new Point(0.0, -1.0);
                }
                else
                {
                    result = new Point(0.0, 1.0);
                }

                return result;
            }
        }
       
        /// <summary>
        /// Creates a storyboard that will perform the required animation
        /// </summary>
        /// <param name="startPosition">The starting position of the animation.</param>
        /// <param name="endPosition">The target position of the animation.</param>
        /// <returns>A storyboard that will perform the animation.</returns>
        protected override Storyboard CreateStoryboard( TransitionPosition startPosition, TransitionPosition endPosition )
        {
            Storyboard        storyboard       = EffectStore.GetStoryboard(startPosition, endPosition);
            DoubleAnimation[] slideAnimations  = storyboard.Children.Take(2).Cast<DoubleAnimation>().ToArray();
            Size              size             = TransitionFrame.FindVisualAncestor<TransitionPresenter>().RenderSize;
            Point             directionFactors = DirectionVector;
 
            // Have we actually been rendered - if not use the required size
            if ((size.Width == 0.0) && (size.Height == 0.0))
            {
                size = ParentEffect.TransitionPresenter.RenderSize;
            }

            // Determine what the transformation is
            if (endPosition == TransitionPosition.Start)
            {
                slideAnimations[0].To = directionFactors.X * size.Width;
                slideAnimations[1].To = directionFactors.Y * size.Height;
            }
            else if (endPosition == TransitionPosition.End)
            {
                slideAnimations[0].To = -directionFactors.X * size.Width;
                slideAnimations[1].To = -directionFactors.Y * size.Height;
            }
            else
            {
                slideAnimations[0].To = 0.0;
                slideAnimations[1].To = 0.0;
            }

            return storyboard;       
        }


        /// <summary>
        /// Sets the style of the target so that it will display correctly at the requested position
        /// </summary>
        /// <remarks>
        /// The standard style is obtained from the underlying <see cref="ResourceDictionaryEffectAnimation"/>;
        /// it is then modified so that the target is positioned correctly based on the current <see cref="Direction"/>.
        /// </remarks>
        /// <param name="position">The required position for the target.</param>
        protected override void InitialiseTransitionFrame( TransitionPosition position )
        {
            Style               style            = new Style(typeof(TransitionFrame), EffectStore.GetStyle(position));
            TranslateTransform  transform        = null;
            Size                size             = TransitionFrame.RenderSize;
            Point               directionFactors = DirectionVector;

            // Have we actually been rendered - if not use the required size
            if ((size.Width == 0.0) && (size.Height == 0.0))
            {
                size = ParentEffect.TransitionPresenter.RenderSize;
            }

            // Determine what the transformation is
            if (position == TransitionPosition.Start)
            {
                transform = new TranslateTransform(directionFactors.X * size.Width, directionFactors.Y * size.Height);
            }
            else if (position == TransitionPosition.End)
            {
                transform = new TranslateTransform(-directionFactors.X * size.Width, -directionFactors.Y * size.Height); 
            }
            else
            {
                // No change
                transform = new TranslateTransform(0, 0);
            }

            // Add to the style
            style.Setters.Add(new Setter(UIElement.RenderTransformProperty, transform));

            // Apply it
            TransitionFrame.Style = style;

            // Call the base class
            TransitionFrame.UseBitmapCaching = (position != TransitionPosition.Center);
        }

        /// <summary>
        /// Sets the style of the target so that is ready to transition from one position to another.
        /// </summary>
        /// <param name="startPosition">The initial position of the target</param>
        /// <param name="endPosition">The target position of the target.</param>
        protected override void InitialiseTransitionFrame( TransitionPosition startPosition, TransitionPosition endPosition )
        {
            InitialiseTransitionFrame(startPosition);
        }
    }
}
