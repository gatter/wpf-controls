using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace BrokenHouse.Windows.Parts.Wizard.Primitives
{
    /// <summary>
    /// This is a control that is used in the layout of the header of the <see cref="ClassicWizardContentPage"/>.
    /// </summary>
    public class ClassicWizardHeader : Control
    {
        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty    TitleProperty;

        /// <summary>
        /// Identifies the Description dependency property.
        /// </summary>
        public static readonly DependencyProperty    DescriptionProperty;

        #region --- Constructor ---	
	
        /// <summary>
        /// Static constructor
        /// </summary>
		static ClassicWizardHeader()
		{
            // Properties
            DescriptionProperty  = DependencyProperty.Register("Description", typeof(string), typeof(ClassicWizardHeader), new FrameworkPropertyMetadata("Sub Heading", null), null);
            TitleProperty        = DependencyProperty.Register("Title",      typeof(string), typeof(ClassicWizardHeader), new FrameworkPropertyMetadata("Heading", null), null);

            // Define the default style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClassicWizardHeader), new FrameworkPropertyMetadata(WizardElements.ClassicWizardHeaderStyleKey));
         
            // Override the keyboard navigation
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(ClassicWizardHeader), new FrameworkPropertyMetadata(false));
        }

        #endregion
        
        #region --- Public Properties ---

        /// <summary>
        /// Gets or Sets the title for this page.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the description for this page.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        #endregion

    }
}
