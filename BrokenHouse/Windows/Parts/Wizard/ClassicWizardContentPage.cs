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
using System.Windows.Media;
using System.Windows.Input;
using  BrokenHouse.Windows.Parts.Wizard.Primitives;

namespace BrokenHouse.Windows.Parts.Wizard
{
     /// <summary>
    /// This class provides a classic content page for the classic wizard.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Wizard97 specification defines that a wizard should have content pages that should meet a specific format. This class provides
    /// a style with additional properties that conforms to that specification.
    /// </para>
    /// </remarks>
    [DefaultEvent("PageActivatingEvent")]
    public class ClassicWizardContentPage : WizardPage
    {
        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property. 
        /// </summary>
        public  static readonly DependencyProperty    TitleProperty;
        /// <summary>
        /// Identifies the <see cref="Description"/> dependency property. 
        /// </summary>
        public  static readonly DependencyProperty    DescriptionProperty;
        /// <summary>
        /// Identifies the <see cref="UseWideMargins"/> dependency property. 
        /// </summary>
        public  static readonly DependencyProperty    UseWideMarginsProperty;
        /// <summary>
        /// Identifies the <see cref="BannerPadding"/> dependency property. 
        /// </summary>
        public  static readonly DependencyProperty    BannerPaddingProperty;
        /// <summary>
        /// Identifies the <see cref="Banner"/> dependency property. 
        /// </summary>
        public  static readonly DependencyProperty    BannerProperty;
        /// <summary>
        /// Identifies the <see cref="BannerTemplate"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     BannerTemplateProperty;
        /// <summary>
        /// Identifies the <see cref="BannerBackground"/> dependency property. 
        /// </summary>
        public  static readonly DependencyProperty    BannerBackgroundProperty;
        /// <summary>
        /// Identifies the <see cref="BannerSeparatorStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     BannerSeparatorStyleProperty;
        /// <summary>
        /// Identifies the <see cref="VerticalBannerAlignment"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     VerticalBannerAlignmentProperty;
        /// <summary>
        /// Identifies the <see cref="HorizontalBannerAlignment"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     HorizontalBannerAlignmentProperty;
        /// <summary>
        /// Identifies the <see cref="HasBanner"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     HasBannerProperty;
        /// <summary>
        /// Identifies the <see cref="HasBanner"/> dependency key. 
        /// </summary>
        private static readonly DependencyPropertyKey HasBannerKey;

