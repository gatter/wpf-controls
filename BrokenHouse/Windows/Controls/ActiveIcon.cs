using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Media.Imaging;
using BrokenHouse.Windows;

namespace BrokenHouse.Windows.Controls
{
    /// <summary>
    /// Represents a control that displays an icon and links it to the state of a visual ancestor (by default a button).
    /// </summary>
    /// <remarks>
    /// <para>
    /// In the majority of cases icons are usually used as part of a button (for example on a toolbar). In these cases there
    /// is a requirement to visually change the icon depending on the state of the button. For example, if you disable a button
    /// it is also preferrable to remove the color from the icon (desaturate it) to produce a gray icon. This class will also
    /// change the gamma of the icon to make it appear richer in color when the mouse hovers over the attached button.
    /// </para>
    /// <para>
    /// Unfortunately, we cannot use PixelShaders based effect to perform the colour modification because the shader
    /// code conflicts with the nearest neighbor scaling used by the underlying <see cref="Icon"/> control. 
    /// When .Net 4.0 is release there will not be a need to use nearest neighbour
    /// scaling and PixelShaders effects will be used to perform the colour modifications.
    /// </para>
    /// <seealso cref="Icon"/>
    /// </remarks>
    public class ActiveIcon : Control
    {
        /// <summary>
        /// Identifies the <see cref="AttachedType"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AttachedTypeProperty;

        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty;

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SourceProperty;

        /// <summary>
        /// Identifies the <see cref="AttachedElement"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AttachedElementProperty;

        /// <summary>
        /// Identifies the <see cref="AttachedElement"/> dependency property key. 
        /// </summary>
        private static readonly DependencyPropertyKey AttachedElementKey;

        /// <summary>
        /// The current active binding
        /// </summary>
        private Binding ActiveBinding { get; set; }
               
        #region --- Constructors ---

        /// <summary>
        /// Do the WPF Stuff
        /// </summary>
        static ActiveIcon()
        {
            // The properties
            AttachedElementKey      = DependencyProperty.RegisterReadOnly("AttachedElement", typeof(FrameworkElement), typeof(ActiveIcon), new FrameworkPropertyMetadata(null), null);
            IsActiveProperty        = DependencyProperty.Register("IsActive", typeof(bool?), typeof(ActiveIcon), new FrameworkPropertyMetadata(null));
            AttachedTypeProperty    = DependencyProperty.Register("AttachedType", typeof(Type), typeof(ActiveIcon), new FrameworkPropertyMetadata(typeof(ButtonBase), FrameworkPropertyMetadataOptions.None, OnAttachedTypeChangedThunk), null);
            SourceProperty          = DependencyProperty.Register("Source", typeof(BitmapSource), typeof(ActiveIcon), new FrameworkPropertyMetadata(null), null);
            AttachedElementProperty = AttachedElementKey.DependencyProperty;

            // Set the metadata
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ActiveIcon), new FrameworkPropertyMetadata(typeof(ActiveIcon)));
        
