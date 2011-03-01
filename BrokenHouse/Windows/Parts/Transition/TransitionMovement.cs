using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// Defines the different directions a transition should take.
    /// </summary>
    /// <remarks>
    /// Only a few transitions have a concept of a direction the animation should take. For example, the
    /// <see cref="BrokenHouse.Windows.Parts.Transition.Effects.SlideTransitionEffect"/> can only
    /// slide in new content in a horizontal or vertical axis whilst the 
    /// <see cref="BrokenHouse.Windows.Parts.Transition.Effects.WipeTransitionEffect"/> can perform
    /// its wipe at any angle. It is for animation effects like the
    /// <see cref="BrokenHouse.Windows.Parts.Transition.Effects.SlideTransitionEffect"/>
    /// that this enumeration can be used to define which direction along the horizontal or vertical
    /// axis the animation should take.
    /// </remarks>
    public enum TransitionMovement 
    { 
        /// <summary>
        /// New content is transitioned in from the left.
        /// </summary>
        LeftToRight, 
        
        /// <summary>
        /// New content is transitioned in from the right.
        /// </summary>
        RightToLeft, 
        
        /// <summary>
        /// New content is transitioned in from the top.
        /// </summary>
        TopToBottom, 
        
        /// <summary>
        /// New content is transitioned in from the bottom.
        /// </summary>
        BottomToTop 
    }
}
