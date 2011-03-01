using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// Provides data for the 
    /// <see cref="BrokenHouse.Windows.Parts.Transition.Primitives.TransitionPresenter.BeginFrameAnimation"/>, and 
    /// <see cref="BrokenHouse.Windows.Parts.Transition.Primitives.TransitionPresenter.EndFrameAnimation"/> events 
    /// of any of the transtions controls.
    /// </summary>
    public sealed class TransitionAnimationEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The <see cref="TransitionFrame"/> that is either starting or ending a transition.
        /// </summary>
        public TransitionFrame TransitionFrame { get; internal set; }
    }
}
