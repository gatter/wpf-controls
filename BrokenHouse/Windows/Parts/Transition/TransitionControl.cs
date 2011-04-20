using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrokenHouse.Windows.Parts.Transition;
using BrokenHouse.Windows.Parts.Transition.Primitives;

namespace BrokenHouse.Windows.Parts.Transition
{
    /// <summary>
    /// Provides a control that can transitions its content when changed.
    /// </summary>
    public class TransitionControl : ContentControl
    {
        #region --- Dependancy Properties & Routed Events ---

        /// <summary>
        /// Identifies the <see cref="TransitionEffect"/> dependency property. 
        /// </summary>
        public static DependencyProperty     TransitionEffectProperty;

        #endregion

        /// <summary>
        /// The presenter that will do the actual animation
        /// </summary>
        private TransitionPresenter          m_TransitionPresenter = null;

        /// <summary>
        /// The target that has been supplied before we are fully initialised
        /// </summary>
        private object                       m_PendingTarget    = null;

        static TransitionControl()
        {
            TransitionEffectProperty = TransitionPresenter.TransitionEffectProperty.AddOwner(typeof(TransitionControl), new FrameworkPropertyMetadata(null));
          
            // Override the style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionControl), new FrameworkPropertyMetadata(TransitionElements.TransitionControlStyleKey));    
        
            // Navigation
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(TransitionControl), new FrameworkPropertyMetadata(false)); 
        }
       

        #region --- Properties ---

        /// <summary>
        /// Gets or sets the <see cref="TransitionEffect"/> that will define and control the transitions.
        /// This is a dependency property. 
        /// </summary>      
        public TransitionEffect TransitionEffect
        {
            get { return (TransitionEffect)GetValue(TransitionEffectProperty); }
            set { SetValue(TransitionEffectProperty, value); }
        }


        #endregion

        #region --- Event Handlers ---

        /// <summary>
        /// The content has changed - transition to this new content.
        /// </summary>
        /// <param name="oldContent">The new content</param>
        /// <param name="newContent">The old content</param>
        [SecuritySafeCritical]
        protected override void OnContentChanged(object oldContent, object newContent)
        {
 	         base.OnContentChanged(oldContent, newContent);

            if (m_TransitionPresenter == null)
            {
                m_PendingTarget = newContent;
            }
            else
            {
                m_TransitionPresenter.DoTransition(newContent, TransitionDirection.Forwards);
            }
        }

        /// <summary>
        /// The template has been applied - search for our transition presenter
        /// </summary>
        [SecuritySafeCritical]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_TransitionPresenter = GetTemplateChild("PART_TransitionPresenter") as TransitionPresenter;

            if (m_PendingTarget != null)
            {
                m_TransitionPresenter.DoTransition(m_PendingTarget, TransitionDirection.Forwards);
                m_PendingTarget = null;
            }
        }


 
        #endregion
       


    }
}