            // Override the keyboard navigation
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(ActiveIcon), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ActiveIcon), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(ActiveIcon), new FrameworkPropertyMetadata(false));
        }

        #endregion
        
        /// <summary>
        /// We have been initialized.
        /// </summary>
        /// <remarks>
        /// When we are initialized we have to set up an element handler that will update the bindings when this objects
        /// layout has changed. The bindings will only be updated if the ancestor that we have attached to has changed.
        /// </remarks>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data</param>
        protected override void OnInitialized( EventArgs e )
        {
            // Call the default
            base.OnInitialized(e);

            // Hook up to the event
            LayoutUpdated += delegate { UpdateBinding(); };
        }

        /// <summary>
        /// Update the binding based on the current attached type
        /// </summary>
        private void UpdateBinding()
        {
            UpdateBinding(AttachedType);
        }

        /// <summary>
        /// Update the binding based on the supplied ancestor type.
        /// </summary>
        /// <param name="type">The ancestor type that we want to attach to.</param>
        private void UpdateBinding( Type type )
        {    
            Binding currentBinding = BindingOperations.GetBinding(this, IsActiveProperty);

            // Are we in control of the binding, or there is no binding and is active is not set.
            if ((type != null) && ((currentBinding == ActiveBinding) || ((currentBinding == null) && (IsActive == null))))
            {
                DependencyObject currentSource = (ActiveBinding == null)? null : (ActiveBinding.Source as DependencyObject); 
                FrameworkElement updatedSource = this.EnumerateAncestors().OfType<FrameworkElement>().Where(a => type.IsAssignableFrom(a.GetType())).FirstOrDefault();

                if (currentSource != updatedSource)
                {
                    // Clear the binding
                    BindingOperations.ClearBinding(this, IsActiveProperty);

                    // Is the source valid
                    if (updatedSource != null)
                    {
                        Binding binding = new Binding { Source = updatedSource, Path = new PropertyPath("IsMouseOver") }; 

                        BindingOperations.SetBinding(this, IsActiveProperty, binding);
                    }

                    // Set the attached element
                    AttachedElement = updatedSource;
                }
            }
        }

        /// <summary>
        /// Called when the <see cref="AttachedType"/> property has changed.
        /// </summary>
        /// <remarks>
        /// When the <see cref="AttachedType"/> has changed we have to search
        /// this element's ancestors to find an element of this type. Once found we
        /// bind the ancestor's <see cref="System.Windows.UIElement.IsMouseOver"/>
        /// property to our <see cref="IsActive"/> property.
        /// </remarks>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnAttachedTypeChanged( Type oldValue, Type newValue )
        {
            UpdateBinding(newValue);
        }
      
        /// <summary>
        /// Static method to trigger the instance <see cref="OnAttachedTypeChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnAttachedTypeChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as ActiveIcon).OnAttachedTypeChanged((Type)args.OldValue, (Type)args.NewValue);
        }

        /// <summary>
        /// The visual parent has changed.
        /// </summary>
        /// <remarks>
        /// When the <see cref="System.Windows.Media.Visual.VisualParent"/> has changed we have to search
        /// this element's ancestors to find another ancestor of the required <see cref="AttachedType"/>. 
        /// </remarks>
        /// <param name="oldParent">The previous parent of this elements.</param>
        protected override void OnVisualParentChanged( DependencyObject oldParent )
        {
            base.OnVisualParentChanged(oldParent);

            UpdateBinding(AttachedType);
        }

        #region --- Properties ---
        
        /// <summary>
        /// Gets or sets the object type that this <see cref="ActiveIcon"/> should be attached. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// By providing an attached type this element can be made to attach to a specific type of ancestor.
        /// It is similar in concept to the <see cref="System.Windows.Data.RelativeSource.AncestorType"/>
        /// property that is used when searching for an ancestor to bind. Once an ancestor has been found the 
        /// <see cref="AttachedElement"/> will be updated to reflect the ancestor to which we are attached.
        /// </para>
        /// </remarks>
        public Type AttachedType
        {
            get { return (Type) base.GetValue(AttachedTypeProperty); }
            set { base.SetValue(AttachedTypeProperty, value); }
        }   

        /// <summary>
        /// Gets the <see cref="System.Windows.FrameworkElement"/> to which this <see cref="ActiveIcon"/> is attached.
        /// </summary>
        public FrameworkElement AttachedElement
        {
            get { return (FrameworkElement) base.GetValue(AttachedElementProperty); }
            private set { base.SetValue(AttachedElementKey, value); }
        }   
        
        /// <summary>
        /// Gets or sets whether the icon should is active. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// This object should not be explicity set as it will be automatically bound to the
        /// <see cref="System.Windows.UIElement.IsMouseOver"/> property of the bound ancestor
        /// element.
        /// </remarks>
        public bool? IsActive
        {
            get { return (bool?) GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Media.ImageSource"/> for the <see cref="ActiveIcon"/>. This is a dependency property. 
        /// </summary>
        public BitmapSource Source
        {
            get { return (BitmapSource) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        
        #endregion
    }

}

