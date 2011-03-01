using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using BrokenHouse.Extensions;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Media.Imaging;
using BrokenHouse.Windows;

namespace BrokenHouse.Windows.Controls
{
    /// <summary>
    /// Allows icons (or in the general case bitmaps) to be rendered in the best way possible.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Rendering images whether they be based on a bitmap or vector graphics suffer from one big issue. The same image
    /// cannot be used to render small 16x16 images as well as large 48x48 images. At the moment (or until high DPI screens are
    /// common) it will always preferable to design different versions of the same image to be used at different sizes. As
    /// icons contain many different frames of the same basic image optimised for different sizes it is still very useful
    /// in providing images for user interfaces. This control will use the most appropriate icon frame based on
    /// the requested size of the control. If this size does not exactly match one of the frames contained in the icon then nearest larger size
    /// is used because as this will produce a better final image.
    /// </para>
    /// <para>
    /// One of the perceived problems of WPF is that you can not gaurentee that a bitmap will be rendered with a one to one mapping between
    /// bitmap pixel and device pixels, and as such images are neraly always fuzzy. Since .Net 3.5 SP1
    /// specifying <see cref="System.Windows.Media.BitmapScalingMode.NearestNeighbor"/> for the 
    /// <see cref="System.Windows.Media.RenderOptions.BitmapScalingModeProperty">BitmapScalingMode</see> attached property the bitmap 
    /// pixels will always be rendered to the nearest neighboring device pixels. Unfortunatly, simply specifying this property on the 
    /// standard <see cref="System.Windows.Controls.Image"/> can result in sub-pixels clipping around the edge of the bitmap. If the
    /// image needs to be scaled down then the resulting image is less than perfect due to how the Nearest Neighbor algorithm works.
    /// To eliminate these side effects this class will rescale the original image to the correct size and then adds a 1 pixel border.
    /// When this final image is rendered the is no resizing and any sub-pixel clipping occurs in the transparent border pixels 
    /// instead of the image. To turn on pixel snapping you have to set the <see cref="System.Windows.UIElement.SnapsToDevicePixels"/> 
    /// property to true.
    /// </para>
    /// <para>
    /// This class has a <see cref="ColorTransform"/> property that allows the colors of the pixels to be change before the image is
    /// rendered, for example, setting the property to a instance of the <see cref="DesaturationTransform"/> will cause the image
    /// to be rendered as a greyed imaged. This is usefull for indicating that an icon is disabled. In normal circumstances it would be
    /// preferable to use PixelShaders to perform this operation; however, the use of PixelShaders with the
    /// Nearest Neighbor algorithm introducing rendering artifacts.
    /// </para>
    /// <seealso cref="SnapDecorator"/>
    /// <seealso cref="ActiveIcon"/>
    /// </remarks>
    public class Icon : FrameworkElement
    {
        /// <summary>
        /// The margin that will be applied to the rendered bitmap
        /// </summary>
        private static Int32Thickness     RenderMargin { get; set; }

        /// <summary>
        /// The bitmap source holding the frame we are going to render
        /// </summary>
        private BitmapSource              SourceBitmap { get; set; }

        /// <summary>
        /// The bitmap source that will resize the source bitmap to the required size
        /// </summary>
        private BitmapSource              PreparedBitmap { get; set; }

        /// <summary>
        /// The image that we will be using to do the final rendering
        /// </summary>
        private BitmapSource              RenderBitmap { get; set; }

        /// <summary>
        /// All the frames that was in the original decoder
        /// </summary>
        private List<BitmapSource>        Frames { get; set; }
 
        /// <summary>
        /// Identifies the <see cref="ColorTransform"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ColorTransformProperty;
        
        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SourceProperty;
       
        /// <summary>
        /// Identifies the <see cref="Background"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty;

        #region --- Constructors ---

