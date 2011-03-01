using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// Defines how a transition effect manages multiple transitions.
    /// </summary>
    public enum TransitionType 
    { 
        /// <summary>
        /// The <see cref="TransitionEffect"/> should only run one animation at a time.
        /// </summary>
        /// <remarks>
        /// If an animation is requested and there is animation already running then the animation
        /// must be put into a pending list. When each animation completes the next animation in this
        /// list should be run. However, if the target of the animation is already pending then all
        /// subsequent requests are ignored. 
        /// </remarks>
        Sequential, 
        
        /// <summary>
        /// The <see cref="TransitionEffect"/> should allow animations to overlap.
        /// </summary>
        /// <remarks>
        /// All animations are started as soon as they are requested irrespective of how many
        /// animations are currently running.
        /// </remarks>
        Overlapped 
    };
}