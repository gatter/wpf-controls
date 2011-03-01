using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrokenHouse.Windows.Media.Imaging
{
    /// <summary>
    /// Modifies the colour of a <see cref="System.Windows.Media.Imaging.BitmapSource"/>.
    /// </summary>
    /// <remarks>
    /// This class only implements the 'safe' copy pixels functionality of a BitmapSource. If the unsafe
    /// CopyPixels method is called (i.e. the one that requries an IntPtr) then an exception will be
    /// thrown. In all cases where this class is intended to be used this is not a problem.
    /// </remarks>
    internal class ColorTransformedBitmap : FixedFormatBitmap
    {
        /// <summary>
        /// Identifies the <see cref="Transform"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(ColorTransform), typeof(ColorTransformedBitmap), new FrameworkPropertyMetadata(null, OnTransformChangedThunk), null);
                
        #region --- Freezable ---

        /// <summary>
        /// Create a new instance of the see cref="ColorTransformedBitmap"/>.
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new ColorTransformedBitmap();
        }

        #endregion
        
        #region --- Dependency Property change handlers ---
                
        /// <summary>
        /// The Transform has changed.
        /// </summary>
        /// <param name="oldValue">The old value of the Transform</param>
        /// <param name="newValue">The new value of the Transform</param>
        protected virtual void OnTransformChanged( ColorTransform oldValue, ColorTransform newValue )
        {
            // Trigger a property changed
            WritePreamble();
            WritePostscript();
        }
               
        /// <summary>
        /// Static method to trigger the instance <see cref="OnTransformChanged"/> property change event handler.
        /// </summary>
        /// <param name="target">The target of the property.</param>
        /// <param name="args">The additional information that describes the property change.</param>
        private static void OnTransformChangedThunk( DependencyObject target, DependencyPropertyChangedEventArgs args )
        {
            (target as ColorTransformedBitmap).OnTransformChanged((ColorTransform)args.OldValue, (ColorTransform)args.NewValue);
        }

        #endregion

        #region --- CopyPixels ---

        /// <summary>
        /// Transforms the pixels supplied by the <see cref="BrokenHouse.Windows.Media.Imaging.CustomBitmap.Source"/> 
        /// using the color transform defined in the <see cref="Transform"/> property.
        /// </summary>
        /// <param name="sourceRect">The source rectangle to copy. An Empty value specifies the entire bitmap.</param>
        /// <param name="array">The destination array.</param>
        /// <param name="stride">The stride of the bitmap.</param>
        /// <param name="offset">The pixel location where copying begins.</param>
        public override void CopyPixels(Int32Rect sourceRect, Array array, int stride, int offset)
        {
            // A bit of testing
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException("ModificatonBitmap only supports pixel arrays of rank 1");
            }
            if (array == null)
            {
                throw new ArgumentException("Modificatonbitmap only supports byte pixel arrays");
            }

            // Do the pixel copying
            base.CopyPixels(sourceRect, array, stride, offset);
            
            // Get the property because it is expensive
            ColorTransform     transform = Transform;

            // Do we do any processing
            if ((transform != null) && !transform.IsIdentity)
            {
                // Work out the start, end and width
                byte[] pixels = array as byte[];
                int    start  = offset + (sourceRect.X * 4) + (sourceRect.Y * stride);
                int    width  = sourceRect.Width * 4;
                int    end    = start + width + ((sourceRect.Height - 1) * stride);

                // Quick check
                if ((end > array.Length) || (width > stride))
                {
                    throw new ArgumentException("Invalid source rect supplied to TransformPixels");
                }

                // Loop over the pixels
                for (int i = start; i < end; i += stride)
                {
                    for (int j = 0; j < width; j += 4)
                    {
                        int   index = i + j;
                        Color color = Color.FromArgb(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]);

                        // Transform the color
                        Color result = transform.TransformColor(color);

                        // Update the pixel
                        pixels[index] = result.B;
                        pixels[index + 1] = result.G;
                        pixels[index + 2] = result.R;
                        pixels[index + 3] = result.A;
                    }
                }
            }
        }

        #endregion 
        
        #region --- Dependency Properties ---

        /// <summary>
        /// Gets or sets the <see cref="ColorTransform"/> that is applied to the underlying image. This is a dependency property. 
        /// </summary>
        public ColorTransform Transform
        {
            get { return (ColorTransform) base.GetValue(TransformProperty); }
            set { base.SetValue(TransformProperty, value); }
        }

        #endregion 

    }
}

