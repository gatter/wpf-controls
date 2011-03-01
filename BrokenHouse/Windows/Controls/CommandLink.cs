using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Interop;

namespace BrokenHouse.Windows.Controls
{
    /// <summary>
    /// A WPF implementation of the Vista Command Link
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Command Link was introduced with Vista as part of the Vista common controls. Unfortunately, the support for the 
    /// standard command link is only available in C# via the Microsoft's Windows API Code Pack. As this does not support
    /// WPF directly and is not available on Windows XP a prure WPF implementation is required.
    /// </para>
    /// <para>
    /// If a <see cref="System.String"/> is provided for the <see cref="Instruction"/> and <see cref="System.Windows.Controls.ContentControl.Content"/> then
    /// a <see cref="System.Windows.DataTemplate"/> built into the default styles will be used to render the
    /// text in the most appropriate manner. If this is not required then you must supply your own (or a null)
    /// <see cref="System.Windows.DataTemplate"/> to the appropriate property.
    /// </para>
    /// </remarks>
    public class CommandLink : Button
    {
        /// <summary>
        /// Identifies the <see cref="Instruction"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     InstructionProperty;

        /// <summary>
        /// Identifies the <see cref="InstructionTemplate"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     InstructionTemplateProperty;

        /// <summary>
        /// Identifies the <see cref="HasInstruction"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     HasInstructionProperty;

        /// <summary>
        /// Identifies the <see cref="HasInstruction"/> dependency property key. 
        /// </summary>
        private static readonly DependencyPropertyKey HasInstructionKey;

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     IconProperty;
        
        /// <summary>
        /// Identifies the <see cref="IconTemplate"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     IconTemplateProperty;

        /// <summary>
        /// Identifies the <see cref="HasIcon"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     HasIconProperty;

        /// <summary>
        /// Identifies the <see cref="HasIcon"/> dependency property key. 
        /// </summary>
        private static readonly DependencyPropertyKey HasIconKey;

        /// <summary>
        /// Identifies the <see cref="IconHasDropShadow"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty     IconHasDropShadowProperty;

