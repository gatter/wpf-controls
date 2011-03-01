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
    /// The aero wizard control is an extension to the standard wizard control that is
    /// styling to match the Areo wizard found in Vista and Windows 7.
    /// </summary>
    public class AeroWizardControl : WizardControl
    {
        /// <summary>
        /// Identifies the Title dependency property
        /// </summary>
        public static readonly DependencyProperty  TitleProperty;
        
        /// <summary>
        /// Identifies the Icon dependency property
        /// </summary>
        public static readonly DependencyProperty  IconProperty;
        
        /// <summary>
        /// Identifies the <see cref="PageStyle"/> dependency property
        /// </summary>
        public  static readonly DependencyProperty     PageStyleProperty;

        static AeroWizardControl()
        {
            // Define the visible properties
            TitleProperty         = DependencyProperty.Register("Title", typeof(string), typeof(AeroWizardControl), new FrameworkPropertyMetadata(null), null);
            IconProperty          = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(AeroWizardControl), new FrameworkPropertyMetadata(null, null), null);
            PageStyleProperty     = DependencyProperty.Register("PageStyle", typeof(Style), typeof(AeroWizardControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnPageStyleChangedThunk), null);
           
            // Override the style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AeroWizardControl), new FrameworkPropertyMetadata(WizardElements.AeroWizardStyleKey));
        }
               
        /// <summary>
        /// Gets or sets the style that should be applied to the wizard pages. This is a depedency property.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public Style PageStyle
        {
            get { return (Style) GetValue(PageStyleProperty); }
            set { SetValue(PageStyleProperty, value); }
        }         

        /// <summary>
        /// Gets or sets the title of the Aero Wizard. This is a depedency property.
        /// </summary>
        /// <remarks>
        /// When in the Aero theme the Aero wizard shows this title just to the right of the Back
        /// navigation button. The standard window title will only be shown in the task bar at the 
        /// bottom of the screen. In classic (and XP themes) both titles are shown.
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

       /// <summary>
        /// Gets or sets the icon of the Aero Wizard. This is a depedency property.
        /// </summary>
        /// <remarks>
        /// When in the Aero theme the Aero wizard shows this icon just to the right of the Back
        /// navigation button. The standard window icon will only be shown in the task bar at the 
        /// bottom of the screen. In classic (and XP themes) both icons are shown.
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
 
        /// <summary>
        /// Called when the <see cref="PageStyle"/> property has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnPageStyleChanged( Style oldValue, Style newValue )
        {
            InvalidateMeasure();
        }
    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnPageStyleChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnPageStyleChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as AeroWizardControl).OnPageStyleChanged((Style)args.OldValue, (Style)args.NewValue);
        }        
        
        /// <summary>
        /// Measures the control.
        /// </summary>
        /// <param name="constraint">The maximum size limit for the control.</param>
        /// <returns>The size of the control.</returns>
        protected override Size MeasureOverride( Size constraint )
        {
            UpdatePageStyles<AeroWizardPage>(PageStyle);

            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Called when the <see cref="WizardControl.Pages"/> has changed. 
        /// </summary>
        /// <remarks>
        /// When the <see cref="WizardControl.Pages"/> have changed we have to make sure that the styles are up to date. As the styles are updated
        /// as part of the measuring of the wizard we can trigger a style update by by invalidating the measured size of the control.
        /// </remarks>
        /// <param name="sender">The collection that triggered the event.</param>
        /// <param name="args">The inforamation about the change.</param>
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