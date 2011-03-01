using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using BrokenHouse.Windows.Interop;

namespace BrokenHouse.Windows.Extensions
{
    /// <summary>
    /// This class provides attached properties that change the native style of a window.
    /// </summary>
    /// <remarks>
    /// Normally WPF does not allow the changing the extended styles of a window. This class
    /// provides the additional properties to take advantage of these styles.
    /// </remarks>
    /// <example>
    /// This example shows how to use the attached properties using XAML
    /// <code>
    /// <Window x:Class="DemoApplication.Test"
    ///         xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///         xmlns:bh="http://www.brokenhouse.co.uk/wpf"
    ///         Title="TestWizard" Icon="/DemoApplication;component/Print.ico"
    ///         Width="500" Height="500" bh:NativeWindowStyles.CanMaximize="False"/>   
    /// </code>
    /// </example>
    public class NativeWindowStyles
    {
        /// <summary>
        /// Identifies the <c>CanMaximize</c> depedency property.
        /// </summary>
        public static readonly DependencyProperty    CanMaximizeProperty;
        /// <summary>
        /// Identifies the <c>CanMinimize</c> depedency property.
        /// </summary>
        public static readonly DependencyProperty    CanMinimizeProperty;
        /// <summary>
        /// Identifies the <c>IsWindowCaptionVisible</c> depedency property.
        /// </summary>
        public static readonly DependencyProperty    IsWindowCaptionVisibleProperty;
        /// <summary>
        /// Identifies the <c>IsWindowIconVisible</c> depedency property.
        /// </summary>
        public static readonly DependencyProperty    IsWindowIconVisibleProperty;
        /// <summary>
        /// Identifies the <c>IsSystemMenuVisible</c> attached depedency property.
        /// </summary>
        public static readonly DependencyProperty    IsSystemMenuVisibleProperty;
        /// <summary>
        /// Identifies the <c>IsAttached</c> attached depedency property.
        /// </summary>
        private static readonly DependencyProperty   IsAttachedProperty;

        /// <summary>
        /// A list of windows that have not been initialised
        /// </summary>
        private static Dictionary<DependencyProperty, Action<Window, object>> PropertyActions { get; set; }
        