        /// <summary>
        /// Do the WPF Stuff
        /// </summary>
        static Icon()
        {
            Style defaultStyle = new Style(typeof(Icon), null);

            // The properties
            ColorTransformProperty = DependencyProperty.Register("ColorTransform", typeof(ColorTransform), typeof(Icon), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnColorTransformChangedThunk), null);
            SourceProperty         = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Icon), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnSourceChangedThunk, null), null);
            BackgroundProperty     = DependencyProperty.Register("Background", typeof(Brush), typeof(Icon), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender, null, null), null);
 
            // Define the default style
            defaultStyle.Setters.Add(new Setter(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight));
            defaultStyle.Seal();
            
            // Update the render options
            FrameworkElement.SnapsToDevicePixelsProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, OnSnapsToDevicePixelsChangedThunk));
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(defaultStyle));
        
            // Override the keyboard navigation
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(false));

            // Create the cache
            RenderMargin = new Int32Thickness(1);
        }

        #endregion

        #region --- Private Helpers ---

        /// <summary>
        /// Helper to define the render bitmap. This will be called every time the colour transform changes.
        /// </summary>
        private void UpdateRenderBitmap()
        {        
            // Create the render source
            if (PreparedBitmap != null)
            {
                ColorTransform colorTransform = ColorTransform;

                // If we have a colour transform then apply it
                if ((colorTransform != null) && !colorTransform.IsIdentity)
                {
                    ColorTransform clonedTransform = colorTransform.CloneCurrentValue() as ColorTransform;
                    RenderBitmap = new ColorTransformedBitmap { Source = PreparedBitmap, Transform = clonedTransform };
                }
                else
                {
                    RenderBitmap = PreparedBitmap;
                }
            }

            // Invalidate the visual
            InvalidateVisual();
        }

        #endregion

        #region --- Internal Overrides ---
       
        /// <summary>
        /// Computes the desired size required to render the icon.
        /// </summary>
        /// <remarks>
        /// The <paramref name="constraintSize"/> is used to determine the size of the image to render. Once the best frame
        /// of the icon is found the <see cref="System.Windows.UIElement.DesiredSize"/> can be determined maintaining the same
        /// aspect ratio of the frame.
        /// </remarks>
        /// <param name="constraintSize">The <see cref="System.Windows.Size"/> of the available area to render the icon.</param>
        /// <returns>The desired <see cref="System.Windows.Size"/> based on the <paramref name="constraintSize"/>
        /// and the frame to be rendered.</returns>
        [SecuritySafeCritical]
        protected override Size MeasureOverride( Size constraintSize )
        {
            Int32Size        bestSize     = new Int32Size(16, 16);
            Size             displaySize  = new Size();

            // Try to find the best size
            if ((Frames != null) && (Frames.Count > 0))
            {
                foreach (var size in Frames.Select(f => new Int32Size(f.PixelWidth, f.PixelHeight)).Distinct().OrderBy(s => s.Diagonal))
                {
                    // Assume this is going to be our size
                    bestSize = size;

                    // Is the constraint smaller than the measured size
                    if (constraintSize.Width.IsLessThanOrCloseTo(bestSize.Width) || constraintSize.Height.IsLessThanOrCloseTo(bestSize.Height))
                    {
                        // Found it
                        break;
                    }
                }
            }
            else
            {
                // If its not a bitmap source - it mouse be a drawingimage
                DrawingImage drawingImage = Source as DrawingImage;

                // Get the best size from the drawing image
                bestSize = (drawingImage == null)? new Int32Size(16, 16) : new Int32Size((int)drawingImage.Width, (int)drawingImage.Height);
            }

            // We need to calculate the measured size
            double bestAspect = (double)bestSize.Width / (double)bestSize.Height;

            if (double.IsInfinity(constraintSize.Width) && double.IsInfinity(constraintSize.Height))
            {
                displaySize = new Size(bestSize.Width, bestSize.Height);
            }
            else if (double.IsInfinity(constraintSize.Height))
            {
                displaySize = new Size(constraintSize.Width, constraintSize.Width / bestAspect);
            }
            else if (double.IsInfinity(constraintSize.Width))
            {
                displaySize = new Size(constraintSize.Height * bestAspect, constraintSize.Height);
            }
            else
            {
                double constraintAspect = constraintSize.Width / constraintSize.Height;
            
                if (bestAspect < constraintAspect)
                {
                    displaySize = new Size(constraintSize.Height * bestAspect, constraintSize.Height);
                }
                else 
                {
                    displaySize = new Size(constraintSize.Width, constraintSize.Width / bestAspect);
                }
            }

            // Do we have to obtain a new image
            if ((RenderBitmap == null) || (displaySize.Width != PreparedBitmap.PixelWidth) || (displaySize.Height != PreparedBitmap.PixelHeight))
            {
                // Ok now find the best frame
                if ((Frames != null) && (Frames.Count > 0))
                {
                    // Get the best frame
                    SourceBitmap = Frames.Where(f => (f.PixelWidth == bestSize.Width) && (f.PixelHeight == bestSize.Height))
                                                      .OrderBy(f => f.Format.BitsPerPixel).First();
     
                    // Only need to compare one size because the display size must be the same aspect
                    if (displaySize.Width != bestSize.Width)
                    {
                        double scale = (double)displaySize.Width / (double)bestSize.Width;

                        SourceBitmap = new TransformedBitmap(SourceBitmap, new ScaleTransform(scale, scale));
                    }
                }
                else
                {
                    // We need to render into the bitmap
                    RenderTargetBitmap renderBitmap   = new RenderTargetBitmap((int)displaySize.Width, (int)displaySize.Height, 96.0, 96.0, PixelFormats.Pbgra32);

                    // Only attempt to draw if we have a valid image
                    if (Source != null)
                    {
                        DrawingVisual      drawingVisual  = new DrawingVisual();
                        DrawingContext     drawingContext = drawingVisual.RenderOpen();

                        // Draw the image at a scale of 1:1
                        drawingContext.DrawImage(Source, new Rect(0, 0, displaySize.Width, displaySize.Height));
                        drawingContext.Close();

                        // Render into the best size
                        renderBitmap.Render(drawingVisual);
                    }

                    // We have our source bitmap
                    SourceBitmap = renderBitmap;
                }

                // Obtain the best size
                PreparedBitmap = new ExpandedBitmap { Source = SourceBitmap, Margin = RenderMargin };

                // Update the render bitmap
                UpdateRenderBitmap();
            }
   
            // Return the size that we want
            return displaySize;
        }
        
        /// <summary>
        /// Renders the best frame of the icon.
        /// </summary>
        /// <param name="drawingContext">An instance of <see cref="System.Windows.Media.DrawingContext"/> used to render the frame.</param>
        [SecuritySafeCritical]
        protected override void OnRender( DrawingContext drawingContext )
        {
            // Draw the background
            drawingContext.DrawRectangle(Background, null,  new Rect(new Point(0, 0), RenderSize));

            // Do we have a dislay bitmap
            if (RenderBitmap != null)
            {
                Rect renderRectangle  = new Rect(-RenderMargin.Left, -RenderMargin.Top, RenderBitmap.PixelWidth, RenderBitmap.PixelHeight);

                drawingContext.DrawImage(RenderBitmap, renderRectangle);
            }
        }
         
        #endregion
        
        #region --- Dependency Property change handlers ---
        
        /// <summary>
        /// The <see cref="System.Windows.UIElement.SnapsToDevicePixels"/> property change handler.
        /// </summary>
        /// <summary>
        /// If we are snapping to pixels then we must enable NearestNeighbor bitmap scaling.
        /// </summary>
        /// <param name="oldValue">The old value of the property</param>
        /// <param name="newValue">The new value of the property</param>
        protected virtual void OnSnapsToDevicePixelsChanged( bool oldValue, bool newValue )
        {
            // Update the scaling mode
            RenderOptions.SetBitmapScalingMode(this, newValue? BitmapScalingMode.NearestNeighbor : BitmapScalingMode.Unspecified);

            // Invalidate the visual
            InvalidateVisual();
        }
        
        /// <summary>
        /// The <see cref="ColorTransform"/> property change handler.
        /// </summary>
        /// <param name="oldValue">The old value of the property</param>
        /// <param name="newValue">The new value of the property</param>
        protected virtual void OnColorTransformChanged( ColorTransform oldValue, ColorTransform newValue )
        {
            UpdateRenderBitmap();

            if ((newValue != null) && !newValue.IsFrozen)
            {
                newValue.Changed += OnColorTransformPropertyChanged;
            }
            if ((oldValue != null) && !oldValue.IsFrozen)
            {
                oldValue.Changed -= OnColorTransformPropertyChanged;
            }
        }

        /// <summary>
        /// A property of the color transform has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnColorTransformPropertyChanged( object sender, EventArgs e )
        {
            UpdateRenderBitmap();
        }
        
        /// <summary>
        /// The <see cref="Source"/> property changed handler.
        /// </summary>
        /// <remarks>
        /// Determine if the new source has multiple frames and if so 
        /// obtain the decoder that gives us access to the other frames. If its not a <see cref="System.Windows.Media.Imaging.BitmapFrame"/>
        /// then it is a single image and we will use the image source directly.
        /// </remarks>
        /// <param name="oldValue">The old value of the property</param>
        /// <param name="newValue">The new value of the property</param>
        protected virtual void OnSourceChanged( ImageSource oldValue, ImageSource newValue )
        {
             // Do we have a new value
            if (newValue != null)
            {
                BitmapSource      bitmapSource = (newValue as BitmapSource);
                BitmapFrame       bitmapFrame  = (newValue as BitmapFrame);
                CustomBitmapFrame customFrame  = (newValue as CustomBitmapFrame);

                // Was we supplied a bitmap
                if (bitmapFrame != null)
                {
                    Frames = bitmapFrame.Decoder.Frames.OfType<BitmapSource>().ToList();
                }
                else if (customFrame != null)
                {
                    Frames = customFrame.Frames.ToList();
                }
                else if (bitmapSource != null)
                {
                    Frames = new List<BitmapSource> { bitmapSource };
                }
                else
                {
                    Frames = null;
                }
            }

            // Invalidate the object
            InvalidateMeasure();
            InvalidateVisual();
        }
      
        /// <summary>
        /// Static method to trigger the instance <see cref="OnSnapsToDevicePixelsChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnSnapsToDevicePixelsChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as Icon).OnSnapsToDevicePixelsChanged((bool)args.OldValue, (bool)args.NewValue);
        }
     
        /// <summary>
        /// Static method to trigger the instance <see cref="OnColorTransformChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnColorTransformChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as Icon).OnColorTransformChanged((ColorTransform)args.OldValue, (ColorTransform)args.NewValue);
        }
     
        /// <summary>
        /// Static method to trigger the instance <see cref="OnSourceChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnSourceChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as Icon).OnSourceChanged(args.OldValue as ImageSource, args.NewValue as ImageSource);
        }

        #endregion

        #region --- Properties ---
        
        /// <summary>
        /// Gets or sets the <see cref="ColorTransform"/> used to modify the colours of the bitmap before
        /// it is rendered. This is a dependency property. 
        /// </summary>
        public ColorTransform ColorTransform
        {
            get { return (ColorTransform) base.GetValue(ColorTransformProperty); }
            set { base.SetValue(ColorTransformProperty, value); }
        }   
        
        /// <summary>
        /// Gets or sets the source for the bitmap. This is a dependency property. 
        /// </summary>
        public ImageSource Source
        {
            get { return (ImageSource) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the background for the bitmap. This is a dependency property. 
        /// </summary>
        public Brush Background
        {
            get { return (Brush) GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        
        #endregion
    }
}

