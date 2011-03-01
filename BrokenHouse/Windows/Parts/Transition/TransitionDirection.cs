using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// The direction the transition will take.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When a transition is initiated on a
    /// <see cref="BrokenHouse.Windows.Parts.Transition.Primitives.TransitionPresenter"/> by calling
    /// <see cref="BrokenHouse.Windows.Parts.Transition.Primitives.TransitionPresenter.DoTransition"/>
    /// the direction of the transition is required. This describes whether the new content should be 
    /// transitioned in using the effects natural direction, its reverse or immediately (without 
    /// any animation). 
    /// </para> 
    /// </remarks>
    public enum TransitionDirection 
    { 
        /// <summary>
        /// The natural movement of a <see cref="TransitionEffect"/>.
        /// </summary>
        Forwards, 
        
        /// <summary>
        /// The movement that is the opposite of the <see cref="TransitionEffect"/> natural movement.
        /// </summary>
        Backwards, 
        
        /// <summary>
        /// The movement that causes the animation to reach its destination immediately.
        /// </summary>
        Immediate };
}     