        /// <summary>
        /// Static constructor to initialise the WPF
        /// </summary>
        static NativeWindowStyles()
        {
            // Register the properties
            CanMaximizeProperty             = DependencyProperty.RegisterAttached("CanMaximize", typeof(bool), typeof(NativeWindowStyles), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAttachedPropertyChangedThunk))); 
            CanMinimizeProperty             = DependencyProperty.RegisterAttached("CanMinimize", typeof(bool), typeof(NativeWindowStyles), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAttachedPropertyChangedThunk)));
            IsWindowCaptionVisibleProperty  = DependencyProperty.RegisterAttached("IsWindowCaptionVisible", typeof(bool), typeof(NativeWindowStyles), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAttachedPropertyChangedThunk))); 
            IsWindowIconVisibleProperty     = DependencyProperty.RegisterAttached("IsWindowIconVisible", typeof(bool), typeof(NativeWindowStyles), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAttachedPropertyChangedThunk))); 
            IsSystemMenuVisibleProperty     = DependencyProperty.RegisterAttached("IsSystemMenuVisible", typeof(bool), typeof(NativeWindowStyles), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAttachedPropertyChangedThunk)));
            IsAttachedProperty              = DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(NativeWindowStyles), new FrameworkPropertyMetadata(false));
        
            // Make sure the action dictional is initialised
            PropertyActions = new Dictionary<DependencyProperty, Action<Window, object>>();

            // Define the actions
            PropertyActions[CanMinimizeProperty] = (w, v) => w.ShowMinimiseBox((bool)v);
            PropertyActions[CanMaximizeProperty] = (w, v) => w.ShowMaximiseBox((bool)v);
            PropertyActions[IsWindowCaptionVisibleProperty] = (w, v) => w.ShowWindowCaption((bool)v);
            PropertyActions[IsWindowIconVisibleProperty] = (w, v) => w.ShowWindowIcon((bool)v);
            PropertyActions[IsSystemMenuVisibleProperty] = (w, v) => w.ShowSystemMenu((bool)v);
        }

        #region --- Attached Properties ---

        /// <summary>
        /// Gets whether a <see cref="System.Windows.Window"/> will have its maximise button visible.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <returns><b>true</b> if the <paramref name="window"/> has the maximise button is visible.</returns>
        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetCanMaximize( Window window )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            return (bool)window.GetValue(CanMaximizeProperty);
        }

        /// <summary>
        /// Sets whether a <see cref="System.Windows.Window"/> has its maximise button visible.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="value">Set to <b>true</b> if the maximise button should be visible</param>
        public static void SetCanMaximize( Window window, bool value )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            window.SetValue(CanMaximizeProperty, value);
        }


        /// <summary>
        /// Gets whether a <see cref="System.Windows.Window"/> has its minimise button visible.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <returns><b>true</b> if the <paramref name="window"/> has its minimise button visible.</returns>
        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetCanMinimize( Window window )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            return (bool)window.GetValue(CanMinimizeProperty);
        }

        /// <summary>
        /// Sets whether a <see cref="System.Windows.Window"/> has its maximise button visible.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="value">Set to <b>true</b> if its maximume button should be visible.</param>
        public static void SetCanMinimize( Window window, bool value )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            window.SetValue(CanMinimizeProperty, value);
        }
        
        /// <summary>
        /// Gets whether a <see cref="System.Windows.Window"/> has its caption is rendered.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <returns><b>false</b> if the <paramref name="window"/>'s caption is hidden.</returns>
        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetIsWindowCaptionVisible( Window window )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            return (bool)window.GetValue(IsWindowCaptionVisibleProperty);
        }

        /// <summary>
        /// Sets whether a <see cref="System.Windows.Window"/>'s caption should be rendered.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="value">Set to <b>true</b> if the <paramref name="window"/>'s caption should be visible.</param>
        public static void SetIsWindowCaptionVisible( Window window, bool value )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            window.SetValue(IsWindowCaptionVisibleProperty, value);
        }
       
        /// <summary>
        /// Gets whether a <see cref="System.Windows.Window"/>'s icon is visible.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <returns><b>false</b> if the <paramref name="window"/>'s icon is hidden.</returns>
        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetIsWindowIconVisible( Window window )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            return (bool)window.GetValue(IsWindowIconVisibleProperty);
        }

        /// <summary>
        /// Sets whether a window's icon is visible.
        /// </summary>
        /// <param name="window">The target window</param>
        /// <param name="value">Set to <b>true</b> if the <paramref name="window"/>'s icon should be visible.</param>
        public static void SetIsWindowIconVisible( Window window, bool value )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            window.SetValue(IsWindowIconVisibleProperty, value);
        }
       
        /// <summary>
        /// Gets whether a window has a system menu
        /// </summary>
        /// <param name="window">The target window</param>
        /// <returns><b>false</b> if the <paramref name="window"/> does not have a system menu.</returns>
        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetIsSystemMenuVisible( Window window )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            return (bool)window.GetValue(IsSystemMenuVisibleProperty);
        }

        /// <summary>
        /// Sets whether a window has a system menu
        /// </summary>
        /// <remarks>
        /// Having a system menu forces the window to have an icon which when clicked with the right mouse
        /// button shows the windows menu. Hiding it is useful when you want a simple modal dialog.
        /// </remarks>
        /// <param name="window">The target window</param>
        /// <param name="value"><b>true</b> if you do not want a system menu (and default icon) to be visible</param>
        public static void SetIsSystemMenuVisible( Window window, bool value )
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            window.SetValue(IsSystemMenuVisibleProperty, value);
        }

        #endregion

        #region --- Property Changed Thunks ---

        /// <summary>
        /// An attached property has been changed on a window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void OnAttachedPropertyChangedThunk( object sender, DependencyPropertyChangedEventArgs args )
        {
            Action<Window, object> action = null;
            Window                 window = sender as Window;

            if (window != null)
            {
                // Make sure we do thing correctly
                WindowInteropHelper helper     = new WindowInteropHelper(window);
                bool                isAttached = (bool)window.GetValue(IsAttachedProperty);
        
                // If the window is valid call the action directly
                if (helper.Handle != IntPtr.Zero)
                {
                    if (PropertyActions.TryGetValue(args.Property, out action))
                    {
                        action(window, args.NewValue);
                    }
                }

                // Have we already been attached
                if (!isAttached)
                {
                    // We want to know when the source is initialised or updated
                    window.SourceInitialized += OnWindowSourceChanged;
                    window.SourceUpdated     += OnWindowSourceChanged;
                    
                    // Have we attached to a window
                    window.SetValue(IsAttachedProperty, true);
                }
            }
        }

        #endregion

        #region --- Helpers ---

        /// <summary>
        /// Our private event that is called when the window's source has been initialised or updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnWindowSourceChanged( object sender, EventArgs args )
        {
            Window window = sender as Window;

            // Loop over the action
            foreach (var entry in PropertyActions)
            {
                // Get the current and default values
                object currentValue = window.GetValue(entry.Key);
                object defaultValue = entry.Key.DefaultMetadata.DefaultValue;

                // If they are differnet
                if (!object.Equals(currentValue, defaultValue))
                {
                    // Call the action to update the property
                    entry.Value(window, currentValue);
                }
            }
        }

        #endregion
    }
}