        /// <summary>
        /// Static constructor to register the WPF properties and override the the default style metadata.
        /// </summary>
        static ClassicWizardContentPage()
        {
            // Read only properties keys
            HasBannerKey                      = DependencyProperty.RegisterReadOnly("HasBanner", typeof(bool), typeof(ClassicWizardTitlePage), new FrameworkPropertyMetadata(false));

            // Define the additional content
            DescriptionProperty               = ClassicWizardHeader.DescriptionProperty.AddOwner(typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            TitleProperty                     = ClassicWizardHeader.TitleProperty.AddOwner(typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            BannerProperty                    = DependencyProperty.Register("Banner", typeof(object), typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnBannerChangedThunk));
            BannerTemplateProperty            = DependencyProperty.Register("BannerTemplate", typeof(DataTemplate), typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            BannerPaddingProperty             = DependencyProperty.Register("BannerPadding", typeof(Thickness), typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(new Thickness(0.0), FrameworkPropertyMetadataOptions.AffectsRender));
            BannerBackgroundProperty          = DependencyProperty.Register("BannerBackground", typeof(Brush), typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            BannerSeparatorStyleProperty      = DependencyProperty.Register("BannerSeparatorStyle", typeof(WizardSeparatorStyle), typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(WizardSeparatorStyle.Etched, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            UseWideMarginsProperty            = DependencyProperty.Register("UseWideMargins", typeof(bool), typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, OnUseWideMarginsChangedThunk));
            HorizontalBannerAlignmentProperty = DependencyProperty.Register("HorizontalBannerAlignment", typeof(HorizontalAlignment), typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(HorizontalAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            VerticalBannerAlignmentProperty   = DependencyProperty.Register("VerticalBannerAlignment", typeof(VerticalAlignment), typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            HasBannerProperty                 = HasBannerKey.DependencyProperty;

            // Define the default style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClassicWizardContentPage), new FrameworkPropertyMetadata(WizardElements.ClassicContentPageStyleKey));
        }
        
        #region --- Event Handlers ---

        /// <summary>
        /// The UseWideMargins property has changed. We need to make sure that things are arranged correctly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnUseWideMarginsChangedThunk( object sender, DependencyPropertyChangedEventArgs args )
        {
            ClassicWizardContentPage page      = sender as ClassicWizardContentPage;
            ContentPresenter         presenter = page.GetTemplateChild("PART_Content") as ContentPresenter;
        
            if (presenter != null)
            {
                (presenter.Parent as FrameworkElement).InvalidateMeasure();
            }
        }

        /// <summary>
        /// Called when the <see cref="Banner"/> property has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnBannerChanged( object oldValue, object newValue )
        {
            RemoveLogicalChild(oldValue);
            AddLogicalChild(newValue);

            HasBanner = (newValue != null);
        }
    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnBannerChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnBannerChangedThunk( object target, DependencyPropertyChangedEventArgs args )
        {
            ClassicWizardContentPage page = target as ClassicWizardContentPage;

            page.OnBannerChanged(args.OldValue, args.NewValue);
        }

        #endregion

        #region --- Public Properties ---

        /// <summary>
        /// Gets or sets whether wide margins are to be used in this wizard page. This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// The Wizard97 specification recommends that wide margins should be used for all
        /// standard content. It says that normal margins should only be used when the page contains particularly 
        /// wide content, such as side by side list boxes. By default wide margins are used unless this property
        /// is explicity set to <c>False</c>.
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public bool UseWideMargins
        {
            get { return (bool)GetValue(UseWideMarginsProperty); }
            set { SetValue(UseWideMarginsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title for this page. This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// The <see cref="Title"/> is the main text that appears in a larger and bold font in the banner across the top
        /// of the wizard page.
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the description of the wizard page. This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The description appears under the main title in the banner accross the top of the page. The description
        /// will span two rows if necessary; however, as the Wizard97 specification only allows for two rows the
        /// description text will be truncated if the text extends beyond this limit.
        /// </para>
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the content of the banner. This will appear underneith the header <see cref="Title"/> and <see cref="Description"/>.
        /// This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// A <see cref="ClassicWizardContentPage"/> will be displayed with a header; this header consists of a
        /// <see cref="Description"/> and <see cref="Title"/> which are displayed ontop of the banner. A common use 
        /// of this property is to set it to an image and then use the <see cref="BannerPadding"/> to stop the content of the
        /// header being rendered over the image. 
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public object Banner
        {
            get { return GetValue(BannerProperty); }
            set { SetValue(BannerProperty, value); }
        }    
       
        /// <summary>
        /// Gets whether this page has been supplied a banner. This is a dependancy property.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public bool HasBanner
        {
            get { return (bool)GetValue(HasBannerProperty); }
            private set { SetValue(HasBannerKey, value); }
        }

        /// <summary>
        /// Gets or sets the template that will be applied to the <see cref="Banner"/>. This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Set this property to a <see cref="System.Windows.DataTemplate"/> to control the appearance of
        /// the banner.
        /// </para>
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public DataTemplate BannerTemplate
        {
            get { return (DataTemplate)GetValue(BannerProperty); }
            set { SetValue(BannerProperty, value); }
        }
      
        /// <summary>
        /// Gets or sets the horizontal alignment of the banner content within the header
        /// section of the page. This is a dependancy property.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public HorizontalAlignment HorizontalBannerAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalBannerAlignmentProperty); }
            set { SetValue(HorizontalBannerAlignmentProperty, value); }
        }
     
        /// <summary>
        /// Gets or sets the vertical alignment of the banner content within the header
        /// section of the page. This is a dependancy property.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public VerticalAlignment VerticalBannerAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalBannerAlignmentProperty); }
            set { SetValue(VerticalBannerAlignmentProperty, value); }
        }
     
        /// <summary>
        /// Gets or sets the amount of space that is reserved for the <see cref="Banner"/>. 
        /// </summary>
        /// <remarks>
        /// This property is mainly used to stop the header text being rendered on top
        /// of key visual elements of the banner.
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public Thickness BannerPadding
        {
            get { return (Thickness)GetValue(BannerPaddingProperty); }
            set { SetValue(BannerPaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background of the banner. This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// The banner background is the background that will be used for the header.
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public Brush BannerBackground
        {
            get { return (Brush)GetValue(BannerBackgroundProperty); }
            set { SetValue(BannerBackgroundProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the style of the banner separator. This is a dependancy property.
        /// </summary>
        /// <remarks>
        /// The banner separator defines how the banner should be separated from content of the page.
        /// </remarks>
        [Category("Appearance"), Bindable(true)]
        public WizardSeparatorStyle BannerSeparatorStyle
        {
            get { return (WizardSeparatorStyle)GetValue(BannerSeparatorStyleProperty); }
            set { SetValue(BannerSeparatorStyleProperty, value); }
        }

        #endregion
    }
}
