using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// Defines where in an animation a <see cref="TransitionFrame"/> should be placed. 
    /// </summary>
    /// <remarks>
    /// All animations are defined as tarking a <see cref="TransitionFrame"/> from one position
    /// to another. A forward animation (or the natural direction of an effect) will move the position
    /// from <see cref="TransitionPosition.Start"/>, to <see cref="TransitionPosition.Center"/> and
    /// then to <see cref="TransitionPosition.End"/>. And the backwards animation will move the  
    /// <see cref="TransitionFrame"/>  from <see cref="TransitionPosition.End"/> to 
    /// <see cref="TransitionPosition.Start"/> via <see cref="TransitionPosition.Center"/>.
    /// </remarks>
    public enum TransitionPosition 
    { 
        /// <summary>
        /// Used to signify that the <see cref="TransitionFrame"/> is positioned at the front of the animation.
        /// </summary>
        /// <remarks>
        /// If a frame is at this position it is either ready to be transitioned forwards or it has been
        /// transitioned backwards.
        /// </remarks>
        Start = -1, 
        
        /// <summary>
        /// Used to signify that the <see cref="TransitionFrame"/> is positioned in the middle of the animation. 
        /// </summary>
        /// <remarks>
        /// If a frame is at this position then it is the frame that is being presented to the user.
        /// </remarks>
        Center = 0, 
        
        /// <summary>
        /// Used to signify that the <see cref="TransitionFrame"/> is positioned at the back of the animation.
        /// </summary>
        /// <remarks>
        /// If a frame is at this position it is either ready to be transitioned backwards or it has been
        /// transitioned forwards from the <see cref="Center"/> position.
        /// </remarks>
        End = 1 
    };
}