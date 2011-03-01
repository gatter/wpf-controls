using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using BrokenHouse.Windows.Data;
using BrokenHouse.Windows.Parts.Wizard;

namespace DemoApplication.Demos.Wizard.Connection
{
    /// <summary>
    /// A very simple behaviour that provides additional properties required for sequenced wizard pages
    /// </summary>
    public static class SequencedWizardPageBehaviour
    {
        /// <summary>
        /// Identifies the <see cref="IsPending"/> attached property key.
        /// </summary>
        private static readonly DependencyPropertyKey IsPendingPropertyKey;

        /// <summary>
        /// Identifies the <see cref="IsSequenced"/>IsSequenced attached property.
        /// </summary>
        public static readonly DependencyProperty IsSequencedProperty;

        /// <summary>
        /// Identifies the <see cref="IsPending"/> attached property key.
        /// </summary>
        public static readonly DependencyProperty IsPendingProperty;
        
        /// <summary>
        /// Register the WPF properties
        /// </summary>
        static SequencedWizardPageBehaviour()
        {
            IsPendingPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsPending", typeof(bool), typeof(SequencedWizardPageBehaviour), new FrameworkPropertyMetadata(false));
            IsPendingProperty    = IsPendingPropertyKey.DependencyProperty;
            IsSequencedProperty  = DependencyProperty.RegisterAttached("IsSequenced", typeof(bool), typeof(SequencedWizardPageBehaviour), new FrameworkPropertyMetadata(false, OnIsSequencedChangedThunk));
        }

        /// <summary>
        /// Gets the flag indicating that this behaviour is attached to the control
        /// </summary>
        /// <param name="control">The WizardControl to which this behaviour might be attached.</param>
        /// <returns><b>true</b> if this behaviour is attached to the wizard.</returns>
        public static bool GetIsSequenced( WizardControl control )
        {
            return (bool)control.GetValue(IsSequencedProperty);
        }

        /// <summary>
        /// Sets the flag indicating that this behaviour is attached to the control
        /// </summary>
        /// <param name="control">The WizardControl to which this behaviour will be attached (or detached).</param>
        /// <param name="value"><b>true</b> if this behaviour is to be attached to the page.</param>
        public static void SetIsSequenced( WizardControl control, bool value )
        {
            control.SetValue(IsSequencedProperty, value);
        }

        /// <summary>
        /// Gets the flag indicating that the page is pending in the sequence of pages
        /// </summary>
        /// <param name="page">The WizardPage</param>
        /// <returns><b>true</b> if the page is pending (i.e. not been seen yet).</returns>
        public static bool GetIsPending( WizardPage page )
        {
            return (bool)page.GetValue(IsPendingProperty);
        }

        /// <summary>
        /// The <c>IsSequenced</c> attached property has changed. This signifies that the behaviour should be
        /// attached/detached from the control
        /// </summary>
        /// <param name="target">The WizardControl to which this behaviour will be attached/detached.</param>
        /// <param name="args">Information on the change flag</param>
        private static void OnIsSequencedChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            WizardControl control = target as WizardControl;

            if (control != null)
            {
                if ((bool)args.NewValue)
                {
                    control.ActiveIndexChanged += new RoutedPropertyChangedEventHandler<int>(OnActiveIndexChanged);
                    control.Pages.CollectionChanged += new NotifyCollectionChangedEventHandler(OnPagesCollectionChanged);
                }
                else
                {
                    control.ActiveIndexChanged -= new RoutedPropertyChangedEventHandler<int>(OnActiveIndexChanged);
                    control.Pages.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnPagesCollectionChanged);
                }
            }
        }

        /// <summary>
        /// Called when the pages collection in the attached WizardControl has changed - need to make sure
        /// that the pending flag is set correctly on all pages.
        /// </summary>
        /// <param name="sender">The Pages collection that has changed</param>
        /// <param name="e">Information on what has actually changed.</param>
        static void OnPagesCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            CompoundCollectionView collection  = sender as CompoundCollectionView;

            if ((collection != null) && (collection.Count > 0))
            {
                WizardControl control = (collection[0] as WizardPage).ParentWizard;

                if (control != null)
                {
                    foreach (WizardPage page in collection)
                    {
                        page.SetValue(IsPendingPropertyKey, (collection.IndexOf(page) > control.ActiveIndex));
                    }
                }
            }
        }

        /// <summary>
        /// Called when the ActiveIndex has changed on the WizardControl
        /// </summary>
        /// <param name="sender">The WizardControl whose ActiveIndex has changed</param>
        /// <param name="e">Information on the new and old values of the property</param>
        static void  OnActiveIndexChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            WizardControl control  = sender as WizardControl;

            if (control != null)
            {
                foreach (WizardPage page in control.Pages)
                {
                    page.SetValue(IsPendingPropertyKey, (control.Pages.IndexOf(page) > e.NewValue));
                }
            }
        } 
    }
}
