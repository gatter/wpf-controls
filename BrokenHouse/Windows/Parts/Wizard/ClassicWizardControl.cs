using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using BrokenHouse.Extensions;
using BrokenHouse.Windows.Interop;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Parts.Wizard
{
    /// <summary>
    /// The classic wizard control is an extension to the standard wizard control that has
    /// styling to match the Wizard97 specification.
    /// </summary>
    public class ClassicWizardControl : WizardControl
    {
        
        /// <summary>
        /// Identifies the <see cref="ContentPageStyle"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     ContentPageStyleProperty;
        
        /// <summary>
        /// Identifies the <see cref="TitlePageStyle"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     TitlePageStyleProperty;

        /// <summary>
        /// Static Constructor
        /// </summary>
        static ClassicWizardControl()
        {
            // Define the visible properties
            ContentPageStyleProperty   = DependencyProperty.Register("ContentPageStyle", typeof(Style), typeof(ClassicWizardControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnContentPageStyleChangedThunk), null);
            TitlePageStyleProperty     = DependencyProperty.Register("TitlePageStyle", typeof(Style), typeof(ClassicWizardControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTitlePageStyleChangedThunk), null);
           
            // Override the style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClassicWizardControl), new FrameworkPropertyMetadata(WizardElements.ClassicWizardStyleKey));
        }
               
        /// <summary>
        /// Gets or sets the style that should be applied to the content wizard pages. This is a dependancy property.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public Style ContentPageStyle
        {
            get { return (Style) GetValue(ContentPageStyleProperty); }
            set { SetValue(ContentPageStyleProperty, value); }
        }   
             
        /// <summary>
        /// Gets or sets the style that should be applied to the title wizard pages. This is a dependancy property.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public Style TitlePageStyle
        {
            get { return (Style) GetValue(TitlePageStyleProperty); }
            set { SetValue(TitlePageStyleProperty, value); }
        }   

        /// <summary>
        /// Called when the <see cref="ContentPageStyle"/> property has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnContentPageStyleChanged( Style oldValue, Style newValue )
        {
        }


        /// <summary>
        /// Called when the <see cref="TitlePageStyle"/> property has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnTitlePageStyleChanged( Style oldValue, Style newValue )
        {
        }
     
        /// <summary>
        /// Static method to trigger the instance <see cref="OnContentPageStyleChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnContentPageStyleChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as ClassicWizardControl).OnContentPageStyleChanged((Style)args.OldValue, (Style)args.NewValue);
        }      
     
        /// <summary>
        /// Static method to trigger the instance <see cref="OnTitlePageStyleChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnTitlePageStyleChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as ClassicWizardControl).OnTitlePageStyleChanged((Style)args.OldValue, (Style)args.NewValue);
        }

        /// <summary>
        /// A Measure has been triggered
        /// </summary>
        /// <remarks>
        /// If any changes have been made to the style of the pages we ensure that they are updated.
        /// </remarks>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            UpdatePageStyles<ClassicWizardContentPage>(ContentPageStyle);
            UpdatePageStyles<ClassicWizardTitlePage>(TitlePageStyle);

            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Called when the number of pages has changed.
        /// </summary>
        /// <remarks>
        /// If new pages have been added then we need to invalidate the measure to trigger 
        /// a check to see if the styles are set appropriately.
        /// </remarks>
        /// <param name="sender">The pages collection that has changed.</param>
        /// <param name="args">The information about which pages that have changed.</param>
        protected override void OnPagesChanged( object sender, NotifyCollectionChangedEventArgs args )
        {
            // Ensure the base class does its stuff
            base.OnPagesChanged(sender, args);

            // Update the styles
            if (args.NewItems != null)
            {
                // Trigger a measure - styles will be then updated
                InvalidateMeasure();
            }
        }
    }
}
