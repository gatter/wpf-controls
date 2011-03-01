using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media.Animation;
using BrokenHouse.Extensions;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Parts.Task
{
    /// <summary>
    /// The <c>TaskDialogControl</c> is used to create complex task dialogs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>TaskDialogControl</c> forms the basis of this WPF implementation of the 
    /// standard Vista Task Dialog. Task dialogs can be created in two ways, 
    /// by using the <see cref="TaskDialog"/> helper class to create basic 
    /// task dialogs or by using the <see cref="TaskDialogControl"/> as the content 
    /// of a window to create complex task dialogs with custom content.
    /// </para>
    /// <para>
    /// When using the <c>TaskDialogControl</c> in a standard window it is important to
    /// set the <see cref="System.Windows.Window.SizeToContent"/> property to 
    /// <see cref="System.Windows.SizeToContent.Height"/> to ensure that the content of
    /// the window is rendered to the correct size. However, when used in this way it
    /// is important to make the window non-resizable.
    /// </para>
    /// <para>
    /// If there is a requirement for the <c>TaskDialogControl</c> to be resizable then you need
    /// to embed the control in a <see cref="TaskDialogWindow"/>. This is an extension to the 
    /// standard <see cref="System.Windows.Window"/> that will automatically resize (and set the 
    /// <c>MinHeight</c>) to the most appropriate height based on the content of the control.
    /// This also means that by using <see cref="TaskDialogWindow"/> the height of the window will
    /// change in response to the height expanding and contracting as the width of the dialog 
    /// is modified. 
    /// </para>
    /// <para>
    /// If an element in the content of the <c>TaskDialogControl</c> has the <c>Expand</c>
    /// attached property set then that control will expand and contract with the 
    /// <c>TaskDialogControl</c>. The expanding and contracting is performed by changing
    /// the <see cref="System.Windows.FrameworkElement.LayoutTransform"/> property. 
    /// </para>
    /// </remarks>
    [DefaultProperty("Content")]
    [ContentProperty("Content")]
    public class TaskDialogControl : ContentControl
    {
        /// <summary>
        /// Identifies the <see cref="Message"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    MessageProperty;
        /// <summary>
        /// Identifies the <see cref="Instruction"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    InstructionProperty;
        /// <summary>
        /// Identifies the Expand attached depedency property, see <see cref="SetExpand"/> and <see cref="GetExpand"/>.
        /// </summary>
        public static readonly DependencyProperty    ExpandProperty;
        /// <summary>
        /// Identifies the InvertExpansion attached depedency property, see <see cref="SetInvertExpand"/> and <see cref="GetInvertExpand"/>.
        /// </summary>
        public static readonly DependencyProperty    InvertExpandProperty;
        /// <summary>
        /// Identifies the <see cref="AllowExpand"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    AllowExpandProperty;
        /// <summary>
        /// Identifies the <see cref="IsExpanded"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    IsExpandedProperty;
        /// <summary>
        /// Identifies the <see cref="ExpandedText"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    ExpandedTextProperty;
        /// <summary>
        /// Identifies the <see cref="CollapsedText"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    CollapsedTextProperty;
        /// <summary>
        /// Identifies the <see cref="CheckBoxContent"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    CheckBoxContentProperty;
        /// <summary>
        /// Identifies the <see cref="HasCheckBoxContent"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    HasCheckBoxContentProperty;
        /// <summary>
        /// Identifies the <see cref="CheckBoxState"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    CheckBoxStateProperty;
        /// <summary>
        /// Identifies the <see cref="HasButtons"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    HasButtonsProperty;
        /// <summary>
        /// Identifies the <see cref="HasMessage"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    HasMessageProperty;
        /// <summary>
        /// Identifies the <see cref="HasFooterIcon"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    HasFooterIconProperty;
        /// <summary>
        /// Identifies the <see cref="HasFooterContent"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    HasFooterContentProperty;
        /// <summary>
        /// Identifies the <see cref="HasMainIcon"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    HasMainIconProperty;
        /// <summary>
        /// Identifies the <see cref="Buttons"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    ButtonsProperty;
        /// <summary>
        /// Identifies the <see cref="AutoExpandMessage"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    AutoExpandMessageProperty;
        /// <summary>
        /// Identifies the <see cref="ShowMessageInFooter"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    ShowMessageInFooterProperty;
        /// <summary>
        /// Identifies the <see cref="FooterIcon"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    FooterIconProperty;
        /// <summary>
        /// Identifies the <see cref="FooterContent"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    FooterContentProperty;
        /// <summary>
        /// Identifies the <see cref="MainIcon"/> depedency property.
        /// </summary>
        public static readonly DependencyProperty    MainIconProperty;
        /// <summary>
        /// Identifies the <see cref="HasCheckBoxContent"/> depedency property key.
        /// </summary>
        private static readonly DependencyPropertyKey HasCheckBoxContentKey;
        /// <summary>
        /// Identifies the <see cref="HasButtons"/> depedency property key.
        /// </summary>
        private static readonly DependencyPropertyKey HasButtonsKey;
        /// <summary>
        /// Identifies the <see cref="HasMessage"/> depedency property key.
        /// </summary>
        private static readonly DependencyPropertyKey HasMessageKey;
        /// <summary>
        /// Identifies the <see cref="HasFooterIcon"/> depedency property key.
        /// </summary>
        private static readonly DependencyPropertyKey HasFooterContentKey;
        /// <summary>
        /// Identifies the <see cref="HasFooterIcon"/> depedency property key.
        /// </summary>
        private static readonly DependencyPropertyKey HasFooterIconKey;
        /// <summary>
        /// Identifies the <see cref="HasMainIcon"/> depedency property key.
        /// </summary>
        private static readonly DependencyPropertyKey HasMainIconKey;

        private bool m_AnimateUpdate = false;

        private Storyboard m_AnimationStoryboard = null;


        /// <summary>
        /// Static constructor to do the WPF stuff
        /// </summary>
        static TaskDialogControl()
        {
            // Define the read only keys
            HasButtonsKey               = DependencyProperty.RegisterReadOnly("HasButtons", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false));
            HasMessageKey               = DependencyProperty.RegisterReadOnly("HasMessage", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false));
            HasFooterContentKey         = DependencyProperty.RegisterReadOnly("HasFooterContent", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false));
            HasFooterIconKey            = DependencyProperty.RegisterReadOnly("HasFooterIcon", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false));
            HasMainIconKey              = DependencyProperty.RegisterReadOnly("HasMainIcon", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false));
            HasCheckBoxContentKey       = DependencyProperty.RegisterReadOnly("HasCheckBoxContent", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false));
            
            // Define the properties and the read only properties
            InstructionProperty         = DependencyProperty.Register("Instruction", typeof(object), typeof(TaskDialogControl), new FrameworkPropertyMetadata("Instruction", null), null);
            MessageProperty             = DependencyProperty.Register("Message", typeof(object), typeof(TaskDialogControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMessageChangedThunk)));
            ButtonsProperty             = DependencyProperty.Register("Buttons", typeof(object), typeof(TaskDialogControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnButtonsChangedThunk)));
            ShowMessageInFooterProperty = DependencyProperty.Register("ShowMessageInFooter", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnShowMessageInFooterChangedThunk)));
            AutoExpandMessageProperty   = DependencyProperty.Register("AutoExpandMessage", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAutoExpandMessageChangedThunk)));
            FooterIconProperty          = DependencyProperty.Register("FooterIcon", typeof(object), typeof(TaskDialogControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFooterIconChangedThunk)));
            FooterContentProperty       = DependencyProperty.Register("FooterContent", typeof(object), typeof(TaskDialogControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFooterContentChangedThunk)));
            MainIconProperty            = DependencyProperty.Register("MainIcon", typeof(object), typeof(TaskDialogControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMainIconChangedThunk)));
            CheckBoxStateProperty       = DependencyProperty.Register("CheckBoxState", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false, null), null);
            CheckBoxContentProperty     = DependencyProperty.Register("CheckBoxContent", typeof(object), typeof(TaskDialogControl), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnCheckBoxContentChangedThunk)), null);
            ExpandedTextProperty        = DependencyProperty.Register("ExpandedText", typeof(string), typeof(TaskDialogControl), new FrameworkPropertyMetadata("Collapse", null), null);
            CollapsedTextProperty       = DependencyProperty.Register("CollapsedText", typeof(string), typeof(TaskDialogControl), new FrameworkPropertyMetadata("Expand", null), null);
            IsExpandedProperty          = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandedChangedThunk)));
            AllowExpandProperty         = DependencyProperty.Register("AllowExpand", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(true, null), null);
            HasButtonsProperty          = HasButtonsKey.DependencyProperty;
            HasMainIconProperty         = HasMainIconKey.DependencyProperty;
            HasMessageProperty          = HasMessageKey.DependencyProperty;
            HasFooterContentProperty    = HasFooterContentKey.DependencyProperty;
            HasFooterIconProperty       = HasFooterIconKey.DependencyProperty;
            HasCheckBoxContentProperty  = HasCheckBoxContentKey.DependencyProperty;
      
            // Attached properties
            ExpandProperty            = DependencyProperty.RegisterAttached("Expand", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnExpandAttachedChangedThunk)));
            InvertExpandProperty      = DependencyProperty.RegisterAttached("InvertExpand", typeof(bool), typeof(TaskDialogControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnInvertExpandAttachedChangedThunk)));
            
            // Define the default style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskDialogControl), new FrameworkPropertyMetadata(TaskElements.DialogControlStyleKey));
        
            // Override the keyboard navigation
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TaskDialogControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TaskDialogControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(TaskDialogControl), new FrameworkPropertyMetadata(false));
        }


        #region --- Public Properties ---

        /// <summary>
        /// Gets or sets the message associated with this task control.
        /// </summary>
        /// <remarks>
        /// The message appears below the instruction text or at the very bottom of 
        /// the dialog depending on the value of the <see cref="ShowMessageInFooter"/> 
        /// dependency property.
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public object Message
        {
            get { return GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the instruction associated with this task control. 
        /// </summary>
        /// <remarks>
        /// The instruction appears in bold or lager font (depending on the theme)
        /// at the top of the control.
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public object Instruction
        {
            get { return (string)GetValue(InstructionProperty); }
            set { SetValue(InstructionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text that appears in the expander control
        /// when the control is expanded.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public string ExpandedText
        {
            get { return (string)GetValue(ExpandedTextProperty); }
            set { SetValue(ExpandedTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text that appears in the expander when the control is collapsed.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public string CollapsedText
        {
            get { return (string)GetValue(CollapsedTextProperty); }
            set { SetValue(CollapsedTextProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the check box content. The check box appears in the top right of the footer
        /// section of the control.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public object CheckBoxContent
        {
            get { return GetValue(CheckBoxContentProperty); }
            set { SetValue(CheckBoxContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the state of the check box.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public bool CheckBoxState
        {
            get { return (bool)GetValue(CheckBoxStateProperty); }
            set { SetValue(CheckBoxStateProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the control is showing the expanded content or hiding it.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value);  }
        }

        /// <summary>
        /// Gets ot sets whether the control is allowed to expand its content.
        /// </summary>
        /// <remarks>
        /// If this property is set to <c>False</c> then the expander control will not be visible.
        /// </remarks>
        [Category("Appearance")]
        [Bindable(true)]
        public bool AllowExpand
        {
            get { return (bool)GetValue(AllowExpandProperty); }
            set { SetValue(AllowExpandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content of the footer used at the bottom of the control. 
        /// </summary>
        /// <remarks>
        /// The <c>FooterContent</c> will appear in a horizontal bar along with
        /// the <see cref="FooterIcon"/> at the bottom of the control.
        /// If <see cref="ShowMessageInFooter"/> property is set to <c>True</c> then
        /// the <see cref="Message"/>  will bellow the this content.
        /// </remarks>
        [Category("Content")]
        [Bindable(true)]
        public object FooterContent
        {
            get { return GetValue(FooterContentProperty); }
            set { SetValue(FooterContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the icon to used with the <c>FooterContent</c> 
        /// at the bottom of the control. 
        /// </summary>
        /// <remarks>
        /// The <c>FooterIcon</c> and <see cref="FooterContent"/> will appear in a 
        /// horizontal bar at the bottom of the control.
        /// If <see cref="ShowMessageInFooter"/> property is set to <c>True</c> then
        /// the <see cref="Message"/> will bellow the this content.
        /// </remarks>
        [Category("Content")]
        [Bindable(true)]
        public object FooterIcon
        {
            get { return GetValue(FooterIconProperty); }
            set { SetValue(FooterIconProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the message is shown in the footer. 
        /// </summary>
        /// <remarks>
        /// If this property is set to <c>True</c> then the <see cref="Message"/>
        /// will appear below the <see cref="FooterContent"/> and
        /// <see cref="FooterIcon"/> (if defined).
        /// </remarks>
        [Category("Content")]
        [Bindable(true)]
        public bool ShowMessageInFooter
        {
            get { return (bool)GetValue(ShowMessageInFooterProperty); }
            set { SetValue(ShowMessageInFooterProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the message is should expand when the expand button is pressed.
        /// </summary>
        [Category("Content")]
        [Bindable(true)]
        public bool AutoExpandMessage
        {
            get { return (bool)GetValue(AutoExpandMessageProperty); }
            set { SetValue(AutoExpandMessageProperty, value); }
        }    
    
        /// <summary>
        /// Gets or sets the content that will be used for the main icon in the control. 
        /// </summary>
        /// <remarks>
        /// The main icon appears to the left of the <see cref="Instruction"/> and 
        /// <see cref="Message"/> text.
        /// </remarks>
        [Category("Content")]
        [Bindable(true)]
        public object MainIcon
        {
            get { return GetValue(MainIconProperty); }
            set { SetValue(MainIconProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content that should be used for the buttons in the control.
        /// </summary>
        /// <remarks>
        /// Normally the <see cref="TaskButtonBar"/> is used to provide the button
        /// for this section of the task dialog.
        /// </remarks>
        [Category("Content")]
        [Bindable(true)]
        public object Buttons
        {
            get { return GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        /// <summary>
        /// Gets whether this control has any <see cref="Message"/>
        /// </summary>
        [Bindable(false)]
        public bool HasMessage
        {
            get { return (bool)GetValue(HasMessageProperty); }
            private set { SetValue(HasMessageKey, value); }
        }
        
        /// <summary>
        /// Gets whether this control has any  <see cref="CheckBoxContent"/>.
        /// </summary>
        [Bindable(false)]
        public bool HasCheckBoxContent
        {
            get { return (bool)GetValue(HasCheckBoxContentProperty); }
            private set { SetValue(HasCheckBoxContentKey, value); }
        }  

        /// <summary>
        /// Gets whether this control has any  <see cref="FooterContent"/>.
        /// </summary>
        [Bindable(false)]
        public bool HasFooterContent
        {
            get { return (bool)GetValue(HasFooterContentProperty); }
            private set { SetValue(HasFooterContentKey, value); }
        }  

        /// <summary>
        /// Gets whether this control has any <see cref="FooterIcon"/>.
        /// </summary>
        [Bindable(false)]
        public bool HasFooterIcon
        {
            get { return (bool)GetValue(HasFooterIconProperty); }
            private set { SetValue(HasFooterIconKey, value); }
        }  
     

        /// <summary>
        /// Gets whether content has a <see cref="MainIcon"/>.
        /// </summary>
        [Bindable(true)]
        public bool HasMainIcon
        {
            get { return (bool)GetValue(HasMainIconProperty); }
            private set { SetValue(HasMainIconKey, value); }
        }

        /// <summary>
        /// Gets whether the <see cref="Buttons"/> content has been set.
        /// </summary>
        [Bindable(false)]
        public bool HasButtons
        {
            get { return (bool)GetValue(HasButtonsProperty); }
            private set { SetValue(HasButtonsKey, value); }
        }       
 

        #endregion

        #region --- Attached Properties ---

        /// <summary>
        /// Gets the whether the supplied element should be expanded or contracted 
        /// with expand/collapse control.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]
        public static bool GetExpand( UIElement element )
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(ExpandProperty);
        }

        /// <summary>
        /// Sets whether the supplied element should be expanded or contracted 
        /// when the control expands and collapses.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetExpand( UIElement element, bool value )
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(ExpandProperty, value);
        }
        
        /// <summary>
        /// Gets the whether the supplied element should be expanded when
        /// all the other controls are collapsing. The <c>Expand</c>
        /// attached property must be set to <c>true</c> for this 
        /// property to have any effect.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]
        public static bool GetInvertExpand( UIElement element )
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(InvertExpandProperty);
        }

        /// <summary>
        /// Sets whether the supplied element should be expanded when
        /// all the other controls are collapsing. The <c>Expand</c>
        /// attached property must be set to <c>true</c> for this 
        /// property to have any effect.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetInvertExpand( UIElement element, bool value )
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(InvertExpandProperty, value);
        }

        #endregion

        #region --- Protected Event Handlers ---
        
        /// <summary>
        /// The expanded flag has changed
        /// </summary>
        /// <param name="oldFlag"></param>
        /// <param name="newFlag"></param>
        protected virtual void OnIsExpandedChanged( bool oldFlag, bool newFlag )
        {
            UpdateControls();
        }
 

        /// <summary>
        /// The main icon content has changed to remove and add the logical children
        /// </summary>
        /// <param name="oldIcon"></param>
        /// <param name="newIcon"></param>
        protected virtual void OnMainIconChanged( object oldIcon, object newIcon )
        {
            RemoveLogicalChild(oldIcon);
            AddLogicalChild(newIcon);
        }

        /// <summary>
        /// The button content has changed to remove and add the logical children
        /// </summary>
        /// <param name="oldButtons"></param>
        /// <param name="newButtons"></param>
        protected virtual void OnButtonsChanged(object oldButtons, object newButtons)
        {
            RemoveLogicalChild(oldButtons);
            AddLogicalChild(newButtons);
        }

        /// <summary>
        /// The checkbox content has changed to remove and add the logical children
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected virtual void OnCheckBoxContentChanged( object oldContent, object newContent )
        {
            RemoveLogicalChild(oldContent);
            AddLogicalChild(newContent);
        }

        /// <summary>
        /// The footer content has changed to remove and add the logical children
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected virtual void OnFooterContentChanged( object oldContent, object newContent )
        {
            RemoveLogicalChild(oldContent);
            AddLogicalChild(newContent);
        }

        /// <summary>
        /// The footer content has changed to remove and add the logical children
        /// </summary>
        /// <param name="oldHeader"></param>
        /// <param name="newHeader"></param>
        protected virtual void OnFooterHeaderChanged( object oldHeader, object newHeader )
        {
            RemoveLogicalChild(oldHeader);
            AddLogicalChild(newHeader);
        }

        /// <summary>
        /// The footer content has changed to remove and add the logical children
        /// </summary>
        /// <param name="oldIcon"></param>
        /// <param name="newIcon"></param>
        protected virtual void OnFooterIconChanged( object oldIcon, object newIcon )
        {
            RemoveLogicalChild(oldIcon);
            AddLogicalChild(newIcon);
        }

        /// <summary>
        /// The message has changed to remove and add the logical children
        /// </summary>
        /// <param name="oldMessage"></param>
        /// <param name="newMessage"></param>
        protected virtual void OnMessageChanged( object oldMessage, object newMessage )
        {
            RemoveLogicalChild(oldMessage);
            AddLogicalChild(newMessage);
        }
 

        #endregion

        #region --- Dependency property Event Handlers ---
          
        /// <summary>
        /// The auto expand message property has changed we need to update the controls
        /// </summary>
        /// <param name="target">The instance of a control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnAutoExpandMessageChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            TaskDialogControl dialogControl = target as TaskDialogControl;

            // Did we find the ancestor
            if (dialogControl != null)
            {
                dialogControl.UpdateControls();
            }
        }

        /// <summary>
        /// The ShowMessageInFooter property has changed we need to update the controls
        /// </summary>
        /// <param name="target">The instance of a control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnShowMessageInFooterChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            TaskDialogControl dialogControl = target as TaskDialogControl;

            // Did we find the ancestor
            if (dialogControl != null)
            {
                dialogControl.UpdateControls();
            }
        }
               
        /// <summary>
        /// The <see cef="InvertExpand"/> property defines whether the child should invert its expand action. For
        /// example, if you want an element to appear when collapsing then set this attached property to <c>true</c>.
        /// </summary>
        /// <param name="target">The instance of a child control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnInvertExpandAttachedChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            FrameworkElement  targetElement   = target as FrameworkElement;
            TaskDialogControl ancestorElement = targetElement.FindVisualAncestor<TaskDialogControl>();

            // Did we find the ancestor
            if (ancestorElement != null)
            {
                ancestorElement.UpdateControls();
            }
        }

        /// <summary>
        /// The <see cef="Expand"/> property defines whether the child should be exapnded or collapsed based
        /// on the the expanded check.
        /// </summary>
        /// <param name="target">The instance of a child control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnExpandAttachedChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            FrameworkElement  targetElement   = target as FrameworkElement;
            TaskDialogControl ancestorElement = targetElement.FindVisualAncestor<TaskDialogControl>();

            // Did we find the ancestor
            if (ancestorElement != null)
            {
                ancestorElement.UpdateControls();
            }

            // We need to know when the target element changes
            if ((bool)args.NewValue)
            {
                targetElement.IsVisibleChanged += OnExpandAttachedIsVisibleChanged;
            }
            else
            {
                targetElement.IsVisibleChanged -= OnExpandAttachedIsVisibleChanged;
            }
        }

                      
        /// <summary>
        /// The layout has changed and we need to update the controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnExpandAttachedIsVisibleChanged( object sender, DependencyPropertyChangedEventArgs e )
        {
            FrameworkElement  targetElement   = sender as FrameworkElement;
            TaskDialogControl ancestorElement = targetElement.FindVisualAncestor<TaskDialogControl>();

            // Did we find the ancestor
            if (ancestorElement != null)
            {
                ancestorElement.UpdateControls();
            }
        }

        /// <summary>
        /// The IsExpanded dependency property has changed. We need to expand of collapse the children of the control.
        /// </summary>
        /// <param name="target">The instance of the task control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnIsExpandedChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            TaskDialogControl control    = target as TaskDialogControl;

            // Trigger the event
            control.OnIsExpandedChanged((bool)args.OldValue, (bool)args.NewValue);
        }
        
        /// <summary>
        /// The Icon dependency property has changed. 
        /// </summary>
        /// <param name="target">The instance of the task control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnMainIconChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            TaskDialogControl control = target as TaskDialogControl;

            control.HasMainIcon = (args.NewValue != null);
            control.OnMainIconChanged(args.OldValue, args.NewValue);
        }

        /// <summary>
        /// The dependency property has changed
        /// </summary>
        /// <param name="target">The instance of the task control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnButtonsChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            TaskDialogControl control = target as TaskDialogControl;

            control.HasButtons = (args.NewValue != null);
            control.OnButtonsChanged(args.OldValue, args.NewValue);
        }

        /// <summary>
        /// The dependency property has changed
        /// </summary>
        /// <param name="target">The instance of the task control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnFooterContentChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            TaskDialogControl control = target as TaskDialogControl;

            control.HasFooterContent = (args.NewValue != null);
            control.OnFooterContentChanged(args.OldValue, args.NewValue);
        }

        /// <summary>
        /// The dependency property has changed
        /// </summary>
        /// <param name="target">The instance of the task control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnCheckBoxContentChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            TaskDialogControl control = target as TaskDialogControl;

            control.HasCheckBoxContent = (args.NewValue != null);
            control.OnCheckBoxContentChanged(args.OldValue, args.NewValue);
        }

        /// <summary>
        /// The dependency property has changed
        /// </summary>
        /// <param name="target">The instance of the task control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnFooterIconChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            TaskDialogControl control = target as TaskDialogControl;

            control.HasFooterIcon = (args.NewValue != null);
            control.OnFooterIconChanged(args.OldValue, args.NewValue);
        }

        /// <summary>
        /// The dependency property has changed
        /// </summary>
        /// <param name="target">The instance of the task control that has changed.</param>
        /// <param name="args">The information on how the property has changed</param>
        private static void OnMessageChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            TaskDialogControl control = target as TaskDialogControl;

            control.HasMessage = (args.NewValue != null);
            control.OnMessageChanged(args.OldValue, args.NewValue);
        }
  
        #endregion
          
        #region --- Overrides ---

        /// <summary>
        /// Overridden to ensure that animations are only used after we have rendered the control.
        /// </summary>
        /// <param name="drawingContext">An instance of <see cref="System.Windows.Media.DrawingContext"/> used to render the control.</param>
        protected override void  OnRender(DrawingContext drawingContext)
        {
 	        base.OnRender(drawingContext);

            m_AnimateUpdate = true;
        }

        /// <summary>
        /// This is called we <see cref="System.Windows.FrameworkElement.ApplyTemplate"/> is called.
        /// </summary>
        /// <remarks>
        /// This is our chance to initialise the control by either expanding or contracting the message
        /// section of the control.
        /// </remarks>
        public override void OnApplyTemplate()
        {
            // Call the default
            base.OnApplyTemplate();
        
            // Modify the control
            UpdateControls();
        }
  
        #endregion

        #region --- Private Helpers ---

        /// <summary>
        /// Animate the epansion or contraction of the element
        /// </summary>
        /// <param name="element">The element that is expanding or collapsing.</param>
        /// <param name="isExpanding">When <b>true</b> the animation will cause the element to expand.</param>
        /// <param name="offsetAnimation">Will cause the animation to be offset (used when we are expanding and collapsing at the same time)</param>
        private IEnumerable<Timeline> CreateStoryboardAnimations( FrameworkElement element, bool isExpanding, bool offsetAnimation )
        {  
            DoubleAnimationUsingKeyFrames   opacityAnimation   = new DoubleAnimationUsingKeyFrames() { SpeedRatio = 2.0 };
            DoubleAnimationUsingKeyFrames   scaleAnimation     = new DoubleAnimationUsingKeyFrames() { SpeedRatio = 2.0 };
            ScaleTransform                  scaleTransform     = new ScaleTransform(1.0, isExpanding? 0.0 : 1.0); 
            double                          timeSpanOffset     = offsetAnimation? 0.5 : 0.0;

            if (isExpanding)
            {
                scaleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeSpanOffset))));
                scaleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeSpanOffset + 0.5))));
                opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeSpanOffset + 0.5))));
                opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeSpanOffset + 1.0))));
            }
            else
            {
                opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeSpanOffset))));
                opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeSpanOffset + 0.5))));
                scaleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeSpanOffset + 0.5))));
                scaleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(timeSpanOffset + 1.0))));
            }

            // Save the transform
            element.LayoutTransform = scaleTransform;

            // Set up the animation targets
            Storyboard.SetTarget(opacityAnimation, element);
            Storyboard.SetTarget(scaleAnimation, element);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
            Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("LayoutTransform.ScaleY"));

            // Start the animations
            yield return opacityAnimation;
            yield return scaleAnimation;
       }

        /// <summary>
        /// We need to change the layout of the control by contracting and expanding certain sections
        /// </summary>
        private void UpdateControls()
        {
            if (m_AnimationStoryboard == null)
            {
                Window                 parentWindow        = this.FindVisualAncestor<Window>();
                bool                   isExpanding         = IsExpanded;
                List<FrameworkElement> expandItems         = new List<FrameworkElement>();
                List<FrameworkElement> collapseItems       = new List<FrameworkElement>();
                List<FrameworkElement> changingItems       = isExpanding? expandItems : collapseItems;
                List<FrameworkElement> invertedItems       = isExpanding? collapseItems : expandItems;
                FrameworkElement       contentMessagePanel = GetTemplateChild("PART_ContentMessagePanel") as FrameworkElement;
                FrameworkElement       footerMessagePanel  = GetTemplateChild("PART_FooterMessagePanel") as FrameworkElement;
                FrameworkElement       visibleMessagePanel = ShowMessageInFooter? footerMessagePanel : contentMessagePanel;
                FrameworkElement       hiddenMessagePanel  = ShowMessageInFooter? contentMessagePanel : footerMessagePanel;
                FrameworkElement       contentPresenter    = GetTemplateChild("PART_ContentPanel") as FrameworkElement;

                // First the content of the dialog
                if (contentPresenter != null)
                {
                    var sourceItems = contentPresenter.EnumerateVisualDescendants().OfType<FrameworkElement>().Where(e => GetExpand(e)).ToList();

                    // The default action is where the invert expand is not set
                    changingItems.AddRange(sourceItems.Where(e => !GetInvertExpand(e)));

                    // All the others will be put in the other list where the opposite will occur.
                    invertedItems.AddRange(sourceItems.Where(e => GetInvertExpand(e)));
                }

                // Second the message component
                if (visibleMessagePanel != null)
                {
                    var visibleItems = visibleMessagePanel.EnumerateVisualDescendants(0).OfType<FrameworkElement>();

                    // Make sure the appropriate panel is visible
                    expandItems.Add(visibleMessagePanel);
                    collapseItems.Add(hiddenMessagePanel);

                    // Add the items to the correct list on whether we auto expand the message panel or not
                    if (AutoExpandMessage)
                    {
                        changingItems.AddRange(visibleItems);
                    }
                    else
                    {
                        expandItems.AddRange(visibleItems);
                    }
                }

                // Filter the lists
                expandItems   = expandItems.Where(i => i.Opacity == 0.0).ToList();
                collapseItems = collapseItems.Where(i => i.Opacity == 1.0).ToList();

                // Do we animate the children?
                if (!DesignerProperties.GetIsInDesignMode(this) && m_AnimateUpdate)
                {
                    TimelineCollection animations  = new TimelineCollection();
                    bool               pauseExpand = ((expandItems.Count > 0) && (collapseItems.Count > 0));

                    // Expand with animation
                    animations.AddRange(expandItems.SelectMany(i => CreateStoryboardAnimations(i, true, pauseExpand)));
                    animations.AddRange(collapseItems.SelectMany(i => CreateStoryboardAnimations(i, false, false)));

                    // Do we have any animations
                    if (animations.Count > 0)
                    {
                        m_AnimationStoryboard = new Storyboard();

                        // Set the animations
                        m_AnimationStoryboard.Children = animations;

                        // We want to know when the animation completed
                        m_AnimationStoryboard.Completed += OnAnimationStoryboardCompleted;

                        // Start them up
                        m_AnimationStoryboard.Begin(this);
                    }
                }
                else
                {
                    // Define the actions that will be performed
                    ScaleTransform           expandTransform   = new ScaleTransform(1.0, 1.0);
                    ScaleTransform           collapseTransform = new ScaleTransform(1.0, 0.0);
                    Action<FrameworkElement> expandAction      = delegate( FrameworkElement e ) { e.LayoutTransform = expandTransform; e.Opacity = 1.0; };
                    Action<FrameworkElement> collapseAction    = delegate( FrameworkElement e ) { e.LayoutTransform = collapseTransform; e.Opacity = 0.0; };

                    // Collapse and exapnd immediately
                    expandItems.ForEach(i => expandAction(i));
                    collapseItems.ForEach(i => collapseAction(i));
                }
            }
        }

        /// <summary>
        /// Triggered when the animation completes. Ensure we are consistant by updating the controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAnimationStoryboardCompleted(object sender, EventArgs e)
        {
            m_AnimationStoryboard.Completed -= OnAnimationStoryboardCompleted;
            m_AnimationStoryboard = null;

            UpdateControls();
        }

        #endregion
    }
}
