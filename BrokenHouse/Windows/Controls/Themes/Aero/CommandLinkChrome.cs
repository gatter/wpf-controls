using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using BrokenHouse.Extensions;
using BrokenHouse.Windows.Extensions;

namespace BrokenHouse.Windows.Controls.Themes.Aero
{
    /// <summary>
    /// The style of the Aero Command Link button does not lend itself to be rendered using
    /// elements and storyboards within a style. So, in order to provide a button that
    /// is visually equivalent then we have to implement a <see cref="System.Windows.Controls.Decorator"/> to render the 
    /// Command Link's chrome in a similar way to the ButtonChrome in PresentationFramework.Aero assembly is used
    /// to render the the chrome for an Aero button.
    /// </summary>
    public class CommandLinkChrome : Decorator
    {        
        #region --- Dependency Properties ---

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty;
        
        /// <summary>
        /// Identifies the <see cref="Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty;
        
        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty;
        
        /// <summary>
        /// Identifies the <see cref="RenderDefaulted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderDefaultedProperty;
        
        /// <summary>
        /// Identifies the <see cref="RenderMouseOver"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderMouseOverProperty;
        
        /// <summary>
        /// Identifies the <see cref="RenderPressed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderPressedProperty;

        /// <summary>
        /// Identifies the <see cref="RenderEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderEnabledProperty;  

        #endregion

        LinearGradientBrush   m_TopDropShadow       = null;
        LinearGradientBrush   m_LeftDropShadow      = null;
        LinearGradientBrush   m_OverlayBackground   = null;
        Pen                   m_OverlayBorder       = null;
        Pen                   m_NormalBorder        = null;
        Pen                   m_InnerBorder         = null;
        static readonly Color c_PressedBackground   = Color.FromArgb(0x40, 0xe0, 0xe0, 0xe0);
        static readonly Color c_PressedForeground   = Color.FromArgb(0xff, 0x18, 0x1c, 0x52);
        static readonly Color c_PressedBorder       = Color.FromArgb(0xff, 0xa0, 0xa0, 0xa0);
        static readonly Color c_HoverForeground     = Color.FromArgb(0xff, 0x07, 0x4A, 0xE5);
        static readonly Color c_HoverBorder         = Color.FromArgb(0xff, 0xc8, 0xc8, 0xc8);
        static readonly Color c_DefaultForeground   = Color.FromArgb(0xff, 0x18, 0x1c, 0x52);
        static readonly Color c_DisabledForeground  = SystemColors.GrayTextColor;

        #region --- Constructors ---

