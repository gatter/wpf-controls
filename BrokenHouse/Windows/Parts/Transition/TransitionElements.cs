using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BrokenHouse.Windows.Parts.Transition.Primitives;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
     /// </summary>
    public static class TransitionElements
    {
        /// <summary>
        /// The resource key used to identify the style of the <see cref="TransitionFrame"/>.
        /// </summary>
        public static ComponentResourceKey TransitionFrameStyleKey              {get; private set;}
        /// <summary>
        /// The resource key used to identify the style of the <see cref="TransitionControl"/>.
        /// </summary>
        public static ComponentResourceKey TransitionControlStyleKey            {get; private set;}
        /// <summary>
        /// The resource key used to identify the style of an empty <see cref="TransitionFrame"/>.
        /// </summary>
        public static ComponentResourceKey TransitionFrameEmptyStyleKey  {get; private set;}

        /// <summary>
        /// Create all the resource keys so that they can be used in the XAML.
        /// </summary>
        static TransitionElements()
        {
            TransitionFrameStyleKey      = new ComponentResourceKey(typeof(TransitionFrame),    "Style");
            TransitionControlStyleKey    = new ComponentResourceKey(typeof(TransitionControl),  "Style");
            TransitionFrameEmptyStyleKey = new ComponentResourceKey(typeof(TransitionFrame),    "EmptyStyle");
        }
    }
}
