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

namespace BrokenHouse.Windows.Parts.Wizard
{
    /// <summary>
    /// This class provides a classic title page for the classic wizard.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Wizard97 specification defines that a wizard should have an welcome and an the page; these pages have a different style to the interior pages. 
    /// </para>
    /// <para>
    /// The <see cref="ClassicWizardTitlePage"/> is an extension to <see cref="WizardPage"/> in that it provides the addition functionality that allows
    /// a wizard page to be style according to the welcome and completion pages of the Wizard97 standard. The Wizard97 standard states that the 
    /// welcome and completion pages contain a graphical area to the left of the wizard. 
    /// </para>
    /// <para>
    /// There are two main ways in which the <see cref="ClassicWizardTitlePage"/> can be styled to meet the needs of the developer. 
    /// The first is to use the <see cref="Watermark"/> property to supply the content of the graphic area and the <see cref="WatermarkBackground"/>
    /// to define the background. The second is to use a <see cref="System.Windows.DataTemplate"/> for the <see cref="WatermarkTemplate"/> to
    /// define how the <see cref="Watermark"/> appears within the graphic area. The advantage of using the latter is that the
    /// <see cref="WatermarkTemplate"/> can be defined the appearance of the graphic area in a application wide style.
    /// </para>
    /// </remarks>
    [DefaultEvent("PageActivatingEvent")]
    public class ClassicWizardTitlePage : WizardPage
    {
        /// <summary>
        /// Identifies the <see cref="Watermark"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     WatermarkProperty;
        /// <summary>
        /// Identifies the <see cref="WatermarkTemplate"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     WatermarkTemplateProperty;
        /// <summary>
        /// Identifies the <see cref="VerticalWatermarkAlignment"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     VerticalWatermarkAlignmentProperty;
        /// <summary>
        /// Identifies the <see cref="HorizontalWatermarkAlignment"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     HorizontalWatermarkAlignmentProperty;
        /// <summary>
        /// Identifies the <see cref="WatermarkSeparatorStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     WatermarkSeparatorStyleProperty;
        /// <summary>
        /// Identifies the <see cref="WatermarkBackground"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     WatermarkBackgroundProperty;
        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     TitleProperty;
        /// <summary>
        /// Identifies the <see cref="HasWatermark"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     HasWatermarkProperty;
        /// <summary>
        /// Identifies the <see cref="HasWatermark">HasWatermark</see> dependency key. 
        /// </summary>
        private static readonly DependencyPropertyKey HasWatermarkKey;

        /// <summary>
        /// Constructo to register the WPF stuff
        /// </summary>
        static ClassicWizardTitlePage()
        {
            // Read only properties keys
            HasWatermarkKey                      = DependencyProperty.RegisterReadOnly("HasWatermark", typeof(bool), typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(false));
            HasWatermarkProperty                 = HasWatermarkKey.DependencyProperty;

            // Read/Write Properties
            WatermarkProperty                    = DependencyProperty.Register("Watermark", typeof(object), typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnWatermarkChangedThunk));
            WatermarkTemplateProperty            = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            HorizontalWatermarkAlignmentProperty = DependencyProperty.Register("HorizontalWatermarkAlignment", typeof(HorizontalAlignment), typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            VerticalWatermarkAlignmentProperty   = DependencyProperty.Register("VerticalWatermarkAlignment", typeof(VerticalAlignment), typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            WatermarkBackgroundProperty          = DependencyProperty.Register("WatermarkBackground", typeof(Brush), typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnWatermarkChangedThunk));
            WatermarkSeparatorStyleProperty      = DependencyProperty.Register("WatermarkSeparatorStyle", typeof(WizardSeparatorStyle), typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(WizardSeparatorStyle.Etched, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            TitleProperty                        = DependencyProperty.Register("Title", typeof(string), typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

            // Define the default style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(WizardElements.ClassicTitlePageStyleKey));
        }
        
        #region --- Public Properties ---

        /// <summary>
        /// Gets whether this page has a watermark.
        /// This is a dependancy property.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public bool HasWatermark
        {
            get { return (bool)GetValue(HasWatermarkProperty); }
            private set { SetValue(HasWatermarkKey, value); }
        }

        /// <summary>
        /// Gets or sets the content of the watermark that be used for the title page.
        /// This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The wartermark is the content that will appear in the graphic area to the lefthand side of the 
        /// page. The position of the watermark in the graphic area is defined by the 
        /// <see cref="HorizontalWatermarkAlignment"/> and the <see cref="VerticalWatermarkAlignment"/>.
        /// </para>
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public object Watermark
        {
            get { return GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
   
        /// <summary>
        /// Gets or sets the template that will control the appearance of the <see cref="Watermark"/>.
        /// This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This template can be defined in a style (at the application level) to give all the title pages
        /// a consistant appearance. For example, the style could contain a template that 
        /// defines the basic color for all title pages. This template could then place 
        /// the <see cref="Watermark"/> content in the top right corner to provide a contextual reference
        /// to the title page.
        /// </para>
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }
    
        /// <summary>
        /// Gets or sets the horizontal alignment of the watermark content within the graphical area
        /// of the Title page.
        /// This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// If the <see cref="Watermark"/> content is not intended to take up the whole 
        /// graphical area then this property along with the <see cref="VerticalWatermarkAlignment"/>
        /// property can be used to position it correctly.
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public HorizontalAlignment HorizontalWatermarkAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalWatermarkAlignmentProperty); }
            set { SetValue(HorizontalWatermarkAlignmentProperty, value); }
        }
     
        /// <summary>
        /// Gets or sets the vertical alignment of the watermark within the graphical area of the Title page.
        /// This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// If the <see cref="Watermark"/> content is not intended to take up the whole 
        /// graphical area then this property along with the <see cref="HorizontalWatermarkAlignment"/>
        /// property can be used to position it correctly.
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public VerticalAlignment VerticalWatermarkAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalWatermarkAlignmentProperty); }
            set { SetValue(VerticalWatermarkAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background of the graphical area.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public Brush WatermarkBackground
        {
            get { return (Brush)GetValue(WatermarkBackgroundProperty); }
            set { SetValue(WatermarkBackgroundProperty, value); }
        }
    
        /// <summary>
        /// Gets or sets the style of the separator that defines how the watermark should be separated from content of the title page.
        /// This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value assigned to the <see cref="WatermarkSeparatorStyle"/> will depend on the source of the watermark. If the watermark
        /// contains a graphical element that visually separates the graphical area from the content then this property
        /// should be set to <see cref="WizardSeparatorStyle.None"/>. 
        /// </para>
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public WizardSeparatorStyle WatermarkSeparatorStyle
        {
            get { return (WizardSeparatorStyle)GetValue(WatermarkSeparatorStyleProperty); }
            set { SetValue(WatermarkSeparatorStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Title for this page. This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// The title, by default, is positioned above the content of the wizard page.
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion    
    
        #region --- Property Changed Event handlers ---

        /// <summary>
        /// Called when the <see cref="Watermark"/> property has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnWatermarkChanged( object oldValue, object newValue )
        {
            RemoveLogicalChild(oldValue);
            AddLogicalChild(newValue);

            HasWatermark = (newValue != null);
        }
     
        /// <summary>
        /// Static method to trigger the instance <see cref="OnWatermarkChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnWatermarkChangedThunk( object target, DependencyPropertyChangedEventArgs args )
        {
            ClassicWizardTitlePage page = target as ClassicWizardTitlePage;

            page.OnWatermarkChanged(args.OldValue, args.NewValue);
        }
        
        #endregion    
    }


}