        /// <summary>
        /// Register the WPF properties
        /// </summary>
        static CommandLinkChrome()
        {
            // Register the additional properties
            ForegroundProperty        = TextElement.ForegroundProperty.AddOwner(typeof(CommandLinkChrome), new FrameworkPropertyMetadata(new SolidColorBrush(c_DefaultForeground), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
            BackgroundProperty        = Control.BackgroundProperty.AddOwner(typeof(CommandLinkChrome), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            BorderBrushProperty       = Border.BorderBrushProperty.AddOwner(typeof(CommandLinkChrome), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            RenderDefaultedProperty   = DependencyProperty.Register("RenderDefaulted", typeof(bool), typeof(CommandLinkChrome), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnRenderDefaultedChangedThunk)));
            RenderMouseOverProperty   = DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(CommandLinkChrome), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnRenderMouseOverChangedThunk)));
            RenderPressedProperty     = DependencyProperty.Register("RenderPressed", typeof(bool), typeof(CommandLinkChrome), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnRenderPressedChangedThunk)));
            RenderEnabledProperty     = DependencyProperty.Register("RenderEnabled", typeof(bool), typeof(CommandLinkChrome), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnRenderEnabledChangedThunk)));
        }
       
        #endregion

        #region --- Positioning ---
       
        /// <summary>
        /// Computes the desired size required to render the chrome of the <see cref="CommandLink"/> around our
        /// child element.
        /// </summary>
        /// <remarks>
        /// The desired size of this <see cref="System.Windows.Controls.Decorator"/> is 
        /// is determined by analyzing its child element's sizing properties, margin, requested size and the thickness
        /// of the chrome itself.
        /// </remarks>
        /// <param name="constraintSize">The <see cref="System.Windows.Size"/> of the available area for the chrome and its child. </param>
        /// <returns>The desired <see cref="System.Windows.Size"/> based on the child element, the thickness of the
        /// chrome and the <paramref name="constraintSize"/>.</returns>
        protected override Size MeasureOverride( Size constraintSize )
        {
            UIElement child = this.Child;
            Size      desiredSize;

            // Do we have a child
            if (child != null)
            {
                double widthAdjust  = Math.Min(constraintSize.Width, 4.0);
                double heightAdjust = Math.Min(constraintSize.Height, 4.0);

                // Have to account for our internal border
                Size childSize = new Size(constraintSize.Width - widthAdjust, constraintSize.Height - heightAdjust);

                // Measure the child
                child.Measure(childSize);

                // What is the child's desired size
                desiredSize = child.DesiredSize;

                // Add the adjustmen
                desiredSize.Width += widthAdjust;
                desiredSize.Height += heightAdjust;
            }
            else
            {
                desiredSize = new Size(Math.Min(4.0, constraintSize.Width), Math.Min(4.0, constraintSize.Height));
            }

            // Return the best size
            return desiredSize;
        }

        /// <summary>
        /// Arranges the child so that it is vertically centred and aligned to the left.
        /// </summary>
        /// <param name="finalSize">The <see cref="System.Windows.Size"/> in which the child should be arranged.</param>
        /// <returns>The actual <see cref="System.Windows.Size"/> of the decorator and its child element.</returns>
        protected override Size ArrangeOverride( Size finalSize )
        {
            UIElement child = this.Child;

             // Do we have a child
            if (child != null)
            {
                double width  = Math.Max(0.0, finalSize.Width - 4.0);
                double height = Math.Max(0.0, finalSize.Height - 4.0);
                double x      = 4.0;
                double y      = (finalSize.Height - height) / 2.0;

                // Position it in the centre
                child.Arrange(new Rect(x, y, width, height));
            }

            // Return the size
            return finalSize;
        }

        #endregion

        #region --- Rendering ---

        /// <summary>
        /// Create the <see cref="System.Windows.Media.Geometry"/> required to render a rectangle with rounded corners.
        /// </summary>
        /// <param name="rect">The <see cref="System.Windows.Rect"/> that defines the size and position of the rectangle.</param>
        /// <param name="radius">The radius that determines the amount the corners are curved.</param>
        private static StreamGeometry CreateSimpleRoundRectangle( Rect rect, double radius )
        {
            StreamGeometry geometry = new StreamGeometry();

            // Open the geometry
            using (StreamGeometryContext context = geometry.Open())
            {
                Vector  vector = new Vector(rect.X, rect.Y);
                Point[] points = new Point[8] { new Point(radius, 0.0), new Point(rect.Width - radius, 0.0),
                                                new Point(rect.Width, radius), new Point(rect.Width, rect.Height - radius),
                                                new Point(rect.Width - radius, rect.Height), new Point(radius, rect.Height),
                                                new Point(0.0, rect.Height - radius),new Point(0.0, radius) };

                // Sanitise the positions
                if (points[0].X > points[1].X)
                {
                    points[0].X = points[1].X = (points[0].X + points[1].X) / 2.0;
                }
                if (points[2].Y > points[3].Y)
                {
                    points[2].Y = points[3].Y = (points[2].Y + points[3].Y) / 2.0;
                }
                if (points[4].X < points[5].X)
                {
                    points[4].X = points[5].X = (points[4].X + points[5].X) / 2.0;
                }
                if (points[6].Y < points[7].Y)
                {
                    points[6].Y = points[7].Y = (points[6].Y + points[7].Y) / 2.0;
                }

                // Offset the points
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] += vector;
                }

                // Work out the sizes for the arcs
                Size topRight    = new Size(rect.TopRight.X - points[1].X, points[2].Y - rect.TopRight.Y);
                Size bottomRight = new Size(rect.BottomRight.X - points[4].X, rect.BottomRight.Y - points[3].Y);
                Size bottomLeft  = new Size(points[5].X - rect.BottomLeft.X, rect.BottomLeft.Y - points[6].Y);
                Size topLeft     = new Size(points[0].X - rect.TopLeft.X, points[7].Y - rect.TopLeft.Y);

                // Now the rendering
                context.BeginFigure(points[0], true, true);
                context.LineTo(points[1], true, false);

                if (!topRight.IsZero())
                {
                    context.ArcTo(points[2], topRight, 0.0, false, SweepDirection.Clockwise, true, false);
                }
                context.LineTo(points[3], true, false);
                if (!bottomRight.IsZero())
                {
                    context.ArcTo(points[4], bottomRight, 0.0, false, SweepDirection.Clockwise, true, false);
                }
                context.LineTo(points[5], true, false);
                if (!bottomLeft.IsZero())
                {
                    context.ArcTo(points[6], bottomLeft, 0.0, false, SweepDirection.Clockwise, true, false);
                }
                context.LineTo(points[7], true, false);
                if (!topLeft.IsZero())
                {
                    context.ArcTo(points[0], topLeft, 0.0, false, SweepDirection.Clockwise, true, false);
                }
            }
            geometry.Freeze();

            return geometry;
        }

        /// <summary>
        /// Renders the chrome for the <see cref="CommandLink"/>.
        /// </summary>
        /// <param name="drawingContext">An instance of <see cref="System.Windows.Media.DrawingContext"/> used to render the control.</param>
        protected override void OnRender( DrawingContext drawingContext )
        {
            Rect  bounds            = new Rect(0.0, 0.0, ActualWidth, ActualHeight);
            Brush defaultBackground = Background;
            Brush overlayBackground = OverlayBackground;
            Pen   overlayBorder     = OverlayBorder;
            Pen   normalBorder      = NormalBorder;
            Pen   innerBorder       = InnerBorder;
            Rect  outerBounds       = bounds;
            Rect  innerBounds       = bounds;

            // If the bounds are valid - adjust them
            if ((bounds.Width > 0.0) && (bounds.Height > 0.0))
            {
                outerBounds = new Rect(bounds.Left + 0.5, bounds.Top + 0.5, bounds.Width - 1.0, bounds.Height - 1.0);
                innerBounds = new Rect(bounds.Left + 1.5, bounds.Top + 1.5, bounds.Width - 3.0, bounds.Height - 3.0);
            }

            // Define the geometry
            StreamGeometry outerGeometry = CreateSimpleRoundRectangle(outerBounds, 2.75);
            StreamGeometry innerGeometry = CreateSimpleRoundRectangle(innerBounds, 1.75);

            if (defaultBackground != null)
            {
                drawingContext.DrawGeometry(defaultBackground, null, outerGeometry);
            }

            if (overlayBackground != null)
            {
                drawingContext.DrawGeometry(overlayBackground, null, outerGeometry);
            }
 
            if ((bounds.Width > 4.0) && (bounds.Height > 4.0))
            {
                Brush leftDropShadow = this.LeftDropShadow;
                Brush topDropShadow  = this.TopDropShadow;

                drawingContext.PushClip(outerGeometry);

                if (leftDropShadow != null)
                {
                    drawingContext.DrawRectangle(leftDropShadow, null, new Rect(1.0, 1.0, 3.0, bounds.Bottom));
                }
                if (topDropShadow != null)
                {
                    drawingContext.DrawRectangle(topDropShadow, null, new Rect(1.0, 1.0, bounds.Right, 3.0));
                }

                drawingContext.Pop();
            }
                  
            if (IsEnabled && (normalBorder != null))
            {
                drawingContext.DrawGeometry(null, normalBorder, outerGeometry);
            }
            if (innerBorder != null)
            {
                drawingContext.DrawGeometry(null, innerBorder, innerGeometry);
            }
            if (overlayBorder != null)
            {
                drawingContext.DrawGeometry(null, overlayBorder, outerGeometry);
            }
        }

        #endregion

        #region --- Dependency Property Event Handlers ---  
    
        /// <summary>
        /// Static method to trigger the instance <see cref="OnRenderMouseOverChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="e">The additional information that describes the property change.</param>
        private static void OnRenderMouseOverChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs e )
        {
            CommandLinkChrome chrome = target as CommandLinkChrome;

            chrome.OnRenderMouseOverChanged((bool)e.OldValue, (bool)e.NewValue);
        }  
      
        /// <summary>
        /// Static method to trigger the instance <see cref="OnRenderPressedChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="e">The additional information that describes the property change.</param>
        private static void OnRenderPressedChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs e )
        {
            CommandLinkChrome chrome = target as CommandLinkChrome;

            chrome.OnRenderPressedChanged((bool)e.OldValue, (bool)e.NewValue);
        }
            
        /// <summary>
        /// Static method to trigger the instance <see cref="OnRenderEnabledChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="e">The additional information that describes the property change.</param>
        private static void OnRenderEnabledChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs e )
        {
            CommandLinkChrome chrome = target as CommandLinkChrome;

            chrome.OnRenderEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }
           
        /// <summary>
        /// Static method to trigger the instance <see cref="OnRenderDefaultedChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="e">The additional information that describes the property change.</param>
        private static void OnRenderDefaultedChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs e )
        {
            CommandLinkChrome chrome = target as CommandLinkChrome;

            chrome.OnRenderDefaultedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// The <see cref="RenderDefaulted"/> property has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnRenderDefaultedChanged( bool oldValue, bool newValue )
        {
            if (!RenderMouseOver && !RenderPressed)
            {
                if (newValue)
                {
                    ColorAnimation  animation       = new ColorAnimation(Color.FromArgb(0xff, 0x00, 0xcc, 0xff), new Duration(TimeSpan.FromSeconds(0.3)));
                    SolidColorBrush defaultedBrush = ((SolidColorBrush) OverlayBorder.Brush);

                    // Define the colour of the defaulted brush
                    defaultedBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

                    // Define the pulse animation
                    DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
                    timeline.KeyFrames.Add(new DiscreteDoubleKeyFrame(0.2, TimeSpan.FromSeconds(0.0)));
                    timeline.KeyFrames.Add(new LinearDoubleKeyFrame(0.8, TimeSpan.FromSeconds(1.0)));
                    timeline.KeyFrames.Add(new DiscreteDoubleKeyFrame(0.8, TimeSpan.FromSeconds(2.0)));
                    timeline.KeyFrames.Add(new LinearDoubleKeyFrame(0.2, TimeSpan.FromSeconds(3.0)));
                    timeline.RepeatBehavior = RepeatBehavior.Forever;
                    Timeline.SetDesiredFrameRate(timeline, 10);

                    OverlayBorder.Brush.BeginAnimation(Brush.OpacityProperty, timeline);
                }
                else
                {
                    OverlayBorder = null;
                }

                InvalidateVisual();
            }
        }

        /// <summary>
        /// The <see cref="RenderMouseOver"/> property has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnRenderMouseOverChanged( bool oldValue, bool newValue )
        {
            if (!RenderPressed)
            {
                if (newValue)
                {
                    Duration        duration            = new Duration(TimeSpan.FromSeconds(0.3));
                    DoubleAnimation doubleAnimation     = new DoubleAnimation(1.0, duration);
                    ColorAnimation  foregroundAnimation = new ColorAnimation(c_HoverForeground, duration);
                    ColorAnimation  borderAnimation     = new ColorAnimation(c_HoverBorder, duration);

                    // Show the overlay
                    OverlayBorder.Brush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnimation);
                    OverlayBorder.Brush.BeginAnimation(Brush.OpacityProperty, doubleAnimation);
                    OverlayBackground.BeginAnimation(Brush.OpacityProperty, doubleAnimation);
                    InnerBorder.Brush.BeginAnimation(Brush.OpacityProperty, doubleAnimation);

                    // Animate the foreground if we can
                    if (Foreground.IsFrozen)
                    {
                        Foreground = Foreground.Clone();
                    }
                    Foreground.BeginAnimation(SolidColorBrush.ColorProperty, foregroundAnimation);
                }
                else
                {
                    Duration        duration            = new Duration(TimeSpan.FromSeconds(0.3));
                    DoubleAnimation doubleAnimation     = new DoubleAnimation { Duration = duration };
                    ColorAnimation  foregroundAnimation = null;
                 
                    // Are we enabled or not?
                    if (IsEnabled)
                    {
                        foregroundAnimation = new ColorAnimation(c_DefaultForeground, duration);
                    }
                    else
                    {
                        foregroundAnimation = new ColorAnimation(c_DisabledForeground, duration);
                    }

                    // Hide the overlay
                    OverlayBorder.Brush.BeginAnimation(Brush.OpacityProperty, doubleAnimation);
                    OverlayBackground.BeginAnimation(Brush.OpacityProperty, doubleAnimation); 
                    InnerBorder.Brush.BeginAnimation(Brush.OpacityProperty, doubleAnimation);

                    // Animate the foreground if we can
                    if (Foreground.IsFrozen)
                    {
                        Foreground = Foreground.Clone();
                    }
                    Foreground.BeginAnimation(SolidColorBrush.ColorProperty, foregroundAnimation);

                    // Were we defaulted
                    if (RenderDefaulted)
                    {
                        double          opacity         = OverlayBackground.Opacity;
                        double          num2            = (1.0 - opacity) * 0.5;
                        ColorAnimation  animation       = new ColorAnimation(Color.FromArgb(0xff, 0x00, 0xcc, 0xff), new Duration(TimeSpan.FromSeconds(0.3)));
                        SolidColorBrush defaultedBrush  = ((SolidColorBrush) OverlayBorder.Brush);

                        // Define the colour of the defaulted brush
                        defaultedBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

                        // Do the glow
                        DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
                        timeline.KeyFrames.Add(new LinearDoubleKeyFrame(0.8, TimeSpan.FromSeconds(num2)));
                        timeline.KeyFrames.Add(new DiscreteDoubleKeyFrame(0.8, TimeSpan.FromSeconds(num2 + 0.5)));
                        timeline.KeyFrames.Add(new LinearDoubleKeyFrame(0.2, TimeSpan.FromSeconds(num2 + 2.0)));
                        timeline.KeyFrames.Add(new LinearDoubleKeyFrame(opacity, TimeSpan.FromSeconds(3.0)));
                        timeline.RepeatBehavior = RepeatBehavior.Forever;
                        Timeline.SetDesiredFrameRate(timeline, 10);

                        OverlayBorder.Brush.BeginAnimation(Brush.OpacityProperty, timeline);
                    }
                }

                InvalidateVisual();
            }
        }

 
        /// <summary>
        /// The <see cref="RenderEnabled"/> property has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnRenderEnabledChanged( bool oldValue, bool newValue )
        {
            if (newValue)
            {
                Duration        duration            = new Duration(TimeSpan.FromSeconds(0.1));
                DoubleAnimation doubleAnimation     = new DoubleAnimation { Duration = duration };
                ColorAnimation  foregroundAnimation = new ColorAnimation(c_DefaultForeground, duration);

                // Hide the overlay
                OverlayBorder.Brush.BeginAnimation(Brush.OpacityProperty, doubleAnimation);
                OverlayBackground.BeginAnimation(Brush.OpacityProperty, doubleAnimation); 
                InnerBorder.Brush.BeginAnimation(Brush.OpacityProperty, doubleAnimation);

                // Animate the foreground if we can
                if (Foreground.IsFrozen)
                {
                    Foreground = Foreground.Clone();
                }
                Foreground.BeginAnimation(SolidColorBrush.ColorProperty, foregroundAnimation);
            }
            else
            {
                Duration        duration            = new Duration(TimeSpan.FromSeconds(0.1));
                DoubleAnimation doubleAnimation     = new DoubleAnimation { Duration = duration };
                ColorAnimation  foregroundAnimation = new ColorAnimation(c_DisabledForeground, duration);

                // Hide the overlay
                OverlayBorder.Brush.BeginAnimation(Brush.OpacityProperty, doubleAnimation);
                OverlayBackground.BeginAnimation(Brush.OpacityProperty, doubleAnimation); 
                InnerBorder.Brush.BeginAnimation(Brush.OpacityProperty, doubleAnimation);

                // Animate the foreground if we can
                if (Foreground.IsFrozen)
                {
                    Foreground = Foreground.Clone();
                }
                Foreground.BeginAnimation(SolidColorBrush.ColorProperty, foregroundAnimation);
            }

            InvalidateVisual();

        }
 
        /// <summary>
        /// The <see cref="RenderPressed"/> property has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void OnRenderPressedChanged( bool oldValue, bool newValue )
        {
            if (newValue)
            {
                Duration        duration            = new Duration(TimeSpan.FromSeconds(0.1));
                DoubleAnimation fadeInAnimation     = new DoubleAnimation(1.0, duration);
                DoubleAnimation fadeOutAnimation    = new DoubleAnimation(0.0, duration);
                ColorAnimation  backgroundAnimation = new ColorAnimation(c_PressedBackground, duration);
                ColorAnimation  foregroundAnimation = new ColorAnimation(c_PressedForeground, duration);
                ColorAnimation  borderAnimation     = new ColorAnimation(c_PressedBorder, duration);

                // Fade in or fade out the key parts
                OverlayBackground.BeginAnimation(Brush.OpacityProperty, fadeInAnimation);
                OverlayBorder.Brush.BeginAnimation(Brush.OpacityProperty, fadeInAnimation);
                LeftDropShadow.BeginAnimation(Brush.OpacityProperty, fadeInAnimation);
                TopDropShadow.BeginAnimation(Brush.OpacityProperty, fadeInAnimation);
                InnerBorder.Brush.BeginAnimation(Brush.OpacityProperty, fadeOutAnimation);

                // Fade in the border
                ((SolidColorBrush)OverlayBorder.Brush).BeginAnimation(SolidColorBrush.ColorProperty, borderAnimation);

                // Set the target for the background
                foreach (GradientStop gradientStop in ((LinearGradientBrush) OverlayBackground).GradientStops)
                {
                    gradientStop.BeginAnimation(GradientStop.ColorProperty, backgroundAnimation);
                }
                
                // Animate the foreground if we can
                if (Foreground.IsFrozen)
                {
                    Foreground = Foreground.Clone();
                }
                Foreground.BeginAnimation(SolidColorBrush.ColorProperty, foregroundAnimation);
            }
            else
            {
                Duration        duration            = new Duration(TimeSpan.FromSeconds(0.1));
                DoubleAnimation fadeOutAnimation    = new DoubleAnimation { Duration = duration };
                DoubleAnimation fadeInAnimation     = new DoubleAnimation { Duration = duration, To = 1.0 };
                ColorAnimation  backgroundAnimation = new ColorAnimation { Duration = duration };
                ColorAnimation  foregroundAnimation = new ColorAnimation { Duration = duration };
                ColorAnimation  borderAnimation     = new ColorAnimation { Duration = duration };

                // Fade out the pressed details
                LeftDropShadow.BeginAnimation(Brush.OpacityProperty, fadeOutAnimation);
                TopDropShadow.BeginAnimation(Brush.OpacityProperty, fadeOutAnimation);
                
                // Fade in the border
                ((SolidColorBrush)OverlayBorder.Brush).BeginAnimation(SolidColorBrush.ColorProperty, borderAnimation);

                // If we are not rendering mouse over
                if (!RenderMouseOver)
                {
                    // Reset the opacity - make the ovelay transparent
                    OverlayBorder.Brush.BeginAnimation(Brush.OpacityProperty, fadeOutAnimation);
                    OverlayBackground.BeginAnimation(Brush.OpacityProperty, fadeOutAnimation);

                    // Are we enabled or not?
                    if (IsEnabled)
                    {
                        foregroundAnimation = new ColorAnimation { Duration = duration };
                    }
                    else
                    {
                        foregroundAnimation = new ColorAnimation(c_DisabledForeground, duration);
                    }
                }
                else
                {
                    // Render the hover foreground
                    foregroundAnimation = new ColorAnimation(c_HoverForeground, duration);

                    // Fade in the inner border
                    InnerBorder.Brush.BeginAnimation(Brush.OpacityProperty, fadeInAnimation);
                }

                // Animate the foreground
                if (Foreground.IsFrozen)
                {
                    Foreground = Foreground.Clone();
                }
                Foreground.BeginAnimation(SolidColorBrush.ColorProperty, foregroundAnimation);
               
                // Reset the border pen
                OverlayBorder.Brush.BeginAnimation(SolidColorBrush.ColorProperty, backgroundAnimation);

                // Reset the stops of the bckground brush
                foreach (GradientStop gradientStop in ((LinearGradientBrush) OverlayBackground).GradientStops)
                {
                    gradientStop.BeginAnimation(GradientStop.ColorProperty, backgroundAnimation);
                }

            }
        }
        
        #endregion

        #region --- Private Brushes and Pens ---  
    
        /// <summary>
        /// The is the pen used for the inner boarder
        /// </summary>
        private Pen InnerBorder
        { 
            get 
            { 
                if (m_InnerBorder == null)
                {
                    m_InnerBorder = new Pen { Thickness = 1.0, 
                                               Brush = new LinearGradientBrush { StartPoint = new Point(0.0, 0.0), EndPoint = new Point(0.0, 1.0), Opacity = 0.0,
                                                                                 GradientStops = { new GradientStop(Color.FromArgb(0x60, 0xff, 0xff, 0xff), 0.0), 
                                                                                                   new GradientStop(Color.FromArgb(0x60, 0xff, 0xff, 0xff), 1.0) }}};
                }
                
                return m_InnerBorder;
            }
        }
        
        /// <summary>
        /// The is the top drop shadow
        /// </summary>
        private LinearGradientBrush TopDropShadow
        { 
            get 
            { 
                if (m_TopDropShadow == null)
                {
                    m_TopDropShadow = new LinearGradientBrush { StartPoint = new Point(0.0, 0.0), EndPoint = new Point(0.0, 1.0), Opacity = 0.0,
                                                                GradientStops = { new GradientStop(Color.FromArgb(0x40, 0x33, 0x33, 0x33), 0.0), 
                                                                                  new GradientStop(Color.FromArgb(0x20, 0x33, 0x33, 0x33), 0.4), 
                                                                                  new GradientStop(Color.FromArgb(0x00, 0x33, 0x33, 0x33), 1.0) }};
                }
                
                return m_TopDropShadow;
            }
        }
       
        /// <summary>
        /// The is the left drop shadow
        /// </summary>
        private LinearGradientBrush LeftDropShadow
        { 
            get 
            { 
                if (m_LeftDropShadow == null)
                {
                    m_LeftDropShadow = new LinearGradientBrush { StartPoint = new Point(0.0, 0.0), EndPoint = new Point(1.0, 0.0), Opacity = 0.0,
                                                                 GradientStops = { new GradientStop(Color.FromArgb(0x40, 0x33, 0x33, 0x33), 0.0), 
                                                                                   new GradientStop(Color.FromArgb(0x20, 0x33, 0x33, 0x33), 0.4), 
                                                                                   new GradientStop(Color.FromArgb(0x00, 0x33, 0x33, 0x33), 1.0)  }};
                }
                
                return m_LeftDropShadow;
            }
        }

        /// <summary>
        /// The is the overlay background brush
        /// </summary>
        private LinearGradientBrush OverlayBackground
        { 
            get 
            { 
                // Create the brush if we had not already created it
                if (m_OverlayBackground == null)
                {
                    m_OverlayBackground = new LinearGradientBrush { StartPoint = new Point(0.0, 0.0), EndPoint = new Point(0.0, 1.0), Opacity = 0.0,
                                                                    GradientStops = { new GradientStop(Color.FromArgb(0xff, 0xff, 0xff, 0xff), 0.0), 
                                                                                      new GradientStop(Color.FromArgb(0x80, 0xf0, 0xf0, 0xf0), 0.5), 
                                                                                      new GradientStop(Color.FromArgb(0x40, 0xe0, 0xe0, 0xe0), 1.0) }};
                }

                // Determine the correct offsets to display a nice background
                double  height     = DesiredSize.Height;
                double  fraction   = (height < 40)? 0.2 : 20.0 / height;

                // Adjsut the stops so that we have a constant distance at top and bottom
                m_OverlayBackground.GradientStops[1].Offset = fraction;
                m_OverlayBackground.GradientStops[2].Offset = 1.0 - fraction;
                
                return m_OverlayBackground; 
            }
        }

        /// <summary>
        /// This is the olveray border brush
        /// </summary>
        private Pen OverlayBorder       
        { 
            get 
            { 
                if (m_OverlayBorder == null)
                {
                    m_OverlayBorder = new Pen { Thickness = 1.0, Brush = new SolidColorBrush(c_HoverBorder) { Opacity = 0.0 } };
                }
                return m_OverlayBorder; 
            }
            set { m_OverlayBorder = value; }
        }
        
        /// <summary>
        /// Obtain the border pen
        /// </summary>
        private Pen NormalBorder
        {
            get
            {
                Brush borderBrush = BorderBrush;

                // Do we have to create the pen
                if ((m_NormalBorder == null) || (m_NormalBorder.Brush != borderBrush))
                {
                    // Freeze the border brush if we can
                    if (!borderBrush.IsFrozen && borderBrush.CanFreeze)
                    {
                        borderBrush = borderBrush.Clone();
                        borderBrush.Freeze();
                    }

                    // Create the pen
                    m_NormalBorder = new Pen(borderBrush, 1.0);

                    // Freeze it
                    if (m_NormalBorder.CanFreeze)
                    {
                        m_NormalBorder.Freeze();
                    }
                }

                return m_NormalBorder;
            }
        }

        #endregion 

        #region --- Public Properties ---

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BrokenHouse.Windows.Controls.CommandLink"/>
        /// has the appearance of the default button on the form. This is a dependency property. 
        /// </summary>
        public bool RenderDefaulted
        {
            get { return (bool) GetValue(RenderDefaultedProperty); }
            set { SetValue(RenderDefaultedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BrokenHouse.Windows.Controls.CommandLink"/>
        /// appears as if it is disabled. This is a dependency property. 
        /// </summary>
        public bool RenderEnabled
        {
            get { return (bool) GetValue(RenderEnabledProperty); }
            set { SetValue(RenderEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BrokenHouse.Windows.Controls.CommandLink"/>
        /// appears as if the mouse is over it. This is a dependency property. 
        /// </summary>
        public bool RenderMouseOver
        {
            get { return (bool) GetValue(RenderMouseOverProperty); }
            set { SetValue(RenderMouseOverProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BrokenHouse.Windows.Controls.CommandLink"/>
        /// appears as if the mouse is pressed. This is a dependency property. 
        /// </summary>
        public bool RenderPressed
        {
            get { return (bool) GetValue(RenderPressedProperty); }
            set { SetValue(RenderPressedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush used to draw the outer border of the <see cref="BrokenHouse.Windows.Controls.CommandLink"/>.
        /// This is a dependency property. 
        /// </summary>
        public Brush BorderBrush
        {
            get { return (Brush) GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets thebrush used to fill the background of the <see cref="BrokenHouse.Windows.Controls.CommandLink"/>.
        /// This is a dependency property. 
        /// </summary>
        public Brush Background
        {
            get { return (Brush) GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets thebrush used to render the foreground elements of the <see cref="BrokenHouse.Windows.Controls.CommandLink"/>.
        /// This is a dependency property. 
        /// </summary>
        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush) GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        #endregion 
    }
}