        /// <summary>
        /// The static constructor to register the WPF stuff
        /// </summary>
        static CommandLink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandLink), new FrameworkPropertyMetadata(typeof(CommandLink)));

            // Read only properties keys
            HasIconKey                  = DependencyProperty.RegisterReadOnly("HasIcon", typeof(bool), typeof(CommandLink), new FrameworkPropertyMetadata(false));
            HasInstructionKey           = DependencyProperty.RegisterReadOnly("HasInstruction", typeof(bool), typeof(CommandLink), new FrameworkPropertyMetadata(false));
            HasIconProperty             = HasIconKey.DependencyProperty;
            HasInstructionProperty      = HasInstructionKey.DependencyProperty;
            
            // Other properties
            InstructionTemplateProperty = DependencyProperty.Register("InstructionTemplate", typeof(DataTemplate), typeof(CommandLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            InstructionProperty         = DependencyProperty.Register("Instruction", typeof(object), typeof(CommandLink), new PropertyMetadata("Instruction", OnInstructionChangedThunk));
            IconProperty                = DependencyProperty.Register("Icon", typeof(object), typeof(CommandLink), new PropertyMetadata(TaskIcons.Arrow, OnIconChangedThunk));
            IconTemplateProperty        = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(CommandLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            IconHasDropShadowProperty   = DependencyProperty.Register("IconHasDropShadow", typeof(bool), typeof(CommandLink), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(CommandLink), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(CommandLink), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(CommandLink), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
        }

        
        #region --- Property Changed Event handlers ---

        /// <summary>
        /// Called when the <see cref="Instruction"/> property changes.
        /// </summary>
        /// <param name="oldValue">The new value of the instruction</param>
        /// <param name="newValue">The olf value of the instruction</param>
        protected virtual void OnInstructionChanged( object oldValue, object newValue )
        {
            RemoveLogicalChild(oldValue);
            AddLogicalChild(newValue);

            HasInstruction = (newValue != null);
        }
      
        /// <summary>
        /// Static method to trigger the instance <see cref="OnInstructionChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnInstructionChangedThunk( object target, DependencyPropertyChangedEventArgs args )
        {
            (target as CommandLink).OnInstructionChanged(args.OldValue, args.NewValue);
        }
        
        /// <summary>
        /// Called when the <see cref="Icon"/> property changes
        /// </summary>
        /// <param name="oldValue">The new value of the icon</param>
        /// <param name="newValue">The olf value of the icon</param>
        protected virtual void OnIconChanged( object oldValue, object newValue )
        {
            RemoveLogicalChild(oldValue);
            AddLogicalChild(newValue);

            HasIcon = (newValue != null);
        }
      
        /// <summary>
        /// Static method to trigger the instance <see cref="OnIconChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnIconChangedThunk( object target, DependencyPropertyChangedEventArgs args )
        {
            (target as CommandLink).OnIconChanged(args.OldValue, args.NewValue);
        }

        #endregion    

        #region --- Public Properties ---

        /// <summary>
        /// Gets or sets the title content of the <see cref="CommandLink"/>. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// The default styles will cause a <see cref="System.String"/> to be rendered in the appropriate style, for 
        /// example the Aero style will display the instruction in large bold text. 
        /// If a different way of presenting a <see cref="System.String"/> is required then you must
        /// supply a new <see cref="InstructionTemplate"/>.
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public object Instruction
        {
            get { return GetValue(InstructionProperty); }
            set { SetValue(InstructionProperty, value); }
        }
 
        /// <summary>
        /// Gets the flag used to indicate that the <see cref="CommandLink"/> has been supplied <see cref="Instruction"/> content.
        /// This is a dependency property. 
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public bool HasInstruction
        {
            get { return (bool)GetValue(HasInstructionProperty); }
            set { SetValue(HasInstructionKey, value); }
        }

        /// <summary>
        /// Gets or sets the content to be used for the image in the <see cref="CommandLink"/>.
        /// This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// This property can be assigned any object. However, if a  
        /// <see cref="System.Windows.Media.Imaging.BitmapSource"/> is supplied
        /// then a <see cref="System.Windows.DataTemplate"/> resource will be used to create
        /// <see cref="Icon"/> visual (of 16x16 pixels) to display the 
        /// <see cref="System.Windows.Media.Imaging.BitmapSource"/>. If a different way of presenting
        /// the <see cref="System.Windows.Media.Imaging.BitmapSource"/> is required then you must
        /// supply a new <see cref="IconTemplate"/>.
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

 
        /// <summary>
        /// Gets the flag used to indicate that the <see cref="CommandLink"/> has been supplied <see cref="Icon"/> content.
        /// This is a dependency property. 
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public bool HasIcon
        {
            get { return (bool)GetValue(HasIconProperty); }
            set { SetValue(HasIconKey, value); }
        }
       
        /// <summary>
        /// Gets or Sets the template that will be applied to the <see cref="Instruction"/>.
        /// This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// Set this property to a <see cref="System.Windows.DataTemplate"/> to control the appearance of
        /// the <see cref="Instruction"/>.
        /// </para>
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public DataTemplate InstructionTemplate
        {
            get { return (DataTemplate)GetValue(InstructionTemplateProperty); }
            set { SetValue(InstructionTemplateProperty, value); }
        }
    
        
        /// <summary>
        /// Gets or Sets the template that will be applied to the icon.
        /// This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// Set this property to a <see cref="System.Windows.DataTemplate"/> to control the appearance of
        /// the <see cref="Icon"/>.
        /// </para>
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }
    
     
        /// <summary>
        /// Gets or Sets whether the icon in the <see cref="CommandLink"/> should be rendered with a drop shaddow.
        /// This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property effects the rendering of the content supplied for the <see cref="Icon"/>.
        /// </para>
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public bool IconHasDropShadow
        {
            get { return (bool)GetValue(IconHasDropShadowProperty); }
            set { SetValue(IconHasDropShadowProperty, value); }
        }
    
        #endregion
    }


}
