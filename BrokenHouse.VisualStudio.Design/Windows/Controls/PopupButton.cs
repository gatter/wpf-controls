using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Interop;

namespace BrokenHouse.VisualStudio.Design.Windows.Controls
{
    public class PopupButton : Button
    {   
        /// <summary>
        /// The IsOpen dependacy property
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty;
                   
        /// <summary>
        /// This is the context menu that provides the dropdown. 
        /// </summary>
        private ContextMenu PopupMenu { get; set; }

        #region --- Constructors ---

        /// <summary>
        /// Register the WPF Properties
        /// </summary>
        static PopupButton()
        {
            // Our only property
            IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(PopupButton), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsOpenChangedThunk)));

            // Register the default style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupButton), new FrameworkPropertyMetadata(typeof(PopupButton)));
        }    
        
        /// <summary>
        /// Our chance to create the instance context menu
        /// </summary>
        public PopupButton()
        {
            // Define the popup menu
            PopupMenu = new ContextMenu();

            // We want to know when it closes
            PopupMenu.Closed += new RoutedEventHandler(OnPopupMenuClosed);
            PopupMenu.MouseMove += new MouseEventHandler(OnPopupMenuMouseMove);
        }

        #endregion

        #region --- Event Handlers ---

        /// <summary>
        /// The mouse has been pushed down. If we are in the toggle part then we 
        /// open the context menu.
        /// </summary>
        /// <param name="e"></param>
        protected override void  OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            FrameworkElement originalElement = e.OriginalSource as FrameworkElement;

            if (originalElement.FindVisualAncestor("PART_Toggle") != null)
            {
                IsOpen = !IsOpen;
            }
            else
            {
                base.OnMouseLeftButtonDown(e);
            }
        }
        
        /// <summary>
        /// Popup Menu mouse move - we use this to determine if we have hovered over another toolbar button.
        /// If so we close the popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPopupMenuMouseMove( object sender, MouseEventArgs e )
        {
            HitTestResult hit = HitTester.HitTestFromCurrentPos();

            // Do we have a valid hit
            if (hit != null)
            {
                ToolBar       toolbar = (hit.VisualHit as FrameworkElement).FindVisualAncestor<ToolBar>();
                PopupButton   popup   = (hit.VisualHit as FrameworkElement).FindVisualAncestor<PopupButton>();

                if ((toolbar != null) && (popup != this))
                {
                    IsOpen = false;
                }
            }
        }

        /// <summary>
        /// The popup menu has closed we need to update the is open flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPopupMenuClosed( object sender, RoutedEventArgs e )
        {
            Action action = delegate { IsOpen = false; } ;

            Dispatcher.Invoke(DispatcherPriority.Background, action); 
        }

        /// <summary>
        /// This thunk is a simple wrapper to allow us to call the instance event handler
        /// </summary>
        /// <param name="element"></param>
        /// <param name="args"></param>
        private static void OnIsOpenChangedThunk( DependencyObject element, DependencyPropertyChangedEventArgs args )
        {
            (element as PopupButton).OnIsOpenChanged((bool)args.OldValue, (bool)args.NewValue);
        }

        /// <summary>
        /// The is open property has changed . this is our chance to pop up the context menu
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private void OnIsOpenChanged( bool oldValue, bool newValue )
        {
            if (newValue)
            {
                // Force the focus - this is to ensure that the command bindings work.
                this.Focus();

                // Define where we want the context menu - this has to be done in this order
                PopupMenu.PlacementTarget    = this;
                PopupMenu.Placement          = PlacementMode.Bottom;
                PopupMenu.PlacementRectangle = new Rect(0, 0, ActualWidth, ActualHeight);

                // Open it
                PopupMenu.Focusable          = true;
                PopupMenu.IsOpen             = true;

                // Invoke the focus late
                Action action = delegate 
                { 
                    if (PopupMenu.IsOpen)
                    {
                        PopupMenu.Focus(); 
                        CommandManager.InvalidateRequerySuggested(); 
                    }
                };

                Dispatcher.Invoke(DispatcherPriority.Background, action);
            }
            else
            {
                // Close the menu
                PopupMenu.IsOpen = false; 
            }
        }


        #endregion

        #region --- Public Properties ---

        /// <summary>
        /// Provide access to the items that make up popup menu. Items can be added to
        /// the WPF XML by adding menu items to the PopupItems property of PopupButton
        /// </summary>
        public ItemCollection PopupItems
        {
            get { return PopupMenu.Items; }
        }

        /// <summary>
        /// This flag will be true when the context menu is open
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }


        #endregion
 
    }
}

