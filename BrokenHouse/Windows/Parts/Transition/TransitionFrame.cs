using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// Provides a control that is used as a container for all content that is being transitioned.
    /// </summary>
    /// <remarks>
    /// This control provides a very similar role to the <see cref="System.Windows.Controls.ContentControl"/>; however,
    /// this class does not add any content to the logical tree. The reason for this is that this control is transient
    /// and will be removed when any animation associated with it ends and any content that it displays needs to be kept
    /// in its own logical tree. as any future transition will involve a different <see cref="TransitionFrame"/>.
    /// </remarks>
    public class TransitionFrame : ContentControl
    {
        /// <summary>
        /// Identifies the <see cref="TransitionEffect"/> dependency property key. 
        /// </summary>
        private static readonly DependencyPropertyKey TransitionEffectPropertyKey;

        /// <summary>
        /// Identifies the <see cref="TransitionEffect"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TransitionEffectProperty;

        /// <summary>
        /// Static constructor
        /// </summary>
        static TransitionFrame()
        {
            TransitionEffectPropertyKey = DependencyProperty.RegisterReadOnly("TransitionEffect", typeof(TransitionEffect), typeof(TransitionFrame), new FrameworkPropertyMetadata(null, null));
            TransitionEffectProperty = TransitionEffectPropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionFrame), new FrameworkPropertyMetadata(TransitionElements.TransitionFrameEmptyStyleKey));

            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(TransitionFrame), new FrameworkPropertyMetadata(false));        
        }

        /// <summary>
        /// Gets or sets the transition effect that controls this <see cref="TransitionFrame"/>. This is a dependency property.
        /// </summary>
        public TransitionEffect TransitionEffect
        {
            get { return (TransitionEffect)GetValue(TransitionEffectProperty); }
            internal set { SetValue(TransitionEffectPropertyKey, value); }
        }

        /// <summary>
        /// Called when the <see cref="System.Windows.Controls.ContentControl.Content"/> property changes. 
        /// </summary>
        /// <remarks>
        /// This implementation will not call the base classes version of this method because we do not
        /// want the content to be added as a logical child.
        /// </remarks>
        /// <param name="oldContent">The old value of the <see cref="System.Windows.Controls.ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="System.Windows.Controls.ContentControl.Content"/> property.</param>
        [SecuritySafeCritical]
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            // Do not call the base classes version.
            // We do not want the content to be added as a logical child
        }
    }
}
